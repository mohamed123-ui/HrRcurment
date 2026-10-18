import { CommonModule } from '@angular/common';
import { Component, OnInit, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { Applicant, ApplicantDetails, Job } from '../../../core/models';
import { HrApiService } from '../../../core/services/hr-api';
import { JobsService } from '../../../core/services/jobs';
import { LanguageService } from '../../../core/services/language';
import { SignOutComponent } from '../../../shared/components/sign-out/sign-out';

@Component({
  selector: 'app-candidates',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, SignOutComponent],
  templateUrl: './candidates.html',
  styleUrl: './candidates.scss',
})
export class CandidatesComponent implements OnInit {
  private readonly hrApi = inject(HrApiService);
  private readonly jobsApi = inject(JobsService);
  readonly language = inject(LanguageService);

  jobs: Job[] = [];
  applicants: Applicant[] = [];
  selectedApplicant: ApplicantDetails | null = null;
  selectedJobId = 0;
  search = '';
  status = '';
  loading = false;
  error = '';
  notice = '';

  ngOnInit(): void {
    this.jobsApi.getAll().subscribe({
      next: (jobs) => {
        this.jobs = jobs;
        if (jobs[0]) {
          this.selectedJobId = jobs[0].id;
          this.loadApplicants();
        }
      },
      error: () => (this.error = 'Unable to load jobs.'),
    });
  }

  loadApplicants(): void {
    if (!this.selectedJobId) return;
    this.loading = true;
    this.hrApi.getApplicants(this.selectedJobId, this.search, this.status).subscribe({
      next: (applicants) => {
        this.applicants = applicants;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
        this.error = 'Unable to load candidates.';
      },
    });
  }

  openApplicant(applicant: Applicant): void {
    this.hrApi.getApplicantDetails(applicant.applicationId).subscribe({
      next: (details) => (this.selectedApplicant = details),
      error: () => (this.error = 'Unable to load candidate details.'),
    });
  }

  updateStatus(applicant: Applicant, action: string): void {
    this.hrApi.updateApplicationStatus(applicant.applicationId, action).subscribe({
      next: () => {
        this.notice = `Candidate ${action}ed successfully.`;
        this.loadApplicants();
        this.selectedApplicant = null;
      },
      error: (response) => (this.error = response.error?.errors?.[0] || 'Unable to update status.'),
    });
  }
}
