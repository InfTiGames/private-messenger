using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Application.Interfaces;
using Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Serilog;
using System.Security.Claims;
using MassTransit;
using Infrastructure.Messaging;
using Microsoft.OpenApi.Models;

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
            IssuerSigningKey = new SymmetricSecurityKey(key),
            RoleClaimType = ClaimTypes.Role // Укажите имя claim для роли
        };
    });

// Добавление сервисов в контейнер
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Private Messenger API", Version = "v1" });
    options.AddSecurityDefinition(
        "Bearer",
        new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Введите токен JWT",
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer"
        }
    );
    options.AddSecurityRequirement(
        new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
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
builder.Services.AddScoped<MessageCacheService>();
builder.Services.AddScoped<IChatRepository, ChatRepository>();
builder.Services.AddScoped<IMessageQueueService, RabbitMqService>();

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<MessageConsumer>();

    x.UsingRabbitMq(
        (context, cfg) =>
        {
            cfg.Host(
                builder.Configuration["RabbitMQ:Host"],
                "/",
                h =>
                {
                    h.Username(
                        builder.Configuration["RabbitMQ:User"]
                            ?? throw new InvalidOperationException(
                                "RabbitMQ username is not configured."
                            )
                    );
                    h.Password(
                        builder.Configuration["RabbitMQ:Password"]
                            ?? throw new InvalidOperationException(
                                "RabbitMQ password is not configured."
                            )
                    );
                }
            );

            cfg.ReceiveEndpoint(
                "message-queue",
                e =>
                {
                    e.ConfigureConsumer<MessageConsumer>(context);
                }
            );
        }
    );
});

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "localhost:6379"; // Укажите адрес вашего Redis-сервера
    options.InstanceName = "PrivateMessenger_";
});

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
