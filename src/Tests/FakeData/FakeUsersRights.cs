namespace Tests.FakeData;

using Domain.Models.Rights;

/// <summary>
/// Фиктивные права пользователя.
/// </summary>
public static class FakeUsersRights
{
    /// <summary>
    /// Получить несколько корректных фиктивных моделей которые есть в бд.
    /// </summary>
    public static IEnumerable<UsersRight> GetManyCorrectExistsModels()
    {
        var fakeRights = FakeRights.GetManyCorrectExistsModels().ToList();

        return new List<UsersRight>
        {
            new()
            {
                Id = new Guid("0151955E-8691-40C4-931D-652E259B8D01"),
                UserId = FakeUsers.GetFirstExistsUserId,
                RightId = fakeRights.ElementAt(0).Id
            },
            new()
            {
                Id = new Guid("54B1D586-2B0F-4C0E-8197-572004EB8536"),
                UserId = FakeUsers.GetFirstExistsUserId,
                RightId = fakeRights.ElementAt(1).Id
            },
            new()
            {
                Id = new Guid("9536CA54-204D-4C92-A5D6-09EEAA1A1835"),
                UserId = FakeUsers.GetFirstExistsUserId,
                RightId = fakeRights.ElementAt(2).Id
            }
        };
    }
}