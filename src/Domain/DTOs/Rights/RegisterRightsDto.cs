namespace Domain.DTOs.Rights;

using Domain.DTOs.Games;

/// <summary>
/// ДТО регистрации прав.
/// </summary>
public record RegisterRightsDto
{
    /// <inheritdoc cref="RegisterRightsDto"/>
    /// <param name="rights">Права, которые необходимо зарегистрировать.</param>
    /// <param name="game">Игра, для которой необходимо зарегистрировать права.</param>
    public RegisterRightsDto(IEnumerable<ExternalCreateRightDto> rights, ExternalGameDto game)
    {
        Rights = rights;
        Game = game;
    }

    /// <summary>
    /// Права, которые необходимо зарегистрировать.
    /// </summary>
    public IEnumerable<ExternalCreateRightDto> Rights { get; init; }

    /// <summary>
    /// Игра, для которой необходимо зарегистрировать права.
    /// </summary>
    public ExternalGameDto Game { get; init; }
}