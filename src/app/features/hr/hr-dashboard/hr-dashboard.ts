import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { HrApiService } from '../../../core/services/hr-api';
import { JobsService } from '../../../core/services/jobs';
import { AuthService } from '../../../core/services/auth';
import { LanguageService } from '../../../core/services/language';
import { LanguageSwitcherComponent } from '../../../shared/components/language-switcher/language-switcher';
import { SignOutComponent } from '../../../shared/components/sign-out/sign-out';
import { TranslatePipe } from '../../../shared/pipes/translate.pipe';
import { DashboardStats, Job } from '../../../core/models';
@Component({
  selector: 'app-hr-dashboard',
  standalone: true,
  imports: [CommonModule, RouterLink, LanguageSwitcherComponent, TranslatePipe, SignOutComponent],
  templateUrl: './hr-dashboard.html',
  styleUrl: './hr-dashboard.scss',
})
export class HrDashboard implements OnInit {
  private api = inject(HrApiService);
  private jobsApi = inject(JobsService);
  auth = inject(AuthService);
  language = inject(LanguageService);
  stats: DashboardStats = {
    activeJobs: 0,
    totalApplicants: 0,
    shortlistedCount: 0,
    upcomingInterviewsCount: 0,
    acceptedCount: 0,
    rejectedCount: 0,
    upcomingInterviews: [],
    applicantsByStatus: [],
  };
  jobs: Job[] = [];
  loading = true;
  ngOnInit() {
    this.api.getDashboard().subscribe({
      next: (s) => {
        this.stats = s;
        this.loading = false;
      },
      error: () => (this.loading = false),
    });
    this.jobsApi.getAll().subscribe({ next: (j) => (this.jobs = j), error: () => {} });
  }
  logout() {
    this.auth.logout();
  }
  getBar(count: number) {
    const max = Math.max(...this.stats.applicantsByStatus.map((x) => x.count), 1);
    return `${Math.max(8, Math.round((count / max) * 100))}%`;
  }
}
