namespace Infrastructure.Exceptions.CRUD;

/// <summary>
/// Ошибка обновления сущности.
/// </summary>
public class UpdateException : Exception
{
    /// <inheritdoc cref="UpdateException" />
    /// <param name="message">Сообщение ошибки.</param>
    /// <param name="innerException">Внутренняя ошибка.</param>
    public UpdateException(string message, Exception? innerException = default) : base(message, innerException)
    {
    }
}