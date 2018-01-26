
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
  [Category("LocalGameController.UseMonopolyCard")]
  public class LocalGameController_UseMonopolyCard_Tests
  { 
    #region Methods
    [Test]
    public void UseMonopolyCard_TurnTokenNotCorrect_MeaningfulErrorIsReceived()
    {
      // Arrange
      var testInstances = this.TestSetup();
      var localGameController = testInstances.LocalGameController;

      TurnToken turnToken = null;
      localGameController.StartPlayerTurnEvent = (TurnToken t) => { turnToken = t; };

      localGameController.StartGamePlay();

      ErrorDetails errorDetails = null;
      localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

      // Act
      localGameController.UseMonopolyCard(new TurnToken(), null, ResourceTypes.Brick);

      // Assert
      errorDetails.ShouldNotBeNull();
      errorDetails.Message.ShouldBe("Turn token not recognised.");
    }

    [Test]
    public void UseMonopolyCard_CardIsNull_MeaningfulErrorIsReceived()
    {
      // Arrange
      var testInstances = this.TestSetup();
      var localGameController = testInstances.LocalGameController;

      TurnToken turnToken = null;
      localGameController.StartPlayerTurnEvent = (TurnToken t) => { turnToken = t; };

      localGameController.StartGamePlay();

      ErrorDetails errorDetails = null;
      localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

      // Act
      localGameController.UseMonopolyCard(turnToken, null, ResourceTypes.Brick);

      // Assert
      errorDetails.ShouldNotBeNull();
      errorDetails.Message.ShouldBe("Development card parameter is null.");
    }

    [Test]
    public void UseMonopolyCard_UseCardPurchasedInSameTurn_MeaningfulErrorIsReceived()
    {
      // Arrange
      var monopolyCard = new MonopolyDevelopmentCard();
      var testInstances = this.TestSetup(monopolyCard);
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
      localGameController.UseMonopolyCard(turnToken, (MonopolyDevelopmentCard)purchasedDevelopmentCard, ResourceTypes.Brick);

      // Assert
      errorDetails.ShouldNotBeNull();
      errorDetails.Message.ShouldBe("Cannot use development card that has been purchased this turn.");
    }

    [Test]
    public void UseMonopolyCard_UseDevelopmentCardMoreThanOnce_MeaningfulErrorIsReceived()
    {
      // Arrange
      var monopolyCard = new MonopolyDevelopmentCard();
      var testInstances = this.TestSetup(monopolyCard);
      var localGameController = testInstances.LocalGameController;

      testInstances.Dice.AddSequence(new[] { 8u, 8u });
      testInstances.MainPlayer.AddResources(ResourceClutch.DevelopmentCard);

      TurnToken turnToken = null;
      localGameController.StartPlayerTurnEvent = (TurnToken t) => { turnToken = t; };

      localGameController.StartGamePlay();

      // Buy the knight cards
      localGameController.BuyDevelopmentCard(turnToken);
      localGameController.EndTurn(turnToken);

      // Play one knight card each turn for the next two turns
      localGameController.UseMonopolyCard(turnToken, monopolyCard, ResourceTypes.Brick);
      localGameController.EndTurn(turnToken);

      ErrorDetails errorDetails = null;
      localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

      // Act
      localGameController.UseMonopolyCard(turnToken, monopolyCard, ResourceTypes.Brick);

      // Assert
      errorDetails.ShouldNotBeNull();
      errorDetails.Message.ShouldBe("Cannot play the same development card more than once.");
    }

    [Test]
    public void UseMonopolyCard_UseDevelopmentCard_ReceiveAllResourcesOfRequestedTypeFromOpponents()
    {
      // Arrange
      var monopolyCard = new MonopolyDevelopmentCard();
      var testInstances = this.TestSetup(monopolyCard, new MockGameBoardWithNoResourcesCollected());
      var localGameController = testInstances.LocalGameController;

      testInstances.Dice.AddSequence(new[] { 3u });

      var player = testInstances.MainPlayer;
      var firstOpponent = testInstances.FirstOpponent;
      var secondOpponent = testInstances.SecondOpponent;
      var thirdOpponent = testInstances.ThirdOpponent;

      player.AddResources(ResourceClutch.DevelopmentCard);
      firstOpponent.AddResources(ResourceClutch.OneOfEach);
      secondOpponent.AddResources(ResourceClutch.OneBrick * 2);
      thirdOpponent.AddResources(new ResourceClutch(0, 1, 1, 1, 1));

      TurnToken turnToken = null;
      localGameController.StartPlayerTurnEvent = (TurnToken t) => { turnToken = t; };

      ResourceTransactionList gainedResources = null;
      localGameController.ResourcesTransferredEvent = (ResourceTransactionList r) => { gainedResources = r; };

      localGameController.StartGamePlay();

      // Buy the monopoly card
      localGameController.BuyDevelopmentCard(turnToken);
      localGameController.EndTurn(turnToken);

      // Act
      localGameController.UseMonopolyCard(turnToken, monopolyCard, ResourceTypes.Brick);

      // Assert
      var expectedResources = new ResourceTransactionList();
      expectedResources.Add(new ResourceTransaction(player.Id, firstOpponent.Id, ResourceClutch.OneBrick));
      expectedResources.Add(new ResourceTransaction(player.Id, secondOpponent.Id, ResourceClutch.OneBrick * 2));

      AssertToolBox.AssertThatTheResourceTransactionListIsAsExpected(gainedResources, expectedResources);
      player.ResourcesCount.ShouldBe(3);
      player.BrickCount.ShouldBe(3);
      firstOpponent.ResourcesCount.ShouldBe(4);
      firstOpponent.BrickCount.ShouldBe(0);
      secondOpponent.ResourcesCount.ShouldBe(0);
      thirdOpponent.ResourcesCount.ShouldBe(4);
      thirdOpponent.BrickCount.ShouldBe(0);
    }

    [Test]
    public void UseMonopolyCard_UseDevelopmentCardWhenOpponentsHaveNoResourcsOfType_ReceiveNullReference()
    {
      // Arrange
      var monopolyCard = new MonopolyDevelopmentCard();
      var testInstances = this.TestSetup(monopolyCard, new MockGameBoardWithNoResourcesCollected());
      var localGameController = testInstances.LocalGameController;

      testInstances.Dice.AddSequence(new[] { 3u });

      var player = testInstances.MainPlayer;
      var firstOpponent = testInstances.FirstOpponent;
      var secondOpponent = testInstances.SecondOpponent;
      var thirdOpponent = testInstances.ThirdOpponent;

      player.AddResources(ResourceClutch.DevelopmentCard);
      firstOpponent.AddResources(ResourceClutch.OneGrain);
      secondOpponent.AddResources(ResourceClutch.OneLumber);
      thirdOpponent.AddResources(ResourceClutch.OneOre);

      TurnToken turnToken = null;
      localGameController.StartPlayerTurnEvent = (TurnToken t) => { turnToken = t; };

      ResourceTransactionList gainedResources = new ResourceTransactionList(); // Ensure that state change can be recognised
      localGameController.ResourcesTransferredEvent = (ResourceTransactionList r) => { gainedResources = r; };

      localGameController.StartGamePlay();

      // Buy the monopoly card
      localGameController.BuyDevelopmentCard(turnToken);
      localGameController.EndTurn(turnToken);

      // Act
      localGameController.UseMonopolyCard(turnToken, monopolyCard, ResourceTypes.Brick);

      // Assert
      gainedResources.ShouldBeNull();
      player.ResourcesCount.ShouldBe(0);
      firstOpponent.ResourcesCount.ShouldBe(1);
      firstOpponent.GrainCount.ShouldBe(1);
      secondOpponent.ResourcesCount.ShouldBe(1);
      secondOpponent.LumberCount.ShouldBe(1);
      thirdOpponent.ResourcesCount.ShouldBe(1);
      thirdOpponent.OreCount.ShouldBe(1);
    }

    [Test]
    public void Scenario_OpponentUsesMonopolyCardAndGetsResourcesFromPlayer()
    {
      // Arrange
      var monopolyCard = new MonopolyDevelopmentCard();
      var testInstances = this.TestSetup(monopolyCard, new MockGameBoardWithNoResourcesCollected());
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
      localGameController.EndTurn(turnToken); // Opponent buys development cards
      localGameController.EndTurn(turnToken); // Opponent plays monopoly cards

      // Assert
      var expectedBuyDevelopmentCardEvent = new BuyDevelopmentCardEvent(firstOpponent.Id);

      var expectedResourceTransactionList = new ResourceTransactionList();
      expectedResourceTransactionList.Add(new ResourceTransaction(firstOpponent.Id, player.Id, ResourceClutch.OneBrick));
      expectedResourceTransactionList.Add(new ResourceTransaction(firstOpponent.Id, secondOpponent.Id, ResourceClutch.OneBrick * 2));
      var expectedPlayMonopolyCardEvent = new PlayMonopolyCardEvent(firstOpponent.Id, expectedResourceTransactionList);

      playerActions.Count.ShouldBe(2);
      keys.Count.ShouldBe(playerActions.Count);
      this.AssertThatPlayerActionsForTurnAreCorrect(playerActions[keys[0]], expectedBuyDevelopmentCardEvent);
       this.AssertThatPlayerActionsForTurnAreCorrect(playerActions[keys[1]], expectedPlayMonopolyCardEvent);

      player.ResourcesCount.ShouldBe(0);
      firstOpponent.ResourcesCount.ShouldBe(3);
      firstOpponent.BrickCount.ShouldBe(firstOpponent.ResourcesCount);
      secondOpponent.ResourcesCount.ShouldBe(4);
      secondOpponent.BrickCount.ShouldBe(0);
      thirdOpponent.ResourcesCount.ShouldBe(0);
    }

    private void AssertThatPlayerActionsForTurnAreCorrect(List<GameEvent> actualEvents, params GameEvent[] expectedEvents)
    {
      actualEvents.Count.ShouldBe(expectedEvents.Length);
      for (var index = 0; index < actualEvents.Count; index++)
      {
        actualEvents[index].ShouldBe(expectedEvents[index], "Index is " + index);
      }
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

    private LocalGameControllerTestCreator.TestInstances TestSetup()
    {
      return this.TestSetup(new DevelopmentCardHolder());
    }

    private LocalGameControllerTestCreator.TestInstances TestSetup(IDevelopmentCardHolder developmentCardHolder)
    {
      var testInstances = LocalGameControllerTestCreator.CreateTestInstances(developmentCardHolder);
      LocalGameControllerTestSetup.LaunchGameAndCompleteSetup(testInstances.LocalGameController);
      testInstances.Dice.AddSequence(new[] { 8u });
      return testInstances;
    }

    private LocalGameControllerTestCreator.TestInstances TestSetup(DevelopmentCard firstDevelopmentCard, params DevelopmentCard[] otherDevelopmentCards)
    {
      var developmentCardHolder = this.CreateMockCardDevelopmentCardHolder(firstDevelopmentCard, otherDevelopmentCards);
      return this.TestSetup(developmentCardHolder);
    }

    private LocalGameControllerTestCreator.TestInstances TestSetup(DevelopmentCard developmentCard, GameBoardData gameBoard)
    {
      var testInstances = LocalGameControllerTestCreator.CreateTestInstances(
        this.CreateMockCardDevelopmentCardHolder(developmentCard),
        gameBoard);
      var localGameController = testInstances.LocalGameController;
      LocalGameControllerTestSetup.LaunchGameAndCompleteSetup(localGameController);

      testInstances.Dice.AddSequence(new[] { 8u });

      return testInstances;
    }
    #endregion 
  }
}
