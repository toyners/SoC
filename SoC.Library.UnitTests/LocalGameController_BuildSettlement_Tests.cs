
namespace Jabberwocky.SoC.Library.UnitTests
{
  using System;
  using NUnit.Framework;
  using Shouldly;

  [TestFixture]
  [Category("All")]
  [Category("LocalGameController")]
  [Category("LocalGameController.BuildSettlement")]
  public class LocalGameController_BuildSettlement_Tests : LocalGameControllerTestBase
  {
    #region Methods
    [Test]
    public void BuildSettlement_ValidScenario_SettlementBuiltEventRaised()
    {
      // Arrange
      MockDice mockDice = null;
      MockPlayer player;
      MockComputerPlayer firstOpponent, secondOpponent, thirdOpponent;
      var localGameController = this.CreateLocalGameControllerAndCompleteGameSetup(out mockDice, out player, out firstOpponent, out secondOpponent, out thirdOpponent);

      mockDice.AddSequence(new[] { 8u });
      player.AddResources(ResourceClutch.RoadSegment); // Need resources to build the precursor road
      player.AddResources(ResourceClutch.Settlement);

      Boolean settlementBuilt = false;
      localGameController.SettlementBuiltEvent = () => { settlementBuilt = true; };

      TurnToken turnToken = null;
      localGameController.StartPlayerTurnEvent = (TurnToken t) => { turnToken = t; };
      localGameController.StartGamePlay();
      localGameController.BuildRoadSegment(turnToken, MainRoadOneEnd, 3);

      // Act
      localGameController.BuildSettlement(turnToken, 3);

      // Assert
      settlementBuilt.ShouldBeTrue();
    }

    [Test]
    [TestCase(0, 1, 1, 0, 1, "Cannot build settlement. Missing 1 brick.")] // Missing brick
    [TestCase(1, 0, 1, 0, 1, "Cannot build settlement. Missing 1 grain.")] // Missing grain
    [TestCase(1, 1, 0, 0, 1, "Cannot build settlement. Missing 1 lumber.")] // Missing lumber
    [TestCase(1, 1, 1, 0, 0, "Cannot build settlement. Missing 1 wool.")] // Missing wool
    [TestCase(0, 0, 0, 0, 0, "Cannot build settlement. Missing 1 brick and 1 grain and 1 lumber and 1 wool.")] // Missing all
    [TestCase(0, 1, 1, 0, 0, "Cannot build settlement. Missing 1 brick and 1 wool.")] // Missing brick and wool
    [TestCase(1, 0, 0, 0, 1, "Cannot build settlement. Missing 1 grain and 1 lumber.")] // Missing grain and lumber
    public void BuildSettlement_InsufficientResources_MeaningfulErrorIsReceived(Int32 brickCount, Int32 grainCount, Int32 lumberCount, Int32 oreCount, Int32 woolCount, String expectedMessage)
    {
      // Arrange
      MockDice mockDice = null;
      MockPlayer player;
      MockComputerPlayer firstOpponent, secondOpponent, thirdOpponent;
      var localGameController = this.CreateLocalGameControllerAndCompleteGameSetup(out mockDice, out player, out firstOpponent, out secondOpponent, out thirdOpponent);

      mockDice.AddSequence(new[] { 8u });
      player.AddResources(ResourceClutch.RoadSegment); // Need resources to build the precursor road
      player.AddResources(new ResourceClutch(brickCount, grainCount, lumberCount, oreCount, woolCount));

      Boolean settlementBuilt = false;
      localGameController.SettlementBuiltEvent = () => { settlementBuilt = true; };

      ErrorDetails errorDetails = null;
      localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

      TurnToken turnToken = null;
      localGameController.StartPlayerTurnEvent = (TurnToken t) => { turnToken = t; };
      localGameController.StartGamePlay();
      localGameController.BuildRoadSegment(turnToken, MainRoadOneEnd, 3);

      // Act
      localGameController.BuildSettlement(turnToken, 3);

      // Assert
      settlementBuilt.ShouldBeFalse();
      errorDetails.ShouldNotBeNull();
      errorDetails.Message.ShouldBe(expectedMessage);
    }
    #endregion 
  }
}
