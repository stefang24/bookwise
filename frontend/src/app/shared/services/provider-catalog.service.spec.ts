import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { ProviderCatalogService } from './provider-catalog.service';
import { ProviderServiceModel } from '../models/provider-service.model';
import { ProviderServiceRequest } from '../models/provider-service-request.model';
import { ProviderDirectoryItemModel } from '../models/provider-directory-item.model';

describe('ProviderCatalogService', () => {
  let service: ProviderCatalogService;
  let httpMock: HttpTestingController;

  const mockServiceModel: ProviderServiceModel = {
    id: 1,
    providerId: 10,
    providerName: 'Test Provider',
    providerImagePath: '/images/provider.jpg',
    providerCity: 'Belgrade',
    name: 'Test Service',
    category: 'Beauty',
    description: 'Test service',
    imageUrl: '/images/service.jpg',
    priceEur: 50,
    durationMinutes: 60,
    isActive: true
  };

  const mockProviderDirectory: ProviderDirectoryItemModel = {
    id: 10,
    name: 'Test Provider',
    username: 'test_provider',
    profileImagePath: '/images/provider.jpg',
    primaryCategory: 'Beauty',
    city: 'Belgrade',
    address: 'Test Street 1',
    servicesCount: 5
  };

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [ProviderCatalogService]
    });

    service = TestBed.inject(ProviderCatalogService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should get my services', () => {
    const mockData = [mockServiceModel];

    service.getMyServices().subscribe((response) => {
      expect(response.success).toBe(true);
      expect(response.data?.length).toBe(1);
      expect(response.data?.[0].name).toBe('Test Service');
    });

    const req = httpMock.expectOne((request) =>
      request.url.includes('/api/provider-services/my') && request.method === 'GET'
    );
    req.flush({ success: true, data: mockData });
  });

  it('should create a service', () => {
    const newService: ProviderServiceRequest = {
      name: 'New Service',
      category: 'Beauty',
      description: 'New service',
      imageUrl: null,
      priceEur: 60,
      durationMinutes: 45
    };

    service.create(newService).subscribe((response) => {
      expect(response.success).toBe(true);
      expect(response.data?.name).toBe('New Service');
    });

    const req = httpMock.expectOne((request) =>
      request.url.includes('/api/provider-services') && request.method === 'POST'
    );
    req.flush({ success: true, data: { ...newService, id: 2, providerId: 10, providerName: 'Test', providerImagePath: null, providerCity: 'Belgrade', imageUrl: null, isActive: true } });
  });

  it('should update a service', () => {
    const updateService: ProviderServiceRequest = {
      name: 'Updated Service',
      category: 'Beauty',
      description: 'Updated',
      imageUrl: null,
      priceEur: 70,
      durationMinutes: 50
    };

    service.update(1, updateService).subscribe((response) => {
      expect(response.success).toBe(true);
      expect(response.data?.name).toBe('Updated Service');
    });

    const req = httpMock.expectOne((request) =>
      request.url.includes('/api/provider-services/1') && request.method === 'PUT'
    );
    req.flush({ success: true, data: { ...updateService, id: 1, providerId: 10, providerName: 'Test', providerImagePath: null, providerCity: 'Belgrade', imageUrl: null, isActive: true } });
  });

  it('should delete a service', () => {
    service.remove(1).subscribe((response) => {
      expect(response.success).toBe(true);
      expect(response.data).toBe(true);
    });

    const req = httpMock.expectOne((request) =>
      request.url.includes('/api/provider-services/1') && request.method === 'DELETE'
    );
    req.flush({ success: true, data: true });
  });

  it('should search services by category and query', () => {
    const mockData = [mockServiceModel];

    service.search('Beauty', 'haircut').subscribe((response) => {
      expect(response.success).toBe(true);
      expect(response.data?.length).toBe(1);
    });

    const req = httpMock.expectOne((request) =>
      request.url.includes('/api/provider-services/search') && request.method === 'GET'
    );
    req.flush({ success: true, data: mockData });
  });

  it('should get services by provider', () => {
    const mockData = [mockServiceModel];

    service.getByProvider(10).subscribe((response) => {
      expect(response.success).toBe(true);
      expect(response.data?.length).toBe(1);
    });

    const req = httpMock.expectOne((request) =>
      request.url.includes('/api/provider-services/provider/10') && request.method === 'GET'
    );
    req.flush({ success: true, data: mockData });
  });

  it('should get categories', () => {
    const mockCategories = ['Beauty', 'Fitness', 'Photography'];

    service.getCategories().subscribe((response) => {
      expect(response.success).toBe(true);
      expect(response.data?.length).toBe(3);
      expect(response.data?.includes('Beauty')).toBe(true);
    });

    const req = httpMock.expectOne((request) =>
      request.url.includes('/api/provider-services/categories') && request.method === 'GET'
    );
    req.flush({ success: true, data: mockCategories });
  });

  it('should get providers with filters', () => {
    const mockData = [mockProviderDirectory];

    service.getProviders('Beauty', 'Belgrade', 'hair', 'name').subscribe((response) => {
      expect(response.success).toBe(true);
      expect(response.data?.length).toBe(1);
    });

    const req = httpMock.expectOne((request) => {
      return request.url.includes('/api/provider-services/providers') &&
        request.params.has('category') &&
        request.params.has('city') &&
        request.params.has('query') &&
        request.params.has('sortBy') &&
        request.method === 'GET';
    });
    req.flush({ success: true, data: mockData });
  });

  it('should upload image', () => {
    const mockFile = new File(['test'], 'test.jpg', { type: 'image/jpeg' });

    service.uploadImage(mockFile).subscribe((response) => {
      expect(response.success).toBe(true);
      expect(response.data).toContain('/images/services/');
    });

    const req = httpMock.expectOne((request) =>
      request.url.includes('/api/provider-services/upload-image') && request.method === 'POST'
    );
    req.flush({ success: true, data: '/images/services/uploaded.jpg' });
  });

  it('should handle empty search results', () => {
    service.search('NonExistent', 'noquery').subscribe((response) => {
      expect(response.success).toBe(true);
      expect(response.data?.length).toBe(0);
    });

    const req = httpMock.expectOne((request) =>
      request.url.includes('/api/provider-services/search') && request.method === 'GET'
    );
    req.flush({ success: true, data: [] });
  });

  it('should handle error response', () => {
    service.getMyServices().subscribe({
      next: () => {
        throw new Error('should have failed');
      },
      error: (error: unknown) => {
        expect(error).toBeTruthy();
      }
    });

    const req = httpMock.expectOne((request) =>
      request.url.includes('/api/provider-services/my') && request.method === 'GET'
    );
    req.flush({ success: false, message: 'Unauthorized' }, { status: 401, statusText: 'Unauthorized' });
  });

  it('should send correct parameters for provider filter', () => {
    service.getProviders('Beauty', 'Belgrade', 'salon', 'rating').subscribe(() => {
      expect(true).toBe(true);
    });

    const req = httpMock.expectOne((request) => {
      const params = request.params;
      return params.get('category') === 'Beauty' &&
        params.get('city') === 'Belgrade' &&
        params.get('query') === 'salon' &&
        params.get('sortBy') === 'rating';
    });
    req.flush({ success: true, data: [] });
  });
});
