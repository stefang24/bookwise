import { Component, computed, inject, signal, OnInit } from '@angular/core';
import { ActivatedRoute, Params, RouterLink } from '@angular/router';
import { DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ProfileService } from '../../../../shared/services/profile.service';
import { WorkingHoursService } from '../../../../shared/services/working-hours.service';
import { ProviderCatalogService } from '../../../../shared/services/provider-catalog.service';
import { AuthService } from '../../../../shared/services/auth.service';
import { LoaderService } from '../../../../shared/services/loader.service';
import { ProfileResponse } from '../../../../shared/models/profile-response.model';
import { UpdateProfileRequest } from '../../../../shared/models/update-profile-request.model';
import { ResultResponse } from '../../../../shared/models/result-response.model';
import { ProviderWorkingHour } from '../../../../shared/models/provider-working-hour.model';
import { ProviderServiceModel } from '../../../../shared/models/provider-service.model';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { TextareaModule } from 'primeng/textarea';
import { MessageModule } from 'primeng/message';
import { TabsModule } from 'primeng/tabs';
import { AvatarModule } from 'primeng/avatar';
import { FileUploadModule } from 'primeng/fileupload';
import { TagModule } from 'primeng/tag';
import { PasswordModule } from 'primeng/password';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { AutoCompleteCompleteEvent, AutoCompleteModule } from 'primeng/autocomplete';
import { ConfirmationService } from 'primeng/api';
import { NotificationService } from '../../../../shared/services/notification.service';
import { toAssetUrl } from '../../../../shared/config/api.config';

@Component({
  selector: 'app-profile',
  imports: [
    FormsModule,
    DatePipe,
    RouterLink,
    ButtonModule,
    InputTextModule,
    TextareaModule,
    MessageModule,
    TabsModule,
    AvatarModule,
    FileUploadModule,
    TagModule,
    PasswordModule,
    ConfirmDialogModule,
    AutoCompleteModule
  ],
  providers: [ConfirmationService],
  templateUrl: './profile.page.html',
  styleUrl: './profile.page.css'
})
export class ProfilePage implements OnInit {
  private route: ActivatedRoute = inject(ActivatedRoute);
  private profileService: ProfileService = inject(ProfileService);
  private workingHoursService: WorkingHoursService = inject(WorkingHoursService);
  private providerCatalogService: ProviderCatalogService = inject(ProviderCatalogService);
  private authService: AuthService = inject(AuthService);
  private loaderService: LoaderService = inject(LoaderService);
  private notificationService: NotificationService = inject(NotificationService);
  private confirmationService: ConfirmationService = inject(ConfirmationService);

  profile = signal<ProfileResponse | null>(null);
  isEditing = signal<boolean>(false);
  errorMessage = signal<string>('');
  workingHours = signal<ProviderWorkingHour[]>([]);
  providerServices = signal<ProviderServiceModel[]>([]);
  categories = signal<string[]>([]);
  filteredCategories = signal<string[]>([]);
  readonly dayNames: string[] = ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'];
  readonly dayOrder: number[] = [1, 2, 3, 4, 5, 6, 0];

  editForm = signal<UpdateProfileRequest>({
    firstName: '',
    lastName: '',
    bio: null,
    phoneNumber: null,
    companyName: null,
    primaryCategory: null,
    companyDescription: null,
    city: null,
    address: null,
    website: null,
    currentPassword: null,
    newPassword: null
  });

  confirmNewPassword = signal<string>('');

  private readonly allowedTypes: string[] = ['image/jpeg', 'image/png', 'image/gif', 'image/webp'];

  passwordMismatch = computed<boolean>(() => {
    const form: UpdateProfileRequest = this.editForm();
    const confirm: string = this.confirmNewPassword();
    if (!form.newPassword && !confirm) return false;
    return form.newPassword !== confirm;
  });

  get isLoading(): boolean {
    return this.loaderService.isLoading('profile');
  }

  get isOwnProfile(): boolean {
    const currentUserId: number | null = this.authService.userId();
    const profileData: ProfileResponse | null = this.profile();
    return currentUserId !== null && profileData !== null && currentUserId === profileData.id;
  }

  get isProvider(): boolean {
    const profileData: ProfileResponse | null = this.profile();
    return profileData !== null && profileData.role === 'Provider';
  }

  get showChatButton(): boolean {
    return this.isProvider && !this.isOwnProfile && this.authService.isLoggedIn();
  }

  get canBookFromProfile(): boolean {
    return this.authService.role() === 'User' && !this.isOwnProfile && this.isProvider;
  }

  get canManageServicesFromProfile(): boolean {
    return this.isProvider && this.isOwnProfile;
  }

  get profileImageUrl(): string | null {
    const profileData: ProfileResponse | null = this.profile();
    return toAssetUrl(profileData?.profileImagePath);
  }

  ngOnInit(): void {
    this.providerCatalogService.getCategories().subscribe({
      next: (response: ResultResponse<string[]>) => {
        if (response.success) {
          const values: string[] = response.data ?? [];
          this.categories.set(values.slice());
          this.filteredCategories.set(values.slice());
        }
      }
    });

    this.route.params.subscribe((params: Params) => {
      const userId: number = parseInt(params['id'], 10);
      this.loadProfile(userId);
    });
  }

  loadProfile(userId: number): void {
    this.loaderService.show('profile');
    this.loaderService.show('fullscreen');
    this.errorMessage.set('');

    this.profileService.getProfile(userId).subscribe({
      next: (response: ResultResponse<ProfileResponse>) => {
        this.loaderService.hide('profile');
        this.loaderService.hide('fullscreen');
        if (response.success) {
          this.profile.set(response.data);
          if (response.data.role === 'Provider') {
            this.loadWorkingHours(response.data.id);
            this.loadProviderServices(response.data.id);
          } else {
            this.workingHours.set([]);
            this.providerServices.set([]);
          }
        } else {
          this.errorMessage.set(response.message);
        }
      },
      error: () => {
        this.loaderService.hide('profile');
        this.loaderService.hide('fullscreen');
        this.errorMessage.set('Failed to load profile.');
      }
    });
  }

  startEditing(): void {
    const profileData: ProfileResponse | null = this.profile();
    if (!profileData) return;

    this.editForm.set({
      firstName: profileData.firstName,
      lastName: profileData.lastName,
      bio: profileData.bio,
      phoneNumber: profileData.phoneNumber,
      companyName: profileData.companyName,
      primaryCategory: profileData.primaryCategory,
      companyDescription: profileData.companyDescription,
      city: profileData.city,
      address: profileData.address,
      website: profileData.website,
      currentPassword: null,
      newPassword: null
    });
    this.confirmNewPassword.set('');
    this.isEditing.set(true);
    this.errorMessage.set('');
  }

  cancelEditing(): void {
    this.isEditing.set(false);
    this.errorMessage.set('');
  }

  saveProfile(): void {
    this.loaderService.show('profile-save');
    this.errorMessage.set('');

    this.profileService.updateProfile(this.editForm()).subscribe({
      next: (response: ResultResponse<ProfileResponse>) => {
        this.loaderService.hide('profile-save');
        if (response.success) {
          this.profile.set(response.data);
          this.isEditing.set(false);
          this.notificationService.success('Profile', 'Profile updated successfully.');
        } else {
          this.errorMessage.set(response.message);
          this.notificationService.error('Profile update failed', response.message || 'Update failed.');
        }
      },
      error: () => {
        this.loaderService.hide('profile-save');
        this.errorMessage.set('Failed to update profile.');
        this.notificationService.error('Profile update failed', 'Failed to update profile.');
      }
    });
  }

  onImageSelect(event: { files: File[] }): void {
    const file: File = event.files[0];

    if (!file) return;

    if (!this.allowedTypes.includes(file.type)) {
      this.errorMessage.set('Only image files are allowed (.jpg, .png, .gif, .webp).');
      return;
    }

    if (file.size > 5 * 1024 * 1024) {
      this.errorMessage.set('File size must be less than 5MB.');
      return;
    }

    this.loaderService.show('profile-image');
    this.errorMessage.set('');

    this.profileService.uploadProfileImage(file).subscribe({
      next: (response: ResultResponse<ProfileResponse>) => {
        this.loaderService.hide('profile-image');
        if (response.success) {
          this.profile.set(response.data);
          this.notificationService.success('Image updated', 'Profile image updated.');
        } else {
          this.errorMessage.set(response.message);
          this.notificationService.error('Upload failed', response.message || 'Failed to upload image.');
        }
      },
      error: () => {
        this.loaderService.hide('profile-image');
        this.errorMessage.set('Failed to upload image.');
        this.notificationService.error('Upload failed', 'Failed to upload image.');
      }
    });
  }

  updateField(field: keyof UpdateProfileRequest, value: string): void {
    this.editForm.update((form: UpdateProfileRequest) => ({
      ...form,
      [field]: value || null
    }));
  }

  filterPrimaryCategorySuggestions(event: AutoCompleteCompleteEvent): void {
    const query: string = (event.query ?? '').trim().toLowerCase();
    const source: string[] = this.categories();

    if (!query) {
      this.filteredCategories.set([...source]);
      return;
    }

    this.filteredCategories.set(source.filter((x: string) => x.toLowerCase().includes(query)).slice());
  }

  get isSaving(): boolean {
    return this.loaderService.isLoading('profile-save');
  }

  getOrderedWorkingHours(): ProviderWorkingHour[] {
    const data: ProviderWorkingHour[] = this.workingHours();
    return this.dayOrder
      .map((day: number) => data.find((x: ProviderWorkingHour) => x.dayOfWeek === day))
      .filter((x): x is ProviderWorkingHour => !!x);
  }

  private loadWorkingHours(providerId: number): void {
    this.workingHoursService.getByProvider(providerId).subscribe({
      next: (response: ResultResponse<ProviderWorkingHour[]>) => {
        if (response.success) {
          this.workingHours.set(response.data ?? []);
        }
      }
    });
  }

  private loadProviderServices(providerId: number): void {
    this.providerCatalogService.getByProvider(providerId).subscribe({
      next: (response: ResultResponse<ProviderServiceModel[]>) => {
        if (response.success) {
          this.providerServices.set(response.data ?? []);
        }
      }
    });
  }

  getServiceImageUrl(path: string | null): string | null {
    return toAssetUrl(path);
  }

  getWebsiteHref(url: string | null): string | null {
    if (!url) {
      return null;
    }

    const trimmed: string = url.trim();
    if (!trimmed) {
      return null;
    }

    if (trimmed.startsWith('http://') || trimmed.startsWith('https://')) {
      return trimmed;
    }

    return `https://${trimmed}`;
  }

  removeOwnService(serviceId: number): void {
    const profileData: ProfileResponse | null = this.profile();
    if (!this.canManageServicesFromProfile || !profileData) {
      return;
    }

    this.confirmationService.confirm({
      header: 'Delete service',
      message: 'Are you sure you want to delete this service?',
      icon: 'pi pi-exclamation-triangle',
      acceptLabel: 'Delete',
      rejectLabel: 'Cancel',
      rejectButtonStyleClass: 'p-button-outlined p-button-secondary',
      acceptButtonStyleClass: 'p-button-danger',
      accept: () => {
        this.providerCatalogService.remove(serviceId).subscribe({
          next: (response: ResultResponse<boolean>) => {
            if (response.success) {
              this.notificationService.success('Service deleted', 'Service removed successfully.');
              this.loadProviderServices(profileData.id);
            } else {
              this.notificationService.error('Delete failed', response.message || 'Delete failed.');
            }
          },
          error: () => {
            this.notificationService.error('Delete failed', 'Delete failed.');
          }
        });
      }
    });
  }
}
