import { Component, inject, OnDestroy, OnInit, signal, ViewChild, ElementRef, AfterViewChecked } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { DatePipe } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { MessageModule } from 'primeng/message';
import { ChatService } from '../../../../shared/services/chat.service';
import { ChatContactModel } from '../../../../shared/models/chat-contact.model';
import { ChatMessageModel } from '../../../../shared/models/chat-message.model';
import { ResultResponse } from '../../../../shared/models/result-response.model';
import { SendChatMessageResponseModel } from '../../../../shared/models/send-chat-message-response.model';
import { AppNotificationModel } from '../../../../shared/models/app-notification.model';
import { toAssetUrl } from '../../../../shared/config/api.config';
import { NotificationService } from '../../../../shared/services/notification.service';
import { ProfileService } from '../../../../shared/services/profile.service';
import { AuthService } from '../../../../shared/services/auth.service';
import { ProfileResponse } from '../../../../shared/models/profile-response.model';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-chat',
  imports: [FormsModule, DatePipe, RouterLink, ButtonModule, InputTextModule, MessageModule],
  templateUrl: './chat.page.html',
  styleUrl: './chat.page.css'
})
export class ChatPage implements OnInit, OnDestroy {
  private readonly chatService: ChatService = inject(ChatService);
  private readonly route: ActivatedRoute = inject(ActivatedRoute);
  private readonly notificationService: NotificationService = inject(NotificationService);
  private readonly profileService: ProfileService = inject(ProfileService);
  private readonly authService: AuthService = inject(AuthService);

  @ViewChild('messagesScroll') private messagesScrollContainer!: ElementRef;

  contacts = signal<ChatContactModel[]>([]);
  selectedContact = signal<ChatContactModel | null>(null);
  messages = signal<ChatMessageModel[]>([]);
  draft = signal<string>('');
  loadingContacts = signal<boolean>(false);
  loadingMessages = signal<boolean>(false);
  errorMessage = signal<string>('');
  unreadByContact = signal<Record<number, number>>({});

  private incomingSub: Subscription | null = null;
  private querySub: Subscription | null = null;
  private refreshContactsSub: Subscription | null = null;

  async ngOnInit(): Promise<void> {
    await this.chatService.ensureRealtimeConnected();
    this.loadUnreadCountsFromNotifications();

    this.incomingSub = this.chatService.incomingMessage$.subscribe((message: ChatMessageModel) => {
      const currentUserId: number | null = this.authService.userId();
      if (currentUserId === null) {
        return;
      }

      const isForCurrentUser: boolean = message.receiverId === currentUserId;
      const otherUserId: number = message.senderId === currentUserId ? message.receiverId : message.senderId;
      const contact: ChatContactModel | null = this.selectedContact();
      if (contact && contact.userId === otherUserId) {
        this.messages.update((items: ChatMessageModel[]) => {
          if (items.some((x: ChatMessageModel) => x.id === message.id)) {
            return items;
          }
          return [...items, message];
        });

        this.unreadByContact.update((state: Record<number, number>) => ({
          ...state,
          [otherUserId]: 0
        }));

        if (isForCurrentUser) {
          this.markConversationNotificationsRead(otherUserId);
        }

        setTimeout(() => this.scrollToBottom(), 50);
      } else if (isForCurrentUser) {
        this.unreadByContact.update((state: Record<number, number>) => ({
          ...state,
          [otherUserId]: (state[otherUserId] ?? 0) + 1
        }));
      }

      this.loadContacts(null, true);
    });

    this.refreshContactsSub = this.chatService.refreshContacts$.subscribe(() => {
      this.loadContacts(null, true);
    });

    this.querySub = this.route.queryParams.subscribe((params: Record<string, string>) => {
      const raw: string | undefined = params['with'];
      const requestedId: number = Number(raw);
      this.loadContacts(Number.isInteger(requestedId) && requestedId > 0 ? requestedId : null);
    });
  }

  ngOnDestroy(): void {
    this.incomingSub?.unsubscribe();
    this.querySub?.unsubscribe();
    this.refreshContactsSub?.unsubscribe();
  }

  private scrollToBottom(): void {
    try {
      if (this.messagesScrollContainer) {
        this.messagesScrollContainer.nativeElement.scrollTop = this.messagesScrollContainer.nativeElement.scrollHeight;
      }
    } catch(err) { }
  }

  loadContacts(preselectUserId: number | null = null, isRefresh: boolean = false): void {
    if (!isRefresh) {
      this.loadingContacts.set(true);
    }

    this.chatService.getContacts().subscribe({
      next: (response: ResultResponse<ChatContactModel[]>) => {
        if (!isRefresh) {
          this.loadingContacts.set(false);
        }
        if (!response.success) {
          this.errorMessage.set(response.message ?? 'Failed to load contacts.');
          return;
        }

        let items: ChatContactModel[] = response.data ?? [];
        this.contacts.set(items);

        const current: ChatContactModel | null = this.selectedContact();
        if (preselectUserId !== null) {
          const preselected: ChatContactModel | undefined = items.find((x: ChatContactModel) => x.userId === preselectUserId);
          if (preselected) {
            this.selectContact(preselected);
            return;
          } else {
            this.profileService.getProfile(preselectUserId).subscribe((profileResp: ResultResponse<ProfileResponse>) => {
              if (profileResp.success && profileResp.data) {
                const p: ProfileResponse = profileResp.data;
                const newContact: ChatContactModel = {
                  userId: p.id,
                  name: p.companyName || `${p.firstName} ${p.lastName}`.trim(),
                  role: p.role,
                  profileImagePath: p.profileImagePath,
                  lastMessage: null,
                  lastMessageAtUtc: null
                };
                items = [newContact, ...items];
                this.contacts.set(items);
                this.selectContact(newContact);
              }
            });
            return;
          }
        }

        if (current) {
          const stillExists: ChatContactModel | undefined = items.find((x: ChatContactModel) => x.userId === current.userId);
          if (stillExists) {
            this.selectedContact.set(stillExists);
            return;
          }
        }
      },
      error: () => {
        this.loadingContacts.set(false);
        this.errorMessage.set('Failed to load contacts.');
      }
    });
  }

  selectContact(contact: ChatContactModel): void {
    this.selectedContact.set(contact);
    this.unreadByContact.update((state: Record<number, number>) => ({
      ...state,
      [contact.userId]: 0
    }));
    this.markConversationNotificationsRead(contact.userId);
    this.loadingMessages.set(true);
    this.errorMessage.set('');

    this.chatService.getMessages(contact.userId).subscribe({
      next: (response: ResultResponse<ChatMessageModel[]>) => {
        this.loadingMessages.set(false);
        if (response.success) {
          this.messages.set(response.data ?? []);
          setTimeout(() => this.scrollToBottom(), 50);
        } else {
          this.errorMessage.set(response.message ?? 'Failed to load messages.');
        }
      },
      error: () => {
        this.loadingMessages.set(false);
        this.errorMessage.set('Failed to load messages.');
      }
    });
  }

  async send(): Promise<void> {
    const contact: ChatContactModel | null = this.selectedContact();
    const content: string = this.draft().trim();

    if (!contact || !content) {
      return;
    }

    this.draft.set('');

    try {
      await this.chatService.sendMessageRealtime({
        receiverId: contact.userId,
        content
      });
      // Locally push message for snappier UI if desired, but here we wait for 'MessageReceived' event
      // If we don't push locally, SignalR will broadcast it right back to us and we scroll then.
      // But just in case, we can trigger a bottom scroll attempt here.
      setTimeout(() => this.scrollToBottom(), 100);
    } catch {
      this.chatService.sendMessageHttp({ receiverId: contact.userId, content }).subscribe({
        next: (response: ResultResponse<SendChatMessageResponseModel>) => {
          if (response.success && response.data?.message) {
            this.messages.update((items: ChatMessageModel[]) => [...items, response.data.message]);
            setTimeout(() => this.scrollToBottom(), 50);
            this.loadContacts(null, true);
          } else {
            this.notificationService.error('Message failed', response.message ?? 'Failed to send message.');
          }
        },
        error: () => {
          this.notificationService.error('Message failed', 'Failed to send message.');
        }
      });
    }
  }

  onDraftKeydown(event: KeyboardEvent): void {
    if (event.key === 'Enter' && !event.shiftKey) {
      event.preventDefault();
      this.send();
    }
  }

  isMine(message: ChatMessageModel): boolean {
    return message.senderId !== this.selectedContact()?.userId;
  }

  unreadCount(contactUserId: number): number {
    return this.unreadByContact()[contactUserId] ?? 0;
  }

  private loadUnreadCountsFromNotifications(): void {
    this.chatService.loadNotifications().subscribe({
      next: (response: ResultResponse<AppNotificationModel[]>) => {
        if (!response.success) {
          return;
        }

        const notifications: AppNotificationModel[] = response.data ?? [];
        this.chatService.notifications.set(notifications);

        const unreadMap: Record<number, number> = {};
        for (const item of notifications) {
          if (item.type !== 'chat' || item.isRead || item.relatedUserId === null) {
            continue;
          }

          unreadMap[item.relatedUserId] = (unreadMap[item.relatedUserId] ?? 0) + 1;
        }

        this.unreadByContact.set(unreadMap);
      }
    });
  }

  private markConversationNotificationsRead(contactUserId: number): void {
    const pending: AppNotificationModel[] = this.chatService.notifications().filter(
      (item: AppNotificationModel) => item.type === 'chat' && !item.isRead && item.relatedUserId === contactUserId
    );

    for (const item of pending) {
      this.chatService.markNotificationRead(item.id).subscribe({
        next: (result: ResultResponse<boolean>) => {
          if (result.success) {
            this.chatService.setNotificationReadLocally(item.id);
          }
        }
      });
    }
  }

  contactAvatar(path: string | null): string | null {
    return toAssetUrl(path);
  }
}
