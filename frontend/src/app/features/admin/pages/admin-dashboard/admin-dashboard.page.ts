import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CurrencyPipe, DatePipe, DecimalPipe } from '@angular/common';
import { Router } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { MessageModule } from 'primeng/message';
import { TabsModule } from 'primeng/tabs';
import { InputTextModule } from 'primeng/inputtext';
import { ChartModule } from 'primeng/chart';
import { AdminService } from '../../../../shared/services/admin.service';
import { ResultResponse } from '../../../../shared/models/result-response.model';
import { AdminUserModel } from '../../../../shared/models/admin-user.model';
import { AdminAppointmentModel } from '../../../../shared/models/admin-appointment.model';
import { AdminStatsModel } from '../../../../shared/models/admin-stats.model';
import { NotificationService } from '../../../../shared/services/notification.service';

@Component({
  selector: 'app-admin-dashboard',
  imports: [FormsModule, DatePipe, CurrencyPipe, DecimalPipe, ButtonModule, MessageModule, TabsModule, InputTextModule, ChartModule],
  templateUrl: './admin-dashboard.page.html',
  styleUrl: './admin-dashboard.page.css'
})
export class AdminDashboardPage implements OnInit {
  private readonly adminService: AdminService = inject(AdminService);
  private readonly notificationService: NotificationService = inject(NotificationService);
  private readonly router: Router = inject(Router);

  users = signal<AdminUserModel[]>([]);
  appointments = signal<AdminAppointmentModel[]>([]);
  stats = signal<AdminStatsModel | null>(null);

  loadingUsers = signal<boolean>(false);
  loadingAppointments = signal<boolean>(false);
  loadingStats = signal<boolean>(false);
  errorMessage = signal<string>('');

  fromDate = signal<string>('');
  toDate = signal<string>('');

  userSearch = signal<string>('');
  userStatusFilter = signal<'all' | 'active' | 'inactive'>('all');
  userSort = signal<'name-asc' | 'name-desc' | 'created-desc' | 'created-asc'>('created-desc');

  providerSearch = signal<string>('');
  providerStatusFilter = signal<'all' | 'active' | 'inactive'>('all');
  providerSort = signal<'name-asc' | 'name-desc' | 'services-desc' | 'created-desc'>('name-asc');

  appointmentSearch = signal<string>('');
  appointmentStatusFilter = signal<string>('all');
  appointmentCategoryFilter = signal<string>('all');
  appointmentSort = signal<'start-desc' | 'start-asc' | 'price-desc' | 'price-asc'>('start-desc');

  chartOptions: any = {
    maintainAspectRatio: false,
    plugins: {
      legend: {
        labels: {
          color: '#334155'
        }
      }
    },
    scales: {
      x: {
        ticks: {
          color: '#64748b'
        },
        grid: {
          color: '#e2e8f0'
        }
      },
      y: {
        ticks: {
          color: '#64748b'
        },
        grid: {
          color: '#e2e8f0'
        }
      }
    }
  };

  usersOnly = computed<AdminUserModel[]>(() => this.users().filter((x) => x.role === 'User'));
  providersOnly = computed<AdminUserModel[]>(() => this.users().filter((x) => x.role === 'Provider'));

  filteredUsers = computed<AdminUserModel[]>(() => {
    const query = this.userSearch().trim().toLowerCase();
    const statusFilter = this.userStatusFilter();
    const sort = this.userSort();

    let items = this.usersOnly().filter((x) => {
      const matchesQuery =
        !query ||
        x.username.toLowerCase().includes(query) ||
        x.email.toLowerCase().includes(query) ||
        x.name.toLowerCase().includes(query) ||
        (x.city ?? '').toLowerCase().includes(query);

      const matchesStatus =
        statusFilter === 'all' ||
        (statusFilter === 'active' && x.isActive) ||
        (statusFilter === 'inactive' && !x.isActive);

      return matchesQuery && matchesStatus;
    });

    items = this.sortUsers(items, sort);
    return items;
  });

  filteredProviders = computed<AdminUserModel[]>(() => {
    const query = this.providerSearch().trim().toLowerCase();
    const statusFilter = this.providerStatusFilter();
    const sort = this.providerSort();

    let items = this.providersOnly().filter((x) => {
      const matchesQuery =
        !query ||
        x.username.toLowerCase().includes(query) ||
        x.email.toLowerCase().includes(query) ||
        x.name.toLowerCase().includes(query) ||
        (x.city ?? '').toLowerCase().includes(query);

      const matchesStatus =
        statusFilter === 'all' ||
        (statusFilter === 'active' && x.isActive) ||
        (statusFilter === 'inactive' && !x.isActive);

      return matchesQuery && matchesStatus;
    });

    if (sort === 'name-asc') {
      items = [...items].sort((a, b) => a.name.localeCompare(b.name));
    } else if (sort === 'name-desc') {
      items = [...items].sort((a, b) => b.name.localeCompare(a.name));
    } else if (sort === 'created-desc') {
      items = [...items].sort((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime());
    }

    return items;
  });

  appointmentCategories = computed<string[]>(() => {
    const categories = this.appointments()
      .map((x) => x.category)
      .filter((x) => !!x)
      .map((x) => x.trim());

    return [...new Set(categories)].sort((a, b) => a.localeCompare(b));
  });

  filteredAppointments = computed<AdminAppointmentModel[]>(() => {
    const query = this.appointmentSearch().trim().toLowerCase();
    const status = this.appointmentStatusFilter();
    const category = this.appointmentCategoryFilter();
    const sort = this.appointmentSort();

    let items = this.appointments().filter((x) => {
      const matchesQuery =
        !query ||
        x.serviceName.toLowerCase().includes(query) ||
        x.providerName.toLowerCase().includes(query) ||
        x.clientName.toLowerCase().includes(query);

      const matchesStatus = status === 'all' || x.status === status;
      const matchesCategory = category === 'all' || x.category === category;

      return matchesQuery && matchesStatus && matchesCategory;
    });

    if (sort === 'start-desc') {
      items = [...items].sort((a, b) => new Date(b.startUtc).getTime() - new Date(a.startUtc).getTime());
    } else if (sort === 'start-asc') {
      items = [...items].sort((a, b) => new Date(a.startUtc).getTime() - new Date(b.startUtc).getTime());
    } else if (sort === 'price-desc') {
      items = [...items].sort((a, b) => b.priceEur - a.priceEur);
    } else if (sort === 'price-asc') {
      items = [...items].sort((a, b) => a.priceEur - b.priceEur);
    }

    return items;
  });

  bookingsByDateText = computed<string>(() => {
    const data: AdminStatsModel | null = this.stats();
    if (!data || data.bookingsByDate.length === 0) {
      return 'No data';
    }

    return data.bookingsByDate.map((x) => `${x.label}: ${x.value}`).join(' | ');
  });

  bookingsByCategoryText = computed<string>(() => {
    const data: AdminStatsModel | null = this.stats();
    if (!data || data.bookingsByCategory.length === 0) {
      return 'No data';
    }

    return data.bookingsByCategory.map((x) => `${x.label}: ${x.value}`).join(' • ');
  });

  bookingsByDateChartData = computed<any>(() => {
    const data: AdminStatsModel | null = this.stats();
    const labels = data?.bookingsByDate.map((x) => x.label) ?? [];
    const values = data?.bookingsByDate.map((x) => x.value) ?? [];

    return {
      labels,
      datasets: [
        {
          label: 'Bookings',
          data: values,
          borderColor: '#1d4ed8',
          backgroundColor: 'rgba(59, 130, 246, 0.25)',
          fill: true,
          tension: 0.3
        }
      ]
    };
  });

  bookingsByCategoryChartData = computed<any>(() => {
    const data: AdminStatsModel | null = this.stats();
    const labels = data?.bookingsByCategory.map((x) => x.label) ?? [];
    const values = data?.bookingsByCategory.map((x) => x.value) ?? [];

    return {
      labels,
      datasets: [
        {
          data: values,
          backgroundColor: ['#1d4ed8', '#0ea5e9', '#10b981', '#f59e0b', '#ef4444', '#8b5cf6']
        }
      ]
    };
  });

  activeUsersCount = computed<number>(() => this.usersOnly().filter((x) => x.isActive).length);
  activeProvidersCount = computed<number>(() => this.providersOnly().filter((x) => x.isActive).length);
  totalRevenueEur = computed<number>(() => this.appointments().filter((x) => x.status === 'Completed').reduce((acc, x) => acc + x.priceEur, 0));
  averagePriceEur = computed<number>(() => {
    const items = this.appointments();
    if (!items.length) {
      return 0;
    }

    return items.reduce((acc, x) => acc + x.priceEur, 0) / items.length;
  });

  completionRate = computed<number>(() => {
    const s = this.stats();
    if (!s || s.totalAppointments === 0) {
      return 0;
    }

    return (s.completedAppointments / s.totalAppointments) * 100;
  });

  cancellationRate = computed<number>(() => {
    const s = this.stats();
    if (!s || s.totalAppointments === 0) {
      return 0;
    }

    return (s.cancelledAppointments / s.totalAppointments) * 100;
  });

  ngOnInit(): void {
    this.loadUsers();
    this.loadAppointments();
    this.loadStats();
  }

  loadUsers(): void {
    this.loadingUsers.set(true);
    this.adminService.getUsers().subscribe({
      next: (response: ResultResponse<AdminUserModel[]>) => {
        this.loadingUsers.set(false);
        if (response.success) {
          this.users.set(response.data ?? []);
        } else {
          this.errorMessage.set(response.message ?? 'Failed to load users.');
        }
      },
      error: () => {
        this.loadingUsers.set(false);
        this.errorMessage.set('Failed to load users.');
      }
    });
  }

  loadAppointments(): void {
    this.loadingAppointments.set(true);
    this.adminService.getAppointments().subscribe({
      next: (response: ResultResponse<AdminAppointmentModel[]>) => {
        this.loadingAppointments.set(false);
        if (response.success) {
          this.appointments.set(response.data ?? []);
        } else {
          this.errorMessage.set(response.message ?? 'Failed to load appointments.');
        }
      },
      error: () => {
        this.loadingAppointments.set(false);
        this.errorMessage.set('Failed to load appointments.');
      }
    });
  }

  loadStats(): void {
    this.loadingStats.set(true);

    const from: string | undefined = this.fromDate() ? `${this.fromDate()}T00:00:00Z` : undefined;
    const to: string | undefined = this.toDate() ? `${this.toDate()}T23:59:59Z` : undefined;

    this.adminService.getStats(from, to).subscribe({
      next: (response: ResultResponse<AdminStatsModel>) => {
        this.loadingStats.set(false);
        if (response.success) {
          this.stats.set(response.data);
        } else {
          this.errorMessage.set(response.message ?? 'Failed to load statistics.');
        }
      },
      error: () => {
        this.loadingStats.set(false);
        this.errorMessage.set('Failed to load statistics.');
      }
    });
  }

  toggleActivation(user: AdminUserModel): void {
    this.adminService.setUserActivation(user.id, !user.isActive).subscribe({
      next: (response: ResultResponse<boolean>) => {
        if (response.success) {
          this.notificationService.success('User updated', `User ${user.username} is now ${user.isActive ? 'deactivated' : 'activated'}.`);
          this.loadUsers();
        } else {
          this.notificationService.error('Update failed', response.message ?? 'Activation update failed.');
        }
      },
      error: () => {
        this.notificationService.error('Update failed', 'Activation update failed.');
      }
    });
  }

  goToProfile(userId: number): void {
    this.router.navigate(['/profile', userId]);
  }

  private sortUsers(items: AdminUserModel[], sort: 'name-asc' | 'name-desc' | 'created-desc' | 'created-asc'): AdminUserModel[] {
    if (sort === 'name-asc') {
      return [...items].sort((a, b) => a.name.localeCompare(b.name));
    }

    if (sort === 'name-desc') {
      return [...items].sort((a, b) => b.name.localeCompare(a.name));
    }

    if (sort === 'created-asc') {
      return [...items].sort((a, b) => new Date(a.createdAt).getTime() - new Date(b.createdAt).getTime());
    }

    return [...items].sort((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime());
  }
}
