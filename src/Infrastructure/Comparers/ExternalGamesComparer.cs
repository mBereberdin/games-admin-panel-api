namespace Infrastructure.Comparers;

using Domain.Models.Games;

/// <summary>
/// Компаратор для сравнения внешних игр.
/// </summary>
public class ExternalGamesComparer : IEqualityComparer<Game>
{
    /// <summary>
    /// Определяет, равны ли указанные объекты.
    /// </summary>
    /// <param name="firstGame">Первый объект типа Game для сравнения.</param>
    /// <param name="secondGame">Второй объект типа Game для сравнения.</param>
    /// <returns>True - если объекты равны; иначе - false.</returns>
    public bool Equals(Game? firstGame, Game? secondGame)
    {
        if (ReferenceEquals(firstGame, secondGame))
        {
            return true;
        }

        if (ReferenceEquals(firstGame, null))
        {
            return false;
        }

        if (ReferenceEquals(secondGame, null))
        {
            return false;
        }

        var areNamesEquals = firstGame.Name.Equals(secondGame.Name, StringComparison.OrdinalIgnoreCase);
        var areDescriptionsEquals = !string.IsNullOrWhiteSpace(firstGame.Description) &&
                                    !string.IsNullOrWhiteSpace(secondGame.Description) && firstGame.Description.Equals(
                                        secondGame.Description, StringComparison.OrdinalIgnoreCase);

        var allEquals = areNamesEquals && areDescriptionsEquals;

        return allEquals;
    }

    /// <summary>
    /// Получить хэш-код для игры.
    /// </summary>
    /// <param name="game">Игра.</param>
    /// <returns>Хэш-код игры.</returns>
    public int GetHashCode(Game game)
    {
        return HashCode.Combine(game.Name, game.Description);
    }
}