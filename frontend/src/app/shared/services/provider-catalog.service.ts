import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ResultResponse } from '../models/result-response.model';
import { ProviderServiceModel } from '../models/provider-service.model';
import { ProviderServiceRequest } from '../models/provider-service-request.model';
import { withApi } from '../config/api.config';
import { ProviderDirectoryItemModel } from '../models/provider-directory-item.model';

@Injectable({
  providedIn: 'root'
})
export class ProviderCatalogService {
  private http: HttpClient = inject(HttpClient);
  private readonly API_URL: string = withApi('/api/provider-services');

  getMyServices(): Observable<ResultResponse<ProviderServiceModel[]>> {
    return this.http.get<ResultResponse<ProviderServiceModel[]>>(`${this.API_URL}/my`);
  }

  create(request: ProviderServiceRequest): Observable<ResultResponse<ProviderServiceModel>> {
    return this.http.post<ResultResponse<ProviderServiceModel>>(this.API_URL, request);
  }

  update(id: number, request: ProviderServiceRequest): Observable<ResultResponse<ProviderServiceModel>> {
    return this.http.put<ResultResponse<ProviderServiceModel>>(`${this.API_URL}/${id}`, request);
  }

  remove(id: number): Observable<ResultResponse<boolean>> {
    return this.http.delete<ResultResponse<boolean>>(`${this.API_URL}/${id}`);
  }

  uploadImage(file: File): Observable<ResultResponse<string>> {
    const formData: FormData = new FormData();
    formData.append('file', file);
    return this.http.post<ResultResponse<string>>(`${this.API_URL}/upload-image`, formData);
  }

  search(category: string, query: string): Observable<ResultResponse<ProviderServiceModel[]>> {
    let params: HttpParams = new HttpParams();
    if (category.trim()) params = params.set('category', category.trim());
    if (query.trim()) params = params.set('query', query.trim());
    return this.http.get<ResultResponse<ProviderServiceModel[]>>(`${this.API_URL}/search`, { params });
  }

  getFeaturedServices(limit: number = 3): Observable<ResultResponse<ProviderServiceModel[]>> {
    const params: HttpParams = new HttpParams().set('limit', String(limit));
    return this.http.get<ResultResponse<ProviderServiceModel[]>>(`${this.API_URL}/featured-services`, { params });
  }

  getByProvider(providerId: number): Observable<ResultResponse<ProviderServiceModel[]>> {
    return this.http.get<ResultResponse<ProviderServiceModel[]>>(`${this.API_URL}/provider/${providerId}`);
  }

  getCategories(): Observable<ResultResponse<string[]>> {
    return this.http.get<ResultResponse<string[]>>(`${this.API_URL}/categories`);
  }

  getProviders(category: string, city: string, query: string, sortBy: string): Observable<ResultResponse<ProviderDirectoryItemModel[]>> {
    let params: HttpParams = new HttpParams();
    if (category.trim()) params = params.set('category', category.trim());
    if (city.trim()) params = params.set('city', city.trim());
    if (query.trim()) params = params.set('query', query.trim());
    if (sortBy.trim()) params = params.set('sortBy', sortBy.trim());
    return this.http.get<ResultResponse<ProviderDirectoryItemModel[]>>(`${this.API_URL}/providers`, { params });
  }

  getTopProviders(limit: number = 3): Observable<ResultResponse<ProviderDirectoryItemModel[]>> {
    const params: HttpParams = new HttpParams().set('limit', String(limit));
    return this.http.get<ResultResponse<ProviderDirectoryItemModel[]>>(`${this.API_URL}/top-providers`, { params });
  }
}
