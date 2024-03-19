namespace Tests.Integrations;

using System.Data.Common;

using Database;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Tests.Fixtures;

/// <summary>
/// Фабрика проекта WebApi.
/// </summary>
/// <typeparam name="TProgram">Класс Program проекта.</typeparam>
public class WebApiApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    /// <summary>
    /// Конфигурирование хоста.
    /// </summary>
    /// <param name="builder">Строитель приложения.</param>
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            RemoveApplicationDbContext(services);
            AddTestDbContextWithData(services);
        });

        builder.UseEnvironment("Development");

        builder.ConfigureAppConfiguration(configurationBuilder =>
        {
            configurationBuilder.AddJsonFile(AppSettingsFixture.GetTestAppsettingsPath);
        });
    }

    /// <summary>
    /// Удалить контекст бд приложения.
    /// </summary>
    /// <param name="services">Коллекция сервисов приложения.</param>
    private void RemoveApplicationDbContext(IServiceCollection services)
    {
        var dbContextDescriptor =
            services.SingleOrDefault(serviceDescriptor =>
                serviceDescriptor.ServiceType == typeof(DbContextOptions<AdminDbContext>));

        services.Remove(dbContextDescriptor!);

        var dbConnectionDescriptor =
            services.SingleOrDefault(serviceDescriptor => serviceDescriptor.ServiceType == typeof(DbConnection));

        services.Remove(dbConnectionDescriptor!);
    }

    /// <summary>
    /// Добавить контекст бд для теста.
    /// </summary>
    /// <param name="services">Коллекция сервисов приложения.</param>
    private void AddTestDbContextWithData(IServiceCollection services)
    {
        services.AddSingleton<DbConnection>(_ =>
        {
            var connection = new SqliteConnection(DatabaseFixture.CONNECTION_STRING);
            connection.Open();

            return connection;
        });

        services.AddDbContext<AdminDbContext>((container, options) =>
        {
            var connection = container.GetRequiredService<DbConnection>();
            options.UseSqlite(connection);

            var context = new AdminDbContext((DbContextOptions<AdminDbContext>)options.Options);
            DatabaseFixture.ReplaceDbWithFakeData(context);
        });
    }
}