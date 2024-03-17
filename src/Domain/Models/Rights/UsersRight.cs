namespace Domain.Models.Rights;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Domain.Models.Common;
using Domain.Models.Users;

using Microsoft.EntityFrameworkCore;

/// <summary>
/// Право пользователя.
/// </summary>
[Table("UsersRights")]
public class UsersRight : EntityBase
{
    /// <summary>
    /// Идентификатор записи пользователя.
    /// </summary>
    [Required]
    [ForeignKey("User")]
    [Comment("Внешний ключ записи пользователя.")]
    public required Guid UserId { get; set; }

    /// <summary>
    /// Пользователь.
    /// </summary>
    public User? User { get; set; }

    /// <summary>
    /// Идентификатор записи права.
    /// </summary>
    [Required]
    [ForeignKey("Right")]
    [Comment("Внешний ключ записи права.")]
    public required Guid RightId { get; set; }

    /// <summary>
    /// Право.
    /// </summary>
    public Right? Right { get; set; }
}