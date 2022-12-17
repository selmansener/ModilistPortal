namespace ModilistPortal.API.Models
{
    public class ResponseModel<T>
    {
        public ResponseModel(T data)
        {
            StatusCode = 200;
            Data = data;
        }

        public ResponseModel(int statusCode, T? data, string? message = null, string? errorType = null, IDictionary<string, List<string>>? errors = null)
        {
            StatusCode = statusCode;
            Data = data;
            Message = message;
            ErrorType = errorType;
            Errors = errors;
        }

        public int StatusCode { get; private set; }

        public T? Data { get; private set; }

        public string? Message { get; private set; }

        public string? ErrorType { get; private set; }

        public IDictionary<string, List<string>>? Errors { get; private set; }
    }
}
