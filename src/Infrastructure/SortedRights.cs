namespace Infrastructure;

using Domain.Models.Rights;

/// <summary>
/// Отсортированные права.
/// </summary>
public class SortedRights
{
    /// <summary>
    /// Права, которые необходимо удалить.
    /// </summary>
    public IList<Right>? RightsToDelete { get; set; }

    /// <summary>
    /// Права, которые необходимо обновить.
    /// </summary>
    public IList<Right>? RightsToUpdate { get; set; }

    /// <summary>
    /// Права, которые необходимо добавить.
    /// </summary>
    public IList<Right>? RightsToCreate { get; set; }
}