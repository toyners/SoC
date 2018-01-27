﻿
namespace Jabberwocky.SoC.Library.UnitTests.LocalGameController_Tests
{
  using System;
  using System.Collections.Generic;
  using GameBoards;
  using Interfaces;
  using MockGameBoards;
  using NSubstitute;
  using NUnit.Framework;
  using Shouldly;

  [TestFixture]
  [Category("All")]
  [Category("LocalGameController")]
  [Category("LocalGameController.UseYearOfPlentyCard")]
  public class LocalGameController_UseYearOfPlentyCard_Tests
  {
    #region Methods
    [Test]
    public void UseYearOfPlentyCard_TurnTokenNotCorrect_MeaningfulErrorIsReceived()
    {
      // Arrange
      var testInstances = this.TestSetupWithDefaultCardHolder();
      var localGameController = testInstances.LocalGameController;

      TurnToken turnToken = null;
      localGameController.StartPlayerTurnEvent = (TurnToken t) => { turnToken = t; };

      localGameController.StartGamePlay();

      ErrorDetails errorDetails = null;
      localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

      // Act
      localGameController.UseYearOfPlentyCard(new TurnToken(), null, ResourceTypes.Brick, ResourceTypes.Brick);

      // Assert
      errorDetails.ShouldNotBeNull();
      errorDetails.Message.ShouldBe("Turn token not recognised.");
    }

    [Test]
    public void UseYearOfPlentyCard_CardIsNull_MeaningfulErrorIsReceived()
    {
      // Arrange
      var testInstances = this.TestSetupWithDefaultCardHolder();
      var localGameController = testInstances.LocalGameController;

      TurnToken turnToken = null;
      localGameController.StartPlayerTurnEvent = (TurnToken t) => { turnToken = t; };

      localGameController.StartGamePlay();

      ErrorDetails errorDetails = null;
      localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

      // Act
      localGameController.UseYearOfPlentyCard(turnToken, null, ResourceTypes.Brick, ResourceTypes.Brick);

      // Assert
      errorDetails.ShouldNotBeNull();
      errorDetails.Message.ShouldBe("Development card parameter is null.");
    }

    [Test]
    public void UseYearOfPlentyCard_UseCardPurchasedInSameTurn_MeaningfulErrorIsReceived()
    {
      // Arrange
      var yearOfPlentyCard = new YearOfPlentyDevelopmentCard();
      var testInstances = this.TestSetupWithExplictDevelopmentCards(yearOfPlentyCard);
      var localGameController = testInstances.LocalGameController;

      testInstances.MainPlayer.AddResources(ResourceClutch.DevelopmentCard);

      TurnToken turnToken = null;
      localGameController.StartPlayerTurnEvent = (TurnToken t) => { turnToken = t; };

      DevelopmentCard purchasedDevelopmentCard = null;
      localGameController.DevelopmentCardPurchasedEvent = (DevelopmentCard d) => { purchasedDevelopmentCard = d; };

      localGameController.StartGamePlay();
      localGameController.BuyDevelopmentCard(turnToken);

      ErrorDetails errorDetails = null;
      localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

      // Act
      localGameController.UseYearOfPlentyCard(turnToken, (YearOfPlentyDevelopmentCard)purchasedDevelopmentCard, ResourceTypes.Brick, ResourceTypes.Brick);

      // Assert
      errorDetails.ShouldNotBeNull();
      errorDetails.Message.ShouldBe("Cannot use development card that has been purchased this turn.");
    }

    [Test]
    public void UseYearOfPlentyCard_UseDevelopmentCardMoreThanOnce_MeaningfulErrorIsReceived()
    {
      // Arrange
      var yearOfPlentyCard = new YearOfPlentyDevelopmentCard();
      var testInstances = this.TestSetupWithExplictDevelopmentCards(yearOfPlentyCard);
      var localGameController = testInstances.LocalGameController;

      testInstances.Dice.AddSequence(new[] { 8u, 8u });
      testInstances.MainPlayer.AddResources(ResourceClutch.DevelopmentCard);

      TurnToken turnToken = null;
      localGameController.StartPlayerTurnEvent = (TurnToken t) => { turnToken = t; };

      localGameController.StartGamePlay();

      // Buy the year of plenty card
      localGameController.BuyDevelopmentCard(turnToken);
      localGameController.EndTurn(turnToken);

      // Play the same year of plenty card each turn for the next two turns
      localGameController.UseYearOfPlentyCard(turnToken, yearOfPlentyCard, ResourceTypes.Brick, ResourceTypes.Brick);
      localGameController.EndTurn(turnToken);

      ErrorDetails errorDetails = null;
      localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

      // Act
      localGameController.UseYearOfPlentyCard(turnToken, yearOfPlentyCard, ResourceTypes.Brick, ResourceTypes.Brick);

      // Assert
      errorDetails.ShouldNotBeNull();
      errorDetails.Message.ShouldBe("Cannot play the same development card more than once.");
    }

    [Test]
    public void UseYearOfPlentyCard_UseDevelopmentCard_ResourcesAreCollected()
    {
      // Arrange
      var yearOfPlentyCard = new YearOfPlentyDevelopmentCard();
      var testInstances = this.TestSetupWithExplictGameBoard(yearOfPlentyCard, new MockGameBoardWithNoResourcesCollected());
      var localGameController = testInstances.LocalGameController;
      var player = testInstances.MainPlayer;

      testInstances.Dice.AddSequence(new[] { 8u });
      testInstances.MainPlayer.AddResources(ResourceClutch.DevelopmentCard);

      TurnToken turnToken = null;
      localGameController.StartPlayerTurnEvent = (TurnToken t) => { turnToken = t; };

      ResourceTransactionList resources = null;
      localGameController.ResourcesTransferredEvent = (ResourceTransactionList r) => { resources = r; };

      localGameController.StartGamePlay();

      // Buy the year of plenty card
      localGameController.BuyDevelopmentCard(turnToken);
      localGameController.EndTurn(turnToken);

      // Act
      localGameController.UseYearOfPlentyCard(turnToken, yearOfPlentyCard, ResourceTypes.Brick, ResourceTypes.Brick);

      // Assert
      var expected = new ResourceTransactionList();
      expected.Add(new ResourceTransaction(player.Id, Guid.Empty, ResourceClutch.OneBrick * 2));

      resources.ShouldNotBeNull();
      AssertToolBox.AssertThatTheResourceTransactionListIsAsExpected(resources, expected);
      player.ResourcesCount.ShouldBe(2);
      player.BrickCount.ShouldBe(2);
    }

    [Test]
    public void Scenario_OpponentUsesYearOfPlentyCardAndGetsResources()
    {
      // Arrange
      var yearOfPlentyCard = new YearOfPlentyDevelopmentCard();
      var testInstances = this.TestSetupWithExplictGameBoard(yearOfPlentyCard, new MockGameBoardWithNoResourcesCollected());
      var localGameController = testInstances.LocalGameController;

      testInstances.Dice.AddSequence(new[] { 8u });

      var player = testInstances.MainPlayer;
      var firstOpponent = testInstances.FirstOpponent;
      var secondOpponent = testInstances.SecondOpponent;
      var thirdOpponent = testInstances.ThirdOpponent;

      testInstances.Dice.AddSequence(new[] { 3u, 3u });  // Only second opp will collect resources (2 Ore)

      player.AddResources(ResourceClutch.OneBrick);
      firstOpponent.AddResources(ResourceClutch.DevelopmentCard);
      firstOpponent.AddBuyDevelopmentCardChoice(1).EndTurn()
        .AddPlaceMonopolyCardAction(new PlayMonopolyCardAction { ResourceType = ResourceTypes.Brick }).EndTurn();

      secondOpponent.AddResources(new ResourceClutch(2, 1, 1, 1, 1));

      var turn = 0;
      TurnToken turnToken = null;
      localGameController.StartPlayerTurnEvent = (TurnToken t) => { turnToken = t; turn++; };

      var playerActions = new Dictionary<String, List<GameEvent>>();
      var keys = new List<String>();
      localGameController.OpponentActionsEvent = (Guid g, List<GameEvent> e) =>
      {
        var key = turn + "-" + g.ToString();
        keys.Add(key);
        playerActions.Add(key, e);
      };

      localGameController.StartGamePlay();
      localGameController.EndTurn(turnToken); // Opponent buys development card
      localGameController.EndTurn(turnToken); // Opponent plays year of plenty card

      // Assert
      var expectedBuyDevelopmentCardEvent = new BuyDevelopmentCardEvent(firstOpponent.Id);

      var expectedResourceTransactionList = new ResourceTransactionList();
      expectedResourceTransactionList.Add(new ResourceTransaction(firstOpponent.Id, player.Id, ResourceClutch.OneBrick));
      expectedResourceTransactionList.Add(new ResourceTransaction(firstOpponent.Id, secondOpponent.Id, ResourceClutch.OneBrick * 2));
      var expectedPlayMonopolyCardEvent = new PlayMonopolyCardEvent(firstOpponent.Id, expectedResourceTransactionList);

      playerActions.Count.ShouldBe(2);
      keys.Count.ShouldBe(playerActions.Count);
      //this.AssertThatPlayerActionsForTurnAreCorrect(playerActions[keys[0]], expectedBuyDevelopmentCardEvent);
      //this.AssertThatPlayerActionsForTurnAreCorrect(playerActions[keys[1]], expectedPlayMonopolyCardEvent);

      player.ResourcesCount.ShouldBe(0);
      firstOpponent.ResourcesCount.ShouldBe(3);
      firstOpponent.BrickCount.ShouldBe(firstOpponent.ResourcesCount);
      secondOpponent.ResourcesCount.ShouldBe(4);
      secondOpponent.BrickCount.ShouldBe(0);
      thirdOpponent.ResourcesCount.ShouldBe(0);
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

    private LocalGameControllerTestCreator.TestInstances TestSetupWithDefaultCardHolder()
    {
      return this.TestSetupWithExplicitDevelopmentCardHolder(new DevelopmentCardHolder());
    }

    private LocalGameControllerTestCreator.TestInstances TestSetupWithExplicitDevelopmentCardHolder(IDevelopmentCardHolder developmentCardHolder)
    {
      var testInstances = LocalGameControllerTestCreator.CreateTestInstances(developmentCardHolder);
      LocalGameControllerTestSetup.LaunchGameAndCompleteSetup(testInstances.LocalGameController);
      testInstances.Dice.AddSequence(new[] { 8u }); // First turn roll i.e. no robber triggered
      return testInstances;
    }

    private LocalGameControllerTestCreator.TestInstances TestSetupWithExplictDevelopmentCards(DevelopmentCard firstDevelopmentCard, params DevelopmentCard[] otherDevelopmentCards)
    {
      var developmentCardHolder = this.CreateMockCardDevelopmentCardHolder(firstDevelopmentCard, otherDevelopmentCards);
      return this.TestSetupWithExplicitDevelopmentCardHolder(developmentCardHolder);
    }

    private LocalGameControllerTestCreator.TestInstances TestSetupWithExplictGameBoard(DevelopmentCard developmentCard, GameBoardData gameBoard)
    {
      var testInstances = LocalGameControllerTestCreator.CreateTestInstances(
        this.CreateMockCardDevelopmentCardHolder(developmentCard),
        gameBoard);
      var localGameController = testInstances.LocalGameController;
      LocalGameControllerTestSetup.LaunchGameAndCompleteSetup(localGameController);

      testInstances.Dice.AddSequence(new[] { 8u }); // First turn roll i.e. no robber triggered

      return testInstances;
    }
    #endregion 
  }
}
