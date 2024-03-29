namespace WebApi.Controllers;

using Domain.DTOs.Passwords;
using Domain.Models.Passwords;

using Infrastructure.Services.Interfaces;

using Mapster;

using Microsoft.AspNetCore.Mvc;

using ILogger = Serilog.ILogger;

/// <summary>
/// Контроллер паролей.
/// </summary>
[Route("[controller]")]
[ApiController]
public class PasswordsController : ControllerBase
{
    /// <summary>
    /// Логгер.
    /// </summary>
    private readonly ILogger _logger;

    /// <inheritdoc cref="IPasswordsService"/>
    private readonly IPasswordsService _passwordsService;

    /// <inheritdoc cref="ICacheService"/>
    private readonly ICacheService _cacheService;

    /// <inheritdoc />
    /// <param name="logger">Логгер.</param>
    /// <param name="passwordsService">Сервис для работы с паролями.</param>
    /// <param name="cacheService">Сервис для работы с кэшем.</param>
    public PasswordsController(ILogger logger, IPasswordsService passwordsService, ICacheService cacheService)
    {
        _logger = logger;
        _logger.Debug($"Инициализация: {nameof(PasswordsController)}.");

        _passwordsService = passwordsService;
        _cacheService = cacheService;

        _logger.Debug($"{nameof(PasswordsController)}: инициализирован.");
    }

    /// <summary>
    /// Получить пароль.
    /// </summary>
    /// <param name="id">Идентификатор пароля.</param>
    /// <param name="cancellationToken">Токен отмены выполнения операции.</param>>
    /// <returns>Пароль в формате JSON.</returns>
    /// <response code="200">Когда пароль успешно получен.</response>
    /// <response code="404">Когда пароль не найден.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PasswordDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPasswordAsync([FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _logger.Information("Получен запрос на получение пароля.");
        _logger.Debug("Идентификатор пароля: {id}", id);

        var foundPassword = await _cacheService.WrapCacheOperationsAsync(id,
            async () => await _passwordsService.GetPasswordAsync(id, cancellationToken), cancellationToken);
        if (foundPassword is null)
        {
            _logger.Information(
                "Запрос на получение пароля - произошла ошибка.");
            _logger.Debug("Идентификатор пароля для получения: {id}", id);

            return NotFound("Пароль с переданым идентификатором - не найден.");
        }

        var passwordDto = foundPassword.Adapt<PasswordDto>();

        _logger.Information(
            "Запрос на получение пароля - успешно обработан.");
        _logger.Debug("Дто полученного пароля: {passwordDto}.", passwordDto);

        return Ok(passwordDto);
    }

    /// <summary>
    /// Добавить пароль.
    /// </summary>
    /// <param name="createPasswordDto">ДТО создания пароля.</param>
    /// <param name="cancellationToken">Токен отмены выполнения операции.</param>>
    /// <response code="201">Когда пароль успешно добавлен.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(PasswordDto))]
    public async Task<IActionResult> AddPasswordAsync([FromBody] CreatePasswordDto createPasswordDto,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _logger.Information("Поступил запрос на добавление пароля.");
        _logger.Debug("ДТО пароля: {createPasswordDto}", createPasswordDto);

        var passwordModel = createPasswordDto.Adapt<Password>();
        var createdPassword = await _passwordsService.CreatePasswordAsync(passwordModel, cancellationToken);
        await _cacheService.SetAsync(createdPassword.Id, createdPassword, cancellationToken);
        var createdPasswordDto = createdPassword.Adapt<PasswordDto>();

        _logger.Information("Запрос на добавление пароля - успешно обработан.");
        _logger.Debug("ДТО добавленного пароля: {createdPasswordDto}.", createdPasswordDto);

        return CreatedAtAction("GetPassword", new { createdPasswordDto.Id }, createdPasswordDto);
    }
}