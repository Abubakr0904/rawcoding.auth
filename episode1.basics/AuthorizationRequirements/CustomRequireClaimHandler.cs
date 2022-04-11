using Microsoft.AspNetCore.Authorization;

namespace episode1.basics.AuthorizationRequirements;

public class CustomRequireClaimHandler : AuthorizationHandler<CustomRequireClaim>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context, 
        CustomRequireClaim requirement)
    {
        var hasClaim = context.User.Claims.Any(x => x.Type == requirement.ClaimType);
        if(hasClaim)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}