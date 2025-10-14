using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using AccessManagerWeb.Infrastructure.Data;
using AccessManagerWeb.Infrastructure.Repositories;
using AccessManagerWeb.Infrastructure.Services;
using AccessManagerWeb.Core.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Настройка URLS через конфигурацию
builder.WebHost.UseUrls("http://0.0.0.0:5080", "https://0.0.0.0:5081");

builder.Services.AddControllers();

// Add services to the container
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register repositories
builder.Services.AddScoped<IResourceRequestRepository, ResourceRequestRepository>();

// Register services
builder.Services.AddSingleton<IActiveDirectoryService>(sp => 
    new ActiveDirectoryService(
        builder.Configuration["ActiveDirectory:Domain"],
        builder.Configuration["ActiveDirectory:Container"]));

builder.Services.AddSingleton<IEmailService>(sp => 
    new EmailService(
        builder.Configuration["Email:SmtpServer"],
        int.Parse(builder.Configuration["Email:SmtpPort"]),
        builder.Configuration["Email:FromAddress"]));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Добавляем детальное логирование
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.SetMinimumLevel(LogLevel.Trace);

var app = builder.Build();

// Добавляем обработку ошибок
app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (Exception ex)
    {
        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Необработанная ошибка: {Message}", ex.Message);
        throw;
    }
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
