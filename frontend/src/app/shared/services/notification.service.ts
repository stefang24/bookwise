import { inject, Injectable } from '@angular/core';
import { MessageService } from 'primeng/api';

export type NotificationSeverity = 'success' | 'info' | 'warn' | 'error';

@Injectable({
  providedIn: 'root'
})
export class NotificationService {
  private messageService: MessageService = inject(MessageService);

  show(severity: NotificationSeverity, summary: string, detail: string, key: string = 'app', life: number = 3500): void {
    this.messageService.add({
      key,
      severity,
      summary,
      detail,
      life
    });
  }

  success(summary: string, detail: string): void {
    this.show('success', summary, detail);
  }

  info(summary: string, detail: string): void {
    this.show('info', summary, detail);
  }

  warn(summary: string, detail: string): void {
    this.show('warn', summary, detail);
  }

  error(summary: string, detail: string): void {
    this.show('error', summary, detail);
  }

  infoTopRight(summary: string, detail: string): void {
    this.show('info', summary, detail, 'realtime', 4000);
  }
}
