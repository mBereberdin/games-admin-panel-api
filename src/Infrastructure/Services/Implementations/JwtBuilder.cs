namespace Infrastructure.Services.Implementations;

using System.IdentityModel.Tokens.Jwt;

using Domain.Models.Rights;

using Infrastructure.Services.Interfaces;
using Infrastructure.Settings;

using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using Serilog;

/// <inheritdoc cref="IJwtBuilder"/>
public class JwtBuilder : IJwtBuilder
{
    /// <summary>
    /// Логгер.
    /// </summary>
    private readonly ILogger _logger;

    /// <inheritdoc cref="JwtSettings"/>
    private readonly JwtSettings _jwtSettings;

    /// <summary>
    /// Описатель токена.
    /// </summary>
    private readonly SecurityTokenDescriptor _descriptor;

    /// <inheritdoc cref="IJwtBuilder"/>
    /// <param name="jwtSettings">Настройки jwt.</param>
    /// <param name="logger">Логгер.</param>
    public JwtBuilder(IOptions<JwtSettings> jwtSettings, ILogger logger)
    {
        _logger = logger;
        logger.Debug($"Инициализация: {nameof(JwtBuilder)}");

        _jwtSettings = jwtSettings.Value;
        _descriptor = new SecurityTokenDescriptor
        {
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
            Claims = new Dictionary<string, object>(),
            SigningCredentials = new SigningCredentials(_jwtSettings.SecurityKey, SecurityAlgorithms.HmacSha256)
        };

        logger.Debug($"{nameof(JwtBuilder)} - инициализирован.");
    }

    /// <inheritdoc />
    public string Build()
    {
        _logger.Information("Построение токена через строитель.");

        _descriptor.Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryInMinutes);
        _descriptor.Claims.Add(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString());

        var jwtString = new JwtSecurityTokenHandler().CreateToken(_descriptor)
                                                     .UnsafeToString();

        _logger.Information("Построение токена через строитель - успешно.");
        _logger.Debug("Токен: {jwtString}.", jwtString);

        return jwtString;
    }

    /// <inheritdoc />
    public IJwtBuilder AddClaims(IList<Right> rights)
    {
        _logger.Information("Добавление требований через строитель.");
        _logger.Debug("Кол-во прав, которые необходимо добавить в требования: {rightsCount}.", rights.Count);
        _logger.Debug("Последнее право, которое необходимо добавить в требования: {rightsLast}.", rights.Last());

        var rightsGroups = rights.GroupBy(right => right.Game!.Name);
        foreach (var group in rightsGroups)
        {
            var rightsDictionary = group.ToDictionary(right => right.Name, right => right.Description);

            _descriptor.Claims.Add(group.Key, rightsDictionary);
        }

        _logger.Information("Добавление требований через строитель - успешно.");

        return this;
    }

    /// <inheritdoc />
    public IJwtBuilder AddUsername(string username)
    {
        _logger.Information("Добавление имя пользователя через строитель.");
        _logger.Debug("Имя пользователя, которое необходимо добавить: {username}.", username);

        _descriptor.Claims.Add("Username", username);

        _logger.Information("Добавление имя пользователя через строитель - успешно.");

        return this;
    }
}