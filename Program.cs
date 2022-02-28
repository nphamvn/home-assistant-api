using HomeAssistant.Api.Middlewares;
using HomeAssistant.API.Data;
using HomeAssistant.API.Extentions;
using HomeAssistant.API.Hubs;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.ConfigureServices(builder.Configuration);
builder.Services.AddSignalR();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<ApplicationDbContext>();
    if (context.Database.GetPendingMigrations().Any())
    {
        context.Database.Migrate();
    }
    await Seed.SeedUsers(services);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(SeviceCollectionExtentions.CORS_POLICY_NAME);

// Authentication & Authorization
app.UseAuthorization();
app.UseAuthorization();

app.MapControllers();
app.MapHub<ChatHub>("hubs/chat");
app.Run();
