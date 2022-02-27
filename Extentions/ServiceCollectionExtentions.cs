using System.Text;
using HomeAssistant.API.Data;
using HomeAssistant.API.Models;
using HomeAssistant.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace HomeAssistant.API.Extentions;

public static class SeviceCollectionExtentions
{
    public static void ConfigureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtOptions>(configuration.GetSection(nameof(JwtOptions)));

        //Add CORS
        services.AddCors(options =>
        {
            options.AddPolicy(name: CORS_POLICY_NAME,
            builder =>
            {
                builder.WithOrigins("http://localhost:4200", "https://nphamvn.github.io")
                        .AllowAnyHeader()
                        .AllowAnyMethod();
            });
        });

        //Add database
        AddDatabase(services, configuration);

        // Add Identity
        services.AddIdentity<AppUser, AppRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        //Add Authenitcation
        var jwtOptions = new JwtOptions();
        configuration.GetSection(nameof(JwtOptions)).Bind(jwtOptions);

        // Adding Authentication
        services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            // Adding Jwt Bearer
            .AddJwtBearer(x =>
            {
                x.SaveToken = true;
                x.RequireHttpsMetadata = false;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Secret)),
                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience
                };
            });

        //Add Authorization
        services.AddAuthorization(x =>
        {
            x.AddPolicy(JwtBearerDefaults.AuthenticationScheme, builder =>
            {
                builder.RequireAuthenticatedUser();
            });
        });

        services.AddScoped<TokenService>();
        services.AddScoped<RefreshTokenService>();
    }
    public static string CORS_POLICY_NAME = "_myAllowSpecificOrigins";

    public static void AddDatabase(this IServiceCollection service, IConfiguration configuration)
    {
        if (configuration["DatabaseProvider"] == "Sqlite")
        {
            service.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(configuration.GetConnectionString("Sqlite")));
        }
        else if (configuration["DatabaseProvider"] == "SqlServer")
        {
            service.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("SqlServer")));
        }
    }
}