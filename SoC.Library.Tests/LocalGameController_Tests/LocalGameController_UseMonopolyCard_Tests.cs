
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
        public void UseMonopolyCard_TurnTokenIsNull_MeaningfulErrorIsReceived()
        {
            // Arrange
            var testInstances = this.TestSetup();
            var localGameController = testInstances.LocalGameController;

            localGameController.StartGamePlay();

            ErrorDetails errorDetails = null;
            localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

            // Act
            localGameController.UseMonopolyCard(null, null, ResourceTypes.Brick);

            // Assert
            errorDetails.ShouldNotBeNull();
            errorDetails.Message.ShouldBe("Turn token is null.");
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

            testInstances.Dice.AddSequenceWithRepeatingRoll(null, 6);
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
            var testInstances = this.TestSetup(new MockGameBoardWithNoResourcesCollected(), monopolyCard);
            var localGameController = testInstances.LocalGameController;

            testInstances.Dice.AddSequenceWithRepeatingRoll(null, 8);

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

            gainedResources.ShouldBe(expectedResources);
            player.ResourcesCount.ShouldBe(3);
            player.BrickCount.ShouldBe(3);
            firstOpponent.ResourcesCount.ShouldBe(4);
            firstOpponent.BrickCount.ShouldBe(0);
            secondOpponent.ResourcesCount.ShouldBe(0);
            thirdOpponent.ResourcesCount.ShouldBe(4);
            thirdOpponent.BrickCount.ShouldBe(0);
        }

        [Test]
        public void UseMonopolyCard_UseDevelopmentCardWhenOpponentsHaveNoResourcesOfType_ReceivedNullReference()
        {
            // Arrange
            var monopolyCard = new MonopolyDevelopmentCard();
            var testInstances = this.TestSetup(new MockGameBoardWithNoResourcesCollected(), monopolyCard);
            var localGameController = testInstances.LocalGameController;

            testInstances.Dice.AddSequence(new uint[] { 8, 8, 8, 3, 8 });

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

            var gainedResources = new ResourceTransactionList(); // Ensure that state change can be recognised
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
        public void UseMonopolyCard_GameIsOver_MeaningfulErrorIsReceived()
        {
            // Arrange
            var testInstances = this.TestSetup(new MockGameBoardWithNoResourcesCollected(), new MonopolyDevelopmentCard());
            var localGameController = testInstances.LocalGameController;

            testInstances.Dice.AddSequenceWithRepeatingRoll(null, 6);

            var player = testInstances.MainPlayer;
            player.AddResources(ResourceClutch.RoadSegment * 5);
            player.AddResources(ResourceClutch.Settlement * 3);
            player.AddResources(ResourceClutch.City * 4);
            player.AddResources(ResourceClutch.DevelopmentCard);

            TurnToken turnToken = null;
            localGameController.StartPlayerTurnEvent = (TurnToken t) => { turnToken = t; };

            ErrorDetails errorDetails = null;
            localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

            MonopolyDevelopmentCard monopolyCard = null;
            localGameController.DevelopmentCardPurchasedEvent = (DevelopmentCard d) => { monopolyCard = (MonopolyDevelopmentCard)d; };

            localGameController.StartGamePlay();
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
            localGameController.BuildCity(turnToken, 40); // Got 10VP, Game over event raised

            // Act
            localGameController.UseMonopolyCard(turnToken, monopolyCard, ResourceTypes.Brick);

            // Assert
            errorDetails.ShouldNotBeNull();
            errorDetails.Message.ShouldBe("Cannot use monopoly card. Game is over.");
        }

        [Test]
        public void Scenario_OpponentUsesMonopolyCardAndGetsResourcesFromPlayer()
        {
            // Arrange
            var monopolyCard = new MonopolyDevelopmentCard();
            var testInstances = this.TestSetup(new MockGameBoardWithNoResourcesCollected(), monopolyCard);
            var localGameController = testInstances.LocalGameController;

            testInstances.Dice.AddSequence(new uint[] { 8, 8, 8, 8 });

            var player = testInstances.MainPlayer;
            var firstOpponent = testInstances.FirstOpponent;
            var secondOpponent = testInstances.SecondOpponent;
            var thirdOpponent = testInstances.ThirdOpponent;

            testInstances.Dice.AddSequence(new uint[] { 3, 3, 8, 8 });  // Only second opp will collect resources (2 Ore)

            player.AddResources(ResourceClutch.OneBrick);
            firstOpponent.AddResources(ResourceClutch.DevelopmentCard);
            firstOpponent.AddBuyDevelopmentCardChoice(1).EndTurn()
              .AddPlaceMonopolyCardInstruction(new PlayMonopolyCardInstruction { ResourceType = ResourceTypes.Brick }).EndTurn();

            secondOpponent.AddResources(new ResourceClutch(2, 1, 1, 1, 1));

            var turn = 0;
            TurnToken turnToken = null;
            localGameController.StartPlayerTurnEvent = (TurnToken t) => { turnToken = t; turn++; };

            var gameEvents = new List<List<GameEvent>>();
            localGameController.GameEvents = (List<GameEvent> e) => { gameEvents.Add(e); };

            localGameController.StartGamePlay();
            localGameController.EndTurn(turnToken); // Opponent buys development cards
            localGameController.EndTurn(turnToken); // Opponent plays monopoly cards

            // Assert
            var expectedBuyDevelopmentCardEvent = new BuyDevelopmentCardEvent(firstOpponent.Id);

            var expectedResourceTransactionList = new ResourceTransactionList();
            expectedResourceTransactionList.Add(new ResourceTransaction(firstOpponent.Id, player.Id, ResourceClutch.OneBrick));
            expectedResourceTransactionList.Add(new ResourceTransaction(firstOpponent.Id, secondOpponent.Id, ResourceClutch.OneBrick * 2));
            var expectedPlayMonopolyCardEvent = new PlayMonopolyCardEvent(firstOpponent.Id, expectedResourceTransactionList);

            gameEvents.Count.ShouldBe(15);
            gameEvents[2].Count.ShouldBe(2);
            gameEvents[2][1].ShouldBe(expectedBuyDevelopmentCardEvent);
            gameEvents[9].Count.ShouldBe(2);
            gameEvents[9][1].ShouldBe(expectedPlayMonopolyCardEvent);
            
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

        private LocalGameControllerTestCreator.TestInstances TestSetup()
        {
            return this.TestSetup(new DevelopmentCardHolder(), new GameBoard(BoardSizes.Standard));
        }

        private LocalGameControllerTestCreator.TestInstances TestSetup(IDevelopmentCardHolder developmentCardHolder, GameBoard gameBoard)
        {
            var testInstances = LocalGameControllerTestCreator.CreateTestInstances(null, null, developmentCardHolder, gameBoard);
            testInstances.Dice.AddSequence(new[] { 8u });

            LocalGameControllerTestSetup.LaunchGameAndCompleteSetup(testInstances.LocalGameController);
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
