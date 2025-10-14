using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Server.IISIntegration;
using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.AspNetCore.Authorization;
using AccessManagerWeb.Infrastructure.Data;
using AccessManagerWeb.Infrastructure.Repositories;
using AccessManagerWeb.Infrastructure.Services;
using AccessManagerWeb.Core.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Настройка URLS через конфигурацию
builder.WebHost.UseUrls("http://localhost:5080", "https://localhost:5081");

// Настройка CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5080", "https://localhost:5081")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });

    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Настройка Windows Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = Microsoft.AspNetCore.Authentication.Negotiate.NegotiateDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = Microsoft.AspNetCore.Authentication.Negotiate.NegotiateDefaults.AuthenticationScheme;
}).AddNegotiate();

builder.Services.Configure<IISOptions>(options => 
{
    options.AutomaticAuthentication = true;
});

builder.Services.AddControllers();

// Настройка Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Access Manager API",
        Version = "v1",
        Description = "API использует Windows Authentication"
    });
});

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

// Настройка глобальной авторизации
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// Настройка CORS должна быть в начале конвейера
app.UseCors("AllowAll");

app.UseRouting();

// Настройка HTTPS после CORS и Routing
if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}
app.UseHttpsRedirection();

// Аутентификация и авторизация после HTTPS
app.UseAuthentication();
app.UseAuthorization();

// Добавляем обработку ошибок аутентификации
app.Use(async (context, next) =>
{
    await next();

    if (context.Response.StatusCode == 401)
    {
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(new
        {
            type = "https://tools.ietf.org/html/rfc9110#section-15.5.2",
            title = "Authentication required",
            status = 401,
            detail = "You must be authenticated to access this resource",
            instance = context.Request.Path
        });
    }
});

// Swagger после базовой настройки безопасности
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Access Manager API V1");
    c.RoutePrefix = string.Empty;
});

// Endpoints должны быть в конце
app.MapControllers();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
