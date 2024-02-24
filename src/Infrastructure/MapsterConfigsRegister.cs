namespace Infrastructure;

using Domain.DTOs.Users;
using Domain.Models.Passwords;
using Domain.Models.Users;

using Mapster;

/// <summary>
/// Реестр конфигураций мапстера.
/// </summary>
public class MapsterConfigsRegister : IRegister
{
    /// <summary>
    /// Регистрация реестра.
    /// </summary>
    /// <param name="config">Конфигурация адаптера типов.</param>
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<User, User>().IgnoreIf((source, destination) => source.Id.Equals(Guid.Empty),
            destination => destination.Id);

        config.NewConfig<CreateUserDto, User>().MapWith(source => new User
        {
            Email = source.Email,
            Nickname = source.Nickname,
            Id = Guid.Empty
        });

        config.NewConfig<UpdateUserDto, User>().IgnoreNullValues(true);

        config.NewConfig<string, Password>()
            .MapWith(source => new Password { Id = Guid.Empty, EncryptedValue = source, UserId = Guid.Empty });

        config.NewConfig<Password, Password>().IgnoreIf((source, destination) => source.Id.Equals(Guid.Empty),
            destination => destination.Id);
    }
}