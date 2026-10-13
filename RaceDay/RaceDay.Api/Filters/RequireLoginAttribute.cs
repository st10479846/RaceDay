using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace RaceDay.Api.Filters;

/// <summary>
/// Marker attribute — apply to any controller or action that should be
/// accessible without logging in (e.g. register, login, public event list).
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AllowAnonymousAttribute : Attribute { }

/// <summary>
/// Global filter — rejects any request that doesn't have an active session
/// with 401 Unauthorized, unless the endpoint is marked [AllowAnonymous].
/// </summary>
public class RequireLoginAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        // Skip the check if the action (or its controller) is [AllowAnonymous]
        var isAnonymous = context.ActionDescriptor.EndpointMetadata
            .OfType<AllowAnonymousAttribute>()
            .Any();

        if (isAnonymous) return;

        // Check the session
        var userId = context.HttpContext.Session.GetUserId();
        if (userId == null)
        {
            context.Result = new UnauthorizedObjectResult(
                new { error = "You must be logged in to access this resource." });
        }
    }
}