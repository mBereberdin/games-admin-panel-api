namespace WebApi.Controllers;

using Domain.DTOs.Login;

using Infrastructure.Services.Interfaces;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Serilog;

/// <summary>
/// Контроллер токенов.
/// </summary>
[Route("[controller]")]
[ApiController]
[AllowAnonymous]
public class TokensController : ControllerBase
{
    /// <inheritdoc cref="IPasswordsService"/>
    private readonly IPasswordsService _passwordsService;

    /// <inheritdoc cref="IJwtBuilder"/>
    private readonly IJwtBuilder _jwtBuilder;

    /// <summary>
    /// Логгер.
    /// </summary>
    private readonly ILogger _logger;

    /// <inheritdoc cref="TokensController"/>
    /// <param name="passwordsService">Сервис для работы с паролями.</param>
    /// <param name="jwtBuilder">Строитель jwt.</param>
    /// <param name="logger">Логгер.</param>
    public TokensController(IPasswordsService passwordsService, IJwtBuilder jwtBuilder,
        ILogger logger)
    {
        _logger = logger;
        _logger.Debug($"Инициализация: {nameof(TokensController)}");

        _passwordsService = passwordsService;
        _jwtBuilder = jwtBuilder;

        _logger.Debug($"{nameof(TokensController)} - инициализирован.");
    }

    /// <summary>
    /// Создать токен.
    /// </summary>
    /// <param name="loginDto">ДТО аутентификации.</param>
    /// <param name="cancellationToken">Токен отмены выполнения операции.</param>
    /// <returns>Токен.</returns>
    /// <response code="200">Когда токен успешно получен.</response>
    /// <response code="401">Когда пароль не соответствует указанному пользователю.</response>
    /// <response code="403">Когда у пользователя нет прав для использования систем.</response>
    /// <response code="404">Когда не удалось найти пользователя с переданным паролем.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateAsync([FromBody] LoginDto loginDto,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _logger.Information("Поступил запрос создания токена.");
        _logger.Debug("ДТО аутентификации запроса: {loginDto}.", loginDto);

        var password = await _passwordsService.GetAsync(loginDto.EncryptedPassword, cancellationToken);
        if (password is null)
        {
            _logger.Error("Не удалось найти пользователя с переданным паролем.");
            _logger.Debug(
                "Зашифрованное значение пароля, для которого не удалось найти пользователя: {encryptedPassword}.",
                loginDto.EncryptedPassword);

            return NotFound();
        }

        if (!password.User!.Nickname.Equals(loginDto.Nickname))
        {
            _logger.Error("Пароль не соответствует указанному пользователю.");
            _logger.Debug("Указанный пользователь: {nickname}.", loginDto.Nickname);

            return Unauthorized();
        }

        if (password.User.UserRights is null || !password.User.UserRights.Any())
        {
            _logger.Error("У пользователя нет прав для использования систем.");
            _logger.Debug("Пользователь: {user}.", password.User);

            return Forbid();
        }

        var userRights = password.User.UserRights.Select(usersRight => usersRight.Right!).ToList();

        var token = _jwtBuilder.AddUsername(password.User.Nickname)
                               .AddClaims(userRights)
                               .Build();

        _logger.Information("Запрос создания токена - успешно.");
        _logger.Debug("Токен: {token}.", token);

        return Ok(token);
    }
}