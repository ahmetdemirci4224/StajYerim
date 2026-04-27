namespace StajYerim.Core.Common
{
    public class Result<T>
    {
        public T? Data { get; private set; }
        public bool IsSuccess { get; private set; }
        public string Message { get; private set; } = string.Empty;
        public List<string> Errors { get; private set; } = new List<string>();

        // Başarılı durumlar için
        public static Result<T> Success(T data, string message = "")
        {
            return new Result<T> { IsSuccess = true, Data = data, Message = message };
        }

        // Başarısız durumlar için
        public static Result<T> Failure(string errorMessage)
        {
            return new Result<T> { IsSuccess = false, Errors = new List<string> { errorMessage }, Message = errorMessage };
        }
        
        public static Result<T> Failure(List<string> errorMessages)
        {
            return new Result<T> { IsSuccess = false, Errors = errorMessages, Message = "Birden fazla hata oluştu." };
        }
    }
}
