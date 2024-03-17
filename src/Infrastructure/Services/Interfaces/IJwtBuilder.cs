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
    /// <exception cref="ArgumentNullException">Когда для добавления требований через строитель были переданы пустые права.</exception>
    public IJwtBuilder AddClaims(IList<Right>? rights);

    /// <summary>
    /// Добавить имя пользователя.
    /// </summary>
    /// <param name="username">Имя пользователя, которое необходимо добавить.</param>
    /// <returns>Строитель jwt.</returns>
    /// <exception cref="ArgumentNullException">Когда для добавления имени пользователя через строитель было передано пустое имя пользователя.</exception>
    public IJwtBuilder AddUsername(string username);
}