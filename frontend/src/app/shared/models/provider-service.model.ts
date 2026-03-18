export interface ProviderServiceModel {
  id: number;
  providerId: number;
  providerName: string;
  providerImagePath: string | null;
  providerCity: string | null;
  name: string;
  category: string;
  description: string | null;
  imageUrl: string | null;
  priceEur: number;
  durationMinutes: number;
  isActive: boolean;
}
