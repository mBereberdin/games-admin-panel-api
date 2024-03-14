namespace Tests.FakeData;

using Domain.Models.Rights;

/// <summary>
/// Фиктивные права.
/// </summary>
public static class FakeRights
{
    /// <summary>
    /// Первое название существующего права.
    /// </summary>
    private static string _firstExistsRightName = string.Empty;

    /// <summary>
    /// Получить первое название существующего права.
    /// </summary>
    public static string GetFirstExistsRightName
    {
        get
        {
            if (_firstExistsRightName.Equals(string.Empty))
            {
                _firstExistsRightName = GetManyCorrectExistsModels().First().Name;
            }

            return _firstExistsRightName;
        }
    }

    /// <summary>
    /// Первый идентификатор существующего права.
    /// </summary>
    private static Guid _firstExistsRightId = Guid.Empty;

    /// <summary>
    /// Получить первый идентификатор существующего права.
    /// </summary>
    public static Guid GetFirstExistsRightId
    {
        get
        {
            if (_firstExistsRightId.Equals(Guid.Empty))
            {
                _firstExistsRightId = GetManyCorrectExistsModels().First().Id;
            }

            return _firstExistsRightId;
        }
    }

    #region Correct

    #region Models

    /// <summary>
    /// Получить несколько корректных фиктивных моделей прав которые есть в бд.
    /// </summary>
    public static IEnumerable<Right> GetManyCorrectExistsModels()
    {
        return new List<Right>
        {
            new()
            {
                Id = new Guid("8793DACB-84B3-4F4B-9F6B-9AD81B08BAF6"),
                Name = "test_right_1",
                Description = "Тестовое право 1",
                GameId = FakeGames.GetFirstExistsGameId
            },
            new()
            {
                Id = new Guid("A36F698A-321A-4A27-8001-0EAF885C1A3B"),
                Name = "test_right_2",
                Description = "Тестовое право 2",
                GameId = FakeGames.GetFirstExistsGameId
            },
            new()
            {
                Id = new Guid("9DB33B6E-E024-4F1B-9994-D8427C2B8916"),
                Name = "test_right_3",
                Description = "Тестовое право 3",
                GameId = FakeGames.GetFirstExistsGameId
            }
        };
    }

    #endregion

    #endregion

    #region Incorrect

    #region Models

    /// <summary>
    /// Получить несколько некорректных фиктивных новых прав, для теста добавления.
    /// </summary>
    public static IEnumerable<object[]> GetManyIncorrectModelsForAddTest()
    {
        var rightsModels = GetManyIncorrectNewModels();
        var objectsList = new List<object[]>();

        objectsList.AddRange(rightsModels.Select(right => new[] { right }));

        return objectsList;
    }

    /// <summary>
    /// Получить несколько некорректных фиктивных новых моделей прав.
    /// </summary>
    public static IEnumerable<Right> GetManyIncorrectNewModels()
    {
        return new List<Right>
        {
            new()
            {
                Id = Guid.Empty,
                Name = null!, // Не корректно, потому что Name - отсутствует.
                Description = "Некорректное тестовое право 1",
                GameId = FakeGames.GetFirstExistsGameId
            },
            new()
            {
                Id = Guid.Empty,
                Name = "", // Не корректно, потому что Name - отсутствует.
                Description = "Некорректное тестовое право 2",
                GameId = FakeGames.GetFirstExistsGameId
            },
            new()
            {
                Id = Guid.Empty,
                Name =
                    "123456781234567812345678123456781234567812345678", // Не корректно, потому что Name - больше лимита.
                Description = "Некорректное тестовое право 3",
                GameId = FakeGames.GetFirstExistsGameId
            },
            new()
            {
                Id = Guid.Empty,
                Name = "incorrectTestRight4",
                Description = null!, // Не корректно, потому что Description - отсутствует.
                GameId = FakeGames.GetFirstExistsGameId
            },
            new()
            {
                Id = Guid.Empty,
                Name = "incorrectTestRight5",
                Description = "", // Не корректно, потому что Description - отсутствует.
                GameId = FakeGames.GetFirstExistsGameId
            },
            new()
            {
                Id = Guid.Empty,
                Name = "incorrectTestRight6",
                Description =
                    "123456781234567812345678123456781234567812345678", // Не корректно, потому что Description - больше лимита.
                GameId = FakeGames.GetFirstExistsGameId
            },
            new()
            {
                Id = Guid.Empty,
                Name = "incorrectTestRight8",
                Description = "Некорректное тестовое право 8",
                GameId = Guid.Empty // Не корректно, потому что GameId - отсутствует.
            },
            new()
            {
                Id = Guid.Empty,
                Name = "incorrectTestRight9",
                Description = "Некорректное тестовое право 9",
                GameId = Guid.NewGuid() // Не корректно, потому что GameId - нет в бд.
            }
        };
    }

    #endregion

    #endregion
}