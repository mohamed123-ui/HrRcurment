import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { AuthService } from '../services/auth';

export const roleGuard: CanActivateFn = (route) => {
  const auth = inject(AuthService);
  const router = inject(Router);
  const expected = String(route.data['roles'] ?? route.data['role'] ?? '')
    .split(',')
    .map((x: string) => x.trim())
    .filter(Boolean);
  if (expected.length && !auth.hasRole(...expected)) return router.createUrlTree(['/unauthorized']);
  return true;
};
