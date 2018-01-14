

namespace Jabberwocky.SoC.Library.UnitTests.LocalGameController_Tests
{
  using System;
  using Interfaces;
  using NSubstitute;
  using NUnit.Framework;
  using Shouldly;
  using static LocalGameControllerTestCreator;

  [TestFixture]
  [Category("All")]
  [Category("LocalGameController")]
  [Category("LocalGameController.BuyDevelopmentCard")]
  public class LocalGameController_BuyDevelopmentCard_Tests
  {
    #region Methods
    [Test]
    public void BuildCity_TurnTokenNotCorrect_MeaningfulErrorIsReceived()
    {
      // Arrange
      var testInstances = this.TestSetup();
      var localGameController = testInstances.LocalGameController;
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
      var testInstances = this.TestSetup();
      var localGameController = testInstances.LocalGameController;
      var player = testInstances.MainPlayer;

      testInstances.Dice.AddSequence(new[] { 8u });

      player.RemoveAllResources();  // Clear down the initial resources
      player.AddResources(new ResourceClutch(0, grainCount, 0, oreCount, woolCount));

      TurnToken turnToken = null;
      localGameController.StartPlayerTurnEvent = (TurnToken t) => { turnToken = t; };

      ErrorDetails errorDetails = null;
      localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

      Boolean developmentCardPurchased = false;
      localGameController.DevelopmentCardPurchasedEvent = (DevelopmentCard d) => { developmentCardPurchased = true; };

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
      var knightDevelopmentCard = new KnightDevelopmentCard();
      var testInstances = this.TestSetup(this.CreateMockOneCardDevelopmentCardHolder(knightDevelopmentCard));
      testInstances.MainPlayer.AddResources(ResourceClutch.DevelopmentCard);
      var localGameController = testInstances.LocalGameController;

      TurnToken turnToken = null;
      localGameController.StartPlayerTurnEvent = (TurnToken t) => { turnToken = t; };

      ErrorDetails errorDetails = null;
      localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };


      DevelopmentCard purchaseddDevelopmentCard = null;
      localGameController.DevelopmentCardPurchasedEvent = (DevelopmentCard d) => { purchaseddDevelopmentCard = d; };

      localGameController.StartGamePlay();

      // Act
      localGameController.BuyDevelopmentCard(turnToken);

      // Assert
      purchaseddDevelopmentCard.ShouldNotBeNull();
      purchaseddDevelopmentCard.ShouldBeSameAs(knightDevelopmentCard);
      errorDetails.ShouldBeNull();
    }

    [Test]
    public void BuyDevelopmentCard_NoMoreDevelopmentCards_MeaningfulErrorIsReceived()
    {
      // Arrange
      var testInstances = this.TestSetup();
      testInstances.MainPlayer.AddResources(ResourceClutch.DevelopmentCard * 26);
      var localGameController = testInstances.LocalGameController;

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

    private TestInstances TestSetup()
    {
      return this.TestSetup(new DevelopmentCardHolder());
    }

    private TestInstances TestSetup(IDevelopmentCardHolder developmentCardHolder)
    {
      var testInstances = LocalGameControllerTestCreator.CreateTestInstances(developmentCardHolder);
      LocalGameControllerTestSetup.LaunchGameAndCompleteSetup(testInstances.LocalGameController);
      testInstances.Dice.AddSequence(new[] { 8u });

      return testInstances;
    }

    private IDevelopmentCardHolder CreateMockOneCardDevelopmentCardHolder(DevelopmentCard developmentCard)
    {
      DevelopmentCard card;
      var developmentCardHolder = Substitute.For<IDevelopmentCardHolder>();
      developmentCardHolder
        .TryGetNextCard(out card)
        .Returns(x => { x[0] = developmentCard; return true; });
      developmentCardHolder.HasCards.Returns(true);
      return developmentCardHolder;
    }
    #endregion
  }
}
