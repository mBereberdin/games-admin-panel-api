namespace Infrastructure.Middlewares;

using System.Text;

using Infrastructure.Exceptions;
using Infrastructure.Exceptions.CRUD;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

/// <summary>
/// Промежуточный слой для обработки ошибок приложения.
/// </summary>
public class ExceptionsMiddleware
{
    /// <summary>
    /// Делегат следующей обработки запроса.
    /// </summary>
    private readonly RequestDelegate _next;

    /// <summary>
    /// Логгер.
    /// </summary>
    private readonly ILogger<ExceptionsMiddleware> _logger;

    /// <inheritdoc cref="ExceptionsMiddleware"/>
    /// <param name="logger">Логгер.</param>
    /// <param name="next">Делегат следующей обработки запроса.</param>
    public ExceptionsMiddleware(ILogger<ExceptionsMiddleware> logger,
        RequestDelegate next)
    {
        _logger = logger;
        _next = next;
    }

    /// <summary>
    /// Выполнить операцию.
    /// </summary>
    /// <param name="httpContext">Контекст запроса.</param>
    /// <returns>Задачу.</returns>
    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (CreateException exception)
        {
            _logger.LogError("При выполнении запроса произошла ошибка создания сущности.");
            _logger.LogDebug($"Ошибка создания сущности: {exception}, трассировка: {exception.StackTrace}.");

            await WriteResponseAsync(httpContext, StatusCodes.Status500InternalServerError, exception.Message);
        }
        catch (UpdateException exception)
        {
            _logger.LogError("При выполнении запроса произошла ошибка обновления сущности.");
            _logger.LogDebug($"Ошибка обновления сущности: {exception}, трассировка: {exception.StackTrace}.");

            await WriteResponseAsync(httpContext, StatusCodes.Status500InternalServerError, exception.Message);
        }
        catch (NotFoundException exception)
        {
            _logger.LogError("При выполнении запроса произошла ошибка поиска сущности.");
            _logger.LogDebug($"Ошибка поиска сущности: {exception}, трассировка: {exception.StackTrace}.");

            await WriteResponseAsync(httpContext, StatusCodes.Status500InternalServerError, exception.Message);
        }
        catch (DeleteException exception)
        {
            _logger.LogError("При выполнении запроса произошла ошибка удаления сущности.");
            _logger.LogDebug($"Ошибка удаления сущности: {exception}, трассировка: {exception.StackTrace}.");

            await WriteResponseAsync(httpContext, StatusCodes.Status500InternalServerError, exception.Message);
        }
        catch (MappingException exception)
        {
            _logger.LogError("При выполнении запроса произошла ошибка преобразования.");
            _logger.LogDebug($"Ошибка преобразования: {exception}, трассировка: {exception.StackTrace}.");

            await WriteResponseAsync(httpContext, StatusCodes.Status500InternalServerError, exception.Message);
        }
        catch (SortException exception)
        {
            _logger.LogError("При выполнении запроса произошла ошибка сортировки.");
            _logger.LogDebug($"Ошибка сортировки: {exception}, трассировка: {exception.StackTrace}.");

            await WriteResponseAsync(httpContext, StatusCodes.Status500InternalServerError, exception.Message);
        }
        catch (HeadersException exception)
        {
            _logger.LogError("При выполнении запроса произошла ошибка заголовков.");
            _logger.LogDebug($"Ошибка заголовков: {exception}, трассировка: {exception.StackTrace}.");

            await WriteResponseAsync(httpContext, StatusCodes.Status500InternalServerError, exception.Message);
        }
        catch (Exception exception)
        {
            _logger.LogError("При выполнении запроса произошла непредвиденная ошибка.");
            _logger.LogDebug($"Непредвиденная ошибка: {exception}, внутренняя ошибка: {exception.InnerException}.");

            await WriteResponseAsync(httpContext, StatusCodes.Status500InternalServerError, exception.Message);
        }
    }

    /// <summary>
    /// Записать ответ.
    /// </summary>
    /// <param name="httpContext">Контекст http запроса.</param>
    /// <param name="statusCode">Код статуса для ответа.</param>
    /// <param name="message">Сообщение для ответа.</param>
    /// <returns>Задачу.</returns>
    private async Task WriteResponseAsync(HttpContext httpContext, int statusCode, string message)
    {
        httpContext.Response.StatusCode = statusCode;
        httpContext.Response.ContentType = "text/plain";
        await httpContext.Response.WriteAsync(message, Encoding.UTF8);
    }
}