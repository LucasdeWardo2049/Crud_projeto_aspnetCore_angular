import { Routes } from '@angular/router';

import { MainLayoutComponent } from './layouts/main-layout/main-layout.component';
import { DashboardComponent } from './modules/dashboard/pages/dashboard/dashboard.component';
import { ScheduleFormComponent } from './modules/schedules/pages/schedule-form/schedule-form.component';
import { ScheduleListComponent } from './modules/schedules/pages/schedule-list/schedule-list.component';

export const routes: Routes = [
  {
    path: '',
    component: MainLayoutComponent,
    children: [
      {
        path: '',
        redirectTo: 'dashboard',
        pathMatch: 'full'
      },
      {
        path: 'dashboard',
        component: DashboardComponent
      },
      {
        path: 'schedules',
        component: ScheduleListComponent
      },
      {
        path: 'schedules/new',
        component: ScheduleFormComponent
      },
      {
        path: 'schedules/:id/edit',
        component: ScheduleFormComponent
      }
    ]
  },
  {
    path: '**',
    redirectTo: 'dashboard'
  }
];
