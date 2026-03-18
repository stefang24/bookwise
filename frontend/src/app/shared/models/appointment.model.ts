export interface AppointmentModel {
  id: number;
  providerServiceId: number;
  serviceName: string;
  providerName: string;
  providerId: number;
  providerImagePath: string | null;
  clientName: string;
  clientId: number;
  clientImagePath: string | null;
  startUtc: string;
  endUtc: string;
  status: string;
  priceEur: number;
  durationMinutes: number;
}
