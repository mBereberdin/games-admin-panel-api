namespace Infrastructure.Services.Interfaces;

using Domain.Models.Games;

using Infrastructure.Exceptions.CRUD;

/// <summary>
/// Сервис для работы с играми.
/// </summary>
public interface IGamesService
{
    /// <summary>
    /// Получить игру.
    /// </summary>
    /// <param name="name">Наименование игры.</param>
    /// <param name="cancellationToken">Токен отмены выполнения операции.</param>
    /// <returns>Задачу, результатом которой является модель игры или null.</returns>
    public Task<Game?> GetAsync(string name, CancellationToken cancellationToken);

    /// <summary>
    /// Получить игру.
    /// </summary>
    /// <param name="id">Идентификатор игры.</param>
    /// <param name="cancellationToken">Токен отмены выполнения операции.</param>
    /// <returns>Задачу, результатом которой является модель игры или null.</returns>
    public Task<Game?> GetAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>
    /// Добавить игру.
    /// </summary>
    /// <param name="gameToAdd">Игра для добавления.</param>
    /// <param name="cancellationToken">Токен отмены выполнения операции.</param>
    /// <returns>Задачу, результатом которой является модель добавленной игры.</returns>
    /// <exception cref="CreateException">Когда при создании игры возникла ошибка.</exception>
    public Task<Game> AddAsync(Game gameToAdd, CancellationToken cancellationToken);

    /// <summary>
    /// Обновить игру.
    /// </summary>
    /// <param name="id">Идентификатор игры для обновления.</param>
    /// <param name="updatedGame">Игра для обновления.</param>
    /// <param name="cancellationToken">Токен отмены выполнения операции.</param>
    /// <returns>Задачу, результатом которой является обновленная модель игры.</returns>
    /// <exception cref="ArgumentException">Когда игра заполнена некорректно.</exception>
    /// <exception cref="NotFoundException">Когда не удалось получить игру для обновления.</exception>
    /// <exception cref="UpdateException">Когда при обновлении игры возникла ошибка.</exception>
    public Task<Game> UpdateAsync(Guid id, Game updatedGame, CancellationToken cancellationToken);
}