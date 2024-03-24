namespace Infrastructure.Services.Implementations;

using Infrastructure.Services.Interfaces;
using Infrastructure.Settings;

using Microsoft.Extensions.Options;

using Serilog;

using StackExchange.Redis;

/// <inheritdoc cref="IRedisProvider"/>
public class RedisProvider : IRedisProvider, IDisposable
{
    /// <summary>
    /// Логгер.
    /// </summary>
    private readonly ILogger _logger;

    /// <summary>
    /// Redis база данных.
    /// </summary>
    private readonly IDatabase _cache;

    /// <summary>
    /// Соединение с redis бд.
    /// </summary>
    private readonly ConnectionMultiplexer _connection;

    /// <inheritdoc cref="IRedisProvider"/>
    /// <param name="cacheSettings">Настройки кэширования.</param>
    /// <param name="logger">Логгер.</param>
    public RedisProvider(IOptions<CacheSettings> cacheSettings, ILogger logger)
    {
        _logger = logger;
        _logger.Debug($"Инициализация: {nameof(RedisProvider)}.");

        var options = new ConfigurationOptions
        {
            EndPoints =
            {
                cacheSettings.Value.RedisUrl
            },
            Password = cacheSettings.Value.RedisPassword
        };

        _connection = ConnectionMultiplexer.Connect(options);
        _cache = _connection.GetDatabase();

        _logger.Debug($"{nameof(RedisProvider)}: инициализирован.");
    }

    /// <inheritdoc />
    public IDatabase GetDatabase()
    {
        _logger.Information("Предоставление redis бд через поставщик.");

        return _cache;
    }

    /// <summary>
    /// Освободить ресурсы объекта.
    /// </summary>
    public void Dispose()
    {
        _connection.Dispose();
    }
}