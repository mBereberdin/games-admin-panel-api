namespace Infrastructure.Extensions;

using System.ComponentModel.DataAnnotations;

using Infrastructure.Services.Interfaces;

/// <summary>
/// Расширения для классов которым необходимо использовать валидацию моделей.
/// </summary>
public static class ModelsValidatorExtensions
{
    /// <summary>
    /// Вылидна ли модель.
    /// </summary>
    /// <param name="_">Сервис, который вызывает валдацию.</param>
    /// <param name="model">Модель, которую необходимо проверить на валидность.</param>
    /// <param name="validationResults">Ошибки вылидации.</param>
    /// <typeparam name="TModel">Тип модели.</typeparam>
    /// <exception cref="ArgumentNullException">Когда для валидации модели была передана пустая модель.</exception>
    /// <returns>True - если модель валидна, иначе - false.</returns>
    public static bool IsValid<TModel>(this IModelsValidator _, TModel model,
        out IList<ValidationResult> validationResults)
    {
        if (model is null)
        {
            throw new ArgumentNullException("Для валидации модели была передана пустая модель.");
        }

        var validationContext = new ValidationContext(model, null, null);
        validationResults = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(model, validationContext, validationResults, true);

        return isValid;
    }
}