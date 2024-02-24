namespace Domain.DTOs.Users;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// ДТО создания пользователя.
/// </summary>
public record CreateUserDto
{
    /// <inheritdoc cref="CreateUserDto"/>
    /// <param name="nickname">Имя пользователя.</param>
    /// <param name="email">Электронная почта.</param>
    /// <param name="encryptedPasswordValue">Зашифрованное значение пароля.</param>
    public CreateUserDto(string nickname, string email, string encryptedPasswordValue)
    {
        Nickname = nickname;
        Email = email;
        EncryptedPasswordValue = encryptedPasswordValue;
    }

    /// <summary>
    /// Имя пользователя.
    /// </summary>
    [Required]
    [StringLength(20)]
    public string Nickname { get; init; }

    /// <summary>
    /// Электронная почта.
    /// </summary>
    [Required]
    [StringLength(32)]
    public string Email { get; init; }

    /// <summary>
    /// Зашифрованное значение пароля.
    /// </summary>
    [Required]
    [StringLength(32)]
    public string EncryptedPasswordValue { get; init; }
}