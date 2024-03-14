namespace Infrastructure.Exceptions;

/// <summary>
/// Ошибка сортировки.
/// </summary>
public class SortException : Exception
{
    /// <inheritdoc cref="SortException" />
    /// <param name="message">Сообщение ошибки.</param>
    /// <param name="innerException">Внутренняя ошибка.</param>
    public SortException(string message, Exception? innerException = default) : base(message, innerException)
    {
    }
}