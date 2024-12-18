using LightApi.Infra;
using LightApi.Infra.RabbitMQ;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LightApi.Api.Controllers.v1;

[AllowAnonymous]
[ApiController]
[Route("api/[controller]/[action]")]
public class RabbitMqController:ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Test()
    {
        await App.GetService<RabbitMqManager>()!.GetPublisher().PublishAsync<string>("test_a","aaa");
        return Ok();
    }
}