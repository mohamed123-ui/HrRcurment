import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../../core/services/auth';
import { LanguageService } from '../../../core/services/language';
import { SignOutComponent } from '../../../shared/components/sign-out/sign-out';
import { LanguageSwitcherComponent } from '../../../shared/components/language-switcher/language-switcher';
import { TranslatePipe } from '../../../shared/pipes/translate.pipe';
@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterLink,
    LanguageSwitcherComponent,
    TranslatePipe,
    SignOutComponent,
  ],
  templateUrl: './login.html',
  styleUrl: './login.scss',
})
export class LoginComponent {
  private fb = inject(FormBuilder);
  readonly auth = inject(AuthService);
  private router = inject(Router);
  readonly language = inject(LanguageService);
  loading = false;
  errorMessage = '';
  form = this.fb.nonNullable.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(6)]],
  });
  get f() {
    return this.form.controls;
  }
  onLogin() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    this.loading = true;
    this.errorMessage = '';
    this.auth.login(this.form.getRawValue()).subscribe({
      next: (res) => {
        this.loading = false;
        if (!res.value) {
          this.errorMessage = res.errors?.[0] || 'بيانات الدخول غير صحيحة.';
          return;
        }
        const returnUrl = new URLSearchParams(location.search).get('returnUrl');
        const role = res.value.role?.toLowerCase();
        this.router.navigateByUrl(
          returnUrl || (role === 'candidate' ? '/candidate/profile' : '/hr/dashboard'),
        );
      },
      error: (e) => {
        this.loading = false;
        this.errorMessage = e.error?.errors?.[0] || e.error?.message || 'تعذر الاتصال بالخادم.';
      },
    });
  }
}
