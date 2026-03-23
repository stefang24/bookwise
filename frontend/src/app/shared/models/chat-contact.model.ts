export interface ChatContactModel {
  userId: number;
  name: string;
  role: string;
  profileImagePath: string | null;
  lastMessage: string | null;
  lastMessageAtUtc: string | null;
}
