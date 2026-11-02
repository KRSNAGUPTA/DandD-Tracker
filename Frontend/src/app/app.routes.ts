import { Routes } from '@angular/router';

export const routes: Routes = [
  { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
  {
    path: 'dashboard',
    title: 'D&D Tracker - Econship',
    loadComponent: () => import('./features/dashboard/dashboard').then(m => m.Dashboard)
  },
  {
    path: 'containers/:id',
    title: 'Container Detail - Econship',
    loadComponent: () => import('./features/container-detail/container-detail').then(m => m.ContainerDetailPage)
  },
  { path: '**', redirectTo: 'dashboard' }
];