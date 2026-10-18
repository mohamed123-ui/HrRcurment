import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../core/services/auth';
import { LanguageService } from '../../core/services/language';
import { SignOutComponent } from '../../shared/components/sign-out/sign-out';
import { LanguageSwitcherComponent } from '../../shared/components/language-switcher/language-switcher';
import { TranslatePipe } from '../../shared/pipes/translate.pipe';
@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, RouterLink, LanguageSwitcherComponent, TranslatePipe, SignOutComponent],
  templateUrl: './home.html',
  styleUrl: './home.scss',
})
export class HomeComponent {
  readonly auth = inject(AuthService);
  readonly language = inject(LanguageService);
}
