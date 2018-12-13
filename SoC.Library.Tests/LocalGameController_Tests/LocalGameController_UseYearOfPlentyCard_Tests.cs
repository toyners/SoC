
namespace Jabberwocky.SoC.Library.UnitTests.LocalGameController_Tests
{
    using System;
    using System.Collections.Generic;
    using GameBoards;
    using GameEvents;
    using Interfaces;
    using Jabberwocky.SoC.Library.UnitTests.Extensions;
    using Mock;
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
        public void UseYearOfPlentyCard_TurnTokenIsNull_MeaningfulErrorIsReceived()
        {
            // Arrange
            var testInstances = this.TestSetupWithDefaultCardHolder();
            var localGameController = testInstances.LocalGameController;

            localGameController.StartGamePlay();

            ErrorDetails errorDetails = null;
            localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

            // Act
            localGameController.UseYearOfPlentyCard(null, null, ResourceTypes.Brick, ResourceTypes.Brick);

            // Assert
            errorDetails.ShouldNotBeNull();
            errorDetails.Message.ShouldBe("Turn token is null.");
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

            testInstances.Dice.AddSequenceWithRepeatingRoll(null, 6);
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
        public void UseYearOfPlentyCard_UseDevelopmentCard_DifferentResourcesAreCollected()
        {
            // Arrange
            var bankId = Guid.NewGuid();
            var yearOfPlentyCard = new YearOfPlentyDevelopmentCard();
            var testInstances = this.TestSetupWithExplictGameBoard(bankId, yearOfPlentyCard, new MockGameBoardWithNoResourcesCollected());
            var localGameController = testInstances.LocalGameController;
            var player = testInstances.MainPlayer;

            testInstances.Dice.AddSequenceWithRepeatingRoll(null, 8);
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
            localGameController.UseYearOfPlentyCard(turnToken, yearOfPlentyCard, ResourceTypes.Brick, ResourceTypes.Grain);

            // Assert
            var expected = new ResourceTransactionList();
            expected.Add(new ResourceTransaction(player.Id, bankId, new ResourceClutch(1, 1, 0, 0, 0)));

            resources.ShouldBe(expected);
            player.ResourcesCount.ShouldBe(2);
            player.BrickCount.ShouldBe(1);
            player.GrainCount.ShouldBe(1);
        }

        [Test]
        public void UseYearOfPlentyCard_UseDevelopmentCard_SameResourcesAreCollected()
        {
            // Arrange
            var bankId = Guid.NewGuid();
            var yearOfPlentyCard = new YearOfPlentyDevelopmentCard();
            var testInstances = this.TestSetupWithExplictGameBoard(bankId, yearOfPlentyCard, new MockGameBoardWithNoResourcesCollected());
            var localGameController = testInstances.LocalGameController;
            var player = testInstances.MainPlayer;

            testInstances.Dice.AddSequenceWithRepeatingRoll(null, 8);
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
            expected.Add(new ResourceTransaction(player.Id, bankId, ResourceClutch.OneBrick * 2));

            resources.ShouldBe(expected);
            player.ResourcesCount.ShouldBe(2);
            player.BrickCount.ShouldBe(2);
        }

        [Test]
        public void UseYearOfPlentyCard_GameIsOver_MeaningfulErrorIsReceived()
        {
            // Arrange
            var testInstances = this.TestSetupWithExplictGameBoard(Guid.Empty, new YearOfPlentyDevelopmentCard(), new MockGameBoardWithNoResourcesCollected());
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

            YearOfPlentyDevelopmentCard yearOfPlentyCard = null;
            localGameController.DevelopmentCardPurchasedEvent = (DevelopmentCard d) => { yearOfPlentyCard = (YearOfPlentyDevelopmentCard)d; };

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
            localGameController.UseYearOfPlentyCard(turnToken, yearOfPlentyCard, ResourceTypes.Brick, ResourceTypes.Grain);

            // Assert
            errorDetails.ShouldNotBeNull();
            errorDetails.Message.ShouldBe("Cannot use year of plenty card. Game is over.");
        }

        [Test]
        public void Scenario_OpponentUsesYearOfPlentyCardAndGetsResourcesofDifferentTypes()
        {
            // Arrange
            var bankId = Guid.NewGuid();
            var yearOfPlentyCard = new YearOfPlentyDevelopmentCard();
            var testInstances = this.TestSetupWithExplictGameBoard(bankId, yearOfPlentyCard, new MockGameBoardWithNoResourcesCollected());
            var localGameController = testInstances.LocalGameController;

            testInstances.Dice.AddSequence(new uint[] { 8, 8, 8, 8, 8, 8, 8, 8 });

            var player = testInstances.MainPlayer;
            var firstOpponent = testInstances.FirstOpponent;
            var secondOpponent = testInstances.SecondOpponent;
            var thirdOpponent = testInstances.ThirdOpponent;

            firstOpponent.AddResources(ResourceClutch.DevelopmentCard);
            firstOpponent.AddBuyDevelopmentCardChoice(1).EndTurn()
              .AddPlayYearOfPlentyCardInstruction(new PlayYearOfPlentyCardInstruction { FirstResourceChoice = ResourceTypes.Brick, SecondResourceChoice = ResourceTypes.Grain }).EndTurn();

            var turn = 0;
            TurnToken turnToken = null;
            localGameController.StartPlayerTurnEvent = (TurnToken t) => { turnToken = t; turn++; };

            var gameEvents = new List<List<GameEvent>>();
            localGameController.GameEvents = (List<GameEvent> e) => { gameEvents.Add(e); };

            localGameController.StartGamePlay();
            localGameController.EndTurn(turnToken); // Opponent buys development card
            localGameController.EndTurn(turnToken); // Opponent plays year of plenty card

            // Assert
            var expectedBuyDevelopmentCardEvent = new BuyDevelopmentCardEvent(firstOpponent.Id);

            var expectedResourceTransactionList = new ResourceTransactionList();
            expectedResourceTransactionList.Add(new ResourceTransaction(firstOpponent.Id, bankId, new ResourceClutch(1, 1, 0, 0, 0)));
            var expectedPlayYearOfPlentyCardEvent = new PlayYearOfPlentyCardEvent(firstOpponent.Id, expectedResourceTransactionList);

            gameEvents.Count.ShouldBe(15);
            gameEvents[2].Count.ShouldBe(2);
            gameEvents[2][1].ShouldBe(expectedBuyDevelopmentCardEvent);
            gameEvents[9].Count.ShouldBe(2);
            gameEvents[9][1].ShouldBe(expectedPlayYearOfPlentyCardEvent);

            player.ResourcesCount.ShouldBe(0);
            firstOpponent.ResourcesCount.ShouldBe(2);
            firstOpponent.BrickCount.ShouldBe(1);
            firstOpponent.GrainCount.ShouldBe(1);
            secondOpponent.ResourcesCount.ShouldBe(0);
            secondOpponent.BrickCount.ShouldBe(0);
            thirdOpponent.ResourcesCount.ShouldBe(0);
        }

        [Test]
        public void Scenario_OpponentUsesYearOfPlentyCardAndGetsResourcesofSameType()
        {
            // Arrange
            var bankId = Guid.NewGuid();
            var yearOfPlentyCard = new YearOfPlentyDevelopmentCard();
            var testInstances = this.TestSetupWithExplictGameBoard(bankId, yearOfPlentyCard, new MockGameBoardWithNoResourcesCollected());
            var localGameController = testInstances.LocalGameController;

            testInstances.Dice.AddSequence(new uint[] { 8, 8, 8, 8, 8, 8, 8, 8 });

            var player = testInstances.MainPlayer;
            var firstOpponent = testInstances.FirstOpponent;
            var secondOpponent = testInstances.SecondOpponent;
            var thirdOpponent = testInstances.ThirdOpponent;

            firstOpponent.AddResources(ResourceClutch.DevelopmentCard);
            firstOpponent.AddBuyDevelopmentCardChoice(1).EndTurn()
              .AddPlayYearOfPlentyCardInstruction(new PlayYearOfPlentyCardInstruction { FirstResourceChoice = ResourceTypes.Brick, SecondResourceChoice = ResourceTypes.Brick }).EndTurn();

            var turn = 0;
            TurnToken turnToken = null;
            localGameController.StartPlayerTurnEvent = (TurnToken t) => { turnToken = t; turn++; };

            var gameEvents = new List<List<GameEvent>>();
            localGameController.GameEvents = (List<GameEvent> e) => { gameEvents.Add(e); };

            localGameController.StartGamePlay();
            localGameController.EndTurn(turnToken); // Opponent buys development card
            localGameController.EndTurn(turnToken); // Opponent plays year of plenty card

            // Assert
            var expectedBuyDevelopmentCardEvent = new BuyDevelopmentCardEvent(firstOpponent.Id);

            var expectedResourceTransactionList = new ResourceTransactionList();
            expectedResourceTransactionList.Add(new ResourceTransaction(firstOpponent.Id, bankId, ResourceClutch.OneBrick * 2));
            var expectedPlayYearOfPlentyCardEvent = new PlayYearOfPlentyCardEvent(firstOpponent.Id, expectedResourceTransactionList);

            gameEvents.Count.ShouldBe(15);
            gameEvents[2].Count.ShouldBe(2);
            gameEvents[2][1].ShouldBe(expectedBuyDevelopmentCardEvent);
            gameEvents[9].Count.ShouldBe(2);
            gameEvents[9][1].ShouldBe(expectedPlayYearOfPlentyCardEvent);

            player.ResourcesCount.ShouldBe(0);
            firstOpponent.ResourcesCount.ShouldBe(2);
            firstOpponent.BrickCount.ShouldBe(2);
            secondOpponent.ResourcesCount.ShouldBe(0);
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

        private LocalGameControllerTestCreator.TestInstances TestSetupWithExplictGameBoard(Guid bankId, DevelopmentCard developmentCard, GameBoard gameBoard)
        {
            MockPlayer player;
            MockComputerPlayer firstOpponent, secondOpponent, thirdOpponent;
            LocalGameControllerTestCreator.CreateDefaultPlayerInstances(out player, out firstOpponent, out secondOpponent, out thirdOpponent);
            var playerPool = LocalGameControllerTestCreator.CreateMockPlayerPool(player, firstOpponent, secondOpponent, thirdOpponent);
            playerPool.GetBankId().Returns(bankId);

            var playerSetup = new LocalGameControllerTestCreator.PlayerSetup(player, firstOpponent, secondOpponent, thirdOpponent, playerPool);

            var testInstances = LocalGameControllerTestCreator.CreateTestInstances(
                null,
              playerSetup,
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
