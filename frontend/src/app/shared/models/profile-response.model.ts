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
  companyDescription: string | null;
  address: string | null;
  website: string | null;
  createdAt: string;
}
