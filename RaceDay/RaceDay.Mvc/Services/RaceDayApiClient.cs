using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace RaceDay.Mvc.Services;

public class RaceDayApiClient
{
    private readonly IHttpClientFactory _factory;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public RaceDayApiClient(IHttpClientFactory factory, IHttpContextAccessor accessor)
    {
        _factory = factory;
        _httpContextAccessor = accessor;
    }

    private HttpClient CreateClient()
    {
        return _factory.CreateClient("RaceDayApi");
    }

    private HttpRequestMessage CreateRequest(HttpMethod method, string url)
    {
        var request = new HttpRequestMessage(method, url);

        // Retrieve the API cookie stored in the MVC session
        string? apiCookie = _httpContextAccessor.HttpContext?.Session.GetString("ApiSessionCookie");

        if (!string.IsNullOrWhiteSpace(apiCookie))
        {
            request.Headers.TryAddWithoutValidation("Cookie", apiCookie);
        }
        return request;
    }

    // This is the NEW method that accepts both Objects (for JSON) and HttpContent (for File uploads)
    public async Task<HttpResponseMessage> SendAsync(HttpMethod method, string url, object? body = null)
    {
        var client = CreateClient();
        using var request = CreateRequest(method, url);

        if (body != null)
        {
            // If the body is already HttpContent (like a file upload), use it directly
            if (body is HttpContent httpContent)
            {
                request.Content = httpContent;
            }
            else
            {
                // Otherwise, serialize the object to JSON
                request.Content = JsonContent.Create(body);
            }
        }

        return await client.SendAsync(request);
    }

    public async Task<T?> ReadAsync<T>(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode) return default;
        string json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(json, JsonOptions);
    }

    public static string? ExtractSessionCookie(HttpResponseMessage response)
    {
        if (!response.Headers.TryGetValues("Set-Cookie", out var values))
            return null;

        // Find the specific ASP.NET Core session cookie
        string? sessionHeader = values.FirstOrDefault(v => v.StartsWith(".AspNetCore.Session=", StringComparison.OrdinalIgnoreCase));

        // Return just the name=value part
        return sessionHeader?.Split(';', 2)[0];
    }
}