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
    /// <param name="_">Сервис, который вызывает валидацию.</param>
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
            throw new ArgumentNullException(nameof(model), "Для валидации модели была передана пустая модель.");
        }

        var validationContext = new ValidationContext(model, null, null);
        validationResults = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(model, validationContext, validationResults, true);

        return isValid;
    }

    /// <summary>
    /// Вылидна ли модель.
    /// </summary>
    /// <param name="_">Сервис, который вызывает валидацию.</param>
    /// <param name="models">Список с моделями, которые необходимо проверить на валидность.</param>
    /// <param name="validationResults">Ошибки вылидации.</param>
    /// <typeparam name="TModel">Тип модели.</typeparam>
    /// <exception cref="ArgumentNullException">Когда для валидации моделей был передан пустой список моделей.</exception>
    /// <returns>True - если все модели списка валидны, иначе - false.</returns>
    public static bool AreValid<TModel>(this IModelsValidator _, IList<TModel> models,
        out IList<ValidationResult> validationResults)
    {
        if (models is null || !models.Any())
        {
            throw new ArgumentNullException(nameof(models),
                "Для валидации моделей был передан пустой список моделей.");
        }

        validationResults = new List<ValidationResult>();
        var areCorrect = true;
        foreach (var model in models)
        {
            if (model is null)
            {
                validationResults.Add(
                    new ValidationResult($"Не удалось проверить значение: {nameof(model)} т.к. оно было null."));
                areCorrect = false;

                continue;
            }

            var validationContext = new ValidationContext(model, null, null);
            var isModelValid = Validator.TryValidateObject(model, validationContext, validationResults, true);
            if (!isModelValid)
            {
                areCorrect = false;
            }
        }

        return areCorrect;
    }
}