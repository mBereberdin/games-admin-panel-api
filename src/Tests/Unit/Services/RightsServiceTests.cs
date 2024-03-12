namespace Tests.Unit.Services;

using Domain.Models.Rights;

using FluentAssertions;

using Infrastructure.Comparers;
using Infrastructure.Exceptions.CRUD;
using Infrastructure.Services.Implementations;

using Tests.FakeData;
using Tests.Fixtures;

/// <summary>
/// Тесты для <see cref="RightsService"/>.
/// </summary>
public class RightsServiceTests : ServiceFixture
{
    #region Get

    [Fact]
    public async Task GetAsync_WithOneExistsRightName_ReturnRightListWithOneModelSuccess()
    {
        //Arrange
        var comparer = new RightsSortComparer();
        var gamesService = CreateServiceForTest((adminDbContext, logger) => new GamesService(adminDbContext, logger));
        var rightsService =
            CreateServiceForTest((adminDbContext, logger) =>
                new RightsService(gamesService, adminDbContext, comparer, logger));
        var existsRightName = FakeRights.GetFirstExistsRightName;

        //Act
        var rightsList = await rightsService.GetAsync(MockCancellationToken, existsRightName);

        //Assert
        rightsList.Should().NotBeNullOrEmpty();
        rightsList.Should().HaveCount(1);
        rightsList!.First().Name.Should().Be(existsRightName);
    }

    [Fact]
    public async Task GetAsync_WithSeveralExistsRightNames_ReturnRightListWithSeveralModelsSuccess()
    {
        //Arrange
        var comparer = new RightsSortComparer();
        var gamesService = CreateServiceForTest((adminDbContext, logger) => new GamesService(adminDbContext, logger));
        var rightsService =
            CreateServiceForTest((adminDbContext, logger) =>
                new RightsService(gamesService, adminDbContext, comparer, logger));
        var existsRightNames = FakeRights.GetManyCorrectExistsModels().Select(right => right.Name).ToArray();

        //Act
        var rightsList = await rightsService.GetAsync(MockCancellationToken, existsRightNames);

        //Assert
        rightsList.Should().NotBeNullOrEmpty();
        rightsList.Should().HaveCount(existsRightNames.Length);
    }

    [Fact]
    public async Task GetAsync_WithNotExistsRightName_ReturnNull()
    {
        //Arrange
        var comparer = new RightsSortComparer();
        var gamesService = CreateServiceForTest((adminDbContext, logger) => new GamesService(adminDbContext, logger));
        var rightsService =
            CreateServiceForTest((adminDbContext, logger) =>
                new RightsService(gamesService, adminDbContext, comparer, logger));
        const string notExistsRightName = "notExistsRightName1";

        //Act
        var rightsList = await rightsService.GetAsync(MockCancellationToken, notExistsRightName);

        //Assert
        rightsList.Should().BeNull();
    }

    [Fact]
    public async Task GetAsync_WithEmptyNames_ArgumentNullException()
    {
        //Arrange
        var comparer = new RightsSortComparer();
        var gamesService = CreateServiceForTest((adminDbContext, logger) => new GamesService(adminDbContext, logger));
        var rightsService =
            CreateServiceForTest((adminDbContext, logger) =>
                new RightsService(gamesService, adminDbContext, comparer, logger));

        //Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await rightsService.GetAsync(MockCancellationToken, null!));
    }

    [Fact]
    public async Task GetAllAsync_WithExistsGameName_ReturnRightModelsListSuccess()
    {
        //Arrange
        var comparer = new RightsSortComparer();
        var gamesService = CreateServiceForTest((adminDbContext, logger) => new GamesService(adminDbContext, logger));
        var rightsService =
            CreateServiceForTest((adminDbContext, logger) =>
                new RightsService(gamesService, adminDbContext, comparer, logger));
        var existsGameName = FakeGames.GetFirstExistsGameName;

        //Act
        var right = await rightsService.GetAllAsync(existsGameName, MockCancellationToken);

        //Assert
        right.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task GetAllAsync_WithEmptyName_ArgumentNullException()
    {
        //Arrange
        var comparer = new RightsSortComparer();
        var gamesService = CreateServiceForTest((adminDbContext, logger) => new GamesService(adminDbContext, logger));
        var rightsService =
            CreateServiceForTest((adminDbContext, logger) =>
                new RightsService(gamesService, adminDbContext, comparer, logger));

        //Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await rightsService.GetAllAsync(string.Empty, MockCancellationToken));
    }

    #endregion

    #region Add

    [Fact]
    public async Task RegisterAsync_WithCorrectParams_Success()
    {
        //Arrange
        var comparer = new RightsSortComparer();
        var gamesService = CreateServiceForTest((adminDbContext, logger) => new GamesService(adminDbContext, logger));
        var rightsService =
            CreateServiceForTest((adminDbContext, logger) =>
                new RightsService(gamesService, adminDbContext, comparer, logger));

        var secondExistsRight = FakeRights.GetManyCorrectExistsModels().ElementAt(1);
        var thirdExistsRight = FakeRights.GetManyCorrectExistsModels().ElementAt(2);
        const string updatedDescription = "Обновленное описание права 3";
        thirdExistsRight.Description = updatedDescription;

        const string name = "newRight1";
        const string description = "Новое право 1";
        var newRight = new Right
        {
            Id = Guid.Empty,
            Name = name,
            Description = description,
            GameId = FakeGames.GetFirstExistsGameId
        };
        var rightsToRegister = new List<Right>
        {
            newRight,
            secondExistsRight,
            thirdExistsRight
        };

        //Act
        await rightsService.RegisterAsync(rightsToRegister, FakeGames.GetFirstExistsGameName, MockCancellationToken);
        var rights = await rightsService.GetAllAsync(FakeGames.GetFirstExistsGameName, MockCancellationToken);

        //Assert
        rights.Should().NotBeNullOrEmpty();
        rights.Should().HaveCount(3); // на регистрацию отправлялись 3 права, 3 права и должны быть после регистрации.
        // Добавлен новый элемент.
        rights.Any(right => right.Name.Equals(name, StringComparison.OrdinalIgnoreCase)).Should().BeTrue();
        // Обновлен существующий.
        rights.Any(right => right.Description.Equals(updatedDescription, StringComparison.OrdinalIgnoreCase)).Should()
            .BeTrue();
        // Право без изменений - осталось.
        rights.Any(right => right.Name.Equals(secondExistsRight.Name, StringComparison.OrdinalIgnoreCase)).Should()
            .BeTrue();
        // Право, которое не пришло на регистрацию (тестовое право 1) - должно быть удалено.
        rights.Any(right => right.Name.Equals(FakeRights.GetFirstExistsRightName, StringComparison.OrdinalIgnoreCase))
            .Should().BeFalse();
    }

    [Fact]
    public async Task RegisterAsync_WithIncorrectRightsToRegister_ThrowArgumentException()
    {
        //Arrange
        var comparer = new RightsSortComparer();
        var gamesService = CreateServiceForTest((adminDbContext, logger) => new GamesService(adminDbContext, logger));
        var rightsService =
            CreateServiceForTest((adminDbContext, logger) =>
                new RightsService(gamesService, adminDbContext, comparer, logger));
        var incorrectRights = FakeRights.GetManyIncorrectNewModels().ToList();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(async () =>
            await rightsService.RegisterAsync(incorrectRights, FakeGames.GetFirstExistsGameName,
                MockCancellationToken));
    }

    [Fact]
    public async Task RegisterAsync_WithNotExistsGameName_ThrowNotFoundException()
    {
        //Arrange
        var comparer = new RightsSortComparer();
        var gamesService = CreateServiceForTest((adminDbContext, logger) => new GamesService(adminDbContext, logger));
        var rightsService =
            CreateServiceForTest((adminDbContext, logger) =>
                new RightsService(gamesService, adminDbContext, comparer, logger));
        var rights = FakeRights.GetManyCorrectExistsModels().ToList();

        //Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(async () =>
            await rightsService.RegisterAsync(rights, "notExistsGameName1", MockCancellationToken));
    }

    [Fact]
    public async Task AddRangeAsync_WithCorrectRights_Success()
    {
        //Arrange
        var comparer = new RightsSortComparer();
        var gamesService = CreateServiceForTest((adminDbContext, logger) => new GamesService(adminDbContext, logger));
        var rightsService =
            CreateServiceForTest((adminDbContext, logger) =>
                new RightsService(gamesService, adminDbContext, comparer, logger));

        const string newRightName = "addRangeTest1";
        var newRights = new List<Right>
        {
            new()
            {
                Id = Guid.Empty,
                Name = newRightName,
                Description = "Тест массового добавления 1.",
                GameId = FakeGames.GetFirstExistsGameId
            }
        };

        //Act
        await rightsService.AddRangeAsync(newRights, MockCancellationToken);
        var rights = await rightsService.GetAllAsync(FakeGames.GetFirstExistsGameName, MockCancellationToken);

        //Assert
        rights.Any(right => right.Name.Equals(newRightName, StringComparison.OrdinalIgnoreCase)).Should().BeTrue();
    }

    [Fact]
    public async Task AddRangeAsync_WithIncorrectRights_ArgumentException()
    {
        //Arrange
        var comparer = new RightsSortComparer();
        var gamesService = CreateServiceForTest((adminDbContext, logger) => new GamesService(adminDbContext, logger));
        var rightsService =
            CreateServiceForTest((adminDbContext, logger) =>
                new RightsService(gamesService, adminDbContext, comparer, logger));
        var incorrectRights = FakeRights.GetManyIncorrectNewModels().ToList();

        //Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(async () =>
            await rightsService.AddRangeAsync(incorrectRights, MockCancellationToken));
    }

    [Fact]
    public async Task AddRangeAsync_ForceDbUpdateException_CreateException()
    {
        //Arrange
        var comparer = new RightsSortComparer();
        var gamesService = CreateServiceForTest((adminDbContext, logger) => new GamesService(adminDbContext, logger));
        var rightsService =
            CreateServiceForTest((adminDbContext, logger) =>
                new RightsService(gamesService, adminDbContext, comparer, logger), true);
        var rights = FakeRights.GetManyCorrectExistsModels().ToList();

        //Act & Assert
        await Assert.ThrowsAsync<CreateException>(async () =>
            await rightsService.AddRangeAsync(rights, MockCancellationToken));
    }

    #endregion

    #region Delete

    [Fact]
    public async Task TryDeleteRangeAsync_WithExistsRights_Success()
    {
        //Arrange
        var comparer = new RightsSortComparer();
        var gamesService = CreateServiceForTest((adminDbContext, logger) => new GamesService(adminDbContext, logger));
        var rightsService =
            CreateServiceForTest((adminDbContext, logger) =>
                new RightsService(gamesService, adminDbContext, comparer, logger));
        var rights = FakeRights.GetManyCorrectExistsModels().ToList();

        //Act
        var areDeleted = await rightsService.TryDeleteRangeAsync(rights, MockCancellationToken);

        //Assert
        areDeleted.Should().BeTrue();
    }

    [Fact]
    public async Task TryDeleteRangeAsync_WithNotExistsRights_Success()
    {
        //Arrange
        var comparer = new RightsSortComparer();
        var gamesService = CreateServiceForTest((adminDbContext, logger) => new GamesService(adminDbContext, logger));
        var rightsService =
            CreateServiceForTest((adminDbContext, logger) =>
                new RightsService(gamesService, adminDbContext, comparer, logger));
        var rights = new List<Right>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Name = "testDelete",
                Description = "Проверка удаления.",
                GameId = Guid.NewGuid()
            }
        };

        //Act
        var areDeleted = await rightsService.TryDeleteRangeAsync(rights, MockCancellationToken);

        //Assert
        areDeleted.Should().BeFalse();
    }

    [Fact]
    public async Task TryDeleteRangeAsync_PartOfRightsNotExists_ReturnTrue()
    {
        //Arrange
        var comparer = new RightsSortComparer();
        var gamesService = CreateServiceForTest((adminDbContext, logger) => new GamesService(adminDbContext, logger));
        var rightsService =
            CreateServiceForTest((adminDbContext, logger) =>
                new RightsService(gamesService, adminDbContext, comparer, logger));
        var rights = FakeRights.GetManyCorrectExistsModels().ToList();
        rights.Add(new Right
        {
            Id = Guid.NewGuid(),
            Name = "testDelete",
            Description = "Проверка удаления.",
            GameId = Guid.NewGuid()
        });

        //Act
        var areDeleted = await rightsService.TryDeleteRangeAsync(rights, MockCancellationToken);

        //Assert
        areDeleted.Should().BeTrue();
    }

    [Fact]
    public async Task TryDeleteRangeAsync_ForceDbUpdateException_DeleteException()
    {
        //Arrange
        var comparer = new RightsSortComparer();
        var gamesService = CreateServiceForTest((adminDbContext, logger) => new GamesService(adminDbContext, logger));
        var rightsService =
            CreateServiceForTest((adminDbContext, logger) =>
                new RightsService(gamesService, adminDbContext, comparer, logger), true);
        var rights = FakeRights.GetManyCorrectExistsModels().ToList();

        //Act & Assert
        await Assert.ThrowsAsync<DeleteException>(async () =>
            await rightsService.TryDeleteRangeAsync(rights, MockCancellationToken));
    }

    #endregion

    #region Update

    [Fact]
    public async Task UpdateRangeAsync_WithCorrectRights_Success()
    {
        //Arrange
        var comparer = new RightsSortComparer();
        var gamesService = CreateServiceForTest((adminDbContext, logger) => new GamesService(adminDbContext, logger));
        var rightsService =
            CreateServiceForTest((adminDbContext, logger) =>
                new RightsService(gamesService, adminDbContext, comparer, logger));

        const string newRightDescription = "Тест массового обновления 1.";

        //Act
        var rights = await rightsService.GetAsync(MockCancellationToken, FakeRights.GetFirstExistsRightName);
        rights!.Single().Description = newRightDescription;
        await rightsService.UpdateRangeAsync(rights!, MockCancellationToken);
        var gotRights = await rightsService.GetAllAsync(FakeGames.GetFirstExistsGameName, MockCancellationToken);

        //Assert
        gotRights.Any(right => right.Description.Equals(newRightDescription, StringComparison.OrdinalIgnoreCase))
            .Should().BeTrue();
    }

    [Fact]
    public async Task UpdateRangeAsync_WithIncorrectRights_ArgumentException()
    {
        //Arrange
        var comparer = new RightsSortComparer();
        var gamesService = CreateServiceForTest((adminDbContext, logger) => new GamesService(adminDbContext, logger));
        var rightsService =
            CreateServiceForTest((adminDbContext, logger) =>
                new RightsService(gamesService, adminDbContext, comparer, logger));
        var incorrectRights = FakeRights.GetManyIncorrectNewModels().ToList();
        incorrectRights.ForEach(right => right.Id = FakeRights.GetFirstExistsRightId);

        //Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(async () =>
            await rightsService.UpdateRangeAsync(incorrectRights, MockCancellationToken));
    }

    [Fact]
    public async Task UpdateRangeAsync_WithNotExistsRight_NotFoundException()
    {
        //Arrange
        var comparer = new RightsSortComparer();
        var gamesService = CreateServiceForTest((adminDbContext, logger) => new GamesService(adminDbContext, logger));
        var rightsService =
            CreateServiceForTest((adminDbContext, logger) =>
                new RightsService(gamesService, adminDbContext, comparer, logger));
        var incorrectRights = new List<Right>
        {
            new()
            {
                Id = FakeRights.GetFirstExistsRightId,
                Name = "notExistsRightName",
                Description = "Описание",
                GameId = FakeGames.GetFirstExistsGameId
            }
        };

        //Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(async () =>
            await rightsService.UpdateRangeAsync(incorrectRights, MockCancellationToken));
    }

    [Fact]
    public async Task UpdateRangeAsync_ForceDbUpdateException_UpdateException()
    {
        //Arrange
        var comparer = new RightsSortComparer();
        var gamesService = CreateServiceForTest((adminDbContext, logger) => new GamesService(adminDbContext, logger));
        var rightsService =
            CreateServiceForTest((adminDbContext, logger) =>
                new RightsService(gamesService, adminDbContext, comparer, logger), true);
        var rights = FakeRights.GetManyCorrectExistsModels().ToList();

        //Act & Assert
        await Assert.ThrowsAsync<UpdateException>(async () =>
            await rightsService.UpdateRangeAsync(rights, MockCancellationToken));
    }

    #endregion

    #region Sort

    [Fact]
    public async Task SortGamesRightsAsync_WithCorrectParams_Success()
    {
        //Arrange
        var comparer = new RightsSortComparer();
        var gamesService = CreateServiceForTest((adminDbContext, logger) => new GamesService(adminDbContext, logger));
        var rightsService =
            CreateServiceForTest((adminDbContext, logger) =>
                new RightsService(gamesService, adminDbContext, comparer, logger));

        var firstExistsRightWillDeleted = FakeRights.GetManyCorrectExistsModels().First();
        firstExistsRightWillDeleted.Game = FakeGames.GetManyCorrectExistsModels().First();
        var secondExistsRight = FakeRights.GetManyCorrectExistsModels().ElementAt(1);
        var thirdExistsRight = FakeRights.GetManyCorrectExistsModels().ElementAt(2);
        const string updatedDescription = "Обновленное описание права 3";
        thirdExistsRight.Description = updatedDescription;

        const string name = "newRight1";
        const string description = "Новое право 1";
        var newRight = new Right
        {
            Id = Guid.Empty,
            Name = name,
            Description = description,
            GameId = FakeGames.GetFirstExistsGameId
        };
        var rightsToSort = new List<Right>
        {
            newRight,
            secondExistsRight,
            thirdExistsRight
        };

        //Act
        var sortedRights = await rightsService.SortGamesRightsAsync(FakeGames.GetFirstExistsGameName, rightsToSort,
            MockCancellationToken);

        //Assert
        sortedRights.RightsToCreate.Should().NotBeNull();
        sortedRights.RightsToUpdate.Should().NotBeNull();
        sortedRights.RightsToDelete.Should().NotBeNull();

        sortedRights.RightsToCreate.Should().HaveCount(1);
        sortedRights.RightsToCreate!.First().Should().BeEquivalentTo(newRight);

        sortedRights.RightsToUpdate.Should().HaveCount(1);
        sortedRights.RightsToUpdate!.First().Should().BeEquivalentTo(thirdExistsRight);

        sortedRights.RightsToDelete.Should().HaveCount(1);
        sortedRights.RightsToDelete!.First().Should().BeEquivalentTo(firstExistsRightWillDeleted);
    }

    [Fact]
    public async Task SortGamesRightsAsync_WhenNoRightsInDb_ReturnAllRightsAsCreate()
    {
        //Arrange
        var comparer = new RightsSortComparer();
        var gamesService = CreateServiceForTest((adminDbContext, logger) => new GamesService(adminDbContext, logger));
        var rightsService =
            CreateServiceForTest((adminDbContext, logger) =>
                new RightsService(gamesService, adminDbContext, comparer, logger));

        const string name = "newRight1";
        const string description = "Новое право 1";
        var newRight = new Right
        {
            Id = Guid.Empty,
            Name = name,
            Description = description,
            GameId = FakeGames.GetFirstExistsGameId
        };
        var rightsToSort = new List<Right>
        {
            newRight
        };

        var allExistsRights = FakeRights.GetManyCorrectExistsModels().ToList();
        await rightsService.TryDeleteRangeAsync(allExistsRights, MockCancellationToken);

        //Act
        var sortedRights = await rightsService.SortGamesRightsAsync(FakeGames.GetFirstExistsGameName, rightsToSort,
            MockCancellationToken);

        //Assert
        sortedRights.RightsToCreate.Should().NotBeNull();

        sortedRights.RightsToCreate.Should().HaveCount(1);
        sortedRights.RightsToCreate!.First().Should().BeEquivalentTo(newRight);
    }

    [Fact]
    public async Task SortGamesRightsAsync_WithIncorrectGameName_ArgumentNullException()
    {
        //Arrange
        var comparer = new RightsSortComparer();
        var gamesService = CreateServiceForTest((adminDbContext, logger) => new GamesService(adminDbContext, logger));
        var rightsService =
            CreateServiceForTest((adminDbContext, logger) =>
                new RightsService(gamesService, adminDbContext, comparer, logger));

        var firstExistsRightWillDeleted = FakeRights.GetManyCorrectExistsModels().First();
        firstExistsRightWillDeleted.Game = FakeGames.GetManyCorrectExistsModels().First();
        var secondExistsRight = FakeRights.GetManyCorrectExistsModels().ElementAt(1);
        var thirdExistsRight = FakeRights.GetManyCorrectExistsModels().ElementAt(2);
        const string updatedDescription = "Обновленное описание права 3";
        thirdExistsRight.Description = updatedDescription;

        const string name = "newRight1";
        const string description = "Новое право 1";
        var newRight = new Right
        {
            Id = Guid.Empty,
            Name = name,
            Description = description,
            GameId = FakeGames.GetFirstExistsGameId
        };
        var rightsToSort = new List<Right>
        {
            newRight,
            secondExistsRight,
            thirdExistsRight
        };

        //Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await rightsService.SortGamesRightsAsync(null!, rightsToSort,
                MockCancellationToken));
    }

    [Fact]
    public async Task SortGamesRightsAsync_WithNotExistsGameName_NotFoundException()
    {
        //Arrange
        var comparer = new RightsSortComparer();
        var gamesService = CreateServiceForTest((adminDbContext, logger) => new GamesService(adminDbContext, logger));
        var rightsService =
            CreateServiceForTest((adminDbContext, logger) =>
                new RightsService(gamesService, adminDbContext, comparer, logger));

        var firstExistsRightWillDeleted = FakeRights.GetManyCorrectExistsModels().First();
        firstExistsRightWillDeleted.Game = FakeGames.GetManyCorrectExistsModels().First();
        var secondExistsRight = FakeRights.GetManyCorrectExistsModels().ElementAt(1);
        var thirdExistsRight = FakeRights.GetManyCorrectExistsModels().ElementAt(2);
        const string updatedDescription = "Обновленное описание права 3";
        thirdExistsRight.Description = updatedDescription;

        const string name = "newRight1";
        const string description = "Новое право 1";
        var newRight = new Right
        {
            Id = Guid.Empty,
            Name = name,
            Description = description,
            GameId = FakeGames.GetFirstExistsGameId
        };
        var rightsToSort = new List<Right>
        {
            newRight,
            secondExistsRight,
            thirdExistsRight
        };

        //Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(async () =>
            await rightsService.SortGamesRightsAsync("negn1", rightsToSort,
                MockCancellationToken));
    }

    [Fact]
    public async Task SortGamesRightsAsync_WithIncorrectRights_ArgumentException()
    {
        //Arrange
        var comparer = new RightsSortComparer();
        var gamesService = CreateServiceForTest((adminDbContext, logger) => new GamesService(adminDbContext, logger));
        var rightsService =
            CreateServiceForTest((adminDbContext, logger) =>
                new RightsService(gamesService, adminDbContext, comparer, logger));

        var rightsToSort = FakeRights.GetManyIncorrectNewModels().ToList();

        //Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(async () =>
            await rightsService.SortGamesRightsAsync(FakeGames.GetFirstExistsGameName, rightsToSort,
                MockCancellationToken));
    }

    #endregion
}