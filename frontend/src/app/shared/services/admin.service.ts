import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { withApi } from '../config/api.config';
import { ResultResponse } from '../models/result-response.model';
import { AdminUserModel } from '../models/admin-user.model';
import { AdminAppointmentModel } from '../models/admin-appointment.model';
import { AdminStatsModel } from '../models/admin-stats.model';

@Injectable({
  providedIn: 'root'
})
export class AdminService {
  private readonly http: HttpClient = inject(HttpClient);
  private readonly API_URL: string = withApi('/api/admin');

  getUsers(): Observable<ResultResponse<AdminUserModel[]>> {
    return this.http.get<ResultResponse<AdminUserModel[]>>(`${this.API_URL}/users`);
  }

  setUserActivation(userId: number, isActive: boolean): Observable<ResultResponse<boolean>> {
    return this.http.put<ResultResponse<boolean>>(`${this.API_URL}/users/${userId}/activation`, { isActive });
  }

  getAppointments(): Observable<ResultResponse<AdminAppointmentModel[]>> {
    return this.http.get<ResultResponse<AdminAppointmentModel[]>>(`${this.API_URL}/appointments`);
  }

  getStats(fromUtc?: string, toUtc?: string): Observable<ResultResponse<AdminStatsModel>> {
    let params: HttpParams = new HttpParams();

    if (fromUtc && fromUtc.trim()) {
      params = params.set('fromUtc', fromUtc);
    }

    if (toUtc && toUtc.trim()) {
      params = params.set('toUtc', toUtc);
    }

    return this.http.get<ResultResponse<AdminStatsModel>>(`${this.API_URL}/stats`, { params });
  }
}
