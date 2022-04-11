using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.IdentityModel.Tokens;
using server;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication("OAuth2")
    .AddJwtBearer("OAuth2", config => 
    {
        var secretBytes = Encoding.UTF8.GetBytes(Constants.Secret);
        var key = new SymmetricSecurityKey(secretBytes);

        config.Events = new JwtBearerEvents()
        {
            OnMessageReceived = context => 
            {
                if(context.Request.Query.ContainsKey("access_token"))
                {
                    context.Token = context.Request.Query["access_token"];
                }

                return Task.CompletedTask;
            }
        };

        config.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidIssuer = Constants.Issuer,
            ValidAudience = Constants.Audience,
            IssuerSigningKey = key
        };
    });

builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();

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
