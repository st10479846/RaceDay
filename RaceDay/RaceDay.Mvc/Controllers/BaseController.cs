using Microsoft.AspNetCore.Mvc;

namespace RaceDay.Mvc.Controllers;

public abstract class BaseController : Controller
{
    protected bool IsLoggedIn()
    {
        return HttpContext.Session.GetInt32("UserId").HasValue;
    }

    protected bool IsOrganiser()
    {
        return string.Equals(HttpContext.Session.GetString("Role"), "Organiser", StringComparison.OrdinalIgnoreCase);
    }

    protected bool IsParticipant()
    {
        return string.Equals(HttpContext.Session.GetString("Role"), "Participant", StringComparison.OrdinalIgnoreCase);
    }
}