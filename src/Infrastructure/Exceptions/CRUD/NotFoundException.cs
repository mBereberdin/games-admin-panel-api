namespace Infrastructure.Exceptions.CRUD;

/// <summary>
/// Ошибка поиска сущности.
/// </summary>
public class NotFoundException : Exception
{
    /// <inheritdoc cref="NotFoundException" />
    /// <param name="message">Сообщение ошибки.</param>
    /// <param name="innerException">Внутренняя ошибка.</param>
    public NotFoundException(string message, Exception? innerException = default) : base(message, innerException)
    {
    }
}