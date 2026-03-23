export interface StatPointModel {
  label: string;
  value: number;
}

export interface AdminStatsModel {
  totalUsers: number;
  totalProviders: number;
  totalAppointments: number;
  scheduledAppointments: number;
  completedAppointments: number;
  cancelledAppointments: number;
  bookingsByDate: StatPointModel[];
  bookingsByCategory: StatPointModel[];
}
