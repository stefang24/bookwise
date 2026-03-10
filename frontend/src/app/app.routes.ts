import { Routes } from '@angular/router';
import { LayoutPage } from './features/layout/pages/layout.page';
import { guestGuard } from './shared/guards/auth.guard';

export const routes: Routes = [
  {
    path: '',
    component: LayoutPage,
    children: [
      {
        path: 'home',
        loadComponent: () => import('./features/home/pages/home/home.page').then(m => m.HomePage)
      },
      {
        path: '',
        redirectTo: 'home',
        pathMatch: 'full'
      }
    ]
  },
  {
    path: 'login',
    canActivate: [guestGuard],
    loadComponent: () => import('./features/auth/pages/login/login.page').then(m => m.LoginPage)
  },
  {
    path: 'register',
    canActivate: [guestGuard],
    loadComponent: () => import('./features/auth/pages/register/register.page').then(m => m.RegisterPage)
  },
  {
    path: '**',
    redirectTo: 'home'
  }
];
