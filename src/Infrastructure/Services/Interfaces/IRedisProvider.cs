namespace Infrastructure.Services.Interfaces;

using StackExchange.Redis;

/// <summary>
/// Поставщик redis.
/// </summary>
public interface IRedisProvider
{
    /// <summary>
    /// Получить базу данных.
    /// </summary>
    /// <returns>Redis база данных.</returns>
    public IDatabase GetDatabase();
}