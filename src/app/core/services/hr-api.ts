import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  ApiEnvelope,
  Applicant,
  ApplicantDetails,
  DashboardStats,
  Interview,
  ScheduleInterviewPayload,
  ShortlistedCandidate,
} from '../models';

@Injectable({ providedIn: 'root' })
export class HrApiService {
  private readonly http = inject(HttpClient);
  private readonly base = `${environment.apiUrl}/hr`;

  getDashboard() {
    return this.http
      .get<ApiEnvelope<DashboardStats>>(`${this.base}/dashboard`)
      .pipe(map((response) => (response.value ?? response) as DashboardStats));
  }

  getApplicants(jobId: number, search = '', status = '') {
    return this.http.get<Applicant[]>(`${this.base}/jobs/${jobId}/applicants`, {
      params: { search, status },
    });
  }

  getShortlisted() {
    return this.http.get<ShortlistedCandidate[]>(`${this.base}/applications`, {
      params: { status: 'Shortlisted' },
    });
  }

  getApplicantDetails(id: number) {
    return this.http.get<ApplicantDetails>(`${this.base}/applications/${id}`);
  }

  updateApplicationStatus(id: number, action: string, note?: string) {
    return this.http.patch(`${this.base}/applications/${id}/status`, { action, note });
  }

  getInterviews(tab = 'upcoming') {
    return this.http.get<Interview[]>(`${this.base}/interviews`, { params: { tab } });
  }

  scheduleInterview(payload: ScheduleInterviewPayload) {
    return this.http.post<Interview>(`${this.base}/interviews`, payload);
  }

  recordInterviewResult(id: number, result: string, notes?: string) {
    return this.http.patch(`${this.base}/interviews/${id}/result`, { result, notes });
  }

  cancelInterview(id: number) {
    return this.http.delete(`${this.base}/interviews/${id}`);
  }
}
