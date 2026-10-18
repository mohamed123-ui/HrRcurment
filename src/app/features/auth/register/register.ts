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
  selector: 'app-register',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterLink,
    LanguageSwitcherComponent,
    TranslatePipe,
    SignOutComponent,
  ],
  templateUrl: './register.html',
  styleUrl: './register.scss',
})
export class RegisterComponent {
  private fb = inject(FormBuilder);
  readonly auth = inject(AuthService);
  private router = inject(Router);
  readonly language = inject(LanguageService);
  loading = false;
  errorMessage = '';
  form = this.fb.nonNullable.group({
    Name: ['', [Validators.required, Validators.minLength(3)]],
    Email: ['', [Validators.required, Validators.email]],
    Password: ['', [Validators.required, Validators.minLength(6)]],
  });
  get f() {
    return this.form.controls;
  }
  onRegister() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    this.loading = true;
    this.auth.register(this.form.getRawValue()).subscribe({
      next: (res) => {
        this.loading = false;
        if (res.value) this.router.navigate(['/candidate/profile']);
        else this.errorMessage = res.errors?.[0] || 'تعذر إنشاء الحساب.';
      },
      error: (e) => {
        this.loading = false;
        this.errorMessage = e.error?.errors?.[0] || e.error?.message || 'تعذر الاتصال بالخادم.';
      },
    });
  }
}
