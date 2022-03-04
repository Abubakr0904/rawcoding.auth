using Microsoft.AspNetCore.Authorization;

namespace episode1.basics.AuthorizationRequirements;

public static class AuthorizationPolicyBuilderExtensions
{
    public static AuthorizationPolicyBuilder RequireCustomClaim(
        this AuthorizationPolicyBuilder builder,
        string claimType)
    {

        builder.AddRequirements(new CustomRequireClaim(claimType));
        return builder;
    }
}