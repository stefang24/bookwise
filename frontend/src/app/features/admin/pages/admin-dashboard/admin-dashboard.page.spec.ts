import { ComponentFixture, TestBed } from '@angular/core/testing';
import { of } from 'rxjs';
import { AdminDashboardPage } from './admin-dashboard.page';
import { AdminService } from '../../../../shared/services/admin.service';
import { NotificationService } from '../../../../shared/services/notification.service';
import { Router } from '@angular/router';
import { AdminUserModel } from '../../../../shared/models/admin-user.model';
import { AdminAppointmentModel } from '../../../../shared/models/admin-appointment.model';
import { AdminStatsModel } from '../../../../shared/models/admin-stats.model';

describe('AdminDashboardPage', () => {
  let component: AdminDashboardPage;
  let fixture: ComponentFixture<AdminDashboardPage>;

  const mockUsers: AdminUserModel[] = [
    {
      id: 1,
      email: 'user1@test.com',
      username: 'user1',
      name: 'John Doe',
      role: 'User',
      city: 'Belgrade',
      isActive: true,
      createdAt: '2026-03-01T10:00:00Z'
    },
    {
      id: 2,
      email: 'provider1@test.com',
      username: 'provider1',
      name: 'Provider One',
      role: 'Provider',
      city: 'Novi Sad',
      isActive: true,
      createdAt: '2026-03-02T10:00:00Z'
    }
  ];

  const mockAppointments: AdminAppointmentModel[] = [
    {
      id: 1,
      serviceName: 'Service A',
      category: 'Beauty',
      providerName: 'Provider One',
      providerId: 2,
      clientName: 'John Doe',
      clientId: 1,
      startUtc: '2026-03-03T10:00:00Z',
      endUtc: '2026-03-03T11:00:00Z',
      status: 'Completed',
      priceEur: 50
    }
  ];

  const mockStats: AdminStatsModel = {
    totalUsers: 10,
    totalProviders: 4,
    totalAppointments: 7,
    scheduledAppointments: 2,
    completedAppointments: 4,
    cancelledAppointments: 1,
    bookingsByDate: [{ label: '2026-03-03', value: 3 }],
    bookingsByCategory: [{ label: 'Beauty', value: 2 }]
  };

  beforeEach(async () => {
    const adminStub = {
      getUsers: () => of({ success: true, data: mockUsers }),
      getAppointments: () => of({ success: true, data: mockAppointments }),
      getStats: () => of({ success: true, data: mockStats }),
      setUserActivation: () => of({ success: true, data: true })
    };

    const notificationStub = {
      success: () => undefined,
      error: () => undefined
    };

    const routerStub = {
      navigate: () => Promise.resolve(true)
    };

    await TestBed.configureTestingModule({
      imports: [AdminDashboardPage],
      providers: [
        { provide: AdminService, useValue: adminStub },
        { provide: NotificationService, useValue: notificationStub },
        { provide: Router, useValue: routerStub }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(AdminDashboardPage);
    component = fixture.componentInstance;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should load data on init', () => {
    component.ngOnInit();

    expect(component.users().length).toBe(2);
    expect(component.appointments().length).toBe(1);
    expect(component.stats()?.totalUsers).toBe(10);
  });

  it('should compute filtered users', () => {
    component.ngOnInit();
    component.userSearch.set('john');

    const filtered = component.filteredUsers();
    expect(filtered.length).toBe(1);
    expect(filtered[0].username).toBe('user1');
  });
});
