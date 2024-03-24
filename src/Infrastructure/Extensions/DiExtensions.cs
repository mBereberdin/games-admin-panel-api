namespace Infrastructure.Extensions;

using System.Reflection;

using Infrastructure.Comparers;
using Infrastructure.Middlewares;
using Infrastructure.Services.Implementations;
using Infrastructure.Services.Interfaces;
using Infrastructure.Settings;

using Mapster;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

using Serilog;

/// <summary>
/// Расширения для DI.
/// </summary>
public static class DiExtensions
{
    /// <summary>
    /// Добавить промежуточные слои приложения.
    /// </summary>
    /// <param name="builder">Строитель приложения.</param>
    public static void AddAppMiddlewares(this IApplicationBuilder builder)
    {
        Log.Logger.Information("Добавление промежуточных слоев приложения.");

        builder.UseMiddleware<ExceptionsMiddleware>();

        Log.Logger.Information("Промежуточные слои приложения добавлены.");
    }

    /// <summary>
    /// Добавить сервисы.
    /// </summary>
    /// <param name="services">Сервисы приложения.</param>
    public static void AddServices(this IServiceCollection services)
    {
        Log.Logger.Information("Добавление сервисов.");

        services.AddTransient<IUsersService, UsersService>();
        services.AddTransient<IPasswordsService, PasswordsService>();
        services.AddTransient<IGamesService, GamesService>();
        services.AddTransient<IRightsService, RightsService>();
        services.AddTransient<IJwtBuilder, JwtBuilder>();
        services.AddTransient<RightsSortComparer>();
        services.AddTransient<ExternalGamesComparer>();

        TypeAdapterConfig.GlobalSettings.Scan(Assembly.GetExecutingAssembly());

        Log.Logger.Information("Сервисы добавлены.");
    }

    /// <summary>
    /// Добавить аутентификацию.
    /// </summary>
    /// <param name="services">Сервисы приложения.</param>
    /// <param name="configuration">Конфигурация приложения.</param>
    public static void AddUsersAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        Log.Logger.Information("Добавление аутентификации.");

        var jwtSettings = configuration.GetSection(nameof(JwtSettings)).Get<JwtSettings>()!;

        services.Configure<JwtSettings>(configuration.GetSection(nameof(JwtSettings)));

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(jwtBearerOptions =>
                {
                    jwtBearerOptions.TokenValidationParameters =
                        new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,

                            ValidIssuer = jwtSettings.Issuer,
                            ValidAudience = jwtSettings.Audience,
                            IssuerSigningKey = jwtSettings.SecurityKey
                        };
                });

        Log.Logger.Information("Аутентификация добавлена.");
    }

    /// <summary>
    /// Добавить кэширование.
    /// </summary>
    /// <param name="services">Сервисы приложения.</param>
    /// <param name="configuration">Конфигурация приложения.</param>
    public static void AddCaching(this IServiceCollection services, IConfiguration configuration)
    {
        Log.Logger.Information("Добавление кэширования.");

        services.Configure<CacheSettings>(configuration.GetSection(nameof(CacheSettings)));

        Log.Logger.Information("Кэширование добавлено.");
    }
}