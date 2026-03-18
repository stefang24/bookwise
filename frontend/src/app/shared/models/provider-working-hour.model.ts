export interface ProviderWorkingHour {
  dayOfWeek: number;
  isWorking: boolean;
  startTime: string | null;
  endTime: string | null;
}
