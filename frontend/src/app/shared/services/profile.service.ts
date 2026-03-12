import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ResultResponse } from '../models/result-response.model';
import { ProfileResponse } from '../models/profile-response.model';
import { UpdateProfileRequest } from '../models/update-profile-request.model';

@Injectable({
  providedIn: 'root'
})
export class ProfileService {
  private http: HttpClient = inject(HttpClient);

  private readonly API_URL: string = 'http://localhost:5227/api/profile';

  getProfile(userId: number): Observable<ResultResponse<ProfileResponse>> {
    return this.http.get<ResultResponse<ProfileResponse>>(`${this.API_URL}/${userId}`);
  }

  updateProfile(request: UpdateProfileRequest): Observable<ResultResponse<ProfileResponse>> {
    return this.http.put<ResultResponse<ProfileResponse>>(this.API_URL, request);
  }

  uploadProfileImage(file: File): Observable<ResultResponse<ProfileResponse>> {
    const formData: FormData = new FormData();
    formData.append('file', file);
    return this.http.post<ResultResponse<ProfileResponse>>(`${this.API_URL}/upload-image`, formData);
  }
}
