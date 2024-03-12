namespace Infrastructure.Services.Implementations;

using Database;

using Domain.Models.Rights;

using Infrastructure.Comparers;
using Infrastructure.Exceptions.CRUD;
using Infrastructure.Extensions;
using Infrastructure.Services.Interfaces;

using Mapster;

using Microsoft.EntityFrameworkCore;

using Serilog;

/// <inheritdoc cref="IRightsService"/>
public class RightsService : IRightsService, IModelsValidator
{
    /// <inheritdoc cref="IGamesService"/>
    private readonly IGamesService _gamesService;

    /// <summary>
    /// Логгер.
    /// </summary>
    private readonly ILogger _logger;

    /// <summary>
    /// Контекст базы данных панели администрирования.
    /// </summary>
    private readonly AdminDbContext _context;

    /// <inheritdoc cref="RightsSortComparer"/>
    private readonly RightsSortComparer _rightsSortComparer;

    /// <inheritdoc cref="RightsService"/>
    /// <param name="gamesService">Сервис для работы с играми.</param>
    /// <param name="context">Контекст базы данных панели администрирования.</param>
    /// <param name="rightsSortComparer">Компаратор для сортировки прав.</param>
    /// <param name="logger">Логгер.</param>
    public RightsService(IGamesService gamesService, AdminDbContext context, RightsSortComparer rightsSortComparer,
        ILogger logger)
    {
        _logger = logger;
        _logger.Debug($"Инициализация {nameof(RightsService)}.");

        _gamesService = gamesService;
        _context = context;
        _rightsSortComparer = rightsSortComparer;

        _logger.Debug($"{nameof(RightsService)} - инициализирован.");
    }

    /// <inheritdoc />
    public async Task<IList<Right>> GetAllAsync(string gameName, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _logger.Information("Получение всех прав через сервис.");
        _logger.Debug("Наименование игры для получения всех прав: {gameName}.", gameName);

        if (string.IsNullOrWhiteSpace(gameName))
        {
            _logger.Error(
                "Невозможно получить все права т.к. для получения всех прав для игры было передано пустое наименование игры.");

            throw new ArgumentNullException(nameof(gameName),
                "Невозможно получить все права т.к. для получения всех прав для игры было передано пустое наименование игры.");
        }

        var game = await _gamesService.GetAsync(gameName, cancellationToken);
        if (game is null)
        {
            _logger.Error("Не удалось найти игру для получения всех прав.");
            _logger.Debug("Наименование игры, для которого не удалось найти игру для получения всех прав: {gameName}.",
                gameName);

            throw new NotFoundException("Не удалось найти игру для получения всех прав.");
        }

        var foundGamesRights = _context.Rights.Where(right => right.GameId == game.Id).ToList();

        _logger.Information("Получение всех прав через сервис - успешно.");
        _logger.Debug("Кол-во всех полученных прав: {count}.", foundGamesRights.Count);
        _logger.Debug("Последнее полученное право: {count}.", foundGamesRights.LastOrDefault());

        return foundGamesRights;
    }

    /// <inheritdoc />
    public async Task<IList<Right>?> GetAsync(CancellationToken cancellationToken, params string[] rightsNames)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _logger.Information("Получение права через сервис.");
        _logger.Debug("Наименование права: {rightsNames}.", rightsNames);

        if (rightsNames is null || rightsNames.Length is 0)
        {
            _logger.Error(
                "Невозможно получить право т.к. было передано пустое наименование права.");

            throw new ArgumentNullException(nameof(rightsNames),
                "Невозможно получить право т.к. было передано пустое наименование права.");
        }

        var foundRight =
            await Task.Run(() => _context.Rights.Where(right => rightsNames.Contains(right.Name)).ToList(),
                cancellationToken);
        if (!foundRight.Any())
        {
            _logger.Error("Не удалось найти право.");
            _logger.Debug("Наименование права, для которого не удалось найти модель: {rightsNames}.", rightsNames);

            return null;
        }

        _logger.Information("Получение права через сервис - успешно.");
        _logger.Debug("Полученное право: {foundRight}.", foundRight);

        return foundRight;
    }

    /// <inheritdoc />
    public async Task RegisterAsync(IList<Right> rightsToRegister, string gameName,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _logger.Information("Регистрация прав через сервис.");
        _logger.Debug("Кол-во прав для регистрации: {count}.", rightsToRegister.Count);
        _logger.Debug("Последнее право для регистрации: {rightsToRegisterLast}.", rightsToRegister.Last());

        if (!this.AreValid(rightsToRegister, out var validationResults))
        {
            _logger.Error("Невозможно зарегистрировать права т.к. некоторые из прав - заполнены некорректно.");
            _logger.Debug("Ошибки заполнения прав: {error}.",
                string.Join(',', validationResults.Select(result => result.ErrorMessage)));

            throw new ArgumentException(
                "Невозможно зарегистрировать права т.к. некоторые из прав - заполнены некорректно.");
        }

        var sortedRights = await SortGamesRightsAsync(gameName, rightsToRegister, cancellationToken);
        if (sortedRights.RightsToDelete != null && sortedRights.RightsToDelete.Any())
        {
            _logger.Information("Удаление прав после сортировки.");

            await TryDeleteRangeAsync(sortedRights.RightsToDelete, cancellationToken);

            _logger.Information("Удаление прав после сортировки - успешно.");
        }

        if (sortedRights.RightsToCreate != null && sortedRights.RightsToCreate.Any())
        {
            _logger.Information("Добавление прав после сортировки.");

            await AddRangeAsync(sortedRights.RightsToCreate, cancellationToken);

            _logger.Information("Добавление прав после сортировки - успешно.");
        }

        if (sortedRights.RightsToUpdate != null && sortedRights.RightsToUpdate.Any())
        {
            _logger.Information("Обновление прав после сортировки.");

            await UpdateRangeAsync(sortedRights.RightsToUpdate, cancellationToken);

            _logger.Information("Обновление прав после сортировки - успешно.");
        }

        _logger.Information("Регистрация прав через сервис - успешно.");
    }

    /// <inheritdoc />
    public async Task<bool> TryDeleteRangeAsync(IList<Right> rightsToDelete, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _logger.Information("Удаление списка прав через сервис.");
        _logger.Debug("Кол-во прав к удалению: {rightsToDeleteCount}.", rightsToDelete.Count);

        var rightsNames = rightsToDelete.Select(right => right.Name).ToArray();
        var rights = await GetAsync(cancellationToken, rightsNames);
        if (rights is null)
        {
            _logger.Warning("Не удалось найти права для удаления из бд.");

            return false;
        }

        _context.Rights.RemoveRange(rights);

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException dbUpdateException)
        {
            _logger.Error("При удалении списка прав в бд произошла ошибка: {dbUpdateException}.", dbUpdateException);

            throw new DeleteException("При удалении списка прав в бд произошла ошибка.", dbUpdateException);
        }

        if (rightsToDelete.Count != rights.Count)
        {
            _logger.Warning("Не удалось удалить весь список прав т.к. некоторые права не были найдены в бд.");
        }

        return true;
    }

    /// <inheritdoc />
    public async Task AddRangeAsync(IList<Right> rightsToAdd, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _logger.Information("Добавление списка прав через сервис.");
        _logger.Debug("Кол-во прав к добавлению: {rightsToAddCount}.", rightsToAdd.Count);

        if (!this.AreValid(rightsToAdd, out var validationResults))
        {
            _logger.Error("Невозможно добавить список прав т.к. некоторые из прав - заполнены некорректно.");
            _logger.Debug("Ошибки заполнения прав: {error}.",
                string.Join(',', validationResults.Select(result => result.ErrorMessage)));

            throw new ArgumentException(
                "Невозможно добавить список прав т.к. некоторые из прав - заполнены некорректно.");
        }

        await _context.Rights.AddRangeAsync(rightsToAdd, cancellationToken);

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException dbUpdateException)
        {
            _logger.Error("При добавлении списка прав в бд произошла ошибка: {dbUpdateException}.", dbUpdateException);

            throw new CreateException("При добавлении списка прав в бд произошла ошибка.", dbUpdateException);
        }
    }

    /// <inheritdoc />
    public async Task UpdateRangeAsync(IList<Right> rightsToUpdate, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _logger.Information("Обновление списка прав через сервис.");
        _logger.Debug("Кол-во прав к обновлению: {rightsToUpdateCount}.", rightsToUpdate.Count);

        if (!this.AreValid(rightsToUpdate, out var validationResults))
        {
            _logger.Error("Невозможно обновить список прав т.к. некоторые из прав - заполнены некорректно.");
            _logger.Debug("Ошибки заполнения прав: {error}.",
                string.Join(',', validationResults.Select(result => result.ErrorMessage)));

            throw new ArgumentException(
                "Невозможно обновить список прав т.к. некоторые из прав - заполнены некорректно.");
        }

        var rightNames = rightsToUpdate.Select(right => right.Name).ToArray();
        var rightModels = await GetAsync(cancellationToken, rightNames);
        if (rightModels is null)
        {
            _logger.Error("Не удалось найти права для обновления.");
            _logger.Debug("Наименования для поска прав: {name}.", string.Join(',', rightNames));

            throw new NotFoundException("Не удалось найти права для обновления.");
        }

        foreach (var rightModel in rightModels)
        {
            var rightDto = rightsToUpdate.Single(right =>
                right.Name.Equals(rightModel.Name, StringComparison.OrdinalIgnoreCase));

            rightDto.Adapt(rightModel);
        }

        _context.Rights.UpdateRange(rightModels);

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException dbUpdateException)
        {
            _logger.Error("При обновлении списка прав в бд произошла ошибка: {dbUpdateException}.", dbUpdateException);

            throw new UpdateException("При обновлении списка прав в бд произошла ошибка.", dbUpdateException);
        }
    }

    /// <inheritdoc />
    public async Task<SortedRights> SortGamesRightsAsync(string gameName, IList<Right> rightsToSort,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _logger.Information("Сортировка прав для игры через сервис.");

        if (string.IsNullOrWhiteSpace(gameName))
        {
            _logger.Error("Для сортировки прав игры было передано пустое наименование игры.");

            throw new ArgumentNullException(nameof(gameName),
                "Для сортировки прав игры было передано пустое наименование игры.");
        }

        if (!this.AreValid(rightsToSort, out var validationResults))
        {
            _logger.Error("Невозможно отсортировать права т.к. некоторые из прав - заполнены некорректно.");
            _logger.Debug("Ошибки заполнения прав: {error}.",
                string.Join(',', validationResults.Select(result => result.ErrorMessage)));

            throw new ArgumentException(
                "Невозможно отсортировать список прав т.к. некоторые из прав - заполнены некорректно.");
        }

        var gameRights = await GetAllAsync(gameName, cancellationToken);
        if (!gameRights.Any())
        {
            return new SortedRights { RightsToCreate = rightsToSort };
        }

        var sortedRights = new SortedRights
        {
            RightsToCreate = new List<Right>(),
            RightsToUpdate = new List<Right>(),
            RightsToDelete = new List<Right>()
        };

        foreach (var rightToSort in rightsToSort)
        {
            var rightWithSameName = gameRights.SingleOrDefault(right =>
                right.Name.Equals(rightToSort.Name, StringComparison.OrdinalIgnoreCase));

            if (rightWithSameName is null)
            {
                sortedRights.RightsToCreate.Add(rightToSort);
                continue;
            }

            var isRightNeedToUpDate = !_rightsSortComparer.Equals(rightToSort, rightWithSameName);
            if (isRightNeedToUpDate)
            {
                sortedRights.RightsToUpdate.Add(rightToSort);
            }
        }

        sortedRights.RightsToDelete = gameRights.Where(right =>
            !rightsToSort.Any(
                rightToSort => rightToSort.Name.Equals(right.Name, StringComparison.OrdinalIgnoreCase))).ToList();

        _logger.Information("Сортировка прав для игры через сервис - успешно.");
        _logger.Debug("Кол-во прав к созданию: {rightsToCreateCount}.", sortedRights.RightsToCreate.Count);
        _logger.Debug("Кол-во прав к обновлению: {rightsToUpdateCount}.", sortedRights.RightsToUpdate.Count);
        _logger.Debug("Кол-во прав к удалению: {rightsToDeleteCount}.", sortedRights.RightsToDelete.Count);

        return sortedRights;
    }
}