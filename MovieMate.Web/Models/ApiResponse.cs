using System.Net;

namespace MovieMate.Web.Models;

public class ApiResponse<T>
{
    public bool IsSuccess { get; set; } // Indicates whether the request was successful
    public T? Data { get; set; }        // The response data (if successful)
    public ApiError? Error { get; set; } // Error details (if failed)
    public HttpStatusCode StatusCode { get; set; } // HTTP status code
}