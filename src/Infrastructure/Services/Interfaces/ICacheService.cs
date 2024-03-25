namespace Infrastructure.Services.Interfaces;

/// <summary>
/// Сервис для работы с кэшем.
/// </summary>
public interface ICacheService
{
    /// <summary>
    /// Получить значение из кэша.
    /// </summary>
    /// <param name="key">Ключ, по которому был сохранен кэш.</param>
    /// <param name="cancellationToken">Токен отмены выполнения операции.</param>
    /// <typeparam name="TType">Тип сохраненного значения.</typeparam>
    /// <returns>Задачу, результатом которой является экземпляр полученного значения или null.</returns>
    /// <exception cref="ArgumentNullException">Когда был передан пустой ключ.</exception>
    public Task<TType?> GetAsync<TType>(object? key, CancellationToken cancellationToken);

    /// <summary>
    /// Установить значение в кэш.
    /// </summary>
    /// <param name="key">Ключ, по которому необходимо сохранить значение в кэш.</param>
    /// <param name="value">Значение.</param>
    /// <param name="cancellationToken">Токен отмены выполнения операции.</param>
    /// <returns>Задачу.</returns>
    /// <exception cref="ArgumentNullException">Когда был передан пустой ключ или значение.</exception>
    public Task SetAsync(object? key, object? value, CancellationToken cancellationToken);

    /// <summary>
    /// Удалить значение из кэша.
    /// </summary>
    /// <param name="key">Ключ, по которому необходимо удалить значение из кэша.</param>
    /// <param name="cancellationToken">Токен отмены выполнения операции.</param>
    /// <returns>Задачу.</returns>
    /// <exception cref="ArgumentNullException">Когда был передан пустой ключ.</exception>
    public Task DeleteAsync(object? key, CancellationToken cancellationToken);

    /// <summary>
    /// Обернуть операции кэширования.
    /// </summary>
    /// <param name="key">Ключ, по которому необходимо получить и сохранить кэш.</param>
    /// <param name="asyncDelegate">Асинхронный метод получения значения если его не будет в кэше.</param>
    /// <param name="cancellationToken">Токен отмены выполнения операции.</param>
    /// <typeparam name="TType">Тип сохраненного значения.</typeparam>
    /// <returns>Задачу, результатом которой является экземпляр полученного значения или null.</returns>
    /// <exception cref="ArgumentNullException">Когда был передан пустой ключ.</exception>
    public Task<TType?> WrapCacheOperationsAsync<TType>(object? key, Func<Task<TType?>> asyncDelegate,
        CancellationToken cancellationToken);
}