namespace Tests.Fixtures;

using Database;

using Domain.Models.Passwords;
using Domain.Models.Users;

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

using Moq;
using Moq.EntityFrameworkCore;

using Tests.FakeData;

/// <summary>
/// Фикстура бд.
/// </summary>
public class DatabaseFixture : IDisposable
{
    /// <summary>
    /// Строка подключения.
    /// </summary>
    public const string CONNECTION_STRING = "DataSource=:memory:";

    /// <summary>
    /// Соединение с бд.
    /// </summary>
    private readonly SqliteConnection _connection;

    /// <inheritdoc cref="DatabaseFixture"/>
    protected DatabaseFixture()
    {
        _connection = new SqliteConnection(CONNECTION_STRING);
        _connection.Open();

        var dbContext = CreateDbContext();
        ReplaceDbWithFakeData(dbContext);
    }

    /// <summary>
    /// Создать контекст бд.
    /// </summary>
    /// <returns>Контекст базы данных панели администрирования.</returns>
    protected AdminDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AdminDbContext>().UseSqlite(_connection).Options;
        var context = new AdminDbContext(options);

        return context;
    }

    /// <summary>
    /// Фиктивный конекст.
    /// </summary>
    /// <remarks>Выкидывает DbUpdateException при вызове метода SaveChangesAsync.</remarks>
    /// <returns>Фиктивный конекст.</returns>
    /// <exception cref="DbUpdateException">Ошибка обновления бд.</exception>
    protected AdminDbContext MockedContextThrowExceptionOnSave()
    {
        var options = new DbContextOptions<AdminDbContext>();
        var mockedContext = new Mock<AdminDbContext>(options);
        mockedContext.SetupGet(context => context.Users).ReturnsDbSet(FakeUsers.GetManyCorrectExistsModels());
        mockedContext.SetupGet(context => context.Passwords).ReturnsDbSet(FakePasswords.GetManyCorrectExistsModels());
        // ReSharper disable once EntityFramework.UnsupportedServerSideFunctionCall
        mockedContext.Setup(context => context.Users.FindAsync(It.IsAny<object?[]>(), It.IsAny<CancellationToken>()))
        mockedContext.SetupGet(context => context.Games).ReturnsDbSet(FakeGames.GetManyCorrectExistsModels());
            .Returns(() => new ValueTask<User>(FakeUsers.GetManyCorrectExistsModels().First())!);
        // ReSharper disable once EntityFramework.UnsupportedServerSideFunctionCall
        mockedContext
            .Setup(context => context.Passwords.FindAsync(It.IsAny<object?[]>(), It.IsAny<CancellationToken>()))
            .Returns(() => new ValueTask<Password>(FakePasswords.GetManyCorrectExistsModels().First())!);

        mockedContext
            .Setup(context => context.Games.FindAsync(It.IsAny<object?[]>(), It.IsAny<CancellationToken>()))
            .Returns(() => new ValueTask<Game>(FakeGames.GetManyCorrectExistsModels().First())!);
        mockedContext.Setup(adminDbContext => adminDbContext.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(() => throw new DbUpdateException());

        return mockedContext.Object;
    }

    /// <summary>
    /// Заменить бд с фиктивными данными.
    /// </summary>
    /// <param name="dbContext">Контекст бд, который необходимо заменить и наполнить фиктивными данными.</param>
    public static void ReplaceDbWithFakeData(AdminDbContext dbContext)
    {
        dbContext.Database.EnsureDeleted();
        dbContext.Database.Migrate();

        dbContext.RemoveRange(dbContext.Users);
        dbContext.RemoveRange(dbContext.Passwords);
        dbContext.RemoveRange(dbContext.Games);
        dbContext.SaveChanges();

        dbContext.Users.AddRange(FakeUsers.GetManyCorrectExistsModels());
        dbContext.Passwords.AddRange(FakePasswords.GetManyCorrectExistsModels());
        dbContext.Games.AddRange(FakeGames.GetManyCorrectExistsModels());
        dbContext.SaveChanges();
    }

    /// <summary>
    /// Освободить ресурсы.
    /// </summary>
    public void Dispose()
    {
        _connection.Close();
    }
}