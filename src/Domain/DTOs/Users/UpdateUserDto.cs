namespace Domain.DTOs.Users;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// ДТО обновления пользователя.
/// </summary>
public record UpdateUserDto
{
    /// <inheritdoc cref="UpdateUserDto"/>
    /// <param name="id">Идентификатор пользователя.</param>
    public UpdateUserDto(Guid id)
    {
        Id = id;
    }

    /// <summary>
    /// Идентификатор пользователя.
    /// </summary>
    [Required]
    public Guid Id { get; init; }

    /// <summary>
    /// Имя пользователя.
    /// </summary>
    [StringLength(20)]
    public string? Nickname { get; init; }

    /// <summary>
    /// Электронная почта.
    /// </summary>
    [StringLength(32)]
    public string? Email { get; init; }

    /// <summary>
    /// Аватар.
    /// </summary>
    public string? Avatar { get; init; }
}