namespace Infrastructure.Services.Interfaces;

using Domain.Models.Rights;

using Infrastructure.Exceptions.CRUD;

/// <summary>
/// Сервис для работы с парвами.
/// </summary>
public interface IRightsService
{
    /// <summary>
    /// Получить все права.
    /// </summary>
    /// <param name="gameName">Наименование игры, для которой необходимо получить все права.</param>
    /// <param name="cancellationToken">Токен отмены выполнения операции.</param>
    /// <returns>Задачу, результатом которой является список всех прав игры.</returns>
    /// <exception cref="ArgumentNullException">Когда было передано пустое наименование игры.</exception>
    /// <exception cref="NotFoundException">Когда не удалось найти игру для получения всех прав.</exception>
    public Task<IList<Right>> GetAllAsync(string gameName, CancellationToken cancellationToken);

    /// <summary>
    /// Получить право.
    /// </summary>
    /// <param name="rightsNames">Наименования прав.</param>
    /// <param name="cancellationToken">Токен отмены выполнения операции.</param>
    /// <returns>Задачу, результатом которой является список прав или null.</returns>
    /// <exception cref="ArgumentNullException">Когда были переданы пустые наименования прав.</exception>
    public Task<IList<Right>?> GetAsync(CancellationToken cancellationToken, params string[] rightsNames);

    /// <summary>
    /// Зарегистрировать права.
    /// </summary>
    /// <param name="rightsToRegister">Права для регистрации.</param>
    /// <param name="gameName">Наименование игры, для которой необходимо зарегистрировать права.</param>
    /// <param name="cancellationToken">Токен отмены выполнения операции.</param>
    /// <returns>Задачу.</returns>
    /// <exception cref="ArgumentException">Когда права заполнены некорректно.</exception>
    public Task RegisterAsync(IList<Right> rightsToRegister, string gameName, CancellationToken cancellationToken);

    /// <summary>
    /// Попытаться удалить список прав.
    /// </summary>
    /// <param name="rightsToDelete">Права, которые необходимо удалить.</param>
    /// <param name="cancellationToken">Токен отмены выполнения операции.</param>
    /// <returns>True - если права успешно удалены, иначе - false.</returns>
    /// <exception cref="DeleteException">Когда при удалении списка прав в бд произошла ошибка.</exception>
    public Task<bool> TryDeleteRangeAsync(IList<Right> rightsToDelete, CancellationToken cancellationToken);

    /// <summary>
    /// Добавить список прав.
    /// </summary>
    /// <param name="rightsToAdd">Права, которые необходимо добавить.</param>
    /// <param name="cancellationToken">Токен отмены выполнения операции.</param>
    /// <returns>Задачу.</returns>
    /// <exception cref="ArgumentException">Когда невозможно добавить список прав т.к. некоторые из прав - заполнены некорректно.</exception>
    /// <exception cref="CreateException">Когда при добавлении списка прав в бд произошла ошибка.</exception>
    public Task AddRangeAsync(IList<Right> rightsToAdd, CancellationToken cancellationToken);

    /// <summary>
    /// Обновить список прав.
    /// </summary>
    /// <param name="rightsToUpdate">Права, которые необходимо обновить.</param>
    /// <param name="cancellationToken">Токен отмены выполнения операции.</param>
    /// <returns>Задачу.</returns>
    /// <exception cref="ArgumentException">Когда невозможно обновить список прав т.к. некоторые из прав - заполнены некорректно.</exception>
    /// <exception cref="NotFoundException">Когда не удалось найти права для обновления.</exception>
    /// <exception cref="UpdateException">Когда при обновлении списка прав в бд произошла ошибка.</exception>
    public Task UpdateRangeAsync(IList<Right> rightsToUpdate, CancellationToken cancellationToken);

    /// <summary>
    /// Отсортировать права.
    /// </summary>
    /// <param name="gameName">Наименование игры, для которой необходимо отсортировтать права.</param>
    /// <param name="rightsToSort">Права для сортировки.</param>
    /// <param name="cancellationToken">Токен отмены выполнения операции.</param>
    /// <returns>Задача, результатом которой являются отсортированные права.</returns>
    /// <remarks>
    /// - Если права есть в списке на сортировку, но нет в бд - будут предоставлены права, которые необходимо добавить;
    /// - Если прав нет в списке на сортировку, но есть в бд - будут предоставлены права, которые необходимо удалить;
    /// - Если права есть в списке на сортировку и в бд - будут сравнены свойства прав, и, в случае отличий - будут предоставлены права, которые необходимо обновить.
    /// </remarks>
    /// <exception cref="ArgumentNullException">Когда для сортировки прав игры было передано пустое наименование игры.</exception>
    /// <exception cref="ArgumentException">Когда невозможно отсортировать список прав т.к. некоторые из прав - заполнены некорректно.</exception>
    public Task<SortedRights> SortGamesRightsAsync(string gameName, IList<Right> rightsToSort,
        CancellationToken cancellationToken);
}