
namespace Jabberwocky.SoC.Library.UnitTests.LocalGameController_Tests
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
      player.AddResources(ResourceClutch.RoadSegment * 7);

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

    [Test]
    public void BuildSettlement_InsufficientResourcesAfterBuildingSettlement_MeaningfulErrorIsReceived()
    {
      // Arrange
      MockDice mockDice = null;
      MockPlayer player;
      MockComputerPlayer firstOpponent, secondOpponent, thirdOpponent;
      var localGameController = this.CreateLocalGameControllerAndCompleteGameSetup(out mockDice, out player, out firstOpponent, out secondOpponent, out thirdOpponent);

      mockDice.AddSequence(new[] { 8u });
      player.AddResources(ResourceClutch.RoadSegment * 3);
      player.AddResources(ResourceClutch.Settlement);

      Int32 settlementBuilt = 0;
      localGameController.SettlementBuiltEvent = () => { settlementBuilt++; };

      ErrorDetails errorDetails = null;
      localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

      TurnToken turnToken = null;
      localGameController.StartPlayerTurnEvent = (TurnToken t) => { turnToken = t; };
      localGameController.StartGamePlay();
      localGameController.BuildRoadSegment(turnToken, MainRoadOneEnd, 3);
      localGameController.BuildRoadSegment(turnToken, 3, 2);
      localGameController.BuildRoadSegment(turnToken, 2, 1);
      localGameController.BuildSettlement(turnToken, 3);

      // Act
      localGameController.BuildSettlement(turnToken, 1);

      // Assert
      settlementBuilt.ShouldBe(1);
      errorDetails.ShouldNotBeNull();
      errorDetails.Message.ShouldBe("Cannot build settlement. Missing 1 brick and 1 grain and 1 lumber and 1 wool.");
    }

    [Test]
    public void BuildSettlement_OnExistingSettlementBelongingToPlayer_MeaningfulErrorIsReceived()
    {
      // Arrange
      MockDice mockDice = null;
      MockPlayer player;
      MockComputerPlayer firstOpponent, secondOpponent, thirdOpponent;
      var localGameController = this.CreateLocalGameControllerAndCompleteGameSetup(out mockDice, out player, out firstOpponent, out secondOpponent, out thirdOpponent);

      mockDice.AddSequence(new[] { 8u });
      player.AddResources(ResourceClutch.RoadSegment);
      player.AddResources(ResourceClutch.Settlement * 2);

      ErrorDetails errorDetails = null;
      localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

      TurnToken turnToken = null;
      localGameController.StartPlayerTurnEvent = (TurnToken t) => { turnToken = t; };
      localGameController.StartGamePlay();
      localGameController.BuildRoadSegment(turnToken, MainRoadOneEnd, 3);
      localGameController.BuildSettlement(turnToken, 3);

      // Act
      localGameController.BuildSettlement(turnToken, 3);

      // Assert
      errorDetails.ShouldNotBeNull();
      errorDetails.Message.ShouldBe("Cannot build settlement. Location 3 already settled by you.");
    }

    [Test]
    public void BuildSettlement_OnExistingSettlementBelongingToOpponent_MeaningfulErrorIsReceived()
    {
      // Arrange
      MockDice mockDice = null;
      MockPlayer player;
      MockComputerPlayer firstOpponent, secondOpponent, thirdOpponent;
      var localGameController = this.CreateLocalGameControllerAndCompleteGameSetup(out mockDice, out player, out firstOpponent, out secondOpponent, out thirdOpponent);

      mockDice.AddSequence(new[] { 8u });
      player.AddResources(ResourceClutch.RoadSegment * 8);
      player.AddResources(ResourceClutch.Settlement);

      ErrorDetails errorDetails = null;
      localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

      TurnToken turnToken = null;
      localGameController.StartPlayerTurnEvent = (TurnToken t) => { turnToken = t; };
      localGameController.StartGamePlay();

      var roadSegmentDetails = new UInt32[] { 4, 3, 3, 2, 2, 1, 1, 0, 0, 8, 8, 9, 9, 19, 19, 18 };
      for (var index = 0; index < roadSegmentDetails.Length; index += 2)
      {
        localGameController.BuildRoadSegment(turnToken, roadSegmentDetails[index], roadSegmentDetails[index + 1]);
      }

      // Act
      localGameController.BuildSettlement(turnToken, 18);

      // Assert
      errorDetails.ShouldNotBeNull();
      errorDetails.Message.ShouldBe("Cannot build settlement. Location 18 already settled by player '"+ FirstOpponentName + "'.");
    }

    [Test]
    public void BuildSettlement_ToCloseToAnotherSettlement_MeaningfulErrorIsReceived()
    {
      // Arrange
      MockDice mockDice = null;
      MockPlayer player;
      MockComputerPlayer firstOpponent, secondOpponent, thirdOpponent;
      var localGameController = this.CreateLocalGameControllerAndCompleteGameSetup(out mockDice, out player, out firstOpponent, out secondOpponent, out thirdOpponent);

      mockDice.AddSequence(new[] { 8u });
      player.AddResources(ResourceClutch.RoadSegment * 7);
      player.AddResources(ResourceClutch.Settlement);

      ErrorDetails errorDetails = null;
      localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

      TurnToken turnToken = null;
      localGameController.StartPlayerTurnEvent = (TurnToken t) => { turnToken = t; };
      localGameController.StartGamePlay();

      // Act
      localGameController.BuildSettlement(turnToken, 4);

      // Assert
      errorDetails.ShouldNotBeNull();
      errorDetails.Message.ShouldBe("Cannot build settlement. Too close to own settlement at location 12.");
    }

    [Test]
    public void BuildSettlement_ToCloseToOpponentSettlement_MeaningfulErrorIsReceived()
    {
      // Arrange
      MockDice mockDice = null;
      MockPlayer player;
      MockComputerPlayer firstOpponent, secondOpponent, thirdOpponent;
      var localGameController = this.CreateLocalGameControllerAndCompleteGameSetup(out mockDice, out player, out firstOpponent, out secondOpponent, out thirdOpponent);

      mockDice.AddSequence(new[] { 8u });
      player.AddResources(ResourceClutch.RoadSegment * 7);
      player.AddResources(ResourceClutch.Settlement);

      ErrorDetails errorDetails = null;
      localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

      TurnToken turnToken = null;
      localGameController.StartPlayerTurnEvent = (TurnToken t) => { turnToken = t; };
      localGameController.StartGamePlay();

      var roadSegmentDetails = new UInt32[] { 4, 3, 3, 2, 2, 1, 1, 0, 0, 8, 8, 9, 9, 19 };
      for (var index = 0; index < roadSegmentDetails.Length; index += 2)
      {
        localGameController.BuildRoadSegment(turnToken, roadSegmentDetails[index], roadSegmentDetails[index + 1]);
      }

      // Act
      localGameController.BuildSettlement(turnToken, 19);

      // Assert
      errorDetails.ShouldNotBeNull();
      errorDetails.Message.ShouldBe("Cannot build settlement. Too close to player '" + FirstOpponentName + "' at location 18.");
    }

    [Test]
    public void BuildSettlement_OffBoard_MeaningfulErrorIsReceived()
    {
      // Arrange
      MockDice mockDice = null;
      MockPlayer player;
      MockComputerPlayer firstOpponent, secondOpponent, thirdOpponent;
      var localGameController = this.CreateLocalGameControllerAndCompleteGameSetup(out mockDice, out player, out firstOpponent, out secondOpponent, out thirdOpponent);

      mockDice.AddSequence(new[] { 8u });
      player.AddResources(ResourceClutch.Settlement);

      ErrorDetails errorDetails = null;
      localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

      TurnToken turnToken = null;
      localGameController.StartPlayerTurnEvent = (TurnToken t) => { turnToken = t; };
      localGameController.StartGamePlay();

      // Act
      localGameController.BuildSettlement(turnToken, 54);

      // Assert
      errorDetails.ShouldNotBeNull();
      errorDetails.Message.ShouldBe("Cannot build settlement: Location 54 is outside of board range (0 - 53).");
    }

    [Test]
    public void BuildSettlement_NotConnectedToExistingRoad_MeaningfulErrorIsReceived()
    {
      // Arrange
      MockDice mockDice = null;
      MockPlayer player;
      MockComputerPlayer firstOpponent, secondOpponent, thirdOpponent;
      var localGameController = this.CreateLocalGameControllerAndCompleteGameSetup(out mockDice, out player, out firstOpponent, out secondOpponent, out thirdOpponent);

      mockDice.AddSequence(new[] { 8u });
      player.AddResources(ResourceClutch.RoadSegment);
      player.AddResources(ResourceClutch.Settlement);

      ErrorDetails errorDetails = null;
      localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

      TurnToken turnToken = null;
      localGameController.StartPlayerTurnEvent = (TurnToken t) => { turnToken = t; };
      localGameController.StartGamePlay();
      localGameController.BuildRoadSegment(turnToken, 4, 3);

      // Act
      localGameController.BuildSettlement(turnToken, 2);

      // Assert
      errorDetails.ShouldNotBeNull();
      errorDetails.Message.ShouldBe("Cannot build settlement: Location 2 not connected to existing road.");
    }

    [Test]
    public void BuildSettlement_TurnTokenNotCorrect_MeaningfulErrorIsReceived()
    {
      // Arrange
      MockDice mockDice = null;
      MockPlayer player;
      MockComputerPlayer firstOpponent, secondOpponent, thirdOpponent;
      var localGameController = this.CreateLocalGameControllerAndCompleteGameSetup(out mockDice, out player, out firstOpponent, out secondOpponent, out thirdOpponent);

      mockDice.AddSequence(new[] { 8u });
      player.AddResources(ResourceClutch.RoadSegment);
      player.AddResources(ResourceClutch.Settlement);

      ErrorDetails errorDetails = null;
      localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

      TurnToken turnToken = null;
      localGameController.StartPlayerTurnEvent = (TurnToken t) => { turnToken = t; };
      localGameController.StartGamePlay();
      localGameController.BuildRoadSegment(turnToken, 4, 3);

      // Act
      localGameController.BuildSettlement(new TurnToken(), 3);

      // Assert
      errorDetails.ShouldNotBeNull();
      errorDetails.Message.ShouldBe("Turn token not recognised.");
    }
    #endregion 
  }
}
 