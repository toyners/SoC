
namespace Jabberwocky.SoC.Library.UnitTests.LocalGameController_Tests
{
  using System;
  using NUnit.Framework;
  using Shouldly;

  [TestFixture]
  [Category("All")]
  [Category("LocalGameController")]
  [Category("LocalGameController.BuildCity")]
  public class LocalGameController_BuildCity_Tests : LocalGameControllerTestBase
  {
    [Test]
    public void BuildCity_OffBoard_MeaningfulErrorIsReceived()
    {
      // Arrange
      MockDice mockDice = null;
      MockPlayer player;
      MockComputerPlayer firstOpponent, secondOpponent, thirdOpponent;
      var localGameController = this.CreateLocalGameControllerAndCompleteGameSetup(out mockDice, out player, out firstOpponent, out secondOpponent, out thirdOpponent);

      mockDice.AddSequence(new[] { 8u });
      player.AddResources(ResourceClutch.RoadSegment); // Need resources to build the precursor road
      player.AddResources(ResourceClutch.Settlement);
      player.AddResources(ResourceClutch.City);

      Boolean cityBuilt = false;
      localGameController.CityBuiltEvent = () => { cityBuilt = true; };

      TurnToken turnToken = null;
      localGameController.StartPlayerTurnEvent = (TurnToken t) => { turnToken = t; };

      ErrorDetails errorDetails = null;
      localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

      localGameController.StartGamePlay();
      localGameController.BuildRoadSegment(turnToken, MainRoadOneEnd, 3);
      localGameController.BuildSettlement(turnToken, 3);

      // Act
      localGameController.BuildCity(turnToken, 3);

      // Assert
      cityBuilt.ShouldBeFalse();
      errorDetails.ShouldNotBeNull();
      errorDetails.Message.ShouldBe("");
    }

    [Test]
    public void BuildCity_OnExistingSettlementBelongingToOpponent_MeaningfulErrorIsReceived()
    {
      throw new NotImplementedException();
    }

    [Test]
    public void BuildCity_OnExistingSettlementBelongingToPlayer_CityBuiltEventRaised()
    {
      throw new NotImplementedException();
    }

    [Test]
    [TestCase(0, 0, "")]
    [TestCase(1, 1, "")]
    [TestCase(1, 2, "")]
    [TestCase(2, 0, "")]
    [TestCase(0, 3, "")]
    public void BuildCity_InsufficientResources_MeaningfulErrorIsReceived(Int32 grainCount, Int32 oreCount, String expectedMessage)
    {
      throw new NotImplementedException();
    }

    [Test]
    public void BuildCity_AllCitiesAreBuilt_MeaningfulErrorIsReceived()
    {
      throw new NotImplementedException();
    }

    [Test]
    public void BuildCity_OnExistingCityBelongingToPlayer_MeaningfulErrorIsReceived()
    {
      throw new NotImplementedException();
    }

    [Test]
    public void BuildCity_OnExistingCityBelongingToOpponent_MeaningfulErrorIsReceived()
    {
      throw new NotImplementedException();
    }

    [Test]
    [TestCase(0)] // Empty location
    [TestCase(1)] // Location on road with no settlement
    public void BuildCity_OnLocationThatIsNotSettlement_MeaningfulErrorIsReceived(UInt32 location)
    {
      throw new NotImplementedException();
    }
  }
}
