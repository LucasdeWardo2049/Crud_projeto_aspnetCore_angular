export type ScheduleStatus = 'Scheduled' | 'Completed' | 'Cancelled' | 'Absent';

export const SCHEDULE_STATUS_OPTIONS: readonly ScheduleStatus[] = [
  'Scheduled',
  'Completed',
  'Cancelled',
  'Absent'
];

export interface Schedule {
  id: number;
  employeeName: string;
  employeeRegistration: string;
  department: string;
  shiftName: string;
  startTime: string;
  endTime: string;
  workDate: string;
  status: ScheduleStatus;
  notes?: string | null;
  createdAt: string;
  updatedAt?: string | null;
}

export interface CreateScheduleRequest {
  employeeName: string;
  employeeRegistration: string;
  department: string;
  shiftName: string;
  startTime: string;
  endTime: string;
  workDate: string;
  status: ScheduleStatus;
  notes?: string | null;
}

export type UpdateScheduleRequest = CreateScheduleRequest;
