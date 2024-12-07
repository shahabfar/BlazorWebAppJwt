using System.Net.Http.Json;

namespace BlazorWebAppJwt.Client.Services;

internal sealed class ClientWeatherForecaster(HttpClient httpClient) : IWeatherForecaster
{
    public Task<string?> LoginAsync(string username, string password)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<WeatherForecast>> GetWeatherForecastAsync()
    {
        return await httpClient.GetFromJsonAsync<WeatherForecast[]>("/weather-forecast") ??
            throw new IOException("No weather forecast!");
    }
}