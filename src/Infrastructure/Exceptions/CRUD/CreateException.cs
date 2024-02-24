namespace Infrastructure.Exceptions.CRUD;

/// <summary>
/// Ошибка создания сущности.
/// </summary>
public class CreateException : Exception
{
    /// <inheritdoc cref="CreateException" />
    /// <param name="message">Сообщение ошибки.</param>
    /// <param name="innerException">Внутренняя ошибка.</param>
    public CreateException(string message, Exception? innerException = default) : base(message, innerException)
    {
    }
}