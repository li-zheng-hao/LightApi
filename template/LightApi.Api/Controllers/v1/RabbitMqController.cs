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
    public IActionResult AS()
    {
        App.GetService<RabbitMqManager>()!.GetPublisher().BasicPublish<string>("test_a","aaa");
        return Ok();
    }
}