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
