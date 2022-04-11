using System.Security.Claims;
using episode1.basics.CustomAuthorizationProvider;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace episode1.basics.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    [Authorize]
    public IActionResult Secret()
    {
        return View();
    }

    [Authorize(Policy = "Claim.DoB")]
    public IActionResult SecretPolicy()
    {
        return View("Secret");
    }

    [Authorize(Roles = "Admin")]
    public IActionResult SecretRole()
    {
        return View("Secret");
    }

    [SecurityLevel(5)]
    public IActionResult SecretLevel()
    {
        return View("Secret");
    }

    [SecurityLevel(10)]
    public IActionResult SecretHigherLevel()
    {
        return View("Secret");
    }

    [AllowAnonymous]
    public IActionResult Authenticate()
    {
        // o'qituvchining userPrincipal haqida bildirgan da'volaridan(claim) tashkil topgan list
        var teacherClaims = new List<Claim>()
        {
            new Claim(ClaimTypes.Name, "Abubakr"),
            new Claim(ClaimTypes.Email, "abubakr@gmail.com"),
            new Claim(ClaimTypes.DateOfBirth, "09/04/2001"),
            new Claim(ClaimTypes.Role, "Admin"),
            new Claim(DynamicPolicies.SecurityLevel, "7"),
            new Claim("Qobiliyatlari", "Yaxshi o'qiydi."),
        };

        // Hukumat idorasining fikricha userPrincipal qanday claim'larga ega bo'lishi kerak.
        var HukumatClaims = new List<Claim>()
        {
            // hukumat idorasi uning to'liq ismini biladi va shu ismni talab qiladi
            new Claim(ClaimTypes.Name, "Abubakr Baxromov Dilshod o'g'li"),
            new Claim("Manzili", "Fergana region")
        };

        // Abubakr'ning claim'lari bo'yicha identity yaratamiz.
        // Ya'ni uning fikricha userPrincipal ushbu claim'larga mos tushishi kerak. 
        
        // AbubakrClaims'dagi barcha claim'lar identity'ni tashkil qiladi.
        var abubakrIdentity = new ClaimsIdentity(teacherClaims, "Abubakr Identity");
        var HukumatIdentity = new ClaimsIdentity(HukumatClaims, "Hukumat");

        // Yangi UserPrincipal yaratiladi. 
        // Turli da'volarga ega bo'lgan berilgan identity'lardan tashkil topgan array - userPrincipal'ni hosil qiladi.   
        var userPrincipal = new ClaimsPrincipal(new[] { abubakrIdentity, HukumatIdentity });

        // sign in this userPrincipal
        // From this made userPrincipal get identity user and sign in it.
        // _signInManager.SignInAsync(_userManager.GetUserAsync(userPrincipal).GetAwaiter().GetResult(), false);
        HttpContext.SignInAsync(userPrincipal);
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> DoStuff(
        [FromServices]IAuthorizationService _authorizationService)
    {
        //we are doing stuff here

        var builder = new AuthorizationPolicyBuilder("Schema");
        var customPolicy = builder.RequireClaim("Hello").Build();

        var authResult =  await _authorizationService.AuthorizeAsync(HttpContext.User, customPolicy);

        if(authResult.Succeeded)
        {
            return View("Index");
        }
        return View("Index");
    }
}