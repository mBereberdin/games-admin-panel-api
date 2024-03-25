namespace Infrastructure.Settings;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// Настройки minio.
/// </summary>
public record MinioSettings
{
    /// <summary>
    /// Конечная точка.
    /// </summary>
    [Required]
    public string Endpoint { get; init; } = null!;

    /// <summary>
    /// Ключ доступа.
    /// </summary>
    [Required]
    public string AccessKey { get; init; } = null!;

    /// <summary>
    /// Секретный ключ.
    /// </summary>
    [Required]
    public string SecretKey { get; init; } = null!;

    /// <summary>
    /// Наименование хранилища аватаров.
    /// </summary>
    [Required]
    public string AvatarsBucketName { get; init; } = null!;
}