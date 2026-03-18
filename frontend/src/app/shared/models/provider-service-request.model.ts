export interface ProviderServiceRequest {
  name: string;
  category: string;
  description: string | null;
  imageUrl: string | null;
  priceEur: number;
  durationMinutes: number;
}
