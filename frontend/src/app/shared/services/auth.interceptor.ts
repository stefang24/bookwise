import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { LocalStorageService } from './local-storage.service';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const localStorageService: LocalStorageService = inject(LocalStorageService);
  const token: string | null = localStorageService.getToken();

  if (token) {
    const clonedReq = req.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`
      }
    });
    return next(clonedReq);
  }

  return next(req);
};
