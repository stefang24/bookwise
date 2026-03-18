import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MessageModule } from 'primeng/message';
import { CheckboxModule } from 'primeng/checkbox';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { WorkingHoursService } from '../../../../shared/services/working-hours.service';
import { AuthService } from '../../../../shared/services/auth.service';
import { ProviderWorkingHour } from '../../../../shared/models/provider-working-hour.model';
import { UpdateWorkingHoursRequest } from '../../../../shared/models/update-working-hours-request.model';
import { ResultResponse } from '../../../../shared/models/result-response.model';
import { NotificationService } from '../../../../shared/services/notification.service';
import { LoaderService } from '../../../../shared/services/loader.service';

@Component({
  selector: 'app-provider-schedule',
  imports: [FormsModule, MessageModule, CheckboxModule, ButtonModule, InputTextModule],
  templateUrl: './provider-schedule.page.html',
  styleUrl: './provider-schedule.page.css'
})
export class ProviderSchedulePage {
  private workingHoursService: WorkingHoursService = inject(WorkingHoursService);
  private authService: AuthService = inject(AuthService);
  private notificationService: NotificationService = inject(NotificationService);
  private loaderService: LoaderService = inject(LoaderService);

  readonly dayNames: string[] = ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'];
  readonly dayOrder: number[] = [1, 2, 3, 4, 5, 6, 0];

  rows = signal<ProviderWorkingHour[]>(this.createDefault());
  errorMessage = signal<string>('');
  successMessage = signal<string>('');
  loading = signal<boolean>(false);
  hasSavedSchedule = signal<boolean>(false);
  loadCompleted = signal<boolean>(false);

  constructor() {
    this.load();
  }

  get isProvider(): boolean {
    return this.authService.role() === 'Provider';
  }

  load(): void {
    if (!this.isProvider) return;

    this.loading.set(true);
    this.loadCompleted.set(false);
    this.loaderService.show('fullscreen');
    this.workingHoursService.getMy().subscribe({
      next: (response: ResultResponse<ProviderWorkingHour[]>) => {
        this.loading.set(false);
        this.loaderService.hide('fullscreen');
        this.loadCompleted.set(true);
        if (response.success && response.data?.length) {
          this.hasSavedSchedule.set(true);
          const mapped: ProviderWorkingHour[] = this.createDefault().map(day => {
            const found: ProviderWorkingHour | undefined = response.data?.find((x: ProviderWorkingHour) => x.dayOfWeek === day.dayOfWeek);
            return found ?? day;
          });
          this.rows.set(mapped);
        } else {
          this.hasSavedSchedule.set(false);
          this.rows.set(this.createDefault());
        }
      },
      error: () => {
        this.loading.set(false);
        this.loaderService.hide('fullscreen');
        this.loadCompleted.set(true);
        this.notificationService.error('Working hours', 'Failed to load schedule.');
      }
    });
  }

  save(): void {
    this.errorMessage.set('');
    this.successMessage.set('');

    const request: UpdateWorkingHoursRequest = {
      days: this.rows().map(x => ({
        dayOfWeek: x.dayOfWeek,
        isWorking: x.isWorking,
        startTime: x.isWorking ? (x.startTime ?? '09:00') : null,
        endTime: x.isWorking ? (x.endTime ?? '17:00') : null
      }))
    };

    this.workingHoursService.updateMy(request).subscribe({
      next: (response: ResultResponse<ProviderWorkingHour[]>) => {
        if (response.success) {
          this.successMessage.set('Working hours saved.');
          this.notificationService.success('Schedule saved', 'Working hours saved.');
          this.hasSavedSchedule.set(true);
          if (response.data) {
            const mapped: ProviderWorkingHour[] = this.createDefault().map(day => {
              const found: ProviderWorkingHour | undefined = response.data?.find((x: ProviderWorkingHour) => x.dayOfWeek === day.dayOfWeek);
              return found ?? day;
            });
            this.rows.set(mapped);
          }
        } else {
          this.errorMessage.set(response.message ?? 'Save failed.');
          this.notificationService.error('Save failed', response.message ?? 'Save failed.');
        }
      },
      error: () => {
        this.errorMessage.set('Save failed.');
        this.notificationService.error('Save failed', 'Save failed.');
      }
    });
  }

  updateWorking(dayOfWeek: number, checked: boolean): void {
    this.rows.update(rows => rows.map(x => x.dayOfWeek === dayOfWeek ? {
      ...x,
      isWorking: checked,
      startTime: checked ? (x.startTime ?? '09:00') : null,
      endTime: checked ? (x.endTime ?? '17:00') : null
    } : x));
  }

  updateTime(dayOfWeek: number, field: 'startTime' | 'endTime', value: string): void {
    this.rows.update(rows => rows.map(x => x.dayOfWeek === dayOfWeek ? { ...x, [field]: value } : x));
  }

  getOrderedRows(): ProviderWorkingHour[] {
    const data: ProviderWorkingHour[] = this.rows();
    return this.dayOrder
      .map((day: number) => data.find((x: ProviderWorkingHour) => x.dayOfWeek === day))
      .filter((x): x is ProviderWorkingHour => !!x);
  }

  private createDefault(): ProviderWorkingHour[] {
    return Array.from({ length: 7 }, (_, i) => ({
      dayOfWeek: i,
      isWorking: false,
      startTime: null,
      endTime: null
    }));
  }
}
