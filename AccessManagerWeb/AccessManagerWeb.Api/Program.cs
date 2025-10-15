using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Server.IISIntegration;
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
builder.Services.AddAuthentication(Microsoft.AspNetCore.Server.IISIntegration.IISDefaults.AuthenticationScheme);

// Настройка AD аутентификации
builder.Services.Configure<IISOptions>(options =>
{
    options.AutomaticAuthentication = true;
    options.ForwardWindowsAuthToken = true;
});
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

// Регистрация сервиса AD аутентификации
builder.Services.AddScoped<ADAuthenticationService>();

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
        Description = "API with JWT Authentication"
    });

    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement()
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = Microsoft.OpenApi.Models.ParameterLocation.Header,
            },
            new List<string>()
        }
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

// Настройка глобальной авторизации
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

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
app.UseAuthorization();

app.MapControllers();

app.Run();
