using System.Text.Json;
using HomeAssistant.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace HomeAssistant.API.Controller;
[ApiController]
[Route("api/[controller]")]
public class LightController : ControllerBase
{
    private readonly LightService lightService;

    public LightController(LightService lightService)
    {
        this.lightService = lightService;
    }
    [HttpGet]
    public async Task<IActionResult> get([FromQuery] String name)
    {
        return Ok(await lightService.Get(name));
    }

    [HttpPost]
    public async Task<IActionResult> set(String name, String action)
    {
        if (action == "on")
        {
            return Ok(await lightService.TurnOn(name));
        }
        else if (action == "off")
        {
            return Ok(await lightService.TurnOff(name));
        }
        else if (action == "night")
        {
            return Ok(await lightService.TurnOnNightMode(name));
        }
        else if (action == "brighten")
        {
            return Ok(await lightService.Brighten(name));
        }
        else if (action == "dim")
        {
            return Ok(await lightService.Dim(name));
        }
        return BadRequest();
    }
}