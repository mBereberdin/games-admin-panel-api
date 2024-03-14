namespace Infrastructure.Services.Implementations;

using Database;

using Domain.Models.Games;

using Infrastructure.Exceptions.CRUD;
using Infrastructure.Extensions;
using Infrastructure.Services.Interfaces;

using Mapster;

using Microsoft.EntityFrameworkCore;

using Serilog;

/// <inheritdoc cref="IGamesService"/>
public class GamesService : IGamesService, IModelsValidator
{
    /// <summary>
    /// Логгер.
    /// </summary>
    private readonly ILogger _logger;

    /// <inheritdoc cref="AdminDbContext"/>
    private readonly AdminDbContext _context;

    /// <inheritdoc cref="GamesService"/>
    /// <param name="context">Контекст базы данных панели администрирования.</param>
    /// <param name="logger">Логгер.</param>
    public GamesService(AdminDbContext context, ILogger logger)
    {
        _logger = logger;
        _logger.Debug($"Инициализация {nameof(GamesService)}.");

        _context = context;

        _logger.Debug($"{nameof(GamesService)} - инициализирован.");
    }

    /// <inheritdoc />
    public async Task<Game?> GetAsync(string name, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _logger.Information("Получение игры через сервис.");
        _logger.Debug("Наименование игры для получения: {name}.", name);

        var foundGame = await _context.Games.Where(game => game.Name == name)
            .SingleOrDefaultAsync(cancellationToken: cancellationToken);
        if (foundGame is null)
        {
            _logger.Error("Не удалось получить игру через сервис.");
            _logger.Debug("Наименование игры, для которой не удалось получить игру: {name}.", name);

            return null;
        }

        _logger.Information("Получение игры через сервис - успешно.");
        _logger.Debug("Полученная игра: {foundGame}.", foundGame);

        return foundGame;
    }

    /// <inheritdoc />
    public async Task<Game?> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _logger.Information("Получение игры через сервис.");
        _logger.Debug("Идентификатор игры для получения: {id}.", id);

        var foundGame = await _context.Games.FindAsync(new object?[] { id }, cancellationToken);
        if (foundGame is null)
        {
            _logger.Error("Не удалось получить игру через сервис.");
            _logger.Debug("Идентификатор игры, для которого не удалось получить игру: {id}.", id);

            return null;
        }

        _logger.Information("Получение игры через сервис - успешно.");
        _logger.Debug("Полученная игра: {foundGame}.", foundGame);

        return foundGame;
    }

    /// <inheritdoc />
    public async Task<Game> AddAsync(Game gameToAdd, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _logger.Information("Создание игры через сервис.");
        _logger.Debug("Игра для создания: {gameToAdd}.", gameToAdd);

        if (!this.IsValid(gameToAdd, out var validationResults))
        {
            _logger.Error("Невозможно создать игру т.к. она заполнена некорректно.");
            _logger.Debug("Ошибки заполнения игры: {error}",
                string.Join(',', validationResults.Select(result => result.ErrorMessage)));

            throw new ArgumentException("Невозможно создать игру т.к. она заполнена некорректно.");
        }

        await _context.Games.AddAsync(gameToAdd, cancellationToken);

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException dbUpdateException)
        {
            _logger.Error("При добавлении игры в бд произошла ошибка: {dbUpdateException}.", dbUpdateException);

            throw new CreateException("При добавлении игры в бд произошла ошибка.", dbUpdateException);
        }

        _logger.Information("Создание игры через сервис - успешно.");
        _logger.Debug("Созданная игра: {gameToAdd}.", gameToAdd);

        return gameToAdd;
    }

    /// <inheritdoc />
    public async Task<Game> UpdateAsync(Guid id, Game updatedGame, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _logger.Information("Обновление игры через сервис.");
        _logger.Debug("Идентификатор игры для обновления: {id}.", id);
        _logger.Debug("Игра для обновления: {updatedGame}.", updatedGame);

        if (!this.IsValid(updatedGame, out var validationResults))
        {
            _logger.Error("Невозможно обновить игру т.к. она заполнена некорректно.");
            _logger.Debug("Ошибки заполнения игры: {error}",
                string.Join(',', validationResults.Select(result => result.ErrorMessage)));

            throw new ArgumentException("Невозможно обновить игру т.к. она заполнена некорректно.");
        }

        var existsGame = await GetAsync(id, cancellationToken);
        if (existsGame is null)
        {
            _logger.Error("Не удалось получить игру для обновления.");
            _logger.Debug("Идентификатор для получения игры: {id}.", id);

            throw new NotFoundException("Не удалось получить игру для обновления.");
        }

        updatedGame.Adapt(existsGame);

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException dbUpdateException)
        {
            _logger.Error("При обновлении игры в бд произошла ошибка: {dbUpdateException}.", dbUpdateException);

            throw new UpdateException("При обновлении игры в бд произошла ошибка.", dbUpdateException);
        }

        _logger.Information("Обновление игры через сервис - успешно.");
        _logger.Debug("Обновленная игра: {existsGame}.", existsGame);

        return existsGame;
    }
}