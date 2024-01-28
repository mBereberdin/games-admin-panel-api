namespace Database.Extensions;

using System.Reflection;

using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Serilog;

/// <summary>
/// Расширения для DI.
/// </summary>
public static class DiExtensions
{
    /// <summary>
    /// Добавить контекст базы данных.
    /// </summary>
    /// <param name="services">Сервисы приложения.</param>
    /// <param name="configuration">Конфигурация приложения.</param>
    public static void AddDatabaseContext(this IServiceCollection services,
        IConfiguration configuration)
    {
        Log.Logger.Information("Добавление контекста базы данных.");

        services.AddDbContext<AdminDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString(nameof(AdminDbContext)),
                builder =>
                    builder.MigrationsAssembly(Assembly.GetExecutingAssembly()
                        .GetName().Name)));

        Log.Logger.Information("Контекст базы данных - добавлен.");
    }

    /// <summary>
    /// Применить миграции базы данных.
    /// </summary>
    /// <param name="builder">Строитель приложения.</param>
    public static void AddMigrateDatabase(this IApplicationBuilder builder)
    {
        Log.Logger.Information("Миграция базы данных.");

        using var scope = builder.ApplicationServices.CreateScope();
        using var context =
            scope.ServiceProvider.GetRequiredService<AdminDbContext>();

        context.Database.Migrate();

        Log.Logger.Information("Миграция базы данных - завершена.");
    }
}