namespace Tests.Unit.Comparers;

using Domain.Models.Games;

using FluentAssertions;

using Infrastructure.Comparers;

using Tests.FakeData;

/// <summary>
/// Тесты для <see cref="ExternalGamesComparer"/>.
/// </summary>
public class ExternalGamesComparerTests
{
    [Fact]
    public void Equals_EqualsGames_ReturnTrue()
    {
        //Arrange
        var gamesComparer = new ExternalGamesComparer();
        var firstGame = new Game
        {
            Name = "testGame",
            Description = "Тестовое описание",
            Id = default
        };

        var secondGame = new Game
        {
            Name = "testGame",
            Description = "Тестовое описание",
            Id = FakeRights.GetFirstExistsRightId
        };

        //Act
        var areEquals = gamesComparer.Equals(firstGame, secondGame);
        var areLinksEquals = gamesComparer.Equals(firstGame, firstGame);

        //Assert
        areEquals.Should().BeTrue();
        areLinksEquals.Should().BeTrue();
    }

    [Fact]
    public void Equals_NotEqualsGames_ReturnFalse()
    {
        //Arrange
        var gamesComparer = new ExternalGamesComparer();
        var firstGame = new Game
        {
            Name = "testGame",
            Description = "Тестовое описание",
            Id = default
        };

        var notEqualsGames = new List<Game>
        {
            new()
            {
                Name = "testGame",
                Description = "Тестовое описание1", // отличное описание
                Id = default
            },
            new()
            {
                Name = "testGame2",
                Description = null, // отличное описание
                Id = default
            },
            new()
            {
                Name = "testGame1", // отличное название
                Description = "Тестовое описание2",
                Id = default
            },
            null!
        };

        //Act
        var areEquals = notEqualsGames.Any(secondGame => gamesComparer.Equals(firstGame, secondGame));
        var areEqualsWhenFirstNull = gamesComparer.Equals(null, notEqualsGames.First());

        //Assert
        areEquals.Should().BeFalse();
        areEqualsWhenFirstNull.Should().BeFalse();
    }
}