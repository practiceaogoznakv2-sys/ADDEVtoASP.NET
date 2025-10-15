using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Server.IISIntegration;
using Microsoft.AspNetCore.Diagnostics;
using AccessManagerWeb.Infrastructure.Data;
using AccessManagerWeb.Infrastructure.Repositories;
using AccessManagerWeb.Infrastructure.Services;
using AccessManagerWeb.Core.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Настройка Kestrel
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.AddServerHeader = false;
    serverOptions.Limits.MaxConcurrentConnections = 100;
    serverOptions.Limits.MaxRequestBodySize = 10 * 1024 * 1024; // 10MB
    serverOptions.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(2);
    serverOptions.Limits.RequestHeadersTimeout = TimeSpan.FromSeconds(30);
});

// Настройка URLS через конфигурацию
builder.WebHost.UseUrls(builder.Configuration["Urls:0"], builder.Configuration["Urls:1"]);

// Добавляем поддержку контроллеров и JSON
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = null;
    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
});

// Настройка Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Access Manager API",
        Version = "v1"
    });
});

// Настройка CORS
builder.Services.AddCors(options =>
{
    var origins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(origins)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// Настройка Windows Authentication
builder.Services.AddAuthentication(IISDefaults.AuthenticationScheme);

// Настройка AD аутентификации
builder.Services.Configure<IISOptions>(options =>
{
    options.AutomaticAuthentication = true;
});

// Настройка DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Регистрация сервисов
builder.Services.AddScoped<IResourceRequestRepository, ResourceRequestRepository>();
builder.Services.AddScoped<IActiveDirectoryService>(sp => 
    new ActiveDirectoryService(
        builder.Configuration["ActiveDirectory:Domain"],
        builder.Configuration["ActiveDirectory:Container"]
    ));
builder.Services.AddScoped<IEmailService>(sp => 
    new EmailService(
        builder.Configuration["Email:SmtpServer"],
        int.Parse(builder.Configuration["Email:SmtpPort"]),
        builder.Configuration["Email:FromAddress"]
    ));

// Настройка глобальной авторизации
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

var app = builder.Build();

// Глобальный обработчик ошибок
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";
        var error = context.Features.Get<IExceptionHandlerFeature>();
        if (error != null)
        {
            await context.Response.WriteAsJsonAsync(new
            {
                StatusCode = 500,
                Message = "An internal server error occurred."
            });
        }
    });
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseHsts();
}

// Правильный порядок middleware
app.UseHttpsRedirection();
app.UseRouting();
app.UseCors();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Добавляем health check endpoint
app.MapGet("/health", () => Results.Ok("Healthy"));

try
{
    app.Run();
}
catch (Exception ex)
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogCritical(ex, "Application terminated unexpectedly");
}
