import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';
@Component({
  selector: 'app-unauthorized',
  standalone: true,
  imports: [RouterLink],
  template: `<main class="empty-state">
    <div class="empty-icon">!</div>
    <h1>لا تملك صلاحية الوصول</h1>
    <p>هذه الصفحة مخصصة لدور مختلف داخل المنصة.</p>
    <a routerLink="/" class="btn btn-primary">العودة للرئيسية</a>
  </main>`,
  styles: [
    `
      .empty-state {
        min-height: 100vh;
        display: grid;
        place-content: center;
        text-align: center;
        background: #f6f8fc;
        color: #14213d;
        font-family: Arial, sans-serif;
        gap: 12px;
      }
      .empty-icon {
        margin: auto;
        background: #fee2e2;
        color: #dc2626;
        border-radius: 50%;
        width: 64px;
        height: 64px;
        display: grid;
        place-content: center;
        font-size: 30px;
        font-weight: 700;
      }
      .btn {
        padding: 13px 22px;
        border-radius: 12px;
        text-decoration: none;
      }
      .btn-primary {
        background: #2563eb;
        color: #fff;
      }
    `,
  ],
})
export class UnauthorizedComponent {}
