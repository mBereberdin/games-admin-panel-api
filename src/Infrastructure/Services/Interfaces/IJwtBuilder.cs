namespace Infrastructure.Services.Interfaces;

using Domain.Models.Rights;

/// <summary>
/// Строитель jwt.
/// </summary>
public interface IJwtBuilder
{
    /// <summary>
    /// Построить токен.
    /// </summary>
    /// <returns>Токен.</returns>
    public string Build();

    /// <summary>
    /// Добавить требования.
    /// </summary>
    /// <param name="rights">Права, которые необходимо добавить в требования.</param>
    /// <returns>Строитель jwt.</returns>
    public IJwtBuilder AddClaims(IList<Right> rights);

    /// <summary>
    /// Добавить имя пользователя.
    /// </summary>
    /// <param name="username">Имя пользователя, которое необходимо добавить.</param>
    /// <returns>Строитель jwt.</returns>
    public IJwtBuilder AddUsername(string username);
}