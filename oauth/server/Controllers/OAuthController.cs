using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace server.Controllers;

public class OAuthController : Controller
{
    [HttpGet]
    public IActionResult Authorize(
        string response_type, // authorization flow type
        string client_id, // client id
        string redirect_uri,
        string scope,  // what info I want = email, grandma, tel
        string state // random string generated to confirm that we are going to back to the same client
    )
    {
        var query = new QueryBuilder();
        query.Add("redirectUri", redirect_uri);
        query.Add("state", state);
        return View(model: query?.ToString() ?? string.Empty);
    }
    [HttpPost]
    public IActionResult Authorize(
        string username,
        string redirectUri,
        string state
    )
    {
        const string code = "BABABABABABA";
        
        var query = new QueryBuilder();
        query.Add("code", code);
        query.Add("state", state);

        return Redirect($"{redirectUri}{query.ToString()}");
    }

    public async Task<IActionResult> Token(
        string grant_type, // flow of access token request
        string code, // confirmation of the authentication process
        string redirect_uri,
        string client_id
    )
    {
        // some mechanism for validating the code

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, "some_id"),
            new Claim("JwtRegisteredClaimNames.Sub", "cookie")
        };

        var secretBytes = Encoding.UTF8.GetBytes(Constants.Secret);
        var key = new SymmetricSecurityKey(secretBytes);
        var algorithm = SecurityAlgorithms.HmacSha256;

        var signingCredentials = new SigningCredentials(key, algorithm);

        var token = new JwtSecurityToken(
            Constants.Issuer,
            Constants.Audience,
            claims,
            DateTime.Now,
            DateTime.Now.AddDays(1),
            signingCredentials);

        var access_token = new JwtSecurityTokenHandler().WriteToken(token);

        var responseObject = new 
        {
            access_token,
            token_type = "Bearer",
            raw_claim = "oauthTutorial"
        };

        var responseJson = JsonConvert.SerializeObject(responseObject);
        var responseBytes = Encoding.UTF8.GetBytes(responseJson);
        await Response.Body.WriteAsync(responseBytes, 0, responseBytes.Length);

        return Redirect(redirect_uri);
    }
}