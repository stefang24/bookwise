export interface AppNotificationModel {
  id: number;
  type: string;
  title: string;
  message: string;
  isRead: boolean;
  relatedUserId: number | null;
  relatedUserName: string | null;
  createdAtUtc: string;
}
