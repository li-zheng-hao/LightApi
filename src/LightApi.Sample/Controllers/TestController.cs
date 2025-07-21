using LightApi.Infra;
using LightApi.Infra.Authorize.CustomCookie;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LightApi.Sample.Controllers;

[ApiController]
[Route("/api/test")]
public class TestController : ControllerBase
{
    [HttpPost("signout")]
    public async Task<IActionResult> SignOutAsync()
    {
        await HttpContext.SignOutAsync(CookieAuthSchemeOptions.SchemeName);
        return Ok();
    }

    [HttpPost("login"), AllowAnonymous]
    public IActionResult LoginAsync()
    {
        App.GetService<CookieGenerator>()!.AddCookie(HttpContext, "test");
        return Ok();
    }
}
