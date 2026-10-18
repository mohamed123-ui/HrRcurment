import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiEnvelope, Application, CandidateProfile, UpsertCandidateProfileDto } from '../models';

@Injectable({ providedIn: 'root' })
export class CandidateApiService {
  private http = inject(HttpClient);
  private base = `${environment.apiUrl}/Candidate`;
  getProfile() {
    return this.http.get<ApiEnvelope<CandidateProfile>>(`${this.base}/profile`).pipe(
      map((response) => {
        if (response.isSuccess === false || !response.value) {
          throw new Error('Candidate profile does not exist.');
        }

        return response.value;
      }),
    );
  }
  saveProfile(dto: UpsertCandidateProfileDto) {
    return this.http.post(`${this.base}/profile`, dto);
  }
  uploadCv(file: File) {
    const form = new FormData();
    form.append('file', file);
    return this.http.post(`${this.base}/upload-cv`, form);
  }
  apply(jobId: number, coverLetter = '') {
    return this.http.post(`${this.base}/apply-job`, { jobId, coverLetter });
  }
  getApplications() {
    return this.http.get<Application[]>(`${this.base}/my-applications`);
  }
}
