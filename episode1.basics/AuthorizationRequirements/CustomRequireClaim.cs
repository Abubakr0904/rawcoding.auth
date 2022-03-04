using Microsoft.AspNetCore.Authorization;

namespace episode1.basics.AuthorizationRequirements;

public class CustomRequireClaim : IAuthorizationRequirement
{
    public CustomRequireClaim(string claimType)
    {
        ClaimType = claimType;
    }

    public string ClaimType { get; }
}
