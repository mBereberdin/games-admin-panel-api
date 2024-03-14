namespace Tests.Unit.Services;

using Domain.Models.Games;

using FluentAssertions;

using Infrastructure.Exceptions.CRUD;
using Infrastructure.Services.Implementations;

using Mapster;

using Tests.FakeData;
using Tests.Fixtures;

/// <summary>
/// Тесты для <see cref="GamesService"/>.
/// </summary>
public class GamesServiceTests : ServiceFixture
{
    #region Get

    [Fact]
    public async Task GetAsync_WithExistsGameName_ReturnGameModelSuccess()
    {
        //Arrange
        var gamesService =
            CreateServiceForTest((adminDbContext, logger) => new GamesService(adminDbContext, logger));
        var existsGameName = FakeGames.GetFirstExistsGameName;

        //Act
        var game = await gamesService.GetAsync(existsGameName, MockCancellationToken);

        //Assert
        game.Should().NotBeNull();
        game!.Name.Should().Be(existsGameName);
    }

    [Fact]
    public async Task GetAsync_WithNotExistsGameName_ReturnNull()
    {
        //Arrange
        var gamesService =
            CreateServiceForTest((adminDbContext, logger) => new GamesService(adminDbContext, logger));
        const string notExistsGameName = "notExistsGameName1";

        //Act
        var game = await gamesService.GetAsync(notExistsGameName, MockCancellationToken);

        //Assert
        game.Should().BeNull();
    }

    [Fact]
    public async Task GetAsync_WithExistsGameId_ReturnGameModelSuccess()
    {
        //Arrange
        var gamesService =
            CreateServiceForTest((adminDbContext, logger) => new GamesService(adminDbContext, logger));
        var existsGameId = FakeGames.GetFirstExistsGameId;

        //Act
        var game = await gamesService.GetAsync(existsGameId, MockCancellationToken);

        //Assert
        game.Should().NotBeNull();
        game!.Id.Should().Be(existsGameId);
    }

    [Fact]
    public async Task GetAsync_WithNotExistsGameId_ReturnNull()
    {
        //Arrange
        var gamesService =
            CreateServiceForTest((adminDbContext, logger) => new GamesService(adminDbContext, logger));
        var notExistsGameId = Guid.NewGuid();

        //Act
        var game = await gamesService.GetAsync(notExistsGameId, MockCancellationToken);

        //Assert
        game.Should().BeNull();
    }

    #endregion

    #region Add

    [Fact]
    public async Task AddAsync_WithCorrectNewGameModel_ReturnCreatedGameModelSuccess()
    {
        //Arrange
        var gamesService =
            CreateServiceForTest((adminDbContext, logger) => new GamesService(adminDbContext, logger));
        var newGame = new Game
        {
            Id = default,
            Name = "newGame1",
            Description = null
        };

        //Act
        var game = await gamesService.AddAsync(newGame, MockCancellationToken);

        //Assert
        game.Id.Should().NotBeEmpty();
    }

    [Theory]
    [MemberData(nameof(FakeGames.GetManyIncorrectModelsForAddTest), MemberType = typeof(FakeGames))]
    public async Task AddAsync_WithInCorrectNewGameModel_ThrowArgumentException(Game gameToCreate)
    {
        //Arrange
        var gamesService =
            CreateServiceForTest((adminDbContext, logger) => new GamesService(adminDbContext, logger));

        //Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(async () =>
            await gamesService.AddAsync(gameToCreate, MockCancellationToken));
    }

    [Fact]
    public async Task AddAsync_ForceThrowDbUpdateException_ThrowCreateException()
    {
        //Arrange
        var gamesService = CreateServiceForTest((context, logger) => new GamesService(context, logger), true);
        var newGame = new Game
        {
            Id = default,
            Name = "newGame2",
            Description = null
        };

        //Act & Assert
        await Assert.ThrowsAsync<CreateException>(async () =>
            await gamesService.AddAsync(newGame, MockCancellationToken));
    }

    #endregion

    #region Update

    [Fact]
    public async Task UpdateAsync_WithCorrectUpdateGameModel_ReturnUpdatedModel()
    {
        //Arrange
        var gamesService = CreateServiceForTest((adminDbContext, logger) => new GamesService(adminDbContext, logger));
        var existsGame = FakeGames.GetManyCorrectExistsModels().First();
        var updateGame = new Game
        {
            Description = "updatedGameDescription1",
            Name = "updatedGameName1",
            Id = existsGame.Id
        };

        //Act
        updateGame.Adapt(existsGame);
        var updatedGame = await gamesService.UpdateAsync(existsGame.Id, existsGame, MockCancellationToken);

        //Assert
        updatedGame.Name.Should().Be(updateGame.Name);
        updatedGame.Description.Should().Be(updateGame.Description);
    }

    [Theory]
    [MemberData(nameof(FakeGames.GetManyIncorrectModelsForAddTest), MemberType = typeof(FakeGames))]
    public async Task UpdateAsync_WithIncorrectGameModel_ArgumentException(Game gameToUpdate)
    {
        //Arrange
        var gamesService = CreateServiceForTest((adminDbContext, logger) => new GamesService(adminDbContext, logger));
        var existsGame = FakeGames.GetManyCorrectExistsModels().First();

        gameToUpdate.Adapt(existsGame);

        //Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(async () =>
            await gamesService.UpdateAsync(existsGame.Id, existsGame, MockCancellationToken));
    }

    [Fact]
    public async Task UpdateAsync_WithNotExistsGameId_NotFoundException()
    {
        //Arrange
        var gameService = CreateServiceForTest((adminDbContext, logger) => new GamesService(adminDbContext, logger));
        var existsGame = FakeGames.GetManyCorrectExistsModels().First();
        var updateGame = new Game
        {
            Description = "updatedGameDescription2",
            Name = "updatedGameName2",
            Id = existsGame.Id
        };

        updateGame.Adapt(existsGame);

        //Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(async () =>
            await gameService.UpdateAsync(Guid.NewGuid(), existsGame, MockCancellationToken));
    }

    [Fact]
    public async Task UpdateAsync_ForceThrowDbUpdateException_ThrowUpdateException()
    {
        //Arrange
        var gameService = CreateServiceForTest((context, logger) => new GamesService(context, logger), true);
        var existsGame = FakeGames.GetManyCorrectExistsModels().First();
        var updateGame = new Game
        {
            Description = "updatedGameDescription3",
            Name = "updatedGameName3",
            Id = existsGame.Id
        };

        //Act & Assert
        await Assert.ThrowsAsync<UpdateException>(async () =>
            await gameService.UpdateAsync(updateGame.Id, updateGame, MockCancellationToken));
    }

    #endregion
}