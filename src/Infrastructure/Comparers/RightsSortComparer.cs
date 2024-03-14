namespace Infrastructure.Comparers;

using Domain.Models.Rights;

/// <summary>
/// Компаратор для сортировки прав.
/// </summary>
public class RightsSortComparer : IEqualityComparer<Right>
{
    /// <summary>
    /// Определяет, равны ли указанные объекты.
    /// </summary>
    /// <param name="firstRight">Первый объект типа Right для сравнения.</param>
    /// <param name="secondRight">Второй объект типа Right для сравнения.</param>
    /// <returns>True - если объекты равны; иначе - false.</returns>
    /// <remarks>Определяет равны ли права по имени, описанию и gameId.</remarks>
    public bool Equals(Right? firstRight, Right? secondRight)
    {
        if (ReferenceEquals(firstRight, secondRight))
        {
            return true;
        }

        if (ReferenceEquals(firstRight, null))
        {
            return false;
        }

        if (ReferenceEquals(secondRight, null))
        {
            return false;
        }

        var areNamesEquals = firstRight.Name.Equals(secondRight.Name, StringComparison.OrdinalIgnoreCase);
        var areDescriptionsEquals =
            firstRight.Description.Equals(secondRight.Description, StringComparison.OrdinalIgnoreCase);
        var areGameIdsEquals = firstRight.GameId.Equals(secondRight.GameId);

        var allEquals = areNamesEquals && areDescriptionsEquals && areGameIdsEquals;

        return allEquals;
    }

    /// <summary>
    /// Получить хэш-код для права.
    /// </summary>
    /// <param name="right">Право.</param>
    /// <returns>Хэш-код права.</returns>
    public int GetHashCode(Right right)
    {
        return HashCode.Combine(right.Name, right.Description, right.GameId);
    }
}