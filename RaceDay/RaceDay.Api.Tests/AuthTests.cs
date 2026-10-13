using System.Net;
using System.Net.Http.Json;
using RaceDay.Contracts;
using Xunit;

namespace RaceDay.Api.Tests;

public class AuthTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public AuthTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Register_ValidUser_Returns201()
    {
        var client = _factory.CreateCookieClient();

        var response = await client.PostAsJsonAsync("/api/auth/register",
            new RegisterRequest("Alice", "Anderson", "alice@test.com", "Password123!", "Participant"));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task Register_DuplicateEmail_Returns409()
    {
        var client = _factory.CreateCookieClient();
        var request = new RegisterRequest("Bob", "Brown", "bob@test.com", "Password123!", "Participant");

        await client.PostAsJsonAsync("/api/auth/register", request);
        var second = await client.PostAsJsonAsync("/api/auth/register", request);

        Assert.Equal(HttpStatusCode.Conflict, second.StatusCode);
    }

    [Fact]
    public async Task Login_CorrectPassword_Returns200()
    {
        var client = _factory.CreateCookieClient();
        await client.PostAsJsonAsync("/api/auth/register",
            new RegisterRequest("Carol", "Clark", "carol@test.com", "Password123!", "Participant"));

        var response = await client.PostAsJsonAsync("/api/auth/login",
            new LoginRequest("carol@test.com", "Password123!"));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Login_WrongPassword_Returns401()
    {
        var client = _factory.CreateCookieClient();
        await client.PostAsJsonAsync("/api/auth/register",
            new RegisterRequest("Dan", "Davis", "dan@test.com", "Password123!", "Participant"));

        var response = await client.PostAsJsonAsync("/api/auth/login",
            new LoginRequest("dan@test.com", "WrongPassword!"));

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task ProtectedEndpoint_Unauthenticated_Returns401()
    {
        var client = _factory.CreateCookieClient();

        var response = await client.GetAsync("/api/profile/me");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Profile_Authenticated_Returns200()
    {
        var client = _factory.CreateCookieClient();
        await client.RegisterAndLoginAsync("eve@test.com", role: "Participant");

        var response = await client.GetAsync("/api/profile/me");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}