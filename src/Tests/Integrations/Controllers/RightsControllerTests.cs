namespace Tests.Integrations.Controllers;

using System.Net;

using Domain.DTOs.Games;
using Domain.DTOs.Rights;

using FluentAssertions;

using Tests.Extensions;
using Tests.FakeData;
using Tests.Fixtures;

using WebApi.Controllers;

/// <summary>
/// Интеграционные тесты для <see cref="RightsController"/>.
/// </summary>
public class RightsControllerTests : IntegrationFixture
{
    /// <inheritdoc cref="RightsControllerTests"/>
    /// <param name="factory">Фабрика проекта WebApi.</param>
    public RightsControllerTests(WebApiApplicationFactory<Program> factory) : base(factory)
    {
    }

    #region Add

    [Fact]
    public async Task RegisterRightsAsync_WithCorrectRightsRegistrationDto_ReturnOk()
    {
        //Arrange
        var game = new ExternalGameDto("TestGame");
        var rights = new List<ExternalCreateRightDto>
        {
            new("TestRight", "Тестовое описание права."),
            new("TestRight1", "Тестовое описание права1."),
            new("TestRight2", "Тестовое описание права2.")
        };
        var rightsToRegisterDto = new RegisterRightsDto(rights, game);

        //Act
        var response = await Client.PostAsync("rights/register", rightsToRegisterDto.ToStringContent());

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task RegisterRightsAsync_WithCorrectUpdatedGameInRightsRegistrationDto_ReturnOk()
    {
        //Arrange
        var gameDto = new ExternalGameDto(FakeGames.GetFirstExistsGameName, "updatedDescription1");
        var rights = new List<ExternalCreateRightDto>
        {
            new("TestRight", "Тестовое описание права."),
            new("TestRight1", "Тестовое описание права1."),
            new("TestRight2", "Тестовое описание права2.")
        };
        var rightsToRegisterDto = new RegisterRightsDto(rights, gameDto);

        //Act
        var response = await Client.PostAsync("rights/register", rightsToRegisterDto.ToStringContent());

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task RegisterRightsAsync_WithIncorrectRights_ReturnBadRequest()
    {
        //Arrange
        var game = new ExternalGameDto("TestGame");
        var rights = new List<ExternalCreateRightDto>
        {
            new(null!, null!),
            new("TestRight1", null!),
            new(null!, "Тестовое описание права2.")
        };
        var rightsToRegisterDto = new RegisterRightsDto(rights, game);

        //Act
        var response = await Client.PostAsync("rights/register", rightsToRegisterDto.ToStringContent());

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task RegisterRightsAsync_WithIncorrectGame_ReturnBadRequest()
    {
        //Arrange
        var game = new ExternalGameDto(null!);
        var rights = new List<ExternalCreateRightDto>
        {
            new("TestRight", "Тестовое описание права.")
        };
        var rightsToRegisterDto = new RegisterRightsDto(rights, game);

        //Act
        var response = await Client.PostAsync("rights/register", rightsToRegisterDto.ToStringContent());

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion
}