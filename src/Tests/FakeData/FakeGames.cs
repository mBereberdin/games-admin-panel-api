namespace Tests.FakeData;

using Domain.Models.Games;

/// <summary>
/// Фиктивные игры.
/// </summary>
public static class FakeGames
{
    /// <summary>
    /// Первое название существующей игры.
    /// </summary>
    private static string _firstExistsGameName = string.Empty;


    /// <summary>
    /// Первый идентификатор существующей игры.
    /// </summary>
    private static Guid _firstExistsGameId = Guid.Empty;

    /// <summary>
    /// Получить первое название существующей игры.
    /// </summary>
    public static string GetFirstExistsGameName
    {
        get
        {
            if (_firstExistsGameName.Equals(string.Empty))
            {
                _firstExistsGameName = GetManyCorrectExistsModels().First().Name;
            }

            return _firstExistsGameName;
        }
    }

    /// <summary>
    /// Получить первый идентификатор существующей игры.
    /// </summary>
    public static Guid GetFirstExistsGameId
    {
        get
        {
            if (_firstExistsGameId.Equals(Guid.Empty))
            {
                _firstExistsGameId = GetManyCorrectExistsModels().First().Id;
            }

            return _firstExistsGameId;
        }
    }

    #region Correct

    #region Models

    /// <summary>
    /// Получить несколько корректных фиктивных моделей игр которые есть в бд.
    /// </summary>
    public static IEnumerable<Game> GetManyCorrectExistsModels()
    {
        return new List<Game>
        {
            new()
            {
                Id = new Guid("98C2A358-2591-4052-BD32-411A73EDEF32"),
                Name = "testGame1",
                Description = "Тестовая игра 1"
            },
            new()
            {
                Id = new Guid("F8B53195-8103-4EE9-8531-DA70677FDE93"),
                Name = "testGame2",
                Description = null
            }
        };
    }

    #endregion

    #endregion

    #region Incorrect

    #region Models

    /// <summary>
    /// Получить несколько некорректных фиктивных пользователей, для теста обновления.
    /// </summary>
    public static IEnumerable<object[]> GetManyIncorrectModelsForUpdateTest()
    {
        var incorrectNewGameModels = GetManyIncorrectNewModels().ToList();
        var existsGameId = GetManyCorrectExistsModels().First().Id;
        foreach (var incorrectNewUserModel in incorrectNewGameModels)
        {
            incorrectNewUserModel.Id = existsGameId;
        }

        incorrectNewGameModels.AddRange(new[]
        {
            new Game
            {
                Name = "additionalNewGameForUpdate1",
                Description = "additionalNewGame1",
                Id = Guid.Empty // для проверки обновления при нулевом id (нет в бд).
            },
            new Game
            {
                Name = "additionalNewGameForUpdate2",
                Description = "additionalNewGame2",
                Id = Guid.NewGuid() // для проверки обновления при id, которого нет в бд.
            }
        });

        var objectsList = new List<object[]>();
        objectsList.AddRange(incorrectNewGameModels.Select(game => new[] { game }));

        return objectsList;
    }

    /// <summary>
    /// Получить несколько некорректных фиктивных новых моделей, для теста добавления.
    /// </summary>
    public static IEnumerable<object[]> GetManyIncorrectModelsForAddTest()
    {
        var models = GetManyIncorrectNewModels();
        var objectsList = new List<object[]>();

        objectsList.AddRange(models.Select(model => new[] { model }));

        return objectsList;
    }

    /// <summary>
    /// Получить несколько некорректных фиктивных новых моделей.
    /// </summary>
    private static IEnumerable<Game> GetManyIncorrectNewModels()
    {
        return new List<Game>
        {
            new()
            {
                Id = Guid.Empty,
                Name = null!, // Не корректно, потому что Name - отсутствует.
                Description = "Некорректная модель 1"
            },
            new()
            {
                Id = Guid.Empty,
                Name = "", // Не корректно, потому что Name - пустой.
                Description = "Некорректная модель 2"
            },
            new()
            {
                Id = Guid.Empty,
                Name =
                    "123456781234567812345678123456781234567812345678", // Не корректно, потому что Name - больше ограничения.
                Description = "Некорректная модель 3"
            },
            new()
            {
                Id = Guid.Empty,
                Name = "incorrectModel4",
                Description = "" // Не корректно, потому что Description - отсутствует.
            },
            new()
            {
                Id = Guid.Empty,
                Name = "incorrectModel4",
                Description =
                    "123456781234567812345678123456781234567812345678" // Не корректно, потому что Description - отсутствует.
            }
        };
    }

    #endregion

    #endregion
}