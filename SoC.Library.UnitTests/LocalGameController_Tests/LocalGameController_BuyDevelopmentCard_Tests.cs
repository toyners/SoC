

namespace Jabberwocky.SoC.Library.UnitTests.LocalGameController_Tests
{
  using System;
  using Interfaces;
  using NSubstitute;
  using NUnit.Framework;
  using Shouldly;

  [TestFixture]
  [Category("All")]
  [Category("LocalGameController")]
  [Category("LocalGameController.BuyDevelopmentCard")]
  public class LocalGameController_BuyDevelopmentCard_Tests : LocalGameControllerTestBase
  {
    private MockPlayer player;
    private MockComputerPlayer firstOpponent, secondOpponent, thirdOpponent;
    private MockDice dice;
    private LocalGameController localGameController;
    //private IDevelopmentCardHolder developmentCardHolder;

    #region Methods
    /*[SetUp]
    public void TestSetup()
    {
      this.CreateDefaultPlayerInstances(out this.player, out this.firstOpponent, out this.secondOpponent, out this.thirdOpponent);
      this.dice = this.CreateMockDice();
      this.dice.AddSequence(new[] { 8u });
      this.developmentCardHolder = new DevelopmentCardHolder();

      var playerPool = this.CreatePlayerPool(this.player, new[] { this.firstOpponent, this.secondOpponent, this.thirdOpponent });
      this.localGameController = this.CreateLocalGameController(dice, playerPool, this.developmentCardHolder);
      this.CompleteGameSetup(this.localGameController);
    }*/

    [Test]
    public void BuildCity_TurnTokenNotCorrect_MeaningfulErrorIsReceived()
    {
      // Arrange
      this.TestSetup();
      ErrorDetails errorDetails = null;
      this.localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

      TurnToken turnToken = null;
      this.localGameController.StartPlayerTurnEvent = (TurnToken t) => { turnToken = t; };
      this.localGameController.StartGamePlay();

      // Act
      this.localGameController.BuyDevelopmentCard(new TurnToken());

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
      this.TestSetup();
      this.player.AddResources(new ResourceClutch(0, grainCount, 0, oreCount, woolCount));

      TurnToken turnToken = null;
      this.localGameController.StartPlayerTurnEvent = (TurnToken t) => { turnToken = t; };

      ErrorDetails errorDetails = null;
      this.localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

      Boolean developmentCardPurchased = false;
      this.localGameController.DevelopmentCardPurchasedEvent = (DevelopmentCard d) => { developmentCardPurchased = true; };

      this.localGameController.StartGamePlay();

      // Act
      this.localGameController.BuyDevelopmentCard(turnToken);

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
      this.TestSetup(this.CreateMockOneCardDevelopmentCardHolder(knightDevelopmentCard));
      this.player.AddResources(ResourceClutch.DevelopmentCard);

      TurnToken turnToken = null;
      this.localGameController.StartPlayerTurnEvent = (TurnToken t) => { turnToken = t; };

      ErrorDetails errorDetails = null;
      this.localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };


      DevelopmentCard purchaseddDevelopmentCard = null;
      this.localGameController.DevelopmentCardPurchasedEvent = (DevelopmentCard d) => { purchaseddDevelopmentCard = d; };

      this.localGameController.StartGamePlay();

      // Act
      this.localGameController.BuyDevelopmentCard(turnToken);

      // Assert
      purchaseddDevelopmentCard.ShouldNotBeNull();
      purchaseddDevelopmentCard.ShouldBeSameAs(knightDevelopmentCard);
      errorDetails.ShouldBeNull();
    }

    [Test]
    public void BuyDevelopmentCard_NoMoreDevelopmentCards_MeaningfulErrorIsReceived()
    {
      // Arrange
      this.TestSetup();
      this.player.AddResources(ResourceClutch.DevelopmentCard * 26);

      TurnToken turnToken = null;
      this.localGameController.StartPlayerTurnEvent = (TurnToken t) => { turnToken = t; };

      ErrorDetails errorDetails = null;
      this.localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

      this.localGameController.StartGamePlay();
      for (var i = 25; i > 0; i--)
      {
        this.localGameController.BuyDevelopmentCard(turnToken);
      }

      // Act
      this.localGameController.BuyDevelopmentCard(turnToken);

      // Assert
      errorDetails.ShouldNotBeNull();
      errorDetails.Message.ShouldBe("Cannot buy development card. No more cards available");
    }

    private void TestSetup()
    {
      this.TestSetup(new DevelopmentCardHolder());
    }

    private void TestSetup(IDevelopmentCardHolder developmentCardHolder)
    {
      this.CreateDefaultPlayerInstances(out this.player, out this.firstOpponent, out this.secondOpponent, out this.thirdOpponent);
      this.dice = this.CreateMockDice();
      this.dice.AddSequence(new[] { 8u });

      var playerPool = this.CreatePlayerPool(this.player, new[] { this.firstOpponent, this.secondOpponent, this.thirdOpponent });
      this.localGameController = this.CreateLocalGameController(dice, playerPool, developmentCardHolder);
      this.CompleteGameSetup(this.localGameController);
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
