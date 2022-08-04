using Newtonsoft.Json;

namespace HomeAssistant.API.Models;
public class Light
{
    public String Name { get; init; }

    public String Status { get; set; } = "off";
    public int Brightness { get; set; } = 0;
}