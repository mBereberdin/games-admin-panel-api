namespace Infrastructure.Settings;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// Настройки кэширования.
/// </summary>
public record CacheSettings
{
    /// <summary>
    /// Адрес сервера redis.
    /// </summary>
    [Required]
    public string RedisUrl { get; init; } = null!;

    /// <summary>
    /// Пароль для подключения к redis.
    /// </summary>
    [Required]
    public string RedisPassword { get; init; } = null!;

    /// <summary>
    /// Время жизни кэша в формате строки.
    /// </summary>
    [Required]
    public string CacheLifetimeString { get; init; } = null!;
}