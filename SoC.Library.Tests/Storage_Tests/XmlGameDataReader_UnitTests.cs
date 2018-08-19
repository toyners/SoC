
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
    private Guid firstId = Guid.NewGuid(), secondId = Guid.NewGuid(), thirdId = Guid.NewGuid(), fourthId = Guid.NewGuid();
    private readonly String firstName = "Name_1", secondName = "Name_2", thirdName = "Name_3", fourthName = "Name_4";
    private readonly Int32 firstBrickCount = 1, firstGrainCount = 2, firstLumberCount = 3, firstOreCount = 4, firstWoolCount = 5;
    private readonly Int32 secondBrickCount = 1, secondGrainCount = 2, secondLumberCount = 3, secondOreCount = 4, secondWoolCount = 5;
    private readonly Int32 thirdBrickCount = 1, thirdGrainCount = 2, thirdLumberCount = 3, thirdOreCount = 4, thirdWoolCount = 5;
    private readonly Int32 fourthBrickCount = 1, fourthGrainCount = 2, fourthLumberCount = 3, fourthOreCount = 4, fourthWoolCount = 5;
    private readonly Int32 firstLocation = 1, secondLocation = 3, thirdLocation = 7, fourthLocation = 15;
    private readonly Int32 firstStart = 1, firstEnd = 2, secondStart = 3, secondEnd = 4;
    private readonly Int32 thirdStart = 7, thirdEnd = 8, fourthStart = 15, fourthEnd = 16;

    [Test]
    [Category("XmlGameDataReader")]
    public void Load_HexDataOnly_DataLoadedCorrectly()
    {
      // Act
      XmlGameDataReader xmlGameDataReader;
      using (var stream = this.GetXmlStream())
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
    public void Load_PlayerDataOnly_DataLoadedCorrectly()
    {
      // Act
      XmlGameDataReader xmlGameDataReader;
      using (var stream = this.GetXmlStream())
      {
        xmlGameDataReader = new XmlGameDataReader(stream);
      }

      // Assert
      xmlGameDataReader.ShouldNotBeNull();
      xmlGameDataReader[GameDataSectionKeys.PlayerOne].GetIdentityValue(GameDataValueKeys.PlayerId).ShouldBe(this.firstId);
      xmlGameDataReader[GameDataSectionKeys.PlayerOne].GetStringValue(GameDataValueKeys.PlayerName).ShouldBe(this.firstName);
      xmlGameDataReader[GameDataSectionKeys.PlayerOne].GetIntegerValue(GameDataValueKeys.PlayerBrick).ShouldBe(this.firstBrickCount);
      xmlGameDataReader[GameDataSectionKeys.PlayerOne].GetIntegerValue(GameDataValueKeys.PlayerGrain).ShouldBe(this.firstGrainCount);
      xmlGameDataReader[GameDataSectionKeys.PlayerOne].GetIntegerValue(GameDataValueKeys.PlayerLumber).ShouldBe(this.firstLumberCount);
      xmlGameDataReader[GameDataSectionKeys.PlayerOne].GetIntegerValue(GameDataValueKeys.PlayerOre).ShouldBe(this.firstOreCount);
      xmlGameDataReader[GameDataSectionKeys.PlayerOne].GetIntegerValue(GameDataValueKeys.PlayerWool).ShouldBe(this.firstWoolCount);

      xmlGameDataReader[GameDataSectionKeys.PlayerTwo].GetIdentityValue(GameDataValueKeys.PlayerId).ShouldBe(this.secondId);
      xmlGameDataReader[GameDataSectionKeys.PlayerTwo].GetStringValue(GameDataValueKeys.PlayerName).ShouldBe(this.secondName);
      xmlGameDataReader[GameDataSectionKeys.PlayerTwo].GetIntegerValue(GameDataValueKeys.PlayerBrick).ShouldBe(this.secondBrickCount);
      xmlGameDataReader[GameDataSectionKeys.PlayerTwo].GetIntegerValue(GameDataValueKeys.PlayerGrain).ShouldBe(this.secondGrainCount);
      xmlGameDataReader[GameDataSectionKeys.PlayerTwo].GetIntegerValue(GameDataValueKeys.PlayerLumber).ShouldBe(this.secondLumberCount);
      xmlGameDataReader[GameDataSectionKeys.PlayerTwo].GetIntegerValue(GameDataValueKeys.PlayerOre).ShouldBe(this.secondOreCount);
      xmlGameDataReader[GameDataSectionKeys.PlayerTwo].GetIntegerValue(GameDataValueKeys.PlayerWool).ShouldBe(this.secondWoolCount);

      xmlGameDataReader[GameDataSectionKeys.PlayerThree].GetIdentityValue(GameDataValueKeys.PlayerId).ShouldBe(this.thirdId);
      xmlGameDataReader[GameDataSectionKeys.PlayerThree].GetStringValue(GameDataValueKeys.PlayerName).ShouldBe(this.thirdName);
      xmlGameDataReader[GameDataSectionKeys.PlayerThree].GetIntegerValue(GameDataValueKeys.PlayerBrick).ShouldBe(this.thirdBrickCount);
      xmlGameDataReader[GameDataSectionKeys.PlayerThree].GetIntegerValue(GameDataValueKeys.PlayerGrain).ShouldBe(this.thirdGrainCount);
      xmlGameDataReader[GameDataSectionKeys.PlayerThree].GetIntegerValue(GameDataValueKeys.PlayerLumber).ShouldBe(this.thirdLumberCount);
      xmlGameDataReader[GameDataSectionKeys.PlayerThree].GetIntegerValue(GameDataValueKeys.PlayerOre).ShouldBe(this.thirdOreCount);
      xmlGameDataReader[GameDataSectionKeys.PlayerThree].GetIntegerValue(GameDataValueKeys.PlayerWool).ShouldBe(this.thirdWoolCount);

      xmlGameDataReader[GameDataSectionKeys.PlayerFour].GetIdentityValue(GameDataValueKeys.PlayerId).ShouldBe(this.fourthId);
      xmlGameDataReader[GameDataSectionKeys.PlayerFour].GetStringValue(GameDataValueKeys.PlayerName).ShouldBe(this.fourthName);
      xmlGameDataReader[GameDataSectionKeys.PlayerFour].GetIntegerValue(GameDataValueKeys.PlayerBrick).ShouldBe(this.fourthBrickCount);
      xmlGameDataReader[GameDataSectionKeys.PlayerFour].GetIntegerValue(GameDataValueKeys.PlayerGrain).ShouldBe(this.fourthGrainCount);
      xmlGameDataReader[GameDataSectionKeys.PlayerFour].GetIntegerValue(GameDataValueKeys.PlayerLumber).ShouldBe(this.fourthLumberCount);
      xmlGameDataReader[GameDataSectionKeys.PlayerFour].GetIntegerValue(GameDataValueKeys.PlayerOre).ShouldBe(this.fourthOreCount);
      xmlGameDataReader[GameDataSectionKeys.PlayerFour].GetIntegerValue(GameDataValueKeys.PlayerWool).ShouldBe(this.fourthWoolCount);
    }

    [Test]
    [Category("XmlGameDataReader")]
    public void Load_PlayerAndInfrastructureData_GameBoardIsAsExpected()
    {
      // Act
      XmlGameDataReader xmlGameDataReader;
      using (var stream = this.GetXmlStream())
      {
        xmlGameDataReader = new XmlGameDataReader(stream);
      }

      // Assert
      xmlGameDataReader.ShouldNotBeNull();
      var settlementData = xmlGameDataReader[GameDataSectionKeys.Buildings].GetSections(GameDataSectionKeys.Building);
      settlementData.Length.ShouldBe(4);
      settlementData[0].GetIdentityValue(GameDataValueKeys.SettlementOwner).ShouldBe(this.firstId);
      settlementData[0].GetIntegerValue(GameDataValueKeys.SettlementLocation).ShouldBe(this.firstLocation);
      settlementData[1].GetIdentityValue(GameDataValueKeys.SettlementOwner).ShouldBe(this.secondId);
      settlementData[1].GetIntegerValue(GameDataValueKeys.SettlementLocation).ShouldBe(this.secondLocation);
      settlementData[2].GetIdentityValue(GameDataValueKeys.SettlementOwner).ShouldBe(this.thirdId);
      settlementData[2].GetIntegerValue(GameDataValueKeys.SettlementLocation).ShouldBe(this.thirdLocation);
      settlementData[3].GetIdentityValue(GameDataValueKeys.SettlementOwner).ShouldBe(this.fourthId);
      settlementData[3].GetIntegerValue(GameDataValueKeys.SettlementLocation).ShouldBe(this.fourthLocation);

      var roadData = xmlGameDataReader[GameDataSectionKeys.Roads].GetSections(GameDataSectionKeys.Road);
      roadData.Length.ShouldBe(4);
      roadData[0].GetIdentityValue(GameDataValueKeys.RoadOwner).ShouldBe(this.firstId);
      roadData[0].GetIntegerValue(GameDataValueKeys.RoadStart).ShouldBe(this.firstStart);
      roadData[0].GetIntegerValue(GameDataValueKeys.RoadEnd).ShouldBe(this.firstEnd);
      roadData[1].GetIdentityValue(GameDataValueKeys.RoadOwner).ShouldBe(this.secondId);
      roadData[1].GetIntegerValue(GameDataValueKeys.RoadStart).ShouldBe(this.secondStart);
      roadData[1].GetIntegerValue(GameDataValueKeys.RoadEnd).ShouldBe(this.secondEnd);
      roadData[2].GetIdentityValue(GameDataValueKeys.RoadOwner).ShouldBe(this.thirdId);
      roadData[2].GetIntegerValue(GameDataValueKeys.RoadStart).ShouldBe(this.thirdStart);
      roadData[2].GetIntegerValue(GameDataValueKeys.RoadEnd).ShouldBe(this.thirdEnd);
      roadData[3].GetIdentityValue(GameDataValueKeys.RoadOwner).ShouldBe(this.fourthId);
      roadData[3].GetIntegerValue(GameDataValueKeys.RoadStart).ShouldBe(this.fourthStart);
      roadData[3].GetIntegerValue(GameDataValueKeys.RoadEnd).ShouldBe(this.fourthEnd);
    }

    private MemoryStream GetXmlStream()
    {
      var content = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><game><board><hexes>" +
        "<resources>glbglogob gwwwlwlbo</resources>" +
        "<production>9,8,5,12,11,3,6,10,6,0,4,11,2,4,3,5,9,10,8</production>" +
        "</hexes></board>" +
        "<players> " +
        $"<playerOne id=\"{this.firstId}\" name=\"{this.firstName}\" brick=\"{this.firstBrickCount}\" grain=\"{this.firstGrainCount}\" lumber=\"{this.firstLumberCount}\" ore=\"{this.firstOreCount}\" wool=\"{this.firstWoolCount}\" />" +
        $"<playerTwo id=\"{this.secondId}\" name=\"{this.secondName}\" brick=\"{this.secondBrickCount}\" grain=\"{this.secondGrainCount}\" lumber=\"{this.secondLumberCount}\" ore=\"{this.secondOreCount}\" wool=\"{this.secondWoolCount}\" />" +
        $"<playerThree id=\"{this.thirdId}\" name=\"{this.thirdName}\" brick=\"{this.thirdBrickCount}\" grain=\"{this.thirdGrainCount}\" lumber=\"{this.thirdLumberCount}\" ore=\"{this.thirdOreCount}\" wool=\"{this.thirdWoolCount}\" />" +
        $"<playerFour id=\"{this.fourthId}\" name=\"{this.fourthName}\" brick=\"{this.fourthBrickCount}\" grain=\"{this.fourthGrainCount}\" lumber=\"{this.fourthLumberCount}\" ore=\"{this.fourthOreCount}\" wool=\"{this.fourthWoolCount}\" />" +
        "</players>" +
        "<settlements>" +
        $"<settlement playerid=\"{this.firstId}\" location=\"{this.firstLocation}\" />" +
        $"<settlement playerid=\"{this.secondId}\" location=\"{this.secondLocation}\" />" +
        $"<settlement playerid=\"{this.thirdId}\" location=\"{this.thirdLocation}\" />" +
        $"<settlement playerid=\"{this.fourthId}\" location=\"{this.fourthLocation}\" />" +
        "</settlements><roads>" +
        $"<road playerid=\"{this.firstId}\" start=\"{this.firstStart}\" end=\"{this.firstEnd}\" />" +
        $"<road playerid=\"{this.secondId}\" start=\"{this.secondStart}\" end=\"{this.secondEnd}\" />" +
        $"<road playerid=\"{this.thirdId}\" start=\"{this.thirdStart}\" end=\"{this.thirdEnd}\" />" +
        $"<road playerid=\"{this.fourthId}\" start=\"{this.fourthStart}\" end=\"{this.fourthEnd}\" />" +
        "</roads>" +
        "</game>";

      var contentBytes = Encoding.UTF8.GetBytes(content);
      return new MemoryStream(contentBytes);
    }
  }
}
