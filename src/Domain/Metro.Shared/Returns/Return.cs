using System.Net;

namespace Metro.Shared.Returns
{
    public class Return<T>
    {
        public HttpStatusCode StatusCode { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
        public Return(T data = default, HttpStatusCode statusCode = HttpStatusCode.Accepted, string message = "Accepted")
        {
            Data = data;
            StatusCode = statusCode;
            Message = message;
        }

        public static Return<T> OK(T data) 
        {
            return new Return<T>
            {
                StatusCode = HttpStatusCode.OK,
                Message = "OK",
                Data = data
            };
        }
        public static Return<T> NoContent(T data)
        {
            return new Return<T>
            {
                StatusCode = HttpStatusCode.NoContent,
                Message = "NoContent",
                Data = data
            };
        }
        public static Return<T> Unauthorized(T data)
        {
            return new Return<T>
            {
                StatusCode = HttpStatusCode.Unauthorized,
                Message = "Unauthorized",
                Data = data
            };
        }
        public static Return<T> NotFound(T data)
        {
            return new Return<T>
            {
                StatusCode = HttpStatusCode.NotFound,
                Message = "NotFound",
                Data = data
            };
        }
    }

     public class Return
    {
        public HttpStatusCode StatusCode { get; set; }
        public string Message { get; set; }
        public Return(HttpStatusCode statusCode = HttpStatusCode.Accepted, string message = "Accepted")
        {
            StatusCode = statusCode;
            Message = message;
        }

        public static Return OK()
        {
            return new Return
            {
                StatusCode = HttpStatusCode.OK,
                Message = "OK"
            };
        }
        public static Return NoContent()
        {
            return new Return
            {
                StatusCode = HttpStatusCode.NoContent,
                Message = "NoContent"
            };
        }
        public static Return Unauthorized()
        {
            return new Return
            {
                StatusCode = HttpStatusCode.Unauthorized,
                Message = "Unauthorized"
            };
        }
        public static Return NotFound()
        {
            return new Return
            {
                StatusCode = HttpStatusCode.NotFound,
                Message = "NotFound",
            };
        }
    }
}