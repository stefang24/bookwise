export interface AdminAppointmentModel {
  id: number;
  serviceName: string;
  category: string;
  providerName: string;
  providerId: number;
  clientName: string;
  clientId: number;
  startUtc: string;
  endUtc: string;
  status: string;
  priceEur: number;
}
