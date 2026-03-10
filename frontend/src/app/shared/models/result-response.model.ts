export interface ResultResponse<T> {
  success: boolean;
  data: T;
  message: string;
}
