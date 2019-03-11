
namespace Jabberwocky.SoC.Library.UnitTests.LocalGameController_Tests
{
    using System;
    using System.Collections.Generic;
    using GameBoards;
    using GameEvents;
    using Interfaces;
    using Jabberwocky.SoC.Library.DevelopmentCards;
    using Jabberwocky.SoC.Library.UnitTests.Extensions;
    using Mock;
    using NSubstitute;
    using NUnit.Framework;
    using Shouldly;

    [TestFixture]
    [Category("All")]
    [Category("LocalGameController")]
    [Category("LocalGameController.UseKnightCard")]
    public class LocalGameController_UseKnightCard_Tests : LocalGameControllerTestBase
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

            GameToken turnToken = null;
            localGameController.StartPlayerTurnEvent = (GameToken t) => { turnToken = t; };

            localGameController.StartGamePlay();

            ErrorDetails errorDetails = null;
            localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

            // Act
            localGameController.UseKnightCard(new GameToken(), null, 0);

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

            GameToken turnToken = null;
            localGameController.StartPlayerTurnEvent = (GameToken t) => { turnToken = t; };

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

            testInstances.Dice.AddSequence(new uint[] { 8, 8, 8, 8 });
            testInstances.MainPlayer.AddResources(ResourceClutch.DevelopmentCard);

            GameToken turnToken = null;
            localGameController.StartPlayerTurnEvent = (GameToken t) => { turnToken = t; };

            DevelopmentCard purchasedDevelopmentCard = null;
            localGameController.DevelopmentCardPurchasedEvent = (DevelopmentCard d) => { purchasedDevelopmentCard = d; };

            localGameController.StartGamePlay();
            localGameController.BuyDevelopmentCard(turnToken);
            localGameController.EndTurn(turnToken);

            ErrorDetails errorDetails = null;
            localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

            // Act
            localGameController.UseKnightCard(turnToken, (KnightDevelopmentCard)purchasedDevelopmentCard, GameBoards.GameBoard.StandardBoardHexCount);

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

            GameToken turnToken = null;
            localGameController.StartPlayerTurnEvent = (GameToken t) => { turnToken = t; };

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

            testInstances.Dice.AddSequenceWithRepeatingRoll(null, 6);
            testInstances.MainPlayer.AddResources(ResourceClutch.DevelopmentCard);

            GameToken turnToken = null;
            localGameController.StartPlayerTurnEvent = (GameToken t) => { turnToken = t; };

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

            testInstances.Dice.AddSequenceWithRepeatingRoll(null, 6);
            testInstances.MainPlayer.AddResources(ResourceClutch.DevelopmentCard * 2);

            GameToken turnToken = null;
            localGameController.StartPlayerTurnEvent = (GameToken t) => { turnToken = t; };

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

            testInstances.Dice.AddSequenceWithRepeatingRoll(null, 6);
            testInstances.MainPlayer.AddResources(ResourceClutch.DevelopmentCard);

            GameToken turnToken = null;
            localGameController.StartPlayerTurnEvent = (GameToken t) => { turnToken = t; };

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

            testInstances.Dice.AddSequence(new uint[] { 8, 8, 8, 8 });
            testInstances.MainPlayer.AddResources(ResourceClutch.DevelopmentCard);

            GameToken turnToken = null;
            localGameController.StartPlayerTurnEvent = (GameToken t) => { turnToken = t; };

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
        public void UseKnightCard_GotEightVictoryPoints_EndOfGameEventRaisedWithPlayerAsWinner()
        {
            // Arrange
            var testInstances = this.TestSetup(new KnightDevelopmentCard(), new KnightDevelopmentCard(), new KnightDevelopmentCard());
            var localGameController = testInstances.LocalGameController;

            testInstances.Dice.AddSequence(new UInt32[] { 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8 });

            var player = testInstances.MainPlayer;
            player.AddResources(ResourceClutch.RoadSegment * 5);
            player.AddResources(ResourceClutch.Settlement * 2);
            player.AddResources(ResourceClutch.City * 2);
            player.AddResources(ResourceClutch.DevelopmentCard * 3);

            GameToken turnToken = null;
            localGameController.StartPlayerTurnEvent = (GameToken t) => { turnToken = t; };

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

            localGameController.EndTurn(turnToken);

            localGameController.UseKnightCard(turnToken, knightCards.Dequeue(), 4);

            localGameController.EndTurn(turnToken);
            localGameController.UseKnightCard(turnToken, knightCards.Dequeue(), 0);

            localGameController.EndTurn(turnToken);

            // Act
            localGameController.UseKnightCard(turnToken, knightCards.Dequeue(), 4);

            // Assert
            winningPlayer.ShouldBe(player.Id);
            player.VictoryPoints.ShouldBe(10u);
        }

        [Test]
        public void UseKnightCard_Override_GotEightVictoryPoints_EndOfGameEventRaisedWithPlayerAsWinner()
        {
            // Arrange
            var testInstances = this.TestSetup(new KnightDevelopmentCard(), new KnightDevelopmentCard(), new KnightDevelopmentCard());
            var localGameController = testInstances.LocalGameController;

            testInstances.Dice.AddSequenceWithRepeatingRoll(new uint[] { }, 6);

            var player = testInstances.MainPlayer;
            player.AddResources(ResourceClutch.RoadSegment * 5);
            player.AddResources(ResourceClutch.Settlement * 2);
            player.AddResources(ResourceClutch.City * 2);
            player.AddResources(ResourceClutch.DevelopmentCard * 3);

            GameToken turnToken = null;
            localGameController.StartPlayerTurnEvent = (GameToken t) => { turnToken = t; };

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

            localGameController.EndTurn(turnToken);

            localGameController.UseKnightCard(turnToken, knightCards.Dequeue(), 4);

            localGameController.EndTurn(turnToken);
            localGameController.UseKnightCard(turnToken, knightCards.Dequeue(), 0);

            localGameController.EndTurn(turnToken);

            // Act
            localGameController.UseKnightCard(turnToken, knightCards.Dequeue(), 3, testInstances.FirstOpponent.Id);

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

            testInstances.Dice.AddSequence(new UInt32[] { 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8 });

            var player = testInstances.MainPlayer;
            player.AddResources(ResourceClutch.RoadSegment * 5);
            player.AddResources(ResourceClutch.Settlement * 2);
            player.AddResources(ResourceClutch.City * 3);
            player.AddResources(ResourceClutch.DevelopmentCard * 3);

            GameToken turnToken = null;
            localGameController.StartPlayerTurnEvent = (GameToken t) => { turnToken = t; };

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

            localGameController.EndTurn(turnToken);

            localGameController.UseKnightCard(turnToken, knightCards.Dequeue(), 4);

            localGameController.EndTurn(turnToken);
            localGameController.UseKnightCard(turnToken, knightCards.Dequeue(), 0);

            localGameController.EndTurn(turnToken);

            // Act
            localGameController.UseKnightCard(turnToken, knightCards.Dequeue(), 4);

            // Assert
            winningPlayer.ShouldBe(player.Id);
            player.VictoryPoints.ShouldBe(11u);
        }

        [Test]
        public void UseKnightCard_Override_GotNineVictoryPoints_EndOfGameEventRaisedWithPlayerAsWinner()
        {
            // Arrange
            var testInstances = this.TestSetup(new KnightDevelopmentCard(), new KnightDevelopmentCard(), new KnightDevelopmentCard());
            var localGameController = testInstances.LocalGameController;

            testInstances.Dice.AddSequenceWithRepeatingRoll(new uint[] { }, 6);

            var player = testInstances.MainPlayer;
            player.AddResources(ResourceClutch.RoadSegment * 5);
            player.AddResources(ResourceClutch.Settlement * 2);
            player.AddResources(ResourceClutch.City * 3);
            player.AddResources(ResourceClutch.DevelopmentCard * 3);

            testInstances.FirstOpponent.AddResources(ResourceClutch.OneBrick);

            GameToken turnToken = null;
            localGameController.StartPlayerTurnEvent = (GameToken t) => { turnToken = t; };

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

            localGameController.EndTurn(turnToken);

            localGameController.UseKnightCard(turnToken, knightCards.Dequeue(), 4);

            localGameController.EndTurn(turnToken);
            localGameController.UseKnightCard(turnToken, knightCards.Dequeue(), 0);

            localGameController.EndTurn(turnToken);

            // Act
            localGameController.UseKnightCard(turnToken, knightCards.Dequeue(), 3, testInstances.FirstOpponent.Id);

            // Assert
            winningPlayer.ShouldBe(player.Id);
            player.VictoryPoints.ShouldBe(11u);
        }

        [Test]
        public void UseKnightCard_GameIsOver_MeaningfulErrorIsReceived()
        {
            // Arrange
            var testInstances = this.TestSetup(new MockGameBoardWithNoResourcesCollected(), new KnightDevelopmentCard(), new KnightDevelopmentCard(), new KnightDevelopmentCard(), new KnightDevelopmentCard());
            var localGameController = testInstances.LocalGameController;

            testInstances.Dice.AddSequenceWithRepeatingRoll(new uint[] { }, 8);

            var player = testInstances.MainPlayer;
            player.AddResources(ResourceClutch.RoadSegment * 5);
            player.AddResources(ResourceClutch.Settlement * 3);
            player.AddResources(ResourceClutch.City * 4);
            player.AddResources(ResourceClutch.DevelopmentCard * 4);

            GameToken turnToken = null;
            localGameController.StartPlayerTurnEvent = (GameToken t) => { turnToken = t; };

            ErrorDetails errorDetails = null;
            localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

            Queue<KnightDevelopmentCard> knightCards = new Queue<KnightDevelopmentCard>();
            localGameController.DevelopmentCardPurchasedEvent = (DevelopmentCard d) => { knightCards.Enqueue((KnightDevelopmentCard)d); };

            localGameController.StartGamePlay();
            localGameController.BuyDevelopmentCard(turnToken);
            localGameController.EndTurn(turnToken);

            localGameController.BuyDevelopmentCard(turnToken);
            localGameController.EndTurn(turnToken);

            localGameController.BuyDevelopmentCard(turnToken);
            localGameController.EndTurn(turnToken);

            localGameController.BuyDevelopmentCard(turnToken);
            localGameController.EndTurn(turnToken);

            localGameController.BuildRoadSegment(turnToken, 4u, 3u);
            localGameController.BuildRoadSegment(turnToken, 3u, 2u);
            localGameController.BuildRoadSegment(turnToken, 2u, 1u);
            localGameController.BuildRoadSegment(turnToken, 1u, 0u); // Got 2VP for longest road (4VP)
            localGameController.BuildRoadSegment(turnToken, 2u, 10u);

            localGameController.BuildSettlement(turnToken, 3);
            localGameController.BuildSettlement(turnToken, 10);

            localGameController.BuildCity(turnToken, 3);
            localGameController.BuildCity(turnToken, 10);
            localGameController.BuildCity(turnToken, 12);

            localGameController.UseKnightCard(turnToken, knightCards.Dequeue(), 4);
            localGameController.EndTurn(turnToken);

            localGameController.UseKnightCard(turnToken, knightCards.Dequeue(), 0);
            localGameController.EndTurn(turnToken);

            localGameController.UseKnightCard(turnToken, knightCards.Dequeue(), 4); // Got 10VP,  Game over event raised.
            localGameController.EndTurn(turnToken);

            // Act
            localGameController.UseKnightCard(turnToken, knightCards.Dequeue(), 0);

            // Assert
            errorDetails.ShouldNotBeNull();
            errorDetails.Message.ShouldBe("Cannot use knight card. Game is over.");
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
            return this.TestSetup(new DevelopmentCardHolder(), new GameBoard(BoardSizes.Standard));
        }

        private LocalGameControllerTestCreator.TestInstances TestSetup(IDevelopmentCardHolder developmentCardHolder, GameBoard gameBoard)
        {
            var testInstances = LocalGameControllerTestCreator.CreateTestInstances(null, null, developmentCardHolder, gameBoard);
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
            var developmentCardHolder = this.CreateMockCardDevelopmentCardHolder(firstDevelopmentCard, otherDevelopmentCards);
            return this.TestSetup(developmentCardHolder, new GameBoard(BoardSizes.Standard));
        }

        private LocalGameControllerTestCreator.TestInstances TestSetup(GameBoard gameBoardData, DevelopmentCard firstDevelopmentCard, params DevelopmentCard[] otherDevelopmentCards)
        {
            return this.TestSetup(this.CreateMockCardDevelopmentCardHolder(firstDevelopmentCard, otherDevelopmentCards), gameBoardData);
        }
        #endregion
    }
}
