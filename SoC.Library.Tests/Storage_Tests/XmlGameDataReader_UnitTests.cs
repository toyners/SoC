
namespace Jabberwocky.SoC.Library.UnitTests.Storage_Tests
{
  using System;
  using System.IO;
  using System.Text;
  using Jabberwocky.SoC.Library.Storage;
  using Jabberwocky.SoC.Library.UnitTests.Extensions;
  using NUnit.Framework;
  using Shouldly;

  [TestFixture]
  public class XmlGameDataReader_UnitTests
  {
    [Test]
    [Category("XmlGameDataReader")]
    public void Load_HexDataOnly_ResourceProvidersLoadedCorrectly()
    {
      // Arrange
      var content = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><game><board><hexes>" +
        "<resources>glbglogob gwwwlwlbo</resources>" +
        "<production>9,8,5,12,11,3,6,10,6,0,4,11,2,4,3,5,9,10,8</production>" +
        "</hexes></board></game>";

      XmlGameDataReader xmlGameDataReader;
      var contentBytes = Encoding.UTF8.GetBytes(content);

      // Act
      using (var stream = new MemoryStream(contentBytes))
      {
        xmlGameDataReader = new XmlGameDataReader(stream);
      }

      // Assert
      xmlGameDataReader.ShouldNotBeNull();
      xmlGameDataReader[GameDataSectionKeys.GameBoard]
        .GetStringValue(GameDataValueKeys.HexResources)
        .ShouldBe("glbglogob gwwwlwlbo");
      xmlGameDataReader[GameDataSectionKeys.GameBoard]
        .GetIntegerArrayValue(GameDataValueKeys.HexProduction)
        .ShouldContainExact(new[] { 9, 8, 5, 12, 11, 3, 6, 10, 6, 0, 4, 11, 2, 4, 3, 5, 9, 10, 8 });
    }

    [Test]
    [Category("XmlGameDataReader")]
    public void Load_PlayerDataOnly_PlayerDataLoadedCorrectly()
    {
      // Arrange
      Guid firstId = Guid.NewGuid(), secondId = Guid.NewGuid(), thirdId = Guid.NewGuid(), fourthId = Guid.NewGuid();
      String firstName = "Name_1", secondName = "Name_2", thirdName = "Name_3", fourthName = "Name_4";
      Int32 firstBrickCount = 1, firstGrainCount = 2, firstLumberCount = 3, firstOreCount = 4, firstWoolCount = 5;
      Int32 secondBrickCount = 1, secondGrainCount = 2, secondLumberCount = 3, secondOreCount = 4, secondWoolCount = 5;
      Int32 thirdBrickCount = 1, thirdGrainCount = 2, thirdLumberCount = 3, thirdOreCount = 4, thirdWoolCount = 5;
      Int32 fourthBrickCount = 1, fourthGrainCount = 2, fourthLumberCount = 3, fourthOreCount = 4, fourthWoolCount = 5;

      // Act
      var content = "<game><players>" +
        $"<player id=\"{firstId}\" name=\"{firstName}\" brick=\"{firstBrickCount}\" grain=\"{firstGrainCount}\" lumber=\"{firstLumberCount}\" ore=\"{firstOreCount}\" wool=\"{firstWoolCount}\" />" +
        $"<player id=\"{secondId}\" name=\"{secondName}\" brick=\"{secondBrickCount}\" grain=\"{secondGrainCount}\" lumber=\"{secondLumberCount}\" ore=\"{secondOreCount}\" wool=\"{secondWoolCount}\" />" +
        $"<player id=\"{thirdId}\" name=\"{thirdName}\" brick=\"{thirdBrickCount}\" grain=\"{thirdGrainCount}\" lumber=\"{thirdLumberCount}\" ore=\"{thirdOreCount}\" wool=\"{thirdWoolCount}\" />" +
        $"<player id=\"{fourthId}\" name=\"{fourthName}\" brick=\"{fourthBrickCount}\" grain=\"{fourthGrainCount}\" lumber=\"{fourthLumberCount}\" ore=\"{fourthOreCount}\" wool=\"{fourthWoolCount}\" />" +
        "</players></game>";

      XmlGameDataReader xmlGameDataReader;
      var contentBytes = Encoding.UTF8.GetBytes(content);
      using (var stream = new MemoryStream(contentBytes))
      {
        xmlGameDataReader = new XmlGameDataReader(stream);
      }

      // Assert
      xmlGameDataReader.ShouldNotBeNull();
      xmlGameDataReader[GameDataSectionKeys.PlayerOne].GetIdentityValue(GameDataValueKeys.PlayerId).ShouldBe(firstId);
      xmlGameDataReader[GameDataSectionKeys.PlayerOne].GetStringValue(GameDataValueKeys.PlayerName).ShouldBe(firstName);
      xmlGameDataReader[GameDataSectionKeys.PlayerOne].GetIntegerValue(GameDataValueKeys.PlayerBrick).ShouldBe(firstBrickCount);
      xmlGameDataReader[GameDataSectionKeys.PlayerOne].GetIntegerValue(GameDataValueKeys.PlayerGrain).ShouldBe(firstGrainCount);
      xmlGameDataReader[GameDataSectionKeys.PlayerOne].GetIntegerValue(GameDataValueKeys.PlayerLumber).ShouldBe(firstLumberCount);
      xmlGameDataReader[GameDataSectionKeys.PlayerOne].GetIntegerValue(GameDataValueKeys.PlayerOre).ShouldBe(firstOreCount);
      xmlGameDataReader[GameDataSectionKeys.PlayerOne].GetIntegerValue(GameDataValueKeys.PlayerWool).ShouldBe(firstWoolCount);

      xmlGameDataReader[GameDataSectionKeys.PlayerTwo].GetIdentityValue(GameDataValueKeys.PlayerId).ShouldBe(secondId);
      xmlGameDataReader[GameDataSectionKeys.PlayerTwo].GetStringValue(GameDataValueKeys.PlayerName).ShouldBe(secondName);
      xmlGameDataReader[GameDataSectionKeys.PlayerTwo].GetIntegerValue(GameDataValueKeys.PlayerBrick).ShouldBe(secondBrickCount);
      xmlGameDataReader[GameDataSectionKeys.PlayerTwo].GetIntegerValue(GameDataValueKeys.PlayerGrain).ShouldBe(secondGrainCount);
      xmlGameDataReader[GameDataSectionKeys.PlayerTwo].GetIntegerValue(GameDataValueKeys.PlayerLumber).ShouldBe(secondLumberCount);
      xmlGameDataReader[GameDataSectionKeys.PlayerTwo].GetIntegerValue(GameDataValueKeys.PlayerOre).ShouldBe(secondOreCount);
      xmlGameDataReader[GameDataSectionKeys.PlayerTwo].GetIntegerValue(GameDataValueKeys.PlayerWool).ShouldBe(secondWoolCount);

      xmlGameDataReader[GameDataSectionKeys.PlayerThree].GetIdentityValue(GameDataValueKeys.PlayerId).ShouldBe(thirdId);
      xmlGameDataReader[GameDataSectionKeys.PlayerThree].GetStringValue(GameDataValueKeys.PlayerName).ShouldBe(thirdName);
      xmlGameDataReader[GameDataSectionKeys.PlayerThree].GetIntegerValue(GameDataValueKeys.PlayerBrick).ShouldBe(thirdBrickCount);
      xmlGameDataReader[GameDataSectionKeys.PlayerThree].GetIntegerValue(GameDataValueKeys.PlayerGrain).ShouldBe(thirdGrainCount);
      xmlGameDataReader[GameDataSectionKeys.PlayerThree].GetIntegerValue(GameDataValueKeys.PlayerLumber).ShouldBe(thirdLumberCount);
      xmlGameDataReader[GameDataSectionKeys.PlayerThree].GetIntegerValue(GameDataValueKeys.PlayerOre).ShouldBe(thirdOreCount);
      xmlGameDataReader[GameDataSectionKeys.PlayerThree].GetIntegerValue(GameDataValueKeys.PlayerWool).ShouldBe(thirdWoolCount);

      xmlGameDataReader[GameDataSectionKeys.PlayerFour].GetIdentityValue(GameDataValueKeys.PlayerId).ShouldBe(fourthId);
      xmlGameDataReader[GameDataSectionKeys.PlayerFour].GetStringValue(GameDataValueKeys.PlayerName).ShouldBe(fourthName);
      xmlGameDataReader[GameDataSectionKeys.PlayerFour].GetIntegerValue(GameDataValueKeys.PlayerBrick).ShouldBe(fourthBrickCount);
      xmlGameDataReader[GameDataSectionKeys.PlayerFour].GetIntegerValue(GameDataValueKeys.PlayerGrain).ShouldBe(fourthGrainCount);
      xmlGameDataReader[GameDataSectionKeys.PlayerFour].GetIntegerValue(GameDataValueKeys.PlayerLumber).ShouldBe(fourthLumberCount);
      xmlGameDataReader[GameDataSectionKeys.PlayerFour].GetIntegerValue(GameDataValueKeys.PlayerOre).ShouldBe(fourthOreCount);
      xmlGameDataReader[GameDataSectionKeys.PlayerFour].GetIntegerValue(GameDataValueKeys.PlayerWool).ShouldBe(fourthWoolCount);
    }

    [Test]
    [Category("XmlGameDataReader")]
    public void Load_PlayerAndInfrastructureData_GameBoardIsAsExpected()
    {
      // Arrange
      Guid firstId = Guid.NewGuid(), secondId = Guid.NewGuid(), thirdId = Guid.NewGuid(), fourthId = Guid.NewGuid();
      Int32 firstLocation = 1, secondLocation = 3, thirdLocation = 7, fourthLocation = 15;
      Int32 firstStart = 1, firstEnd = 2, secondStart = 3, secondEnd = 4;
      Int32 thirdStart = 7, thirdEnd = 8, fourthStart = 15, fourthEnd = 16;

      // Act
      var content = "<game><settlements>" +
        $"<settlement playerid=\"{firstId}\" location=\"{firstLocation}\" />" +
        $"<settlement playerid=\"{secondId}\" location=\"{secondLocation}\" />" +
        $"<settlement playerid=\"{thirdId}\" location=\"{thirdLocation}\" />" +
        $"<settlement playerid=\"{fourthId}\" location=\"{fourthLocation}\" />" +
        "</settlements><roads>" +
        $"<road playerid=\"{firstId}\" start=\"{firstStart}\" end=\"{firstEnd}\" />" +
        $"<road playerid=\"{secondId}\" start=\"{secondStart}\" end=\"{secondEnd}\" />" +
        $"<road playerid=\"{thirdId}\" start=\"{thirdStart}\" end=\"{thirdEnd}\" />" +
        $"<road playerid=\"{fourthId}\" start=\"{fourthStart}\" end=\"{fourthEnd}\" />" +
        "</roads></game>";

      XmlGameDataReader xmlGameDataReader = null;
      var contentBytes = Encoding.UTF8.GetBytes(content);
      using (var stream = new MemoryStream(contentBytes))
      {
        xmlGameDataReader = new XmlGameDataReader(stream);
      }

      // Assert
      xmlGameDataReader.ShouldNotBeNull();
      var settlementData = xmlGameDataReader[GameDataSectionKeys.Buildings].GetSections(GameDataSectionKeys.Building);
      settlementData.Length.ShouldBe(4);
      settlementData[0].GetIdentityValue(GameDataValueKeys.SettlementOwner).ShouldBe(firstId);
      settlementData[0].GetIntegerValue(GameDataValueKeys.SettlementLocation).ShouldBe(firstLocation);
      settlementData[1].GetIdentityValue(GameDataValueKeys.SettlementOwner).ShouldBe(secondId);
      settlementData[1].GetIntegerValue(GameDataValueKeys.SettlementLocation).ShouldBe(secondLocation);
      settlementData[2].GetIdentityValue(GameDataValueKeys.SettlementOwner).ShouldBe(thirdId);
      settlementData[2].GetIntegerValue(GameDataValueKeys.SettlementLocation).ShouldBe(thirdLocation);
      settlementData[3].GetIdentityValue(GameDataValueKeys.SettlementOwner).ShouldBe(fourthId);
      settlementData[3].GetIntegerValue(GameDataValueKeys.SettlementLocation).ShouldBe(fourthLocation);

      var roadData = xmlGameDataReader[GameDataSectionKeys.Roads].GetSections(GameDataSectionKeys.Road);
      roadData.Length.ShouldBe(4);
      roadData[0].GetIdentityValue(GameDataValueKeys.RoadOwner).ShouldBe(firstId);
      roadData[0].GetIntegerValue(GameDataValueKeys.RoadStart).ShouldBe(firstStart);
      roadData[0].GetIntegerValue(GameDataValueKeys.RoadEnd).ShouldBe(firstEnd);
      roadData[1].GetIdentityValue(GameDataValueKeys.RoadOwner).ShouldBe(secondId);
      roadData[1].GetIntegerValue(GameDataValueKeys.RoadStart).ShouldBe(secondStart);
      roadData[1].GetIntegerValue(GameDataValueKeys.RoadEnd).ShouldBe(secondEnd);
      roadData[2].GetIdentityValue(GameDataValueKeys.RoadOwner).ShouldBe(thirdId);
      roadData[2].GetIntegerValue(GameDataValueKeys.RoadStart).ShouldBe(thirdStart);
      roadData[2].GetIntegerValue(GameDataValueKeys.RoadEnd).ShouldBe(thirdEnd);
      roadData[3].GetIdentityValue(GameDataValueKeys.RoadOwner).ShouldBe(fourthId);
      roadData[3].GetIntegerValue(GameDataValueKeys.RoadStart).ShouldBe(fourthStart);
      roadData[3].GetIntegerValue(GameDataValueKeys.RoadEnd).ShouldBe(fourthEnd);
    }
  }
}
