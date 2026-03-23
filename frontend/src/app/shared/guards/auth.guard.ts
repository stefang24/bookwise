import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { LocalStorageService } from '../services/local-storage.service';

export const guestGuard: CanActivateFn = () => {
  const localStorageService: LocalStorageService = inject(LocalStorageService);
  const router: Router = inject(Router);

  if (localStorageService.getToken()) {
    router.navigate(['/home']);
    return false;
  }

  return true;
};

export const authGuard: CanActivateFn = () => {
  const localStorageService: LocalStorageService = inject(LocalStorageService);
  const router: Router = inject(Router);

  if (!localStorageService.getToken()) {
    router.navigate(['/login']);
    return false;
  }

  return true;
};

export const providerGuard: CanActivateFn = () => {
  const localStorageService: LocalStorageService = inject(LocalStorageService);
  const router: Router = inject(Router);

  if (!localStorageService.getToken()) {
    router.navigate(['/login']);
    return false;
  }

  if (localStorageService.getRole() !== 'Provider') {
    router.navigate(['/home']);
    return false;
  }

  return true;
};

export const userGuard: CanActivateFn = () => {
  const localStorageService: LocalStorageService = inject(LocalStorageService);
  const router: Router = inject(Router);

  if (!localStorageService.getToken()) {
    router.navigate(['/login']);
    return false;
  }

  if (localStorageService.getRole() !== 'User') {
    router.navigate(['/home']);
    return false;
  }

  return true;
};

export const userOrProviderGuard: CanActivateFn = () => {
  const localStorageService: LocalStorageService = inject(LocalStorageService);
  const router: Router = inject(Router);

  if (!localStorageService.getToken()) {
    router.navigate(['/login']);
    return false;
  }

  const role: string = localStorageService.getRole();
  if (role !== 'User' && role !== 'Provider') {
    router.navigate(['/home']);
    return false;
  }

  return true;
};

export const adminGuard: CanActivateFn = () => {
  const localStorageService: LocalStorageService = inject(LocalStorageService);
  const router: Router = inject(Router);

  if (!localStorageService.getToken()) {
    router.navigate(['/login']);
    return false;
  }

  if (localStorageService.getRole() !== 'Admin') {
    router.navigate(['/home']);
    return false;
  }

  return true;
};

export const chatGuard: CanActivateFn = () => {
  const localStorageService: LocalStorageService = inject(LocalStorageService);
  const router: Router = inject(Router);

  if (!localStorageService.getToken()) {
    router.navigate(['/login']);
    return false;
  }

  const role: string = localStorageService.getRole();
  if (role !== 'User' && role !== 'Provider') {
    router.navigate(['/home']);
    return false;
  }

  return true;
};
