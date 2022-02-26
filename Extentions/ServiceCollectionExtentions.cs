namespace HomeAssistant.API.Extentions;

public static class SeviceCollectionExtentions
{
    public static string CORS_POLICY_NAME = "_myAllowSpecificOrigins";
    public static void ConfigureCors(this IServiceCollection service)
    {
        service.AddCors(options =>
        {
            options.AddPolicy(name: CORS_POLICY_NAME,
            builder =>
            {
                builder.WithOrigins("http://localhost:4200", "https://nphamvn.github.io")
                        .AllowAnyHeader()
                        .AllowAnyMethod();
            });
        });
    }
}