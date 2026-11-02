using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RaceDay.Contracts;
using RaceDay.Mvc.Services;

namespace RaceDay.Mvc.Controllers
{
    public class ParticipantController : BaseController
    {
        private readonly RaceDayApiClient _api;

        public ParticipantController(RaceDayApiClient api)
        {
            _api = api;
        }

        private IActionResult? CheckAccess()
        {
            // These are inherited from BaseController
            if (!IsLoggedIn())
            {
                return RedirectToAction("Login", "Auth");
            }

            if (!IsParticipant()) return RedirectToAction("Index", "Home");
            return null;
        }

        // GET: /Participant
        public async Task<IActionResult> Index()
        {
            var accessCheck = CheckAccess();
            if (accessCheck != null) return accessCheck;

            var response = await _api.SendAsync(HttpMethod.Get, "api/events");
            var events = await _api.ReadAsync<List<EventDto>>(response) ?? new List<EventDto>();

            var upcomingEvents = events
                .Where(e => e.StartDate >= DateTime.Today && e.Status == "Open")
                .OrderBy(e => e.StartDate)
                .ToList();

            return View(upcomingEvents);
        }

        // GET: /Participant/EventDetails/5
        public async Task<IActionResult> EventDetails(int id)
        {
            var accessCheck = CheckAccess();
            if (accessCheck != null) return accessCheck;

            var eventResponse = await _api.SendAsync(HttpMethod.Get, $"api/events/{id}");
            var eventDto = await _api.ReadAsync<EventDto>(eventResponse);

            if (eventDto == null) return NotFound();

            var catResponse = await _api.SendAsync(HttpMethod.Get, $"api/events/{id}/categories");
            var categories = await _api.ReadAsync<List<CategoryDto>>(catResponse) ?? new List<CategoryDto>();

            ViewBag.Event = eventDto;
            ViewBag.Categories = categories;

            return View();
        }

        // POST: /Participant/Enrol
        [HttpPost]
        public async Task<IActionResult> Enrol(int eventId, int categoryId)
        {
            var accessCheck = CheckAccess();
            if (accessCheck != null) return accessCheck;

            var request = new CreateEnrolmentRequest(eventId, categoryId);
            var response = await _api.SendAsync(HttpMethod.Post, "api/enrolments", request);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "You have successfully enrolled in this event!";
            }
            else
            {
                TempData["Error"] = "Failed to enrol. You might already be enrolled, or the event isn't open.";
            }

            return RedirectToAction(nameof(MyEvents));
        }

        // GET: /Participant/MyEvents
        public async Task<IActionResult> MyEvents()
        {
            var accessCheck = CheckAccess();
            if (accessCheck != null) return accessCheck;

            var response = await _api.SendAsync(HttpMethod.Get, "api/enrolments/mine");
            var enrolments = await _api.ReadAsync<List<EnrolmentDto>>(response) ?? new List<EnrolmentDto>();

            return View(enrolments);
        }

        // GET: /Participant/MyResults
        public async Task<IActionResult> MyResults()
        {
            var accessCheck = CheckAccess();
            if (accessCheck != null) return accessCheck;

            var response = await _api.SendAsync(HttpMethod.Get, "api/results/mine");
            var results = await _api.ReadAsync<List<ResultDto>>(response) ?? new List<ResultDto>();

            return View(results);
        }
    }
}