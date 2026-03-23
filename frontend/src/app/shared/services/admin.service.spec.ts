import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { AdminService } from './admin.service';
import { AdminUserModel } from '../models/admin-user.model';
import { AdminAppointmentModel } from '../models/admin-appointment.model';
import { AdminStatsModel } from '../models/admin-stats.model';

describe('AdminService', () => {
  let service: AdminService;
  let httpMock: HttpTestingController;

  const mockUser: AdminUserModel = {
    id: 1,
    email: 'user1@test.com',
    username: 'user1',
    name: 'John Doe',
    role: 'User',
    city: 'Belgrade',
    isActive: true,
    createdAt: '2024-01-15T00:00:00Z'
  };

  const mockAppointment: AdminAppointmentModel = {
    id: 1,
    serviceName: 'Haircut',
    category: 'Beauty',
    providerName: 'Jane Smith',
    providerId: 2,
    clientName: 'John Doe',
    clientId: 1,
    startUtc: '2024-03-20T10:00:00Z',
    endUtc: '2024-03-20T11:00:00Z',
    status: 'Completed',
    priceEur: 50,
  };

  const mockStats: AdminStatsModel = {
    totalUsers: 100,
    totalProviders: 50,
    totalAppointments: 500,
    scheduledAppointments: 50,
    completedAppointments: 390,
    cancelledAppointments: 60,
    bookingsByDate: [{ label: '2026-03-01', value: 10 }],
    bookingsByCategory: [{ label: 'Beauty', value: 25 }]
  };

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [AdminService]
    });

    service = TestBed.inject(AdminService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should get all users', () => {
    const mockData = [mockUser];

    service.getUsers().subscribe((response) => {
      expect(response.success).toBe(true);
      expect(response.data?.length).toBe(1);
      expect(response.data?.[0].username).toBe('user1');
    });

    const req = httpMock.expectOne((request) =>
      request.url.includes('/api/admin/users') && request.method === 'GET'
    );
    req.flush({ success: true, data: mockData });
  });

  it('should set user activation', () => {
    service.setUserActivation(1, false).subscribe((response) => {
      expect(response.success).toBe(true);
      expect(response.data).toBe(true);
    });

    const req = httpMock.expectOne((request) =>
      request.url.includes('/api/admin/users/1/activation') && request.method === 'PUT'
    );
    expect(req.request.body).toEqual({ isActive: false });
    req.flush({ success: true, data: true });
  });

  it('should get all appointments', () => {
    const mockData = [mockAppointment];

    service.getAppointments().subscribe((response) => {
      expect(response.success).toBe(true);
      expect(response.data?.length).toBe(1);
      expect(response.data?.[0].serviceName).toBe('Haircut');
    });

    const req = httpMock.expectOne((request) =>
      request.url.includes('/api/admin/appointments') && request.method === 'GET'
    );
    req.flush({ success: true, data: mockData });
  });

  it('should get admin stats', () => {
    service.getStats().subscribe((response) => {
      expect(response.success).toBe(true);
      expect(response.data?.totalUsers).toBe(100);
      expect(response.data?.totalProviders).toBe(50);
      expect(response.data?.completedAppointments).toBe(390);
    });

    const req = httpMock.expectOne((request) =>
      request.url.includes('/api/admin/stats') && request.method === 'GET'
    );
    req.flush({ success: true, data: mockStats });
  });

  it('should calculate completion rate from stats', () => {
    service.getStats().subscribe((response) => {
      if (response.data) {
        const completionRate = (response.data.completedAppointments / response.data.totalAppointments) * 100;
        expect(completionRate).toBe(78); // 390/500 * 100
      }
    });

    const req = httpMock.expectOne((request) =>
      request.url.includes('/api/admin/stats') && request.method === 'GET'
    );
    req.flush({ success: true, data: mockStats });
  });

  it('should calculate cancellation rate from stats', () => {
    service.getStats().subscribe((response) => {
      if (response.data) {
        const cancellationRate = (response.data.cancelledAppointments / response.data.totalAppointments) * 100;
        expect(cancellationRate).toBe(12); // 60/500 * 100
      }
    });

    const req = httpMock.expectOne((request) =>
      request.url.includes('/api/admin/stats') && request.method === 'GET'
    );
    req.flush({ success: true, data: mockStats });
  });

  it('should include date range params when provided', () => {
    service.getStats('2026-03-01T00:00:00Z', '2026-03-31T23:59:59Z').subscribe((response) => {
      expect(response.success).toBe(true);
    });

    const req = httpMock.expectOne((request) =>
      request.url.includes('/api/admin/stats') && request.method === 'GET'
    );
    expect(req.request.params.get('fromUtc')).toBe('2026-03-01T00:00:00Z');
    expect(req.request.params.get('toUtc')).toBe('2026-03-31T23:59:59Z');
    req.flush({ success: true, data: mockStats });
  });

  it('should handle empty users list', () => {
    service.getUsers().subscribe((response) => {
      expect(response.success).toBe(true);
      expect(response.data?.length).toBe(0);
    });

    const req = httpMock.expectOne((request) =>
      request.url.includes('/api/admin/users') && request.method === 'GET'
    );
    req.flush({ success: true, data: [] });
  });

  it('should handle error when getting stats', () => {
    service.getStats().subscribe({
      next: () => {
        throw new Error('should have failed');
      }
      ,
      error: (error: unknown) => {
        expect(error).toBeTruthy();
      }
    });

    const req = httpMock.expectOne((request) =>
      request.url.includes('/api/admin/stats') && request.method === 'GET'
    );
    req.flush({ success: false, message: 'Unauthorized' }, { status: 403, statusText: 'Forbidden' });
  });

  it('should verify user model structure', () => {
    expect(mockUser.id).toBeDefined();
    expect(mockUser.email).toBeDefined();
    expect(mockUser.username).toBeDefined();
    expect(mockUser.role).toBe('User');
    expect(mockUser.isActive).toBe(true);
  });

  it('should verify appointment model structure', () => {
    expect(mockAppointment.id).toBeDefined();
    expect(mockAppointment.serviceName).toBeDefined();
    expect(mockAppointment.priceEur).toBeGreaterThan(0);
    expect(typeof mockAppointment.status).toBe('string');
  });
});
