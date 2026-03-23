import { Component, computed, ElementRef, HostListener, inject, OnInit, signal, ViewChild } from '@angular/core';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { AuthService } from '../../../../shared/services/auth.service';
import { ButtonModule } from 'primeng/button';
import { ChatService } from '../../../../shared/services/chat.service';
import { AppNotificationModel } from '../../../../shared/models/app-notification.model';

@Component({
  selector: 'app-header',
  imports: [RouterLink, RouterLinkActive, ButtonModule],
  templateUrl: './header.component.html',
  styleUrl: './header.component.css'
})
export class HeaderComponent implements OnInit {
  private authService: AuthService = inject(AuthService);
  private chatService: ChatService = inject(ChatService);
  private router: Router = inject(Router);

  isLoggedIn = computed<boolean>(() => this.authService.isLoggedIn());
  username = computed<string>(() => this.authService.username());
  userId = computed<number | null>(() => this.authService.userId());
  role = computed<string>(() => this.authService.role());
  isAdmin = computed<boolean>(() => this.role() === 'Admin');
  isChatRole = computed<boolean>(() => this.role() === 'User' || this.role() === 'Provider');
  unreadNotifications = computed<number>(() => this.chatService.unreadNotifications());

  notificationsOpen = signal<boolean>(false);
  @ViewChild('notifWrap') private notifWrap?: ElementRef<HTMLElement>;

  ngOnInit(): void {
    if (!this.isLoggedIn() || !this.isChatRole()) {
      return;
    }

    this.chatService.ensureRealtimeConnected();
    this.chatService.refreshNotifications();
  }

  notifications(): AppNotificationModel[] {
    return this.chatService.notifications();
  }

  toggleNotifications(): void {
    this.notificationsOpen.set(!this.notificationsOpen());
  }

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent): void {
    if (!this.notificationsOpen()) {
      return;
    }

    const wrapper: HTMLElement | undefined = this.notifWrap?.nativeElement;
    const target = event.target;
    if (!wrapper || !(target instanceof Node)) {
      return;
    }

    if (!wrapper.contains(target)) {
      this.notificationsOpen.set(false);
    }
  }

  openNotification(item: AppNotificationModel): void {
    if (!item.isRead) {
      this.chatService.markNotificationRead(item.id).subscribe({
        next: () => {
          this.chatService.setNotificationReadLocally(item.id);
        }
      });
    }

    this.notificationsOpen.set(false);

    const type: string = (item.type ?? '').trim().toLowerCase();

    if (type === 'chat') {
      if (item.relatedUserId) {
        this.router.navigate(['/chat'], { queryParams: { with: item.relatedUserId } });
        return;
      }

      this.router.navigate(['/chat']);
      return;
    }

    this.router.navigate(['/appointments']);
  }

  markAsRead(item: AppNotificationModel, event: Event): void {
    event.stopPropagation();
    if (!item.isRead) {
      this.chatService.markNotificationRead(item.id).subscribe({
        next: () => {
          this.chatService.setNotificationReadLocally(item.id);
        }
      });
    }
  }

  onLogout(): void {
    this.authService.logout();
  }
}
