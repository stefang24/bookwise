import { inject, Injectable, signal, computed } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';
import { ResultResponse } from '../models/result-response.model';
import { AuthResponse } from '../models/auth-response.model';
import { LoginRequest } from '../models/login-request.model';
import { RegisterRequest } from '../models/register-request.model';
import { LocalStorageService } from './local-storage.service';
import { jwtDecode } from 'jwt-decode';
import { withApi } from '../config/api.config';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private http: HttpClient = inject(HttpClient);
  private router: Router = inject(Router);
  private localStorageService: LocalStorageService = inject(LocalStorageService);

  private readonly API_URL: string = withApi('/api/auth');

  private _isLoggedIn = signal<boolean>(!!this.localStorageService.getToken());
  private _username = signal<string>(this.localStorageService.getUsername());
  private _role = signal<string>(this.localStorageService.getRole());
  private _userId = signal<number | null>(this.localStorageService.getUserId());

  isLoggedIn = computed<boolean>(() => this._isLoggedIn());
  username = computed<string>(() => this._username());
  role = computed<string>(() => this._role());
  userId = computed<number | null>(() => this._userId());

  login(request: LoginRequest): Observable<ResultResponse<AuthResponse>> {
    return this.http.post<ResultResponse<AuthResponse>>(`${this.API_URL}/login`, request);
  }

  register(request: RegisterRequest): Observable<ResultResponse<AuthResponse>> {
    return this.http.post<ResultResponse<AuthResponse>>(`${this.API_URL}/register`, request);
  }

  setSession(response: AuthResponse): void {
    this.localStorageService.setToken(response.token);
    this.localStorageService.setUsername(response.username);
    this.localStorageService.setRole(response.role);

    const userId: number | null = this.extractUserIdFromToken(response.token);
    if (userId !== null) {
      this.localStorageService.setUserId(userId);
    }

    this._isLoggedIn.set(true);
    this._username.set(response.username);
    this._role.set(response.role);
    this._userId.set(userId);
  }

  logout(): void {
    this.localStorageService.clear();
    this._isLoggedIn.set(false);
    this._username.set('');
    this._role.set('');
    this._userId.set(null);
    this.router.navigate(['/login']);
  }

  private extractUserIdFromToken(token: string): number | null {
    try {
      const decoded: Record<string, string> = jwtDecode(token);
      const id: string | undefined =
        decoded['nameid'] ??
        decoded['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'] ??
        decoded['sub'];
      return id ? parseInt(id, 10) : null;
    } catch {
      return null;
    }
  }
}
