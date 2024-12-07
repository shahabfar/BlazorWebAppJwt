using BlazorWebAppJwt.Client.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BlazorWebAppJwt;

internal static class WeatherForecasterEndpointRouteBuilderExtensions
{
    internal static void MapWeatherForecaster(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("");

        group.MapPost("/login", async ([FromBody] LoginRequest loginRequest, IWeatherForecaster weatherForecaster, HttpContext httpContext) =>
        {
            var token = await weatherForecaster.LoginAsync(loginRequest.Username, loginRequest.Password);

            if (!string.IsNullOrWhiteSpace(token))
            {
                var claims = new[]
                {
                    new Claim(ClaimTypes.Name, loginRequest.Username),
                    new Claim("JWT", token)
                };
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                return Results.Ok();
            }

            return Results.Unauthorized();
        })
        .AllowAnonymous();

        group.MapPost("/logout", async (HttpContext httpContext) =>
        {
            await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return TypedResults.Ok();
        })
        .AllowAnonymous();

        //group.MapGet("/weather-forecast", async (IWeatherForecaster weatherForecaster) =>
        //{
        //    var forecast = await weatherForecaster.GetWeatherForecastAsync();
        //    return Results.Ok(forecast);
        //}).RequireAuthorization();
    }
}

public record LoginRequest(string Username, string Password);

