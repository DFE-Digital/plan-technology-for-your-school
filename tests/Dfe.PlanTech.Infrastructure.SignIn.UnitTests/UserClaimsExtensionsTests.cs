using System.Security.Claims;
using System.Text.Json;
using Dfe.PlanTech.Domain.SignIn.Enums;
using Dfe.PlanTech.Domain.SignIn.Models;
using Dfe.PlanTech.Infrastructure.SignIn.Extensions;

namespace Dfe.PlanTech.Infrastructure.SignIn.UnitTests;

public class UserClaimsExtensionsTests
{
    [Fact]
    public void GetUserId_Should_Return_UserId_When_ClaimsPrincipal_Exists()
    {
        string expectedUserId = "TestingName";

        var identity = new ClaimsIdentity(new[] { new Claim(ClaimConstants.NameIdentifier, expectedUserId) });
        var claimsPrincipal = new ClaimsPrincipal(identity);

        var userId = UserClaimsExtensions.GetUserId(claimsPrincipal);

        Assert.Equal(expectedUserId, userId);
    }

    [Fact]
    public void GetUserId_Should_Throw_When_Claim_Is_Missing()
    {
        var identity = new ClaimsIdentity(Array.Empty<Claim>());
        var claimsPrincipal = new ClaimsPrincipal(identity);

        Assert.ThrowsAny<Exception>(() => UserClaimsExtensions.GetUserId(claimsPrincipal));
    }


    [Fact]
    public void GetUserId_Should_Throw_When_ClaimsPrincipal_Is_Null()
    {
        Assert.ThrowsAny<ArgumentNullException>(() => UserClaimsExtensions.GetUserId(null!));
    }

    [Fact]
    public void GetOrganisation_Should_Throw_When_ClaimsPrincipal_Is_Null()
    {
        Assert.ThrowsAny<ArgumentNullException>(() => UserClaimsExtensions.GetOrganisation(null!));
    }

    [Fact]
    public void GetOrganisation_Should_ReturnNull_When_Org_Claim_Is_Missing()
    {
        var identity = new ClaimsIdentity(Array.Empty<Claim>());
        var claimsPrincipal = new ClaimsPrincipal(identity);

        var organisation = UserClaimsExtensions.GetOrganisation(claimsPrincipal);

        Assert.Null(organisation);
    }

    [Fact]
    public void GetOrganisation_Should_Throw_When_OrgClaim_Is_NotJson()
    {
        var identity = new ClaimsIdentity(new[] { new Claim(ClaimConstants.Organisation, "not a real claim") });
        var claimsPrincipal = new ClaimsPrincipal(identity);

        Assert.ThrowsAny<Exception>(() => UserClaimsExtensions.GetOrganisation(claimsPrincipal));
    }

    [Fact]
    public void GetOrganisation_Should_ReturnNull_When_Organisation_Has_No_Id()
    {
        var organisation = new Organisation()
        {
            Id = Guid.Empty,
        };

        var organisationJson = JsonSerializer.Serialize(organisation);

        var identity = new ClaimsIdentity(new[] { new Claim(ClaimConstants.Organisation, organisationJson) });
        var claimsPrincipal = new ClaimsPrincipal(identity);

        var foundOrganisation = UserClaimsExtensions.GetOrganisation(claimsPrincipal);

        Assert.Null(foundOrganisation);
    }

    [Fact]
    public void GetOrganisation_Should_Return_Organisation_When_Exists()
    {
        var organisation = new Organisation()
        {
            Id = Guid. NewGuid()
        };

        var organisationJson = JsonSerializer.Serialize(organisation);

        var identity = new ClaimsIdentity(new[] { new Claim(ClaimConstants.Organisation, organisationJson) });
        var claimsPrincipal = new ClaimsPrincipal(identity);

        var foundOrganisation = UserClaimsExtensions.GetOrganisation(claimsPrincipal);

        Assert.NotNull(foundOrganisation);
        Assert.Equal(organisation.Id, foundOrganisation.Id);
    }
}