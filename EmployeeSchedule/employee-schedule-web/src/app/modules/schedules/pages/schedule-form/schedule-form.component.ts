import { Component, OnInit, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatRadioModule } from '@angular/material/radio';
import { MatSelectModule } from '@angular/material/select';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { Observable, map, of, switchMap } from 'rxjs';
import { CreateEmployeeRequest, Employee } from '../../../employees/models/employee.model';
import { EmployeeService } from '../../../employees/services/employee.service';
import { CreateTaskRequest, Task } from '../../../tasks/models/task.model';
import { TaskService } from '../../../tasks/services/task.service';

import {
  CreateScheduleRequest,
  SCHEDULE_STATUS_OPTIONS,
  Schedule,
  ScheduleStatus
} from '../../models/schedule.model';
import { ScheduleService } from '../../services/schedule.service';

type EmployeeFormMode = 'existing' | 'new';

@Component({
  selector: 'app-schedule-form',
  imports: [
    ReactiveFormsModule,
    RouterLink,
    MatButtonModule,
    MatExpansionModule,
    MatFormFieldModule,
    MatInputModule,
    MatRadioModule,
    MatSelectModule
  ],
  templateUrl: './schedule-form.component.html',
  styleUrl: './schedule-form.component.css'
})
export class ScheduleFormComponent implements OnInit {
  private readonly formBuilder = inject(FormBuilder);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly scheduleService = inject(ScheduleService);
  private readonly employeeService = inject(EmployeeService);
  private readonly taskService = inject(TaskService);

  employees: Employee[] = [];
  schedules: Schedule[] = [];
  tasks: Task[] = [];
  statusOptions = SCHEDULE_STATUS_OPTIONS;

  isEditMode = false;
  scheduleId: number | null = null;
  loading = false;
  errorMessage = '';
  submitted = false;

  form = this.formBuilder.nonNullable.group({
    employeeMode: ['existing' as EmployeeFormMode, Validators.required],
    employeeId: [0],
    newEmployeeName: [''],
    newEmployeeRegistration: [''],
    newEmployeeDepartment: [''],
    shiftName: ['', Validators.required],
    startTime: ['', Validators.required],
    endTime: ['', Validators.required],
    workDate: ['', Validators.required],
    status: ['Scheduled' as ScheduleStatus, Validators.required],
    notes: [''],
    taskTitle: [''],
    taskDescription: ['']
  });

  get selectedEmployee(): Employee | undefined {
    if (this.usesNewEmployee) {
      return undefined;
    }

    const employeeId = Number(this.form.controls.employeeId.value);

    return this.employees.find(employee => employee.id === employeeId);
  }

  get usesNewEmployee(): boolean {
    return this.form.controls.employeeMode.value === 'new';
  }

  get selectedEmployeeTaskCount(): number {
    const employeeId = Number(this.form.controls.employeeId.value);

    return this.tasks.filter(task => task.employeeId === employeeId).length;
  }

  get selectedEmployeeFutureScheduleCount(): number {
    const employeeId = Number(this.form.controls.employeeId.value);
    const today = this.toDateOnlyString(new Date());

    return this.schedules.filter(schedule =>
      schedule.employeeId === employeeId &&
      schedule.workDate >= today &&
      schedule.id !== this.scheduleId
    ).length;
  }

  ngOnInit(): void {
    this.loadEmployees();
    this.loadSchedules();
    this.loadTasks();

    const idParam = this.route.snapshot.paramMap.get('id');

    if (!idParam) {
      return;
    }

    this.isEditMode = true;
    this.scheduleId = Number(idParam);

    if (Number.isNaN(this.scheduleId)) {
      this.errorMessage = 'ID invalido.';
      return;
    }

    this.loadSchedule(this.scheduleId);
  }

  onSubmit(): void {
    this.errorMessage = '';
    this.submitted = true;

    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const employeeValidationMessage = this.validateEmployeeSelection();

    if (employeeValidationMessage) {
      this.form.markAllAsTouched();
      this.errorMessage = employeeValidationMessage;
      return;
    }

    const value = this.form.getRawValue();
    const startTime = this.normalizeTime(value.startTime);
    const endTime = this.normalizeTime(value.endTime);

    if (startTime === endTime) {
      this.errorMessage = 'O horario final nao pode ser igual ao horario inicial.';
      return;
    }

    this.loading = true;

    this.resolveEmployeeId()
      .pipe(
        switchMap(employeeId => {
          const request = this.buildRequest(employeeId);
          const saveSchedule$ = this.isEditMode && this.scheduleId !== null
            ? this.scheduleService.update(this.scheduleId, request)
            : this.scheduleService.create(request);

          return saveSchedule$.pipe(switchMap(() => this.createTaskIfFilled(employeeId)));
        })
      )
      .subscribe({
        next: () => {
          this.loading = false;
          this.router.navigate(['/schedules']);
        },
        error: (error) => {
          this.loading = false;
          this.errorMessage = error?.error?.message ?? 'Erro ao salvar schedule.';
        }
      });
  }

  isInvalid(controlName: string): boolean {
    const control = this.form.get(controlName);

    if (!control) {
      return false;
    }

    return control.invalid && (control.dirty || control.touched);
  }

  private loadSchedule(id: number): void {
    this.loading = true;

    this.scheduleService.getById(id).subscribe({
      next: (schedule) => {
        this.form.patchValue({
          employeeMode: 'existing',
          employeeId: schedule.employeeId,
          newEmployeeName: '',
          newEmployeeRegistration: '',
          newEmployeeDepartment: '',
          shiftName: schedule.shiftName,
          startTime: this.toHtmlTime(schedule.startTime),
          endTime: this.toHtmlTime(schedule.endTime),
          workDate: schedule.workDate,
          status: schedule.status,
          notes: schedule.notes ?? '',
          taskTitle: '',
          taskDescription: ''
        });

        this.loading = false;
      },
      error: () => {
        this.errorMessage = 'Erro ao carregar schedule.';
        this.loading = false;
      }
    });
  }

  private loadEmployees(): void {
    this.employeeService.getAll().subscribe({
      next: (employees) => {
        this.employees = employees;
      },
      error: () => {
        this.errorMessage = 'Erro ao carregar employees.';
      }
    });
  }

  private loadSchedules(): void {
    this.scheduleService.getAll().subscribe({
      next: (schedules) => {
        this.schedules = schedules;
      },
      error: () => {
        this.errorMessage = 'Erro ao carregar schedules.';
      }
    });
  }

  private loadTasks(): void {
    this.taskService.getAll().subscribe({
      next: (tasks) => {
        this.tasks = tasks;
      },
      error: () => {
        this.errorMessage = 'Erro ao carregar tasks.';
      }
    });
  }

  isExistingEmployeeInvalid(): boolean {
    return !this.usesNewEmployee &&
      Number(this.form.controls.employeeId.value) <= 0 &&
      (this.submitted || this.form.controls.employeeId.touched || this.form.controls.employeeId.dirty);
  }

  isNewEmployeeFieldInvalid(controlName: 'newEmployeeName' | 'newEmployeeRegistration' | 'newEmployeeDepartment'): boolean {
    const control = this.form.controls[controlName];

    return this.usesNewEmployee &&
      !control.value.trim() &&
      (this.submitted || control.touched || control.dirty);
  }

  private buildRequest(employeeId: number): CreateScheduleRequest {
    const value = this.form.getRawValue();

    return {
      employeeId,
      shiftName: value.shiftName.trim(),
      startTime: this.normalizeTime(value.startTime),
      endTime: this.normalizeTime(value.endTime),
      workDate: value.workDate,
      status: value.status,
      notes: value.notes.trim() ? value.notes.trim() : null
    };
  }

  private buildEmployeeRequest(): CreateEmployeeRequest | null {
    const value = this.form.getRawValue();
    const name = value.newEmployeeName.trim();
    const registration = value.newEmployeeRegistration.trim();
    const department = value.newEmployeeDepartment.trim();

    if (!name || !registration || !department) {
      return null;
    }

    return {
      name,
      registration,
      department,
      isActive: true
    };
  }

  private resolveEmployeeId(): Observable<number> {
    if (!this.usesNewEmployee) {
      return of(Number(this.form.controls.employeeId.value));
    }

    const request = this.buildEmployeeRequest();

    if (!request) {
      return of(0);
    }

    return this.employeeService.create(request).pipe(map(employee => employee.id));
  }

  private validateEmployeeSelection(): string {
    if (!this.usesNewEmployee && Number(this.form.controls.employeeId.value) <= 0) {
      return 'Selecione um funcionario.';
    }

    if (this.usesNewEmployee && !this.buildEmployeeRequest()) {
      return 'Preencha nome, matricula e departamento do novo funcionario.';
    }

    return '';
  }

  private buildTaskRequest(employeeId: number): CreateTaskRequest | null {
    const value = this.form.getRawValue();
    const title = value.taskTitle.trim();

    if (!title) {
      return null;
    }

    return {
      employeeId,
      title,
      description: value.taskDescription.trim() ? value.taskDescription.trim() : null
    };
  }

  private createTaskIfFilled(employeeId: number): Observable<Task | null> {
    const request = this.buildTaskRequest(employeeId);

    if (!request) {
      return of(null);
    }

    return this.taskService.create(request);
  }

  private normalizeTime(value: string): string {
    if (value.length === 5) {
      return `${value}:00`;
    }

    return value;
  }

  private toHtmlTime(value: string): string {
    if (!value) {
      return '';
    }

    return value.slice(0, 5);
  }

  private toDateOnlyString(value: Date): string {
    const year = value.getFullYear();
    const month = String(value.getMonth() + 1).padStart(2, '0');
    const day = String(value.getDate()).padStart(2, '0');

    return `${year}-${month}-${day}`;
  }
}
