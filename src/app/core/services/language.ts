import { Injectable, signal } from '@angular/core';

export type AppLanguage = 'ar' | 'en';

@Injectable({ providedIn: 'root' })
export class LanguageService {
  private readonly storageKey = 'smart_language';
  readonly language = signal<AppLanguage>(this.readLanguage());

  constructor() {
    this.applyDocumentSettings(this.language());
  }

  setLanguage(language: AppLanguage): void {
    this.language.set(language);
    localStorage.setItem(this.storageKey, language);
    this.applyDocumentSettings(language);
  }

  toggle(): void {
    this.setLanguage(this.language() === 'ar' ? 'en' : 'ar');
  }

  isArabic(): boolean {
    return this.language() === 'ar';
  }

  private readLanguage(): AppLanguage {
    return localStorage.getItem(this.storageKey) === 'en' ? 'en' : 'ar';
  }

  private applyDocumentSettings(language: AppLanguage): void {
    document.documentElement.lang = language;
    document.documentElement.dir = language === 'ar' ? 'rtl' : 'ltr';
  }
}
