import { CommonModule } from '@angular/common';
import { Component, OnInit, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { Job, JobPayload } from '../../../core/models';
import { JobsService } from '../../../core/services/jobs';
import { LanguageService } from '../../../core/services/language';
import { SignOutComponent } from '../../../shared/components/sign-out/sign-out';

@Component({
  selector: 'app-job-management',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, SignOutComponent],
  templateUrl: './job-management.html',
  styleUrl: './job-management.scss',
})
export class JobManagementComponent implements OnInit {
  private readonly jobsApi = inject(JobsService);
  readonly language = inject(LanguageService);

  jobs: Job[] = [];
  selectedJob: Job | null = null;
  loading = true;
  saving = false;
  notice = '';
  error = '';
  form: JobPayload = this.emptyForm();

  ngOnInit(): void {
    this.loadJobs();
  }

  loadJobs(): void {
    this.loading = true;
    this.jobsApi.getAll().subscribe({
      next: (jobs) => {
        this.jobs = jobs;
        this.loading = false;
      },
      error: () => {
        this.error = 'Unable to load jobs.';
        this.loading = false;
      },
    });
  }

  startCreate(): void {
    this.selectedJob = null;
    this.form = this.emptyForm();
    this.clearMessages();
  }

  startEdit(job: Job): void {
    this.selectedJob = job;
    this.form = {
      title: job.title,
      description: job.description,
      requiredSkills: job.requiredSkills,
      experienceRequired: job.experienceRequired,
    };
    this.clearMessages();
  }

  save(): void {
    if (!this.form.title.trim() || !this.form.description.trim()) {
      this.error = 'Title and description are required.';
      return;
    }

    this.saving = true;
    const onSuccess = () => {
      this.saving = false;
      this.notice = this.selectedJob ? 'Job updated successfully.' : 'Job created successfully.';
      this.loadJobs();
      this.startCreate();
    };
    const onError = (response: any) => {
      this.saving = false;
      this.error = response.error?.errors?.[0] || 'Unable to save this job.';
    };

    if (this.selectedJob) {
      this.jobsApi.update(this.selectedJob.id, this.form).subscribe({
        next: onSuccess,
        error: onError,
      });
      return;
    }

    this.jobsApi.create(this.form).subscribe({
      next: () => {
        this.saving = false;
        this.notice = 'Job created successfully.';
        this.loadJobs();
        this.startCreate();
      },
      error: onError,
    });
  }

  toggle(job: Job): void {
    this.jobsApi.toggle(job.id).subscribe({
      next: () => {
        this.notice = 'Job status updated.';
        this.loadJobs();
      },
      error: () => (this.error = 'Unable to update job status.'),
    });
  }

  remove(job: Job): void {
    if (!confirm(`Delete "${job.title}"?`)) return;

    this.jobsApi.delete(job.id).subscribe({
      next: () => {
        this.notice = 'Job deleted successfully.';
        this.loadJobs();
      },
      error: () => (this.error = 'Unable to delete this job.'),
    });
  }

  private emptyForm(): JobPayload {
    return { title: '', description: '', requiredSkills: '', experienceRequired: 0 };
  }

  private clearMessages(): void {
    this.notice = '';
    this.error = '';
  }
}
