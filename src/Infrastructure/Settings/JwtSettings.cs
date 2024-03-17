namespace Infrastructure.Settings;

using System.Text;

using Microsoft.IdentityModel.Tokens;

/// <summary>
/// Настройки jwt.
/// </summary>
public record JwtSettings
{
    /// <summary>
    /// Выдающий.
    /// </summary>
    public string Issuer { get; init; } = null!;

    /// <summary>
    /// Аудитория.
    /// </summary>
    public string Audience { get; init; } = null!;

    /// <summary>
    /// Через сколько минут истекает токен.
    /// </summary>
    public int ExpiryInMinutes { get; init; }

    /// <summary>
    /// Слово ключа безопасности.
    /// </summary>
    public string SecurityKeyWord { get; init; } = null!;

    /// <summary>
    /// Ключ безопасности.
    /// </summary>
    public SecurityKey SecurityKey => new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecurityKeyWord));
}