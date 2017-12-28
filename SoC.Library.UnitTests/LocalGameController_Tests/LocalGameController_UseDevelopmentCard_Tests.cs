
namespace Jabberwocky.SoC.Library.UnitTests.LocalGameController_Tests
{
  using System;
  using System.Collections.Generic;
  using Interfaces;
  using NSubstitute;
  using NUnit.Framework;
  using Shouldly;

  [TestFixture]
  [Category("All")]
  [Category("LocalGameController")]
  [Category("LocalGameController.UseKnightDevelopmentCard")]
  public class LocalGameController_UseDevelopmentCard_Tests : LocalGameControllerTestBase
  {
    #region Fields
    private MockPlayer player;
    private MockComputerPlayer firstOpponent, secondOpponent, thirdOpponent;
    private MockDice dice;
    private LocalGameController localGameController;
    #endregion

    #region Methods
    [Test]
    public void UseKnightDevelopmentCard_TurnTokenNotCorrect_MeaningfulErrorIsReceived()
    {
      // Arrange
      this.TestSetup();

      ErrorDetails errorDetails = null;
      this.localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

      TurnToken turnToken = null;
      this.localGameController.StartPlayerTurnEvent = (TurnToken t) => { turnToken = t; };
      this.localGameController.StartGamePlay();

      // Act
      this.localGameController.UseKnightDevelopmentCard(new TurnToken(), null);

      // Assert
      errorDetails.ShouldNotBeNull();
      errorDetails.Message.ShouldBe("Turn token not recognised.");
    }

    [Test]
    public void UseKnightDevelopmentCard_UseCardPurchasedInSameTurn_MeaningfulErrorIsReceived()
    {
      // Arrange
      var knightDevelopmentCard = new KnightDevelopmentCard();
      this.TestSetup(knightDevelopmentCard);
      this.player.AddResources(ResourceClutch.DevelopmentCard);

      ErrorDetails errorDetails = null;
      this.localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

      TurnToken turnToken = null;
      this.localGameController.StartPlayerTurnEvent = (TurnToken t) => { turnToken = t; };

      DevelopmentCard purchasedDevelopmentCard = null;
      this.localGameController.DevelopmentCardPurchasedEvent = (DevelopmentCard d) => { purchasedDevelopmentCard = d; };

      this.localGameController.StartGamePlay();
      this.localGameController.BuyDevelopmentCard(turnToken);

      // Act
      this.localGameController.UseKnightDevelopmentCard(turnToken, (KnightDevelopmentCard)purchasedDevelopmentCard);

      // Assert
      errorDetails.ShouldNotBeNull();
      errorDetails.Message.ShouldBe("Cannot use development card that is purchased in the same turn.");
    }

    [Test]
    public void UseKnightDevelopmentCard_UseKnightDevelopmentCardAndTryToMoveRobberToSameSpot_MeaningfulErrorIsReceived()
    {
      // Arrange
      var knightDevelopmentCard = new KnightDevelopmentCard();
      this.TestSetup(knightDevelopmentCard);
      ErrorDetails errorDetails = null;
      this.localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

      TurnToken turnToken = null;
      this.localGameController.StartPlayerTurnEvent = (TurnToken t) => { turnToken = t; };

      DevelopmentCard purchasedDevelopmentCard = null;
      this.localGameController.DevelopmentCardPurchasedEvent = (DevelopmentCard d) => { purchasedDevelopmentCard = d; };

      this.localGameController.StartGamePlay();
      this.localGameController.EndTurn(turnToken);

      // Act
      this.localGameController.UseKnightDevelopmentCard(turnToken, (KnightDevelopmentCard)purchasedDevelopmentCard);
      this.localGameController.SetRobberLocation(0);

      // Assert
      errorDetails.ShouldNotBeNull();
      errorDetails.Message.ShouldBe("Must move Robber away from present hex (0).");
    }

    [Test]
    public void UseKnightDevelopmentCard_PlayerPlaysThirdKnightDevelopmentCard_LargestArmyEventRaised()
    {
      // Arrange
      var knightDevelopmentCard1 = new KnightDevelopmentCard();
      var knightDevelopmentCard2 = new KnightDevelopmentCard();
      var knightDevelopmentCard3 = new KnightDevelopmentCard();
      this.TestSetup(knightDevelopmentCard1, knightDevelopmentCard2, knightDevelopmentCard3);

      ErrorDetails errorDetails = null;
      this.localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

      TurnToken turnToken = null;
      this.localGameController.StartPlayerTurnEvent = (TurnToken t) => { turnToken = t; };

      Guid newPlayer = Guid.Empty, oldPlayer = Guid.Empty;
      this.localGameController.LargestArmyEvent = (Guid np, Guid op) => { newPlayer = np; oldPlayer = op; };

      this.localGameController.StartGamePlay();
      this.localGameController.EndTurn(turnToken);

      // Act
      this.localGameController.UseKnightDevelopmentCard(turnToken, knightDevelopmentCard1);
      this.localGameController.UseKnightDevelopmentCard(turnToken, knightDevelopmentCard2);
      this.localGameController.UseKnightDevelopmentCard(turnToken, knightDevelopmentCard3);

      // Assert
      newPlayer.ShouldBe(this.player.Id);
      oldPlayer.ShouldBeNull();
    }

    [Test]
    public void UseKnightDevelopmentCard_OpponentPlaysMoreKnightDevelopmentCardThanPlayer_LargestArmyEventRaised()
    {
      // Arrange
      this.TestSetup();
      ErrorDetails errorDetails = null;
      this.localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

      TurnToken turnToken = null;
      this.localGameController.StartPlayerTurnEvent = (TurnToken t) => { turnToken = t; };
      this.localGameController.StartGamePlay();

      // Act
      this.localGameController.UseKnightDevelopmentCard(new TurnToken(), null);

      // Assert
      errorDetails.ShouldNotBeNull();
      errorDetails.Message.ShouldBe("Turn token not recognised.");
    }

    [Test]
    public void UseKnightDevelopmentCard_PlayerPlaysMoreKnightDevelopmentCardThanOpponent_LargestArmyEventRaised()
    {
      // Arrange
      this.TestSetup();
      ErrorDetails errorDetails = null;
      this.localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

      TurnToken turnToken = null;
      this.localGameController.StartPlayerTurnEvent = (TurnToken t) => { turnToken = t; };
      this.localGameController.StartGamePlay();

      // Act
      this.localGameController.UseKnightDevelopmentCard(new TurnToken(), null);

      // Assert
      errorDetails.ShouldNotBeNull();
      errorDetails.Message.ShouldBe("Turn token not recognised.");
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

    private void TestSetup(DevelopmentCard firstDevelopmentCard, params DevelopmentCard[] otherDevelopmentCards)
    {
      this.TestSetup(this.CreateMockCardDevelopmentCardHolder(firstDevelopmentCard, otherDevelopmentCards));
    }

    private IDevelopmentCardHolder CreateMockCardDevelopmentCardHolder(DevelopmentCard firstDevelopmentCard, params DevelopmentCard[] otherDevelopmentCards)
    {
      var developmentCardHolder = Substitute.For<IDevelopmentCardHolder>();
      var developmentCards = new Queue<DevelopmentCard>();
      developmentCards.Enqueue(firstDevelopmentCard);
      foreach (var developmentCard in otherDevelopmentCards)
      {
        developmentCards.Enqueue(developmentCard);
      }

      DevelopmentCard card;
      developmentCardHolder
        .TryGetNextCard(out card)
        .Returns(x =>
        {
          if (developmentCards.Count > 0)
          {
            x[0] = developmentCards.Dequeue();
            return true;
          }

          x[0] = null;
          return false;
        });

      developmentCardHolder.HasCards.Returns(x => { return developmentCards.Count > 0; });
      return developmentCardHolder;
    }
    #endregion
  }
}
