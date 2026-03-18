import { Component, computed, inject, signal } from '@angular/core';
import { DatePipe, CurrencyPipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { MessageModule } from 'primeng/message';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { ConfirmationService } from 'primeng/api';
import { AppointmentsService } from '../../../../shared/services/appointments.service';
import { AppointmentModel } from '../../../../shared/models/appointment.model';
import { ResultResponse } from '../../../../shared/models/result-response.model';
import { AuthService } from '../../../../shared/services/auth.service';
import { NotificationService } from '../../../../shared/services/notification.service';
import { toAssetUrl } from '../../../../shared/config/api.config';

@Component({
  selector: 'app-history',
  imports: [FormsModule, RouterLink, DatePipe, CurrencyPipe, ButtonModule, MessageModule, ConfirmDialogModule],
  providers: [ConfirmationService],
  templateUrl: './history.page.html',
  styleUrl: './history.page.css'
})
export class HistoryPage {
  private appointmentsService: AppointmentsService = inject(AppointmentsService);
  private authService: AuthService = inject(AuthService);
  private confirmationService: ConfirmationService = inject(ConfirmationService);
  private notificationService: NotificationService = inject(NotificationService);

  items = signal<AppointmentModel[]>([]);
  errorMessage = signal<string>('');
  successMessage = signal<string>('');
  loading = signal<boolean>(false);
  search = signal<string>('');
  statusFilter = signal<string>('all');
  sortBy = signal<string>('date-desc');
  calendarMonth = signal<string>(new Date().toISOString().slice(0, 7));
  selectedCalendarDate = signal<string>('');

  calendarDays = computed<{ key: string; day: number; count: number; busy: boolean; today: boolean }[]>(() => {
    const monthValue: string = this.calendarMonth();
    if (!monthValue || !monthValue.includes('-')) return [];

    const [yearRaw, monthRaw] = monthValue.split('-');
    const year: number = parseInt(yearRaw, 10);
    const monthIndex: number = parseInt(monthRaw, 10) - 1;
    if (Number.isNaN(year) || Number.isNaN(monthIndex)) return [];

    const first: Date = new Date(Date.UTC(year, monthIndex, 1));
    const last: Date = new Date(Date.UTC(year, monthIndex + 1, 0));
    const now: Date = new Date();

    const countMap: Record<string, number> = {};
    for (const item of this.items()) {
      const key: string = item.startUtc.slice(0, 10);
      countMap[key] = (countMap[key] ?? 0) + 1;
    }

    const days: { key: string; day: number; count: number; busy: boolean; today: boolean }[] = [];
    for (let d = 1; d <= last.getUTCDate(); d += 1) {
      const current: Date = new Date(Date.UTC(year, monthIndex, d));
      const key: string = current.toISOString().slice(0, 10);
      const count: number = countMap[key] ?? 0;
      days.push({
        key,
        day: d,
        count,
        busy: count > 0,
        today: now.getUTCFullYear() === current.getUTCFullYear() &&
          now.getUTCMonth() === current.getUTCMonth() &&
          now.getUTCDate() === current.getUTCDate()
      });
    }

    const leadingEmpty = (first.getUTCDay() + 6) % 7;
    for (let i = 0; i < leadingEmpty; i += 1) {
      days.unshift({ key: `empty-before-${i}`, day: 0, count: 0, busy: false, today: false });
    }

    while (days.length % 7 !== 0) {
      days.push({ key: `empty-after-${days.length}`, day: 0, count: 0, busy: false, today: false });
    }

    return days;
  });

  filteredItems = computed<AppointmentModel[]>(() => {
    const term: string = this.search().trim().toLowerCase();
    const status: string = this.statusFilter();
    const sort: string = this.sortBy();

    let list: AppointmentModel[] = [...this.items()];

    if (status !== 'all') {
      list = list.filter((x: AppointmentModel) => x.status.toLowerCase() === status);
    }

    if (term) {
      list = list.filter((x: AppointmentModel) =>
        x.serviceName.toLowerCase().includes(term) ||
        x.providerName.toLowerCase().includes(term) ||
        x.clientName.toLowerCase().includes(term));
    }

    const selectedDate: string = this.selectedCalendarDate();
    if (selectedDate) {
      list = list.filter((x: AppointmentModel) => x.startUtc.slice(0, 10) === selectedDate);
    }

    if (sort === 'date-asc') {
      list.sort((a, b) => new Date(a.startUtc).getTime() - new Date(b.startUtc).getTime());
    } else if (sort === 'price-desc') {
      list.sort((a, b) => b.priceEur - a.priceEur);
    } else if (sort === 'price-asc') {
      list.sort((a, b) => a.priceEur - b.priceEur);
    } else {
      list.sort((a, b) => new Date(b.startUtc).getTime() - new Date(a.startUtc).getTime());
    }

    return list;
  });

  constructor() {
    this.load();
  }

  load(): void {
    this.loading.set(true);
    this.errorMessage.set('');

    this.appointmentsService.getMyHistory().subscribe({
      next: (response: ResultResponse<AppointmentModel[]>) => {
        this.loading.set(false);
        if (response.success) {
          this.items.set(response.data ?? []);
        } else {
          this.errorMessage.set(response.message ?? 'Failed to load appointments.');
        }
      },
      error: () => {
        this.loading.set(false);
        this.errorMessage.set('Failed to load appointments.');
      }
    });
  }

  cancel(item: AppointmentModel): void {
    this.confirmationService.confirm({
      header: 'Cancel appointment',
      message: `Are you sure you want to cancel ${item.serviceName} on ${this.formatDateTime(item.startUtc)}?`,
      icon: 'pi pi-exclamation-triangle',
      acceptLabel: 'Cancel Appointment',
      rejectLabel: 'Keep',
      rejectButtonStyleClass: 'p-button-outlined p-button-secondary',
      acceptButtonStyleClass: 'p-button-danger',
      accept: () => {
        this.errorMessage.set('');
        this.successMessage.set('');

        this.appointmentsService.cancel(item.id).subscribe({
          next: (response: ResultResponse<boolean>) => {
            if (response.success) {
              this.successMessage.set('Appointment cancelled.');
              this.notificationService.success('Cancelled', 'Appointment cancelled.');
              this.load();
            } else {
              this.errorMessage.set(response.message ?? 'Cancel failed.');
              this.notificationService.error('Cancel failed', response.message ?? 'Cancel failed.');
            }
          },
          error: () => {
            this.errorMessage.set('Cancel failed.');
            this.notificationService.error('Cancel failed', 'Cancel failed.');
          }
        });
      }
    });
  }

  get isProvider(): boolean {
    return this.authService.role() === 'Provider';
  }

  getUserImageUrl(path: string | null): string | null {
    return toAssetUrl(path);
  }

  selectCalendarDate(date: string): void {
    this.selectedCalendarDate.set(date);
  }

  clearCalendarDateFilter(): void {
    this.selectedCalendarDate.set('');
  }

  private formatDateTime(value: string): string {
    const date: Date = new Date(value);
    const dd: string = String(date.getDate()).padStart(2, '0');
    const mm: string = String(date.getMonth() + 1).padStart(2, '0');
    const yyyy: string = String(date.getFullYear());
    const hh: string = String(date.getHours()).padStart(2, '0');
    const min: string = String(date.getMinutes()).padStart(2, '0');
    return `${dd}/${mm}/${yyyy} ${hh}:${min}`;
  }
}
