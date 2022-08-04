using HomeAssistant.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeAssistant.API.Controllers;
[AllowAnonymous]
[ApiController]
[Route("api/[controller]")]
public class WeatherController : ControllerBase
{
    private readonly WeatherService weatherService;

    public WeatherController(WeatherService weatherService)
    {
        this.weatherService = weatherService;
    }
    [AllowAnonymous]
    [HttpGet]
    [Route("outside")]
    public async Task<IActionResult> GetOutsideWeather()
    {
        var temperature = await weatherService.GetWeather(36.270295, 139.773257);
        if (temperature != null)
        {
            return Ok(temperature);
        }
        else
        {
            return NotFound();
        }
    }

    [HttpGet]
    [Route("inside")]
    public async Task<IActionResult> GetInsideWeather()
    {
        return Ok(new
        {
            TemperatureC = Random.Shared.Next(-25, 50)
        });
    }
}