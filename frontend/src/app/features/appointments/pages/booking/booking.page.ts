import { Component, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { DatePipe, CurrencyPipe } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { InputTextModule } from 'primeng/inputtext';
import { ButtonModule } from 'primeng/button';
import { MessageModule } from 'primeng/message';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { ConfirmationService } from 'primeng/api';
import { DialogModule } from 'primeng/dialog';
import { ProviderCatalogService } from '../../../../shared/services/provider-catalog.service';
import { AppointmentsService } from '../../../../shared/services/appointments.service';
import { AuthService } from '../../../../shared/services/auth.service';
import { ProviderServiceModel } from '../../../../shared/models/provider-service.model';
import { AvailableSlot } from '../../../../shared/models/available-slot.model';
import { ResultResponse } from '../../../../shared/models/result-response.model';
import { NotificationService } from '../../../../shared/services/notification.service';
import { toAssetUrl } from '../../../../shared/config/api.config';
import { CalendarSlotModel } from '../../../../shared/models/calendar-slot.model';
import { LoaderService } from '../../../../shared/services/loader.service';

@Component({
  selector: 'app-booking',
  imports: [
    FormsModule,
    DatePipe,
    CurrencyPipe,
    RouterLink,
    InputTextModule,
    ButtonModule,
    MessageModule,
    ConfirmDialogModule,
    DialogModule
  ],
  providers: [ConfirmationService],
  templateUrl: './booking.page.html',
  styleUrl: './booking.page.css'
})
export class BookingPage {
  private providerCatalogService: ProviderCatalogService = inject(ProviderCatalogService);
  private appointmentsService: AppointmentsService = inject(AppointmentsService);
  private authService: AuthService = inject(AuthService);
  private confirmationService: ConfirmationService = inject(ConfirmationService);
  private notificationService: NotificationService = inject(NotificationService);
  private route: ActivatedRoute = inject(ActivatedRoute);
  private loaderService: LoaderService = inject(LoaderService);

  category = signal<string>('');
  query = signal<string>('');
  sortBy = signal<string>('relevance');
  selectedDate = signal<string>('');
  selectedService = signal<ProviderServiceModel | null>(null);
  detailsDialogVisible = signal<boolean>(false);

  services = signal<ProviderServiceModel[]>([]);
  categoryOptions = signal<string[]>([]);
  slots = signal<AvailableSlot[]>([]);
  calendarSlots = signal<CalendarSlotModel[]>([]);

  errorMessage = signal<string>('');
  successMessage = signal<string>('');
  loading = signal<boolean>(false);
  private requestedServiceId = signal<number | null>(null);

  filteredServices = computed<ProviderServiceModel[]>(() => {
    const sort: string = this.sortBy();
    const items: ProviderServiceModel[] = [...this.services()];

    if (sort === 'price-asc') {
      items.sort((a, b) => a.priceEur - b.priceEur);
    } else if (sort === 'price-desc') {
      items.sort((a, b) => b.priceEur - a.priceEur);
    } else if (sort === 'duration-asc') {
      items.sort((a, b) => a.durationMinutes - b.durationMinutes);
    } else if (sort === 'name') {
      items.sort((a, b) => a.name.localeCompare(b.name));
    }

    return items;
  });

  constructor() {
    this.route.queryParams.subscribe(params => {
      const raw: string | undefined = params['serviceId'];
      const parsed: number = raw ? parseInt(raw, 10) : NaN;
      this.requestedServiceId.set(Number.isNaN(parsed) ? null : parsed);
    });

    this.search();
    this.loadCategories();
  }

  get isClient(): boolean {
    return this.authService.role() === 'User';
  }

  search(): void {
    this.errorMessage.set('');
    this.successMessage.set('');
    this.loading.set(true);
    this.loaderService.show('fullscreen');

    this.providerCatalogService.search(this.category(), this.query()).subscribe({
      next: (response: ResultResponse<ProviderServiceModel[]>) => {
        this.loading.set(false);
        this.loaderService.hide('fullscreen');
        if (response.success) {
          const items: ProviderServiceModel[] = response.data ?? [];
          this.services.set(items);

          const serviceId: number | null = this.requestedServiceId();
          if (serviceId !== null) {
            const requested: ProviderServiceModel | undefined = items.find((x: ProviderServiceModel) => x.id === serviceId);
            if (requested) {
              this.chooseService(requested);
            }
            this.requestedServiceId.set(null);
          }
        } else {
          this.errorMessage.set(response.message ?? 'Search failed.');
        }
      },
      error: () => {
        this.loading.set(false);
        this.loaderService.hide('fullscreen');
        this.errorMessage.set('Search failed.');
      }
    });
  }

  loadCategories(): void {
    this.providerCatalogService.getCategories().subscribe({
      next: (response: ResultResponse<string[]>) => {
        if (response.success && response.data) {
          this.categoryOptions.set(response.data);
        }
      }
    });
  }

  chooseService(item: ProviderServiceModel): void {
    this.selectedService.set(item);
    this.slots.set([]);
    this.calendarSlots.set([]);
    this.successMessage.set('');
    this.errorMessage.set('');
    this.detailsDialogVisible.set(true);

    if (!this.selectedDate()) {
      this.selectedDate.set(new Date().toISOString().slice(0, 10));
    }

    this.loadCalendarSlots();
  }

  loadSlots(): void {
    const service: ProviderServiceModel | null = this.selectedService();
    if (!service || !this.selectedDate()) {
      this.errorMessage.set('Select service and date first.');
      return;
    }

    this.errorMessage.set('');
    this.slots.set([]);

    this.appointmentsService.getAvailableSlots(service.id, this.selectedDate()).subscribe({
      next: (response: ResultResponse<AvailableSlot[]>) => {
        if (response.success) {
          this.slots.set(response.data ?? []);
        } else {
          this.errorMessage.set(response.message ?? 'Failed to load slots.');
          this.notificationService.error('Slots', response.message ?? 'Failed to load slots.');
        }
      },
      error: () => {
        this.errorMessage.set('Failed to load slots.');
        this.notificationService.error('Slots', 'Failed to load slots.');
      }
    });
  }

  loadCalendarSlots(): void {
    const service: ProviderServiceModel | null = this.selectedService();
    if (!service || !this.selectedDate()) return;

    this.appointmentsService.getCalendarSlots(service.id, this.selectedDate()).subscribe({
      next: (response: ResultResponse<CalendarSlotModel[]>) => {
        if (response.success) {
          this.calendarSlots.set(response.data ?? []);
          this.slots.set((response.data ?? []).filter((x: CalendarSlotModel) => x.isAvailable).map((x: CalendarSlotModel) => ({
            startUtc: x.startUtc,
            endUtc: x.endUtc
          })));
        } else {
          this.errorMessage.set(response.message ?? 'Failed to load calendar slots.');
          this.notificationService.error('Slots', response.message ?? 'Failed to load calendar slots.');
        }
      },
      error: () => {
        this.errorMessage.set('Failed to load calendar slots.');
        this.notificationService.error('Slots', 'Failed to load calendar slots.');
      }
    });
  }

  book(slot: AvailableSlot): void {
    this.bookByStart(slot.startUtc);
  }

  bookCalendarSlot(slot: CalendarSlotModel): void {
    if (!slot.isAvailable) return;
    this.bookByStart(slot.startUtc);
  }

  private bookByStart(startUtc: string): void {
    if (!this.isClient) {
      this.errorMessage.set('Only user accounts can book appointments.');
      return;
    }

    const service: ProviderServiceModel | null = this.selectedService();
    if (!service) return;

    this.confirmationService.confirm({
      header: 'Confirm booking',
      message: `Book ${service.name} with ${service.providerName} on ${this.formatDateTime(startUtc)} for €${service.priceEur}?`,
      icon: 'pi pi-calendar-plus',
      acceptLabel: 'Book',
      rejectLabel: 'Cancel',
      rejectButtonStyleClass: 'p-button-outlined p-button-secondary',
      accept: () => {
        this.errorMessage.set('');
        this.successMessage.set('');

        this.appointmentsService.create({
          providerServiceId: service.id,
          startUtc
        }).subscribe({
          next: (response: ResultResponse<unknown>) => {
            if (response.success) {
              this.successMessage.set('Appointment booked successfully.');
              this.notificationService.success('Booked', `You booked ${service.name} with ${service.providerName}.`);
              this.loadCalendarSlots();
            } else {
              this.errorMessage.set(response.message ?? 'Booking failed.');
              this.notificationService.error('Booking failed', response.message ?? 'Booking failed.');
            }
          },
          error: () => {
            this.errorMessage.set('Booking failed.');
            this.notificationService.error('Booking failed', 'Booking failed.');
          }
        });
      }
    });
  }

  getProviderImageUrl(service: ProviderServiceModel): string | null {
    return toAssetUrl(service.providerImagePath);
  }

  getServiceImageUrl(service: ProviderServiceModel): string | null {
    return toAssetUrl(service.imageUrl);
  }

  closeDetailsDialog(): void {
    this.detailsDialogVisible.set(false);
  }

  formatDateTime(value: string): string {
    const date: Date = new Date(value);
    const dd: string = String(date.getDate()).padStart(2, '0');
    const mm: string = String(date.getMonth() + 1).padStart(2, '0');
    const yyyy: string = String(date.getFullYear());
    const hh: string = String(date.getHours()).padStart(2, '0');
    const min: string = String(date.getMinutes()).padStart(2, '0');
    return `${dd}/${mm}/${yyyy} ${hh}:${min}`;
  }
}
