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
        _logger.Debug($"Инициализация: {nameof(CacheService)}.");

        _cacheLifetime = TimeSpan.Parse(cacheSettings.Value.CacheLifetimeString);
        _cache = redisProvider.GetDatabase();

        _logger.Debug($"{nameof(CacheService)} - инициализирован.");
    }

    /// <inheritdoc />
    public async Task<TType?> GetAsync<TType>(object? key, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _logger.Information("Получение значения из кэша через сервис.");
        _logger.Debug("Ключ, по которому необходимо получить значение из кэша: {key}.", key);

        if (key is null || (key is string stringKey && string.IsNullOrWhiteSpace(stringKey)))
        {
            _logger.Error("Невозможно получить значение кэша - без ключа кэша.");

            throw new ArgumentNullException(nameof(key), "Невозможно получить значение кэша - без ключа кэша.");
        }

        var keyString = key.ToString();
        var redisValue = await _cache.StringGetAsync(keyString);
        if (!redisValue.HasValue)
        {
            _logger.Information("Получение значения из кэша через сервис - завершено.");
            _logger.Debug("Значение не было найдено в кэше.");

            return default;
        }

        var objectInstance = redisValue.ToString()
                                       .CreateInstance<TType>();

        _logger.Information("Получение значения из кэша через сервис - завершено.");
        _logger.Debug("Полученное из кэша значение: {objectInstance}.", objectInstance);

        return objectInstance;
    }

    /// <inheritdoc />
    public async Task SetAsync(object? key, object? value, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _logger.Information("Установка значения в кэш через сервис.");
        _logger.Debug("Ключ, по которому необходимо установить значение в кэш: {key}.", key);
        _logger.Debug("Значение, которое необходимо установить в кэш: {value}.", value);

        if (key is null || (key is string stringKey && string.IsNullOrWhiteSpace(stringKey)))
        {
            _logger.Error("Невозможно установить значение кэша - без ключа кэша.");

            throw new ArgumentNullException(nameof(key), "Невозможно установить значение кэша - без ключа кэша.");
        }

        if (value is null || (value is string stringValue && string.IsNullOrWhiteSpace(stringValue)))
        {
            _logger.Error("Невозможно установить значение кэша - без значения кэша.");

            throw new ArgumentNullException(nameof(value), "Невозможно установить значение кэша - без значения кэша.");
        }

        var keyString = key.ToString();
        var valueJsonString = value.ToJsonString();

        await _cache.StringSetAsync(keyString, valueJsonString, _cacheLifetime);

        _logger.Information("Установка значения в кэш через сервис - успешно.");
    }

    /// <inheritdoc />
    public async Task DeleteAsync(object? key, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _logger.Information("Удаление значения из кэша через сервис.");
        _logger.Debug("Ключ, по которому необходимо удалить значение: {key}.", key);

        if (key is null || (key is string stringKey && string.IsNullOrWhiteSpace(stringKey)))
        {
            _logger.Error("Невозможно удалить кэш - без ключа кэша.");

            throw new ArgumentNullException(nameof(key), "Невозможно удалить кэш для ключа - без ключа.");
        }

        var keyString = key.ToString();
        var redisKey = new RedisKey(keyString);
        await _cache.KeyDeleteAsync(redisKey);

        _logger.Information("Удаление значения из кэша через сервис - успешно.");
    }

    /// <inheritdoc />
    public async Task<TType?> WrapCacheOperationsAsync<TType>(object? key, Func<Task<TType?>> asyncDelegate,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _logger.Information("Обертывание операций кэша через сервис.");
        _logger.Debug("Ключ, по которому необходимо получить и сохранить значение: {key}.", key);

        if (key is null || (key is string stringKey && string.IsNullOrWhiteSpace(stringKey)))
        {
            _logger.Error("Для операций с кэшем был передан пустой ключ кэша.");

            throw new ArgumentNullException(nameof(key), "Для операций с кэшем был передан пустой ключ кэша.");
        }

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
        _logger.Information("Полученное и сохраненное в кэш значение поставщика: {resultForCache}.", resultForCache);

        return resultForCache;
    }
}