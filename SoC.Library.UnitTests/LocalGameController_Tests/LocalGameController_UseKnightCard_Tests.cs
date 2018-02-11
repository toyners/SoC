
namespace Jabberwocky.SoC.Library.UnitTests.LocalGameController_Tests
{
  using System;
  using System.Collections.Generic;
  using GameBoards;
  using GameEvents;
  using Interfaces;
  using MockGameBoards;
  using NSubstitute;
  using NUnit.Framework;
  using Shouldly;

  [TestFixture]
  [Category("All")]
  [Category("LocalGameController")]
  [Category("LocalGameController.UseKnightCard")]
  public class LocalGameController_UseKnightCard_Tests
  {
    #region Fields
    private const UInt32 MainSettlementOneHex = 1;
    private const UInt32 SecondSettlementOneHex = 8;
    private const UInt32 HexWithTwoPlayerSettlements = 14;
    private Dictionary<Guid, IPlayer> playersById;
    #endregion

    #region Methods
    [Test]
    public void UseKnightCard_TurnTokenNotCorrect_MeaningfulErrorIsReceived()
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
      localGameController.UseKnightCard(new TurnToken(), null, 0);

      // Assert
      errorDetails.ShouldNotBeNull();
      errorDetails.Message.ShouldBe("Turn token not recognised.");
    }

    [Test]
    public void UseKnightCard_TurnTokenIsNull_MeaningfulErrorIsReceived()
    {
      // Arrange
      var testInstances = this.TestSetup();
      var localGameController = testInstances.LocalGameController;

      localGameController.StartGamePlay();

      ErrorDetails errorDetails = null;
      localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

      // Act
      localGameController.UseKnightCard(null, null, 0);

      // Assert
      errorDetails.ShouldNotBeNull();
      errorDetails.Message.ShouldBe("Turn token is null.");
    }

    [Test]
    public void UseKnightCard_CardIsNull_MeaningfulErrorIsReceived()
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
      localGameController.UseKnightCard(turnToken, null, 0);

      // Assert
      errorDetails.ShouldNotBeNull();
      errorDetails.Message.ShouldBe("Development card parameter is null.");
    }

    [Test]
    public void UseKnightCard_NewRobberHexIsOutOfBounds_MeaningfulErrorIsReceived()
    {
      // Arrange
      var knightCard = new KnightDevelopmentCard();
      var testInstances = this.TestSetup(knightCard);
      var localGameController = testInstances.LocalGameController;

      testInstances.Dice.AddSequence(new[] { 8u });
      testInstances.MainPlayer.AddResources(ResourceClutch.DevelopmentCard);

      TurnToken turnToken = null;
      localGameController.StartPlayerTurnEvent = (TurnToken t) => { turnToken = t; };

      DevelopmentCard purchasedDevelopmentCard = null;
      localGameController.DevelopmentCardPurchasedEvent = (DevelopmentCard d) => { purchasedDevelopmentCard = d; };

      localGameController.StartGamePlay();
      localGameController.BuyDevelopmentCard(turnToken);
      localGameController.EndTurn(turnToken);

      ErrorDetails errorDetails = null;
      localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

      // Act
      localGameController.UseKnightCard(turnToken, (KnightDevelopmentCard)purchasedDevelopmentCard, GameBoards.GameBoardData.StandardBoardHexCount);

      // Assert
      errorDetails.ShouldNotBeNull();
      errorDetails.Message.ShouldBe("Cannot move robber to hex 19 because it is out of bounds (0.. 18).");
    }

    [Test]
    public void UseKnightCard_UseCardPurchasedInSameTurn_MeaningfulErrorIsReceived()
    {
      // Arrange
      var knightCard = new KnightDevelopmentCard();
      var testInstances = this.TestSetup(knightCard);
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
      localGameController.UseKnightCard(turnToken, (KnightDevelopmentCard)purchasedDevelopmentCard, 0);

      // Assert
      errorDetails.ShouldNotBeNull();
      errorDetails.Message.ShouldBe("Cannot use development card that has been purchased this turn.");
    }

    [Test]
    public void UseKnightCard_UseKnightDevelopmentCardAndTryToMoveRobberToSameSpot_MeaningfulErrorIsReceived()
    {
      // Arrange
      var knightCard = new KnightDevelopmentCard();
      var testInstances = this.TestSetup(knightCard);
      var localGameController = testInstances.LocalGameController;

      testInstances.Dice.AddSequence(new[] { 8u });
      testInstances.MainPlayer.AddResources(ResourceClutch.DevelopmentCard);

      TurnToken turnToken = null;
      localGameController.StartPlayerTurnEvent = (TurnToken t) => { turnToken = t; };

      DevelopmentCard purchasedDevelopmentCard = null;
      localGameController.DevelopmentCardPurchasedEvent = (DevelopmentCard d) => { purchasedDevelopmentCard = d; };

      localGameController.StartGamePlay();
      localGameController.BuyDevelopmentCard(turnToken);
      localGameController.EndTurn(turnToken);

      ErrorDetails errorDetails = null;
      localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

      // Act
      localGameController.UseKnightCard(turnToken, (KnightDevelopmentCard)purchasedDevelopmentCard, 0);

      // Assert
      errorDetails.ShouldNotBeNull();
      errorDetails.Message.ShouldBe("Cannot place robber back on present hex (0).");
    }

    [Test]
    public void UseKnightCard_UseMoreThanOneKnightDevelopmentCardInSingleTurn_MeaningfulErrorIsReceived()
    {
      // Arrange
      var knightCard1 = new KnightDevelopmentCard();
      var knightCard2 = new KnightDevelopmentCard();
      var testInstances = this.TestSetup(knightCard1, knightCard2);
      var localGameController = testInstances.LocalGameController;

      testInstances.Dice.AddSequence(new[] { 8u });
      testInstances.MainPlayer.AddResources(ResourceClutch.DevelopmentCard * 2);

      TurnToken turnToken = null;
      localGameController.StartPlayerTurnEvent = (TurnToken t) => { turnToken = t; };

      var purchasedDevelopmentCards = new Queue<DevelopmentCard>();
      localGameController.DevelopmentCardPurchasedEvent = (DevelopmentCard d) => { purchasedDevelopmentCards.Enqueue(d); };

      localGameController.StartGamePlay();
      localGameController.BuyDevelopmentCard(turnToken);
      localGameController.BuyDevelopmentCard(turnToken);
      localGameController.EndTurn(turnToken);
      localGameController.UseKnightCard(turnToken, (KnightDevelopmentCard)purchasedDevelopmentCards.Dequeue(), 3);

      ErrorDetails errorDetails = null;
      localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

      // Act
      localGameController.UseKnightCard(turnToken, (KnightDevelopmentCard)purchasedDevelopmentCards.Dequeue(), 0);

      // Assert
      errorDetails.ShouldNotBeNull();
      errorDetails.Message.ShouldBe("Cannot play more than one development card in a turn.");
    }

    [Test]
    public void UseKnightCard_UseKnightDevelopmentCardMoreThanOnce_MeaningfulErrorIsReceived()
    {
      // Arrange
      var knightCard = new KnightDevelopmentCard();
      var testInstances = this.TestSetup(knightCard);
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
      localGameController.UseKnightCard(turnToken, knightCard, 3);
      localGameController.EndTurn(turnToken);

      ErrorDetails errorDetails = null;
      localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

      // Act
      localGameController.UseKnightCard(turnToken, knightCard, 0);

      // Assert
      errorDetails.ShouldNotBeNull();
      errorDetails.Message.ShouldBe("Cannot play the same development card more than once.");
    }

    [Test]
    public void UseKnightCard_IdParameterDoesNotMatchWithAnyPlayerOnHex_MeaningfulErrorIsReceived()
    {
      // Arrange
      var knightCard = new KnightDevelopmentCard();
      var testInstances = this.TestSetup(knightCard);
      var localGameController = testInstances.LocalGameController;

      testInstances.Dice.AddSequence(new[] { 8u });
      testInstances.MainPlayer.AddResources(ResourceClutch.DevelopmentCard);

      TurnToken turnToken = null;
      localGameController.StartPlayerTurnEvent = (TurnToken t) => { turnToken = t; };

      localGameController.StartGamePlay();

      // Buy the knight cards
      localGameController.BuyDevelopmentCard(turnToken);
      localGameController.EndTurn(turnToken);

      ErrorDetails errorDetails = null;
      localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

      // Act
      localGameController.UseKnightCard(turnToken, knightCard, HexWithTwoPlayerSettlements, testInstances.MainPlayer.Id);

      // Assert
      errorDetails.ShouldNotBeNull();
      errorDetails.Message.ShouldBe("Player Id (" + testInstances.MainPlayer.Id + ") does not match with any players on hex " + HexWithTwoPlayerSettlements + ".");
    }

    [Test]
    public void UseKnightCard_PlayerPlaysThirdKnightDevelopmentCard_LargestArmyEventRaised()
    {
      // Arrange
      var knightCard1 = new KnightDevelopmentCard();
      var knightCard2 = new KnightDevelopmentCard();
      var knightCard3 = new KnightDevelopmentCard();
      var testInstances = this.TestSetup(knightCard1, knightCard2, knightCard3);
      var localGameController = testInstances.LocalGameController;
      var player = testInstances.MainPlayer;

      testInstances.Dice.AddSequence(new[] { 8u, 8u, 8u });
      player.AddResources(ResourceClutch.DevelopmentCard * 3);

      TurnToken turnToken = null;
      localGameController.StartPlayerTurnEvent = (TurnToken t) => { turnToken = t; };

      Guid oldPlayerId = Guid.NewGuid(), newPlayerId = Guid.Empty;
      localGameController.LargestArmyEvent = (Guid op, Guid np) => { oldPlayerId = op; newPlayerId = np; };

      localGameController.StartGamePlay();

      // Buy the knight cards
      localGameController.BuyDevelopmentCard(turnToken);
      localGameController.BuyDevelopmentCard(turnToken);
      localGameController.BuyDevelopmentCard(turnToken);
      localGameController.EndTurn(turnToken);

      // Play one knight card each turn for the next two turns
      localGameController.UseKnightCard(turnToken, knightCard1, 3);
      localGameController.EndTurn(turnToken);

      localGameController.UseKnightCard(turnToken, knightCard2, 0);
      localGameController.EndTurn(turnToken);

      // Act
      localGameController.UseKnightCard(turnToken, knightCard3, 3);

      // Assert
      oldPlayerId.ShouldBe(Guid.Empty);
      this.AssertThatPlayerIdIsCorrect("newPlayer", newPlayerId, player.Id, player.Name);
    }

    /// <summary>
    /// Test that the largest army event is raised when the player has played 3 knight cards. Also
    /// the largest army event is returned once the opponent has played 4 cards.
    /// </summary>
    [Test]
    public void Scenario_LargestArmyEventsRaisedWhenBothPlayerAndOpponentPlayTheMostKnightDevelopmentCards()
    {
      // Arrange
      var knightCard1 = new KnightDevelopmentCard();
      var knightCard2 = new KnightDevelopmentCard();
      var knightCard3 = new KnightDevelopmentCard();
      var knightCard4 = new KnightDevelopmentCard();
      var knightCard5 = new KnightDevelopmentCard();
      var knightCard6 = new KnightDevelopmentCard();
      var knightCard7 = new KnightDevelopmentCard();
      var testInstances = this.TestSetup(knightCard1, knightCard2, knightCard3, knightCard4, knightCard5, knightCard6, knightCard7);
      var localGameController = testInstances.LocalGameController;
      var player = testInstances.MainPlayer;
      var firstOpponent = testInstances.FirstOpponent;

      testInstances.Dice.AddSequence(new[] { 8u, 8u, 8u, 8u, 8u });

      player.AddResources(ResourceClutch.DevelopmentCard * 3);

      var playKnightCardAction = new PlayKnightInstruction { RobberHex = 0, RobbedPlayerId = Guid.Empty };
      firstOpponent.AddResources(ResourceClutch.DevelopmentCard * 4);
      firstOpponent.AddBuyDevelopmentCardChoice(4).EndTurn()
        .AddPlaceKnightCardInstruction(playKnightCardAction).EndTurn()
        .AddPlaceKnightCardInstruction(playKnightCardAction).EndTurn()
        .AddPlaceKnightCardInstruction(playKnightCardAction).EndTurn()
        .AddPlaceKnightCardInstruction(playKnightCardAction).EndTurn();

      var turn = 0;
      TurnToken turnToken = null;
      localGameController.StartPlayerTurnEvent = (TurnToken t) => { turnToken = t; turn++; };

      Guid oldPlayerId = Guid.NewGuid(), newPlayerId = Guid.Empty;
      var expectedTurn = -1;
      localGameController.LargestArmyEvent = (Guid o, Guid n) => { oldPlayerId = o; newPlayerId = n; expectedTurn = turn; };

      var playerActions = new Dictionary<String, List<GameEvent>>();
      var keys = new List<String>();
      localGameController.OpponentActionsEvent = (Guid g, List<GameEvent> e) =>
      {
        var key = turn + "-" + g.ToString();
        keys.Add(key);
        playerActions.Add(key, e);
      };

      localGameController.StartGamePlay();

      // Turn 1: Buy the knight cards
      localGameController.BuyDevelopmentCard(turnToken);
      localGameController.BuyDevelopmentCard(turnToken);
      localGameController.BuyDevelopmentCard(turnToken);
      localGameController.EndTurn(turnToken); // Opponent buys development cards

      // Turn 2: Play knight card
      localGameController.UseKnightCard(turnToken, knightCard1, 3);
      localGameController.EndTurn(turnToken); // Opponent plays knight card

      // Turn 3: Play knight card
      localGameController.UseKnightCard(turnToken, knightCard2, 3);
      localGameController.EndTurn(turnToken); // Opponent plays knight card

      // Turn 4: Play knight card
      localGameController.UseKnightCard(turnToken, knightCard3, 3); // Largest Army event raised
      localGameController.EndTurn(turnToken); // Opponent plays knight card

      // Turn 5: Play knight card
      localGameController.EndTurn(turnToken); // Opponent plays last knight card. Largest Army event returned

      // Assert
      oldPlayerId.ShouldBe(Guid.Empty);
      this.AssertThatPlayerIdIsCorrect("newPlayer", newPlayerId, player.Id, player.Name);

      var expectedBuyDevelopmentCardEvent = new BuyDevelopmentCardEvent(firstOpponent.Id);
      var expectedPlayKnightCardEvent = new PlayKnightCardEvent(firstOpponent.Id);
      var expectedDifferentPlayerHasLargestArmyEvent = new LargestArmyChangedEvent(player.Id, firstOpponent.Id);

      playerActions.Count.ShouldBe(5);
      keys.Count.ShouldBe(5);
      AssertToolBox.AssertThatPlayerActionsForTurnAreCorrect(playerActions[keys[0]], expectedBuyDevelopmentCardEvent, expectedBuyDevelopmentCardEvent, expectedBuyDevelopmentCardEvent, expectedBuyDevelopmentCardEvent);
      AssertToolBox.AssertThatPlayerActionsForTurnAreCorrect(playerActions[keys[1]], expectedPlayKnightCardEvent);
      AssertToolBox.AssertThatPlayerActionsForTurnAreCorrect(playerActions[keys[2]], expectedPlayKnightCardEvent);
      AssertToolBox.AssertThatPlayerActionsForTurnAreCorrect(playerActions[keys[3]], expectedPlayKnightCardEvent);
      AssertToolBox.AssertThatPlayerActionsForTurnAreCorrect(playerActions[keys[4]], expectedPlayKnightCardEvent, expectedDifferentPlayerHasLargestArmyEvent);
    }

    /// <summary>
    /// Test that the largest army event is not returned when the opponent plays knight cards and already has the largest army
    /// </summary>
    [Test]
    public void Scenario_LargestArmyEventOnlyReturnedFirstTimeThatOpponentHasMostKnightCardsPlayed()
    {
      // Arrange
      var knightCard1 = new KnightDevelopmentCard();
      var knightCard2 = new KnightDevelopmentCard();
      var knightCard3 = new KnightDevelopmentCard();
      var knightCard4 = new KnightDevelopmentCard();
      var testInstances = this.TestSetup(knightCard1, knightCard2, knightCard3, knightCard4);
      var localGameController = testInstances.LocalGameController;
      var firstOpponent = testInstances.FirstOpponent;

      testInstances.Dice.AddSequence(new[] { 8u, 8u, 8u, 8u, 8u });

      var playKnightCardAction = new PlayKnightInstruction { RobberHex = 0, RobbedPlayerId = Guid.Empty };
      firstOpponent.AddResources(ResourceClutch.DevelopmentCard * 4);
      firstOpponent.AddBuyDevelopmentCardChoice(4).EndTurn()
        .AddPlaceKnightCardInstruction(playKnightCardAction).EndTurn()
        .AddPlaceKnightCardInstruction(playKnightCardAction).EndTurn()
        .AddPlaceKnightCardInstruction(playKnightCardAction).EndTurn()
        .AddPlaceKnightCardInstruction(playKnightCardAction).EndTurn();

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
      localGameController.EndTurn(turnToken); // Opponent plays knight card
      localGameController.EndTurn(turnToken); // Opponent plays knight card
      localGameController.EndTurn(turnToken); // Opponent plays knight card; raises Largest Army event for Opponent
      localGameController.EndTurn(turnToken); // Opponent plays knight card

      // Assert
      var expectedBuyDevelopmentCardEvent = new BuyDevelopmentCardEvent(firstOpponent.Id);
      var expectedPlayKnightCardEvent = new PlayKnightCardEvent(firstOpponent.Id);
      var expectedDifferentPlayerHasLargestArmyEvent = new LargestArmyChangedEvent(Guid.Empty, firstOpponent.Id);

      playerActions.Count.ShouldBe(5);
      keys.Count.ShouldBe(5);
      AssertToolBox.AssertThatPlayerActionsForTurnAreCorrect(playerActions[keys[0]], expectedBuyDevelopmentCardEvent, expectedBuyDevelopmentCardEvent, expectedBuyDevelopmentCardEvent, expectedBuyDevelopmentCardEvent);
      AssertToolBox.AssertThatPlayerActionsForTurnAreCorrect(playerActions[keys[1]], expectedPlayKnightCardEvent);
      AssertToolBox.AssertThatPlayerActionsForTurnAreCorrect(playerActions[keys[2]], expectedPlayKnightCardEvent);
      AssertToolBox.AssertThatPlayerActionsForTurnAreCorrect(playerActions[keys[3]], expectedPlayKnightCardEvent, expectedDifferentPlayerHasLargestArmyEvent);
      AssertToolBox.AssertThatPlayerActionsForTurnAreCorrect(playerActions[keys[4]], expectedPlayKnightCardEvent);
    }

    /// <summary>
    /// Test that the largest army event is not raised when the player plays knight cards and already has the largest army
    /// </summary>
    [Test]
    public void Scenario_LargestArmyEventOnlyRaisedFirstTimeThatPlayerHasMostKnightCardsPlayed()
    {
      // Arrange
      var knightCard1 = new KnightDevelopmentCard();
      var knightCard2 = new KnightDevelopmentCard();
      var knightCard3 = new KnightDevelopmentCard();
      var knightCard4 = new KnightDevelopmentCard();
      var knightCard5 = new KnightDevelopmentCard();
      var knightCard6 = new KnightDevelopmentCard();
      var knightCard7 = new KnightDevelopmentCard();
      var testInstances = this.TestSetup(knightCard1, knightCard2, knightCard3, knightCard4, knightCard5, knightCard6, knightCard7);
      var localGameController = testInstances.LocalGameController;
      var player = testInstances.MainPlayer;
      var firstOpponent = testInstances.FirstOpponent;

      testInstances.Dice.AddSequence(new[] { 8u, 8u, 8u, 8u });

      player.AddResources(ResourceClutch.DevelopmentCard * 4);

      var playKnightCardAction = new PlayKnightInstruction { RobberHex = 0, RobbedPlayerId = Guid.Empty };
      firstOpponent.AddResources(ResourceClutch.DevelopmentCard * 3);
      firstOpponent.AddBuyDevelopmentCardChoice(3).EndTurn()
        .AddPlaceKnightCardInstruction(playKnightCardAction).EndTurn()
        .AddPlaceKnightCardInstruction(playKnightCardAction).EndTurn()
        .AddPlaceKnightCardInstruction(playKnightCardAction).EndTurn();

      var turn = 0;
      TurnToken turnToken = null;
      localGameController.StartPlayerTurnEvent = (TurnToken t) => { turnToken = t; turn++; };

      Guid newPlayerId = Guid.Empty, oldPlayerId = Guid.Empty;
      var expectedTurn = -1;
      localGameController.LargestArmyEvent = (Guid o, Guid n) => { oldPlayerId = o; newPlayerId = n; expectedTurn = turn; };

      localGameController.StartGamePlay();

      // Turn 1: Buy the knight cards
      localGameController.BuyDevelopmentCard(turnToken);
      localGameController.BuyDevelopmentCard(turnToken);
      localGameController.BuyDevelopmentCard(turnToken);
      localGameController.BuyDevelopmentCard(turnToken);
      localGameController.EndTurn(turnToken); // Opponent buys development cards

      // Turn 2: Play knight card
      localGameController.UseKnightCard(turnToken, knightCard1, 3);
      localGameController.EndTurn(turnToken); // Opponent plays knight card

      // Turn 3: Play kight card
      localGameController.UseKnightCard(turnToken, knightCard2, 3);
      localGameController.EndTurn(turnToken); // Opponent plays knight card

      // Turn 4: Play knight card
      localGameController.UseKnightCard(turnToken, knightCard3, 3); // Largest Army event raised
      localGameController.EndTurn(turnToken); // Opponent plays knight card

      // Turn 5: Play knight card
      localGameController.UseKnightCard(turnToken, knightCard4, 3); // Largest Army event not raised

      // Assert
      expectedTurn.ShouldBe(4);
      oldPlayerId.ShouldBe(Guid.Empty);
      this.AssertThatPlayerIdIsCorrect("newPlayer", newPlayerId, player.Id, player.Name);
    }

    /// <summary>
    /// Test that the transaction between players happens as expected when human plays the knight card and the robber
    /// is moved to a hex populated by two computer players.
    /// </summary>
    [Test]
    public void Scenario_OpponentLosesResourceWhenPlayerPlaysTheKnightCard()
    {
      // Arrange
      var knightCard = new KnightDevelopmentCard();
      var testInstances = this.TestSetup(knightCard, new MockGameBoardWithNoResourcesCollected());
      var localGameController = testInstances.LocalGameController;

      testInstances.Dice.AddSequence(new[] { 3u, 0u });

      var player = testInstances.MainPlayer;
      var firstOpponent = testInstances.FirstOpponent;
      var secondOpponent = testInstances.SecondOpponent;

      player.AddResources(ResourceClutch.DevelopmentCard);
      firstOpponent.AddResources(ResourceClutch.OneBrick);

      TurnToken turnToken = null;
      localGameController.StartPlayerTurnEvent = (TurnToken t) => { turnToken = t; };

      ResourceTransactionList gainedResources = null;
      localGameController.ResourcesTransferredEvent = (ResourceTransactionList r) => { gainedResources = r; };

      localGameController.StartGamePlay();

      // Turn 1: Buy the knight cards
      localGameController.BuyDevelopmentCard(turnToken);
      localGameController.EndTurn(turnToken);

      // Act: Turn 2: Play knight card
      localGameController.UseKnightCard(turnToken, knightCard, SecondSettlementOneHex, firstOpponent.Id);

      // Assert
      var expectedResources = new ResourceTransactionList();
      expectedResources.Add(new ResourceTransaction(player.Id, firstOpponent.Id, ResourceClutch.OneBrick));
      AssertToolBox.AssertThatTheResourceTransactionListIsAsExpected(gainedResources, expectedResources);

      player.ResourcesCount.ShouldBe(1);
      firstOpponent.ResourcesCount.ShouldBe(0);
    }

    [Test]
    public void Scenario_PlayerLosesResourceWhenOpponentPlaysTheKnightCard()
     {
      // Arrange
      var knightCard = new KnightDevelopmentCard();
      var testInstances = this.TestSetup(knightCard, new MockGameBoardWithNoResourcesCollected());
      var localGameController = testInstances.LocalGameController;

      testInstances.Dice.AddSequence(new[] { 8u, 0u, 8u });

      var player = testInstances.MainPlayer;
      var firstOpponent = testInstances.FirstOpponent;

      player.AddResources(ResourceClutch.OneOre);
      firstOpponent.AddResources(ResourceClutch.DevelopmentCard);
      firstOpponent.AddBuyDevelopmentCardChoice(1).EndTurn()
        .AddPlaceKnightCardInstruction(new PlayKnightInstruction { RobberHex = MainSettlementOneHex, RobbedPlayerId = player.Id }).EndTurn();

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

      // Act
      localGameController.EndTurn(turnToken); // Opponent plays knight cards

      // Assert
      var expectedBuyDevelopmentCardEvent = new BuyDevelopmentCardEvent(firstOpponent.Id);
      var expectedPlayKnightCardEvent = new PlayKnightCardEvent(firstOpponent.Id);

      var expectedResourceTransaction = new ResourceTransaction(player.Id, firstOpponent.Id, ResourceClutch.OneOre);
      var expectedResourceTransactionList = new ResourceTransactionList();
      expectedResourceTransactionList.Add(expectedResourceTransaction);
      var expectedResourceLostEvent = new ResourceTransactionEvent(firstOpponent.Id, expectedResourceTransactionList);

      playerActions.Count.ShouldBe(2);
      keys.Count.ShouldBe(playerActions.Count);
      AssertToolBox.AssertThatPlayerActionsForTurnAreCorrect(playerActions[keys[0]], expectedBuyDevelopmentCardEvent);
      AssertToolBox.AssertThatPlayerActionsForTurnAreCorrect(playerActions[keys[1]], expectedPlayKnightCardEvent, expectedResourceLostEvent);
      
      player.ResourcesCount.ShouldBe(0);
      firstOpponent.ResourcesCount.ShouldBe(1);
      firstOpponent.OreCount.ShouldBe(1);
    }

    [Test]
    public void UseKnightCard_GotEightVictoryPoints_EndOfGameEventRaisedWithPlayerAsWinner()
    {
      // Arrange
      var testInstances = this.TestSetup(new KnightDevelopmentCard(), new KnightDevelopmentCard(), new KnightDevelopmentCard());
      var localGameController = testInstances.LocalGameController;
      LocalGameControllerTestSetup.LaunchGameAndCompleteSetup(localGameController);

      testInstances.Dice.AddSequence(new[] { 8u, 8u });

      var player = testInstances.MainPlayer;
      player.AddResources(ResourceClutch.RoadSegment * 5);
      player.AddResources(ResourceClutch.Settlement * 2);
      player.AddResources(ResourceClutch.City * 2);
      player.AddResources(ResourceClutch.DevelopmentCard * 3);

      TurnToken turnToken = null;
      localGameController.StartPlayerTurnEvent = (TurnToken t) => { turnToken = t; };

      Guid winningPlayer = Guid.Empty;
      localGameController.GameOverEvent = (Guid g) => { winningPlayer = g; };

      var knightCards = new Queue<KnightDevelopmentCard>();
      localGameController.DevelopmentCardPurchasedEvent = (DevelopmentCard d) => { knightCards.Enqueue((KnightDevelopmentCard)d); };

      localGameController.StartGamePlay();
      localGameController.BuyDevelopmentCard(turnToken);
      localGameController.BuyDevelopmentCard(turnToken);
      localGameController.BuyDevelopmentCard(turnToken);

      localGameController.BuildRoadSegment(turnToken, 4, 3);
      localGameController.BuildRoadSegment(turnToken, 3, 2);
      localGameController.BuildRoadSegment(turnToken, 2, 1);
      localGameController.BuildRoadSegment(turnToken, 1, 0); // 2VP for longest road (4VP in total)
      localGameController.BuildRoadSegment(turnToken, 4, 5);

      localGameController.BuildSettlement(turnToken, 3);
      localGameController.BuildSettlement(turnToken, 5);

      localGameController.BuildCity(turnToken, 3);
      localGameController.BuildCity(turnToken, 5);

      // Act
      localGameController.UseKnightCard(turnToken, knightCards.Dequeue(), 0);

      localGameController.EndTurn(turnToken);
      localGameController.UseKnightCard(turnToken, knightCards.Dequeue(), 4);

      localGameController.EndTurn(turnToken);

      // Act
      localGameController.UseKnightCard(turnToken, knightCards.Dequeue(), 0);

      // Assert
      winningPlayer.ShouldBe(player.Id);
      player.VictoryPoints.ShouldBe(10u);
    }

    [Test]
    public void UseKnightCard_GotNineVictoryPoints_EndOfGameEventRaisedWithPlayerAsWinner()
    {
      // Arrange
      var testInstances = this.TestSetup(new KnightDevelopmentCard(), new KnightDevelopmentCard(), new KnightDevelopmentCard());
      var localGameController = testInstances.LocalGameController;
      LocalGameControllerTestSetup.LaunchGameAndCompleteSetup(localGameController);

      testInstances.Dice.AddSequence(new[] { 8u, 8u });

      var player = testInstances.MainPlayer;
      player.AddResources(ResourceClutch.RoadSegment * 5);
      player.AddResources(ResourceClutch.Settlement * 2);
      player.AddResources(ResourceClutch.City * 3);
      player.AddResources(ResourceClutch.DevelopmentCard * 3);

      TurnToken turnToken = null;
      localGameController.StartPlayerTurnEvent = (TurnToken t) => { turnToken = t; };

      Guid winningPlayer = Guid.Empty;
      localGameController.GameOverEvent = (Guid g) => { winningPlayer = g; };

      var knightCards = new Queue<KnightDevelopmentCard>();
      localGameController.DevelopmentCardPurchasedEvent = (DevelopmentCard d) => { knightCards.Enqueue((KnightDevelopmentCard)d); };

      localGameController.StartGamePlay();
      localGameController.BuyDevelopmentCard(turnToken);
      localGameController.BuyDevelopmentCard(turnToken);
      localGameController.BuyDevelopmentCard(turnToken);

      localGameController.BuildRoadSegment(turnToken, 4, 3);
      localGameController.BuildRoadSegment(turnToken, 3, 2);
      localGameController.BuildRoadSegment(turnToken, 2, 1);
      localGameController.BuildRoadSegment(turnToken, 1, 0); // 2VP for longest road (4VP in total)
      localGameController.BuildRoadSegment(turnToken, 4, 5);

      localGameController.BuildSettlement(turnToken, 3);
      localGameController.BuildSettlement(turnToken, 5);

      localGameController.BuildCity(turnToken, 3);
      localGameController.BuildCity(turnToken, 5);
      localGameController.BuildCity(turnToken, 12);

      // Act
      localGameController.UseKnightCard(turnToken, knightCards.Dequeue(), 0);

      localGameController.EndTurn(turnToken);
      localGameController.UseKnightCard(turnToken, knightCards.Dequeue(), 4);

      localGameController.EndTurn(turnToken);

      // Act
      localGameController.UseKnightCard(turnToken, knightCards.Dequeue(), 0);

      // Assert
      winningPlayer.ShouldBe(player.Id);
      player.VictoryPoints.ShouldBe(11u);
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

    private void LoadPlayersByIdForCustomAsserts(MockPlayer player, MockComputerPlayer firstOpponent, MockComputerPlayer secondOpponent, MockComputerPlayer thirdOpponent)
    {
      this.playersById = new Dictionary<Guid, IPlayer>();
      this.playersById.Add(player.Id, player);
      this.playersById.Add(firstOpponent.Id, firstOpponent);
      this.playersById.Add(secondOpponent.Id, secondOpponent);
      this.playersById.Add(thirdOpponent.Id, thirdOpponent);
    }

    private LocalGameControllerTestCreator.TestInstances TestSetup()
    {
      return this.TestSetup(new DevelopmentCardHolder(), new GameBoardData(BoardSizes.Standard));
    }

    private LocalGameControllerTestCreator.TestInstances TestSetup(IDevelopmentCardHolder developmentCardHolder, GameBoardData gameBoard)
    {
      var testInstances = LocalGameControllerTestCreator.CreateTestInstances(null, developmentCardHolder, gameBoard);
      testInstances.Dice.AddSequence(new[] { 8u });

      LocalGameControllerTestSetup.LaunchGameAndCompleteSetup(testInstances.LocalGameController);

      this.LoadPlayersByIdForCustomAsserts(testInstances.MainPlayer,
        testInstances.FirstOpponent,
        testInstances.SecondOpponent,
        testInstances.ThirdOpponent);

      return testInstances;
    }

    private LocalGameControllerTestCreator.TestInstances TestSetup(DevelopmentCard firstDevelopmentCard, params DevelopmentCard[] otherDevelopmentCards)
    {
      return this.TestSetup(this.CreateMockCardDevelopmentCardHolder(firstDevelopmentCard, otherDevelopmentCards), new GameBoardData(BoardSizes.Standard));
    }

    private LocalGameControllerTestCreator.TestInstances TestSetup(DevelopmentCard developmentCard, GameBoardData gameBoard)
    {
      return this.TestSetup(this.CreateMockCardDevelopmentCardHolder(developmentCard), gameBoard);
    }
    #endregion
  }
}
