namespace Tests.Fixtures;

using Microsoft.Extensions.Configuration;

/// <summary>
/// Фикстура appsettings.
/// </summary>
public class AppSettingsFixture
{
    /// <summary>
    /// Получить объект из настроек приложения.
    /// </summary>
    /// <param name="sectionName">Наименование секции, которую необходимо получить.</param>
    /// <typeparam name="T">Тип, к которому необходимо преобразовать полученную секцию.</typeparam>
    /// <returns>Экземпляр секции.</returns>
    public static T GetObjectFromAppSettings<T>(string sectionName)
    {
        var config = new ConfigurationBuilder()
                     .AddJsonFile("appsettings.Tests.json")
                     .Build();

        var requiredSectionInstance = config.GetSection(sectionName).Get<T>() ??
                                      throw new InvalidOperationException("Не удалось получить секцию для тестов.");

        return requiredSectionInstance;
    }

    /// <summary>
    /// Получить путь appsettigns для тестов.
    /// </summary>
    public static string GetTestAppsettingsPath => $"{Directory.GetCurrentDirectory()}/appsettings.Tests.json";
}