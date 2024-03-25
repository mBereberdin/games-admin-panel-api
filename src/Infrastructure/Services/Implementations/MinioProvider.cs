namespace Infrastructure.Services.Implementations;

using Infrastructure.Settings;

using Microsoft.Extensions.Options;

using Minio;
using Minio.DataModel.Args;

using Serilog;

public class MinioProvider
{
    private readonly IMinioClient _minioClient;
    private readonly MinioSettings _minioSettings;
    private readonly ILogger _logger;

    public MinioProvider(IMinioClient minioClient, IOptions<MinioSettings> minioSettings, ILogger logger)
    {
        _minioClient = minioClient;
        _minioSettings = minioSettings.Value;
        _logger = logger;
    }

    private async Task CreateIfNotExistsAvatarsBucketAsync()
    {
        var bucketExistsArgs = new BucketExistsArgs().WithBucket(_minioSettings.AvatarsBucketName);

        var isExists = await _minioClient.BucketExistsAsync(bucketExistsArgs);
        if (isExists)
        {
            return;
        }

        var makeBucketArgs = new MakeBucketArgs().WithBucket(_minioSettings.AvatarsBucketName);

        await _minioClient.MakeBucketAsync(makeBucketArgs);
    }

    public void Initialize()
    {
        CreateIfNotExistsAvatarsBucketAsync().Wait();
    }
}