import { Component, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CurrencyPipe } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { ProviderCatalogService } from '../../../../shared/services/provider-catalog.service';
import { AuthService } from '../../../../shared/services/auth.service';
import { ProviderServiceModel } from '../../../../shared/models/provider-service.model';
import { ProviderServiceRequest } from '../../../../shared/models/provider-service-request.model';
import { ResultResponse } from '../../../../shared/models/result-response.model';
import { InputTextModule } from 'primeng/inputtext';
import { InputNumberModule } from 'primeng/inputnumber';
import { ButtonModule } from 'primeng/button';
import { MessageModule } from 'primeng/message';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { AutoCompleteCompleteEvent, AutoCompleteModule } from 'primeng/autocomplete';
import { ConfirmationService } from 'primeng/api';
import { NotificationService } from '../../../../shared/services/notification.service';
import { toAssetUrl } from '../../../../shared/config/api.config';

@Component({
  selector: 'app-provider-services',
  imports: [
    FormsModule,
    CurrencyPipe,
    InputTextModule,
    InputNumberModule,
    ButtonModule,
    MessageModule,
    ConfirmDialogModule,
    AutoCompleteModule
  ],
  providers: [ConfirmationService],
  templateUrl: './provider-services.page.html',
  styleUrl: './provider-services.page.css'
})
export class ProviderServicesPage {
  private providerCatalogService: ProviderCatalogService = inject(ProviderCatalogService);
  private authService: AuthService = inject(AuthService);
  private confirmationService: ConfirmationService = inject(ConfirmationService);
  private notificationService: NotificationService = inject(NotificationService);
  private route: ActivatedRoute = inject(ActivatedRoute);
  private router: Router = inject(Router);

  items = signal<ProviderServiceModel[]>([]);
  categorySuggestions = signal<string[]>([]);
  errorMessage = signal<string>('');
  successMessage = signal<string>('');
  isLoading = signal<boolean>(false);
  isUploading = signal<boolean>(false);
  editingId = signal<number | null>(null);
  isFormOpen = signal<boolean>(false);
  searchQuery = signal<string>('');
  filterCategory = signal<string>('');
  sortBy = signal<string>('name');
  durationMinutesInput = signal<number | null>(30);
  filteredCategorySuggestions = signal<string[]>([]);
  pendingImageFile = signal<File | null>(null);
  pendingImagePreviewUrl = signal<string | null>(null);
  private pendingEditId = signal<number | null>(null);

  filteredItems = computed<ProviderServiceModel[]>(() => {
    const term: string = this.searchQuery().trim().toLowerCase();
    const category: string = this.filterCategory().trim().toLowerCase();
    const sort: string = this.sortBy();

    let list: ProviderServiceModel[] = [...this.items()];

    if (term) {
      list = list.filter((x: ProviderServiceModel) =>
        x.name.toLowerCase().includes(term) ||
        x.category.toLowerCase().includes(term) ||
        (x.description ?? '').toLowerCase().includes(term));
    }

    if (category) {
      list = list.filter((x: ProviderServiceModel) => x.category.toLowerCase() === category);
    }

    if (sort === 'price-asc') {
      list.sort((a, b) => a.priceEur - b.priceEur);
    } else if (sort === 'price-desc') {
      list.sort((a, b) => b.priceEur - a.priceEur);
    } else if (sort === 'duration-asc') {
      list.sort((a, b) => a.durationMinutes - b.durationMinutes);
    } else if (sort === 'duration-desc') {
      list.sort((a, b) => b.durationMinutes - a.durationMinutes);
    } else {
      list.sort((a, b) => a.name.localeCompare(b.name));
    }

    return list;
  });

  form = signal<ProviderServiceRequest>({
    name: '',
    category: '',
    description: null,
    imageUrl: null,
    priceEur: 0,
    durationMinutes: 30
  });

  constructor() {
    this.route.queryParams.subscribe(params => {
      const rawId: string | undefined = params['editId'];
      const parsedId: number = Number(rawId);
      this.pendingEditId.set(Number.isInteger(parsedId) && parsedId > 0 ? parsedId : null);
      this.tryOpenRequestedEdit();
    });

    this.load();
    this.loadCategories();
  }

  get isProvider(): boolean {
    return this.authService.role() === 'Provider';
  }

  load(): void {
    this.isLoading.set(true);
    this.errorMessage.set('');

    this.providerCatalogService.getMyServices().subscribe({
      next: (response: ResultResponse<ProviderServiceModel[]>) => {
        this.isLoading.set(false);
        if (response.success) {
          this.items.set(response.data ?? []);
          this.tryOpenRequestedEdit();
        } else {
          this.errorMessage.set(response.message ?? 'Failed to load services.');
        }
      },
      error: () => {
        this.isLoading.set(false);
        this.errorMessage.set('Failed to load services.');
      }
    });
  }

  submit(): void {
    this.errorMessage.set('');
    this.successMessage.set('');

    const durationMinutes: number | null = this.durationMinutesInput();

    if (!this.form().name.trim() || !this.form().category.trim() || durationMinutes === null || durationMinutes < 5) {
      this.errorMessage.set('Fill all fields correctly.');
      return;
    }

    const pendingFile: File | null = this.pendingImageFile();

    if (pendingFile) {
      this.isUploading.set(true);
      this.providerCatalogService.uploadImage(pendingFile).subscribe({
        next: (response: ResultResponse<string>) => {
          this.isUploading.set(false);
          if (response.success && response.data) {
            this.form.update(v => ({ ...v, imageUrl: response.data }));
            this.pendingImageFile.set(null);
            this.doSubmit();
          } else {
            this.errorMessage.set(response.message ?? 'Image upload failed.');
            this.notificationService.error('Upload failed', response.message ?? 'Image upload failed.');
          }
        },
        error: () => {
          this.isUploading.set(false);
          this.errorMessage.set('Image upload failed.');
          this.notificationService.error('Upload failed', 'Image upload failed.');
        }
      });
    } else {
      this.doSubmit();
    }
  }

  private doSubmit(): void {
    const durationMinutes: number = this.durationMinutesInput() ?? 0;

    const request: ProviderServiceRequest = {
      name: this.form().name.trim(),
      category: this.form().category.trim(),
      description: this.form().description?.trim() || null,
      imageUrl: this.form().imageUrl || null,
      priceEur: this.form().priceEur,
      durationMinutes
    };

    const id: number | null = this.editingId();

    const action = id === null
      ? this.providerCatalogService.create(request)
      : this.providerCatalogService.update(id, request);

    action.subscribe({
      next: (response: ResultResponse<ProviderServiceModel>) => {
        if (response.success) {
          this.successMessage.set(id === null ? 'Service created.' : 'Service updated.');
          this.notificationService.success('Service saved', id === null ? 'Service created.' : 'Service updated.');
          this.resetForm();
          this.isFormOpen.set(false);
          this.load();
          this.loadCategories();
        } else {
          this.errorMessage.set(response.message ?? 'Save failed.');
          this.notificationService.error('Save failed', response.message ?? 'Save failed.');
        }
      },
      error: () => {
        this.errorMessage.set('Save failed.');
        this.notificationService.error('Save failed', 'Please try again.');
      }
    });
  }

  onImageSelected(event: Event): void {
    const input: HTMLInputElement = event.target as HTMLInputElement;
    const file: File | null = input.files && input.files[0] ? input.files[0] : null;

    if (!file) return;

    const existing: string | null = this.pendingImagePreviewUrl();
    if (existing) URL.revokeObjectURL(existing);

    this.pendingImageFile.set(file);
    this.pendingImagePreviewUrl.set(URL.createObjectURL(file));

    input.value = '';
  }

  getImagePreview(url: string | null): string | null {
    return toAssetUrl(url);
  }

  filterCategorySuggestions(event: AutoCompleteCompleteEvent): void {
    const query: string = (event.query ?? '').trim().toLowerCase();
    const source: string[] = this.categorySuggestions();

    if (!query) {
      this.filteredCategorySuggestions.set([...source]);
      return;
    }

    this.filteredCategorySuggestions.set(source.filter((x: string) => x.toLowerCase().includes(query)).slice());
  }

  setDurationMinutes(value: number | null): void {
    this.durationMinutesInput.set(value);
  }

  edit(item: ProviderServiceModel): void {
    this.editingId.set(item.id);
    this.isFormOpen.set(true);
    this.durationMinutesInput.set(item.durationMinutes);
    this.form.set({
      name: item.name,
      category: item.category,
      description: item.description,
      imageUrl: item.imageUrl,
      priceEur: item.priceEur,
      durationMinutes: item.durationMinutes
    });
  }

  remove(id: number): void {
    this.confirmationService.confirm({
      header: 'Remove service',
      message: 'Are you sure you want to remove this service?',
      icon: 'pi pi-exclamation-triangle',
      acceptLabel: 'Remove',
      rejectLabel: 'Cancel',
      rejectButtonStyleClass: 'p-button-outlined p-button-secondary',
      acceptButtonStyleClass: 'p-button-danger',
      accept: () => {
        this.errorMessage.set('');
        this.successMessage.set('');

        this.providerCatalogService.remove(id).subscribe({
          next: (response: ResultResponse<boolean>) => {
            if (response.success) {
              this.successMessage.set('Service removed.');
              this.notificationService.success('Removed', 'Service removed.');
              this.load();
              this.loadCategories();
            } else {
              this.errorMessage.set(response.message ?? 'Delete failed.');
              this.notificationService.error('Delete failed', response.message ?? 'Delete failed.');
            }
          },
          error: () => {
            this.errorMessage.set('Delete failed.');
            this.notificationService.error('Delete failed', 'Please try again.');
          }
        });
      }
    });
  }

  private loadCategories(): void {
    this.providerCatalogService.getCategories().subscribe({
      next: (response: ResultResponse<string[]>) => {
        if (response.success && response.data) {
          this.categorySuggestions.set(response.data.slice());
          this.filteredCategorySuggestions.set(response.data.slice());
        }
      }
    });
  }

  cancelEdit(): void {
    this.resetForm();
    this.isFormOpen.set(false);
  }

  openCreateForm(): void {
    this.resetForm();
    this.isFormOpen.set(true);
  }

  closeCreateForm(): void {
    this.cancelEdit();
  }

  private resetForm(): void {
    this.editingId.set(null);
    this.durationMinutesInput.set(30);
    this.form.set({
      name: '',
      category: '',
      description: null,
      imageUrl: null,
      priceEur: 0,
      durationMinutes: 30
    });
    const previewUrl: string | null = this.pendingImagePreviewUrl();
    if (previewUrl) URL.revokeObjectURL(previewUrl);
    this.pendingImageFile.set(null);
    this.pendingImagePreviewUrl.set(null);
  }

  private tryOpenRequestedEdit(): void {
    const requestedId: number | null = this.pendingEditId();
    if (requestedId === null || !this.items().length) {
      return;
    }

    const target: ProviderServiceModel | undefined = this.items().find(x => x.id === requestedId);
    if (!target) {
      return;
    }

    this.edit(target);
    this.pendingEditId.set(null);
    this.router.navigate([], {
      queryParams: { editId: null },
      queryParamsHandling: 'merge'
    });
  }
}
