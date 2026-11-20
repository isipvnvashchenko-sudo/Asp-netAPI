using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using WebApplicationStas.Models;

namespace WebApplicationStas.Authorization;

public class SameUserOrAdminRequirement : IAuthorizationRequirement
{
}

public class SameUserOrAdminUserHandler : AuthorizationHandler<SameUserOrAdminRequirement, User>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, SameUserOrAdminRequirement requirement, User resource)
    {
        if (context.User.IsInRole("Admin") || OwnsResource(context.User, resource.Id))
            context.Succeed(requirement);

        return Task.CompletedTask;
    }

    private static bool OwnsResource(ClaimsPrincipal user, int targetId)
        => int.TryParse(user.FindFirstValue(ClaimTypes.NameIdentifier), out var id) && id == targetId;
}

public class SameUserOrAdminReservationHandler : AuthorizationHandler<SameUserOrAdminRequirement, ProductReservation>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, SameUserOrAdminRequirement requirement, ProductReservation resource)
    {
        if (context.User.IsInRole("Admin") || OwnsResource(context.User, resource.UserId))
            context.Succeed(requirement);

        return Task.CompletedTask;
    }

    private static bool OwnsResource(ClaimsPrincipal user, int ownerId)
        => int.TryParse(user.FindFirstValue(ClaimTypes.NameIdentifier), out var id) && id == ownerId;
}
