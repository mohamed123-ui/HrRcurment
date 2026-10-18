import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth-guard';
import { roleGuard } from './core/guards/role-guard';

export const routes: Routes = [
  { path: '', loadComponent: () => import('./features/home/home').then((m) => m.HomeComponent) },
  {
    path: 'careers',
    loadComponent: () => import('./features/careers/careers').then((m) => m.CareersComponent),
  },
  { path: 'auth', loadChildren: () => import('./features/auth/routes').then((m) => m.AUTH_ROUTES) },
  {
    path: 'candidate/profile',
    canActivate: [authGuard, roleGuard],
    data: { roles: 'Candidate' },
    loadComponent: () =>
      import('./features/candidate/candidate-portal/candidate-portal').then(
        (m) => m.CandidatePortal,
      ),
  },
  {
    path: 'hr/dashboard',
    canActivate: [authGuard, roleGuard],
    data: { roles: 'Admin,HR,Hr,admin,hr' },
    loadComponent: () =>
      import('./features/hr/hr-dashboard/hr-dashboard').then((m) => m.HrDashboard),
  },
  {
    path: 'hr/jobs',
    canActivate: [authGuard, roleGuard],
    data: { roles: 'Admin,HR,Hr,admin,hr' },
    loadComponent: () =>
      import('./features/hr/job-management/job-management').then((m) => m.JobManagementComponent),
  },
  {
    path: 'hr/candidates',
    canActivate: [authGuard, roleGuard],
    data: { roles: 'Admin,HR,Hr,admin,hr' },
    loadComponent: () =>
      import('./features/hr/candidates/candidates').then((m) => m.CandidatesComponent),
  },
  {
    path: 'hr/interviews',
    canActivate: [authGuard, roleGuard],
    data: { roles: 'Admin,HR,Hr,admin,hr' },
    loadComponent: () =>
      import('./features/hr/interviews/interviews').then((m) => m.InterviewsComponent),
  },
  {
    path: 'admin/hr/create',
    canActivate: [authGuard, roleGuard],
    data: { roles: 'Admin,admin' },
    loadComponent: () =>
      import('./features/admin/create-hr/create-hr').then((m) => m.CreateHrComponent),
  },
  {
    path: 'unauthorized',
    loadComponent: () =>
      import('./features/unauthorized/unauthorized').then((m) => m.UnauthorizedComponent),
  },
  { path: '**', redirectTo: '' },
];
