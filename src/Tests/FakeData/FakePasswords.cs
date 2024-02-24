namespace Tests.FakeData;

using Domain.DTOs.Passwords;
using Domain.Models.Passwords;

/// <summary>
/// Фиктивные пароли.
/// </summary>
public static class FakePasswords
{
    /// <summary>
    /// Первый идентификатор существующего пароля.
    /// </summary>
    private static Guid _firstExistsPasswordId = Guid.Empty;

    /// <summary>
    /// Получить первый идентификатор существующего пароля.
    /// </summary>
    public static Guid GetFirstExistsPasswordId
    {
        get
        {
            if (_firstExistsPasswordId.Equals(Guid.Empty))
            {
                _firstExistsPasswordId = GetManyCorrectExistsModels().First().Id;
            }

            return _firstExistsPasswordId;
        }
    }

    /// <summary>
    /// Получить несколько некорректных фиктивных дто создания пароля, для теста создания.
    /// </summary>
    public static IEnumerable<object[]> GetManyIncorrectCreateDtosForAddTest()
    {
        var passwords = GetManyIncorrectCreatePasswordDtos();
        var objectsList = new List<object[]>();

        objectsList.AddRange(passwords.Select(password => new[] { password }));

        return objectsList;
    }

    /// <summary>
    /// Получить несколько некорректных фиктивных дто создания пароля.
    /// </summary>
    private static IEnumerable<CreatePasswordDto> GetManyIncorrectCreatePasswordDtos()
    {
        return new List<CreatePasswordDto>
        {
            new(null!, FakeUsers.GetFirstExistsUserId), // Не корректно, потому что encryptedValue - null.
            new("", FakeUsers.GetFirstExistsUserId), // Не корректно, потому что encryptedValue - пустой.
            new("1234567812345678123456781234567812345678",
                FakeUsers.GetFirstExistsUserId), // Не корректно, потому что encryptedValue - пустой.
            new(null!, Guid.Empty), // Не корректно, потому что userId - пустой.
            new(null!, Guid.NewGuid()) // Не корректно, потому что userId - которого нет в бд.
        };
    }

    /// <summary>
    /// Получить несколько некорректных фиктивных новых паролей, для теста добавления.
    /// </summary>
    public static IEnumerable<object[]> GetManyIncorrectModelsForAddTest()
    {
        var passwords = GetManyIncorrectNewModels();
        var objectsList = new List<object[]>();

        objectsList.AddRange(passwords.Select(password => new[] { password }));

        return objectsList;
    }

    /// <summary>
    /// Получить несколько некорректных фиктивных новых моделей паролей.
    /// </summary>
    private static IEnumerable<Password> GetManyIncorrectNewModels()
    {
        return new List<Password>
        {
            new()
            {
                Id = Guid.Empty,
                EncryptedValue = null!, // Не корректно, потому что EncryptedValue - отсутствует.
                UserId = FakeUsers.GetFirstExistsUserId
            },
            new()
            {
                Id = Guid.Empty,
                EncryptedValue = "", // Не корректно, потому что EncryptedValue - пустой.
                UserId = FakeUsers.GetFirstExistsUserId
            },
            new()
            {
                Id = Guid.Empty,
                EncryptedValue =
                    "test123456123456123456123456123456123456123456123456", // Не корректно, потому что EncryptedValue - больше ограничения.
                UserId = FakeUsers.GetFirstExistsUserId
            },
            new()
            {
                Id = Guid.Empty,
                EncryptedValue = "12",
                UserId = Guid.Empty // Не корректно, потому что UserId - новый (пустой).
            },
            new()
            {
                Id = Guid.Empty,
                EncryptedValue = "123",
                UserId = Guid.NewGuid() // Не корректно, потому что UserId - отсутствует в бд.
            }
        };
    }

    /// <summary>
    /// Получить несколько корректных фиктивных моделей паролей которые есть в бд.
    /// </summary>
    public static IEnumerable<Password> GetManyCorrectExistsModels()
    {
        return new List<Password>
        {
            new()
            {
                Id = new Guid("81913DFE-3742-44EC-87F1-DDFA72B2B0F5"),
                EncryptedValue = "qwe",
                UserId = FakeUsers.GetFirstExistsUserId
            },
            new()
            {
                Id = new Guid("26C14296-A912-4167-8D45-8733D4FD0DA0"),
                EncryptedValue = "rty",
                UserId = FakeUsers.GetManyCorrectExistsModels().ElementAt(1).Id
            },
            new()
            {
                Id = new Guid("8F514889-96A5-4BC5-B6C1-75765BA39DA9"),
                EncryptedValue = "uio",
                UserId = FakeUsers.GetManyCorrectExistsModels().ElementAt(2).Id
            },
            new()
            {
                Id = new Guid("974B0E7D-A4D4-4C0C-A83B-EEB9A146D42E"),
                EncryptedValue = "asd",
                UserId = FakeUsers.GetManyCorrectExistsModels().ElementAt(3).Id
            },
            new()
            {
                Id = new Guid("841E4EB7-ACED-4B00-A7D6-4103C6833F16"),
                EncryptedValue = "fgh",
                UserId = FakeUsers.GetManyCorrectExistsModels().ElementAt(4).Id
            }
        };
    }
}