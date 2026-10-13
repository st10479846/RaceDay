using System.Net;
using System.Net.Http.Json;
using RaceDay.Contracts;
using Xunit;

namespace RaceDay.Api.Tests;

public class RoleEnforcementTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public RoleEnforcementTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Participant_CannotCreateEvent_Returns403()
    {
        var client = _factory.CreateCookieClient();
        await client.RegisterAndLoginAsync("p1@test.com", role: "Participant");

        var response = await client.PostAsJsonAsync("/api/events",
            new CreateEventRequest(
                "Test Event", "Desc",
                DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(2),
                "Cape Town", 10m, "Road"));

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Organiser_CanCreateEvent_Returns201()
    {
        var client = _factory.CreateCookieClient();
        await client.RegisterAndLoginAsync("o1@test.com", role: "Organiser");

        var response = await client.PostAsJsonAsync("/api/events",
            new CreateEventRequest(
                "Cape Town Cycle Tour", "A big race",
                DateTime.UtcNow.AddDays(30), DateTime.UtcNow.AddDays(31),
                "Cape Town", 109m, "Cycling"));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task Organiser_CannotEnrolInEvent_Returns403()
    {
        var client = _factory.CreateCookieClient();
        await client.RegisterAndLoginAsync("o2@test.com", role: "Organiser");

        var response = await client.PostAsJsonAsync("/api/enrolments",
            new CreateEnrolmentRequest(1, 1));

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }
}