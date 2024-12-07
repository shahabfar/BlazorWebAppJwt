using BlazorWebAppJwt.Client.Services;
using Microsoft.AspNetCore.Authentication;
using System.Net.Http.Headers;

namespace BlazorWebAppJwt;

public class ServerWeatherForecaster(HttpClient httpClient, IHttpContextAccessor httpContextAccessor) : IWeatherForecaster
{
    public async Task<string?> LoginAsync(string username, string password)
    {
        var response = await httpClient.PostAsJsonAsync("/auth/login", new { username, password });
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<LoginResult>();
            return result?.Token;
        }
        return null;
    }

    public async Task<IEnumerable<WeatherForecast>> GetWeatherForecastAsync()
    {
        var httpContext = httpContextAccessor.HttpContext ??
                          throw new InvalidOperationException("No HttpContext available from the IHttpContextAccessor!");

        var accessToken = await httpContext.GetTokenAsync("JWT") ??
                          throw new InvalidOperationException("No JWT token found!");

        using var requestMessage = new HttpRequestMessage(HttpMethod.Get, "/weather-forecast");
        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        using var response = await httpClient.SendAsync(requestMessage);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<WeatherForecast[]>() ??
               throw new IOException("No weather forecast!");
    }
}

public record LoginResult(string Token);

