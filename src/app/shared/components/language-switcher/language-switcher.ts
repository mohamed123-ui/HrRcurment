import { Component, inject } from '@angular/core';
import { LanguageService } from '../../../core/services/language';
import { TranslatePipe } from '../../pipes/translate.pipe';

@Component({
  selector: 'app-language-switcher',
  standalone: true,
  imports: [TranslatePipe],
  template: `
    <button class="language-switcher" type="button" (click)="language.toggle()">
      <span class="globe">◎</span>
      {{ 'language' | translate }}
    </button>
  `,
  styles: [
    `
      .language-switcher {
        display: inline-flex;
        align-items: center;
        gap: 7px;
        border: 1px solid #dbe3ee;
        border-radius: 10px;
        background: #ffffff;
        color: #475569;
        padding: 8px 12px;
        cursor: pointer;
        font:
          600 12px Cairo,
          Arial,
          sans-serif;
      }

      .language-switcher:hover {
        border-color: #2563eb;
        color: #2563eb;
      }

      .globe {
        color: #2563eb;
        font-size: 16px;
      }
    `,
  ],
})
export class LanguageSwitcherComponent {
  readonly language = inject(LanguageService);
}
