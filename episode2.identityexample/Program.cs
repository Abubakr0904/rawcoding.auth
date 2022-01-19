using episode2.identityexample.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add Services
builder.Services.AddDbContext<AppDbContext>(config =>
{
    config.UseInMemoryDatabase("Memory");
});

// Add Identity registers the services 
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Password.RequiredLength = 4;
    options.Password.RequireDigit = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
})
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders(); 

builder.Services.ConfigureApplicationCookie(options =>
{
    // configure cookies for add identity
    options.Cookie.Name = "Identity.Cookie";
    options.LoginPath = "/Home/Login";
});

// // If identity authentication is being used, then the below code will not work! 
// // Instead use above service
// // configure cookies for custom cookie
// builder.Services.AddAuthentication("CookieAuth")
//     .AddCookie("CookieAuth", config =>
//     {
//         config.Cookie.Name = "Abubakrs.Cookie";
//         config.LoginPath = "Home/Authenticate";
//     });

builder.Services.AddControllersWithViews();

// Use defined services
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
