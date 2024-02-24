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
    public CreateUserDto(string nickname, string email)
    {
        Nickname = nickname;
        Email = email;
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
}