using Microsoft.AspNetCore.Mvc;
using RaceDay.Contracts;
using RaceDay.Mvc.Services;
using RaceDay.Mvc.ViewModels;

namespace RaceDay.Mvc.Controllers;

public class AuthController : Controller
{
    private readonly RaceDayApiClient _api;
    public AuthController(RaceDayApiClient api) => _api = api;

    // GET: /Auth/Register
    [HttpGet]
    public IActionResult Register() => View();

    // POST: /Auth/Register
    [HttpPost]
    public async Task<IActionResult> Register(RegisterVm vm)
    {
        if (!ModelState.IsValid) return View(vm);

        var request = new RegisterRequest(vm.FirstName, vm.LastName, vm.Email, vm.Password, vm.Role);
        var response = await _api.SendAsync(HttpMethod.Post, "api/auth/register", request);

        if (!response.IsSuccessStatusCode)
        {
            ModelState.AddModelError("", "Registration failed. Email might already exist.");
            return View(vm);
        }

        TempData["Success"] = "Registration successful. Please log in.";
        return RedirectToAction(nameof(Login));
    }

    // GET: /Auth/Login
    [HttpGet]
    public IActionResult Login() => View();

    // POST: /Auth/Login
    [HttpPost]
    public async Task<IActionResult> Login(LoginVm vm)
    {
        if (!ModelState.IsValid) return View(vm);

        var request = new LoginRequest(vm.Email, vm.Password);
        var response = await _api.SendAsync(HttpMethod.Post, "api/auth/login", request);

        if (!response.IsSuccessStatusCode)
        {
            ModelState.AddModelError("", "Invalid email or password.");
            return View(vm);
        }

        var auth = await _api.ReadAsync<AuthResponse>(response);
        if (auth == null)
        {
            ModelState.AddModelError("", "Error reading API response.");
            return View(vm);
        }

        // Save the API cookie to the MVC session
        string? apiCookie = RaceDayApiClient.ExtractSessionCookie(response);
        if (!string.IsNullOrWhiteSpace(apiCookie))
            HttpContext.Session.SetString("ApiSessionCookie", apiCookie);

        // Save user details to MVC session
        HttpContext.Session.SetInt32("UserId", auth.UserId);
        HttpContext.Session.SetString("Role", auth.Role);
        HttpContext.Session.SetString("DisplayName", auth.FullName ?? auth.Email);

        // Redirect based on role
        if (auth.Role.Equals("Organiser", StringComparison.OrdinalIgnoreCase))
            return RedirectToAction("Index", "Organiser");

        return RedirectToAction("Index", "Participant");
    }

    // POST: /Auth/Logout
    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await _api.SendAsync(HttpMethod.Post, "api/auth/logout");
        HttpContext.Session.Clear();
        return RedirectToAction("Index", "Home");
    }
}