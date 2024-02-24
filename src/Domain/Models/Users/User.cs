namespace Domain.Models.Users;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Domain.Models.Common;

using Microsoft.EntityFrameworkCore;

/// <summary>
/// Пользователь.
/// </summary>
[Table("Users")]
[Index(nameof(Nickname), Name = "IX_Nickname")]
public class User : EntityBase
{
    /// <summary>
    /// Электронная почта.
    /// </summary>
    [Required]
    [StringLength(32)]
    [Comment("Электронная почта.")]
    public required string Email { get; set; }

    /// <summary>
    /// Имя пользователя.
    /// </summary>
    [Required]
    [StringLength(20)]
    [Comment("Имя пользователя.")]
    public required string Nickname { get; set; }
}