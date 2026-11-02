using Microsoft.AspNetCore.Mvc;
using RaceDay.Contracts;
using RaceDay.Mvc.Services;
using RaceDay.Mvc.ViewModels;

namespace RaceDay.Mvc.Controllers;

public class OrganiserController : BaseController
{
    private readonly RaceDayApiClient _api;

    public OrganiserController(RaceDayApiClient api)
    {
        _api = api;
    }

    private IActionResult? CheckAccess()
    {
        if (!IsLoggedIn()) return RedirectToAction("Login", "Auth");
        if (!IsOrganiser()) return RedirectToAction("Index", "Home");
        return null;
    }

    // GET: /Organiser
    public async Task<IActionResult> Index()
    {
        var accessCheck = CheckAccess();
        if (accessCheck != null) return accessCheck;

        var response = await _api.SendAsync(HttpMethod.Get, "api/events/mine");
        var events = await _api.ReadAsync<List<EventDto>>(response) ?? new List<EventDto>();

        var vm = new OrganiserDashboardVm
        {
            Events = events.Select(e => new OrganiserEventSummaryVm
            {
                EventId = e.EventId,
                Title = e.Title,
                StartDate = e.StartDate,
                Location = e.Location,
                Status = e.Status
            }).OrderByDescending(e => e.StartDate).ToList()
        };

        return View(vm);
    }

    // GET: /Organiser/CreateEvent
    [HttpGet]
    public IActionResult CreateEvent()
    {
        var accessCheck = CheckAccess();
        if (accessCheck != null) return accessCheck;
        return View(new CreateEventRequest("", "", DateTime.Now.AddDays(7), DateTime.Now.AddDays(8), "", 5.0m, "Run"));
    }

    // POST: /Organiser/CreateEvent
    [HttpPost]
    public async Task<IActionResult> CreateEvent(CreateEventRequest model)
    {
        var accessCheck = CheckAccess();
        if (accessCheck != null) return accessCheck;

        if (!ModelState.IsValid) return View(model);

        var response = await _api.SendAsync(HttpMethod.Post, "api/events", model);

        if (!response.IsSuccessStatusCode)
        {
            // READ THE EXACT ERROR FROM THE API
            string errorDetails = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError("", $"API Error: {errorDetails}");
            return View(model);
        }

        TempData["Success"] = "Event created successfully!";
        return RedirectToAction(nameof(Index));
    }

    // GET: /Organiser/ManageEvent/5
    public async Task<IActionResult> ManageEvent(int id)
    {
        var accessCheck = CheckAccess();
        if (accessCheck != null) return accessCheck;

        var eventResponse = await _api.SendAsync(HttpMethod.Get, $"api/events/{id}");
        var eventDto = await _api.ReadAsync<EventDto>(eventResponse);
        if (eventDto == null) return NotFound();

        var catResponse = await _api.SendAsync(HttpMethod.Get, $"api/events/{id}/categories");
        var categories = await _api.ReadAsync<List<CategoryDto>>(catResponse) ?? new List<CategoryDto>();

        var enrolResponse = await _api.SendAsync(HttpMethod.Get, $"api/enrolments/event/{id}");
        var enrolments = await _api.ReadAsync<List<EnrolmentDto>>(enrolResponse) ?? new List<EnrolmentDto>();

        var vm = new ManageEventVm
        {
            Event = eventDto,
            Categories = categories,
            Enrolments = enrolments
        };

        return View(vm);
    }

    // POST: /Organiser/UpdateStatus
    [HttpPost]
    public async Task<IActionResult> UpdateStatus(int eventId, string status)
    {
        var accessCheck = CheckAccess();
        if (accessCheck != null) return accessCheck;

        var eventResponse = await _api.SendAsync(HttpMethod.Get, $"api/events/{eventId}");
        var ev = await _api.ReadAsync<EventDto>(eventResponse);
        if (ev == null) return NotFound();

        var updateReq = new UpdateEventRequest(ev.Title, ev.Description, ev.StartDate, ev.EndDate, ev.Location, ev.DistanceKm, ev.EventType, status);
        await _api.SendAsync(HttpMethod.Put, $"api/events/{eventId}", updateReq);

        TempData["Success"] = $"Event status updated to {status}.";
        return RedirectToAction(nameof(ManageEvent), new { id = eventId });
    }

    // POST: /Organiser/AddCategory
    [HttpPost]
    public async Task<IActionResult> AddCategory(int eventId, string name, decimal distanceKm, decimal entryFee, string startTime)
    {
        var accessCheck = CheckAccess();
        if (accessCheck != null) return accessCheck;

        if (!TimeOnly.TryParse(startTime, out TimeOnly parsedTime))
        {
            TempData["Error"] = "Invalid time format.";
            return RedirectToAction(nameof(ManageEvent), new { id = eventId });
        }

        var request = new CreateCategoryRequest(
            Name: name,
            Description: null,
            DistanceKm: distanceKm,
            EntryFee: entryFee,
            StartTime: parsedTime,
            MaxParticipants: null
        );

        var response = await _api.SendAsync(HttpMethod.Post, $"api/events/{eventId}/categories", request);

        if (response.IsSuccessStatusCode)
        {
            TempData["Success"] = "Category added successfully.";
        }
        else
        {
            string errorDetails = await response.Content.ReadAsStringAsync();
            TempData["Error"] = $"API Error: {errorDetails}";
        }

        return RedirectToAction(nameof(ManageEvent), new { id = eventId });
    }

    // POST: /Organiser/ApproveEnrolment
    [HttpPost]
    public async Task<IActionResult> ApproveEnrolment(int eventId, int enrolId)
    {
        var accessCheck = CheckAccess();
        if (accessCheck != null) return accessCheck;

        var req = new UpdateEnrolmentStatusRequest("Confirmed", null);
        await _api.SendAsync(HttpMethod.Put, $"api/enrolments/{enrolId}/status", req);

        TempData["Success"] = "Enrolment confirmed.";
        return RedirectToAction(nameof(ManageEvent), new { id = eventId });
    }

    // POST: /Organiser/CaptureResult
    [HttpPost]
    public async Task<IActionResult> CaptureResult(int eventId, CreateResultRequest model)
    {
        var accessCheck = CheckAccess();
        if (accessCheck != null) return accessCheck;

        var response = await _api.SendAsync(HttpMethod.Post, "api/results", model);

        if (response.IsSuccessStatusCode)
            TempData["Success"] = "Result captured successfully.";
        else
            TempData["Error"] = "Failed to capture result. Enrolment must be Confirmed.";

        return RedirectToAction(nameof(ManageEvent), new { id = eventId });
    }

    // POST: /Organiser/UploadBanner
    [HttpPost]
    public async Task<IActionResult> UploadBanner(int eventId, IFormFile bannerFile)
    {
        var accessCheck = CheckAccess();
        if (accessCheck != null) return accessCheck;

        if (bannerFile == null || bannerFile.Length == 0)
        {
            TempData["Error"] = "Please select an image file.";
            return RedirectToAction(nameof(ManageEvent), new { id = eventId });
        }

        // Prepare the file to send to the API
        using var content = new MultipartFormDataContent();
        var fileContent = new StreamContent(bannerFile.OpenReadStream());
        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(bannerFile.ContentType);
        content.Add(fileContent, "file", bannerFile.FileName);

        // Send the POST request to the API
        var response = await _api.SendAsync(HttpMethod.Post, $"api/events/{eventId}/banner", content);

        if (response.IsSuccessStatusCode)
        {
            TempData["Success"] = "Banner uploaded successfully.";
        }
        else
        {
            // Read the exact error from the API
            string errorDetails = await response.Content.ReadAsStringAsync();
            TempData["Error"] = $"Upload failed: {errorDetails}";
        }

        return RedirectToAction(nameof(ManageEvent), new { id = eventId });
    }
}