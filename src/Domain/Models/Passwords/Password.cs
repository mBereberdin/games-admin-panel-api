namespace Domain.Models.Passwords;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Domain.Models.Common;
using Domain.Models.Users;

using Microsoft.EntityFrameworkCore;

/// <summary>
/// Пароль.
/// </summary>
[Table("Passwords")]
public class Password : EntityBase
{
    /// <summary>
    /// Зашифрованное значение.
    /// </summary>
    [Required]
    [StringLength(32)]
    [Comment("Зашифрованное значение.")]
    public required string EncryptedValue { get; set; }

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
}