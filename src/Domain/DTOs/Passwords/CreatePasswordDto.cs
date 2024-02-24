namespace Domain.DTOs.Passwords;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// ДТО создания пароля.
/// </summary>
public record CreatePasswordDto
{
    /// <inheritdoc cref="CreatePasswordDto"/>
    /// <param name="encryptedValue">Зашифрованное значение пароля.</param>
    /// <param name="userId">Идентификатор записи пользователя.</param>
    public CreatePasswordDto(string encryptedValue, Guid userId)
    {
        EncryptedValue = encryptedValue;
        UserId = userId;
    }

    /// <summary>
    /// Зашифрованное значение пароля.
    /// </summary>
    [Required]
    [StringLength(32)]
    public string EncryptedValue { get; init; }

    /// <summary>
    /// Идентификатор записи пользователя.
    /// </summary>
    [Required]
    public Guid UserId { get; init; }
}