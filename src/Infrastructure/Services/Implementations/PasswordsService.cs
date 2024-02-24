namespace Infrastructure.Services.Implementations;

using Database;

using Domain.Models.Passwords;

using Infrastructure.Exceptions.CRUD;
using Infrastructure.Extensions;
using Infrastructure.Services.Interfaces;

using Microsoft.EntityFrameworkCore;

using Serilog;

/// <inheritdoc cref="IPasswordsService"/>
public class PasswordsService : IPasswordsService, IModelsValidator
{
    /// <summary>
    /// Логгер.
    /// </summary>
    private readonly ILogger _logger;

    /// <summary>
    /// Контекст базы данных панели администрирования.
    /// </summary>
    private readonly AdminDbContext _context;

    /// <inheritdoc cref="IPasswordsService"/>
    /// <param name="context">Контекст базы данных панели администрирования.</param>
    /// <param name="logger">Логгер.</param>
    public PasswordsService(AdminDbContext context, ILogger logger)
    {
        _logger = logger;
        _logger.Debug($"Инициализация {nameof(PasswordsService)}.");

        _context = context;

        _logger.Debug($"{nameof(PasswordsService)} - инициализирован.");
    }

    /// <inheritdoc />
    public async Task<Password> CreatePasswordAsync(Password newPassword, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _logger.Information("Создание пароля через сервис.");
        _logger.Debug("Пароль для создания: {newPassword}", newPassword);

        if (!this.IsValid(newPassword, out var validationResults))
        {
            _logger.Error("Невозможно создать пароль т.к. он заполнен некорректно.");
            _logger.Debug("Ошибки заполнения пароля: {error}",
                string.Join(',', validationResults.Select(result => result.ErrorMessage)));

            throw new CreateException("Невозможно создать пароль т.к. он заполнен некорректно.");
        }

        await _context.Passwords.AddAsync(newPassword, cancellationToken);

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException updateException)
        {
            _logger.Error("При добавлении пароля в бд произошла ошибка: {updateException}", updateException);

            throw new CreateException("При добавлении пароля в бд произошла ошибка.", updateException);
        }

        _logger.Information("Создание пароля через сервис - успешно.");
        _logger.Debug("Модель созданного пароля: {newPassword}", newPassword);

        return newPassword;
    }

    /// <inheritdoc />
    public async Task<Password?> GetPasswordAsync(Guid id, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _logger.Information("Получение пароля через сервис.");
        _logger.Debug("Идентификатор пароля для получения: {id}", id);

        var foundPassword = await _context.Passwords.FindAsync(new object[] { id }, cancellationToken);
        if (foundPassword is null)
        {
            _logger.Error("Не удалось получить пароль через сервис.");
            _logger.Debug("Идентификатор пароля, для которого не удалось получить пароль: {id}.", id);

            return null;
        }

        _logger.Information("Получение пароля через сервис - успешно.");
        _logger.Debug("Полученный пароль: {foundPassword}.", foundPassword);

        return foundPassword;
    }
}