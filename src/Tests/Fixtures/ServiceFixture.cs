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
    /// <summary>
    /// Контекст базы данных панели администрирования.
    /// </summary>
    private readonly AdminDbContext _context;

    /// <summary>
    /// Фиктивный конекст.
    /// </summary>
    private readonly AdminDbContext _mockedContextThrowException;

    /// <summary>
    /// Фиктивный логгер.
    /// </summary>
    private readonly ILogger _mockLogger;

    /// <inheritdoc cref="ServiceFixture"/>
    protected ServiceFixture()
    {
        _context = CreateDbContext();
        _mockedContextThrowException = MockedContextThrowExceptionOnSave();
        _mockLogger = new Mock<ILogger>().Object;
        MockCancellationToken = new CancellationTokenSource(int.MaxValue).Token;
        TypeAdapterConfig.GlobalSettings.Scan(typeof(MapsterConfigsRegister).Assembly);
    }

    /// <summary>
    /// Фиктивный токен отмены выполнения операции.
    /// </summary>
    protected CancellationToken MockCancellationToken { get; }

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
        if (needThrowDbUpdateException)
        {
            return creation(_mockedContextThrowException, _mockLogger);
        }

        return creation(_context, _mockLogger);
    }

    /// <summary>
    /// Освободить ресурсы.
    /// </summary>
    public new void Dispose()
    {
        base.Dispose();
    }
}