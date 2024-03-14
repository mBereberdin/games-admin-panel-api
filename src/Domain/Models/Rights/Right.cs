namespace Domain.Models.Rights;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Domain.Models.Common;
using Domain.Models.Games;

using Microsoft.EntityFrameworkCore;

/// <summary>
/// Право.
/// </summary>
[Table("Rights")]
public class Right : EntityBase
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
    [Required]
    [StringLength(32)]
    [Comment("Описание.")]
    public required string Description { get; set; }

    /// <summary>
    /// Идентификатор записи игры.
    /// </summary>
    [Required]
    [ForeignKey("Game")]
    [Comment("Внешний ключ записи игры.")]
    public required Guid GameId { get; set; }

    /// <summary>
    /// Игра.
    /// </summary>
    public Game? Game { get; set; }
}