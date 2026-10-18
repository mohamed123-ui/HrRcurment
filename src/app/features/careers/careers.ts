import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { JobsService } from '../../core/services/jobs';
import { CandidateApiService } from '../../core/services/candidate-api';
import { AuthService } from '../../core/services/auth';
import { LanguageService } from '../../core/services/language';
import { SignOutComponent } from '../../shared/components/sign-out/sign-out';
import { LanguageSwitcherComponent } from '../../shared/components/language-switcher/language-switcher';
import { TranslatePipe } from '../../shared/pipes/translate.pipe';
import { Job } from '../../core/models';
@Component({
  selector: 'app-careers',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    RouterLink,
    LanguageSwitcherComponent,
    TranslatePipe,
    SignOutComponent,
  ],
  templateUrl: './careers.html',
  styleUrl: './careers.scss',
})
export class CareersComponent implements OnInit {
  private jobsApi = inject(JobsService);
  private candidate = inject(CandidateApiService);
  private router = inject(Router);
  auth = inject(AuthService);
  language = inject(LanguageService);
  jobs: Job[] = [];
  filtered: Job[] = [];
  selected: Job | null = null;
  query = '';
  applying = false;
  message = '';
  error = '';
  ngOnInit() {
    this.jobsApi.getAll().subscribe({
      next: (jobs) => {
        this.jobs = jobs;
        this.filtered = jobs;
      },
      error: () => {
        this.jobs = [];
        this.filtered = [];
        this.error = 'تعذر تحميل الوظائف من الخادم.';
      },
    });
  }
  search() {
    const q = this.query.trim().toLowerCase();
    this.filtered = this.jobs.filter(
      (j) => !q || `${j.title} ${j.description} ${j.requiredSkills}`.toLowerCase().includes(q),
    );
  }
  open(job: Job) {
    this.selected = job;
    this.message = '';
    this.error = '';
  }
  apply() {
    if (!this.selected) return;
    if (!this.auth.isLoggedIn()) {
      this.router.navigate(['/auth/login'], {
        queryParams: { returnUrl: `/careers?job=${this.selected.id}` },
      });
      return;
    }
    if (!this.auth.hasRole('Candidate')) {
      this.error = 'التقديم متاح لحسابات المرشحين فقط.';
      return;
    }
    this.applying = true;
    this.candidate.apply(this.selected.id).subscribe({
      next: () => {
        this.applying = false;
        this.message = 'تم إرسال طلبك بنجاح.';
      },
      error: (e) => {
        this.applying = false;
        this.error = e.error?.errors?.[0] || e.error?.message || 'تعذر إرسال الطلب الآن.';
      },
    });
  }
}
