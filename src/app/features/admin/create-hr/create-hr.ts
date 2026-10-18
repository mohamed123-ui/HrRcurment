import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';

import { AuthService } from '../../../core/services/auth';
import { LanguageService } from '../../../core/services/language';
import { SignOutComponent } from '../../../shared/components/sign-out/sign-out';

@Component({
  selector: 'app-create-hr',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, SignOutComponent],
  templateUrl: './create-hr.html',
  styleUrl: './create-hr.scss',
})
export class CreateHrComponent {
  private readonly auth = inject(AuthService);
  readonly language = inject(LanguageService);

  form = {
    Name: '',
    Email: '',
    Password: '',
  };

  saving = false;
  notice = '';
  error = '';

  save(): void {
    this.notice = '';
    this.error = '';

    if (!this.form.Name.trim()) {
      this.error = 'Name is required.';
      return;
    }

    if (!this.form.Email.trim()) {
      this.error = 'Email is required.';
      return;
    }

    if (this.form.Password.length < 6) {
      this.error = 'Password must be at least 6 characters.';
      return;
    }

    this.saving = true;

    this.auth.createHr({
      Name: this.form.Name.trim(),
      Email: this.form.Email.trim(),
      Password: this.form.Password,
    }).subscribe({
      next: (response) => {
        this.saving = false;

        if (response.isSuccess === false) {
          this.error = response.errors?.[0] || 'Unable to create HR account.';
          return;
        }

        this.notice = 'HR account created successfully.';
        this.form = {
          Name: '',
          Email: '',
          Password: '',
        };
      },
      error: (response) => {
        this.saving = false;
        this.error =
          response.error?.errors?.[0] ||
          response.error?.message ||
          'Unable to create HR account.';
      },
    });
  }
}
