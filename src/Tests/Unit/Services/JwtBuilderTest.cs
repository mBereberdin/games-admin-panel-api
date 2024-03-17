namespace Tests.Unit.Services;

using System.IdentityModel.Tokens.Jwt;

using Domain.Models.Rights;

using FluentAssertions;

using Infrastructure.Services.Implementations;
using Infrastructure.Services.Interfaces;
using Infrastructure.Settings;

using Microsoft.Extensions.Options;

using Tests.FakeData;
using Tests.Fixtures;

/// <summary>
/// Тесты для <see cref="IJwtBuilder"/>.
/// </summary>
public class JwtBuilderTests : AppSettingsFixture
{
    /// <summary>
    /// Настройки jwt.
    /// </summary>
    private readonly IOptions<JwtSettings> _jwtSettingsOptions;

    /// <inheritdoc cref="JwtBuilderTests"/>
    public JwtBuilderTests()
    {
        var jwtSettings = GetObjectFromAppSettings<JwtSettings>(nameof(JwtSettings));
        _jwtSettingsOptions = Options.Create(jwtSettings);
    }

    [Fact]
    public void Build_WithFilledSettings_ReturnTokenWithSettingsInfo()
    {
        var builder = new JwtBuilder(_jwtSettingsOptions, ServiceFixture.MockLogger);
        var jwtSettings = _jwtSettingsOptions.Value;

        // Act
        var token = builder.Build();

        var tokenTimeStart = DateTime.UtcNow.AddMinutes(jwtSettings.ExpiryInMinutes - 1).TimeOfDay;
        var tokenTimeEnd = DateTime.UtcNow.AddMinutes(jwtSettings.ExpiryInMinutes + 1).TimeOfDay;

        var parsedToken = new JwtSecurityTokenHandler().ReadJwtToken(token);

        // Assert
        parsedToken.Should().NotBeNull();
        parsedToken.Issuer.Should().BeEquivalentTo(jwtSettings.Issuer);
        parsedToken.Audiences.Should().ContainSingle(jwtSettings.Audience);
        parsedToken.ValidTo.Should().BeLessThan(tokenTimeEnd);
        parsedToken.ValidTo.Should().BeMoreThan(tokenTimeStart);
    }

    [Fact]
    public void AddClaims_WithRights_Success()
    {
        var builder = new JwtBuilder(_jwtSettingsOptions, ServiceFixture.MockLogger);
        var rights = FakeRights.GetManyCorrectExistsModels().ToList();
        rights.ForEach(right => right.Game = FakeGames.GetManyCorrectExistsModels().First());

        var rightsGames = rights.GroupBy(right => right.Game!.Name).Select(group => group.First().Game);

        // Act
        builder.AddClaims(rights);
        var token = builder.Build();
        var parsedToken = new JwtSecurityTokenHandler().ReadJwtToken(token);

        // Assert
        rightsGames.All(game => parsedToken.Claims.Any(claim => claim.Type.Equals(game!.Name))).Should().BeTrue();
    }

    [Fact]
    public void AddClaims_WithoutRights_ArgumentNullException()
    {
        var builder = new JwtBuilder(_jwtSettingsOptions, ServiceFixture.MockLogger);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => builder.AddClaims(null!));
        Assert.Throws<ArgumentNullException>(() => builder.AddClaims(new List<Right>()));
    }

    [Fact]
    public void AddUsername_WithUsername_Success()
    {
        var builder = new JwtBuilder(_jwtSettingsOptions, ServiceFixture.MockLogger);
        const string username = "testUsername";

        // Act
        builder.AddUsername(username);
        var token = builder.Build();
        var parsedToken = new JwtSecurityTokenHandler().ReadJwtToken(token);

        // Assert
        parsedToken.Claims.Single(claim => claim.Type.Equals("Username")).Value.Should().BeEquivalentTo(username);
    }

    [Fact]
    public void AddUsername_WithoutUsername_ArgumentNullException()
    {
        var builder = new JwtBuilder(_jwtSettingsOptions, ServiceFixture.MockLogger);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => builder.AddUsername(null!));
        Assert.Throws<ArgumentNullException>(() => builder.AddUsername(""));
    }
}