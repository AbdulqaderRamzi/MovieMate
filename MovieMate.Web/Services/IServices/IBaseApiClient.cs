using MovieMate.Web.Models;

namespace MovieMate.Web.Services.IServices;

public interface IBaseApiClient
{
    Task<ApiResponse<TResponse>> SendAsync<TRequest, TResponse>(
        HttpMethod httpMethod, string endpoint, TRequest? data = default, 
        bool requiresAuth = true);
}