namespace Tests.Integrations.Controllers;

using System.Net;

using Domain.DTOs.Passwords;

using FluentAssertions;

using Tests.Extensions;
using Tests.FakeData;
using Tests.Fixtures;

using WebApi.Controllers;

/// <summary>
/// Интеграционные тесты для <see cref="PasswordsController"/>.
/// </summary>
public class PasswordsControllerTests : IntegrationFixture
{
    /// <inheritdoc cref="UsersControllerTests"/>
    /// <param name="factory">Фабрика проекта WebApi.</param>
    public PasswordsControllerTests(WebApiApplicationFactory<Program> factory) : base(factory)
    {
    }

    #region Get

    [Fact]
    public async Task GetPasswordAsync_WithExistsPasswordId_Return200WithPasswordDto()
    {
        //Arrange
        var existsUserId = FakePasswords.GetFirstExistsPasswordId;

        //Act
        var response = await Client.GetAsync($"passwords/{existsUserId}");

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetPasswordAsync_WithNotExistsPasswordId_Return404()
    {
        //Arrange
        var notExistsPasswordId = Guid.NewGuid();

        //Act
        var response = await Client.GetAsync($"passwords/{notExistsPasswordId}");

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region Add

    [Fact]
    public async Task AddPasswordAsync_WithCorrectNewPasswordDto_Return201()
    {
        //Arrange
        var existsUserId = FakeUsers.GetFirstExistsUserId;
        const string encryptedPasswordValue = "1234";
        var createPasswordDto = new CreatePasswordDto(encryptedPasswordValue, existsUserId);

        //Act
        var response = await Client.PostAsync("passwords", createPasswordDto.ToStringContent());
        var createdPassword = response.CreateInstance<PasswordDto>();

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        createdPassword.Id.Should().NotBeEmpty();
        createdPassword.EncryptedValue.Should().Be(encryptedPasswordValue);
        createdPassword.UserId.Should().Be(existsUserId);
    }

    [Theory]
    [MemberData(nameof(FakePasswords.GetManyIncorrectCreateDtosForAddTest), MemberType = typeof(FakePasswords))]
    public async Task AddPasswordAsync_WithIncorrectNewPasswordDto_Return400(CreatePasswordDto createPasswordDto)
    {
        //Act
        var response = await Client.PostAsync("passwords", createPasswordDto.ToStringContent());

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion
}