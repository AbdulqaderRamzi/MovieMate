using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using MovieMate.Web.Contracts.Identity;
using MovieMate.Web.Exceptions;
using MovieMate.Web.Models;
using MovieMate.Web.Services.IServices;
using Newtonsoft.Json;

namespace MovieMate.Web.Services;

public class BaseApiClient : IBaseApiClient
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ITokenService _tokenService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public BaseApiClient(
        IHttpClientFactory httpClientFactory,
        ITokenService tokenService,
        IHttpContextAccessor httpContextAccessor)
    {
        _httpClientFactory = httpClientFactory;
        _tokenService = tokenService;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<ApiResponse<TResponse>> SendAsync<TRequest, TResponse>(
        HttpMethod httpMethod, string endpoint, TRequest? data = default,
        bool requiresAuth = true)
    {
        var client = _httpClientFactory.CreateClient("ApiClient");
        var request = new HttpRequestMessage(httpMethod, endpoint);
        if (data is not null)
        {
            request.Content = new StringContent(
                JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
        }

        if (requiresAuth)
        {
            var token = _tokenService.GetAccessToken();
            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        var response = await client.SendAsync(request);
        if (response.StatusCode == HttpStatusCode.Unauthorized && requiresAuth)
        {
            
            var newAccessToken = await RefreshTokenAsync();
            if (newAccessToken is not null)
            {
                request = new HttpRequestMessage(httpMethod, endpoint);
                if (data is not null)
                {
                    request.Content = new StringContent(
                        JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                }
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", newAccessToken);
                response = await client.SendAsync(request);
            }
            else
            {
                throw new AuthException();
            }
        }
        var content = await response.Content.ReadAsStringAsync();
        if (response.IsSuccessStatusCode)
        {
            var result = JsonConvert.DeserializeObject<TResponse>(content);
            return new ApiResponse<TResponse>
            {
                IsSuccess = true,
                Data = result,
                StatusCode = response.StatusCode
            };
        }
        
        var apiError = JsonConvert.DeserializeObject<ApiError>(content);
        return new ApiResponse<TResponse>
        {
            IsSuccess = false,
            Error = apiError,
            StatusCode = response.StatusCode
        };
    }

    private async Task<string?> RefreshTokenAsync()
    {
        var accessToken = _tokenService.GetAccessToken();
        var refreshToken = _tokenService.GetRefreshToken();
        if (string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(refreshToken))
            return null;
        var client = _httpClientFactory.CreateClient("ApiClient");
        var request = new HttpRequestMessage(HttpMethod.Post, "auth/refresh")
        {
            Content = new StringContent(
                JsonConvert.SerializeObject(new { accessToken, refreshToken }),
                Encoding.UTF8,
                "application/json")
        };
        var response = await client.SendAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            await _httpContextAccessor.HttpContext!.SignOutAsync();
            _tokenService.ClearTokens();
            return null;
        }
        var content = await response.Content.ReadAsStringAsync();
        
        var result = JsonConvert.DeserializeObject<AuthenticationResponse>(content);
        if (result is null)
        {
            await _httpContextAccessor.HttpContext!.SignOutAsync();
            _tokenService.ClearTokens();
            return null;
        }

        var claimsPrincipal = AddClaims(result);
        
        await _httpContextAccessor.HttpContext!.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            claimsPrincipal);
        _tokenService.SetTokens(result.AccessToken, result.RefreshToken);
        return result.AccessToken;
    }
    
    private ClaimsPrincipal AddClaims(AuthenticationResponse response)
    {
        var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
        identity.AddClaim(new Claim(ClaimTypes.Name, response.FullName));
        identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, response.Email));
        identity.AddClaim(new Claim(ClaimTypes.Email, response.Email));
        identity.AddClaim(new Claim(ClaimTypes.Role, response.Role));
        return new ClaimsPrincipal(identity);
    }
}