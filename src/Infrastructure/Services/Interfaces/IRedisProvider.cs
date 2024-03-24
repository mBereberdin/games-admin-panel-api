namespace Infrastructure.Services.Interfaces;

using StackExchange.Redis;

/// <summary>
/// Поставщик redis.
/// </summary>
public interface IRedisProvider
{
    /// <summary>
    /// Получить базу данны.
    /// </summary>
    /// <returns>Redis база данных.</returns>
    public IDatabase GetDatabase();
}