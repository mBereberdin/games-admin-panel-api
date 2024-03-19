namespace Domain.DTOs.Login;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// ДТО аутентификации.
/// </summary>
public record LoginDto
{
    /// <inheritdoc cref="LoginDto"/>
    /// <param name="nickname">Имя пользователя.</param>
    /// <param name="encryptedPassword">Зашифрованный пароль.</param>
    public LoginDto(string nickname, string encryptedPassword)
    {
        Nickname = nickname;
        EncryptedPassword = encryptedPassword;
    }

    /// <summary>
    /// Имя пользователя.
    /// </summary>
    [Required]
    [StringLength(20)]
    public string Nickname { get; init; }

    /// <summary>
    /// Зашифрованный пароль.
    /// </summary>
    [Required]
    [StringLength(32)]
    public string EncryptedPassword { get; init; }
}