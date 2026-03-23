import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';
import { AuthService } from './auth.service';
import { LocalStorageService } from './local-storage.service';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const localStorageService: LocalStorageService = inject(LocalStorageService);
  const authService: AuthService = inject(AuthService);
  const router: Router = inject(Router);
  const token: string | null = localStorageService.getToken();

  if (token) {
    const clonedReq = req.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`
      }
    });
    return next(clonedReq).pipe(
      catchError((error: HttpErrorResponse) => {
        if (error.status === 401) {
          authService.logout();
        } else if (error.status === 403) {
          router.navigate(['/forbidden']);
        } else if (error.status === 500) {
          router.navigate(['/internal-server-error']);
        }

        return throwError(() => error);
      })
    );
  }

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      if (error.status === 401) {
        authService.logout();
      } else if (error.status === 403) {
        router.navigate(['/forbidden']);
      } else if (error.status === 500) {
        router.navigate(['/internal-server-error']);
      }

      return throwError(() => error);
    })
  );
};
