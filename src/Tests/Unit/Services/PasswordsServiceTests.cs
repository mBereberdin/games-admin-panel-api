namespace Tests.Unit.Services;

using Domain.Models.Passwords;

using FluentAssertions;

using Infrastructure.Exceptions.CRUD;
using Infrastructure.Services.Implementations;

using Tests.FakeData;
using Tests.Fixtures;

/// <summary>
/// Тесты для <see cref="PasswordsService"/>.
/// </summary>
public class PasswordsServiceTests : ServiceFixture
{
    #region Add

    [Fact]
    public async Task AddPasswordAsync_WithCorrectNewPasswordModel_ReturnCreatedPasswordModelSuccess()
    {
        //Arrange
        var passwordsService =
            CreateServiceForTest((adminDbContext, logger) => new PasswordsService(adminDbContext, logger));
        var newPassword = new Password
        {
            Id = default,
            EncryptedValue = "123",
            UserId = FakeUsers.GetFirstExistsUserId
        };

        //Act
        var password = await passwordsService.CreatePasswordAsync(newPassword, MockCancellationToken);

        //Assert
        password.Id.Should().NotBeEmpty();
    }

    [Theory]
    [MemberData(nameof(FakePasswords.GetManyIncorrectModelsForAddTest), MemberType = typeof(FakePasswords))]
    public async Task AddPasswordAsync_WithInCorrectNewPasswordModel_ThrowCreateException(Password passwordToCreate)
    {
        //Arrange
        var passwordsService =
            CreateServiceForTest((adminDbContext, logger) => new PasswordsService(adminDbContext, logger));

        //Act & Assert
        await Assert.ThrowsAsync<CreateException>(async () =>
            await passwordsService.CreatePasswordAsync(passwordToCreate, MockCancellationToken));
    }

    [Fact]
    public async Task AddPasswordAsync_ForceThrowDbUpdateException_ThrowCreateException()
    {
        //Arrange
        var passwordsService = CreateServiceForTest((context, logger) => new PasswordsService(context, logger), true);
        var newPasswordModel = new Password
        {
            EncryptedValue = "1234",
            UserId = FakePasswords.GetFirstExistsPasswordId,
            Id = Guid.Empty
        };

        //Act & Assert
        await Assert.ThrowsAsync<CreateException>(async () =>
            await passwordsService.CreatePasswordAsync(newPasswordModel, MockCancellationToken));
    }

    #endregion

    #region Get

    [Fact]
    public async Task GetPasswordAsync_WithExistsPasswordId_ReturnPasswordModelSuccess()
    {
        //Arrange
        var passwordsService =
            CreateServiceForTest((adminDbContext, logger) => new PasswordsService(adminDbContext, logger));
        var existsPasswordId = FakePasswords.GetFirstExistsPasswordId;

        //Act
        var password = await passwordsService.GetPasswordAsync(existsPasswordId, MockCancellationToken);

        //Assert
        password.Should().NotBeNull();
        password!.Id.Should().Be(existsPasswordId);
    }

    [Fact]
    public async Task GetPasswordAsync_WithNotExistsPasswordId_ReturnNull()
    {
        //Arrange
        var passwordsService =
            CreateServiceForTest((adminDbContext, logger) => new PasswordsService(adminDbContext, logger));
        var notExistsPasswordId = Guid.NewGuid();

        //Act
        var password = await passwordsService.GetPasswordAsync(notExistsPasswordId, MockCancellationToken);

        //Assert
        password.Should().BeNull();
    }

    [Fact]
    public async Task GetAsync_WithExistsEncryptedValue_ReturnPassword()
    {
        //Arrange
        var passwordsService =
            CreateServiceForTest((adminDbContext, logger) => new PasswordsService(adminDbContext, logger));
        var existsPasswordEncryptedValue = FakePasswords.GetManyCorrectExistsModels().First().EncryptedValue;

        //Act
        var password = await passwordsService.GetAsync(existsPasswordEncryptedValue, MockCancellationToken);

        //Assert
        password.Should().NotBeNull();
        password!.EncryptedValue.Should().BeEquivalentTo(existsPasswordEncryptedValue);
    }

    [Fact]
    public async Task GetAsync_WithNotExistsEncryptedValue_ReturnNull()
    {
        //Arrange
        var passwordsService =
            CreateServiceForTest((adminDbContext, logger) => new PasswordsService(adminDbContext, logger));
        const string notExistsPasswordEncryptedValue = "notExistsPasswordEncryptedValue";

        //Act
        var password = await passwordsService.GetAsync(notExistsPasswordEncryptedValue, MockCancellationToken);

        //Assert
        password.Should().BeNull();
    }

    #endregion
}