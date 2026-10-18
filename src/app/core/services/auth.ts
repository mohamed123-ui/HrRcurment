import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import { AuthUser, ApiEnvelope, CreateHrPayload } from '../models';
import { TokenService } from './token';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private http = inject(HttpClient);
  private router = inject(Router);
  private tokens = inject(TokenService);
  private baseUrl = `${environment.apiUrl}/Auth`;

  login(credentials: { email: string; password: string }): Observable<ApiEnvelope<AuthUser>> {
    return this.http
      .post<ApiEnvelope<AuthUser>>(`${this.baseUrl}/Login`, credentials)
      .pipe(tap((res) => this.persist(res)));
  }
  register(data: {
    Name: string;
    Email: string;
    Password: string;
  }): Observable<ApiEnvelope<AuthUser>> {
    return this.http
      .post<ApiEnvelope<AuthUser>>(`${this.baseUrl}/Register`, data)
      .pipe(tap((res) => this.persist(res)));
  }
  refreshToken(): Observable<ApiEnvelope<AuthUser>> {
    return this.http
      .post<ApiEnvelope<AuthUser>>(`${this.baseUrl}/RefreshToken`, {
        refreshToken: this.tokens.getRefreshToken(),
      })
      .pipe(tap((res) => this.persist(res)));
  }
  forgotPassword(email: string) {
    return this.http.post(`${this.baseUrl}/forgotPassword`, { email });
  }
  resetPassword(data: unknown) {
    return this.http.post(`${this.baseUrl}/resetPassword`, data);
  }

  createHr(data: CreateHrPayload) {
    return this.http.post<ApiEnvelope<unknown>>(
      `${this.baseUrl}/CreateHrByAdmin`,
      data,
    );
  }

  persist(res: ApiEnvelope<AuthUser>): void {
    const value = res?.value;
    if (value?.accessToken) {
      this.tokens.setTokens(value.accessToken, value.refreshToken);
      this.tokens.setUser(value);
    }
  }
  getToken() {
    return this.tokens.getToken();
  }
  getUser<Auth = AuthUser>() {
    return this.tokens.getUser<Auth>();
  }
  isLoggedIn() {
    return !!this.tokens.getToken();
  }
  hasRole(...roles: string[]) {
    const role = String(this.getUser<AuthUser>()?.role ?? '').toLowerCase();
    return roles.some((r) => r.toLowerCase() === role);
  }
  logout(): void {
    this.tokens.clearTokens();
    this.router.navigate(['/auth/login']);
  }
}
