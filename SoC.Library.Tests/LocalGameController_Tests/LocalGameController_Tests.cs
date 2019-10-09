
namespace Jabberwocky.SoC.Library.UnitTests.LocalGameController_Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using GameBoards;
    using Interfaces;
    using Jabberwocky.SoC.Library.GameEvents;
    using Jabberwocky.SoC.Library.PlayerData;
    using Jabberwocky.SoC.Library.UnitTests.Extensions;
    using Mock;
    using NUnit.Framework;
    using Shouldly;

    [TestFixture]
    [Category("All")]
    [Category("LocalGameController")]
    public class LocalGameController_Tests : LocalGameControllerTestBase
    {
        #region Methods
        [Test]
        [Category("LocalGameController")]
        public void JoinGame_NullGameOptions_OpponentDataPassedBack()
        {
            this.RunOpponentDataPassBackTests(null);
        }

        [Test]
        [Category("LocalGameController")]
        public void JoinGame_DefaultGameOptions_OpponentDataPassedBack()
        {
            this.RunOpponentDataPassBackTests(new GameOptions());
        }

        [Test]
        [Category("LocalGameController")]
        public void TryingToJoinGameMoreThanOnce_MeaningfulErrorIsReceived()
        {
            var player = new MockPlayer(PlayerName);
            var opponents = new[]
            {
        new MockComputerPlayer(FirstOpponentName),
        new MockComputerPlayer(SecondOpponentName),
        new MockComputerPlayer(ThirdOpponentName)
      };

            var mockPlayerFactory = LocalGameControllerTestCreator.CreateMockPlayerPool(player, opponents);

            var localGameController = new LocalGameControllerCreator().ChangePlayerPool(mockPlayerFactory).Create();
            ErrorDetails errorDetails = null;
            localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };
            localGameController.JoinGame();
            localGameController.JoinGame();

            errorDetails.ShouldNotBeNull();
            errorDetails.Message.ShouldBe("Cannot call 'JoinGame' more than once.");
        }

        [Test]
        [Category("LocalGameController")]
        public void LaunchGame_LaunchGameWithoutJoining_MeaningfulErrorIsReceived()
        {
            var localGameController = new LocalGameControllerCreator().Create();
            ErrorDetails errorDetails = null;
            localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };
            localGameController.LaunchGame();

            errorDetails.ShouldNotBeNull();
            errorDetails.Message.ShouldBe("Cannot call 'LaunchGame' without joining game.");
        }

        [Test]
        [Category("LocalGameController")]
        public void LaunchGame_GameLaunchedAfterJoining_InitialBoardPassedBack()
        {
            var mockDice = new MockDiceCreator()
                .AddExplicitDiceRollSequence(new uint[] { 12, 10, 8, 2 })
                .Create();

            var player = new MockPlayer(PlayerName);
            var opponents = new[]
            {
        new MockComputerPlayer(FirstOpponentName),
        new MockComputerPlayer(SecondOpponentName),
        new MockComputerPlayer(ThirdOpponentName)
      };

            var mockPlayerFactory = LocalGameControllerTestCreator.CreateMockPlayerPool(player, opponents);

            var localGameController = new LocalGameControllerCreator().ChangeDice(mockDice).ChangePlayerPool(mockPlayerFactory).Create();

            GameBoardSetup gameBoardData = null;
            localGameController.InitialBoardSetupEvent = (GameBoardSetup g) => { gameBoardData = g; };

            localGameController.JoinGame();
            localGameController.LaunchGame();

            gameBoardData.ShouldNotBeNull();
        }

        [Test]
        [Category("LocalGameController")]
        public void CompleteSetupWithPlayerInFirstSlot_ExpectedPlacementsAreReturned()
        {
            var gameSetupOrder = new[] { 12u, 10u, 8u, 6u };
            var mockDice = new MockDiceCreator()
              .AddExplicitDiceRollSequence(gameSetupOrder)
              .Create();

            var testInstances = LocalGameControllerTestCreator.CreateTestInstances(mockDice);
            var firstOpponent = testInstances.FirstOpponent;
            var secondOpponent = testInstances.SecondOpponent;
            var thirdOpponent = testInstances.ThirdOpponent;
            var localGameController = testInstances.LocalGameController;

            List<GameEvent> gameEvents = null;
            localGameController.GameEvents = (List<GameEvent> e) => { gameEvents = e; };

            localGameController.JoinGame();
            localGameController.LaunchGame();

            localGameController.StartGameSetup();
            gameEvents.Count.ShouldBe(0);

            localGameController.ContinueGameSetup(MainSettlementOneLocation, MainRoadOneEnd);
            gameEvents.Count.ShouldBe(9);
            gameEvents[0].ShouldBe(new InfrastructurePlacedEvent(firstOpponent.Id, FirstSettlementOneLocation, FirstRoadOneEnd));
            gameEvents[1].ShouldBe(new InfrastructurePlacedEvent(secondOpponent.Id, SecondSettlementOneLocation, SecondRoadOneEnd));
            gameEvents[2].ShouldBe(new InfrastructurePlacedEvent(thirdOpponent.Id, ThirdSettlementOneLocation, ThirdRoadOneEnd));
            gameEvents[3].ShouldBe(new InfrastructurePlacedEvent(thirdOpponent.Id, ThirdSettlementTwoLocation, ThirdRoadTwoEnd));
            //gameEvents[4].ShouldBe(new ResourcesCollectedEvent(thirdOpponent.Id, new[] { new ResourceCollection(ThirdSettlementTwoLocation, new ResourceClutch(0, 1, 1, 0, 1)) }));
            gameEvents[5].ShouldBe(new InfrastructurePlacedEvent(secondOpponent.Id, SecondSettlementTwoLocation, SecondRoadTwoEnd));
            //gameEvents[6].ShouldBe(new ResourcesCollectedEvent(secondOpponent.Id, new[] { new ResourceCollection(SecondSettlementTwoLocation, new ResourceClutch(0, 0, 1, 1, 1)) }));
            gameEvents[7].ShouldBe(new InfrastructurePlacedEvent(firstOpponent.Id, FirstSettlementTwoLocation, FirstRoadTwoEnd));
            //gameEvents[8].ShouldBe(new ResourcesCollectedEvent(firstOpponent.Id, new[] { new ResourceCollection(FirstSettlementTwoLocation, new ResourceClutch(0, 1, 1, 0, 1)) }));

            localGameController.CompleteGameSetup(MainSettlementTwoLocation, MainRoadTwoEnd);
            gameEvents.Count.ShouldBe(1);
            //gameEvents[0].ShouldBe(new ResourcesCollectedEvent(testInstances.MainPlayer.Id, new[] { new ResourceCollection(MainSettlementTwoLocation, new ResourceClutch(1, 1, 0, 0, 1)) }));
        }

        [Test]
        [Category("LocalGameController")]
        public void CompleteSetupWithPlayerInSecondSlot_ExpectedPlacementsAreReturned()
        {
            var gameSetupOrder = new[] { 10u, 12u, 8u, 6u };
            var mockDice = new MockDiceCreator()
              .AddExplicitDiceRollSequence(gameSetupOrder)
              .Create();

            var testInstances = LocalGameControllerTestCreator.CreateTestInstances(mockDice);
            var firstOpponent = testInstances.FirstOpponent;
            var secondOpponent = testInstances.SecondOpponent;
            var thirdOpponent = testInstances.ThirdOpponent;
            var localGameController = testInstances.LocalGameController;

            var gameEvents = new List<GameEvent>();
            localGameController.GameEvents = (List<GameEvent> e) => { gameEvents = e; };

            localGameController.JoinGame();
            localGameController.LaunchGame();

            localGameController.StartGameSetup();
            gameEvents.Count.ShouldBe(1);
            gameEvents[0].ShouldBe(new InfrastructurePlacedEvent(firstOpponent.Id, FirstSettlementOneLocation, FirstRoadOneEnd));

            gameEvents.Clear();
            localGameController.ContinueGameSetup(MainSettlementOneLocation, MainRoadOneEnd);

            gameEvents.Count.ShouldBe(6);
            gameEvents[0].ShouldBe(new InfrastructurePlacedEvent(secondOpponent.Id, SecondSettlementOneLocation, SecondRoadOneEnd));
            gameEvents[1].ShouldBe(new InfrastructurePlacedEvent(thirdOpponent.Id, ThirdSettlementOneLocation, ThirdRoadOneEnd));
            gameEvents[2].ShouldBe(new InfrastructurePlacedEvent(thirdOpponent.Id, ThirdSettlementTwoLocation, ThirdRoadTwoEnd));
            //gameEvents[3].ShouldBe(new ResourcesCollectedEvent(thirdOpponent.Id, new[] { new ResourceCollection(ThirdSettlementTwoLocation, new ResourceClutch(0, 1, 1, 0, 1)) }));
            gameEvents[4].ShouldBe(new InfrastructurePlacedEvent(secondOpponent.Id, SecondSettlementTwoLocation, SecondRoadTwoEnd));
            //gameEvents[5].ShouldBe(new ResourcesCollectedEvent(secondOpponent.Id, new[] { new ResourceCollection(SecondSettlementTwoLocation, new ResourceClutch(0, 0, 1, 1, 1)) }));

            gameEvents.Clear();
            localGameController.CompleteGameSetup(MainSettlementTwoLocation, MainRoadTwoEnd);

            gameEvents.Count.ShouldBe(3);
            //gameEvents[0].ShouldBe(new ResourcesCollectedEvent(testInstances.MainPlayer.Id, new[] { new ResourceCollection(MainSettlementTwoLocation, new ResourceClutch(1, 1, 0, 0, 1)) }));
            gameEvents[1].ShouldBe(new InfrastructurePlacedEvent(firstOpponent.Id, FirstSettlementTwoLocation, FirstRoadTwoEnd));
            //gameEvents[2].ShouldBe(new ResourcesCollectedEvent(firstOpponent.Id, new[] { new ResourceCollection(FirstSettlementTwoLocation, new ResourceClutch(0, 1, 1, 0, 1)) }));
        }

        [Test]
        [Category("LocalGameController")]
        public void CompleteSetupWithPlayerInThirdSlot_ExpectedPlacementsAreReturned()
        {
            var gameSetupOrders = new[] { 8u, 12u, 10u, 6u };
            var mockDice = new MockDiceCreator()
              .AddExplicitDiceRollSequence(gameSetupOrders)
              .Create();

            var testInstances = LocalGameControllerTestCreator.CreateTestInstances(mockDice);
            var firstOpponent = testInstances.FirstOpponent;
            var secondOpponent = testInstances.SecondOpponent;
            var thirdOpponent = testInstances.ThirdOpponent;
            var localGameController = testInstances.LocalGameController;

            var gameEvents = new List<GameEvent>();
            localGameController.GameEvents = (List<GameEvent> e) => { gameEvents = e; };

            localGameController.JoinGame();
            localGameController.LaunchGame();

            localGameController.StartGameSetup();
            gameEvents.Count.ShouldBe(2);
            gameEvents[0].ShouldBe(new InfrastructurePlacedEvent(firstOpponent.Id, FirstSettlementOneLocation, FirstRoadOneEnd));
            gameEvents[1].ShouldBe(new InfrastructurePlacedEvent(secondOpponent.Id, SecondSettlementOneLocation, SecondRoadOneEnd));

            localGameController.ContinueGameSetup(MainSettlementOneLocation, MainRoadOneEnd);
            gameEvents.Count.ShouldBe(3);
            gameEvents[0].ShouldBe(new InfrastructurePlacedEvent(thirdOpponent.Id, ThirdSettlementOneLocation, ThirdRoadOneEnd));
            gameEvents[1].ShouldBe(new InfrastructurePlacedEvent(thirdOpponent.Id, ThirdSettlementTwoLocation, ThirdRoadTwoEnd));
            //gameEvents[2].ShouldBe(new ResourcesCollectedEvent(thirdOpponent.Id, new[] { new ResourceCollection(ThirdSettlementTwoLocation, new ResourceClutch(0, 1, 1, 0, 1)) }));

            localGameController.CompleteGameSetup(MainSettlementTwoLocation, MainRoadTwoEnd);
            gameEvents.Count.ShouldBe(5);
            //gameEvents[0].ShouldBe(new ResourcesCollectedEvent(testInstances.MainPlayer.Id, new[] { new ResourceCollection(MainSettlementTwoLocation, new ResourceClutch(1, 1, 0, 0, 1)) }));
            gameEvents[1].ShouldBe(new InfrastructurePlacedEvent(secondOpponent.Id, SecondSettlementTwoLocation, SecondRoadTwoEnd));
           // gameEvents[2].ShouldBe(new ResourcesCollectedEvent(secondOpponent.Id, new[] { new ResourceCollection(SecondSettlementTwoLocation, new ResourceClutch(0, 0, 1, 1, 1)) }));
            gameEvents[3].ShouldBe(new InfrastructurePlacedEvent(firstOpponent.Id, FirstSettlementTwoLocation, FirstRoadTwoEnd));
            //gameEvents[4].ShouldBe(new ResourcesCollectedEvent(firstOpponent.Id, new[] { new ResourceCollection(FirstSettlementTwoLocation, new ResourceClutch(0, 1, 1, 0, 1)) }));
        }

        [Test]
        [Category("LocalGameController")]
        public void CompleteSetupWithPlayerInFourthSlot_ExpectedPlacementsAreReturned()
        {
            var gameSetupOrder = new[] { 6u, 12u, 10u, 8u };
            var mockDice = new MockDiceCreator()
              .AddExplicitDiceRollSequence(gameSetupOrder)
              .Create();

            var testInstances = LocalGameControllerTestCreator.CreateTestInstances(mockDice);
            var firstOpponent = testInstances.FirstOpponent;
            var secondOpponent = testInstances.SecondOpponent;
            var thirdOpponent = testInstances.ThirdOpponent;
            var localGameController = testInstances.LocalGameController;

            var gameEvents = new List<GameEvent>();
            localGameController.GameEvents = (List<GameEvent> e) => { gameEvents = e; };

            localGameController.JoinGame();
            localGameController.LaunchGame();

            localGameController.StartGameSetup();
            gameEvents.Count.ShouldBe(3);
            gameEvents[0].ShouldBe(new InfrastructurePlacedEvent(firstOpponent.Id, FirstSettlementOneLocation, FirstRoadOneEnd));
            gameEvents[1].ShouldBe(new InfrastructurePlacedEvent(secondOpponent.Id, SecondSettlementOneLocation, SecondRoadOneEnd));
            gameEvents[2].ShouldBe(new InfrastructurePlacedEvent(thirdOpponent.Id, ThirdSettlementOneLocation, ThirdRoadOneEnd));

            localGameController.ContinueGameSetup(MainSettlementOneLocation, MainRoadOneEnd);
            gameEvents.Count.ShouldBe(0);

            localGameController.CompleteGameSetup(MainSettlementTwoLocation, MainRoadTwoEnd);
            gameEvents.Count.ShouldBe(7);
            //gameEvents[0].ShouldBe(new ResourcesCollectedEvent(testInstances.MainPlayer.Id, new[] { new ResourceCollection(MainSettlementTwoLocation, new ResourceClutch(1, 1, 0, 0, 1)) }));
            gameEvents[1].ShouldBe(new InfrastructurePlacedEvent(thirdOpponent.Id, ThirdSettlementTwoLocation, ThirdRoadTwoEnd));
            //gameEvents[2].ShouldBe(new ResourcesCollectedEvent(thirdOpponent.Id, new[] { new ResourceCollection(ThirdSettlementTwoLocation, new ResourceClutch(0, 1, 1, 0, 1)) }));
            gameEvents[3].ShouldBe(new InfrastructurePlacedEvent(secondOpponent.Id, SecondSettlementTwoLocation, SecondRoadTwoEnd));
            //gameEvents[4].ShouldBe(new ResourcesCollectedEvent(secondOpponent.Id, new[] { new ResourceCollection(SecondSettlementTwoLocation, new ResourceClutch(0, 0, 1, 1, 1)) }));
            gameEvents[5].ShouldBe(new InfrastructurePlacedEvent(firstOpponent.Id, FirstSettlementTwoLocation, FirstRoadTwoEnd));
            //gameEvents[6].ShouldBe(new ResourcesCollectedEvent(firstOpponent.Id, new[] { new ResourceCollection(FirstSettlementTwoLocation, new ResourceClutch(0, 1, 1, 0, 1)) }));
        }

        [Test]
        public void CompleteSetupForPlayer_PlayerGetsExpectedStartingVictoryPoints()
        {
            var testInstances = LocalGameControllerTestCreator.CreateTestInstances();
            var localGameController = testInstances.LocalGameController;
            LocalGameControllerTestSetup.LaunchGameAndCompleteSetup(localGameController);

            testInstances.MainPlayer.VictoryPoints.ShouldBe(2u);
        }

        [Test]
        public void CompleteSetupForComputerPlayers_ComputerPlayersGetExpectedResources()
        {
            var testInstances = LocalGameControllerTestCreator.CreateTestInstances();
            LocalGameControllerTestSetup.LaunchGameAndCompleteSetup(testInstances.LocalGameController);

            var firstOpponent = testInstances.FirstOpponent;
            var secondOpponent = testInstances.SecondOpponent;
            var thirdOpponent = testInstances.ThirdOpponent;

            firstOpponent.Resources.GrainCount.ShouldBe(1);
            firstOpponent.Resources.LumberCount.ShouldBe(1);
            firstOpponent.Resources.WoolCount.ShouldBe(1);

            secondOpponent.Resources.LumberCount.ShouldBe(1);
            secondOpponent.Resources.OreCount.ShouldBe(1);
            secondOpponent.Resources.WoolCount.ShouldBe(1);

            thirdOpponent.Resources.GrainCount.ShouldBe(1);
            thirdOpponent.Resources.LumberCount.ShouldBe(1);
            thirdOpponent.Resources.WoolCount.ShouldBe(1);
        }

        [Test]
        public void CompleteSetupForComputerPlayers_ComputerPlayersGetExpectedStartingVictoryPoints()
        {
            var testInstances = LocalGameControllerTestCreator.CreateTestInstances();
            LocalGameControllerTestSetup.LaunchGameAndCompleteSetup(testInstances.LocalGameController);

            testInstances.FirstOpponent.VictoryPoints.ShouldBe(2u);
            testInstances.SecondOpponent.VictoryPoints.ShouldBe(2u);
            testInstances.ThirdOpponent.VictoryPoints.ShouldBe(2u);
        }

        [Test]
        [Category("LocalGameController")]
        public void ContinueGameSetup_CallOutOfSequence_MeaningfulErrorIsReceived()
        {
            var localGameController = new LocalGameControllerCreator().Create();
            ErrorDetails errorDetails = null;
            localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

            localGameController.ContinueGameSetup(0u, 1u);

            errorDetails.ShouldNotBeNull();
            errorDetails.Message.ShouldBe("Cannot call 'ContinueGameSetup' until 'StartGameSetup' has completed.");
        }

        [Test]
        [Category("LocalGameController")]
        public void CompleteGameSetup_CallOutOfSequence_MeaningfulErrorIsReceived()
        {
            var localGameController = new LocalGameControllerCreator().Create();
            ErrorDetails errorDetails = null;
            localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

            localGameController.CompleteGameSetup(0u, 1u);

            errorDetails.ShouldNotBeNull();
            errorDetails.Message.ShouldBe("Cannot call 'CompleteGameSetup' until 'ContinueGameSetup' has completed.");
        }

        [Test]
        [Category("LocalGameController")]
        public void PlayerSelectsSameLocationAsComputerPlayerDuringFirstSetupRound_MeaningfulErrorIsReceived()
        {
            var mockDice = new MockDiceCreator()
                .AddExplicitDiceRollSequence(new uint[] { 10, 12, 8, 6 })
                .Create();

            var localGameController = LocalGameControllerTestCreator.CreateTestInstances(mockDice, null, null, null)
                .LocalGameController;

            ErrorDetails exception = null;
            localGameController.ErrorRaisedEvent = (ErrorDetails e) => { exception = e; };

            localGameController.JoinGame();
            localGameController.LaunchGame();
            localGameController.StartGameSetup();

            localGameController.ContinueGameSetup(FirstSettlementOneLocation, 1u);

            exception.ShouldNotBeNull();
            exception.Message.ShouldBe("Cannot build settlement. Location " + FirstSettlementOneLocation + " already settled by player '" + FirstOpponentName + "'.");
        }

        [Test]
        [Category("LocalGameController")]
        public void PlayerSelectsSameLocationAsComputerPlayerDuringSecondSetupRound_MeaningfulErrorIsReceived()
        {
            var localGameController = LocalGameControllerTestCreator.CreateTestInstances()
                .LocalGameController;

                
            ErrorDetails exception = null;
            localGameController.ErrorRaisedEvent = (ErrorDetails e) => { exception = e; };

            localGameController.JoinGame();
            localGameController.LaunchGame();
            localGameController.StartGameSetup();
            localGameController.ContinueGameSetup(0, 1);

            localGameController.CompleteGameSetup(FirstSettlementOneLocation, 1);

            exception.ShouldNotBeNull();
            exception.Message.ShouldBe("Cannot build settlement. Location " + FirstSettlementOneLocation + " already settled by player '" + FirstOpponentName + "'.");
        }

        [Test]
        [Category("LocalGameController")]
        public void PlayerSelectsLocationTooCloseToComputerPlayerDuringFirstSetupRound_MeaningfulErrorIsReceived()
        {
            var mockDice = new MockDiceCreator()
                .AddExplicitDiceRollSequence(new uint[] { 10, 12, 8, 6 })
                .Create();

            MockPlayer player;
            MockComputerPlayer firstOpponent, secondOpponent, thirdOpponent;
            this.CreateDefaultPlayerInstances(out player, out firstOpponent, out secondOpponent, out thirdOpponent);

            var localGameController = this.CreateLocalGameController(mockDice, player, firstOpponent, secondOpponent, thirdOpponent);

            ErrorDetails exception = null;
            localGameController.ErrorRaisedEvent = (ErrorDetails e) => { exception = e; };
            localGameController.JoinGame();
            localGameController.LaunchGame();
            localGameController.StartGameSetup();

            localGameController.ContinueGameSetup(19u, 1u);

            exception.ShouldNotBeNull();
            exception.Message.ShouldBe("Cannot build settlement. Too close to player '" + FirstOpponentName + "' at location " + FirstSettlementOneLocation + ".");
        }

        [Test]
        [Category("LocalGameController")]
        public void PlayerSelectsLocationTooCloseToComputerPlayerDuringSecondSetupRound_MeaningfulErrorIsReceived()
        {

            var localGameController = LocalGameControllerTestCreator.CreateTestInstances()
                .LocalGameController;

            ErrorDetails exception = null;
            localGameController.ErrorRaisedEvent = (ErrorDetails e) => { exception = e; };
            localGameController.JoinGame();
            localGameController.LaunchGame();
            localGameController.StartGameSetup();
            localGameController.ContinueGameSetup(0, 1);

            localGameController.CompleteGameSetup(19, 18);

            exception.ShouldNotBeNull();
            exception.Message.ShouldBe("Cannot build settlement. Too close to player '" + FirstOpponentName + "' at location " + FirstSettlementOneLocation + ".");
        }

        [Test]
        [Category("LocalGameController")]
        public void PlayerPlacesSettlementOffGameBoardDuringFirstSetupRound_MeaningfulErrorReceived()
        {
            var localGameController = LocalGameControllerTestCreator.CreateTestInstances()
                .LocalGameController;

            ErrorDetails errorDetails = null;
            localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

            localGameController.JoinGame();
            localGameController.LaunchGame();
            localGameController.StartGameSetup();
            localGameController.ContinueGameSetup(100, 101);

            errorDetails.ShouldNotBeNull();
            errorDetails.Message.ShouldBe("Cannot build settlement. Location 100 is outside of board range (0 - 53).");
        }

        [Test]
        [Category("LocalGameController")]
        public void PlayerPlacesRoadOverEdgeOfGameBoardDuringFirstSetupRound_MeaningfulErrorIsReceived()
        {
            var localGameController = LocalGameControllerTestCreator.CreateTestInstances()
                .LocalGameController;

            ErrorDetails errorDetails = null;
            localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

            localGameController.JoinGame();
            localGameController.LaunchGame();
            localGameController.StartGameSetup();
            localGameController.ContinueGameSetup(53, 54);

            errorDetails.ShouldNotBeNull();
            errorDetails.Message.ShouldBe("Cannot build road segment. Locations 53 and/or 54 are outside of board range (0 - 53).");
        }

        [Test]
        [Category("LocalGameController")]
        public void PlayerPlacesRoadWhereNoConnectionExistsDuringFirstSetupRound_MeaningfulErrorIsReceived()
        {
            var localGameController = LocalGameControllerTestCreator.CreateTestInstances()
                .LocalGameController;

            ErrorDetails errorDetails = null;
            localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

            localGameController.JoinGame();
            localGameController.LaunchGame();
            localGameController.StartGameSetup();
            localGameController.ContinueGameSetup(28, 40);

            errorDetails.ShouldNotBeNull();
            errorDetails.Message.ShouldBe("Cannot build road segment. No direct connection between locations [28, 40].");
        }

        [Test]
        [Category("LocalGameController")]
        public void PlayerPlacesSettlementOffGameBoardDuringSecondSetupRound_MeaningfulErrorIsReceived()
        {
            var localGameController = LocalGameControllerTestCreator.CreateTestInstances()
                .LocalGameController;

            ErrorDetails errorDetails = null;
            localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

            localGameController.JoinGame();
            localGameController.LaunchGame();
            localGameController.StartGameSetup();
            localGameController.ContinueGameSetup(0, 1);

            localGameController.CompleteGameSetup(100, 101);

            errorDetails.ShouldNotBeNull();
            errorDetails.Message.ShouldBe("Cannot build settlement. Location 100 is outside of board range (0 - 53).");
        }

        [Test]
        [Category("LocalGameController")]
        public void PlayerPlacesRoadOverEdgeOfGameBoardDuringSecondSetupRound_MeaningfulErrorIsReceived()
        {
            var localGameController = LocalGameControllerTestCreator.CreateTestInstances()
                .LocalGameController;

            ErrorDetails errorDetails = null;
            localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

            localGameController.JoinGame();
            localGameController.LaunchGame();
            localGameController.StartGameSetup();
            localGameController.ContinueGameSetup(0, 1);

            localGameController.CompleteGameSetup(53, 54);

            errorDetails.ShouldNotBeNull();
            errorDetails.Message.ShouldBe("Cannot build road segment. Locations 53 and/or 54 are outside of board range (0 - 53).");
        }

        [Test]
        [Category("LocalGameController")]
        public void PlayerPlacesRoadWhereNoConnectionExistsDuringSecondSetupRound_MeaningfulErrorIsReceived()
        {
            var localGameController = this.CreateLocalGameControllerWithMainPlayerGoingFirstInSetup();

            ErrorDetails errorDetails = null;
            localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

            localGameController.JoinGame();
            localGameController.LaunchGame();
            localGameController.StartGameSetup();
            localGameController.ContinueGameSetup(0, 1);

            localGameController.CompleteGameSetup(28, 40);

            errorDetails.ShouldNotBeNull();
            errorDetails.Message.ShouldBe("Cannot build road segment. No direct connection between locations [28, 40].");
        }

        [Test]
        [Category("LocalGameController")]
        [Category("Main Player Turn")]
        public void StartOfMainPlayerTurn_TurnTokenReceived()
        {
            // Arrange
            MockDice mockDice = null;
            Guid id = Guid.Empty;
            MockPlayer player;
            MockComputerPlayer firstComputerPlayer, secondComputerPlayer, thirdComputerPlayer;
            var localGameController = this.CreateLocalGameControllerAndCompleteGameSetup(out mockDice, out player, out firstComputerPlayer, out secondComputerPlayer, out thirdComputerPlayer);

            mockDice.AddSequence(new[] { 8u });

            // Act
            GameToken turnToken = null;
            localGameController.StartPlayerTurnEvent = (GameToken t) => { turnToken = t; };
            localGameController.StartGamePlay();

            // Assert
            turnToken.ShouldNotBeNull();
        }

        [Test]
        [Category("LocalGameController")]
        [Category("Main Player Turn")]
        public void StartOfMainPlayerTurn_ChooseResourceFromOpponentCalledOutOfSequence_MeaningfulErrorIsReceived()
        {
            // Arrange
            var testInstances = LocalGameControllerTestCreator.CreateTestInstances();
            var localGameController = testInstances.LocalGameController;
            LocalGameControllerTestSetup.LaunchGameAndCompleteSetup(localGameController);
            testInstances.Dice.AddSequence(new[] { 7u, 0u });

            ErrorDetails errorDetails = null;
            localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

            var resourceUpdateReceived = false;
            localGameController.ResourcesTransferredEvent = (ResourceTransactionList r) => { resourceUpdateReceived = true; };

            // Act
            localGameController.ChooseResourceFromOpponent(testInstances.FirstOpponent.Id);

            // Assert
            resourceUpdateReceived.ShouldBeFalse();
            errorDetails.ShouldNotBeNull();
            errorDetails.Message.ShouldBe("Cannot call 'ChooseResourceFromOpponent' until 'SetRobberLocation' has completed.");
        }

        /// <summary>
        /// The robber hex set by the player has no adjacent settlements so calling the CallingChooseResourceFromOpponent 
        /// method raises an error
        /// </summary>
        [Test]
        [Category("LocalGameController")]
        [Category("Main Player Turn")]
        public void StartOfMainPlayerTurn_RobberLocationHasNoSettlementsAndCallingChooseResourceFromOpponent_MeaningfulErrorIsReceived()
        {
            // Arrange
            var testInstances = LocalGameControllerTestCreator.CreateTestInstances();
            var localGameController = testInstances.LocalGameController;
            testInstances.Dice.AddSequence(new[] { 7u, 0u });

            ErrorDetails errorDetails = null;
            localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

            LocalGameControllerTestSetup.LaunchGameAndCompleteSetup(localGameController);
            localGameController.StartGamePlay();
            localGameController.SetRobberHex(0u);

            // Act
            localGameController.ChooseResourceFromOpponent(Guid.NewGuid());

            // Assert
            errorDetails.ShouldNotBeNull();
            errorDetails.Message.ShouldBe("Cannot call 'ChooseResourceFromOpponent' when no robbing choices are available.");
        }

        [Test]
        [Category("LocalGameController")]
        public void Load_HexDataOnly_ResourceProvidersLoadedCorrectly()
        {
            // Arrange
            var localGameController = new LocalGameControllerCreator().Create();

            GameBoard boardData = null;
            localGameController.GameLoadedEvent = (PlayerDataBase[] pd, GameBoard bd) => { boardData = bd; };

            // Act
            var content = "<game><board><hexes>" +
              "<resources>glbglogob gwwwlwlbo</resources>" +
              "<production>9,8,5,12,11,3,6,10,6,0,4,11,2,4,3,5,9,10,8</production>" +
              "</hexes></board></game>";

            var contentBytes = Encoding.UTF8.GetBytes(content);
            using (var memoryStream = new MemoryStream(contentBytes))
            {
                localGameController.Load(memoryStream);
            }

            // Assert
            boardData.ShouldNotBeNull();
            HexInformation[] hexes = boardData.GetHexData();
            hexes.Length.ShouldBe(GameBoard.StandardBoardHexCount);
            hexes[0].ShouldBe(new HexInformation { ResourceType = ResourceTypes.Grain, ProductionFactor = 9 });
            hexes[1].ShouldBe(new HexInformation { ResourceType = ResourceTypes.Lumber, ProductionFactor = 8});
            hexes[2].ShouldBe(new HexInformation { ResourceType = ResourceTypes.Brick, ProductionFactor = 5});
            hexes[3].ShouldBe(new HexInformation { ResourceType = ResourceTypes.Grain, ProductionFactor = 12});
            hexes[4].ShouldBe(new HexInformation { ResourceType = ResourceTypes.Lumber, ProductionFactor = 11});
            hexes[5].ShouldBe(new HexInformation { ResourceType = ResourceTypes.Ore, ProductionFactor = 3});
            hexes[6].ShouldBe(new HexInformation { ResourceType = ResourceTypes.Grain, ProductionFactor = 6});
            hexes[7].ShouldBe(new HexInformation { ResourceType = ResourceTypes.Ore, ProductionFactor = 10});
            hexes[8].ShouldBe(new HexInformation { ResourceType = ResourceTypes.Brick, ProductionFactor = 6});
            hexes[9].ShouldBe(new HexInformation { ResourceType = null, ProductionFactor = 0});
            hexes[10].ShouldBe(new HexInformation { ResourceType = ResourceTypes.Grain, ProductionFactor = 4});
            hexes[11].ShouldBe(new HexInformation { ResourceType = ResourceTypes.Wool, ProductionFactor = 11});
            hexes[12].ShouldBe(new HexInformation { ResourceType = ResourceTypes.Wool, ProductionFactor = 2});
            hexes[13].ShouldBe(new HexInformation { ResourceType = ResourceTypes.Wool, ProductionFactor = 4});
            hexes[14].ShouldBe(new HexInformation { ResourceType = ResourceTypes.Lumber, ProductionFactor = 3});
            hexes[15].ShouldBe(new HexInformation { ResourceType = ResourceTypes.Wool, ProductionFactor = 5});
            hexes[16].ShouldBe(new HexInformation { ResourceType = ResourceTypes.Lumber, ProductionFactor = 9});
            hexes[17].ShouldBe(new HexInformation { ResourceType = ResourceTypes.Brick, ProductionFactor = 10});
            hexes[18].ShouldBe(new HexInformation { ResourceType = ResourceTypes.Ore, ProductionFactor = 8});
        }

        //TODO: Replace with test for latest Load method
        [Test]
        [Category("LocalGameController")]
        public void Load_PlayerDataOnly_PlayerDataViewsAreAsExpected()
        {
            // Arrange
            MockPlayer player;
            MockComputerPlayer firstOpponent, secondOpponent, thirdOpponent;
            this.CreateDefaultPlayerInstances(out player, out firstOpponent, out secondOpponent, out thirdOpponent);

            player.AddResources(new ResourceClutch(1, 2, 3, 4, 5));
            firstOpponent.AddResources(new ResourceClutch(1, 1, 1, 1, 1));
            secondOpponent.AddResources(new ResourceClutch(2, 2, 2, 2, 2));
            thirdOpponent.AddResources(new ResourceClutch(3, 3, 3, 3, 3));

            var localGameController = new LocalGameControllerCreator().Create();

            PlayerDataBase[] playerDataViews = null;
            localGameController.GameLoadedEvent = (PlayerDataBase[] pd, GameBoard bd) => { playerDataViews = pd; };

            // Act
            var streamContent = "<game>" +
              "<players>" +
              "<player id=\"" + player.Id + "\" name=\"" + player.Name + "\" brick=\"" + player.Resources.BrickCount + "\" grain=\"" + player.Resources.GrainCount + "\" lumber=\"" + player.Resources.LumberCount + "\" ore=\"" + player.Resources.OreCount + "\" wool=\"" + player.Resources.WoolCount + "\" />" +
              "<player id=\"" + firstOpponent.Id + "\" name=\"" + firstOpponent.Name + "\" iscomputer=\"true\" brick=\"" + firstOpponent.Resources.BrickCount + "\" grain=\"" + firstOpponent.Resources.GrainCount + "\" lumber=\"" + firstOpponent.Resources.LumberCount + "\" ore=\"" + firstOpponent.Resources.OreCount + "\" wool=\"" + firstOpponent.Resources.WoolCount + "\" />" +
              "<player id=\"" + secondOpponent.Id + "\" name=\"" + secondOpponent.Name + "\" iscomputer=\"true\" brick=\"" + secondOpponent.Resources.BrickCount + "\" grain=\"" + secondOpponent.Resources.GrainCount + "\" lumber=\"" + secondOpponent.Resources.LumberCount + "\" ore=\"" + secondOpponent.Resources.OreCount + "\" wool=\"" + secondOpponent.Resources.WoolCount + "\" />" +
              "<player id=\"" + thirdOpponent.Id + "\" name=\"" + thirdOpponent.Name + "\" iscomputer=\"true\" brick=\"" + thirdOpponent.Resources.BrickCount + "\" grain=\"" + thirdOpponent.Resources.GrainCount + "\" lumber=\"" + thirdOpponent.Resources.LumberCount + "\" ore=\"" + thirdOpponent.Resources.OreCount + "\" wool=\"" + thirdOpponent.Resources.WoolCount + "\" />" +
              "</players>" +
              "</game>";
            var streamContentBytes = Encoding.UTF8.GetBytes(streamContent);
            using (var stream = new MemoryStream(streamContentBytes))
            {
                localGameController.Load(stream);
            }

            // Assert
            playerDataViews.ShouldNotBeNull();
            playerDataViews.Length.ShouldBe(4);

            this.AssertPlayerDataViewIsCorrect(player, (PlayerDataModel)playerDataViews[0]);
            this.AssertPlayerDataViewIsCorrect(firstOpponent, (PlayerDataModel)playerDataViews[1]);
            this.AssertPlayerDataViewIsCorrect(secondOpponent, (PlayerDataModel)playerDataViews[2]);
            this.AssertPlayerDataViewIsCorrect(thirdOpponent, (PlayerDataModel)playerDataViews[3]);
        }

        //TODO: Replace with test for latest Load method
        [Test]
        [Category("LocalGameController")]
        public void Load_PlayerAndInfrastructureData_GameBoardIsAsExpected()
        {
            // Arrange
            Guid playerId = Guid.NewGuid(), firstOpponentId = Guid.NewGuid(), secondOpponentId = Guid.NewGuid(), thirdOpponentId = Guid.NewGuid();
            var localGameController = LocalGameControllerTestCreator.CreateTestInstances().LocalGameController;

            GameBoard boardData = null;
            localGameController.GameLoadedEvent = (PlayerDataBase[] pd, GameBoard bd) => { boardData = bd; };

            // Act
            var streamContent = "<game>" +
              "<players>" +
              "<player id=\"" + playerId + "\" name=\"" + PlayerName + "\" brick=\"5\" grain=\"\" />" +
              "<player id=\"" + firstOpponentId + "\" name=\"" + FirstOpponentName + "\" brick=\"6\" />" +
              "<player id=\"" + secondOpponentId + "\" name=\"" + SecondOpponentName + "\" brick=\"7\" />" +
              "<player id=\"" + thirdOpponentId + "\" name=\"" + ThirdOpponentName + "\" brick=\"8\" />" +
              "</players>" +
              "<settlements>" +
              "<settlement playerid=\"" + playerId + "\" location=\"" + MainSettlementOneLocation + "\" />" +
              "<settlement playerid=\"" + playerId + "\" location=\"" + MainSettlementTwoLocation + "\" />" +
              "<settlement playerid=\"" + firstOpponentId + "\" location=\"" + FirstSettlementOneLocation + "\" />" +
              "</settlements>" +
              "<roads>" +
              "<road playerid=\"" + playerId + "\" start=\"" + MainSettlementOneLocation + "\" end=\"" + MainRoadOneEnd + "\" />" +
              "</roads>" +
              "</game>";

            var streamContentBytes = Encoding.UTF8.GetBytes(streamContent);
            using (var stream = new MemoryStream(streamContentBytes))
            {
                localGameController.Load(stream);
            }

            // Assert
            boardData.ShouldNotBeNull();

            var settlements = boardData.GetSettlementData();
            settlements.Count.ShouldBe(3);
            settlements.ShouldContainKeyAndValue(MainSettlementOneLocation, playerId);
            settlements.ShouldContainKeyAndValue(MainSettlementTwoLocation, playerId);
            settlements.ShouldContainKeyAndValue(FirstSettlementOneLocation, firstOpponentId);

            var roads = boardData.GetRoadData();
            roads.Length.ShouldBe(1);
            roads[0].ShouldBe(new Tuple<UInt32, UInt32, Guid>(MainSettlementOneLocation, MainRoadOneEnd, playerId));
        }

        private void AssertPlayerDataViewIsCorrect(IPlayer player, PlayerDataModel playerDataView)
        {
            playerDataView.Id.ShouldBe(player.Id);
            playerDataView.Name.ShouldBe(player.Name);
            playerDataView.PlayedDevelopmentCards.ShouldBeNull();
            playerDataView.HiddenDevelopmentCards.ShouldBe(0);
            playerDataView.ResourceCards.ShouldBe(player.Resources.Count);
            playerDataView.IsComputer.ShouldBe(player.IsComputer);
        }

        private LocalGameController CreateLocalGameControllerWithMainPlayerGoingFirstInSetup()
        {
            var mockDice = new MockDiceCreator()
                .AddExplicitDiceRollSequence(new uint[] { 12, 10, 8, 6 })
                .Create();

            MockPlayer player;
            MockComputerPlayer firstOpponent, secondOpponent, thirdOpponent;
            this.CreateDefaultPlayerInstances(out player, out firstOpponent, out secondOpponent, out thirdOpponent);

            return this.CreateLocalGameController(mockDice, player, firstOpponent, secondOpponent, thirdOpponent);
        }

        private void RunOpponentDataPassBackTests(GameOptions gameOptions)
        {
            var player = new MockPlayer(PlayerName);
            var firstOpponent = new MockComputerPlayer(FirstOpponentName);
            var secondOpponent = new MockComputerPlayer(SecondOpponentName);
            var thirdOpponent = new MockComputerPlayer(ThirdOpponentName);

            var mockPlayerPool = LocalGameControllerTestCreator.CreateMockPlayerPool(player, firstOpponent, secondOpponent, thirdOpponent);
            var playerSetup = new LocalGameControllerTestCreator.PlayerSetup(player, firstOpponent, secondOpponent, thirdOpponent, mockPlayerPool);
            var localGameController = LocalGameControllerTestCreator.CreateTestInstances(null, playerSetup, null, null).LocalGameController;

            PlayerDataBase[] playerDataViews = null;
            localGameController.GameJoinedEvent = (PlayerDataBase[] p) => { playerDataViews = p; };
            localGameController.JoinGame(gameOptions);

            playerDataViews.ShouldNotBeNull();
            playerDataViews.Length.ShouldBe(4);

            this.AssertPlayerDataViewIsCorrect(player, (PlayerDataModel)playerDataViews[0]);
            this.AssertPlayerDataViewIsCorrect(firstOpponent, (PlayerDataModel)playerDataViews[1]);
            this.AssertPlayerDataViewIsCorrect(secondOpponent, (PlayerDataModel)playerDataViews[2]);
            this.AssertPlayerDataViewIsCorrect(thirdOpponent, (PlayerDataModel)playerDataViews[3]);
        }

        private void VerifyNewSettlements(List<Tuple<UInt32, Guid>> actualSettlements, params Tuple<UInt32, Guid>[] expectedSettlements)
        {
            actualSettlements.Count.ShouldBe(expectedSettlements.Length);

            foreach (var expectedSettlement in expectedSettlements)
            {
                var foundExpectedSettlement = false;
                for (var index = 0; index < actualSettlements.Count; index++)
                {
                    var actualSettlement = actualSettlements[index];

                    if (actualSettlement.Item1 == expectedSettlement.Item1 &&
                        actualSettlement.Item2 == expectedSettlement.Item2)
                    {
                        actualSettlements.RemoveAt(index);
                        index--;
                        foundExpectedSettlement = true;
                        break;
                    }
                }

                if (!foundExpectedSettlement)
                {
                    throw new Exception("Did not find expected settlement: " + expectedSettlement.ToString());
                }
            }
        }

        private void VerifyNewRoads(List<Tuple<UInt32, UInt32, Guid>> actualRoads, params Tuple<UInt32, UInt32, Guid>[] expectedRoads)
        {
            actualRoads.Count.ShouldBe(expectedRoads.Length);

            foreach (var expectedRoad in expectedRoads)
            {
                var foundExpectedRoad = false;
                for (var index = 0; index < actualRoads.Count; index++)
                {
                    var actualRoad = actualRoads[index];

                    if (actualRoad.Item1 == expectedRoad.Item1 &&
                        actualRoad.Item2 == expectedRoad.Item2 &&
                        actualRoad.Item3 == expectedRoad.Item3)
                    {
                        foundExpectedRoad = true;
                        actualRoads.RemoveAt(index);
                        index--;
                        break;
                    }
                }

                if (!foundExpectedRoad)
                {
                    throw new Exception("Did not find expected road: " + expectedRoad);
                }
            }
        }
        #endregion
    }
}
