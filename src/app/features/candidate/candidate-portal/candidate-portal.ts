import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { CandidateApiService } from '../../../core/services/candidate-api';
import { AuthService } from '../../../core/services/auth';
import { LanguageService } from '../../../core/services/language';
import { LanguageSwitcherComponent } from '../../../shared/components/language-switcher/language-switcher';
import { SignOutComponent } from '../../../shared/components/sign-out/sign-out';
import { TranslatePipe } from '../../../shared/pipes/translate.pipe';
import { Application, CandidateProfile } from '../../../core/models';
@Component({
  selector: 'app-candidate-portal',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    RouterLink,
    LanguageSwitcherComponent,
    TranslatePipe,
    SignOutComponent,
  ],
  templateUrl: './candidate-portal.html',
  styleUrl: './candidate-portal.scss',
})
export class CandidatePortal implements OnInit {
  private api = inject(CandidateApiService);
  auth = inject(AuthService);
  language = inject(LanguageService);
  profile: CandidateProfile | null = null;
  applications: Application[] = [];
  editing = false;
  saving = false;
  notice = '';
  error = '';
  form: any = {
    age: null,
    educationLevel: '',
    universityTier: '',
    cgpa: null,
    internships: null,
    projects: null,
    programmingLanguages: null,
    certifications: null,
    experienceYears: null,
    hackathons: null,
    skillsScore: null,
    companyType: '',
    skills: '',
    bio: '',
  };
  ngOnInit() {
    this.api.getProfile().subscribe({
      next: (p) => {
        this.profile = p;
        this.form = { ...this.form, ...p };
      },
      error: () => {
        this.profile = null;
        this.editing = true;
        this.error = 'Please save your candidate profile before uploading a CV.';
      },
    });
    this.api.getApplications().subscribe({ next: (a) => (this.applications = a), error: () => {} });
  }
  startProfileEditing(): void {
    this.editing = this.profile ? !this.editing : true;
    this.error = '';
  }

  save(): void {
    if (!this.form.educationLevel?.trim() || !this.form.universityTier?.trim()) {
      this.error = 'Education level and university tier are required.';
      return;
    }

    if (this.form.age === null || this.form.age < 18 || this.form.age > 80) {
      this.error = 'Age must be between 18 and 80.';
      return;
    }

    if (this.form.cgpa === null || this.form.cgpa < 0 || this.form.cgpa > 4) {
      this.error = 'CGPA must be between 0 and 4.';
      return;
    }

    if (!this.form.companyType?.trim()) {
      this.error = 'Company type is required.';
      return;
    }

    this.error = '';
    this.saving = true;
    this.api.saveProfile(this.toProfilePayload()).subscribe({
      next: () => {
        this.saving = false;
        this.notice = 'تم حفظ ملفك بنجاح.';
        this.editing = false;
        this.api.getProfile().subscribe((p) => (this.profile = p));
      },
      error: (e) => {
        this.saving = false;
        this.error = e.error?.errors?.[0] || 'تحقق من البيانات المطلوبة.';
      },
    });
  }

  private toProfilePayload() {
    return {
      age: Number(this.form.age),
      educationLevel: String(this.form.educationLevel ?? '').trim(),
      universityTier: String(this.form.universityTier ?? '').trim(),
      companyType: String(this.form.companyType ?? '').trim(),
      skills: String(this.form.skills ?? '').trim(),
      bio: String(this.form.bio ?? '').trim(),
      cgpa: Number(this.form.cgpa),
      internships: Number(this.form.internships || 0),
      projects: Number(this.form.projects || 0),
      programmingLanguages: Number(this.form.programmingLanguages || 0),
      certifications: Number(this.form.certifications || 0),
      experienceYears: Number(this.form.experienceYears || 0),
      hackathons: Number(this.form.hackathons || 0),
      skillsScore: Number(this.form.skillsScore || 0),
    };
  }

  upload(event: Event) {
    if (!this.profile) {
      this.editing = true;
      this.error = 'Please create and save your candidate profile before uploading a CV.';
      return;
    }

    const file = (event.target as HTMLInputElement).files?.[0];
    if (!file) return;

    const allowedTypes = [
      'application/pdf',
      'application/msword',
      'application/vnd.openxmlformats-officedocument.wordprocessingml.document',
    ];
    const maxSize = 5 * 1024 * 1024;

    if (!allowedTypes.includes(file.type) || file.size > maxSize) {
      this.error = 'Please choose a PDF or DOC/DOCX file smaller than 5MB.';
      return;
    }

    this.error = '';
    this.api.uploadCv(file).subscribe({
      next: () => (this.notice = 'تم رفع السيرة الذاتية.'),
      error: (e) => (this.error = e.error?.errors?.[0] || 'تعذر رفع الملف.'),
    });
  }
  logout() {
    this.auth.logout();
  }
}
