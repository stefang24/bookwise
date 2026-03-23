import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { Router } from '@angular/router';
import { AuthService } from './auth.service';
import { LocalStorageService } from './local-storage.service';
import { LoginRequest } from '../models/login-request.model';
import { RegisterRequest } from '../models/register-request.model';
import { AuthResponse } from '../models/auth-response.model';

describe('AuthService', () => {
  let service: AuthService;
  let httpMock: HttpTestingController;

  let token: string | null;
  let username: string;
  let role: string;
  let userId: number | null;
  let clearCalled: boolean;
  let navigatedTo: string[] | null;

  beforeEach(() => {
    token = null;
    username = '';
    role = '';
    userId = null;
    clearCalled = false;
    navigatedTo = null;

    const localStorageStub: Partial<LocalStorageService> = {
      setToken: (value: string) => {
        token = value;
      },
      setUsername: (value: string) => {
        username = value;
      },
      setRole: (value: string) => {
        role = value;
      },
      setUserId: (value: number) => {
        userId = value;
      },
      getToken: () => token,
      getUsername: () => username,
      getRole: () => role,
      getUserId: () => userId,
      clear: () => {
        clearCalled = true;
        token = null;
        username = '';
        role = '';
        userId = null;
      }
    };

    const routerStub: Partial<Router> = {
      navigate: (commands: string[]) => {
        navigatedTo = commands;
        return Promise.resolve(true);
      }
    };

    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [
        AuthService,
        { provide: LocalStorageService, useValue: localStorageStub },
        { provide: Router, useValue: routerStub }
      ]
    });

    service = TestBed.inject(AuthService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should login successfully', () => {
    const loginRequest: LoginRequest = { email: 'test@test.com', password: 'password123' };
    const mockResponse: AuthResponse = {
      token: 'test-token-123',
      email: 'test@test.com',
      username: 'testuser',
      role: 'User'
    };

    service.login(loginRequest).subscribe((response) => {
      expect(response.success).toBe(true);
      expect(response.data?.token).toBe('test-token-123');
    });

    const req = httpMock.expectOne((request) =>
      request.url.includes('/api/auth/login') && request.method === 'POST'
    );
    req.flush({ success: true, data: mockResponse });
  });

  it('should register successfully', () => {
    const registerRequest: RegisterRequest = {
      email: 'newuser@test.com',
      username: 'newuser',
      password: 'password123',
      firstName: 'John',
      lastName: 'Doe',
      role: 'User',
      companyName: undefined,
      city: undefined
    };

    service.register(registerRequest).subscribe((response) => {
      expect(response.success).toBe(true);
    });

    const req = httpMock.expectOne((request) =>
      request.url.includes('/api/auth/register') && request.method === 'POST'
    );
    req.flush({ success: true, data: { token: 't', email: registerRequest.email, username: registerRequest.username, role: registerRequest.role } });
  });

  it('should set session values', () => {
    service.setSession({
      token: 'invalid.jwt.token',
      email: 'test@test.com',
      username: 'tester',
      role: 'Provider'
    });

    expect(token).toBe('invalid.jwt.token');
    expect(username).toBe('tester');
    expect(role).toBe('Provider');
    expect(service.isLoggedIn()).toBe(true);
  });

  it('should logout successfully', () => {
    service.logout();

    expect(clearCalled).toBe(true);
    expect(navigatedTo).toEqual(['/login']);
    expect(service.isLoggedIn()).toBe(false);
  });

  it('should handle login error', () => {
    const loginRequest: LoginRequest = { email: 'test@test.com', password: 'wrongpassword' };

    service.login(loginRequest).subscribe({
      next: () => {
        throw new Error('should have failed');
      },
      error: (error: unknown) => {
        expect(error).toBeTruthy();
      }
    });

    const req = httpMock.expectOne((request) =>
      request.url.includes('/api/auth/login') && request.method === 'POST'
    );
    req.flush({ success: false, message: 'Invalid credentials' }, { status: 401, statusText: 'Unauthorized' });
  });
});
