import { Component, inject } from '@angular/core';
import { AuthService } from '../../../core/services/auth';

@Component({
  selector: 'app-sign-out',
  standalone: true,
  template: `
    <button class="sign-out" type="button" (click)="auth.logout()">
      <span aria-hidden="true">↪</span>
      {{ label }}
    </button>
  `,
  styles: [
    `
      .sign-out {
        display: inline-flex;
        align-items: center;
        gap: 7px;
        border: 0;
        border-radius: 9px;
        background: transparent;
        color: #dc2626;
        padding: 8px 10px;
        cursor: pointer;
        font:
          600 12px Cairo,
          Arial,
          sans-serif;
      }
      .sign-out:hover {
        background: #fef2f2;
      }
    `,
  ],
})
export class SignOutComponent {
  readonly auth = inject(AuthService);
  readonly label = 'Sign out';
}
