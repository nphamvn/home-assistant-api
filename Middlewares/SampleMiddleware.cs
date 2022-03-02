namespace HomeAssistant.API.Middlewares;

public class SampleMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        //throw new NotImplementedException();
        Console.WriteLine("Sample Middleware");
    }
}