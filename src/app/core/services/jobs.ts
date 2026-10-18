import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map, Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiEnvelope, Job, JobPayload } from '../models';

@Injectable({ providedIn: 'root' })
export class JobsService {
  private readonly http = inject(HttpClient);
  private readonly base = `${environment.apiUrl}/Job`;

  getAll(): Observable<Job[]> {
    return this.http.get<Job[]>(this.base);
  }

  getById(id: number): Observable<Job> {
    return this.http
      .get<ApiEnvelope<Job>>(`${this.base}/${id}`)
      .pipe(map((response) => (response.value ?? response) as Job));
  }

  create(payload: JobPayload) {
    return this.http.post<Job>(this.base, payload);
  }

  update(id: number, payload: JobPayload) {
    return this.http.put<void>(`${this.base}/${id}`, payload);
  }

  toggle(id: number) {
    return this.http.patch<void>(`${this.base}/${id}/toggle-status`, {});
  }

  delete(id: number) {
    return this.http.delete<void>(`${this.base}/${id}`);
  }
}
