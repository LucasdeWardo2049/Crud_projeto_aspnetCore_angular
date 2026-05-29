import { Component, OnInit, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';

import {
  CreateScheduleRequest,
  SCHEDULE_STATUS_OPTIONS,
  ScheduleStatus
} from '../../models/schedule.model';
import { ScheduleService } from '../../services/schedule.service';

@Component({
  selector: 'app-schedule-form',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './schedule-form.component.html',
  styleUrl: './schedule-form.component.css'
})
export class ScheduleFormComponent implements OnInit {
  private readonly formBuilder = inject(FormBuilder);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly scheduleService = inject(ScheduleService);

  statusOptions = SCHEDULE_STATUS_OPTIONS;

  isEditMode = false;
  scheduleId: number | null = null;
  loading = false;
  errorMessage = '';

  form = this.formBuilder.nonNullable.group({
    employeeName: ['', Validators.required],
    employeeRegistration: ['', Validators.required],
    department: ['', Validators.required],
    shiftName: ['', Validators.required],
    startTime: ['', Validators.required],
    endTime: ['', Validators.required],
    workDate: ['', Validators.required],
    status: ['Scheduled' as ScheduleStatus, Validators.required],
    notes: ['']
  });

  ngOnInit(): void {
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

    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const request = this.buildRequest();

    if (request.startTime === request.endTime) {
      this.errorMessage = 'O horario final nao pode ser igual ao horario inicial.';
      return;
    }

    this.loading = true;

    if (this.isEditMode && this.scheduleId !== null) {
      this.scheduleService.update(this.scheduleId, request).subscribe({
        next: () => {
          this.loading = false;
          this.router.navigate(['/schedules']);
        },
        error: (error) => {
          this.loading = false;
          this.errorMessage = error?.error?.message ?? 'Erro ao atualizar schedule.';
        }
      });

      return;
    }

    this.scheduleService.create(request).subscribe({
      next: () => {
        this.loading = false;
        this.router.navigate(['/schedules']);
      },
      error: (error) => {
        this.loading = false;
        this.errorMessage = error?.error?.message ?? 'Erro ao criar schedule.';
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
          employeeName: schedule.employeeName,
          employeeRegistration: schedule.employeeRegistration,
          department: schedule.department,
          shiftName: schedule.shiftName,
          startTime: this.toHtmlTime(schedule.startTime),
          endTime: this.toHtmlTime(schedule.endTime),
          workDate: schedule.workDate,
          status: schedule.status,
          notes: schedule.notes ?? ''
        });

        this.loading = false;
      },
      error: () => {
        this.errorMessage = 'Erro ao carregar schedule.';
        this.loading = false;
      }
    });
  }

  private buildRequest(): CreateScheduleRequest {
    const value = this.form.getRawValue();

    return {
      employeeName: value.employeeName.trim(),
      employeeRegistration: value.employeeRegistration.trim(),
      department: value.department.trim(),
      shiftName: value.shiftName.trim(),
      startTime: this.normalizeTime(value.startTime),
      endTime: this.normalizeTime(value.endTime),
      workDate: value.workDate,
      status: value.status,
      notes: value.notes.trim() ? value.notes.trim() : null
    };
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
}
