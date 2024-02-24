namespace Tests.Extensions;

using System.Text;
using System.Text.Json;

/// <summary>
/// Расширения для <see cref="object"/>.
/// </summary>
public static class ObjectExtensions
{
    /// <summary>
    /// Преобразовать к строковому контенту.
    /// </summary>
    /// <param name="object">Объект, которой необходимо преобразовать.</param>
    /// <returns>Объект, в формате строкового контента.</returns>
    public static StringContent ToStringContent(this object @object)
    {
        var objectJsonString = JsonSerializer.Serialize(@object);
        var objectStringContent = new StringContent(objectJsonString, Encoding.UTF8, "application/json");

        return objectStringContent;
    }
}