namespace Tests.Fixtures;

using Database;

using Infrastructure;

using Mapster;

using Moq;

using Serilog;

/// <summary>
/// Фикстура сервиса.
/// </summary>
/// <remarks>Получает контекст бд для сервиса, мокает логгер и токен отмены, подгружает конфиги маппинга.</remarks>
public class ServiceFixture : DatabaseFixture, IDisposable
{
    /// <inheritdoc cref="AdminDbContext"/>
    private readonly AdminDbContext _context;

    /// <summary>
    /// Фиктивный конекст.
    /// </summary>
    private readonly AdminDbContext _mockedContextThrowException;

    /// <inheritdoc cref="ServiceFixture"/>
    protected ServiceFixture()
    {
        _context = CreateDbContext();
        _mockedContextThrowException = MockedContextThrowExceptionOnSave();
        MockCancellationToken = new CancellationTokenSource(int.MaxValue).Token;
        TypeAdapterConfig.GlobalSettings.Scan(typeof(MapsterConfigsRegister).Assembly);
    }

    /// <summary>
    /// Фиктивный токен отмены выполнения операции.
    /// </summary>
    protected CancellationToken MockCancellationToken { get; }

    /// <summary>
    /// Фиктивный логгер.
    /// </summary>
    public static ILogger MockLogger => new Mock<ILogger>().Object;

    /// <summary>
    /// Создать сервис для теста.
    /// </summary>
    /// <param name="creation">Создание экземляра сервиса.</param>
    /// <param name="needThrowDbUpdateException">Нужно ли проверить DbUpdateException.</param>
    /// <typeparam name="TService">Тип сервиса.</typeparam>
    /// <remarks>Создается экземляр сервиса, необходимго для теста. Экземпляру предоставляется контекст бд и мокнутый логгер.</remarks>
    /// <returns>Экземпляр сервиса для теста.</returns>
    protected TService CreateServiceForTest<TService>(Func<AdminDbContext, ILogger, TService> creation,
        bool needThrowDbUpdateException = false)
    {
        var providedContext = needThrowDbUpdateException ? _mockedContextThrowException : _context;

        return creation(providedContext, MockLogger);
    }

    /// <summary>
    /// Освободить ресурсы.
    /// </summary>
    public new void Dispose()
    {
        base.Dispose();
    }
}