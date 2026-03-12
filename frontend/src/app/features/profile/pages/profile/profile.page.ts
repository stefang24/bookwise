import { Component, computed, inject, signal, OnInit } from '@angular/core';
import { ActivatedRoute, Params } from '@angular/router';
import { DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ProfileService } from '../../../../shared/services/profile.service';
import { AuthService } from '../../../../shared/services/auth.service';
import { LoaderService } from '../../../../shared/services/loader.service';
import { ProfileResponse } from '../../../../shared/models/profile-response.model';
import { UpdateProfileRequest } from '../../../../shared/models/update-profile-request.model';
import { ResultResponse } from '../../../../shared/models/result-response.model';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { TextareaModule } from 'primeng/textarea';
import { MessageModule } from 'primeng/message';
import { TabsModule } from 'primeng/tabs';
import { AvatarModule } from 'primeng/avatar';
import { FileUploadModule } from 'primeng/fileupload';
import { TagModule } from 'primeng/tag';
import { PasswordModule } from 'primeng/password';

@Component({
  selector: 'app-profile',
  imports: [
    FormsModule,
    DatePipe,
    ButtonModule,
    InputTextModule,
    TextareaModule,
    MessageModule,
    TabsModule,
    AvatarModule,
    FileUploadModule,
    TagModule,
    PasswordModule
  ],
  templateUrl: './profile.page.html',
  styleUrl: './profile.page.css'
})
export class ProfilePage implements OnInit {
  private route: ActivatedRoute = inject(ActivatedRoute);
  private profileService: ProfileService = inject(ProfileService);
  private authService: AuthService = inject(AuthService);
  private loaderService: LoaderService = inject(LoaderService);

  profile = signal<ProfileResponse | null>(null);
  isEditing = signal<boolean>(false);
  errorMessage = signal<string>('');
  successMessage = signal<string>('');

  editForm = signal<UpdateProfileRequest>({
    firstName: '',
    lastName: '',
    bio: null,
    phoneNumber: null,
    companyName: null,
    companyDescription: null,
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

  get profileImageUrl(): string | null {
    const profileData: ProfileResponse | null = this.profile();
    if (profileData?.profileImagePath) {
      return `http://localhost:5227${profileData.profileImagePath}`;
    }
    return null;
  }

  ngOnInit(): void {
    this.route.params.subscribe((params: Params) => {
      const userId: number = parseInt(params['id'], 10);
      this.loadProfile(userId);
    });
  }

  loadProfile(userId: number): void {
    this.loaderService.show('profile');
    this.errorMessage.set('');

    this.profileService.getProfile(userId).subscribe({
      next: (response: ResultResponse<ProfileResponse>) => {
        this.loaderService.hide('profile');
        if (response.success) {
          this.profile.set(response.data);
        } else {
          this.errorMessage.set(response.message);
        }
      },
      error: () => {
        this.loaderService.hide('profile');
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
      companyDescription: profileData.companyDescription,
      address: profileData.address,
      website: profileData.website,
      currentPassword: null,
      newPassword: null
    });
    this.confirmNewPassword.set('');
    this.isEditing.set(true);
    this.successMessage.set('');
    this.errorMessage.set('');
  }

  cancelEditing(): void {
    this.isEditing.set(false);
    this.errorMessage.set('');
  }

  saveProfile(): void {
    this.loaderService.show('profile-save');
    this.errorMessage.set('');
    this.successMessage.set('');

    this.profileService.updateProfile(this.editForm()).subscribe({
      next: (response: ResultResponse<ProfileResponse>) => {
        this.loaderService.hide('profile-save');
        if (response.success) {
          this.profile.set(response.data);
          this.isEditing.set(false);
          this.successMessage.set('Profile updated successfully.');
        } else {
          this.errorMessage.set(response.message);
        }
      },
      error: () => {
        this.loaderService.hide('profile-save');
        this.errorMessage.set('Failed to update profile.');
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
          this.successMessage.set('Profile image updated.');
        } else {
          this.errorMessage.set(response.message);
        }
      },
      error: () => {
        this.loaderService.hide('profile-image');
        this.errorMessage.set('Failed to upload image.');
      }
    });
  }

  updateField(field: keyof UpdateProfileRequest, value: string): void {
    this.editForm.update((form: UpdateProfileRequest) => ({
      ...form,
      [field]: value || null
    }));
  }

  get isSaving(): boolean {
    return this.loaderService.isLoading('profile-save');
  }
}
