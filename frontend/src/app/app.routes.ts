import { Routes } from '@angular/router';
import { LayoutPage } from './features/layout/pages/layout.page';
import { adminGuard, authGuard, chatGuard, guestGuard, providerGuard } from './shared/guards/auth.guard';

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
        path: 'profile/:id',
        loadComponent: () => import('./features/profile/pages/profile/profile.page').then(m => m.ProfilePage)
      },
      {
        path: 'provider/services',
        canActivate: [providerGuard],
        loadComponent: () => import('./features/provider/pages/provider-services/provider-services.page').then(m => m.ProviderServicesPage)
      },
      {
        path: 'provider/schedule',
        canActivate: [providerGuard],
        loadComponent: () => import('./features/provider/pages/provider-schedule/provider-schedule.page').then(m => m.ProviderSchedulePage)
      },
      {
        path: 'book',
        loadComponent: () => import('./features/appointments/pages/booking/booking.page').then(m => m.BookingPage)
      },
      {
        path: 'providers',
        loadComponent: () => import('./features/providers/pages/providers/providers.page').then(m => m.ProvidersPage)
      },
      {
        path: 'appointments',
        canActivate: [authGuard],
        loadComponent: () => import('./features/appointments/pages/appointments/appointments.page').then(m => m.AppointmentsPage)
      },
      {
        path: 'chat',
        canActivate: [chatGuard],
        loadComponent: () => import('./features/chat/pages/chat/chat.page').then(m => m.ChatPage)
      },
      {
        path: 'admin',
        canActivate: [adminGuard],
        loadComponent: () => import('./features/admin/pages/admin-dashboard/admin-dashboard.page').then(m => m.AdminDashboardPage)
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
    path: 'forbidden',
    loadComponent: () => import('./features/errors/pages/forbidden/forbidden.page').then(m => m.ForbiddenPage)
  },
  {
    path: 'internal-server-error',
    loadComponent: () => import('./features/errors/pages/internal-server-error/internal-server-error.page').then(m => m.InternalServerErrorPage)
  },
  {
    path: '**',
    redirectTo: 'home'
  }
];
