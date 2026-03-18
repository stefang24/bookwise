export interface ProviderDirectoryItemModel {
  id: number;
  name: string;
  username: string;
  profileImagePath: string | null;
  primaryCategory: string | null;
  city: string | null;
  address: string | null;
  servicesCount: number;
}
