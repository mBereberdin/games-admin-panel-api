namespace Tests.Extensions;

using System.Runtime.Serialization;
using System.Text.Json;

/// <summary>
/// Расширения для <see cref="HttpResponseMessage"/>.
/// </summary>
public static class HttpResponseMessageExtensions
{
    /// <summary>
    /// Создать экземпляр.
    /// </summary>
    /// <param name="httpResponseMessage">Сообщение http ответа.</param>
    /// <typeparam name="TType">Тип, к экземпляру которого необходимо привести сообщение.</typeparam>
    /// <returns>Экземпляр требуемого типа, на основе сообщения http ответа.</returns>
    /// <exception cref="InvalidOperationException">Когда был передан null.</exception>
    /// <exception cref="SerializationException">Когда не удалось десериализовать содержимое запроса.</exception>
    public static TType CreateInstance<TType>(this HttpResponseMessage? httpResponseMessage)
    {
        if (httpResponseMessage is null)
        {
            throw new InvalidOperationException($"Невозможно привести null к типу: {typeof(TType)}");
        }

        var responseString = httpResponseMessage.Content.ReadAsStringAsync().Result;
        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        var instance = JsonSerializer.Deserialize<TType>(responseString, jsonOptions) ??
                       throw new SerializationException("Не удалось десериализовать содержимое запроса.");

        return instance;
    }
}