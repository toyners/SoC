
namespace Jabberwocky.SoC.Library.UnitTests.LocalGameController_Tests
{
  using System;
  using NUnit.Framework;
  using Shouldly;

  [TestFixture]
  [Category("All")]
  [Category("LocalGameController")]
  [Category("LocalGameController.BuildRoadSegment")]
  public class LocalGameController_BuildRoadSegment_Tests : LocalGameControllerTestBase
  {
    #region Methods
    [Test]
    public void BuildRoadSegment_ValidScenario_RoadSegmentBuiltEventRaised()
    {
      // Arrange
      MockDice mockDice = null;
      MockPlayer player;
      MockComputerPlayer firstOpponent, secondOpponent, thirdOpponent;
      var localGameController = this.CreateLocalGameControllerAndCompleteGameSetup(out mockDice, out player, out firstOpponent, out secondOpponent, out thirdOpponent);
      mockDice.AddSequence(new[] { 8u });
      player.AddResources(new ResourceClutch(1, 0, 1, 0, 0));

      var buildCompleted = false;
      localGameController.RoadSegmentBuiltEvent = () => { buildCompleted = true; };

      TurnToken turnToken = null;
      localGameController.StartPlayerTurnEvent = (TurnToken t) => { turnToken = t; };
      localGameController.StartGamePlay();

      // Act
      localGameController.BuildRoadSegment(turnToken, 4u, 3u);

      // Assert
      buildCompleted.ShouldBeTrue();
    }

    [Test]
    public void BuildRoadSegment_RequiredResourcesAvailable_PlayerResourcesUpdated()
    {
      // Arrange
      MockDice mockDice = null;
      MockPlayer player;
      MockComputerPlayer firstOpponent, secondOpponent, thirdOpponent;
      var localGameController = this.CreateLocalGameControllerAndCompleteGameSetup(out mockDice, out player, out firstOpponent, out secondOpponent, out thirdOpponent);
      mockDice.AddSequence(new[] { 8u });
      player.AddResources(new ResourceClutch(1, 0, 1, 0, 0));

      TurnToken turnToken = null;
      localGameController.StartPlayerTurnEvent = (TurnToken t) => { turnToken = t; };
      localGameController.StartGamePlay();

      // Act
      localGameController.BuildRoadSegment(turnToken, 4u, 3u);

      // Assert
      player.BrickCount.ShouldBe(0);
      player.LumberCount.ShouldBe(0);
    }

    [Test]
    public void BuildRoadSegment_MainPlayerBuildsLongestRoad_LongestRoadEventRaised()
    {
      // Arrange
      MockDice mockDice = null;
      MockPlayer player;
      MockComputerPlayer firstOpponent, secondOpponent, thirdOpponent;
      var localGameController = this.CreateLocalGameControllerAndCompleteGameSetup(out mockDice, out player, out firstOpponent, out secondOpponent, out thirdOpponent);

      mockDice.AddSequence(new[] { 8u });
      player.AddResources(new ResourceClutch(5, 0, 5, 0, 0));

      Guid playerId = Guid.Empty;
      localGameController.LongestRoadBuiltEvent = (Guid pid) => { playerId = pid; };

      TurnToken turnToken = null;
      localGameController.StartPlayerTurnEvent = (TurnToken t) => { turnToken = t; };
      localGameController.StartGamePlay();

      // Act
      localGameController.BuildRoadSegment(turnToken, 4, 3);
      localGameController.BuildRoadSegment(turnToken, 3, 2);
      localGameController.BuildRoadSegment(turnToken, 2, 1);
      localGameController.BuildRoadSegment(turnToken, 1, 0);

      // Assert
      playerId.ShouldBe(player.Id);
    }

    [Test]
    public void BuildRoadSegment_SubsequentLongestRoadBuiltDuringOpponentsTurn_LongestRoadEventRaised()
    {
      // Arrange
      MockDice mockDice = null;
      MockPlayer player;
      MockComputerPlayer firstOpponent, secondOpponent, thirdOpponent;
      var localGameController = this.CreateLocalGameControllerAndCompleteGameSetup(out mockDice, out player, out firstOpponent, out secondOpponent, out thirdOpponent);

      mockDice.AddSequence(new[] { 8u });
      player.AddResources(new ResourceClutch(5, 0, 5, 0, 0));

      firstOpponent.AddResources(new ResourceClutch(6, 0, 6, 0, 0));
      firstOpponent.AddRoadChoices(new UInt32[] { 18, 19, 19, 9, 9, 10, 10, 11, 11, 21 });

      Guid playerId = Guid.Empty;
      localGameController.LongestRoadBuiltEvent = (Guid pid) => { playerId = pid; };

      TurnToken turnToken = null;
      localGameController.StartPlayerTurnEvent = (TurnToken t) => { turnToken = t; };
      localGameController.StartGamePlay();

      localGameController.BuildRoadSegment(turnToken, 4, 3);
      localGameController.BuildRoadSegment(turnToken, 3, 2);
      localGameController.BuildRoadSegment(turnToken, 2, 1);
      localGameController.BuildRoadSegment(turnToken, 1, 0);

      // Act - Opponent builds longer road.
      localGameController.EndTurn(turnToken);

      // Assert
      playerId.ShouldBe(firstOpponent.Id);
    }

    [Test]
    [TestCase(new UInt32[] { 4, 3 })]
    [TestCase(new UInt32[] { 4, 3, 3, 2 })]
    [TestCase(new UInt32[] { 4, 3, 3, 2, 2, 1 })]
    public void BuildRoadSegment_AddToRoadShorterThanFiveSegments_LongestRoadEventNotRaised(UInt32[] roadLocations)
    {
      // Arrange
      MockDice mockDice = null;
      MockPlayer player;
      MockComputerPlayer firstOpponent, secondOpponent, thirdOpponent;
      var localGameController = this.CreateLocalGameControllerAndCompleteGameSetup(out mockDice, out player, out firstOpponent, out secondOpponent, out thirdOpponent);

      mockDice.AddSequence(new[] { 8u });
      var roadCount = roadLocations.Length / 2;
      var brickCount = roadCount;
      var lumberCount = roadCount;
      player.AddResources(new ResourceClutch(brickCount, 0, lumberCount, 0, 0));

      Boolean longestRoadBuiltEventRaised = false;
      localGameController.LongestRoadBuiltEvent = (Guid pid) => { longestRoadBuiltEventRaised = true; };

      TurnToken turnToken = null;
      localGameController.StartPlayerTurnEvent = (TurnToken t) => { turnToken = t; };
      localGameController.StartGamePlay();

      // Act
      for (var index = 0; index < roadLocations.Length; index += 2)
      {
        localGameController.BuildRoadSegment(turnToken, roadLocations[index], roadLocations[index + 1]);
      }

      // Assert
      longestRoadBuiltEventRaised.ShouldBeFalse();
    }

    [Test]
    public void BuildRoadSegment_OnExistingRoadSegment_MeaningfulErrorIsReceived()
    {
      // Arrange
      MockDice mockDice = null;
      MockPlayer player;
      MockComputerPlayer firstOpponent, secondOpponent, thirdOpponent;
      var localGameController = this.CreateLocalGameControllerAndCompleteGameSetup(out mockDice, out player, out firstOpponent, out secondOpponent, out thirdOpponent);

      mockDice.AddSequence(new[] { 8u });
      player.AddResources(new ResourceClutch(1, 0, 1, 0, 0));

      localGameController.LongestRoadBuiltEvent = (Guid pid) => { throw new NotImplementedException(); };
      ErrorDetails errorDetails = null;
      localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

      TurnToken turnToken = null;
      localGameController.StartPlayerTurnEvent = (TurnToken t) => { turnToken = t; };
      localGameController.StartGamePlay();

      // Act
      localGameController.BuildRoadSegment(turnToken, MainRoadOneEnd, MainSettlementOneLocation);

      // Assert
      errorDetails.ShouldNotBeNull();
      errorDetails.Message.ShouldBe("Cannot place road segment because road segment already exists.");
    }

    [Test]
    public void BuildRoadSegment_NotConnectedToExistingInfrastructure_MeaningfulErrorIsReceived()
    {
      // Arrange
      MockDice mockDice = null;
      MockPlayer player;
      MockComputerPlayer firstOpponent, secondOpponent, thirdOpponent;
      var localGameController = this.CreateLocalGameControllerAndCompleteGameSetup(out mockDice, out player, out firstOpponent, out secondOpponent, out thirdOpponent);

      mockDice.AddSequence(new[] { 8u });
      player.AddResources(new ResourceClutch(1, 0, 1, 0, 0));

      localGameController.LongestRoadBuiltEvent = (Guid pid) => { throw new NotImplementedException(); };
      ErrorDetails errorDetails = null;
      localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

      TurnToken turnToken = null;
      localGameController.StartPlayerTurnEvent = (TurnToken t) => { turnToken = t; };
      localGameController.StartGamePlay();

      // Act
      localGameController.BuildRoadSegment(turnToken, 0, 1);

      // Assert
      errorDetails.ShouldNotBeNull();
      errorDetails.Message.ShouldBe("Cannot place road segment because it is not connected to an existing road segment.");
    }

    [Test]
    public void BuildRoadSegment_OffBoard_MeaningfulErrorIsReceived()
    {
      // Arrange
      MockDice mockDice = null;
      MockPlayer player;
      MockComputerPlayer firstOpponent, secondOpponent, thirdOpponent;
      var localGameController = this.CreateLocalGameControllerAndCompleteGameSetup(out mockDice, out player, out firstOpponent, out secondOpponent, out thirdOpponent);

      mockDice.AddSequence(new[] { 8u });
      player.AddResources(new ResourceClutch(1, 0, 1, 0, 0));

      localGameController.LongestRoadBuiltEvent = (Guid pid) => { throw new NotImplementedException(); };
      ErrorDetails errorDetails = null;
      localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

      TurnToken turnToken = null;
      localGameController.StartPlayerTurnEvent = (TurnToken t) => { turnToken = t; };
      localGameController.StartGamePlay();

      // Act
      localGameController.BuildRoadSegment(turnToken, 100, 101);

      // Assert
      errorDetails.ShouldNotBeNull();
      errorDetails.Message.ShouldBe("Cannot place road segment because board location is not valid.");
    }

    [Test]
    public void BuildRoadSegment_NoDirectConnection_MeaningfulErrorIsReceived()
    {
      // Arrange
      MockDice mockDice = null;
      MockPlayer player;
      MockComputerPlayer firstOpponent, secondOpponent, thirdOpponent;
      var localGameController = this.CreateLocalGameControllerAndCompleteGameSetup(out mockDice, out player, out firstOpponent, out secondOpponent, out thirdOpponent);

      mockDice.AddSequence(new[] { 8u });
      player.AddResources(new ResourceClutch(1, 0, 1, 0, 0));

      localGameController.LongestRoadBuiltEvent = (Guid pid) => { throw new NotImplementedException(); };
      ErrorDetails errorDetails = null;
      localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

      TurnToken turnToken = null;
      localGameController.StartPlayerTurnEvent = (TurnToken t) => { turnToken = t; };
      localGameController.StartGamePlay();

      // Act
      localGameController.BuildRoadSegment(turnToken, MainRoadOneEnd, 0);

      // Assert
      errorDetails.ShouldNotBeNull();
      errorDetails.Message.ShouldBe("Cannot build road segment because no direct connection between start location and end location.");
    }

    [Test]
    [TestCase(0, 0, "Cannot build road segment. Missing 1 brick and 1 lumber.")]
    [TestCase(1, 0, "Cannot build road segment. Missing 1 lumber.")]
    [TestCase(0, 1, "Cannot build road segment. Missing 1 brick.")]
    public void BuildRoadSegment_WithoutRequiredResourcesAvailable_MeaningfulErrorIsReceived(Int32 brickCount, Int32 lumberCount, String expectedErrorMessage)
    {
      // Arrange
      MockDice mockDice = null;
      MockPlayer player;
      MockComputerPlayer firstOpponent, secondOpponent, thirdOpponent;
      var localGameController = this.CreateLocalGameControllerAndCompleteGameSetup(out mockDice, out player, out firstOpponent, out secondOpponent, out thirdOpponent);
      mockDice.AddSequence(new[] { 8u });
      player.AddResources(new ResourceClutch(brickCount, 0, lumberCount, 0, 0));

      ErrorDetails errorDetails = null;
      localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

      TurnToken turnToken = null;
      localGameController.StartPlayerTurnEvent = (TurnToken t) => { turnToken = t; };
      localGameController.StartGamePlay();

      // Act
      localGameController.BuildRoadSegment(turnToken, 4u, 3u);

      // Assert
      errorDetails.ShouldNotBeNull();
      errorDetails.Message.ShouldBe(expectedErrorMessage);
    }

    [Test]
    public void BuildRoadSegment_AllRoadSegmentsAreBuilt_MeaningfulErrorIsReceived()
    {
      // Arrange
      MockDice mockDice = null;
      MockPlayer player;
      MockComputerPlayer firstOpponent, secondOpponent, thirdOpponent;
      var localGameController = this.CreateLocalGameControllerAndCompleteGameSetup(out mockDice, out player, out firstOpponent, out secondOpponent, out thirdOpponent);
      mockDice.AddSequence(new[] { 8u });
      player.AddResources(new ResourceClutch(15, 0, 15, 0, 0));

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

      var roadSegmentDetails = new UInt32[] { 4, 3, 3, 2, 2, 1, 1, 0, 0, 8, 8, 7, 7, 17, 17, 16, 16, 27, 27, 28, 28, 38, 38, 39, 39, 47 };
      for (var index = 0; index < roadSegmentDetails.Length; index += 2)
      {
        localGameController.BuildRoadSegment(turnToken, roadSegmentDetails[index], roadSegmentDetails[index + 1]);
      }

      // Act
      localGameController.BuildRoadSegment(turnToken, 47, 48);

      // Assert
      errorDetails.ShouldNotBeNull();
      errorDetails.Message.ShouldBe("Cannot build road segment. All road segments already built.");
    }
    #endregion 
  }
}
