// src/app/core/interceptors/loading-error.interceptor.ts
import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { LoadingService } from '../services/loading';
import { finalize, catchError, throwError } from 'rxjs';

export const loadingErrorInterceptor: HttpInterceptorFn = (req, next) => {
  const loadingService = inject(LoadingService);
  loadingService.show();

  return next(req).pipe(
    catchError((error) => {
      console.error('Error Interceptor caught:', error);
      return throwError(() => error);
    }),
    finalize(() => {
      loadingService.hide();
    }),
  );
};
