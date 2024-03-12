#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
namespace Database;

using Domain.Models.Games;
using Domain.Models.Passwords;
using Domain.Models.Rights;
using Domain.Models.Users;

using Microsoft.EntityFrameworkCore;

/// <summary>
/// Контекст базы данных панели администрирования.
/// </summary>
public class AdminDbContext : DbContext
{
    /// <inheritdoc cref="AdminDbContext"/>
    /// <param name="options">Настройки контекста.</param>
    public AdminDbContext(DbContextOptions<AdminDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Пароли.
    /// </summary>
    public virtual DbSet<Password> Passwords { get; set; }

    /// <summary>
    /// Пользователи.
    /// </summary>
    public virtual DbSet<User> Users { get; set; }

    /// <summary>
    /// Игры.
    /// </summary>
    public virtual DbSet<Game> Games { get; set; }

    /// <summary>
    /// Права.
    /// </summary>
    public virtual DbSet<Right> Rights { get; set; }

    /// <summary>
    /// Действия при создании модели.
    /// </summary>
    /// <param name="modelBuilder">Строитель моделей.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Game>()
            .HasIndex(game => game.Name)
            .IsUnique();

        modelBuilder.Entity<Right>()
            .HasIndex(game => game.Name)
            .IsUnique();
    }
}