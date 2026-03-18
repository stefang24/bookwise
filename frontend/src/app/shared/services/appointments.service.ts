import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ResultResponse } from '../models/result-response.model';
import { AvailableSlot } from '../models/available-slot.model';
import { CreateAppointmentRequest } from '../models/create-appointment-request.model';
import { AppointmentModel } from '../models/appointment.model';
import { withApi } from '../config/api.config';
import { CalendarSlotModel } from '../models/calendar-slot.model';

@Injectable({
  providedIn: 'root'
})
export class AppointmentsService {
  private http: HttpClient = inject(HttpClient);
  private readonly API_URL: string = withApi('/api/appointments');

  getAvailableSlots(providerServiceId: number, date: string): Observable<ResultResponse<AvailableSlot[]>> {
    const params: HttpParams = new HttpParams()
      .set('providerServiceId', providerServiceId)
      .set('date', date);

    return this.http.get<ResultResponse<AvailableSlot[]>>(`${this.API_URL}/available-slots`, { params });
  }

  getCalendarSlots(providerServiceId: number, date: string): Observable<ResultResponse<CalendarSlotModel[]>> {
    const params: HttpParams = new HttpParams()
      .set('providerServiceId', providerServiceId)
      .set('date', date);

    return this.http.get<ResultResponse<CalendarSlotModel[]>>(`${this.API_URL}/calendar-slots`, { params });
  }

  create(request: CreateAppointmentRequest): Observable<ResultResponse<AppointmentModel>> {
    return this.http.post<ResultResponse<AppointmentModel>>(this.API_URL, request);
  }

  getMyHistory(): Observable<ResultResponse<AppointmentModel[]>> {
    return this.http.get<ResultResponse<AppointmentModel[]>>(`${this.API_URL}/my-history`);
  }

  cancel(id: number): Observable<ResultResponse<boolean>> {
    return this.http.post<ResultResponse<boolean>>(`${this.API_URL}/${id}/cancel`, {});
  }
}
