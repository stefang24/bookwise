import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ResultResponse } from '../models/result-response.model';
import { ProviderWorkingHour } from '../models/provider-working-hour.model';
import { UpdateWorkingHoursRequest } from '../models/update-working-hours-request.model';
import { withApi } from '../config/api.config';

@Injectable({
  providedIn: 'root'
})
export class WorkingHoursService {
  private http: HttpClient = inject(HttpClient);
  private readonly API_URL: string = withApi('/api/working-hours');

  getMy(): Observable<ResultResponse<ProviderWorkingHour[]>> {
    return this.http.get<ResultResponse<ProviderWorkingHour[]>>(`${this.API_URL}/my`);
  }

  updateMy(request: UpdateWorkingHoursRequest): Observable<ResultResponse<ProviderWorkingHour[]>> {
    return this.http.put<ResultResponse<ProviderWorkingHour[]>>(`${this.API_URL}/my`, request);
  }

  getByProvider(providerId: number): Observable<ResultResponse<ProviderWorkingHour[]>> {
    return this.http.get<ResultResponse<ProviderWorkingHour[]>>(`${this.API_URL}/provider/${providerId}`);
  }
}
