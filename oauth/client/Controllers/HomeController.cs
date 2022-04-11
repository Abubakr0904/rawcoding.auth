using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace client.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    [Authorize]
    public async Task<IActionResult> Secret()
    {
        // var token = HttpContext.GetTokenAsync("access_token");
        return View();
    }

}