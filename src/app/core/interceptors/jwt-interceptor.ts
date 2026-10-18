import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, switchMap, throwError } from 'rxjs';
import { TokenService } from '../services/token';
import { AuthService } from '../services/auth';

let refreshing = false;
export const jwtInterceptor: HttpInterceptorFn = (req, next) => {
  const tokens = inject(TokenService);
  const auth = inject(AuthService);
  const access = tokens.getToken();
  const request = access ? req.clone({ setHeaders: { Authorization: `Bearer ${access}` } }) : req;
  return next(request).pipe(
    catchError((error: HttpErrorResponse) => {
      if (error.status !== 401 || request.url.toLowerCase().includes('/auth/'))
        return throwError(() => error);
      const refresh = tokens.getRefreshToken();
      if (!refresh || refreshing) {
        auth.logout();
        return throwError(() => error);
      }
      refreshing = true;
      return auth.refreshToken().pipe(
        switchMap(() => {
          refreshing = false;
          const token = tokens.getToken();
          return next(
            token ? req.clone({ setHeaders: { Authorization: `Bearer ${token}` } }) : req,
          );
        }),
        catchError((refreshError) => {
          refreshing = false;
          auth.logout();
          return throwError(() => refreshError);
        }),
      );
    }),
  );
};
