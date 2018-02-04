
namespace Jabberwocky.SoC.Library.UnitTests.LocalGameController_Tests
{
  using System;
  using MockGameBoards;
  using NUnit.Framework;
  using Shouldly;

  [TestFixture]
  [Category("All")]
  [Category("LocalGameController")]
  [Category("LocalGameController.BuildCity")]
  public class LocalGameController_BuildCity_Tests : LocalGameControllerTestBase
  {
    #region Tests
    [Test]
    public void BuildCity_OffBoard_MeaningfulErrorIsReceived()
    {
      // Arrange
      MockDice mockDice = null;
      MockPlayer player;
      MockComputerPlayer firstOpponent, secondOpponent, thirdOpponent;
      var localGameController = this.CreateLocalGameControllerAndCompleteGameSetup(out mockDice, out player, out firstOpponent, out secondOpponent, out thirdOpponent);

      mockDice.AddSequence(new[] { 8u });
      player.AddResources(ResourceClutch.City);

      Boolean cityBuilt = false;
      localGameController.CityBuiltEvent = () => { cityBuilt = true; };

      TurnToken turnToken = null;
      localGameController.StartPlayerTurnEvent = (TurnToken t) => { turnToken = t; };

      ErrorDetails errorDetails = null;
      localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

      localGameController.StartGamePlay();

      // Act
      localGameController.BuildCity(turnToken, 100);

      // Assert
      cityBuilt.ShouldBeFalse();
      errorDetails.ShouldNotBeNull();
      errorDetails.Message.ShouldBe("Cannot build city. Location 100 is outside of board range (0 - 53).");
    }

    [Test]
    public void BuildCity_OnExistingSettlementBelongingToOpponent_MeaningfulErrorIsReceived()
    {
      // Arrange
      MockDice mockDice = null;
      MockPlayer player;
      MockComputerPlayer firstOpponent, secondOpponent, thirdOpponent;
      var localGameController = this.CreateLocalGameControllerAndCompleteGameSetup(out mockDice, out player, out firstOpponent, out secondOpponent, out thirdOpponent);

      mockDice.AddSequence(new[] { 8u });
      player.AddResources(ResourceClutch.City);

      Boolean cityBuilt = false;
      localGameController.CityBuiltEvent = () => { cityBuilt = true; };

      TurnToken turnToken = null;
      localGameController.StartPlayerTurnEvent = (TurnToken t) => { turnToken = t; };

      ErrorDetails errorDetails = null;
      localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

      localGameController.StartGamePlay();

      // Act
      localGameController.BuildCity(turnToken, FirstSettlementOneLocation);

      // Assert
      cityBuilt.ShouldBeFalse();
      errorDetails.ShouldNotBeNull();
      errorDetails.Message.ShouldBe("Cannot build city. Location " + FirstSettlementOneLocation + " is owned by player '" + FirstOpponentName + "'.");
    }

    [Test]
    public void BuildCity_OnExistingSettlementBelongingToPlayer_CityBuiltEventRaised()
    {
      // Arrange
      MockDice mockDice = null;
      MockPlayer player;
      MockComputerPlayer firstOpponent, secondOpponent, thirdOpponent;
      var localGameController = this.CreateLocalGameControllerAndCompleteGameSetup(out mockDice, out player, out firstOpponent, out secondOpponent, out thirdOpponent);

      mockDice.AddSequence(new[] { 8u });
      player.AddResources(ResourceClutch.City);

      Boolean cityBuilt = false;
      localGameController.CityBuiltEvent = () => { cityBuilt = true; };

      TurnToken turnToken = null;
      localGameController.StartPlayerTurnEvent = (TurnToken t) => { turnToken = t; };

      ErrorDetails errorDetails = null;
      localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

      localGameController.StartGamePlay();

      // Act
      localGameController.BuildCity(turnToken, MainSettlementOneLocation);

      // Assert
      cityBuilt.ShouldBeTrue();
      errorDetails.ShouldBeNull();
    }

    [Test]
    [TestCase(0, 0, "Cannot build city. Missing 2 grain and 3 ore.")]
    [TestCase(1, 1, "Cannot build city. Missing 1 grain and 2 ore.")]
    [TestCase(1, 2, "Cannot build city. Missing 1 grain and 1 ore.")]
    [TestCase(2, 0, "Cannot build city. Missing 3 ore.")]
    [TestCase(0, 3, "Cannot build city. Missing 2 grain.")]
    public void BuildCity_InsufficientResources_MeaningfulErrorIsReceived(Int32 grainCount, Int32 oreCount, String expectedMessage)
    {
      // Arrange
      var testInstances = LocalGameControllerTestCreator.CreateTestInstances(new MockGameBoardWithNoResourcesCollected());
      var localGameController = testInstances.LocalGameController;
      var player = testInstances.MainPlayer;
      LocalGameControllerTestSetup.LaunchGameAndCompleteSetup(localGameController);

      testInstances.Dice.AddSequence(new[] { 8u });

      player.AddResources(new ResourceClutch(0, grainCount, 0, oreCount, 0));

      Boolean cityBuilt = false;
      localGameController.CityBuiltEvent = () => { cityBuilt = true; };

      TurnToken turnToken = null;
      localGameController.StartPlayerTurnEvent = (TurnToken t) => { turnToken = t; };

      ErrorDetails errorDetails = null;
      localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

      localGameController.StartGamePlay();

      // Act
      localGameController.BuildCity(turnToken, MainSettlementOneLocation);

      // Assert
      cityBuilt.ShouldBeFalse();
      errorDetails.ShouldNotBeNull();
      errorDetails.Message.ShouldBe(expectedMessage);
    }

    [Test]
    public void BuildCity_AllCitiesAreBuilt_MeaningfulErrorIsReceived()
    {
      // Arrange
      MockDice mockDice = null;
      MockPlayer player;
      MockComputerPlayer firstOpponent, secondOpponent, thirdOpponent;
      var localGameController = this.CreateLocalGameControllerAndCompleteGameSetup(out mockDice, out player, out firstOpponent, out secondOpponent, out thirdOpponent);

      mockDice.AddSequence(new[] { 8u });
      player.AddResources(ResourceClutch.RoadSegment * 4);
      player.AddResources(ResourceClutch.Settlement * 3);
      player.AddResources(ResourceClutch.City * 5);

      TurnToken turnToken = null;
      localGameController.StartPlayerTurnEvent = (TurnToken t) => { turnToken = t; };

      ErrorDetails errorDetails = null;
      localGameController.ErrorRaisedEvent = (ErrorDetails e) => 
      {
        if (errorDetails != null)
        {
          throw new Exception("Error already raised: " + errorDetails.Message);
        }

        errorDetails = e;
      };

      localGameController.StartGamePlay();
      localGameController.BuildCity(turnToken, MainSettlementOneLocation);
      localGameController.BuildCity(turnToken, MainSettlementTwoLocation);
      localGameController.BuildRoadSegment(turnToken, 4, 3);
      localGameController.BuildRoadSegment(turnToken, 4, 5);
      localGameController.BuildSettlement(turnToken, 3);
      localGameController.BuildCity(turnToken, 3);
      localGameController.BuildSettlement(turnToken, 5);
      localGameController.BuildCity(turnToken, 5);
      localGameController.BuildRoadSegment(turnToken, 3, 2);
      localGameController.BuildRoadSegment(turnToken, 2, 1);
      localGameController.BuildSettlement(turnToken, 1);

      // Act
      localGameController.BuildCity(turnToken, 1);

      // Assert
      errorDetails.ShouldNotBeNull();
      errorDetails.Message.ShouldBe("Cannot build city. All cities already built.");
    }

    [Test]
    public void BuildCity_OnExistingCityBelongingToPlayer_MeaningfulErrorIsReceived()
    {
      // Arrange
      MockDice mockDice = null;
      MockPlayer player;
      MockComputerPlayer firstOpponent, secondOpponent, thirdOpponent;
      var localGameController = this.CreateLocalGameControllerAndCompleteGameSetup(out mockDice, out player, out firstOpponent, out secondOpponent, out thirdOpponent);

      mockDice.AddSequence(new[] { 8u });
      player.AddResources(ResourceClutch.City * 2);

      TurnToken turnToken = null;
      localGameController.StartPlayerTurnEvent = (TurnToken t) => { turnToken = t; };

      ErrorDetails errorDetails = null;
      localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

      localGameController.StartGamePlay();
      localGameController.BuildCity(turnToken, MainSettlementOneLocation);

      // Act
      localGameController.BuildCity(turnToken, MainSettlementOneLocation);

      // Assert
      errorDetails.ShouldNotBeNull();
      errorDetails.Message.ShouldBe("Cannot build city. There is already a city at location " + MainSettlementOneLocation + " that belongs to you.");
    }

    [Test]
    public void BuildCity_OnExistingCityBelongingToOpponent_MeaningfulErrorIsReceived()
    {
      // Arrange
      var testInstances = LocalGameControllerTestCreator.CreateTestInstances();
      var localGameController = testInstances.LocalGameController;
      LocalGameControllerTestSetup.LaunchGameAndCompleteSetup(localGameController);
      var player = testInstances.MainPlayer;
      var firstOpponent = testInstances.FirstOpponent;

      testInstances.Dice.AddSequence(new[] { 8u, 8u });
      player.AddResources(ResourceClutch.City);
      firstOpponent.AddResources(ResourceClutch.City);
      firstOpponent.AddCityChoices(new[] { FirstSettlementOneLocation });

      TurnToken turnToken = null;
      localGameController.StartPlayerTurnEvent = (TurnToken t) => { turnToken = t; };

      ErrorDetails errorDetails = null;
      localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

      localGameController.StartGamePlay();
      localGameController.EndTurn(turnToken);

      // Act
      localGameController.BuildCity(turnToken, FirstSettlementOneLocation);

      // Assert
      errorDetails.ShouldNotBeNull();
      errorDetails.Message.ShouldBe("Cannot build city. Location " + FirstSettlementOneLocation + " is owned by player '" + FirstOpponentName + "'.");
    }

    [Test]
    public void BuildCity_OnLocationThatIsEmpty_MeaningfulErrorIsReceived()
    {
      // Arrange
      MockDice mockDice = null;
      MockPlayer player;
      MockComputerPlayer firstOpponent, secondOpponent, thirdOpponent;
      var localGameController = this.CreateLocalGameControllerAndCompleteGameSetup(out mockDice, out player, out firstOpponent, out secondOpponent, out thirdOpponent);

      mockDice.AddSequence(new[] { 8u });
      player.AddResources(ResourceClutch.City);

      TurnToken turnToken = null;
      localGameController.StartPlayerTurnEvent = (TurnToken t) => { turnToken = t; };

      ErrorDetails errorDetails = null;
      localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

      localGameController.StartGamePlay();

      // Act
      localGameController.BuildCity(turnToken, 0);

      // Assert
      errorDetails.ShouldNotBeNull();
      errorDetails.Message.ShouldBe("Cannot build city. No settlement at location 0.");
    }

    [Test]
    public void BuildCity_OnLocationThatIsNotSettlement_MeaningfulErrorIsReceived()
    {
      // Arrange
      MockDice mockDice = null;
      MockPlayer player;
      MockComputerPlayer firstOpponent, secondOpponent, thirdOpponent;
      var localGameController = this.CreateLocalGameControllerAndCompleteGameSetup(out mockDice, out player, out firstOpponent, out secondOpponent, out thirdOpponent);

      mockDice.AddSequence(new[] { 8u });
      player.AddResources(ResourceClutch.RoadSegment);
      player.AddResources(ResourceClutch.City);

      TurnToken turnToken = null;
      localGameController.StartPlayerTurnEvent = (TurnToken t) => { turnToken = t; };

      ErrorDetails errorDetails = null;
      localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

      localGameController.StartGamePlay();
      localGameController.BuildRoadSegment(turnToken, 4, 3);

      // Act
      localGameController.BuildCity(turnToken, 3);

      // Assert
      errorDetails.ShouldNotBeNull();
      errorDetails.Message.ShouldBe("Cannot build city. No settlement at location 3.");
    }

    [Test]
    public void BuildCity_TurnTokenNotCorrect_MeaningfulErrorIsReceived()
    {
      // Arrange
      MockDice mockDice = null;
      MockPlayer player;
      MockComputerPlayer firstOpponent, secondOpponent, thirdOpponent;
      var localGameController = this.CreateLocalGameControllerAndCompleteGameSetup(out mockDice, out player, out firstOpponent, out secondOpponent, out thirdOpponent);

      mockDice.AddSequence(new[] { 8u });

      ErrorDetails errorDetails = null;
      localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

      TurnToken turnToken = null;
      localGameController.StartPlayerTurnEvent = (TurnToken t) => { turnToken = t; };
      localGameController.StartGamePlay();

      // Act
      localGameController.BuildCity(new TurnToken(), 3);

      // Assert
      errorDetails.ShouldNotBeNull();
      errorDetails.Message.ShouldBe("Turn token not recognised.");
    }

    [Test]
    public void BuildCity_AlmostGotAllVictoryPoints_EndOfGameEventRaisedWithPlayerAsWinner()
    {
      // Arrange
      /*MockDice mockDice = null;
      MockPlayer player;
      MockComputerPlayer firstOpponent, secondOpponent, thirdOpponent;
      var localGameController = this.CreateLocalGameControllerAndCompleteGameSetup(out mockDice, out player, out firstOpponent, out secondOpponent, out thirdOpponent);

      mockDice.AddSequence(new[] { 8u });

      ErrorDetails errorDetails = null;
      localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

      TurnToken turnToken = null;
      localGameController.StartPlayerTurnEvent = (TurnToken t) => { turnToken = t; };
      localGameController.StartGamePlay();

      // Act
      localGameController.BuildCity(turnToken, MainSettlementOneLocation);

      // Assert
      errorDetails.ShouldNotBeNull();
      errorDetails.Message.ShouldBe("Turn token not recognised.");*/
      throw new NotImplementedException();
    }

    [Test]
    public void Scenario_OpponentBuildsCity()
    {
      throw new NotImplementedException();
    }
    #endregion
  }
}
