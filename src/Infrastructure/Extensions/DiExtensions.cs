namespace Infrastructure.Extensions;

using System.Reflection;

using Infrastructure.Comparers;
using Infrastructure.Middlewares;
using Infrastructure.Services.Implementations;
using Infrastructure.Services.Interfaces;

using Mapster;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

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
        services.AddTransient<RightsSortComparer>();
        services.AddTransient<ExternalGamesComparer>();

        TypeAdapterConfig.GlobalSettings.Scan(Assembly.GetExecutingAssembly());

        Log.Logger.Information("Сервисы добавлены.");
    }
}