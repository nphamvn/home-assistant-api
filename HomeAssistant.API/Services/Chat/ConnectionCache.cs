using HomeAssistant.API.Services.Interfaces;

namespace HomeAssistant.API.Services.Chat;

public class ConnectionCache
{
    public Task Add<T>(string key, T value)
    {
        throw new NotImplementedException();
    }

    public Task<T> Get<T>(string key)
    {
        //throw new NotImplementedException();
        return Task.FromResult<T>(default(T));
    }

    public void Remove(string key)
    {
        throw new NotImplementedException();
    }
}