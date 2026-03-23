import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { ProviderCatalogService } from '../../../../shared/services/provider-catalog.service';
import { ProviderServiceModel } from '../../../../shared/models/provider-service.model';
import { ProviderDirectoryItemModel } from '../../../../shared/models/provider-directory-item.model';
import { toAssetUrl } from '../../../../shared/config/api.config';
import { AuthService } from '../../../../shared/services/auth.service';
import { AdminService } from '../../../../shared/services/admin.service';
import { AdminStatsModel } from '../../../../shared/models/admin-stats.model';

@Component({
  selector: 'app-home',
  imports: [CommonModule, RouterLink, ButtonModule],
  templateUrl: './home.page.html',
  styleUrl: './home.page.css'
})
export class HomePage implements OnInit {
  private static readonly FEATURED_SERVICES_LIMIT: number = 3;
  private static readonly TOP_PROVIDERS_LIMIT: number = 3;
  private readonly defaultServiceImagePath: string = '/images/services/default-service.svg';
  private readonly defaultProfileImagePath: string = '/images/profiles/default.png';
  private catalogService: ProviderCatalogService = inject(ProviderCatalogService);
  private authService: AuthService = inject(AuthService);
  private adminService: AdminService = inject(AdminService);

  featuredServices = signal<ProviderServiceModel[]>([]);
  topProviders = signal<ProviderDirectoryItemModel[]>([]);
  adminPreviewStats = signal<AdminStatsModel | null>(null);
  loading = signal<boolean>(false);
  loadingAdminPreview = signal<boolean>(false);

  isAdmin = computed<boolean>(() => this.authService.role() === 'Admin');

  ngOnInit(): void {
    this.loadFeaturedData();

    if (this.isAdmin()) {
      this.loadAdminPreview();
    }
  }

  private loadFeaturedData(): void {
    this.loading.set(true);
    this.catalogService.getFeaturedServices(HomePage.FEATURED_SERVICES_LIMIT).subscribe({
      next: (searchResponse) => {
        if (searchResponse.success && searchResponse.data) {
          this.featuredServices.set(searchResponse.data);
        }
      }
    });

    this.catalogService.getTopProviders(HomePage.TOP_PROVIDERS_LIMIT).subscribe({
      next: (providersResponse) => {
        if (providersResponse.success && providersResponse.data) {
          this.topProviders.set(providersResponse.data);
          this.loading.set(false);
        }
      },
      error: () => {
        this.loading.set(false);
      }
    });
  }

  private loadAdminPreview(): void {
    this.loadingAdminPreview.set(true);
    this.adminService.getStats().subscribe({
      next: (response) => {
        this.loadingAdminPreview.set(false);
        if (response.success && response.data) {
          this.adminPreviewStats.set(response.data);
        }
      },
      error: () => {
        this.loadingAdminPreview.set(false);
      }
    });
  }

  getServiceImageUrl(service: ProviderServiceModel): string {
    return toAssetUrl(service.imageUrl) ?? toAssetUrl(this.defaultServiceImagePath)!;
  }

  getProviderImageUrl(provider: ProviderDirectoryItemModel): string {
    return toAssetUrl(provider.profileImagePath) ?? toAssetUrl(this.defaultProfileImagePath)!;
  }
}
