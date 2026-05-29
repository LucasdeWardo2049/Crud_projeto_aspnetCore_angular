import { Routes } from '@angular/router';

import { MainLayoutComponent } from './layouts/main-layout/main-layout.component';
import { ScheduleFormComponent } from './modules/schedules/pages/schedule-form/schedule-form.component';
import { ScheduleListComponent } from './modules/schedules/pages/schedule-list/schedule-list.component';

export const routes: Routes = [
  {
    path: '',
    component: MainLayoutComponent,
    children: [
      {
        path: '',
        redirectTo: 'schedules',
        pathMatch: 'full'
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
    redirectTo: 'schedules'
  }
];
