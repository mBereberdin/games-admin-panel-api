namespace Domain.Models.Common;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

/// <summary>
/// Базовая сущность.
/// </summary>
public class EntityBase
{
    /// <summary>
    /// Идентификатор.
    /// </summary>
    [Key]
    [Required]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column(Order = 0)]
    [Comment("Идентификатор сущности.")]
    public required Guid Id { get; set; }
}