using System.Security.Claims;
using Asp.Versioning;
using LightApi.Infra.Extension;
using LightApi.Service.Dtos.User;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace LightApi.Api.Controllers;
[ApiController]
[ApiVersion("1.0")]
[Route("api/[controller]/[action]")]
public class UserController : ControllerBase
{
    /// <summary>
    /// 登录
    /// </summary>
    /// <param name="loginDto"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        Check.ThrowIf(loginDto.UserName != "admin" || loginDto.Password != "admin","用户名或密码错误");
        var claimsIdentity = new ClaimsIdentity(new List<Claim>()
        {
            new Claim(ClaimTypes.Name,loginDto.UserName),
            new Claim(ClaimTypes.Role,"admin"),
            new Claim(ClaimTypes.Sid,"1")
        },CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(claimsIdentity);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,principal);
        var userInfo=new UserInfoDto()
        {
            UserId = 1,
            UserName = loginDto.UserName,
            UserPermissions = new List<string>() { "super-admin" },
            Roles = new List<string>() { "admin" }
        };
        return Ok(userInfo);
    }

    [HttpPost]
    public async Task<IActionResult> LogOut()
    {
        await HttpContext.SignOutAsync();
        return Ok();
    }
}