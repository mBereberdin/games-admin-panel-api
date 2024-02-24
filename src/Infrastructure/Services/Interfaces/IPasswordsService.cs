namespace Infrastructure.Services.Interfaces;

using Domain.Models.Passwords;

/// <summary>
/// Сервис для работы с паролями.
/// </summary>
public interface IPasswordsService
{
    /// <summary>
    /// Создать пароль.
    /// </summary>
    /// <param name="newPassword">Модель нового пароля.</param>
    /// <param name="cancellationToken">Токен отмены выполнения операции.</param>
    /// <returns>Модель созданного пароля.</returns>
    public Task<Password> CreatePasswordAsync(Password newPassword, CancellationToken cancellationToken);

    /// <summary>
    /// Получить пароль.
    /// </summary>
    /// <param name="id">Идентификатор пароля.</param>
    /// <param name="cancellationToken">Токен отмены выполнения операции.</param>
    /// <returns>Модель пароля или null.</returns>
    public Task<Password?> GetPasswordAsync(Guid id, CancellationToken cancellationToken);
}