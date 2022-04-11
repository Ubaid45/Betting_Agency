using System.Net;

namespace BettingAgency.Application.Exceptions
{
    public class HttpStatusException : Exception
    {
        public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.InternalServerError;
        public string ErrorCode { get; set; }

        public HttpStatusException(string message, string errorCode) : base(message)
        {
            ErrorCode = errorCode;
        }

        public HttpStatusException(Exception exception, string errorCode) : base(exception.Message)
        {
            ErrorCode = errorCode;
        }

        public HttpStatusException(string message, HttpStatusCode statusCode, string errorCode) : base(message)
        {
            StatusCode = statusCode;
            ErrorCode = errorCode;
        }

        public HttpStatusException(Exception exception, HttpStatusCode statusCode, string errorCode) : base(exception.Message)
        {
            StatusCode = statusCode;
            ErrorCode = errorCode;
        }

        public HttpStatusException(HttpStatusCode statusCode, string errorCode) : base()
        {
            StatusCode = statusCode;
            ErrorCode = errorCode;
        }
    }
}