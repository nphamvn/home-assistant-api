namespace HomeAssistant.API.Services.Interfaces;

public interface ICacheService
{
    Task Set<T>(string key, T value);
    Task<T> Get<T>(string key);
    void Remove(string key);
}
