using System.Text;
using Microsoft.AspNetCore.Authentication.OAuth;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(config => 
{
    // we check the cookie to confirm that we are authenticated
    config.DefaultAuthenticateScheme = "ClientCookie";
    // When we sign in we will deal out a cookie
    config.DefaultSignInScheme = "ClientCookie";
    // use this to check that we are allowed to do something.
    config.DefaultChallengeScheme = "OurServer";
})
    .AddCookie("ClientCookie")
    .AddOAuth("OurServer", config => 
    {
        config.ClientId = "client_id";
        config.ClientSecret = "client_secret";
        config.CallbackPath = "/oauth/callback";
        config.AuthorizationEndpoint = "https://localhost:7143/oauth/authorize";
        config.TokenEndpoint = "https://localhost:7143/oauth/token";

        config.SaveTokens = true;

        config.Events = new OAuthEvents()
        {
            OnCreatingTicket = context => 
            {
                var accessToken = context.AccessToken;
                var base64Payload = accessToken?.Split('.')[1];
                var bytes = Convert.FromBase64String(base64Payload ?? string.Empty);
                var jsonPayload = Encoding.UTF8.GetString(bytes);
                var claim = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonPayload);
                
                foreach(var item in claim)
                {
                    context?.Identity?.AddClaim(new System.Security.Claims.Claim(item.Key, item.Value));
                }
                
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddControllersWithViews()
    .AddRazorRuntimeCompilation();

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
