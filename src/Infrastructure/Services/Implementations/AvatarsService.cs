namespace Infrastructure.Services.Implementations;

using System.Text;

using Infrastructure.Settings;

using Microsoft.Extensions.Options;

using Minio;
using Minio.DataModel.Args;

using Serilog;

public class AvatarsService
{
    private const string AVATAR_PREFIX_TEMPLATE = "avatar-{0}.png";
    private readonly IMinioClient _minioClient;
    private readonly string _avatarsBucketName;
    private readonly ILogger _logger;

    public AvatarsService(IMinioClient minioClient, IOptions<MinioSettings> minioSettings, ILogger logger)
    {
        _minioClient = minioClient;
        _avatarsBucketName = minioSettings.Value.AvatarsBucketName;
        _logger = logger;
    }

    public async Task<string> GetAsync(Guid avatarId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _logger.Information("");
        var avatarFilename = string.Format(AVATAR_PREFIX_TEMPLATE, avatarId.ToString());

        var memoryStream = new MemoryStream();
        var getObjectArgs = new GetObjectArgs().WithBucket(_avatarsBucketName)
                                               .WithObject(avatarFilename)
                                               .WithCallbackStream(stream => stream.CopyTo(memoryStream));

        await _minioClient.GetObjectAsync(getObjectArgs, cancellationToken);

        memoryStream.Position = 0;
        using var reader = new StreamReader(memoryStream);
        var string64 = await reader.ReadToEndAsync(cancellationToken);
        await memoryStream.DisposeAsync();

        return string64;
    }

    public async Task<Guid> SaveAsync(string avatarBase64, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var guidForAvatar = Guid.NewGuid();
        var avatarFilename = string.Format(AVATAR_PREFIX_TEMPLATE, guidForAvatar.ToString());

        using var stream = new MemoryStream();
        var bytes = Encoding.UTF8.GetBytes(avatarBase64);
        await stream.WriteAsync(bytes, cancellationToken);
        stream.Position = 0;

        var putObjectArgs = new PutObjectArgs().WithBucket(_avatarsBucketName)
                                               .WithStreamData(stream)
                                               .WithContentType("image/png")
                                               .WithObjectSize(bytes.Length)
                                               .WithObject(avatarFilename);

        await _minioClient.PutObjectAsync(putObjectArgs, cancellationToken);

        return guidForAvatar;
    }
}