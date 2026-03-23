export interface AdminUserModel {
  id: number;
  email: string;
  username: string;
  name: string;
  role: string;
  isActive: boolean;
  city: string | null;
  createdAt: string;
}
