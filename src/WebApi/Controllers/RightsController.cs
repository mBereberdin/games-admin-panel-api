namespace WebApi.Controllers;

using Domain.DTOs.Rights;
using Domain.Models.Games;
using Domain.Models.Rights;

using Infrastructure.Comparers;
using Infrastructure.Services.Interfaces;

using Mapster;

using Microsoft.AspNetCore.Mvc;

using ILogger = Serilog.ILogger;

/// <summary>
/// Контроллер прав.
/// </summary>
[Route("[controller]")]
[ApiController]
public class RightsController : ControllerBase, IModelsValidator
{
    /// <summary>
    /// Логгер.
    /// </summary>
    private readonly ILogger _logger;

    /// <inheritdoc cref="IRightsService"/>
    private readonly IRightsService _rightsService;

    /// <inheritdoc cref="IGamesService"/>
    private readonly IGamesService _gamesService;

    /// <inheritdoc cref="ExternalGamesComparer"/>
    private readonly ExternalGamesComparer _externalGamesComparer;

    /// <inheritdoc cref="RightsController"/>
    /// <param name="logger">Логгер.</param>
    /// <param name="rightsService">Сервис для работы с правами.</param>
    /// <param name="gamesService">Сервис для работы с играми.</param>
    /// <param name="externalGamesComparer">Компаратор для сравнения внешних игр.</param>
    public RightsController(ILogger logger, IRightsService rightsService, IGamesService gamesService,
        ExternalGamesComparer externalGamesComparer)
    {
        _logger = logger;
        _logger.Debug($"Инициализация: {nameof(RightsController)}.");

        _rightsService = rightsService;
        _gamesService = gamesService;
        _externalGamesComparer = externalGamesComparer;

        _logger.Debug($"{nameof(RightsController)}: инициализирован.");
    }

    /// <summary>
    /// Зарегистрировать права.
    /// </summary>
    /// <param name="registerRightsDto">ДТО регистрации прав.</param>
    /// <param name="cancellationToken">Токен отмены выполнения операции.</param>>
    /// <response code="200">Когда права были успешно зарегистрированы.</response>
    /// <response code="400">Когда были переданы не корректные данные для регистрации прав.</response>
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RegisterAsync([FromBody] RegisterRightsDto registerRightsDto,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _logger.Information("Получен запрос на регистрацию прав.");
        _logger.Debug($"Кол-во прав, которые необходимо зарегистрировать: {registerRightsDto.Rights.Count()}");
        _logger.Debug($"Последнее право из списка прав на регистрацию: {registerRightsDto.Rights.LastOrDefault()}");
        _logger.Debug($"Игра, для которой необходимо зарегистрировать права: {registerRightsDto.Game.Name}");

        var externalGame = registerRightsDto.Game.Adapt<Game>();
        var gameModel = await _gamesService.GetAsync(registerRightsDto.Game.Name, cancellationToken);
        if (gameModel is null)
        {
            _logger.Information(
                "Не удалось найти игру для регистрации прав. Игра будет создана перед регистрацией прав.");
            _logger.Debug("ДТО игры для создания: {gameDto}.", registerRightsDto.Game);

            gameModel = await _gamesService.AddAsync(externalGame, cancellationToken);

            _logger.Information("Игра для регистрации прав успешно создана.");
            _logger.Debug("Модель созданной игры: {gameModel}.", gameModel);
        }
        else if (!_externalGamesComparer.Equals(gameModel, externalGame))
        {
            gameModel = await _gamesService.UpdateAsync(gameModel.Id, externalGame, cancellationToken);
        }

        var rightsModels = registerRightsDto.Rights.Adapt<IList<Right>>();
        foreach (var rightsModel in rightsModels)
        {
            rightsModel.GameId = gameModel.Id;
        }

        await _rightsService.RegisterAsync(rightsModels, gameModel.Name, cancellationToken);

        return Ok();
    }
}