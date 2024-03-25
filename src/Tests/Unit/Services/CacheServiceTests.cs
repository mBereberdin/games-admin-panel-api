namespace Tests.Unit.Services;

using Domain.Models.Users;

using FluentAssertions;

using Infrastructure.Services.Implementations;
using Infrastructure.Services.Interfaces;
using Infrastructure.Settings;

using Microsoft.Extensions.Options;

using Tests.FakeData;
using Tests.Fixtures;

/// <summary>
/// Тесты для <see cref="ICacheService"/>.
/// </summary>
public class CacheServiceTests : ServiceFixture
{
    /// <summary>
    /// Настройки .
    /// </summary>
    private readonly IOptions<CacheSettings> _cacheSettingsOptions;

    /// <inheritdoc cref="JwtBuilderTests"/>
    public CacheServiceTests()
    {
        var cacheSettings = AppSettingsFixture.GetObjectFromAppSettings<CacheSettings>(nameof(CacheSettings));
        var cacheSettingsForUnitTests = new CacheSettings
        {
            RedisUrl = cacheSettings.RedisUrl,
            RedisPassword = cacheSettings.RedisPassword,
            CacheLifetimeString = "01:00"
        };
        _cacheSettingsOptions = Options.Create(cacheSettingsForUnitTests);
    }

    #region Set

    [Fact]
    public async Task SetAsync_WithGuidKeyAndModel_CacheSetSuccess()
    {
        var redisProvider = new RedisProvider(_cacheSettingsOptions, MockLogger);
        var cacheService = new CacheService(redisProvider, _cacheSettingsOptions, MockLogger);
        var cacheObject = FakeUsers.GetManyCorrectExistsModels().First();
        var guidForTest = Guid.NewGuid();

        // Act
        await cacheService.SetAsync(guidForTest, cacheObject, MockCancellationToken);

        var gotCache = await cacheService.GetAsync<User>(guidForTest, MockCancellationToken);

        // Assert
        gotCache.Should().NotBeNull();
        gotCache!.Id.Should().Be(cacheObject.Id);
        gotCache.Nickname.Should().BeEquivalentTo(cacheObject.Nickname);
        gotCache.Email.Should().BeEquivalentTo(cacheObject.Email);
    }

    [Fact]
    public async Task SetAsync_WithStringKeyAndModel_CacheSetSuccess()
    {
        var redisProvider = new RedisProvider(_cacheSettingsOptions, MockLogger);
        var cacheService = new CacheService(redisProvider, _cacheSettingsOptions, MockLogger);
        var cacheObject = FakeUsers.GetManyCorrectExistsModels().First();
        const string stringKey = "testStringForCache";

        // Act
        await cacheService.SetAsync(stringKey, cacheObject, MockCancellationToken);

        var gotCache = await cacheService.GetAsync<User>(stringKey, MockCancellationToken);

        // Assert
        gotCache.Should().NotBeNull();
        gotCache!.Id.Should().Be(cacheObject.Id);
        gotCache.Nickname.Should().BeEquivalentTo(cacheObject.Nickname);
        gotCache.Email.Should().BeEquivalentTo(cacheObject.Email);
    }
    
    [Fact]
    public async Task SetAsync_WithStringKeyAndModel_CacheNotSet()
    {
        var redisProvider = new RedisProvider(_cacheSettingsOptions, MockLogger);
        var cacheService = new CacheService(redisProvider, _cacheSettingsOptions, MockLogger);
        var cacheObject = FakeUsers.GetManyCorrectExistsModels().First();
        const string stringKey = "testStringForCache";

        // Act
        await cacheService.SetAsync(stringKey, cacheObject, MockCancellationToken);

        var gotCache = await cacheService.GetAsync<User>(stringKey, MockCancellationToken);

        // Assert
        gotCache.Should().NotBeNull();
        gotCache!.Id.Should().Be(cacheObject.Id);
        gotCache.Nickname.Should().BeEquivalentTo(cacheObject.Nickname);
        gotCache.Email.Should().BeEquivalentTo(cacheObject.Email);
    }

    [Fact]
    public async Task SetAsync_WithNulls_ArgumentNullException()
    {
        var redisProvider = new RedisProvider(_cacheSettingsOptions, MockLogger);
        var cacheService = new CacheService(redisProvider, _cacheSettingsOptions, MockLogger);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await cacheService.SetAsync(null!, new object(), MockCancellationToken)
        );

        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await cacheService.SetAsync("", new object(), MockCancellationToken)
        );

        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await cacheService.SetAsync("test", null!, MockCancellationToken)
        );
    }

    #endregion

    #region Get

    [Fact]
    public async Task GetAsync_WithExistsKey_ReturnObjectFromCacheSuccess()
    {
        var redisProvider = new RedisProvider(_cacheSettingsOptions, MockLogger);
        var cacheService = new CacheService(redisProvider, _cacheSettingsOptions, MockLogger);
        var cacheObject = FakeUsers.GetManyCorrectExistsModels().First();
        var guidForTest = Guid.NewGuid();

        await cacheService.SetAsync(guidForTest, cacheObject, MockCancellationToken);

        // Act
        var gotCache = await cacheService.GetAsync<User>(guidForTest, MockCancellationToken);

        // Assert
        gotCache.Should().NotBeNull();
        gotCache!.Id.Should().Be(cacheObject.Id);
        gotCache.Nickname.Should().BeEquivalentTo(cacheObject.Nickname);
        gotCache.Email.Should().BeEquivalentTo(cacheObject.Email);
    }

    [Fact]
    public async Task GetAsync_WithNotExistsKey_ReturnNull()
    {
        var redisProvider = new RedisProvider(_cacheSettingsOptions, MockLogger);
        var cacheService = new CacheService(redisProvider, _cacheSettingsOptions, MockLogger);
        var guidForTest = Guid.NewGuid();

        // Act
        var gotCache = await cacheService.GetAsync<User>(guidForTest, MockCancellationToken);

        // Assert
        gotCache.Should().BeNull();
    }

    [Fact]
    public async Task GetAsync_WithNullKey_ArgumentNullException()
    {
        var redisProvider = new RedisProvider(_cacheSettingsOptions, MockLogger);
        var cacheService = new CacheService(redisProvider, _cacheSettingsOptions, MockLogger);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await cacheService.GetAsync<User>(null!, MockCancellationToken));
    }

    #endregion

    #region Delete

    [Fact]
    public async Task DeleteAsync_WithExistsKey_DeleteSuccess()
    {
        var redisProvider = new RedisProvider(_cacheSettingsOptions, MockLogger);
        var cacheService = new CacheService(redisProvider, _cacheSettingsOptions, MockLogger);
        var cacheObject = FakeUsers.GetManyCorrectExistsModels().First();
        var guidForTest = Guid.NewGuid();

        await cacheService.SetAsync(guidForTest, cacheObject, MockCancellationToken);
        var gotCache = await cacheService.GetAsync<User>(guidForTest, MockCancellationToken);

        // Act
        await cacheService.DeleteAsync(guidForTest, MockCancellationToken);
        var gotCacheAfterDelete = await cacheService.GetAsync<User>(guidForTest, MockCancellationToken);

        // Assert
        gotCache.Should().NotBeNull();
        gotCache!.Nickname.Should().BeEquivalentTo(cacheObject.Nickname);
        gotCacheAfterDelete.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_WithNotExistsKey_NoExceptions()
    {
        var redisProvider = new RedisProvider(_cacheSettingsOptions, MockLogger);
        var cacheService = new CacheService(redisProvider, _cacheSettingsOptions, MockLogger);
        var guidForTest = Guid.NewGuid();

        // Act & Assert
        await cacheService.DeleteAsync(guidForTest, MockCancellationToken);
    }

    [Fact]
    public async Task DeleteAsync_WithNullKey_ArgumentNullException()
    {
        var redisProvider = new RedisProvider(_cacheSettingsOptions, MockLogger);
        var cacheService = new CacheService(redisProvider, _cacheSettingsOptions, MockLogger);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await cacheService.DeleteAsync(null!, MockCancellationToken));
    }

    #endregion

    #region WrapCacheOperations

    [Fact]
    public async Task WrapCacheOperationsAsync_WithExistsCacheKey_ReturnCacheValueSuccess()
    {
        var redisProvider = new RedisProvider(_cacheSettingsOptions, MockLogger);
        var cacheService = new CacheService(redisProvider, _cacheSettingsOptions, MockLogger);
        var guidForTest = Guid.NewGuid();
        var cacheObject = FakeUsers.GetManyCorrectExistsModels().First();

        await cacheService.SetAsync(guidForTest, cacheObject, MockCancellationToken);

        // Act
        var cache = await cacheService.WrapCacheOperationsAsync(guidForTest,
            async () => await Task.Run(() => FakeUsers.GetManyCorrectExistsModels().Last()),
            MockCancellationToken);

        // Assert
        cache.Should().NotBeNull();
        cache!.Nickname.Should().BeEquivalentTo(cacheObject.Nickname);
        cache.Id.Should().Be(cacheObject.Id);
    }

    [Fact]
    public async Task WrapCacheOperationsAsync_WithNotExistsCacheKey_SetCacheAndReturnValueSuccess()
    {
        var redisProvider = new RedisProvider(_cacheSettingsOptions, MockLogger);
        var cacheService = new CacheService(redisProvider, _cacheSettingsOptions, MockLogger);
        var guidForTest = Guid.NewGuid();
        var cacheObject = FakeUsers.GetManyCorrectExistsModels().First();

        // Act
        await cacheService.WrapCacheOperationsAsync(guidForTest,
            async () => await Task.Run(() => cacheObject),
            MockCancellationToken);

        var cache = await cacheService.GetAsync<User>(guidForTest, MockCancellationToken);

        // Assert
        cache.Should().NotBeNull();
        cache!.Nickname.Should().BeEquivalentTo(cacheObject.Nickname);
        cache.Id.Should().Be(cacheObject.Id);
    }

    [Fact]
    public async Task WrapCacheOperationsAsync_WithNotExistsCacheKeyAndDelegateReturnsNull_ReturnNull()
    {
        var redisProvider = new RedisProvider(_cacheSettingsOptions, MockLogger);
        var cacheService = new CacheService(redisProvider, _cacheSettingsOptions, MockLogger);
        var guidForTest = Guid.NewGuid();

        // Act
        var gotValue = await cacheService.WrapCacheOperationsAsync(guidForTest,
            async () => await Task.Run(() => null! as User),
            MockCancellationToken);

        var cache = await cacheService.GetAsync<User>(guidForTest, MockCancellationToken);

        // Assert
        gotValue.Should().BeNull();
        cache.Should().BeNull();
    }

    [Fact]
    public async Task WrapCacheOperationsAsync_NullKey_ArgumentNullException()
    {
        var redisProvider = new RedisProvider(_cacheSettingsOptions, MockLogger);
        var cacheService = new CacheService(redisProvider, _cacheSettingsOptions, MockLogger);
        var cacheObject = FakeUsers.GetManyCorrectExistsModels().First();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await cacheService.WrapCacheOperationsAsync(null!,
            async () => await Task.Run(() => cacheObject),
            MockCancellationToken));
    }

    #endregion
}