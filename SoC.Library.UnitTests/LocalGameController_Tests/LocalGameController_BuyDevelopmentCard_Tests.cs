

namespace Jabberwocky.SoC.Library.UnitTests.LocalGameController_Tests
{
  using System;
  using NUnit.Framework;
  using Shouldly;

  [TestFixture]
  [Category("All")]
  [Category("LocalGameController")]
  [Category("LocalGameController.BuyDevelopmentCard")]
  public class LocalGameController_BuyDevelopmentCard_Tests : LocalGameControllerTestBase
  {
    #region Methods
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
      localGameController.BuyDevelopmentCard(new TurnToken());

      // Assert
      errorDetails.ShouldNotBeNull();
      errorDetails.Message.ShouldBe("Turn token not recognised.");
    }

    [Test]
    [TestCase(0, 0, 0, "Cannot buy development card. Missing 1 grain and 1 ore and 1 wool.")]
    [TestCase(0, 0, 1, "Cannot buy development card. Missing 1 grain and 1 ore.")]
    [TestCase(0, 1, 0, "Cannot buy development card. Missing 1 grain and 1 wool.")]
    [TestCase(0, 1, 1, "Cannot buy development card. Missing 1 grain.")]
    [TestCase(1, 0, 0, "Cannot buy development card. Missing 1 ore and 1 wool.")]
    [TestCase(1, 0, 1, "Cannot buy development card. Missing 1 ore.")]
    [TestCase(1, 1, 0, "Cannot buy development card. Missing 1 wool.")]
    public void BuyDevelopmentCard_InsufficientResources_MeaningfulErrorIsReceived(Int32 grainCount, Int32 oreCount, Int32 woolCount, String expectedErrorMessage)
    {
      // Arrange
      MockDice mockDice = null;
      MockPlayer player;
      MockComputerPlayer firstOpponent, secondOpponent, thirdOpponent;
      var localGameController = this.CreateLocalGameControllerAndCompleteGameSetup(out mockDice, out player, out firstOpponent, out secondOpponent, out thirdOpponent);

      mockDice.AddSequence(new[] { 8u });
      player.AddResources(new ResourceClutch(0, grainCount, 0, oreCount, woolCount));

      TurnToken turnToken = null;
      localGameController.StartPlayerTurnEvent = (TurnToken t) => { turnToken = t; };

      ErrorDetails errorDetails = null;
      localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

      Boolean developmentCardPurchased = false;
      localGameController.DevelopmentCardPurchasedEvent = () => { developmentCardPurchased = true; };

      localGameController.StartGamePlay();

      // Act
      localGameController.BuyDevelopmentCard(turnToken);

      // Assert
      developmentCardPurchased.ShouldBeFalse();
      errorDetails.ShouldNotBeNull();
      errorDetails.Message.ShouldBe(expectedErrorMessage);
    }

    [Test]
    public void BuyDevelopmentCard_GotResources_DevelopmentCardPurchasedEventIsRaised()
    {
      // Arrange
      MockDice mockDice = null;
      MockPlayer player;
      MockComputerPlayer firstOpponent, secondOpponent, thirdOpponent;
      var localGameController = this.CreateLocalGameControllerAndCompleteGameSetup(out mockDice, out player, out firstOpponent, out secondOpponent, out thirdOpponent);

      mockDice.AddSequence(new[] { 8u });
      player.AddResources(ResourceClutch.DevelopmentCard);

      TurnToken turnToken = null;
      localGameController.StartPlayerTurnEvent = (TurnToken t) => { turnToken = t; };

      ErrorDetails errorDetails = null;
      localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

      Boolean developmentCardPurchased = false;
      localGameController.DevelopmentCardPurchasedEvent = () => { developmentCardPurchased = true; };

      localGameController.StartGamePlay();

      // Act
      localGameController.BuyDevelopmentCard(turnToken);

      // Assert
      developmentCardPurchased.ShouldBeTrue();
      errorDetails.ShouldBeNull();
    }

    [Test]
    public void BuyDevelopmentCard_NoMoreDevelopmentCards_MeaningfulErrorIsReceived()
    {
      // Arrange
      MockDice mockDice = null;
      MockPlayer player;
      MockComputerPlayer firstOpponent, secondOpponent, thirdOpponent;
      var localGameController = this.CreateLocalGameControllerAndCompleteGameSetup(out mockDice, out player, out firstOpponent, out secondOpponent, out thirdOpponent);

      mockDice.AddSequence(new[] { 8u });
      player.AddResources(ResourceClutch.DevelopmentCard * 26);

      TurnToken turnToken = null;
      localGameController.StartPlayerTurnEvent = (TurnToken t) => { turnToken = t; };

      ErrorDetails errorDetails = null;
      localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

      localGameController.StartGamePlay();
      for (var i = 25; i > 0; i--)
      {
        localGameController.BuyDevelopmentCard(turnToken);
      }

      // Act
      localGameController.BuyDevelopmentCard(turnToken);

      // Assert
      errorDetails.ShouldNotBeNull();
      errorDetails.Message.ShouldBe("Cannot buy development card. No more cards available");
    }
    #endregion 
  }
}
