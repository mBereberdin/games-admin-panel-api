namespace Tests.FakeData;

using Domain.DTOs.Users;
using Domain.Models.Users;

/// <summary>
/// Фиктивные пользователи.
/// </summary>
public static class FakeUsers
{
    /// <summary>
    /// Первый идентификатор существующего пользователя.
    /// </summary>
    private static Guid _firstExistsUserId = Guid.Empty;

    /// <summary>
    /// Получить первый идентификатор существующего пользователя.
    /// </summary>
    public static Guid GetFirstExistsUserId
    {
        get
        {
            if (_firstExistsUserId.Equals(Guid.Empty))
            {
                _firstExistsUserId = GetManyCorrectExistsModels().First().Id;
            }

            return _firstExistsUserId;
        }
    }

    #region Correct

    /// <summary>
    /// Получить несколько корректных фиктивных моделей пользователей которые есть в бд.
    /// </summary>
    public static IEnumerable<User> GetManyCorrectExistsModels()
    {
        return new List<User>
        {
            new()
            {
                Id = new Guid("C71EA4AA-AF5F-48A8-8BBE-C1C887A1AE55"),
                Email = "test1@test.test",
                Nickname = "Test1Test"
            },
            new()
            {
                Id = new Guid("693CB4C0-0460-40E8-BD09-DF9D4A262012"),
                Email = "test2@test.test",
                Nickname = "Test2Test"
            },
            new()
            {
                Id = new Guid("3B707A3E-E136-42FD-83DC-1BF676058B1C"),
                Email = "test3@test.test",
                Nickname = "Test3Test"
            },
            new()
            {
                Id = new Guid("33FBE0E6-1081-4FDB-95B9-B948B976A8C2"),
                Email = "test4@test.test",
                Nickname = "Test4Test"
            },
            new()
            {
                Id = new Guid("20C0947D-9D55-4F75-AE53-9B4FDF9264FD"),
                Email = "test5@test.test",
                Nickname = "Test5Test"
            }
        };
    }

    #endregion

    #region Incorrect

    #region Dtos

    /// <summary>
    /// Получить несколько некорректных фиктивных дто обновления пользователя, для теста обновления.
    /// </summary>
    public static IEnumerable<object[]> GetManyIncorrectUpdateDtosForUpdateTest()
    {
        var usersModels = GetManyIncorrectUpdateUserDtos();
        var objectsList = new List<object[]>();

        objectsList.AddRange(usersModels.Select(user => new[] { user }));

        return objectsList;
    }

    /// <summary>
    /// Получить несколько некорректных фиктивных дто обновления пользователя.
    /// </summary>
    private static IEnumerable<UpdateUserDto> GetManyIncorrectUpdateUserDtos()
    {
        return new List<UpdateUserDto>
        {
            new(GetFirstExistsUserId)
            {
                Email = null // Некорректно потому что пустой email.
            },
            new(GetFirstExistsUserId)
            {
                Email = "" // Некорректно потому что пустая строка.
            },
            new(GetFirstExistsUserId)
            {
                Email = "12345678123456781234567812345678@test.test" // Некорректно потому что больше лимита.
            },
            new(GetFirstExistsUserId)
            {
                Nickname = null // Некорректно потому что пустой nickname.
            },
            new(GetFirstExistsUserId)
            {
                Nickname = "" // Некорректно потому что пустая строка.
            },
            new(GetFirstExistsUserId)
            {
                Nickname = "1234512345123451234512345" // Некорректно потому что больше лимита.
            },
            new(Guid.NewGuid()) // Некорректно, потому что такого id нет в бд.
        };
    }

    /// <summary>
    /// Получить несколько некорректных фиктивных дто создания пользователя, для теста создания.
    /// </summary>
    public static IEnumerable<object[]> GetManyIncorrectCreateDtosForAddTest()
    {
        var usersModels = GetManyIncorrectCreateUserDtos();
        var objectsList = new List<object[]>();

        objectsList.AddRange(usersModels.Select(user => new[] { user }));

        return objectsList;
    }

    /// <summary>
    /// Получить несколько некорректных фиктивных дто создания пользователя.
    /// </summary>
    private static IEnumerable<CreateUserDto> GetManyIncorrectCreateUserDtos()
    {
        return new List<CreateUserDto>
        {
            new(null!, "addTest1@test.com"), // Не корректно, потому что Nickname - null.
            new("", "addTest2@test.com"), // Не корректно, потому что Nickname - пустой.
            new("1234512345123451234512345", "addTest3@test.com"), // Не корректно, потому что Nickname - больше лимита.
            new("addTest4", null!), // Не корректно, потому что Email - null.
            new("addTest5", ""), // Не корректно, потому что Email - пустой.
            new("addTest6",
                "12345678123456781234567812345678@test.test"), // Не корректно, потому что Email - больше лимита.
            new(null!, null!), // Не корректно, потому что не заполнен.
            new("", "") // Не корректно, потому что не заполнен.
        };
    }

    #endregion

    #region Models

    /// <summary>
    /// Получить несколько некорректных фиктивных новых пользователей, для теста добавления.
    /// </summary>
    public static IEnumerable<object[]> GetManyIncorrectModelsForUpdateTest()
    {
        // Пропускаем первого потому что у него некорректен только id, который ниже будет заменен.
        var incorrectNewUserModels = GetManyIncorrectNewModels().Skip(1).ToList();
        var existsUserId = GetFirstExistsUserId;
        foreach (var incorrectNewUserModel in incorrectNewUserModels)
        {
            incorrectNewUserModel.Id = existsUserId;
        }

        incorrectNewUserModels.AddRange(new[]
        {
            new User
            {
                Email = "additionalNewUserForUpdate1@test.com",
                Nickname = "additionalNewUser1",
                Id = Guid.Empty // для проверки обновления при нулевом id (пользователя нет в бд).
            },
            new User
            {
                Email = "additionalNewUserForUpdate2@test.com",
                Nickname = "additionalNewUser2",
                Id = Guid.NewGuid() // для проверки обновления при id, которого нет в бд.
            }
        });

        var objectsList = new List<object[]>();
        objectsList.AddRange(incorrectNewUserModels.Select(user => new[] { user }));

        return objectsList;
    }

    /// <summary>
    /// Получить несколько некорректных фиктивных новых пользователей, для теста добавления.
    /// </summary>
    public static IEnumerable<object[]> GetManyIncorrectModelsForAddTest()
    {
        var usersModels = GetManyIncorrectNewModels();
        var objectsList = new List<object[]>();

        objectsList.AddRange(usersModels.Select(user => new[] { user }));

        return objectsList;
    }

    /// <summary>
    /// Получить несколько некорректных фиктивных новых моделей пользователей.
    /// </summary>
    private static IEnumerable<User> GetManyIncorrectNewModels()
    {
        return new List<User>
        {
            new()
            {
                Id = Guid.Empty,
                Email = null!, // Не корректно, потому что email - отсутствует.
                Nickname = "inCorrect2Test"
            },
            new()
            {
                Id = Guid.Empty,
                Email = "", // Не корректно, потому что email - пустой.
                Nickname = "inCorrect3Test"
            },
            new()
            {
                Id = Guid.Empty,
                Email =
                    "test123456123456123456123456123456123456@test.com", // Не корректно, потому что email - больше ограничения.
                Nickname = "inCorrect4Test"
            },
            new()
            {
                Id = Guid.Empty,
                Email = "inCorrect5@test.test",
                Nickname = null! // Не корректно, потому что nickname - отсутствует.
            },
            new()
            {
                Id = Guid.Empty,
                Email = "inCorrect6@test.test",
                Nickname = "" // Не корректно, потому что nickname - пустой.
            },
            new()
            {
                Id = Guid.Empty,
                Email = "inCorrect7@test.test",
                Nickname = "1234512345123451234512345" // Не корректно, потому что nickname - больше ограничения.
            }
        };
    }

    #endregion

    #endregion
}