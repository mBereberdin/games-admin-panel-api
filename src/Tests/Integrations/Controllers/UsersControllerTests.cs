namespace Tests.Integrations.Controllers;

using System.Net;

using Domain.DTOs.Users;

using FluentAssertions;

using Tests.Extensions;
using Tests.FakeData;
using Tests.Fixtures;

using WebApi.Controllers;

/// <summary>
/// Интеграционные тесты для <see cref="UsersController"/>.
/// </summary>
public class UsersControllerTests : IntegrationFixture
{
    /// <inheritdoc cref="UsersControllerTests"/>
    /// <param name="factory">Фабрика проекта Example.</param>
    public UsersControllerTests(WebApiApplicationFactory<Program> factory) : base(factory)
    {
    }

    #region Get

    [Fact]
    public async Task GetUserAsync_WithExistsUserId_Return200WithUserDto()
    {
        //Arrange
        var existsUser = FakeUsers.GetManyCorrectExistsModels().First();

        //Act
        var response = await Client.GetAsync($"users/{existsUser.Id}");
        var userDto = response.CreateInstance<UserDto>();

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        userDto.Id.Should().Be(existsUser.Id);
        userDto.Nickname.Should().Be(existsUser.Nickname);
        userDto.Email.Should().Be(existsUser.Email);
    }

    [Fact]
    public async Task GetUsersAsync_Return200WithUsersDtos()
    {
        //Arrange
        var existsUsersCount = FakeUsers.GetManyCorrectExistsModels().ToList().Count;

        //Act
        var response = await Client.GetAsync("users");
        var usersDtos = response.CreateInstance<List<UserDto>>();

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        usersDtos.Count.Should().Be(existsUsersCount);
    }

    [Fact]
    public async Task GetUserAsync_WithNotExistsUserId_Return404()
    {
        //Arrange
        var notExistsUserId = Guid.NewGuid();

        //Act
        var response = await Client.GetAsync($"users/{notExistsUserId}");

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region Add

    [Fact]
    public async Task AddUserAsync_WithCorrectNewUserDto_Return201()
    {
        //Arrange
        const string nickname = "testAdd1";
        const string email = "testAdd1@test.com";
        var createUserDto = new CreateUserDto(nickname, email);

        //Act
        var response = await Client.PostAsync("users", createUserDto.ToStringContent());
        var createdUser = response.CreateInstance<UserDto>();

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        createdUser.Id.Should().NotBeEmpty();
        createdUser.Nickname.Should().Be(nickname);
        createdUser.Email.Should().Be(email);
    }

    [Theory]
    [MemberData(nameof(FakeUsers.GetManyIncorrectCreateDtosForAddTest), MemberType = typeof(FakeUsers))]
    public async Task AddUserAsync_WithIncorrectNewUserDto_Return400(CreateUserDto createUserDto)
    {
        //Act
        var response = await Client.PostAsync("users", createUserDto.ToStringContent());

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region Update

    [Theory]
    [MemberData(nameof(FakeUsers.GetManyIncorrectUpdateDtosForUpdateTest), MemberType = typeof(FakeUsers))]
    public async Task UpdateUserAsync_WithIncorrectUpdateUserDto_Return400(UpdateUserDto updateUserDto)
    {
        //Act
        var response = await Client.PutAsync($"users/{Guid.Empty}", updateUserDto.ToStringContent());

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateUserAsync_WithCorrectPartUpdateUserDto_Return200WithUserDto()
    {
        var existsUser = FakeUsers.GetManyCorrectExistsModels().First();
        const string updatedEmailValue = "updateTest2@test.test";
        var updateUserDto = new UpdateUserDto(existsUser.Id)
        {
            Email = updatedEmailValue
        };

        //Act
        var response = await Client.PutAsync($"users/{updateUserDto.Id}", updateUserDto.ToStringContent());
        var updatedUserDto = response.CreateInstance<UserDto>();

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        updatedUserDto.Id.Should().Be(existsUser.Id);
        updatedUserDto.Email.Should().Be(updatedEmailValue);
        updatedUserDto.Nickname.Should().Be(existsUser.Nickname);
    }

    [Fact]
    public async Task UpdateUserAsync_WithCorrectFullUpdateUserDto_Return200WithUserDto()
    {
        const string updatedEmailValue = "updateTest1@test.test";
        const string updatedNicknameValue = "updateTest1";
        var updateUserDto = new UpdateUserDto(FakeUsers.GetFirstExistsUserId)
        {
            Email = updatedEmailValue,
            Nickname = updatedNicknameValue
        };

        //Act
        var response = await Client.PutAsync($"users/{updateUserDto.Id}", updateUserDto.ToStringContent());
        var updatedUserDto = response.CreateInstance<UserDto>();

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        updatedUserDto.Id.Should().Be(FakeUsers.GetFirstExistsUserId);
        updatedUserDto.Email.Should().Be(updatedEmailValue);
        updatedUserDto.Nickname.Should().Be(updatedNicknameValue);
    }

    [Fact]
    public async Task UpdateUserAsync_WithDifferentIdsInRouteAndDto_Return400()
    {
        var updateUserDto = new UpdateUserDto(FakeUsers.GetFirstExistsUserId);

        //Act
        var response = await Client.PutAsync($"users/{Guid.NewGuid()}", updateUserDto.ToStringContent());

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateUserAsync_WithEmptyIdInRoute_Return400()
    {
        var updateUserDto = new UpdateUserDto(FakeUsers.GetFirstExistsUserId);

        //Act
        var response = await Client.PutAsync($"users/{Guid.Empty}", updateUserDto.ToStringContent());

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region Add via update

    [Fact]
    public async Task UpdateUserAsync_WithCorrectForAddUpdateUserDto_Return201WithCreatedUserDto()
    {
        //Arrange
        const string nickname = "addViaUpdate1";
        const string email = "addViaUpdate1@test.test";
        var updateUserDto = new UpdateUserDto(Guid.Empty)
        {
            Email = email,
            Nickname = nickname
        };

        //Act
        var response = await Client.PutAsync($"users/{updateUserDto.Id}", updateUserDto.ToStringContent());
        var createdUser = response.CreateInstance<UserDto>();

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        createdUser.Id.Should().NotBeEmpty();
        createdUser.Nickname.Should().Be(nickname);
        createdUser.Email.Should().Be(email);
    }

    #endregion

    #region Delete

    [Fact]
    public async Task DeleteUserAsync_WithExistsUserId_Return204()
    {
        //Arrange
        var existsUserId = FakeUsers.GetFirstExistsUserId;

        //Act
        var response = await Client.DeleteAsync($"users/{existsUserId}");

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteUserAsync_WithNotExistsUserId_Return404()
    {
        //Arrange
        var notExistsUserId = Guid.NewGuid();

        //Act
        var response = await Client.DeleteAsync($"users/{notExistsUserId}");

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteUserAsync_WithEmptyUserId_Return400()
    {
        //Arrange
        var notExistsUserId = Guid.Empty;

        //Act
        var response = await Client.DeleteAsync($"users/{notExistsUserId}");

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion
}