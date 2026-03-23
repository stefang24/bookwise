import { ComponentFixture, TestBed } from '@angular/core/testing';
import { signal } from '@angular/core';
import { of } from 'rxjs';
import { RouterTestingModule } from '@angular/router/testing';
import { HomePage } from './home.page';
import { ProviderCatalogService } from '../../../../shared/services/provider-catalog.service';
import { AdminService } from '../../../../shared/services/admin.service';
import { AuthService } from '../../../../shared/services/auth.service';
import { ProviderServiceModel } from '../../../../shared/models/provider-service.model';
import { ProviderDirectoryItemModel } from '../../../../shared/models/provider-directory-item.model';
import { AdminStatsModel } from '../../../../shared/models/admin-stats.model';

describe('HomePage', () => {
  let component: HomePage;
  let fixture: ComponentFixture<HomePage>;
  const role = signal('User');

  const mockService: ProviderServiceModel = {
    id: 1,
    providerId: 100,
    providerName: 'Test Provider',
    providerImagePath: '/images/provider.jpg',
    providerCity: 'Belgrade',
    name: 'Test Service',
    category: 'Beauty',
    description: 'A test service',
    imageUrl: '/images/service.jpg',
    priceEur: 50,
    durationMinutes: 60,
    isActive: true
  };

  const mockProvider: ProviderDirectoryItemModel = {
    id: 100,
    name: 'Test Provider',
    username: 'test_provider',
    profileImagePath: '/images/provider.jpg',
    primaryCategory: 'Beauty',
    city: 'Belgrade',
    address: 'Test Street 1',
    servicesCount: 5
  };

  const mockStats: AdminStatsModel = {
    totalUsers: 10,
    totalProviders: 5,
    totalAppointments: 30,
    scheduledAppointments: 2,
    completedAppointments: 25,
    cancelledAppointments: 3,
    bookingsByDate: [],
    bookingsByCategory: []
  };

  beforeEach(async () => {
    const catalogStub = {
      search: () => of({ success: true, data: [mockService] }),
      getProviders: () => of({ success: true, data: [mockProvider] })
    };

    const adminStub = {
      getStats: () => of({ success: true, data: mockStats })
    };

    const authStub = {
      role: () => role()
    };

    await TestBed.configureTestingModule({
      imports: [HomePage, RouterTestingModule],
      providers: [
        { provide: ProviderCatalogService, useValue: catalogStub },
        { provide: AdminService, useValue: adminStub },
        { provide: AuthService, useValue: authStub }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(HomePage);
    component = fixture.componentInstance;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should load featured data', () => {
    component.ngOnInit();

    expect(component.featuredServices().length).toBe(1);
    expect(component.topProviders().length).toBe(1);
  });

  it('should load admin preview when role is admin', () => {
    role.set('Admin');
    component.ngOnInit();

    expect(component.isAdmin()).toBe(true);
    expect(component.adminPreviewStats()?.totalUsers).toBe(10);
  });
});
