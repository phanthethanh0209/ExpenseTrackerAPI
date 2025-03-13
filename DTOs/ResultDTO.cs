namespace ExpenseTrackerAPI.DTOs
{
    public class ResultDTO<T>
    {
        public bool IsSuccess { get; set; }
        public T Data { get; set; }
        public string ErrorMessage { get; set; }

        public static ResultDTO<T> Success(T data)
        {
            return new ResultDTO<T> { IsSuccess = true, Data = data };
        }

        public static ResultDTO<T> Fail(string errorMessage)
        {
            return new ResultDTO<T> { IsSuccess = false, ErrorMessage = errorMessage };
        }
    }
}
