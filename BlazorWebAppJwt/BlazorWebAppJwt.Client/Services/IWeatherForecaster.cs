namespace BlazorWebAppJwt.Client.Services;

public interface IWeatherForecaster
{
    Task<IEnumerable<WeatherForecast>> GetWeatherForecastAsync();
    Task<string?> LoginAsync(string username, string password);
}