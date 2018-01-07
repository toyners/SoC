
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
  public class LocalGameController_UseKnightDevelopmentCard_Tests : LocalGameControllerTestBase
  {
    #region Fields
    private MockPlayer player;
    private MockComputerPlayer firstOpponent, secondOpponent, thirdOpponent;
    private MockDice dice;
    private LocalGameController localGameController;
    private Dictionary<Guid, IPlayer> playersById;
    #endregion

    #region Methods
    [Test]
    public void UseKnightDevelopmentCard_TurnTokenNotCorrect_MeaningfulErrorIsReceived()
    {
      // Arrange
      this.TestSetup();

      TurnToken turnToken = null;
      this.localGameController.StartPlayerTurnEvent = (TurnToken t) => { turnToken = t; };

      this.localGameController.StartGamePlay();

      ErrorDetails errorDetails = null;
      this.localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

      // Act
      this.localGameController.UseKnightDevelopmentCard(new TurnToken(), null, 0);

      // Assert
      errorDetails.ShouldNotBeNull();
      errorDetails.Message.ShouldBe("Turn token not recognised.");
    }

    [Test]
    public void UseKnightDevelopmentCard_CardIsNull_MeaningfulErrorIsReceived()
    {
      // Arrange
      this.TestSetup();

      TurnToken turnToken = null;
      this.localGameController.StartPlayerTurnEvent = (TurnToken t) => { turnToken = t; };

      this.localGameController.StartGamePlay();

      ErrorDetails errorDetails = null;
      this.localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

      // Act
      this.localGameController.UseKnightDevelopmentCard(turnToken, null, 0);

      // Assert
      errorDetails.ShouldNotBeNull();
      errorDetails.Message.ShouldBe("Knight development card parameter is null.");
    }

    [Test]
    public void UseKnightDevelopmentCard_NewRobberHexIsOutOfBounds_MeaningfulErrorIsReceived()
    {
      // Arrange
      var knightDevelopmentCard = new KnightDevelopmentCard();
      this.TestSetup(knightDevelopmentCard);

      this.player.AddResources(ResourceClutch.DevelopmentCard);

      TurnToken turnToken = null;
      this.localGameController.StartPlayerTurnEvent = (TurnToken t) => { turnToken = t; };

      DevelopmentCard purchasedDevelopmentCard = null;
      this.localGameController.DevelopmentCardPurchasedEvent = (DevelopmentCard d) => { purchasedDevelopmentCard = d; };

      this.localGameController.StartGamePlay();
      this.localGameController.BuyDevelopmentCard(turnToken);
      this.localGameController.EndTurn(turnToken);

      ErrorDetails errorDetails = null;
      this.localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

      // Act
      this.localGameController.UseKnightDevelopmentCard(turnToken, (KnightDevelopmentCard)purchasedDevelopmentCard, GameBoards.GameBoardData.StandardBoardHexCount);

      // Assert
      errorDetails.ShouldNotBeNull();
      errorDetails.Message.ShouldBe("Cannot move robber to hex 19 because it is out of bounds (0.. 18).");
    }

    [Test]
    public void UseKnightDevelopmentCard_UseCardPurchasedInSameTurn_MeaningfulErrorIsReceived()
    {
      // Arrange
      var knightDevelopmentCard = new KnightDevelopmentCard();
      this.TestSetup(knightDevelopmentCard);
      this.player.AddResources(ResourceClutch.DevelopmentCard);

      TurnToken turnToken = null;
      this.localGameController.StartPlayerTurnEvent = (TurnToken t) => { turnToken = t; };

      DevelopmentCard purchasedDevelopmentCard = null;
      this.localGameController.DevelopmentCardPurchasedEvent = (DevelopmentCard d) => { purchasedDevelopmentCard = d; };

      this.localGameController.StartGamePlay();
      this.localGameController.BuyDevelopmentCard(turnToken);

      ErrorDetails errorDetails = null;
      this.localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

      // Act
      this.localGameController.UseKnightDevelopmentCard(turnToken, (KnightDevelopmentCard)purchasedDevelopmentCard, 0);

      // Assert
      errorDetails.ShouldNotBeNull();
      errorDetails.Message.ShouldBe("Cannot use development card that has been purchased this turn.");
    }

    [Test]
    public void UseKnightDevelopmentCard_UseKnightDevelopmentCardAndTryToMoveRobberToSameSpot_MeaningfulErrorIsReceived()
    {
      // Arrange
      var knightDevelopmentCard = new KnightDevelopmentCard();
      this.TestSetup(knightDevelopmentCard);

      this.player.AddResources(ResourceClutch.DevelopmentCard);

      TurnToken turnToken = null;
      this.localGameController.StartPlayerTurnEvent = (TurnToken t) => { turnToken = t; };

      DevelopmentCard purchasedDevelopmentCard = null;
      this.localGameController.DevelopmentCardPurchasedEvent = (DevelopmentCard d) => { purchasedDevelopmentCard = d; };

      this.localGameController.StartGamePlay();
      this.localGameController.BuyDevelopmentCard(turnToken);
      this.localGameController.EndTurn(turnToken);

      ErrorDetails errorDetails = null;
      this.localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

      // Act
      this.localGameController.UseKnightDevelopmentCard(turnToken, (KnightDevelopmentCard)purchasedDevelopmentCard, 0);

      // Assert
      errorDetails.ShouldNotBeNull();
      errorDetails.Message.ShouldBe("Cannot place robber back on present hex (0).");
    }

    [Test]
    public void UseKnightDevelopmentCard_UseMoreThanOneKnightDevelopmentCardInSingleTurn_MeaningfulErrorIsReceived()
    {
      // Arrange
      var knightDevelopmentCard1 = new KnightDevelopmentCard();
      var knightDevelopmentCard2 = new KnightDevelopmentCard();
      this.TestSetup(knightDevelopmentCard1, knightDevelopmentCard2);

      this.player.AddResources(ResourceClutch.DevelopmentCard * 2);

      TurnToken turnToken = null;
      this.localGameController.StartPlayerTurnEvent = (TurnToken t) => { turnToken = t; };

      var purchasedDevelopmentCards = new Queue<DevelopmentCard>();
      this.localGameController.DevelopmentCardPurchasedEvent = (DevelopmentCard d) => { purchasedDevelopmentCards.Enqueue(d); };

      this.localGameController.StartGamePlay();
      this.localGameController.BuyDevelopmentCard(turnToken);
      this.localGameController.BuyDevelopmentCard(turnToken);
      this.localGameController.EndTurn(turnToken);
      this.localGameController.UseKnightDevelopmentCard(turnToken, (KnightDevelopmentCard)purchasedDevelopmentCards.Dequeue(), 3);

      ErrorDetails errorDetails = null;
      this.localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

      // Act
      this.localGameController.UseKnightDevelopmentCard(turnToken, (KnightDevelopmentCard)purchasedDevelopmentCards.Dequeue(), 0);

      // Assert
      errorDetails.ShouldNotBeNull();
      errorDetails.Message.ShouldBe("Cannot play more than one development card in a turn.");
    }

    [Test]
    public void UseKnightDevelopmentCard_UseKnightDevelopmentCardMoreThanOnce_MeaningfulErrorIsReceived()
    {
      // Arrange
      var knightDevelopmentCard = new KnightDevelopmentCard();
      this.TestSetup(knightDevelopmentCard);

      this.player.AddResources(ResourceClutch.DevelopmentCard);

      TurnToken turnToken = null;
      this.localGameController.StartPlayerTurnEvent = (TurnToken t) => { turnToken = t; };

      this.localGameController.StartGamePlay();

      // Buy the knight cards
      this.localGameController.BuyDevelopmentCard(turnToken);
      this.localGameController.EndTurn(turnToken);

      // Play one knight card each turn for the next two turns
      this.localGameController.UseKnightDevelopmentCard(turnToken, knightDevelopmentCard, 3);
      this.localGameController.EndTurn(turnToken);

      ErrorDetails errorDetails = null;
      this.localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

      // Act
      this.localGameController.UseKnightDevelopmentCard(turnToken, knightDevelopmentCard, 0);

      // Assert
      errorDetails.ShouldNotBeNull();
      errorDetails.Message.ShouldBe("Cannot play the same development card more than once.");
    }

    [Test]
    public void UseKnightDevelopmentCard_PlayerPlaysThirdKnightDevelopmentCard_LargestArmyEventRaised()
    {
      // Arrange
      var knightDevelopmentCard1 = new KnightDevelopmentCard();
      var knightDevelopmentCard2 = new KnightDevelopmentCard();
      var knightDevelopmentCard3 = new KnightDevelopmentCard();
      this.TestSetup(knightDevelopmentCard1, knightDevelopmentCard2, knightDevelopmentCard3);

      this.player.AddResources(ResourceClutch.DevelopmentCard * 3);

      TurnToken turnToken = null;
      this.localGameController.StartPlayerTurnEvent = (TurnToken t) => { turnToken = t; };

      Guid oldPlayerId = Guid.NewGuid(), newPlayerId = Guid.Empty;
      this.localGameController.LargestArmyEvent = (Guid op, Guid np) => { oldPlayerId = op; newPlayerId = np; };

      this.localGameController.StartGamePlay();

      // Buy the knight cards
      this.localGameController.BuyDevelopmentCard(turnToken);
      this.localGameController.BuyDevelopmentCard(turnToken);
      this.localGameController.BuyDevelopmentCard(turnToken);
      this.localGameController.EndTurn(turnToken);

      // Play one knight card each turn for the next two turns
      this.localGameController.UseKnightDevelopmentCard(turnToken, knightDevelopmentCard1, 3);
      this.localGameController.EndTurn(turnToken);

      this.localGameController.UseKnightDevelopmentCard(turnToken, knightDevelopmentCard2, 0);
      this.localGameController.EndTurn(turnToken);

      // Act
      this.localGameController.UseKnightDevelopmentCard(turnToken, knightDevelopmentCard3, 3);

      // Assert
      oldPlayerId.ShouldBe(Guid.Empty);
      this.AssertThatPlayerIdIsCorrect("newPlayer", newPlayerId, this.player.Id, this.player.Name);
    }

    /// <summary>
    /// Test that the largest army event is raised when the player has played 3 knight cards. Also
    /// the largest army event is returned once the opponent has played 4 cards.
    /// </summary>
    [Test]
    public void Scenario_LargestArmyEventsRaisedWhenBothPlayerAndOpponentPlayTheMostKnightDevelopmentCards()
    {
      // Arrange
      var knightDevelopmentCard1 = new KnightDevelopmentCard();
      var knightDevelopmentCard2 = new KnightDevelopmentCard();
      var knightDevelopmentCard3 = new KnightDevelopmentCard();
      var knightDevelopmentCard4 = new KnightDevelopmentCard();
      var knightDevelopmentCard5 = new KnightDevelopmentCard();
      var knightDevelopmentCard6 = new KnightDevelopmentCard();
      var knightDevelopmentCard7 = new KnightDevelopmentCard();
      this.TestSetup(knightDevelopmentCard1, knightDevelopmentCard2, knightDevelopmentCard3, knightDevelopmentCard4, knightDevelopmentCard5, knightDevelopmentCard6, knightDevelopmentCard7);

      this.player.AddResources(ResourceClutch.DevelopmentCard * 3);
      this.firstOpponent.AddResources(ResourceClutch.DevelopmentCard * 4);
      this.firstOpponent.AddBuyDevelopmentCardChoice(4).EndTurn()
        .AddPlaceKnightCard(0).EndTurn()
        .AddPlaceKnightCard(0).EndTurn()
        .AddPlaceKnightCard(0).EndTurn()
        .AddPlaceKnightCard(0).EndTurn();

      this.localGameController.ErrorRaisedEvent = (ErrorDetails e) => { throw new Exception(e.Message); };

      var turn = 0;
      TurnToken turnToken = null;
      this.localGameController.StartPlayerTurnEvent = (TurnToken t) => { turnToken = t; turn++; };

      Guid oldPlayerId = Guid.NewGuid(), newPlayerId = Guid.Empty;
      var expectedTurn = -1;
      this.localGameController.LargestArmyEvent = (Guid o, Guid n) => { oldPlayerId = o; newPlayerId = n; expectedTurn = turn; };

      var playerActions = new Dictionary<String, List<GameEvent>>();
      var keys = new List<String>();
      this.localGameController.OpponentActionsEvent = (Guid g, List<GameEvent> e) =>
      {
        var key = turn + "-" + g.ToString();
        keys.Add(key);
        playerActions.Add(key, e);
      };

      this.localGameController.StartGamePlay();

      // Turn 1: Buy the knight cards
      this.localGameController.BuyDevelopmentCard(turnToken);
      this.localGameController.BuyDevelopmentCard(turnToken);
      this.localGameController.BuyDevelopmentCard(turnToken);
      this.localGameController.EndTurn(turnToken); // Opponent buys development cards

      // Turn 2: Play knight card
      this.localGameController.UseKnightDevelopmentCard(turnToken, knightDevelopmentCard1, 3);
      this.localGameController.EndTurn(turnToken); // Opponent plays knight card

      // Turn 3: Play knight card
      this.localGameController.UseKnightDevelopmentCard(turnToken, knightDevelopmentCard2, 3);
      this.localGameController.EndTurn(turnToken); // Opponent plays knight card

      // Turn 4: Play knight card
      this.localGameController.UseKnightDevelopmentCard(turnToken, knightDevelopmentCard3, 3); // Largest Army event raised
      this.localGameController.EndTurn(turnToken); // Opponent plays knight card

      // Turn 5: Play knight card
      this.localGameController.EndTurn(turnToken); // Opponent plays last knight card. Largest Army event returned

      // Assert
      oldPlayerId.ShouldBe(Guid.Empty);
      this.AssertThatPlayerIdIsCorrect("newPlayer", newPlayerId, this.player.Id, this.player.Name);

      var expectedBuyDevelopmentCardEvent = new BuyDevelopmentCardEvent(this.firstOpponent.Id);
      var expectedPlayKnightCardEvent = new PlayKnightCardEvent(this.firstOpponent.Id);
      var expectedDifferentPlayerHasLargestArmyEvent = new PlayerWithLargestArmyChangedEvent(this.player.Id, this.firstOpponent.Id);

      playerActions.Count.ShouldBe(5);
      keys.Count.ShouldBe(5);
      this.AssertThatPlayerActionsForTurnAreCorrect(playerActions[keys[0]], expectedBuyDevelopmentCardEvent, expectedBuyDevelopmentCardEvent, expectedBuyDevelopmentCardEvent, expectedBuyDevelopmentCardEvent);
      this.AssertThatPlayerActionsForTurnAreCorrect(playerActions[keys[1]], expectedPlayKnightCardEvent);
      this.AssertThatPlayerActionsForTurnAreCorrect(playerActions[keys[2]], expectedPlayKnightCardEvent);
      this.AssertThatPlayerActionsForTurnAreCorrect(playerActions[keys[3]], expectedPlayKnightCardEvent);
      this.AssertThatPlayerActionsForTurnAreCorrect(playerActions[keys[4]], expectedPlayKnightCardEvent, expectedDifferentPlayerHasLargestArmyEvent);
    }

    /// <summary>
    /// Test that the largest army event is not returned when the opponent plays knight cards and already has the largest army
    /// </summary>
    [Test]
    public void Scenario_LargestArmyEventOnlyReturnedFirstTimeThatOpponentHasMostKnightCardsPlayed()
    {
      // Arrange
      var knightDevelopmentCard1 = new KnightDevelopmentCard();
      var knightDevelopmentCard2 = new KnightDevelopmentCard();
      var knightDevelopmentCard3 = new KnightDevelopmentCard();
      var knightDevelopmentCard4 = new KnightDevelopmentCard();
      this.TestSetup(knightDevelopmentCard1, knightDevelopmentCard2, knightDevelopmentCard3, knightDevelopmentCard4);

      this.firstOpponent.AddResources(ResourceClutch.DevelopmentCard * 4);
      this.firstOpponent.AddBuyDevelopmentCardChoice(4).EndTurn()
        .AddPlaceKnightCard(0).EndTurn()
        .AddPlaceKnightCard(0).EndTurn()
        .AddPlaceKnightCard(0).EndTurn()
        .AddPlaceKnightCard(0).EndTurn();

      this.localGameController.ErrorRaisedEvent = (ErrorDetails e) => { throw new Exception(e.Message); };

      var turn = 0;
      TurnToken turnToken = null;
      this.localGameController.StartPlayerTurnEvent = (TurnToken t) => { turnToken = t; turn++; };

      var playerActions = new Dictionary<String, List<GameEvent>>();
      var keys = new List<String>();
      this.localGameController.OpponentActionsEvent = (Guid g, List<GameEvent> e) =>
      {
        var key = turn + "-" + g.ToString();
        keys.Add(key);
        playerActions.Add(key, e);
      };

      this.localGameController.StartGamePlay();
      this.localGameController.EndTurn(turnToken); // Opponent buys development cards
      this.localGameController.EndTurn(turnToken); // Opponent plays knight card
      this.localGameController.EndTurn(turnToken); // Opponent plays knight card
      this.localGameController.EndTurn(turnToken); // Opponent plays knight card; raises Largest Army event for Opponent
      this.localGameController.EndTurn(turnToken); // Opponent plays knight card

      // Assert
      var expectedBuyDevelopmentCardEvent = new BuyDevelopmentCardEvent(this.firstOpponent.Id);
      var expectedPlayKnightCardEvent = new PlayKnightCardEvent(this.firstOpponent.Id);
      var expectedDifferentPlayerHasLargestArmyEvent = new PlayerWithLargestArmyChangedEvent(Guid.Empty, this.firstOpponent.Id);

      playerActions.Count.ShouldBe(5);
      keys.Count.ShouldBe(5);
      this.AssertThatPlayerActionsForTurnAreCorrect(playerActions[keys[0]], expectedBuyDevelopmentCardEvent, expectedBuyDevelopmentCardEvent, expectedBuyDevelopmentCardEvent, expectedBuyDevelopmentCardEvent);
      this.AssertThatPlayerActionsForTurnAreCorrect(playerActions[keys[1]], expectedPlayKnightCardEvent);
      this.AssertThatPlayerActionsForTurnAreCorrect(playerActions[keys[2]], expectedPlayKnightCardEvent);
      this.AssertThatPlayerActionsForTurnAreCorrect(playerActions[keys[3]], expectedPlayKnightCardEvent, expectedDifferentPlayerHasLargestArmyEvent);
      this.AssertThatPlayerActionsForTurnAreCorrect(playerActions[keys[4]], expectedPlayKnightCardEvent);
    }

    /// <summary>
    /// Test that the largest army event is not raised when the player plays knight cards and already has the largest army
    /// </summary>
    [Test]
    public void Scenario_LargestArmyEventOnlyRaisedFirstTimeThatPlayerHasMostKnightCardsPlayed()
    {
      // Arrange
      var knightDevelopmentCard1 = new KnightDevelopmentCard();
      var knightDevelopmentCard2 = new KnightDevelopmentCard();
      var knightDevelopmentCard3 = new KnightDevelopmentCard();
      var knightDevelopmentCard4 = new KnightDevelopmentCard();
      var knightDevelopmentCard5 = new KnightDevelopmentCard();
      var knightDevelopmentCard6 = new KnightDevelopmentCard();
      var knightDevelopmentCard7 = new KnightDevelopmentCard();
      this.TestSetup(knightDevelopmentCard1, knightDevelopmentCard2, knightDevelopmentCard3, knightDevelopmentCard4, knightDevelopmentCard5, knightDevelopmentCard6, knightDevelopmentCard7);

      this.player.AddResources(ResourceClutch.DevelopmentCard * 4);
      this.firstOpponent.AddResources(ResourceClutch.DevelopmentCard * 3);
      this.firstOpponent.AddBuyDevelopmentCardChoice(3).EndTurn()
        .AddPlaceKnightCard(0).EndTurn()
        .AddPlaceKnightCard(0).EndTurn()
        .AddPlaceKnightCard(0).EndTurn();

      this.localGameController.ErrorRaisedEvent = (ErrorDetails e) => { throw new Exception(e.Message); };

      var turn = 0;
      TurnToken turnToken = null;
      this.localGameController.StartPlayerTurnEvent = (TurnToken t) => { turnToken = t; turn++; };

      Guid newPlayerId = Guid.Empty, oldPlayerId = Guid.Empty;
      var expectedTurn = -1;
      this.localGameController.LargestArmyEvent = (Guid o, Guid n) => { oldPlayerId = o; newPlayerId = n; expectedTurn = turn; };

      this.localGameController.StartGamePlay();

      // Turn 1: Buy the knight cards
      this.localGameController.BuyDevelopmentCard(turnToken);
      this.localGameController.BuyDevelopmentCard(turnToken);
      this.localGameController.BuyDevelopmentCard(turnToken);
      this.localGameController.BuyDevelopmentCard(turnToken);
      this.localGameController.EndTurn(turnToken); // Opponent buys development cards

      // Turn 2: Play knight card
      this.localGameController.UseKnightDevelopmentCard(turnToken, knightDevelopmentCard1, 3);
      this.localGameController.EndTurn(turnToken); // Opponent plays knight card

      // Turn 3: Play kight card
      this.localGameController.UseKnightDevelopmentCard(turnToken, knightDevelopmentCard2, 3);
      this.localGameController.EndTurn(turnToken); // Opponent plays knight card

      // Turn 4: Play knight card
      this.localGameController.UseKnightDevelopmentCard(turnToken, knightDevelopmentCard3, 3); // Largest Army event raised
      this.localGameController.EndTurn(turnToken); // Opponent plays knight card

      // Turn 5: Play knight card
      this.localGameController.UseKnightDevelopmentCard(turnToken, knightDevelopmentCard4, 3); // Largest Army event not raised

      // Assert
      expectedTurn.ShouldBe(4);
      oldPlayerId.ShouldBe(Guid.Empty);
      this.AssertThatPlayerIdIsCorrect("newPlayer", newPlayerId, this.player.Id, this.player.Name);
    }

    /// <summary>
    /// 
    /// </summary>
    [Test]
    public void Scenario_CompleteTransactionWhenPlayingKnightDevelopmentCards()
    {
      // Arrange
      var knightDevelopmentCard1 = new KnightDevelopmentCard();
      this.TestSetup(knightDevelopmentCard1);

      this.player.AddResources(ResourceClutch.DevelopmentCard * 3);

      this.localGameController.ErrorRaisedEvent = (ErrorDetails e) => { throw new Exception(e.Message); };

      var turn = 0;
      TurnToken turnToken = null;
      this.localGameController.StartPlayerTurnEvent = (TurnToken t) => { turnToken = t; turn++; };

      Dictionary<Guid, Int32> robbingChoices = null;
      this.localGameController.RobbingChoicesEvent = (Dictionary<Guid, Int32> r) => { robbingChoices = r; };

      this.localGameController.StartGamePlay();

      // Turn 1: Buy the knight cards
      this.localGameController.BuyDevelopmentCard(turnToken);
      this.localGameController.EndTurn(turnToken); // Opponent buys development cards

      // Turn 2: Play knight card. Event raised with robbing choices for user to select from
      this.localGameController.UseKnightDevelopmentCard(turnToken, knightDevelopmentCard1, 11);

      // Make choice using call to LGC
      this.localGameController.ChooseResourceFromOpponent(this.firstOpponent.Id, 1);

      // Event raised with card received update
      ResourceClutch gainedResources = ResourceClutch.Zero;
      this.localGameController.ResourcesGainedEvent = (ResourceClutch r) => { gainedResources = r; };

      this.localGameController.EndTurn(turnToken); // Opponent plays knight card

      // Assert
      robbingChoices.ShouldNotBeNull();
      robbingChoices.Count.ShouldBe(1);
      robbingChoices.ShouldContainKeyAndValue(this.firstOpponent.Id, 3);

      gainedResources.Count.ShouldBe(1);

      this.player.ResourcesCount.ShouldBe(4);
      this.firstOpponent.ResourcesCount.ShouldBe(2);
    }

    private void AssertThatPlayerActionsForTurnAreCorrect(List<GameEvent> actualEvents, params GameEvent[] expectedEvents)
    {
      actualEvents.Count.ShouldBe(expectedEvents.Length);
      for (var index = 0; index < actualEvents.Count; index++)
      {
        actualEvents[index].ShouldBe(expectedEvents[index], "Index is " + index);
      }
    }

    private void AssertThatPlayerIdIsCorrect(String variableName, Guid actualPlayerId, Guid expectedPlayerId, String expectedPlayerName)
    {
      if (actualPlayerId != expectedPlayerId)
      {
        var actualPlayerName = (this.playersById.ContainsKey(actualPlayerId) ? this.playersById[actualPlayerId].Name : actualPlayerId.ToString());

        var message = variableName + " should be '" + expectedPlayerName + "' but was '" + actualPlayerName + "'";
        actualPlayerId.ShouldBe(expectedPlayerId, message);
      }
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

      this.playersById = new Dictionary<Guid, IPlayer>();
      this.playersById.Add(this.player.Id, this.player);
      this.playersById.Add(this.firstOpponent.Id, this.firstOpponent);
      this.playersById.Add(this.secondOpponent.Id, this.secondOpponent);
      this.playersById.Add(this.thirdOpponent.Id, this.thirdOpponent);

      var playerPool = this.CreatePlayerPool(this.player, new[] { this.firstOpponent, this.secondOpponent, this.thirdOpponent });
      this.localGameController = this.CreateLocalGameController(dice, playerPool, developmentCardHolder);

      // Throw any errors as exceptions by default
      this.localGameController.ErrorRaisedEvent = (ErrorDetails e) => { throw new Exception(e.Message); };

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
