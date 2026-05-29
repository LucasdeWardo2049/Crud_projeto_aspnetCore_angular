import { Component, OnInit, inject } from '@angular/core';
import { RouterLink } from '@angular/router';

import { Schedule } from '../../models/schedule.model';
import { ScheduleService } from '../../services/schedule.service';

@Component({
  selector: 'app-schedule-list',
  imports: [RouterLink],
  templateUrl: './schedule-list.component.html',
  styleUrl: './schedule-list.component.css'
})
export class ScheduleListComponent implements OnInit {
  private readonly scheduleService = inject(ScheduleService);

  schedules: Schedule[] = [];
  loading = false;
  errorMessage = '';

  ngOnInit(): void {
    this.loadSchedules();
  }

  loadSchedules(): void {
    this.loading = true;
    this.errorMessage = '';

    this.scheduleService.getAll().subscribe({
      next: (schedules) => {
        this.schedules = schedules;
        this.loading = false;
      },
      error: () => {
        this.errorMessage = 'Erro ao carregar schedules.';
        this.loading = false;
      }
    });
  }

  deleteSchedule(schedule: Schedule): void {
    const confirmed = confirm(`Deseja excluir a escala de ${schedule.employeeName}?`);

    if (!confirmed) {
      return;
    }

    this.scheduleService.delete(schedule.id).subscribe({
      next: () => {
        this.schedules = this.schedules.filter(item => item.id !== schedule.id);
      },
      error: () => {
        this.errorMessage = 'Erro ao excluir schedule.';
      }
    });
  }
}
