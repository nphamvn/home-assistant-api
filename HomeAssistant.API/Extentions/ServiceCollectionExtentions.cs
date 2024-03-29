using System.Text;
using AutoMapper;
using grpc;
using HomeAssistant.API.Data;
using HomeAssistant.API.DTOs;
using HomeAssistant.API.Entities;
using HomeAssistant.API.Middlewares;
using HomeAssistant.API.Models;
using HomeAssistant.API.Models.OptionModels;
using HomeAssistant.API.Services;
using HomeAssistant.API.Services.Chat;
using HomeAssistant.API.Services.Interfaces;
using MassTransit;
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
        services.Configure<CloudinaryOptions>(configuration.GetSection(nameof(CloudinaryOptions)));

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

                x.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];

                        // If the request is for our hub...
                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) &&
                            (path.StartsWithSegments("/hubs")))
                        {
                            // Read the token out of the query string
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
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
        services.AddTransient<IRepository<Message>, MessageRepository>();
        services.AddTransient<IRepository<Conversation>, ConversationRepository>();
        services.AddTransient<IRepository<AppUser>, UserRepository>();
        services.AddTransient<WeatherService>();
        services.AddSingleton<LightService>();
        services.AddTransient<PhotoStorageService>();
        // Add services to the container.
        //services.AddMemoryCache();
        services.AddDistributedMemoryCache();
        services.AddSingleton<ICacheService, CacheService>();
        services.AddSingleton<ConnectionManager>();
        services.AddSingleton<MessageService>();

        services.AddMassTransit(x =>
        {
            x.AddConsumer<UserConnectedEventHandler>();
            x.AddConsumer<MessageReceivedEventHandler>();
            x.AddConsumer<UserDisconnectedEventHandler>();

            x.UsingInMemory((context, cfg) =>
            {
                cfg.TransportConcurrencyLimit = 100;
                cfg.ConfigureEndpoints(context);
            });


        });
        services.AddMassTransitHostedService();

        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

        services.AddGrpcClient<Greeter.GreeterClient>(options =>
        {
            options.Address = new Uri(configuration["GRPC_SERVER_URL"]);
        })
        .ConfigureChannel(options =>
        {
            //options.Credentials = ChannelCredentials.Insecure;
            options.HttpHandler = new HttpClientHandler
            {
                //ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };
        });

        //services.AddHostedService<Workers.Worker>();

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
        else if (configuration["DatabaseProvider"] == "Postgres")
        {
            Console.WriteLine("ConnectionString: " + configuration.GetConnectionString("Postgres"));
            service.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("Postgres")));
        }
    }

    public static void AddMiddlewares(this IServiceCollection services)
    {
        services.AddTransient<SampleMiddleware>();
    }
}