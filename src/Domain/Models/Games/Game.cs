namespace Domain.Models.Games;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Domain.Models.Common;

using Microsoft.EntityFrameworkCore;

/// <summary>
/// Игра.
/// </summary>
[Table("Games")]
public class Game : EntityBase
{
    /// <summary>
    /// Наименование.
    /// </summary>
    [Required]
    [StringLength(32)]
    [Comment("Наименование.")]
    public required string Name { get; set; }

    /// <summary>
    /// Описание.
    /// </summary>
    [StringLength(32, MinimumLength = 1)]
    [Comment("Описание.")]
    public string? Description { get; set; }
}