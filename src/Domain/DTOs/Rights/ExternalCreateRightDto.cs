namespace Domain.DTOs.Rights;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// ДТО создания права из внешней системы (игры).
/// </summary>
public record ExternalCreateRightDto
{
    /// <inheritdoc cref="ExternalCreateRightDto"/>
    /// <param name="name">Наименование</param>
    /// <param name="description">Описание</param>
    public ExternalCreateRightDto(string name, string description)
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
    [Required]
    [StringLength(32)]
    public string Description { get; init; }
}