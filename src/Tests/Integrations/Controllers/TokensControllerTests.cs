namespace Tests.Integrations.Controllers;

using System.IdentityModel.Tokens.Jwt;
using System.Net;

using Domain.DTOs.Login;

using FluentAssertions;

using Infrastructure.Settings;

using Tests.Extensions;
using Tests.FakeData;
using Tests.Fixtures;

using WebApi.Controllers;

/// <summary>
/// Тесты для <see cref="TokensController"/>.
/// </summary>
public class TokensControllerTests : IntegrationFixture
{
    /// <inheritdoc cref="TokensControllerTests"/>
    /// <param name="factory">Фабрика проекта WebApi.</param>
    public TokensControllerTests(WebApiApplicationFactory<Program> factory) : base(factory)
    {
    }

    [Fact]
    public async Task CreateAsync_CorrectLoginDto_200()
    {
        //Arrange
        var existsUser = FakeUsers.GetManyCorrectExistsModels().First();
        var existsUserPassword = FakePasswords.GetManyCorrectExistsModels()
                                              .Single(password => password.UserId.Equals(existsUser.Id));
        var jwtSettings = AppSettingsFixture.GetObjectFromAppSettings<JwtSettings>(nameof(JwtSettings));

        var loginDto = new LoginDto(existsUser.Nickname, existsUserPassword.EncryptedValue);

        //Act
        var response = await Client.PostAsync("tokens", loginDto.ToStringContent());

        var tokenTimeStart = DateTime.UtcNow.AddMinutes(jwtSettings.ExpiryInMinutes - 1).TimeOfDay;
        var tokenTimeEnd = DateTime.UtcNow.AddMinutes(jwtSettings.ExpiryInMinutes + 1).TimeOfDay;

        var token = await response.Content.ReadAsStringAsync();
        var parsedToken = new JwtSecurityTokenHandler().ReadJwtToken(token);

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        parsedToken.Claims.Single(claim => claim.Type.Equals("Username")).Value.Should()
                   .BeEquivalentTo(existsUser.Nickname);
        parsedToken.Issuer.Should().BeEquivalentTo(jwtSettings.Issuer);
        parsedToken.Audiences.Should().ContainSingle(jwtSettings.Audience);
        parsedToken.ValidTo.Should().BeLessThan(tokenTimeEnd);
        parsedToken.ValidTo.Should().BeMoreThan(tokenTimeStart);
    }

    [Theory]
    [MemberData(nameof(GetIncorrectLoginDto))]
    public async Task CreateAsync_IncorrectLoginDto_400(LoginDto loginDto)
    {
        //Act
        var response = await Client.PostAsync("tokens", loginDto.ToStringContent());

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateAsync_IncorrectUserPassword_401()
    {
        //Arrange
        var existsUser = FakeUsers.GetManyCorrectExistsModels().First();
        var existsUserPassword = FakePasswords.GetManyCorrectExistsModels()
                                              .Single(password => password.UserId.Equals(existsUser.Id));

        var loginDto = new LoginDto("notExistsNickname", existsUserPassword.EncryptedValue);

        //Act
        var response = await Client.PostAsync("tokens", loginDto.ToStringContent());

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateAsync_UserWithoutRights_403()
    {
        //Arrange
        var existsUser = FakeUsers.GetManyCorrectExistsModels().ElementAt(1);
        var existsUserPassword = FakePasswords.GetManyCorrectExistsModels()
                                              .Single(password => password.UserId.Equals(existsUser.Id));

        var loginDto = new LoginDto(existsUser.Nickname, existsUserPassword.EncryptedValue);

        //Act
        var response = await Client.PostAsync("tokens", loginDto.ToStringContent());

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task CreateAsync_NotExistsPassword_404()
    {
        //Arrange
        var existsUser = FakeUsers.GetManyCorrectExistsModels().First();
        var loginDto = new LoginDto(existsUser.Nickname, "notExistsUsersPassword");

        //Act
        var response = await Client.PostAsync("tokens", loginDto.ToStringContent());

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    /// <summary>
    /// Получить некорректные ДТО авторизации.
    /// </summary>
    /// <returns>Перечисление некорректных ДТО авторизаций.</returns>
    public static IEnumerable<object[]> GetIncorrectLoginDto()
    {
        var dtos = new List<LoginDto>
        {
            new(null!, "12345"), // Nickname - отсутствует
            new("", "12345"), // Nickname - отсутствует
            new("1234512345123451234512345", "12345"), // Nickname - больше лимита
            new("test1", null!), // EncryptedPassword - отсутствует
            new("test1", ""), // EncryptedPassword - отсутствует
            new("test1", "123456781234567812345678123456781234567812345678") // EncryptedPassword - больше лимита
        };
        var objectsList = new List<object[]>();

        objectsList.AddRange(dtos.Select(right => new[] { right }));

        return objectsList;
    }
}