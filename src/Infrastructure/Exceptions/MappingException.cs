namespace Infrastructure.Exceptions;

/// <summary>
/// Ошибка преобразования.
/// </summary>
public class MappingException : Exception
{
    /// <inheritdoc cref="MappingException" />
    /// <param name="message">Сообщение ошибки.</param>
    /// <param name="innerException">Внутренняя ошибка.</param>
    public MappingException(string message, Exception? innerException = default) : base(message, innerException)
    {
    }
}