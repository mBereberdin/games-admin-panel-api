namespace Infrastructure.Services.Implementations;

using Database;

using Domain.Models.Users;

using Infrastructure.Exceptions.CRUD;
using Infrastructure.Extensions;
using Infrastructure.Services.Interfaces;

using Mapster;

using Microsoft.EntityFrameworkCore;

using Serilog;

/// <inheritdoc cref="IUsersService"/>
public class UsersService : IUsersService, IModelsValidator
{
    /// <summary>
    /// Логгер.
    /// </summary>
    private readonly ILogger _logger;

    /// <inheritdoc cref="AdminDbContext"/>
    private readonly AdminDbContext _context;

    /// <inheritdoc cref="IUsersService"/>
    /// <param name="context">Контекст базы данных панели администрирования.</param>
    /// <param name="logger">Логгер.</param>
    public UsersService(AdminDbContext context, ILogger logger)
    {
        _logger = logger;
        _logger.Debug($"Инициализация {nameof(UsersService)}.");

        _context = context;

        _logger.Debug($"{nameof(UsersService)} - инициализирован.");
    }

    /// <inheritdoc />
    public async Task<User> CreateUserAsync(User createUser, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _logger.Information("Создание пользователя через сервис.");
        _logger.Debug("Пользователь для создания: {createUser}", createUser);

        if (!this.IsValid(createUser, out var validationResults))
        {
            _logger.Error("Невозможно создать пользователя т.к. он заполнен некорректно.");
            _logger.Debug("Ошибки заполнения пользователя: {error}",
                string.Join(',', validationResults.Select(result => result.ErrorMessage)));

            throw new CreateException("Невозможно создать пользователя т.к. он заполнен некорректно.");
        }

        await _context.Users.AddAsync(createUser, cancellationToken);

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException dbUpdateException)
        {
            _logger.Error("При добавлении пользователя в бд произошла ошибка: {dbUpdateException}", dbUpdateException);

            throw new CreateException("При добавлении пользователя в бд произошла ошибка.", dbUpdateException);
        }

        _logger.Information("Создание пользователя через сервис - успешно.");
        _logger.Debug("Созданный пользователь: {createdUser}.", createUser);

        return createUser;
    }

    /// <inheritdoc />
    public async Task<User?> GetUserAsync(Guid id, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _logger.Information("Получение пользователя через сервис.");
        _logger.Debug("Идентификатор пользователя для получения: {id}", id);

        var foundUser = await _context.Users.FindAsync(new object?[] { id }, cancellationToken: cancellationToken);
        if (foundUser is null)
        {
            _logger.Error("Не удалось получить пользователя через сервис.");
            _logger.Debug("Идентификатор пользователя, для которого не удалось получить пользователя: {id}.", id);

            return null;
        }

        _logger.Information("Получение пользователя через сервис - успешно.");
        _logger.Debug("Полученный пользователь: {foundUser}.", foundUser);

        return foundUser;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<User>> GetUsersAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _logger.Information("Получение пользователей через сервис.");

        var users = await _context.Users.ToListAsync(cancellationToken);

        _logger.Information("Получение пользователей через сервис - успешно.");

        return users;
    }

    /// <inheritdoc />
    public async Task UpdateUserAsync(Guid id, User updateUser, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _logger.Information("Обновление пользователя через сервис.");
        _logger.Debug("Идентификатор пользователя для обновления: {id}; модель обновленного пользователя: {updateUser}",
            id, updateUser);

        if (!this.IsValid(updateUser, out var validationResults))
        {
            _logger.Error("Невозможно обновить пользователя т.к. он заполнен некорректно.");
            _logger.Debug("Ошибки заполнения пользователя: {error}",
                string.Join(',', validationResults.Select(result => result.ErrorMessage)));

            throw new UpdateException("Невозможно обновить пользователя т.к. он заполнен некорректно.");
        }

        var foundUser = await GetUserAsync(id, cancellationToken);
        if (foundUser is null)
        {
            _logger.Error("Не удалось получить пользователя для обновления.");
            _logger.Debug("Идентификатор для получения пользователя: {id}.", id);

            throw new NotFoundException("Не удалось получить пользователя для обновления.");
        }

        updateUser.Adapt(foundUser);

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException dbUpdateException)
        {
            _logger.Error("При обновлении пользователя в бд произошла ошибка: {dbUpdateException}", dbUpdateException);

            throw new UpdateException("При обновлении пользователя в бд произошла ошибка.", dbUpdateException);
        }

        _logger.Information("Обновление пользователя через сервис - успешно.");
        _logger.Debug("Обновленный пользователь: {foundUser}.", foundUser);
    }

    /// <inheritdoc />
    public async Task<bool> TryDeleteUserAsync(Guid id, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _logger.Information("Удаление пользователя через сервис.");
        _logger.Debug("Идентификатор пользователя для удаления: {id}", id);

        var foundUser = await GetUserAsync(id, cancellationToken);
        if (foundUser is null)
        {
            _logger.Error("Не удалось получить пользователя для удаления.");
            _logger.Debug("Идентификатор для получения пользователя: {id}.", id);

            return false;
        }

        _context.Users.Remove(foundUser);

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException dbUpdateException)
        {
            _logger.Error("При удалении пользователя в бд произошла ошибка: {dbUpdateException}", dbUpdateException);

            throw new DeleteException("При удалении пользователя в бд произошла ошибка.", dbUpdateException);
        }

        _logger.Information("Удаление пользователя через сервис - успешно.");
        _logger.Debug("Удаленный пользователь: {deletedUser}.", foundUser);

        return true;
    }
}