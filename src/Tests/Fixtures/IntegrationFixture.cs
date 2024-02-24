namespace Tests.Fixtures;

using Tests.Integrations;

/// <summary>
/// Фикстура интеграционного теста.
/// </summary>
public class IntegrationFixture : IClassFixture<WebApiApplicationFactory<Program>>
{
    /// <inheritdoc cref="IntegrationFixture"/>
    /// <param name="factory">Фабрика проекта Example.</param>
    protected IntegrationFixture(WebApiApplicationFactory<Program> factory)
    {
        Client = factory.CreateClient();
    }

    /// <summary>
    /// HTTP клиент.
    /// </summary>
    protected HttpClient Client { get; }
}