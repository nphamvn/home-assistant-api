using HomeAssistant.API.Models;
using RestSharp;

namespace HomeAssistant.API.Services;

public class WeatherService
{
    public async Task<Temperature> GetWeather(double lat, double lan)
    {
        var client = new RestClient("https://api.openweathermap.org/data/2.5");
        var request = new RestRequest("weather")
        .AddParameter("lat", lat)
        .AddParameter("lon", lan)
        .AddParameter("appid", "4fd5e90e32626b7ce67294b2d72ca1d9")
        .AddParameter("units", "metric");

        return await client.GetAsync<Temperature>(request);
    }
}