using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Application.Interfaces;
using Infrastructure.Persistence.Repositories;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Настройка Serilog для логирования
Log.Logger = new LoggerConfiguration().ReadFrom
    .Configuration(configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

// Настройка JWT
var jwtSettings = configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = true;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"], // Without "Jwt" prefix!
            ValidAudience = jwtSettings["Audience"], // Without "Jwt" prefix!
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });

// Добавление сервисов в контейнер
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new() { Title = "PrivateMessenger API", Version = "v1" });

    // Настройка JWT Bearer Auth для Swagger
    options.AddSecurityDefinition(
        "Bearer",
        new Microsoft.OpenApi.Models.OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = Microsoft.OpenApi.Models.ParameterLocation.Header,
            Description = "Введите JWT токен: Bearer {your token}"
        }
    );

    options.AddSecurityRequirement(
        new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
        {
            {
                new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Reference = new Microsoft.OpenApi.Models.OpenApiReference
                    {
                        Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        }
    );
});

// Настройка базы данных
builder.Services.AddDbContext<AppDbContext>(
    options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// Регистрация зависимостей
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();

try
{
    Log.Information("Приложение запускается...");
    var app = builder.Build();
    Log.Information("Приложение построено успешно.");

    // Настройка middleware
    if (app.Environment.IsDevelopment())
    {
        Log.Information("Настройка Swagger...");
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "PrivateMessenger API V1");
            c.RoutePrefix = string.Empty; // Swagger UI доступен по корню
        });
    }

    Log.Information("Настройка HTTPS редиректа...");
    app.UseHttpsRedirection();

    Log.Information("Настройка аутентификации...");
    app.UseAuthentication(); // Обязательно ДО UseAuthorization

    Log.Information("Настройка авторизации...");
    app.UseAuthorization();

    Log.Information("Настройка маршрутов контроллеров...");
    app.MapControllers();

    Log.Information("Запуск приложения...");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Ошибка при запуске приложения");
}
finally
{
    Log.CloseAndFlush();
}
