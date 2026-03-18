export interface ProfileResponse {
  id: number;
  email: string;
  username: string;
  firstName: string;
  lastName: string;
  role: string;
  profileImagePath: string | null;
  bio: string | null;
  phoneNumber: string | null;
  companyName: string | null;
  primaryCategory: string | null;
  companyDescription: string | null;
  city: string | null;
  address: string | null;
  website: string | null;
  createdAt: string;
}
