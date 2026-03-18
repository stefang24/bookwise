import { inject, Injectable } from '@angular/core';
import { MessageService } from 'primeng/api';

export type NotificationSeverity = 'success' | 'info' | 'warn' | 'error';

@Injectable({
  providedIn: 'root'
})
export class NotificationService {
  private messageService: MessageService = inject(MessageService);

  show(severity: NotificationSeverity, summary: string, detail: string): void {
    this.messageService.add({
      key: 'app',
      severity,
      summary,
      detail,
      life: 3500
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
}
