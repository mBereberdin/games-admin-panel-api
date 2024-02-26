namespace Tests.Unit.Services;

using Domain.Models.Users;

using FluentAssertions;

using Infrastructure.Exceptions.CRUD;
using Infrastructure.Services.Implementations;

using Mapster;

using Tests.FakeData;
using Tests.Fixtures;

/// <summary>
/// Тесты для <see cref="UsersService"/>.
/// </summary>
public class UsersServiceTests : ServiceFixture
{
    #region Get

    [Fact]
    public async Task GetUsersAsync_ReturnUsersListSuccess()
    {
        //Arrange
        var userService = CreateServiceForTest((adminDbContext, logger) => new UsersService(adminDbContext, logger));

        //Act
        var usersList = (await userService.GetUsersAsync(MockCancellationToken)).ToList();

        //Assert
        usersList.Should().NotBeNull();
        usersList.Count.Should().BeGreaterOrEqualTo(0);
    }

    [Fact]
    public async Task GetUserAsync_WithExistsUserId_ReturnUserModelSuccess()
    {
        //Arrange
        var userService = CreateServiceForTest((adminDbContext, logger) => new UsersService(adminDbContext, logger));
        var existsUserId = FakeUsers.GetFirstExistsUserId;

        //Act
        var user = await userService.GetUserAsync(existsUserId, MockCancellationToken);

        //Assert
        user.Should().NotBeNull();
        user!.Id.Should().Be(existsUserId);
    }

    [Fact]
    public async Task GetUserAsync_WithNotExistsUserId_ReturnNull()
    {
        //Arrange
        var userService = CreateServiceForTest((adminDbContext, logger) => new UsersService(adminDbContext, logger));
        var notExistsUserId = Guid.NewGuid();

        //Act
        var user = await userService.GetUserAsync(notExistsUserId, MockCancellationToken);

        //Assert
        user.Should().BeNull();
    }

    #endregion

    #region Add

    [Fact]
    public async Task AddUserAsync_WithCorrectNewUserModel_ReturnCreatedUserModelSuccess()
    {
        //Arrange
        var userService = CreateServiceForTest((adminDbContext, logger) => new UsersService(adminDbContext, logger));

        var newUser = new User
        {
            Email = "addUser@test.com",
            Nickname = "addUser",
            Id = default
        };

        //Act
        var user = await userService.CreateUserAsync(newUser, MockCancellationToken);

        //Assert
        user.Id.Should().NotBeEmpty();
    }

    [Theory]
    [MemberData(nameof(FakeUsers.GetManyIncorrectModelsForAddTest), MemberType = typeof(FakeUsers))]
    public async Task AddUserAsync_WithInCorrectNewUserModel_ThrowCreateException(User userToCreate)
    {
        //Arrange
        var userService = CreateServiceForTest((adminDbContext, logger) => new UsersService(adminDbContext, logger));

        //Act & Assert
        await Assert.ThrowsAsync<CreateException>(async () =>
            await userService.CreateUserAsync(userToCreate, MockCancellationToken));
    }

    [Fact]
    public async Task AddUserAsync_ForceThrowDbUpdateException_ThrowDbUpdateException()
    {
        //Arrange
        var userService =
            CreateServiceForTest((adminDbContext, logger) => new UsersService(adminDbContext, logger), true);
        var newUser = new User
        {
            Email = "addUserForceException@test.com",
            Nickname = "addUserForce",
            Id = default
        };

        //Act & Assert
        await Assert.ThrowsAsync<CreateException>(async () =>
            await userService.CreateUserAsync(newUser, MockCancellationToken));
    }

    #endregion

    #region Delete

    [Fact]
    public async Task TryDeleteUserAsync_WithExistsUserId_ReturnTrueSuccess()
    {
        //Arrange
        var userService = CreateServiceForTest((adminDbContext, logger) => new UsersService(adminDbContext, logger));
        var existsUserId = FakeUsers.GetFirstExistsUserId;

        //Act
        var isDeleted = await userService.TryDeleteUserAsync(existsUserId, MockCancellationToken);

        //Assert
        isDeleted.Should().BeTrue();
    }

    [Fact]
    public async Task TryDeleteUserAsync_WithNotExistsUserId_ReturnFalseSuccess()
    {
        //Arrange
        var userService = CreateServiceForTest((adminDbContext, logger) => new UsersService(adminDbContext, logger));
        var notExistsUserId = Guid.NewGuid();

        //Act
        var isDeleted = await userService.TryDeleteUserAsync(notExistsUserId, MockCancellationToken);

        //Assert
        isDeleted.Should().BeFalse();
    }

    [Fact]
    public async Task TryDeleteUserAsync_ForceThrowDbUpdateException_ThrowDeleteException()
    {
        //Arrange
        var userService = CreateServiceForTest((context, logger) => new UsersService(context, logger), true);
        var existsUserId = FakeUsers.GetFirstExistsUserId;

        //Act & Assert
        await Assert.ThrowsAsync<DeleteException>(async () =>
            await userService.TryDeleteUserAsync(existsUserId, MockCancellationToken));
    }

    #endregion

    #region Update

    [Fact]
    public async Task UpdateUserAsync_WithCorrectUpdatedUserModel_ReturnUpdatedModelSuccess()
    {
        //Arrange
        var userService = CreateServiceForTest((adminDbContext, logger) => new UsersService(adminDbContext, logger));
        var existsUser = FakeUsers.GetManyCorrectExistsModels().First();
        var updateUser = new User
        {
            Email = "updatedEmail@.test",
            Nickname = "updatedNickname",
            Id = existsUser.Id
        };

        //Act
        updateUser.Adapt(existsUser);
        await userService.UpdateUserAsync(existsUser.Id, existsUser, MockCancellationToken);
        var updatedUser = await userService.GetUserAsync(existsUser.Id, MockCancellationToken);

        //Assert
        updatedUser!.Email.Should().Be(updateUser.Email);
        updatedUser.Nickname.Should().Be(updateUser.Nickname);
    }

    [Fact]
    public async Task UpdateUserAsync_WithNotExistsUserId_ReturnNotFoundException()
    {
        //Arrange
        var userService = CreateServiceForTest((adminDbContext, logger) => new UsersService(adminDbContext, logger));
        var existsUser = FakeUsers.GetManyCorrectExistsModels().First();
        var updateUser = new User
        {
            Email = "updatedEmail1@.test",
            Nickname = "updatedNickname",
            Id = existsUser.Id
        };

        updateUser.Adapt(existsUser);

        //Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(async () =>
            await userService.UpdateUserAsync(Guid.NewGuid(), existsUser, MockCancellationToken));
    }

    [Theory]
    [MemberData(nameof(FakeUsers.GetManyIncorrectModelsForUpdateTest), MemberType = typeof(FakeUsers))]
    public async Task UpdateUserAsync_WithInCorrectUpdatedUserModel_ThrowUpdateException(User userToUpdate)
    {
        //Arrange
        var userService = CreateServiceForTest((adminDbContext, logger) => new UsersService(adminDbContext, logger));
        var existsUser = FakeUsers.GetManyCorrectExistsModels().First();

        userToUpdate.Adapt(existsUser);

        //Act & Assert
        await Assert.ThrowsAsync<UpdateException>(async () =>
            await userService.UpdateUserAsync(existsUser.Id, existsUser, MockCancellationToken));
    }

    [Fact]
    public async Task UpdateUserAsync_ForceThrowDbUpdateException_ThrowUpdateException()
    {
        //Arrange
        var userService = CreateServiceForTest((context, logger) => new UsersService(context, logger), true);
        var existsUser = FakeUsers.GetManyCorrectExistsModels().First();
        var updateUser = new User
        {
            Email = "updatedEmail@.test",
            Nickname = "updatedNickname",
            Id = existsUser.Id
        };

        //Act & Assert
        await Assert.ThrowsAsync<UpdateException>(async () =>
            await userService.UpdateUserAsync(updateUser.Id, updateUser, MockCancellationToken));
    }

    #endregion
}