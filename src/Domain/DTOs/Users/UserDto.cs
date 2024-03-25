namespace Domain.DTOs.Users;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// ДТО пользователя.
/// </summary>
public record UserDto
{
    /// <inheritdoc cref="UserDto"/>
    /// <param name="id">Идентификатор пользователя.</param>
    /// <param name="nickname">Имя пользователя.</param>
    /// <param name="email">Электронная почта.</param>
    public UserDto(Guid id, string nickname, string email)
    {
        Id = id;
        Nickname = nickname;
        Email = email;
    }

    /// <summary>
    /// Идентификатор пользователя.
    /// </summary>
    [Required]
    public Guid Id { get; init; }

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
    /// Аватар.
    /// </summary>
    public string? Avatar { get; set; }
}