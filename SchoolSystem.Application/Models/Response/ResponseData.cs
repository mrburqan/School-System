namespace SchoolSystem.Application.Models.Response
{
    public class ResponseData<T>
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }

        public static ResponseData<T> Success(T result, string message = "Operation succeeded")
        {
            return new ResponseData<T>
            {
                IsSuccess = true,
                Data = result,
                Message = string.IsNullOrWhiteSpace(message) ? "Operation succeeded" : message
            };
        }

        public static ResponseData<T> Failure(string message = "Operation failed")
        {
            return new ResponseData<T>
            {
                IsSuccess = false,
                Data = default,
                Message = string.IsNullOrWhiteSpace(message) ? "Operation failed" : message
            };
        }
    }
}
