using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LightApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController:ControllerBase
{
    [HttpPost("login"), AllowAnonymous]
    public async Task<IActionResult> Login([FromQuery] string name, [FromQuery] string passsword)
    {
        var userClaims = new List<Claim>()
        {
            new("id", "1"),
        };

        var claimsIdentity = new ClaimsIdentity(userClaims, CookieAuthenticationDefaults.AuthenticationScheme);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),new AuthenticationProperties()
            {
                IsPersistent = true,
                AllowRefresh = true
            });

        return Ok();
    }
    
    /// <summary>
    /// 注销登录
    /// </summary>
    /// <returns></returns>
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        return Ok();
    }

    [HttpGet("test")]
    public IActionResult Test()
    {
        return Ok("hello");
    }
}