namespace Infrastructure.Extensions;

using System.Runtime.Serialization;
using System.Text.Json;

using Serilog;

/// <summary>
/// Расширения для <see cref="object"/>.
/// </summary>
public static class ObjectExtensions
{
    /// <summary>
    /// Настройки сериализации json.
    /// </summary>
    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    /// <summary>
    /// Преобразовать объект к json строке.
    /// </summary>
    /// <param name="object">Объект, которой необходимо преобразовать.</param>
    /// <returns>Json строка.</returns>
    /// <exception cref="ArgumentNullException">Когда был передан null.</exception>
    /// <exception cref="SerializationException">Когда не удалось сериализовать объект.</exception>
    public static string ToJsonString(this object? @object)
    {
        Log.Debug("Преобразование объекта к json строке.");
        Log.Debug("Объект для преобразования: {object}.", @object);

        if (@object is null)
        {
            Log.Error("Невозможно привести пустой объект к строке.");

            throw new ArgumentNullException(nameof(@object), "Невозможно привести пустой объект к строке.");
        }

        var jsonString = JsonSerializer.Serialize(@object, JsonSerializerOptions);
        if (jsonString is null)
        {
            Log.Error("Не удалось сериализовать объект.");

            throw new SerializationException("Не удалось сериализовать объект.");
        }

        Log.Debug("Преобразование объекта к json строке - завершено.");
        Log.Debug("Json строка: {jsonString}.", jsonString);

        return jsonString;
    }

    /// <summary>
    /// Создать экземпляр.
    /// </summary>
    /// <param name="jsonString">Строка в формате JSON, содержащая объект.</param>
    /// <typeparam name="TType">Тип, к экземпляру которого необходимо привести строку.</typeparam>
    /// <returns>Экземпляр требуемого типа, на основе строки с JSON.</returns>
    /// <exception cref="ArgumentNullException">Когда был передан null.</exception>
    /// <exception cref="SerializationException">Когда не удалось десериализовать содержимое строки.</exception>
    public static TType CreateInstance<TType>(this string? jsonString)
    {
        Log.Debug("Создание экземпляра из json строки.");
        Log.Debug("Json строка: {jsonString}.", jsonString);

        if (string.IsNullOrWhiteSpace(jsonString))
        {
            Log.Error("Невозможно привести пустую строку к требуемому типу.");

            throw new ArgumentNullException(nameof(jsonString), "Невозможно привести пустую строку к требуемому типу.");
        }

        var instance = JsonSerializer.Deserialize<TType>(jsonString, JsonSerializerOptions);
        if (instance is null)
        {
            Log.Error("Не удалось десериализовать содержимое строки.");

            throw new SerializationException("Не удалось десериализовать содержимое строки.");
        }

        return instance;
    }
}