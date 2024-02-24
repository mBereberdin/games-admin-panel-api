namespace Infrastructure.Exceptions.CRUD;

/// <summary>
/// Ошибка удаления сущности.
/// </summary>
public class DeleteException : Exception
{
    /// <inheritdoc cref="DeleteException" />
    /// <param name="message">Сообщение ошибки.</param>
    /// <param name="innerException">Внутренняя ошибка.</param>
    public DeleteException(string message, Exception? innerException = default) : base(message, innerException)
    {
    }
}