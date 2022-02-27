namespace HomeAssistant.Api.Middlewares;

public class FirstMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        context.Request.Headers.TryGetValue("Authorization", out var token);
        Console.WriteLine(token);
        await next(context);
    }
}