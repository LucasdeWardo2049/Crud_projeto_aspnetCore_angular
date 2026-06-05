import { AfterViewInit, Component, ElementRef, OnDestroy, ViewChild, inject } from '@angular/core';
import { PieChart } from 'echarts/charts';
import { LegendComponent, TooltipComponent } from 'echarts/components';
import * as echarts from 'echarts/core';
import { CanvasRenderer } from 'echarts/renderers';
import type { ECharts, EChartsOption } from 'echarts';
import type { CallbackDataParams } from 'echarts/types/dist/shared';
import { forkJoin } from 'rxjs';

import { Schedule, SCHEDULE_STATUS_OPTIONS, ScheduleStatus } from '../../../schedules/models/schedule.model';
import { ScheduleService } from '../../../schedules/services/schedule.service';
import { Task } from '../../../tasks/models/task.model';
import { TaskService } from '../../../tasks/services/task.service';

interface PieItem {
  name: string;
  value: number;
}

interface DepartmentEmployeeSummary {
  employeeId: number;
  employeeName: string;
  taskCount: number;
}

echarts.use([PieChart, TooltipComponent, LegendComponent, CanvasRenderer]);

@Component({
  selector: 'app-dashboard',
  imports: [],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css'
})
export class DashboardComponent implements AfterViewInit, OnDestroy {
  @ViewChild('schedulesChart') schedulesChartRef?: ElementRef<HTMLDivElement>;
  @ViewChild('tasksChart') tasksChartRef?: ElementRef<HTMLDivElement>;

  private readonly scheduleService = inject(ScheduleService);
  private readonly taskService = inject(TaskService);
  private schedulesChart?: ECharts;
  private tasksChart?: ECharts;

  totalSchedules = 0;
  totalTasks = 0;
  loading = false;
  errorMessage = '';
  selectedDepartment = '';
  selectedDepartmentEmployees: DepartmentEmployeeSummary[] = [];

  private currentTasks: Task[] = [];

  private readonly resizeCharts = () => {
    this.schedulesChart?.resize();
    this.tasksChart?.resize();
  };

  ngAfterViewInit(): void {
    window.addEventListener('resize', this.resizeCharts);
    this.loadDashboard();
  }

  ngOnDestroy(): void {
    window.removeEventListener('resize', this.resizeCharts);
    this.schedulesChart?.dispose();
    this.tasksChart?.dispose();
  }

  private loadDashboard(): void {
    this.loading = true;
    this.errorMessage = '';

    forkJoin({
      schedules: this.scheduleService.getAll(),
      tasks: this.taskService.getAll()
    }).subscribe({
      next: ({ schedules, tasks }) => {
        this.totalSchedules = schedules.length;
        this.totalTasks = tasks.length;
        this.currentTasks = tasks;
        this.selectedDepartment = '';
        this.selectedDepartmentEmployees = [];
        this.renderCharts(schedules, tasks);
        this.loading = false;
      },
      error: () => {
        this.errorMessage = 'Erro ao carregar dashboard.';
        this.loading = false;
      }
    });
  }

  private renderCharts(schedules: Schedule[], tasks: Task[]): void {
    if (!this.schedulesChartRef || !this.tasksChartRef) {
      return;
    }

    this.schedulesChart ??= echarts.init(this.schedulesChartRef.nativeElement);
    this.tasksChart ??= echarts.init(this.tasksChartRef.nativeElement);

    this.schedulesChart.setOption(this.createHalfPieOption(
      'Schedules por status',
      this.buildScheduleStatusData(schedules)
    ));

    this.tasksChart.setOption(this.createDoughnutPieOption(
      'Tasks por departamento',
      this.buildTaskDepartmentData(tasks)
    ));

    this.tasksChart.off('click');
    this.tasksChart.on('click', (params: CallbackDataParams) => {
      if (typeof params.name !== 'string' || params.name === 'Sem dados') {
        return;
      }

      this.selectDepartment(params.name);
    });
  }

  clearDepartmentSelection(): void {
    this.selectedDepartment = '';
    this.selectedDepartmentEmployees = [];
  }

  private buildScheduleStatusData(schedules: Schedule[]): PieItem[] {
    const counts = new Map<ScheduleStatus, number>(
      SCHEDULE_STATUS_OPTIONS.map(status => [status, 0])
    );

    for (const schedule of schedules) {
      counts.set(schedule.status, (counts.get(schedule.status) ?? 0) + 1);
    }

    return this.withFallback(
      SCHEDULE_STATUS_OPTIONS.map(status => ({
        name: status,
        value: counts.get(status) ?? 0
      }))
    );
  }

  private buildTaskDepartmentData(tasks: Task[]): PieItem[] {
    const counts = new Map<string, number>();

    for (const task of tasks) {
      const department = task.department || 'Sem departamento';
      counts.set(department, (counts.get(department) ?? 0) + 1);
    }

    return this.withFallback(
      Array.from(counts.entries())
        .sort(([firstDepartment], [secondDepartment]) =>
          firstDepartment.localeCompare(secondDepartment)
        )
        .map(([name, value]) => ({ name, value }))
    );
  }

  private selectDepartment(department: string): void {
    const counts = new Map<number, DepartmentEmployeeSummary>();

    for (const task of this.currentTasks) {
      if ((task.department || 'Sem departamento') !== department) {
        continue;
      }

      const current = counts.get(task.employeeId);

      if (current) {
        current.taskCount += 1;
        continue;
      }

      counts.set(task.employeeId, {
        employeeId: task.employeeId,
        employeeName: task.employeeName || `Employee #${task.employeeId}`,
        taskCount: 1
      });
    }

    this.selectedDepartment = department;
    this.selectedDepartmentEmployees = Array.from(counts.values())
      .sort((firstEmployee, secondEmployee) =>
        firstEmployee.employeeName.localeCompare(secondEmployee.employeeName)
      );
  }

  private withFallback(data: PieItem[]): PieItem[] {
    if (data.some(item => item.value > 0)) {
      return data;
    }

    return [{ name: 'Sem dados', value: 1 }];
  }

  private createHalfPieOption(title: string, data: PieItem[]): EChartsOption {
    return {
      color: ['#4CC9F0', '#80ED99', '#FFD166', '#FF6B6B', '#D4D4D5'],
      textStyle: {
        color: '#F4F4F5'
      },
      tooltip: {
        trigger: 'item',
        backgroundColor: '#363638',
        borderColor: '#68686A',
        textStyle: {
          color: '#F4F4F5'
        }
      },
      legend: {
        top: '5%',
        left: 'center',
        textStyle: {
          color: '#D4D4D5'
        }
      },
      series: [
        {
          name: title,
          type: 'pie',
          radius: ['40%', '70%'],
          center: ['50%', '70%'],
          startAngle: 180,
          endAngle: 360,
          label: {
            color: '#FFFFFF',
            fontSize: 13
          },
          labelLine: {
            lineStyle: {
              width: 1.5
            }
          },
          data
        }
      ]
    };
  }

  private createDoughnutPieOption(title: string, data: PieItem[]): EChartsOption {
    return {
      color: ['#4CC9F0', '#80ED99', '#FFD166', '#FF6B6B', '#C77DFF', '#F4A261', '#D4D4D5'],
      textStyle: {
        color: '#F4F4F5'
      },
      tooltip: {
        trigger: 'item',
        backgroundColor: '#363638',
        borderColor: '#68686A',
        textStyle: {
          color: '#F4F4F5'
        }
      },
      legend: {
        top: '5%',
        left: 'center',
        textStyle: {
          color: '#D4D4D5'
        }
      },
      series: [
        {
          name: title,
          type: 'pie',
          radius: ['40%', '70%'],
          avoidLabelOverlap: false,
          label: {
            show: false,
            position: 'center'
          },
          labelLine: {
            show: false
          },
          data
        }
      ]
    };
  }
}
