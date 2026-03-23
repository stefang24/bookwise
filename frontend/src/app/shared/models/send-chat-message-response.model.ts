import { AppNotificationModel } from './app-notification.model';
import { ChatMessageModel } from './chat-message.model';

export interface SendChatMessageResponseModel {
  message: ChatMessageModel;
  notification: AppNotificationModel | null;
}
