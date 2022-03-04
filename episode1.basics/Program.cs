using System.Security.Claims;
using episode1.basics.AuthorizationRequirements;
using episode1.basics.Controllers;
using episode1.basics.CustomAuthorizationProvider;
using episode1.basics.Data;
using episode1.basics.Transformer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllersWithViews(config =>
{
    var defaultAuthBuilder = new AuthorizationPolicyBuilder();
    var defaultAuthPolicy = defaultAuthBuilder
        .RequireAuthenticatedUser()
        .Build();

    // global authorization filter
    // config.Filters.Add(new AuthorizeFilter(defaultAuthPolicy));
});

// Add new cookieauthentication
builder.Services.AddAuthentication("CookieAuth")
.AddCookie("CookieAuth", config =>
{
    config.Cookie.Name = "Abubakr.Cookie";

    // change default account/login redirection to custom home/authenticate
    config.LoginPath = "/Home/Authenticate";
});

builder.Services.AddAuthorization(config =>
{
    // var defaultAuthBuilder = new AuthorizationPolicyBuilder();
    // var defaultAuthPolicy = defaultAuthBuilder
    // .RequireAuthenticatedUser()
    // .RequireClaim(ClaimTypes.DateOfBirth)
    // .Build();

    // config.DefaultPolicy = defaultAuthPolicy;

    // config.AddPolicy("Claim.DoB", policyBuilder => 
    // {
    //     policyBuilder.RequireClaim(ClaimTypes.DateOfBirth);
    // });

    config.AddPolicy("Claim.DoB", policyBuilder => 
    {
        policyBuilder.RequireCustomClaim(ClaimTypes.DateOfBirth);
    });

    config.AddPolicy("Admin", policyBuilder 
        => policyBuilder.RequireClaim(ClaimTypes.Role, "Admin"));
});
builder.Services.AddSingleton<IAuthorizationPolicyProvider, CustomAuthorizationPolicyProvider>();
builder.Services.AddScoped<IAuthorizationHandler, SecurityLevelHandler>();
builder.Services.AddScoped<IAuthorizationHandler, CustomRequireClaimHandler>();
builder.Services.AddScoped<IAuthorizationHandler, CookieJarAuthorizationFilter>();
builder.Services.AddScoped<IClaimsTransformation, ClaimsTransformation>();

var app = builder.Build();

if(app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapDefaultControllerRoute();

app.Run();
