export interface RegisterRequest {
  email: string;
  username: string;
  password: string;
  firstName: string;
  lastName: string;
  companyName?: string;
  city?: string;
  role: string;
}
