using System.Net;
using System.Net.Http.Json;
using RaceDay.Contracts;

namespace RaceDay.Api.Tests;

public static class TestClientExtensions
{
    /// <summary>
    /// Create an HttpClient that automatically persists cookies between requests.
    /// Uses WebApplicationFactory's built-in cookie-aware handler.
    /// </summary>
    public static HttpClient CreateCookieClient(this CustomWebApplicationFactory factory)
    {
        var client = factory.CreateClient(new Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost"),
            AllowAutoRedirect = false,
            HandleCookies = true           // ← the built-in cookie jar
        });

        return client;
    }

    /// <summary>
    /// Register and log in a new user, returning the authenticated client.
    /// </summary>
    public static async Task<HttpClient> RegisterAndLoginAsync(
        this HttpClient client,
        string email,
        string password = "Password123!",
        string role = "Participant")
    {
        var reg = await client.PostAsJsonAsync("/api/auth/register",
            new RegisterRequest("Test", "User", email, password, role));
        reg.EnsureSuccessStatusCode();

        var login = await client.PostAsJsonAsync("/api/auth/login",
            new LoginRequest(email, password));
        login.EnsureSuccessStatusCode();

        return client;
    }
}