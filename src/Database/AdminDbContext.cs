#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
namespace Database;

using Domain.Models;

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
    /// Тестовые сущности.
    /// </summary>
    public DbSet<TestEntity> TestEntities { get; set; }
}