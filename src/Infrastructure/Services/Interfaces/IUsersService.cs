namespace Infrastructure.Services.Interfaces;

using Domain.Models.Users;

/// <summary>
/// Сервис для работы с пользователями.
/// </summary>
public interface IUsersService
{
    /// <summary>
    /// Создать пользователя.
    /// </summary>
    /// <param name="createUser">Пользователь для создания.</param>
    /// <param name="cancellationToken">Токен отмены выполнения операции.</param>
    /// <returns>Модель созданного пользователя.</returns>
    public Task<User> CreateUserAsync(User createUser, CancellationToken cancellationToken);

    /// <summary>
    /// Получить пользователя.
    /// </summary>
    /// <param name="id">Идентификатор пользователя.</param>
    /// <param name="cancellationToken">Токен отмены выполнения операции.</param>
    /// <returns>Модель пользователя или null.</returns>
    public Task<User?> GetUserAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>
    /// Получить пользователей.
    /// </summary>
    /// <param name="cancellationToken">Токен отмены выполнения операции.</param>
    /// <returns>Перечисление пользователей.</returns>
    public Task<IEnumerable<User>> GetUsersAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Обновить пользователя.
    /// </summary>
    /// <param name="id">Идентификатор пользователя.</param>
    /// <param name="updateUser">Обновленный пользователь.</param>
    /// <param name="cancellationToken">Токен отмены выполнения операции.</param>
    public Task UpdateUserAsync(Guid id, User updateUser, CancellationToken cancellationToken);

    /// <summary>
    /// Попытаться удалить пользователя.
    /// </summary>
    /// <param name="id">Идентификатор пользователя.</param>
    /// <param name="cancellationToken">Токен отмены выполнения операции.</param>
    /// <returns>True - если пользователь был удален, иначе - false.</returns>
    public Task<bool> TryDeleteUserAsync(Guid id, CancellationToken cancellationToken);
}