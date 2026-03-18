export interface UpdateProfileRequest {
  firstName: string;
  lastName: string;
  bio: string | null;
  phoneNumber: string | null;
  companyName: string | null;
  primaryCategory: string | null;
  companyDescription: string | null;
  city: string | null;
  address: string | null;
  website: string | null;
  currentPassword: string | null;
  newPassword: string | null;
}
