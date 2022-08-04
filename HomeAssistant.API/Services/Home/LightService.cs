using HomeAssistant.API.Models;
using RestSharp;

namespace HomeAssistant.API.Services;

public class LightService
{
    private readonly List<Light> lights = new List<Light>
    {
        new Light{Name = "bedroom"},
        new Light{Name = "kitchen"}
    };
    public async Task<Light> Get(String name)
    {
        return lights.Where(l => l.Name == name).First();
    }

    public async Task<Light> TurnOn(String name)
    {
        var light = lights.Find(l => l.Name == name);
        if (light != null)
        {
            light.Status = "on";
            return light;
        }
        else
        {
            return default;
        }
    }
    public async Task<Light> TurnOff(String name)
    {
        var light = lights.Find(l => l.Name == name);
        if (light != null)
        {
            light.Status = "off";
            return light;
        }
        else
        {
            return default;
        }
    }
    public async Task<Light> TurnOnNightMode(String name)
    {
        var light = lights.Find(l => l.Name == name);
        if (light != null)
        {
            light.Status = "night";
            return light;
        }
        else
        {
            return default;
        }
    }
    public async Task<Light> Brighten(String name)
    {
        var light = lights.Find(l => l.Name == name);
        if (light != null)
        {
            light.Brightness += 1;
            return light;
        }
        else
        {
            return default;
        }
    }
    public async Task<Light> Dim(String name)
    {
        var light = lights.Find(l => l.Name == name);
        if (light != null)
        {
            light.Brightness -= 1;
            return light;
        }
        else
        {
            return default;
        }
    }
}