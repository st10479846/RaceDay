using System.Net;
using System.Net.Http.Json;
using RaceDay.Api.Tests;
using RaceDay.Contracts;
using Xunit;

namespace RaceDay.Api.Tests;

public class EnrolmentFlowTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public EnrolmentFlowTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Participant_CanEnrolInOpenEvent_Returns201()
    {
        // 1. Organiser creates an event
        var organiser = _factory.CreateCookieClient();
        await organiser.RegisterAndLoginAsync("org@test.com", role: "Organiser");

        var createEvent = await organiser.PostAsJsonAsync("/api/events",
            new CreateEventRequest(
                "Soweto Marathon", "Community race",
                DateTime.UtcNow.AddDays(30), DateTime.UtcNow.AddDays(30).AddHours(6),
                "Soweto", 42.2m, "Road"));
        Assert.Equal(HttpStatusCode.Created, createEvent.StatusCode);
        var ev = await createEvent.Content.ReadFromJsonAsync<EventDto>();
        Assert.NotNull(ev);

        // 2. Organiser creates a category
        var createCat = await organiser.PostAsJsonAsync(
            $"/api/events/{ev!.EventId}/categories",
            new CreateCategoryRequest("Open Men", null, 42.2m, 500m,
                new TimeOnly(6, 0), 5000));
        Assert.Equal(HttpStatusCode.Created, createCat.StatusCode);
        var cat = await createCat.Content.ReadFromJsonAsync<CategoryDto>();
        Assert.NotNull(cat);

        // 3. Organiser sets the event to Open
        var updateEvent = await organiser.PutAsJsonAsync(
            $"/api/events/{ev.EventId}",
            new UpdateEventRequest(
                ev.Title, ev.Description,
                ev.StartDate, ev.EndDate,
                ev.Location, ev.DistanceKm, ev.EventType,
                "Open"));
        Assert.Equal(HttpStatusCode.OK, updateEvent.StatusCode);

        // 4. Participant enrols
        var participant = _factory.CreateCookieClient();
        await participant.RegisterAndLoginAsync("enrollee@test.com", role: "Participant");

        var enrol = await participant.PostAsJsonAsync("/api/enrolments",
            new CreateEnrolmentRequest(ev.EventId, cat!.CategoryId));

        Assert.Equal(HttpStatusCode.Created, enrol.StatusCode);
    }

    [Fact]
    public async Task Participant_CannotEnrolTwice_Returns409()
    {
        // Setup (same as above, but faster to inline)
        var organiser = _factory.CreateCookieClient();
        await organiser.RegisterAndLoginAsync("org2@test.com", role: "Organiser");

        var ev = await (await organiser.PostAsJsonAsync("/api/events",
            new CreateEventRequest("Event X", null,
                DateTime.UtcNow.AddDays(10), DateTime.UtcNow.AddDays(10).AddHours(2),
                "Durban", 21.1m, "Road")))
            .Content.ReadFromJsonAsync<EventDto>();

        var cat = await (await organiser.PostAsJsonAsync(
            $"/api/events/{ev!.EventId}/categories",
            new CreateCategoryRequest("Half Marathon", null, 21.1m, 350m,
                new TimeOnly(6, 0), 8000)))
            .Content.ReadFromJsonAsync<CategoryDto>();

        await organiser.PutAsJsonAsync($"/api/events/{ev.EventId}",
            new UpdateEventRequest(ev.Title, ev.Description,
                ev.StartDate, ev.EndDate, ev.Location,
                ev.DistanceKm, ev.EventType, "Open"));

        var participant = _factory.CreateCookieClient();
        await participant.RegisterAndLoginAsync("twice@test.com", role: "Participant");

        // First enrolment — OK
        var first = await participant.PostAsJsonAsync("/api/enrolments",
            new CreateEnrolmentRequest(ev.EventId, cat!.CategoryId));
        Assert.Equal(HttpStatusCode.Created, first.StatusCode);

        // Second enrolment — 409
        var second = await participant.PostAsJsonAsync("/api/enrolments",
            new CreateEnrolmentRequest(ev.EventId, cat.CategoryId));
        Assert.Equal(HttpStatusCode.Conflict, second.StatusCode);
    }
}