

namespace Jabberwocky.SoC.Library.UnitTests.LocalGameController_Tests
{
  using System;
  using NSubstitute;
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
    #endregion 
  }
}
