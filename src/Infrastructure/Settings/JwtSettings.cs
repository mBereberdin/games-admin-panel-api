namespace Infrastructure.Settings;

using System.ComponentModel.DataAnnotations;
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
    [Required]
    [MinLength(3)]
    public string Issuer { get; init; } = null!;

    /// <summary>
    /// Аудитория.
    /// </summary>
    [Required]
    [MinLength(3)]
    public string Audience { get; init; } = null!;

    /// <summary>
    /// Через сколько минут истекает токен.
    /// </summary>
    public int ExpiryInMinutes { get; init; } = 1;

    /// <summary>
    /// Слово ключа безопасности.
    /// </summary>
    [Required]
    [MinLength(3)]
    public string SecurityKeyWord { get; init; } = null!;

    /// <summary>
    /// Ключ безопасности.
    /// </summary>
    public SecurityKey SecurityKey => new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecurityKeyWord));
}