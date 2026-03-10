import { inject, Injectable, signal, computed } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';
import { ResultResponse } from '../models/result-response.model';
import { AuthResponse } from '../models/auth-response.model';
import { LoginRequest } from '../models/login-request.model';
import { RegisterRequest } from '../models/register-request.model';
import { LocalStorageService } from './local-storage.service';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private http: HttpClient = inject(HttpClient);
  private router: Router = inject(Router);
  private localStorageService: LocalStorageService = inject(LocalStorageService);

  private readonly API_URL: string = 'http://localhost:5227/api/auth';

  private _isLoggedIn = signal<boolean>(!!this.localStorageService.getToken());
  private _username = signal<string>(this.localStorageService.getUsername());

  isLoggedIn = computed<boolean>(() => this._isLoggedIn());
  username = computed<string>(() => this._username());

  login(request: LoginRequest): Observable<ResultResponse<AuthResponse>> {
    return this.http.post<ResultResponse<AuthResponse>>(`${this.API_URL}/login`, request);
  }

  register(request: RegisterRequest): Observable<ResultResponse<AuthResponse>> {
    return this.http.post<ResultResponse<AuthResponse>>(`${this.API_URL}/register`, request);
  }

  setSession(response: AuthResponse): void {
    this.localStorageService.setToken(response.token);
    this.localStorageService.setUsername(response.username);
    this._isLoggedIn.set(true);
    this._username.set(response.username);
  }

  logout(): void {
    this.localStorageService.clear();
    this._isLoggedIn.set(false);
    this._username.set('');
    this.router.navigate(['/login']);
  }
}
