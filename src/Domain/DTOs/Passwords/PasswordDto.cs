namespace Domain.DTOs.Passwords;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// ДТО пароля.
/// </summary>
public class PasswordDto
{
    /// <inheritdoc cref="PasswordDto"/>
    /// <param name="encryptedValue">Зашифрованное значение пароля.</param>
    /// <param name="userId">Идентификатор записи пользователя.</param>
    /// <param name="id">Идентификатор пароля.</param>
    public PasswordDto(string encryptedValue, Guid userId, Guid id)
    {
        EncryptedValue = encryptedValue;
        UserId = userId;
        Id = id;
    }

    /// <summary>
    /// Идентификатор пароля.
    /// </summary>
    [Required]
    public Guid Id { get; init; }

    /// <summary>
    /// Зашифрованное значение пароля.
    /// </summary>
    [Required]
    [StringLength(32)]
    public string EncryptedValue { get; set; }

    /// <summary>
    /// Идентификатор записи пользователя.
    /// </summary>
    [Required]
    public Guid UserId { get; set; }
}