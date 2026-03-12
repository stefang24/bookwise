import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class LocalStorageService {
  private readonly TOKEN_KEY: string = 'bookwise_token';
  private readonly USERNAME_KEY: string = 'bookwise_username';
  private readonly ROLE_KEY: string = 'bookwise_role';
  private readonly USER_ID_KEY: string = 'bookwise_user_id';

  getToken(): string | null {
    return localStorage.getItem(this.TOKEN_KEY);
  }

  setToken(token: string): void {
    localStorage.setItem(this.TOKEN_KEY, token);
  }

  removeToken(): void {
    localStorage.removeItem(this.TOKEN_KEY);
  }

  getUsername(): string {
    return localStorage.getItem(this.USERNAME_KEY) ?? '';
  }

  setUsername(username: string): void {
    localStorage.setItem(this.USERNAME_KEY, username);
  }

  removeUsername(): void {
    localStorage.removeItem(this.USERNAME_KEY);
  }

  getRole(): string {
    return localStorage.getItem(this.ROLE_KEY) ?? '';
  }

  setRole(role: string): void {
    localStorage.setItem(this.ROLE_KEY, role);
  }

  removeRole(): void {
    localStorage.removeItem(this.ROLE_KEY);
  }

  getUserId(): number | null {
    const id: string | null = localStorage.getItem(this.USER_ID_KEY);
    return id ? parseInt(id, 10) : null;
  }

  setUserId(id: number): void {
    localStorage.setItem(this.USER_ID_KEY, id.toString());
  }

  removeUserId(): void {
    localStorage.removeItem(this.USER_ID_KEY);
  }

  clear(): void {
    this.removeToken();
    this.removeUsername();
    this.removeRole();
    this.removeUserId();
  }
}
