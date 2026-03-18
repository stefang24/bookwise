import { Routes } from '@angular/router';
import { LayoutPage } from './features/layout/pages/layout.page';
import { authGuard, guestGuard, providerGuard, userGuard } from './shared/guards/auth.guard';

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
        canActivate: [authGuard],
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
        canActivate: [userGuard],
        loadComponent: () => import('./features/appointments/pages/booking/booking.page').then(m => m.BookingPage)
      },
      {
        path: 'providers',
        canActivate: [userGuard],
        loadComponent: () => import('./features/providers/pages/providers/providers.page').then(m => m.ProvidersPage)
      },
      {
        path: 'appointments',
        canActivate: [authGuard],
        loadComponent: () => import('./features/appointments/pages/history/history.page').then(m => m.HistoryPage)
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
