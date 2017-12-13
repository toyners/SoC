
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
      errorDetails.Message.ShouldBe("Cannot build city: ");
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
      errorDetails.Message.ShouldBe("Cannot build city: ");
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
    [TestCase(0, 0, "")]
    [TestCase(1, 1, "")]
    [TestCase(1, 2, "")]
    [TestCase(2, 0, "")]
    [TestCase(0, 3, "")]
    public void BuildCity_InsufficientResources_MeaningfulErrorIsReceived(Int32 grainCount, Int32 oreCount, String expectedMessage)
    {
      // Arrange
      MockDice mockDice = null;
      MockPlayer player;
      MockComputerPlayer firstOpponent, secondOpponent, thirdOpponent;
      var localGameController = this.CreateLocalGameControllerAndCompleteGameSetup(out mockDice, out player, out firstOpponent, out secondOpponent, out thirdOpponent);

      mockDice.AddSequence(new[] { 8u });
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
      player.AddResources(ResourceClutch.RoadSegment * 6);
      player.AddResources(ResourceClutch.Settlement * 4);
      player.AddResources(ResourceClutch.City * 5);

      TurnToken turnToken = null;
      localGameController.StartPlayerTurnEvent = (TurnToken t) => { turnToken = t; };

      ErrorDetails errorDetails = null;
      localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

      localGameController.StartGamePlay();
      localGameController.BuildRoadSegment(turnToken, 4, 3);
      localGameController.BuildRoadSegment(turnToken, 4, 5);
      localGameController.BuildSettlement(turnToken, 3);
      localGameController.BuildSettlement(turnToken, 5);
      localGameController.BuildRoadSegment(turnToken, 3, 2);
      localGameController.BuildRoadSegment(turnToken, 2, 1);
      localGameController.BuildSettlement(turnToken, 1);
      localGameController.BuildRoadSegment(turnToken, 5, 6);
      localGameController.BuildRoadSegment(turnToken, 6, 14);
      localGameController.BuildSettlement(turnToken, 14);

      // Act
      localGameController.BuildCity(turnToken, 1);

      // Assert
      errorDetails.ShouldNotBeNull();
      errorDetails.Message.ShouldBe("Cannot build city: ");
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
      errorDetails.Message.ShouldBe("Cannot build city: ");
    }

    [Test]
    public void BuildCity_OnExistingCityBelongingToOpponent_MeaningfulErrorIsReceived()
    {
      // Arrange
      MockDice mockDice = null;
      MockPlayer player;
      MockComputerPlayer firstOpponent, secondOpponent, thirdOpponent;
      var localGameController = this.CreateLocalGameControllerAndCompleteGameSetup(out mockDice, out player, out firstOpponent, out secondOpponent, out thirdOpponent);

      mockDice.AddSequence(new[] { 8u });
      player.AddResources(ResourceClutch.City);
      firstOpponent.AddResources(ResourceClutch.City);

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
      errorDetails.Message.ShouldBe("Cannot build city: ");
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
      errorDetails.Message.ShouldBe("Cannot build city: ");
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
      errorDetails.Message.ShouldBe("Cannot build city: ");
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
  }
}
