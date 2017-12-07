
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

    [Test]
    public void BuildSettlement_AllSettlementsAreBuilt_MeaningfulErrorIsReceived()
    {
      // Arrange
      MockDice mockDice = null;
      MockPlayer player;
      MockComputerPlayer firstOpponent, secondOpponent, thirdOpponent;
      var localGameController = this.CreateLocalGameControllerAndCompleteGameSetup(out mockDice, out player, out firstOpponent, out secondOpponent, out thirdOpponent);
      mockDice.AddSequence(new[] { 8u });
      player.AddResources(ResourceClutch.Settlement * 3);
      player.AddResources(ResourceClutch.RoadSegment * 5);

      ErrorDetails errorDetails = null;
      localGameController.ErrorRaisedEvent = (ErrorDetails e) =>
      {
        if (errorDetails != null)
        {
          // Ensure that the error details are only received once.
          throw new Exception("Already received error details");
        }

        errorDetails = e;
      };

      TurnToken turnToken = null;
      localGameController.StartPlayerTurnEvent = (TurnToken t) => { turnToken = t; };
      localGameController.StartGamePlay();

      var roadSegmentDetails = new UInt32[] { 4, 3, 3, 2, 2, 1, 1, 0, 0, 8, 8, 7, 7, 17 };
      for (var index = 0; index < roadSegmentDetails.Length; index += 2)
      {
        localGameController.BuildRoadSegment(turnToken, roadSegmentDetails[index], roadSegmentDetails[index + 1]);
      }

      localGameController.BuildSettlement(turnToken, 3);
      localGameController.BuildSettlement(turnToken, 1);
      localGameController.BuildSettlement(turnToken, 8);

      // Act
      localGameController.BuildSettlement(turnToken, 17);

      // Assert
      errorDetails.ShouldNotBeNull();
      errorDetails.Message.ShouldBe("Cannot build settlement. All settlements already built.");
    }
    #endregion 
  }
}
