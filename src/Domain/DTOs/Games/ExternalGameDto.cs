namespace Domain.DTOs.Games;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// ДТО игры для внешних систем (игр).
/// </summary>
public record ExternalGameDto
{
    /// <inheritdoc cref="ExternalGameDto"/>
    /// <param name="name">Наименование.</param>
    /// <param name="description">Описание.</param>
    public ExternalGameDto(string name, string? description = null)
    {
        Name = name;
        Description = description;
    }

    /// <summary>
    /// Наименование.
    /// </summary>
    [Required]
    [StringLength(32)]
    public string Name { get; init; }

    /// <summary>
    /// Описание.
    /// </summary>
    [StringLength(32)]
    public string? Description { get; init; }
}