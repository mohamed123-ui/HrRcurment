import { Injectable } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class TokenService {
  private readonly ACCESS_TOKEN = 'access_token';
  private readonly REFRESH_TOKEN = 'refresh_token';
  private readonly USER = 'smart_user';

  getToken(): string | null {
    return localStorage.getItem(this.ACCESS_TOKEN);
  }
  getRefreshToken(): string | null {
    return localStorage.getItem(this.REFRESH_TOKEN);
  }
  setTokens(access: string, refresh: string): void {
    localStorage.setItem(this.ACCESS_TOKEN, access);
    localStorage.setItem(this.REFRESH_TOKEN, refresh);
  }
  setUser(user: unknown): void {
    localStorage.setItem(this.USER, JSON.stringify(user));
  }
  getUser<T>(): T | null {
    const raw = localStorage.getItem(this.USER);
    return raw ? (JSON.parse(raw) as T) : null;
  }
  clearTokens(): void {
    localStorage.removeItem(this.ACCESS_TOKEN);
    localStorage.removeItem(this.REFRESH_TOKEN);
    localStorage.removeItem(this.USER);
  }
}
