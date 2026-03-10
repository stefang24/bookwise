namespace backend.DTOs
{
    public class ResultResponse<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string? Message { get; set; }

        public static ResultResponse<T> Ok(T data)
        {
            return new ResultResponse<T> { Success = true, Data = data };
        }

        public static ResultResponse<T> Fail(string message)
        {
            return new ResultResponse<T> { Success = false, Message = message };
        }
    }
}
