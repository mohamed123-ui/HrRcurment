import { CommonModule } from '@angular/common';
import { Component, OnInit, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { Interview, ScheduleInterviewPayload, ShortlistedCandidate } from '../../../core/models';
import { HrApiService } from '../../../core/services/hr-api';
import { LanguageService } from '../../../core/services/language';
import { SignOutComponent } from '../../../shared/components/sign-out/sign-out';

@Component({
  selector: 'app-interviews',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, SignOutComponent],
  templateUrl: './interviews.html',
  styleUrl: './interviews.scss',
})
export class InterviewsComponent implements OnInit {
  private readonly hrApi = inject(HrApiService);
  readonly language = inject(LanguageService);

  interviews: Interview[] = [];
  shortlisted: ShortlistedCandidate[] = [];
  selectedTab = 'upcoming';
  saving = false;
  notice = '';
  error = '';
  form: ScheduleInterviewPayload = {
    applicationId: 0,
    type: 'HR',
    scheduledAt: '',
    location: '',
  };

  ngOnInit(): void {
    this.loadInterviews();
    this.hrApi.getShortlisted().subscribe({
      next: (candidates) => (this.shortlisted = candidates),
      error: () => (this.error = 'Unable to load shortlisted candidates.'),
    });
  }

  loadInterviews(): void {
    this.hrApi.getInterviews(this.selectedTab).subscribe({
      next: (interviews) => (this.interviews = interviews),
      error: () => (this.error = 'Unable to load interviews.'),
    });
  }

  schedule(): void {
    if (!this.form.applicationId || !this.form.scheduledAt) {
      this.error = 'Candidate and date are required.';
      return;
    }

    this.saving = true;
    this.hrApi.scheduleInterview(this.form).subscribe({
      next: () => {
        this.saving = false;
        this.notice = 'Interview scheduled successfully.';
        this.form = { applicationId: 0, type: 'HR', scheduledAt: '', location: '' };
        this.loadInterviews();
      },
      error: (response) => {
        this.saving = false;
        this.error = response.error?.errors?.[0] || 'Unable to schedule interview.';
      },
    });
  }

  recordResult(interview: Interview, result: string): void {
    this.hrApi.recordInterviewResult(interview.id, result).subscribe({
      next: () => {
        this.notice = 'Interview result saved.';
        this.loadInterviews();
      },
      error: () => (this.error = 'Unable to save interview result.'),
    });
  }

  cancel(interview: Interview): void {
    if (!confirm(`Cancel interview with ${interview.candidateName}?`)) return;

    this.hrApi.cancelInterview(interview.id).subscribe({
      next: () => {
        this.notice = 'Interview cancelled.';
        this.loadInterviews();
      },
      error: () => (this.error = 'Unable to cancel interview.'),
    });
  }
}
