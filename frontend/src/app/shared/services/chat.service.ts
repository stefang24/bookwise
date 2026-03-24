import { computed, inject, Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, Subject } from 'rxjs';
import * as signalR from '@microsoft/signalr';
import { withApi } from '../config/api.config';
import { ResultResponse } from '../models/result-response.model';
import { ChatContactModel } from '../models/chat-contact.model';
import { ChatMessageModel } from '../models/chat-message.model';
import { SendChatMessageRequest } from '../models/send-chat-message-request.model';
import { SendChatMessageResponseModel } from '../models/send-chat-message-response.model';
import { AppNotificationModel } from '../models/app-notification.model';
import { LocalStorageService } from './local-storage.service';
import { AuthService } from './auth.service';
import { NotificationService } from './notification.service';

@Injectable({
  providedIn: 'root'
})
export class ChatService {
  private readonly http: HttpClient = inject(HttpClient);
  private readonly localStorageService: LocalStorageService = inject(LocalStorageService);
  private readonly authService: AuthService = inject(AuthService);
  private readonly notificationService: NotificationService = inject(NotificationService);

  private readonly API_URL: string = withApi('/api/chat');
  private readonly HUB_URL: string = withApi('/hubs/chat');

  private hubConnection: signalR.HubConnection | null = null;
  private connectionToken: string | null = null;
  private connectionStarting = false;

  private incomingMessageSubject: Subject<ChatMessageModel> = new Subject<ChatMessageModel>();
  incomingMessage$: Observable<ChatMessageModel> = this.incomingMessageSubject.asObservable();

  private refreshContactsSubject: Subject<void> = new Subject<void>();
  refreshContacts$: Observable<void> = this.refreshContactsSubject.asObservable();

  notifications = signal<AppNotificationModel[]>([]);
  unreadNotifications = computed<number>(() => this.notifications().filter((x: AppNotificationModel) => !x.isRead).length);

  async ensureRealtimeConnected(): Promise<void> {
    const token: string | null = this.localStorageService.getToken();

    if (!this.authService.isLoggedIn() || !token) {
      await this.disconnectRealtime();
      return;
    }

    const role: string = this.authService.role();
    if (role !== 'User' && role !== 'Provider') {
      await this.disconnectRealtime();
      return;
    }

    if (this.hubConnection && this.hubConnection.state === signalR.HubConnectionState.Connected) {
      if (this.connectionToken === token) {
        return;
      }

      await this.disconnectRealtime();
    }

    if (this.hubConnection && this.hubConnection.state === signalR.HubConnectionState.Connecting) {
      return;
    }

    if (this.connectionStarting) {
      return;
    }

    this.connectionStarting = true;

    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(this.HUB_URL, {
        accessTokenFactory: () => this.localStorageService.getToken() ?? ''
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection.on('MessageReceived', (message: ChatMessageModel) => {
      this.incomingMessageSubject.next(message);
    });

    this.hubConnection.on('NotificationReceived', (notification: AppNotificationModel) => {
      this.notifications.update((items: AppNotificationModel[]) => [notification, ...items]);
      this.playNotificationBeep();
      this.notificationService.infoTopRight(notification.title, notification.message);
    });

    this.hubConnection.on('ChatCreated', (data: any) => {
      this.refreshContactsSubject.next();
    });

    try {
      await this.hubConnection.start();
      this.connectionToken = token;
    } catch {
      this.hubConnection = null;
      this.connectionToken = null;
    } finally {
      this.connectionStarting = false;
    }
  }

  async disconnectRealtime(): Promise<void> {
    if (!this.hubConnection) {
      this.connectionToken = null;
      return;
    }

    try {
      await this.hubConnection.stop();
    } catch {
    } finally {
      this.hubConnection = null;
      this.connectionToken = null;
      this.connectionStarting = false;
    }
  }

  getContacts(): Observable<ResultResponse<ChatContactModel[]>> {
    return this.http.get<ResultResponse<ChatContactModel[]>>(`${this.API_URL}/contacts`);
  }

  getMessages(otherUserId: number): Observable<ResultResponse<ChatMessageModel[]>> {
    return this.http.get<ResultResponse<ChatMessageModel[]>>(`${this.API_URL}/messages/${otherUserId}`);
  }

  sendMessageHttp(request: SendChatMessageRequest): Observable<ResultResponse<SendChatMessageResponseModel>> {
    return this.http.post<ResultResponse<SendChatMessageResponseModel>>(`${this.API_URL}/messages`, request);
  }

  async sendMessageRealtime(request: SendChatMessageRequest): Promise<void> {
    await this.ensureRealtimeConnected();

    if (this.hubConnection && this.hubConnection.state === signalR.HubConnectionState.Connected) {
      await this.hubConnection.invoke('SendMessage', request);
      return;
    }

    throw new Error('Realtime connection unavailable.');
  }

  loadNotifications(): Observable<ResultResponse<AppNotificationModel[]>> {
    return this.http.get<ResultResponse<AppNotificationModel[]>>(`${this.API_URL}/notifications`);
  }

  refreshNotifications(): void {
    this.loadNotifications().subscribe({
      next: (response: ResultResponse<AppNotificationModel[]>) => {
        if (response.success) {
          this.notifications.set(response.data ?? []);
        }
      }
    });
  }

  markNotificationRead(notificationId: number): Observable<ResultResponse<boolean>> {
    return this.http.post<ResultResponse<boolean>>(`${this.API_URL}/notifications/${notificationId}/read`, {});
  }

  setNotificationReadLocally(notificationId: number): void {
    this.notifications.update((items: AppNotificationModel[]) => items.map((x: AppNotificationModel) =>
      x.id === notificationId ? { ...x, isRead: true } : x));
  }

  private playNotificationBeep(): void {
    try {
      const AudioCtx = window.AudioContext || (window as any).webkitAudioContext;
      if (!AudioCtx) return;
      const context = new AudioCtx();

      const playNote = (freq: number, startTime: number, duration: number) => {
        const osc = context.createOscillator();
        const gain = context.createGain();

        osc.type = 'sine';
        osc.frequency.value = freq;

        gain.gain.setValueAtTime(0, context.currentTime + startTime);
        gain.gain.linearRampToValueAtTime(0.08, context.currentTime + startTime + 0.02);
        gain.gain.exponentialRampToValueAtTime(0.001, context.currentTime + startTime + duration);

        osc.connect(gain);
        gain.connect(context.destination);

        osc.start(context.currentTime + startTime);
        osc.stop(context.currentTime + startTime + duration);
      };
      playNote(880.00, 0, 0.3);
      playNote(1318.51, 0.15, 0.4);
    } catch {
    }
  }
}
