namespace WebApi.Controllers;

using Domain.DTOs.Users;
using Domain.Models.Users;

using Infrastructure.Exceptions;
using Infrastructure.Services.Interfaces;

using Mapster;

using Microsoft.AspNetCore.Mvc;

using ILogger = Serilog.ILogger;

/// <summary>
/// Контроллер пользователей.
/// </summary>
[Route("[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    /// <summary>
    /// Логгер.
    /// </summary>
    private readonly ILogger _logger;

    /// <inheritdoc cref="IUsersService"/>
    private readonly IUsersService _usersService;

    /// <inheritdoc />
    /// <param name="logger">Логгер.</param>
    /// <param name="usersService">Сервис тестовых сущностей.</param>
    public UsersController(ILogger logger, IUsersService usersService)
    {
        _logger = logger;
        _logger.Debug($"Инициализация: {nameof(UsersController)}.");

        _usersService = usersService;

        _logger.Debug($"{nameof(UsersController)}: инициализирован.");
    }

    /// <summary>
    /// Получить пользователей.
    /// </summary>
    /// <param name="cancellationToken">Токен отмены выполнения операции.</param>>
    /// <returns>Перечисление пользователей в формате JSON.</returns>
    /// <response code="200">Когда пользователи успешно получены.</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IList<UserDto>))]
    public async Task<IActionResult> GetUsersAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _logger.Information("Получен запрос на получение пользователей.");

        var foundUsers = await _usersService.GetUsersAsync(cancellationToken);
        var foundUsersDtos = foundUsers.Adapt<IList<UserDto>>();

        _logger.Information(
            "Запрос на получение пользователя - успешно обработан.");
        _logger.Debug("Кол-во полученных пользователей: {foundUsersCount}.", foundUsersDtos.Count);
        _logger.Debug("ДТО последнего полученного пользователя: {userDto}.", foundUsersDtos.LastOrDefault());

        return Ok(foundUsersDtos);
    }

    /// <summary>
    /// Получить пользователя.
    /// </summary>
    /// <param name="id">Идентификатор пользователя.</param>
    /// <param name="cancellationToken">Токен отмены выполнения операции.</param>>
    /// <returns>Пользователь в формате JSON.</returns>
    /// <response code="200">Когда пользователь успешно получен.</response>
    /// <response code="404">Когда пользователь не найден.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserAsync([FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _logger.Information("Получен запрос на получение пользователя.");
        _logger.Debug("Идентификатор пользователя: {id}", id);

        var foundUser = await _usersService.GetUserAsync(id, cancellationToken);
        if (foundUser is null)
        {
            _logger.Information(
                "Запрос на получение пользователя - произошла ошибка.");
            _logger.Debug("Идентификатор пользователя для получения: {id}", id);

            return NotFound("Пользователь с переданым идентификатором - не найден.");
        }

        var userDto = foundUser.Adapt<UserDto>();

        _logger.Information(
            "Запрос на получение пользователя - успешно обработан.");
        _logger.Debug("Дто полученного пользователя: {userDto}.", userDto);

        return Ok(userDto);
    }

    /// <summary>
    /// Добавить пользователя.
    /// </summary>
    /// <param name="createUserDto">ДТО создания пользователя.</param>
    /// <param name="cancellationToken">Токен отмены выполнения операции.</param>>
    /// <response code="204">Когда пользователь успешно добавлен.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(UserDto))]
    public async Task<IActionResult> AddUserAsync([FromBody] CreateUserDto createUserDto,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _logger.Information("Поступил запрос на добавление пользователя.");
        _logger.Debug("ДТО пользователя: {updateUserDto}", createUserDto);

        var userModel = createUserDto.Adapt<User>();
        var user = await _usersService.CreateUserAsync(userModel, cancellationToken);

        _logger.Information("Запрос на добавление пользователя - успешно обработан.");
        _logger.Debug("Модель добавленного пользователя: {user}.", user);

        return CreatedAtAction("GetUser", new { user.Id }, user);
    }

    /// <summary>
    /// Обновить пользователя.
    /// </summary>
    /// <param name="id">Идентификатор пользователя.</param>
    /// <param name="updateUserDto">ДТО обновления пользователя.</param>
    /// <param name="cancellationToken">Токен отмены выполнения операции.</param>>
    /// <response code="201">Когда пользователь успешно добавлен.</response>
    /// <response code="204">Когда пользователь успешно обновлен.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(UserDto))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdateUserAsync([FromRoute] Guid id, [FromBody] UpdateUserDto updateUserDto,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _logger.Information("Поступил запрос на обновление пользователя.");
        _logger.Debug("ДТО обновления пользователя: {updateUserDto}", updateUserDto);

        if (!id.Equals(updateUserDto.Id))
        {
            _logger.Error(
                "Невозможно обновить пользователя, т.к. запрос обновления направлен к одному пользователю, а тело запроса - содержит другого пользователя.");

            return BadRequest(
                "Невозможно обновить пользователя, т.к. запрос обновления направлен к одному пользователю, а тело запроса - содержит другого пользователя.");
        }

        var foundUser = await _usersService.GetUserAsync(id, cancellationToken);
        if (foundUser is null)
        {
            _logger.Warning("Не удалось найти пользователя для обновления. Будет создан новый пользователь.");

            var user = updateUserDto.Adapt<User>() ??
                       throw new MappingException(
                           "Не удалось преобразовать ДТО обновления пользователя в ДТО создания пользователя.");

            var createdUser = await _usersService.CreateUserAsync(user, cancellationToken);
            var createdUserDto = createdUser.Adapt<UserDto>();

            _logger.Information("Запрос на обновление пользователя - успешно обработан.");
            _logger.Debug("Модель созданного пользователя: {user}.", createdUser);

            return CreatedAtAction("GetUser", new { createdUserDto.Id }, createdUserDto);
        }

        updateUserDto.Adapt(foundUser);
        await _usersService.UpdateUserAsync(id, foundUser, cancellationToken);

        var savedUpdatedUser = await _usersService.GetUserAsync(id, cancellationToken);
        var savedUpdatedUserDto = savedUpdatedUser.Adapt<UserDto>();

        _logger.Information("Запрос на обновление пользователя - успешно обработан.");
        _logger.Debug("Модель обновленного пользователя: {user}.", foundUser);

        return Ok(savedUpdatedUserDto);
    }

    /// <summary>
    /// Удалить пользователя.
    /// </summary>
    /// <param name="id">Идентификатор пользователя, которого необходимо удалить.</param>
    /// <param name="cancellationToken">Токен отмены выполнения операции.</param>>
    /// <response code="204">Когда пользователь успешно удален.</response>
    /// <response code="400">Для удаления пользователя был передан некорректный идентификатор.</response>
    /// <response code="404">Когда пользователь не найден.</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteUserAsync([FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _logger.Information("Поступил запрос на удаление пользователя.");
        _logger.Debug("Идентификатор пользователя: {id}", id);

        if (id.Equals(Guid.Empty))
        {
            _logger.Error("Для удаления пользователя был передан некорректный идентификатор.");

            return BadRequest("Для удаления пользователя был передан некорректный идентификатор.");
        }

        var isUserDeleted = await _usersService.TryDeleteUserAsync(id, cancellationToken);
        if (!isUserDeleted)
        {
            _logger.Error("Не удалось удалить пользователя т.к. пользователь с таким идентификатором - не найден.");
            _logger.Debug("Переданный идентификатор: {id}", id);

            return NotFound();
        }

        _logger.Information("Запрос на удаление пользователя - успешно обработан.");

        return NoContent();
    }
}