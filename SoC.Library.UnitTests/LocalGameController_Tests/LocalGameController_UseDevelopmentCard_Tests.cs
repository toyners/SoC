
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
  [Category("LocalGameController.UseDevelopmentCard")]
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
    public void UseDevelopmentCard_TurnTokenNotCorrect_MeaningfulErrorIsReceived()
    {
      // Arrange
      this.TestSetup();

      ErrorDetails errorDetails = null;
      this.localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

      TurnToken turnToken = null;
      this.localGameController.StartPlayerTurnEvent = (TurnToken t) => { turnToken = t; };
      this.localGameController.StartGamePlay();

      // Act
      this.localGameController.UseDevelopmentCard(new TurnToken(), null);

      // Assert
      errorDetails.ShouldNotBeNull();
      errorDetails.Message.ShouldBe("Turn token not recognised.");
    }

    public void UseDevelopmentCard_UseCardPurchasedInSameTurn_MeaningfulErrorIsReceived()
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
      this.localGameController.UseDevelopmentCard(turnToken, purchasedDevelopmentCard);

      // Assert
      errorDetails.ShouldNotBeNull();
      errorDetails.Message.ShouldBe("Cannot use development card that is purchased in the same turn.");
    }

    public void UseDevelopmentCard_UseKnightDevelopmentCardAndTryToMoveRobberToSameSpot_MeaningfulErrorIsReceived()
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
      this.localGameController.UseDevelopmentCard(turnToken, purchasedDevelopmentCard);
      //this.localGameController

      // Assert
      errorDetails.ShouldNotBeNull();
      errorDetails.Message.ShouldBe("Must move Robber away from starting hex.");
    }

    public void UseDevelopmentCard_PlayerPlaysThirdKnightDevelopmentCard_LargestArmyEventRaised()
    {
      // Arrange
      this.TestSetup();
      ErrorDetails errorDetails = null;
      this.localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

      TurnToken turnToken = null;
      this.localGameController.StartPlayerTurnEvent = (TurnToken t) => { turnToken = t; };
      this.localGameController.StartGamePlay();

      // Act
      this.localGameController.UseDevelopmentCard(new TurnToken(), null);

      // Assert
      errorDetails.ShouldNotBeNull();
      errorDetails.Message.ShouldBe("Turn token not recognised.");
    }

    public void UseDevelopmentCard_OpponentPlaysMoreKnightDevelopmentCardThanPlayer_LargestArmyEventRaised()
    {
      // Arrange
      this.TestSetup();
      ErrorDetails errorDetails = null;
      this.localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

      TurnToken turnToken = null;
      this.localGameController.StartPlayerTurnEvent = (TurnToken t) => { turnToken = t; };
      this.localGameController.StartGamePlay();

      // Act
      this.localGameController.UseDevelopmentCard(new TurnToken(), null);

      // Assert
      errorDetails.ShouldNotBeNull();
      errorDetails.Message.ShouldBe("Turn token not recognised.");
    }

    public void UseDevelopmentCard_PlayerPlaysMoreKnightDevelopmentCardThanOpponent_LargestArmyEventRaised()
    {
      // Arrange
      this.TestSetup();
      ErrorDetails errorDetails = null;
      this.localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

      TurnToken turnToken = null;
      this.localGameController.StartPlayerTurnEvent = (TurnToken t) => { turnToken = t; };
      this.localGameController.StartGamePlay();

      // Act
      this.localGameController.UseDevelopmentCard(new TurnToken(), null);

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
