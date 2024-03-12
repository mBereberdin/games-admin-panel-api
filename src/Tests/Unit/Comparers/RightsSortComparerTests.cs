namespace Tests.Unit.Comparers;

using Domain.Models.Rights;

using FluentAssertions;

using Infrastructure.Comparers;

using Tests.FakeData;

/// <summary>
/// Тесты для <see cref="RightsSortComparer"/>.
/// </summary>
public class RightsSortComparerTests
{
    [Fact]
    public void Equals_EqualsRights_ReturnTrue()
    {
        //Arrange
        var rightsComparer = new RightsSortComparer();
        var firstRight = new Right
        {
            Name = "test_right",
            Description = "Тестовое описание",
            GameId = FakeGames.GetFirstExistsGameId,
            Id = default
        };

        var secondRight = new Right
        {
            Name = "test_right",
            Description = "Тестовое описание",
            GameId = FakeGames.GetFirstExistsGameId,
            Id = FakeRights.GetFirstExistsRightId
        };

        //Act
        var areEquals = rightsComparer.Equals(firstRight, secondRight);
        var areLinksEquals = rightsComparer.Equals(firstRight, firstRight);

        //Assert
        areEquals.Should().BeTrue();
        areLinksEquals.Should().BeTrue();
    }

    [Fact]
    public void Equals_NotEqualsRights_ReturnFalse()
    {
        //Arrange
        var rightsComparer = new RightsSortComparer();
        var firstRight = new Right
        {
            Name = "test_right",
            Description = "Тестовое описание",
            GameId = FakeGames.GetFirstExistsGameId,
            Id = default
        };

        var notEqualsRights = new List<Right>
        {
            new()
            {
                Name = "test_right",
                Description = "Тестовое описание1",
                GameId = FakeGames.GetFirstExistsGameId,
                Id = default
            },
            new()
            {
                Name = "test_right1",
                Description = "Тестовое описание",
                GameId = FakeGames.GetFirstExistsGameId,
                Id = default
            },
            new()
            {
                Name = "test_right",
                Description = "Тестовое описание",
                GameId = Guid.NewGuid(),
                Id = default
            },
            null!
        };

        //Act
        var areEquals = notEqualsRights.Any(secondRight => rightsComparer.Equals(firstRight, secondRight));
        var areEqualsWhenFirstNull = rightsComparer.Equals(null, notEqualsRights.First());

        //Assert
        areEquals.Should().BeFalse();
        areEqualsWhenFirstNull.Should().BeFalse();
    }
}