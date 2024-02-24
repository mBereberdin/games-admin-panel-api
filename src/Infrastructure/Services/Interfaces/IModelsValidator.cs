namespace Infrastructure.Services.Interfaces;

using Infrastructure.Extensions;

/// <summary>
/// Сервис, которому необходимо проверять валидность моделей.
/// </summary>
/// <remarks>Используется для пердоставления доступа к методу расширения:
/// <see cref="ModelsValidatorExtensions.IsValid(IModelsValidator, TModel, System.Collections.Generic.IList{System.ComponentModel.DataAnnotations.ValidationResult})"/>.
/// </remarks>
public interface IModelsValidator
{
}