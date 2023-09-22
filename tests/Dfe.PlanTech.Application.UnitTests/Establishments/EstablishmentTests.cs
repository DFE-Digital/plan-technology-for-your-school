using Bogus;
using Dfe.PlanTech.Domain.Establishments.Models;

namespace Dfe.PlanTech.Application.UnitTests.Establishments;

public class EstablishmentTests
{
  [Theory]
  [InlineData(1)]
  [InlineData(10)]
  [InlineData(50)]
  public void Should_Trim_OrgName_When_Over_Length(int extraCharacters)
  {
    var faker = new Faker();

    var orgName = faker.Random.AlphaNumeric(Establishment.OrgNameLength + extraCharacters);

    var establishment = new Establishment()
    {
      OrgName = orgName,
      EstablishmentRef = faker.Random.AlphaNumeric(Establishment.EstablishmentRefLength - 1),
      EstablishmentType = faker.Random.AlphaNumeric(Establishment.EstablishmentTypeLength - 1),
    };

    Assert.True(establishment.OrgName.Length <= Establishment.OrgNameLength, $"OrgName is {establishment.OrgName.Length}");
    Assert.Equal(Establishment.OrgNameLength, establishment.OrgName.Length);
    Assert.StartsWith(establishment.OrgName, orgName);
    Assert.NotEqual(orgName, establishment.OrgName);
  }

  [Theory]
  [InlineData(1)]
  [InlineData(10)]
  [InlineData(50)]
  public void Should_Trim_EstablishmentRef_When_Over_Length(int extraCharacters)
  {
    var faker = new Faker();

    var establishmentRef = faker.Random.AlphaNumeric(Establishment.EstablishmentRefLength + extraCharacters);

    var establishment = new Establishment()
    {
      OrgName = faker.Random.AlphaNumeric(Establishment.OrgNameLength - 1),
      EstablishmentRef = establishmentRef,
      EstablishmentType = faker.Random.AlphaNumeric(Establishment.EstablishmentTypeLength - 1),
    };

    Assert.True(establishment.EstablishmentRef.Length <= Establishment.EstablishmentRefLength, $"Ref is {establishment.EstablishmentRef.Length}");
    Assert.Equal(Establishment.EstablishmentTypeLength, establishment.EstablishmentRef.Length);
    Assert.StartsWith(establishment.EstablishmentRef, establishmentRef);
    Assert.NotEqual(establishmentRef, establishment.EstablishmentRef);
  }

  [Theory]
  [InlineData(1)]
  [InlineData(10)]
  [InlineData(50)]
  public void Should_Trim_EstablishmentType_When_Over_Length(int extraCharacters)
  {
    var faker = new Faker();

    var establishmentType = faker.Random.AlphaNumeric(Establishment.EstablishmentTypeLength + extraCharacters);

    var establishment = new Establishment()
    {
      OrgName = faker.Random.AlphaNumeric(Establishment.OrgNameLength - 1),
      EstablishmentRef = faker.Random.AlphaNumeric(Establishment.EstablishmentRefLength - 1),
      EstablishmentType = establishmentType
    };

    Assert.True(establishment.EstablishmentType.Length <= Establishment.EstablishmentTypeLength, $"Type is {establishment.EstablishmentType.Length}");
    Assert.Equal(Establishment.EstablishmentTypeLength, establishment.EstablishmentType.Length);
    Assert.StartsWith(establishment.EstablishmentType, establishmentType);
    Assert.NotEqual(establishmentType, establishment.EstablishmentType);
  }
}