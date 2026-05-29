import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { environment } from '../../../../environments/environment';
import {
  CreateScheduleRequest,
  Schedule,
  UpdateScheduleRequest
} from '../models/schedule.model';

@Injectable({
  providedIn: 'root'
})
export class ScheduleService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiUrl}/schedules`;

  getAll(): Observable<Schedule[]> {
    return this.http.get<Schedule[]>(this.apiUrl);
  }

  getById(id: number): Observable<Schedule> {
    return this.http.get<Schedule>(`${this.apiUrl}/${id}`);
  }

  create(request: CreateScheduleRequest): Observable<Schedule> {
    return this.http.post<Schedule>(this.apiUrl, request);
  }

  update(id: number, request: UpdateScheduleRequest): Observable<Schedule> {
    return this.http.put<Schedule>(`${this.apiUrl}/${id}`, request);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
