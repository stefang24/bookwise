import { Component, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { MessageModule } from 'primeng/message';
import { InputTextModule } from 'primeng/inputtext';
import { ButtonModule } from 'primeng/button';
import { ProviderCatalogService } from '../../../../shared/services/provider-catalog.service';
import { ProviderDirectoryItemModel } from '../../../../shared/models/provider-directory-item.model';
import { ResultResponse } from '../../../../shared/models/result-response.model';
import { toAssetUrl } from '../../../../shared/config/api.config';

@Component({
  selector: 'app-providers',
  imports: [FormsModule, RouterLink, MessageModule, InputTextModule, ButtonModule],
  templateUrl: './providers.page.html',
  styleUrl: './providers.page.css'
})
export class ProvidersPage {
  private providerCatalogService: ProviderCatalogService = inject(ProviderCatalogService);

  items = signal<ProviderDirectoryItemModel[]>([]);
  categories = signal<string[]>([]);
  cityOptions = signal<string[]>([]);

  query = signal<string>('');
  category = signal<string>('');
  city = signal<string>('');
  sortBy = signal<string>('name');

  errorMessage = signal<string>('');
  loading = signal<boolean>(false);

  heading = computed<string>(() => `Providers (${this.items().length})`);

  constructor() {
    this.loadCategories();
    this.search();
  }

  search(): void {
    this.loading.set(true);
    this.errorMessage.set('');

    this.providerCatalogService.getProviders(this.category(), this.city(), this.query(), this.sortBy()).subscribe({
      next: (response: ResultResponse<ProviderDirectoryItemModel[]>) => {
        this.loading.set(false);
        if (response.success) {
          const data: ProviderDirectoryItemModel[] = response.data ?? [];
          this.items.set(data);
          this.cityOptions.set([...new Set(data.map(x => x.city).filter((x): x is string => !!x))].sort((a, b) => a.localeCompare(b)));
        } else {
          this.errorMessage.set(response.message ?? 'Failed to load providers.');
        }
      },
      error: () => {
        this.loading.set(false);
        this.errorMessage.set('Failed to load providers.');
      }
    });
  }

  loadCategories(): void {
    this.providerCatalogService.getCategories().subscribe({
      next: (response: ResultResponse<string[]>) => {
        if (response.success && response.data) {
          this.categories.set(response.data);
        }
      }
    });
  }

  getImage(path: string | null): string | null {
    return toAssetUrl(path);
  }
}
