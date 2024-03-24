namespace Infrastructure.Services.Implementations;

using Infrastructure.Extensions;
using Infrastructure.Services.Interfaces;
using Infrastructure.Settings;

using Microsoft.Extensions.Options;

using Serilog;

using StackExchange.Redis;

/// <inheritdoc cref="ICacheService"/>
public class CacheService : ICacheService
{
    /// <summary>
    /// Redis база данных.
    /// </summary>
    private readonly IDatabase _cache;

    /// <summary>
    /// Время хранения кэша.
    /// </summary>
    private readonly TimeSpan _cacheLifetime;

    /// <summary>
    /// Логгер.
    /// </summary>
    private readonly ILogger _logger;

    /// <inheritdoc cref="ICacheService"/>
    /// <param name="redisProvider">Поставщик redis.</param>
    /// <param name="cacheSettings">Настройки кэширования.</param>
    /// <param name="logger">Логгер.</param>
    public CacheService(IRedisProvider redisProvider, IOptions<CacheSettings> cacheSettings, ILogger logger)
    {
        _logger = logger;
        _logger.Debug($"Инициализация: {nameof(CacheService)}");

        _cacheLifetime = TimeSpan.Parse(cacheSettings.Value.CacheLifetimeString);
        _cache = redisProvider.GetDatabase();

        _logger.Debug($"{nameof(CacheService)} - инициализирован.");
    }

    /// <inheritdoc />
    public async Task<TType?> GetAsync<TType>(object key, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _logger.Information("Получение значения из кэша через сервис.");
        _logger.Debug("Ключ, по которому необходимо получить значение из кэша: {key}", key);

        var keyString = key.ToString();
        var objectJsonString = await _cache.StringGetAsync(keyString);
        if (!objectJsonString.HasValue)
        {
            _logger.Information("Получение значения из кэша через сервис - завершено.");
            _logger.Debug("Значение не было найдено в кэше.");

            return default;
        }

        var objectInstance = objectJsonString.ToString()
                                             .CreateInstance<TType>();

        _logger.Information("Получение значения из кэша через сервис - завершено.");
        _logger.Debug("Полученное из кэша значение: {objectInstance}.", objectInstance);

        return objectInstance;
    }

    /// <inheritdoc />
    public async Task SetAsync(object key, object value, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _logger.Information("Установить значение в кэш через сервис.");
        _logger.Debug("Ключ, по которому необходимо установить значение в кэш: {key}", key);
        _logger.Debug("Значение, которое необходимо установить в кэш: {value}", value);

        var keyString = key.ToString();
        var valueJsonString = value.ToJsonString();

        await _cache.StringSetAsync(keyString, valueJsonString, _cacheLifetime);

        _logger.Information("Установить значение в кэш через сервис - успешно.");
    }

    /// <inheritdoc />
    public async Task DeleteAsync(object key, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _logger.Information("Удалить значение из кэша через сервис.");
        _logger.Debug("Ключ, по которому необходимо удалить значение: {key}", key);

        var keyString = key.ToString();
        var redisKey = new RedisKey(keyString);
        await _cache.KeyDeleteAsync(redisKey);

        _logger.Information("Удалить значение из кэша через сервис - успешно.");
    }

    /// <inheritdoc />
    public async Task<TType?> WrapCacheOperationsAsync<TType>(object key, Func<Task<TType?>> asyncDelegate,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _logger.Information("Обертывание операций кэша через сервис.");
        _logger.Debug("Ключ, по которому необходимо получить и сохранить значение: {key}", key);

        var gotCache = await GetAsync<TType>(key, cancellationToken);
        if (gotCache is not null)
        {
            _logger.Information("Обертывание операций кэша через сервис - завершено.");
            _logger.Debug("Получено значение из кэша: {gotCache}.", gotCache);

            return gotCache;
        }

        var resultForCache = await asyncDelegate();
        if (resultForCache is null)
        {
            _logger.Information("Обертывание операций кэша через сервис - завершено.");
            _logger.Information("Не удалось получить значение ни из кэша, ни из метода поставщика.");

            return resultForCache;
        }

        await SetAsync(key, resultForCache, cancellationToken);

        _logger.Information("Обертывание операций кэша через сервис - завершено.");
        _logger.Information("Полученное и сохраненное в кэш значение поставщика: {resultForCache}", resultForCache);

        return resultForCache;
    }
}