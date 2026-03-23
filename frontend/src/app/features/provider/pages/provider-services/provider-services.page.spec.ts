import { ComponentFixture, TestBed } from '@angular/core/testing';
import { signal } from '@angular/core';
import { of } from 'rxjs';
import { ActivatedRoute, Router } from '@angular/router';
import { ProviderServicesPage } from './provider-services.page';
import { ProviderCatalogService } from '../../../../shared/services/provider-catalog.service';
import { AuthService } from '../../../../shared/services/auth.service';
import { NotificationService } from '../../../../shared/services/notification.service';
import { ProviderServiceModel } from '../../../../shared/models/provider-service.model';

describe('ProviderServicesPage', () => {
  let component: ProviderServicesPage;
  let fixture: ComponentFixture<ProviderServicesPage>;
  const role = signal('Provider');

  const services: ProviderServiceModel[] = [
    {
      id: 1,
      providerId: 10,
      providerName: 'Test Provider',
      providerImagePath: '/images/provider.jpg',
      providerCity: 'Belgrade',
      name: 'Haircut',
      category: 'Beauty',
      description: 'Professional haircut',
      imageUrl: '/images/service.jpg',
      priceEur: 30,
      durationMinutes: 45,
      isActive: true
    }
  ];

  beforeEach(async () => {
    const catalogStub = {
      getMyServices: () => of({ success: true, data: services }),
      getCategories: () => of({ success: true, data: ['Beauty', 'Fitness'] }),
      create: () => of({ success: true, data: services[0] }),
      update: () => of({ success: true, data: services[0] }),
      remove: () => of({ success: true, data: true }),
      uploadImage: () => of({ success: true, data: '/images/services/test.jpg' })
    };

    const authStub = {
      role: () => role()
    };

    const notificationStub = {
      success: () => undefined,
      error: () => undefined
    };

    const routeStub = {
      queryParams: of({})
    };

    const routerStub = {
      navigate: () => Promise.resolve(true)
    };

    await TestBed.configureTestingModule({
      imports: [ProviderServicesPage],
      providers: [
        { provide: ProviderCatalogService, useValue: catalogStub },
        { provide: AuthService, useValue: authStub },
        { provide: NotificationService, useValue: notificationStub },
        { provide: ActivatedRoute, useValue: routeStub },
        { provide: Router, useValue: routerStub }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(ProviderServicesPage);
    component = fixture.componentInstance;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should load services from constructor flow', () => {
    expect(component.items().length).toBe(1);
    expect(component.items()[0].name).toBe('Haircut');
  });

  it('should filter by search query', () => {
    component.searchQuery.set('hair');
    const filtered = component.filteredItems();

    expect(filtered.length).toBe(1);
    expect(filtered[0].name).toBe('Haircut');
  });

  it('should expose provider role', () => {
    expect(component.isProvider).toBe(true);
    role.set('User');
    expect(component.isProvider).toBe(false);
  });
});
