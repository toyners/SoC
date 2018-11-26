
namespace Jabberwocky.SoC.Library.UnitTests.LocalGameController_Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using GameBoards;
    using Interfaces;
    using Jabberwocky.SoC.Library.PlayerData;
    using Jabberwocky.SoC.Library.UnitTests.Extensions;
    using Mock;
    using NSubstitute;
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

            GameBoard gameBoardData = null;
            localGameController.InitialBoardSetupEvent = (GameBoard g) => { gameBoardData = g; };

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

            MockPlayer player;
            MockComputerPlayer firstOpponent, secondOpponent, thirdOpponent;
            this.CreateDefaultPlayerInstances(out player, out firstOpponent, out secondOpponent, out thirdOpponent);

            var localGameController = this.CreateLocalGameController(mockDice, player, firstOpponent, secondOpponent, thirdOpponent);

            GameBoardUpdate gameBoardUpdate = null;
            localGameController.GameSetupUpdateEvent = (GameBoardUpdate u) => { gameBoardUpdate = u; };
            localGameController.InitialBoardSetupEvent = (GameBoard d) => { };

            localGameController.JoinGame();
            localGameController.LaunchGame();

            gameBoardUpdate = new GameBoardUpdate(); // Ensure that there is a state change for the gameBoardUpdate variable 
            localGameController.StartGameSetup();
            gameBoardUpdate.ShouldBeNull();

            gameBoardUpdate = null; // Ensure that there is a state change for the gameBoardUpdate variable

            localGameController.ContinueGameSetup(MainSettlementOneLocation, MainRoadOneEnd);
            gameBoardUpdate.ShouldNotBeNull();
            this.VerifyNewSettlements(gameBoardUpdate.NewSettlements,
              new Tuple<UInt32, Guid>(FirstSettlementOneLocation, firstOpponent.Id),
              new Tuple<UInt32, Guid>(FirstSettlementTwoLocation, firstOpponent.Id),
              new Tuple<UInt32, Guid>(SecondSettlementOneLocation, secondOpponent.Id),
              new Tuple<UInt32, Guid>(SecondSettlementTwoLocation, secondOpponent.Id),
              new Tuple<UInt32, Guid>(ThirdSettlementOneLocation, thirdOpponent.Id),
              new Tuple<UInt32, Guid>(ThirdSettlementTwoLocation, thirdOpponent.Id));

            this.VerifyNewRoads(gameBoardUpdate.NewRoads,
              new Tuple<UInt32, UInt32, Guid>(FirstSettlementOneLocation, FirstRoadOneEnd, firstOpponent.Id),
              new Tuple<UInt32, UInt32, Guid>(FirstSettlementTwoLocation, FirstRoadTwoEnd, firstOpponent.Id),
              new Tuple<UInt32, UInt32, Guid>(SecondSettlementOneLocation, SecondRoadOneEnd, secondOpponent.Id),
              new Tuple<UInt32, UInt32, Guid>(SecondSettlementTwoLocation, SecondRoadTwoEnd, secondOpponent.Id),
              new Tuple<UInt32, UInt32, Guid>(ThirdSettlementOneLocation, ThirdRoadOneEnd, thirdOpponent.Id),
              new Tuple<UInt32, UInt32, Guid>(ThirdSettlementTwoLocation, ThirdRoadTwoEnd, thirdOpponent.Id));

            gameBoardUpdate = new GameBoardUpdate(); // Ensure that there is a state change for the gameBoardUpdate variable 

            localGameController.CompleteGameSetup(MainSettlementTwoLocation, MainRoadTwoEnd);
            gameBoardUpdate.ShouldBeNull();
        }

        [Test]
        [Category("LocalGameController")]
        public void CompleteSetupWithPlayerInSecondSlot_ExpectedPlacementsAreReturned()
        {
            var gameSetupOrder = new[] { 10u, 12u, 8u, 6u };
            var mockDice = new MockDiceCreator()
              .AddExplicitDiceRollSequence(gameSetupOrder)
              .Create();

            MockPlayer player;
            MockComputerPlayer firstOpponent, secondOpponent, thirdOpponent;
            this.CreateDefaultPlayerInstances(out player, out firstOpponent, out secondOpponent, out thirdOpponent);

            var localGameController = this.CreateLocalGameController(mockDice, player, firstOpponent, secondOpponent, thirdOpponent);

            GameBoardUpdate gameBoardUpdate = null;
            localGameController.GameSetupUpdateEvent = (GameBoardUpdate u) => { gameBoardUpdate = u; };
            localGameController.InitialBoardSetupEvent = (GameBoard d) => { };

            localGameController.JoinGame();
            localGameController.LaunchGame();

            localGameController.StartGameSetup();
            gameBoardUpdate.ShouldNotBeNull();

            this.VerifyNewSettlements(gameBoardUpdate.NewSettlements,
              new Tuple<UInt32, Guid>(FirstSettlementOneLocation, firstOpponent.Id));

            this.VerifyNewRoads(gameBoardUpdate.NewRoads,
              new Tuple<UInt32, UInt32, Guid>(FirstSettlementOneLocation, FirstRoadOneEnd, firstOpponent.Id));

            gameBoardUpdate = null; // Ensure that there is a state change for the gameBoardUpdate variable 
            localGameController.ContinueGameSetup(MainSettlementOneLocation, MainRoadOneEnd);

            gameBoardUpdate.ShouldNotBeNull();

            this.VerifyNewSettlements(gameBoardUpdate.NewSettlements,
              new Tuple<UInt32, Guid>(SecondSettlementOneLocation, secondOpponent.Id),
              new Tuple<UInt32, Guid>(SecondSettlementTwoLocation, secondOpponent.Id),
              new Tuple<UInt32, Guid>(ThirdSettlementOneLocation, thirdOpponent.Id),
              new Tuple<UInt32, Guid>(ThirdSettlementTwoLocation, thirdOpponent.Id));

            this.VerifyNewRoads(gameBoardUpdate.NewRoads,
              new Tuple<UInt32, UInt32, Guid>(SecondSettlementOneLocation, SecondRoadOneEnd, secondOpponent.Id),
              new Tuple<UInt32, UInt32, Guid>(SecondSettlementTwoLocation, SecondRoadTwoEnd, secondOpponent.Id),
              new Tuple<UInt32, UInt32, Guid>(ThirdSettlementOneLocation, ThirdRoadOneEnd, thirdOpponent.Id),
              new Tuple<UInt32, UInt32, Guid>(ThirdSettlementTwoLocation, ThirdRoadTwoEnd, thirdOpponent.Id));

            gameBoardUpdate = null; // Ensure that there is a state change for the gameBoardUpdate variable 
            localGameController.CompleteGameSetup(MainSettlementTwoLocation, MainRoadTwoEnd);

            this.VerifyNewSettlements(gameBoardUpdate.NewSettlements,
              new Tuple<UInt32, Guid>(FirstSettlementTwoLocation, firstOpponent.Id));

            this.VerifyNewRoads(gameBoardUpdate.NewRoads,
              new Tuple<UInt32, UInt32, Guid>(FirstSettlementTwoLocation, FirstRoadTwoEnd, firstOpponent.Id));
        }

        [Test]
        [Category("LocalGameController")]
        public void CompleteSetupWithPlayerInThirdSlot_ExpectedPlacementsAreReturned()
        {
            var gameSetupOrders = new[] { 8u, 12u, 10u, 6u };
            var mockDice = new MockDiceCreator()
              .AddExplicitDiceRollSequence(gameSetupOrders)
              .Create();

            MockPlayer player;
            MockComputerPlayer firstOpponent, secondOpponent, thirdOpponent;
            this.CreateDefaultPlayerInstances(out player, out firstOpponent, out secondOpponent, out thirdOpponent);

            var localGameController = this.CreateLocalGameController(mockDice, player, firstOpponent, secondOpponent, thirdOpponent);

            GameBoardUpdate gameBoardUpdate = null;
            localGameController.GameSetupUpdateEvent = (GameBoardUpdate u) => { gameBoardUpdate = u; };
            localGameController.InitialBoardSetupEvent = (GameBoard d) => { };

            localGameController.JoinGame();
            localGameController.LaunchGame();

            localGameController.StartGameSetup();
            gameBoardUpdate.ShouldNotBeNull();

            this.VerifyNewSettlements(gameBoardUpdate.NewSettlements,
              new Tuple<UInt32, Guid>(FirstSettlementOneLocation, firstOpponent.Id),
              new Tuple<UInt32, Guid>(SecondSettlementOneLocation, secondOpponent.Id));

            this.VerifyNewRoads(gameBoardUpdate.NewRoads,
              new Tuple<UInt32, UInt32, Guid>(FirstSettlementOneLocation, FirstRoadOneEnd, firstOpponent.Id),
              new Tuple<UInt32, UInt32, Guid>(SecondSettlementOneLocation, SecondRoadOneEnd, secondOpponent.Id));

            gameBoardUpdate = null; // Ensure that there is a state change for the gameBoardUpdate variable 
            localGameController.ContinueGameSetup(MainSettlementOneLocation, MainRoadOneEnd);

            gameBoardUpdate.ShouldNotBeNull();

            this.VerifyNewSettlements(gameBoardUpdate.NewSettlements,
              new Tuple<UInt32, Guid>(ThirdSettlementOneLocation, thirdOpponent.Id),
              new Tuple<UInt32, Guid>(ThirdSettlementTwoLocation, thirdOpponent.Id));

            this.VerifyNewRoads(gameBoardUpdate.NewRoads,
              new Tuple<UInt32, UInt32, Guid>(ThirdSettlementOneLocation, ThirdRoadOneEnd, thirdOpponent.Id),
              new Tuple<UInt32, UInt32, Guid>(ThirdSettlementTwoLocation, ThirdRoadTwoEnd, thirdOpponent.Id));

            gameBoardUpdate = null; // Ensure that there is a state change for the gameBoardUpdate variable 
            localGameController.CompleteGameSetup(MainSettlementTwoLocation, MainRoadTwoEnd);

            this.VerifyNewSettlements(gameBoardUpdate.NewSettlements,
              new Tuple<UInt32, Guid>(SecondSettlementTwoLocation, secondOpponent.Id),
              new Tuple<UInt32, Guid>(FirstSettlementTwoLocation, firstOpponent.Id));

            this.VerifyNewRoads(gameBoardUpdate.NewRoads,
              new Tuple<UInt32, UInt32, Guid>(SecondSettlementTwoLocation, SecondRoadTwoEnd, secondOpponent.Id),
              new Tuple<UInt32, UInt32, Guid>(FirstSettlementTwoLocation, FirstRoadTwoEnd, firstOpponent.Id));
        }

        [Test]
        [Category("LocalGameController")]
        public void CompleteSetupWithPlayerInFourthSlot_ExpectedPlacementsAreReturned()
        {
            var gameSetupOrder = new[] { 6u, 12u, 10u, 8u };
            var mockDice = new MockDiceCreator()
              .AddExplicitDiceRollSequence(gameSetupOrder)
              .Create();

            MockPlayer player;
            MockComputerPlayer firstOpponent, secondOpponent, thirdOpponent;
            this.CreateDefaultPlayerInstances(out player, out firstOpponent, out secondOpponent, out thirdOpponent);

            var localGameController = this.CreateLocalGameController(mockDice, player, firstOpponent, secondOpponent, thirdOpponent);

            GameBoardUpdate gameBoardUpdate = null;
            localGameController.GameSetupUpdateEvent = (GameBoardUpdate u) => { gameBoardUpdate = u; };
            localGameController.InitialBoardSetupEvent = (GameBoard d) => { };

            localGameController.JoinGame();
            localGameController.LaunchGame();

            localGameController.StartGameSetup();
            gameBoardUpdate.ShouldNotBeNull();

            this.VerifyNewSettlements(gameBoardUpdate.NewSettlements,
              new Tuple<UInt32, Guid>(FirstSettlementOneLocation, firstOpponent.Id),
              new Tuple<UInt32, Guid>(SecondSettlementOneLocation, secondOpponent.Id),
              new Tuple<UInt32, Guid>(ThirdSettlementOneLocation, thirdOpponent.Id));

            this.VerifyNewRoads(gameBoardUpdate.NewRoads,
              new Tuple<UInt32, UInt32, Guid>(FirstSettlementOneLocation, FirstRoadOneEnd, firstOpponent.Id),
              new Tuple<UInt32, UInt32, Guid>(SecondSettlementOneLocation, SecondRoadOneEnd, secondOpponent.Id),
              new Tuple<UInt32, UInt32, Guid>(ThirdSettlementOneLocation, ThirdRoadOneEnd, thirdOpponent.Id));

            gameBoardUpdate = new GameBoardUpdate(); // Ensure that there is a state change for the gameBoardUpdate variable 
            localGameController.ContinueGameSetup(MainSettlementOneLocation, MainRoadOneEnd);
            gameBoardUpdate.ShouldBeNull();

            gameBoardUpdate = null; // Ensure that there is a state change for the gameBoardUpdate variable 
            localGameController.CompleteGameSetup(MainSettlementTwoLocation, MainRoadTwoEnd);

            gameBoardUpdate.ShouldNotBeNull();
            this.VerifyNewSettlements(gameBoardUpdate.NewSettlements,
              new Tuple<UInt32, Guid>(ThirdSettlementTwoLocation, thirdOpponent.Id),
              new Tuple<UInt32, Guid>(SecondSettlementTwoLocation, secondOpponent.Id),
              new Tuple<UInt32, Guid>(FirstSettlementTwoLocation, firstOpponent.Id));

            this.VerifyNewRoads(gameBoardUpdate.NewRoads,
              new Tuple<UInt32, UInt32, Guid>(ThirdSettlementTwoLocation, ThirdRoadTwoEnd, thirdOpponent.Id),
              new Tuple<UInt32, UInt32, Guid>(SecondSettlementTwoLocation, SecondRoadTwoEnd, secondOpponent.Id),
              new Tuple<UInt32, UInt32, Guid>(FirstSettlementTwoLocation, FirstRoadTwoEnd, firstOpponent.Id));
        }

        [Test]
        public void CompleteSetupForPlayer_ExpectedResourcesAreReturned()
        {
            var mockDice = new MockDiceCreator()
              .AddRandomSequenceWithNoDuplicates(4)
              .Create();

            MockPlayer player;
            MockComputerPlayer firstOpponent, secondOpponent, thirdOpponent;
            this.CreateDefaultPlayerInstances(out player, out firstOpponent, out secondOpponent, out thirdOpponent);

            var localGameController = this.CreateLocalGameController(mockDice, player, firstOpponent, secondOpponent, thirdOpponent);

            ResourceUpdate resourceUpdate = null;
            localGameController.GameSetupResourcesEvent = (ResourceUpdate r) => { resourceUpdate = r; };

            localGameController.JoinGame();
            localGameController.LaunchGame();
            localGameController.StartGameSetup();
            localGameController.ContinueGameSetup(MainSettlementOneLocation, MainRoadOneEnd);
            localGameController.CompleteGameSetup(MainSettlementTwoLocation, MainRoadTwoEnd);

            resourceUpdate.ShouldNotBeNull();
            resourceUpdate.Resources.Count.ShouldBe(4);
            resourceUpdate.Resources.ShouldContainKey(player.Id);
            resourceUpdate.Resources.ShouldContainKey(firstOpponent.Id);
            resourceUpdate.Resources.ShouldContainKey(secondOpponent.Id);
            resourceUpdate.Resources.ShouldContainKey(thirdOpponent.Id);

            resourceUpdate.Resources[player.Id].BrickCount.ShouldBe(1);
            resourceUpdate.Resources[player.Id].GrainCount.ShouldBe(1);
            resourceUpdate.Resources[player.Id].LumberCount.ShouldBe(0);
            resourceUpdate.Resources[player.Id].OreCount.ShouldBe(0);
            resourceUpdate.Resources[player.Id].WoolCount.ShouldBe(1);

            resourceUpdate.Resources[firstOpponent.Id].BrickCount.ShouldBe(0);
            resourceUpdate.Resources[firstOpponent.Id].GrainCount.ShouldBe(1);
            resourceUpdate.Resources[firstOpponent.Id].LumberCount.ShouldBe(1);
            resourceUpdate.Resources[firstOpponent.Id].OreCount.ShouldBe(0);
            resourceUpdate.Resources[firstOpponent.Id].WoolCount.ShouldBe(1);

            resourceUpdate.Resources[secondOpponent.Id].BrickCount.ShouldBe(0);
            resourceUpdate.Resources[secondOpponent.Id].GrainCount.ShouldBe(0);
            resourceUpdate.Resources[secondOpponent.Id].LumberCount.ShouldBe(1);
            resourceUpdate.Resources[secondOpponent.Id].OreCount.ShouldBe(1);
            resourceUpdate.Resources[secondOpponent.Id].WoolCount.ShouldBe(1);

            resourceUpdate.Resources[thirdOpponent.Id].BrickCount.ShouldBe(0);
            resourceUpdate.Resources[thirdOpponent.Id].GrainCount.ShouldBe(1);
            resourceUpdate.Resources[thirdOpponent.Id].LumberCount.ShouldBe(1);
            resourceUpdate.Resources[thirdOpponent.Id].OreCount.ShouldBe(0);
            resourceUpdate.Resources[thirdOpponent.Id].WoolCount.ShouldBe(1);
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

            firstOpponent.GrainCount.ShouldBe(1);
            firstOpponent.LumberCount.ShouldBe(1);
            firstOpponent.WoolCount.ShouldBe(1);

            secondOpponent.LumberCount.ShouldBe(1);
            secondOpponent.OreCount.ShouldBe(1);
            secondOpponent.WoolCount.ShouldBe(1);

            thirdOpponent.GrainCount.ShouldBe(1);
            thirdOpponent.LumberCount.ShouldBe(1);
            thirdOpponent.WoolCount.ShouldBe(1);
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

            MockPlayer player;
            MockComputerPlayer firstOpponent, secondOpponent, thirdOpponent;
            this.CreateDefaultPlayerInstances(out player, out firstOpponent, out secondOpponent, out thirdOpponent);

            var localGameController = this.CreateLocalGameController(mockDice, player, firstOpponent, secondOpponent, thirdOpponent);

            ErrorDetails exception = null;
            localGameController.ErrorRaisedEvent = (ErrorDetails e) => { exception = e; };

            localGameController.JoinGame();
            localGameController.LaunchGame();
            localGameController.StartGameSetup();

            GameBoardUpdate gameBoardUpdate = null;
            localGameController.GameSetupUpdateEvent = (GameBoardUpdate u) => { gameBoardUpdate = u; };

            localGameController.ContinueGameSetup(FirstSettlementOneLocation, 1u);
            exception.ShouldNotBeNull();
            exception.Message.ShouldBe("Cannot build settlement. Location " + FirstSettlementOneLocation + " already settled by player '" + FirstOpponentName + "'.");
            gameBoardUpdate.ShouldBeNull();
        }

        [Test]
        [Category("LocalGameController")]
        public void PlayerSelectsSameLocationAsComputerPlayerDuringSecondSetupRound_MeaningfulErrorIsReceived()
        {
            var mockDice = new MockDiceCreator()
                .AddExplicitDiceRollSequence(new uint[] { 12, 10, 8, 6 })
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
            localGameController.ContinueGameSetup(0, 1);

            GameBoardUpdate gameBoardUpdate = null;
            localGameController.GameSetupUpdateEvent = (GameBoardUpdate u) => { gameBoardUpdate = u; };

            localGameController.CompleteGameSetup(FirstSettlementOneLocation, 1);

            exception.ShouldNotBeNull();
            exception.Message.ShouldBe("Cannot build settlement. Location " + FirstSettlementOneLocation + " already settled by player '" + FirstOpponentName + "'.");
            gameBoardUpdate.ShouldBeNull();
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

            GameBoardUpdate gameBoardUpdate = null;
            localGameController.GameSetupUpdateEvent = (GameBoardUpdate u) => { gameBoardUpdate = u; };

            localGameController.ContinueGameSetup(19u, 1u);

            exception.ShouldNotBeNull();
            exception.Message.ShouldBe("Cannot build settlement. Too close to player '" + FirstOpponentName + "' at location " + FirstSettlementOneLocation + ".");
            gameBoardUpdate.ShouldBeNull();
        }

        [Test]
        [Category("LocalGameController")]
        public void PlayerSelectsLocationTooCloseToComputerPlayerDuringSecondSetupRound_MeaningfulErrorIsReceived()
        {
            var mockDice = new MockDiceCreator()
                .AddExplicitDiceRollSequence(new uint[] { 12, 10, 8, 6 })
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
            localGameController.ContinueGameSetup(0, 1);

            GameBoardUpdate gameBoardUpdate = null;
            localGameController.GameSetupUpdateEvent = (GameBoardUpdate u) => { gameBoardUpdate = u; };

            localGameController.CompleteGameSetup(19, 18);

            exception.ShouldNotBeNull();
            exception.Message.ShouldBe("Cannot build settlement. Too close to player '" + FirstOpponentName + "' at location " + FirstSettlementOneLocation + ".");
            gameBoardUpdate.ShouldBeNull();
        }

        [Test]
        [Category("LocalGameController")]
        public void PlayerPlacesSettlementOffGameBoardDuringFirstSetupRound_MeaningfulErrorReceived()
        {
            var localGameController = CreateLocalGameControllerWithMainPlayerGoingFirstInSetup();

            ErrorDetails errorDetails = null;
            localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

            GameBoardUpdate gameBoardUpdate = null;
            localGameController.GameSetupUpdateEvent = (GameBoardUpdate u) => { gameBoardUpdate = u; };

            localGameController.JoinGame();
            localGameController.LaunchGame();
            localGameController.StartGameSetup();
            localGameController.ContinueGameSetup(100, 101);

            errorDetails.ShouldNotBeNull();
            errorDetails.Message.ShouldBe("Cannot build settlement. Location 100 is outside of board range (0 - 53).");
            gameBoardUpdate.ShouldBeNull();
        }

        [Test]
        [Category("LocalGameController")]
        public void PlayerPlacesRoadOverEdgeOfGameBoardDuringFirstSetupRound_MeaningfulErrorIsReceived()
        {
            var localGameController = CreateLocalGameControllerWithMainPlayerGoingFirstInSetup();

            ErrorDetails errorDetails = null;
            localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

            GameBoardUpdate gameBoardUpdate = null;
            localGameController.GameSetupUpdateEvent = (GameBoardUpdate u) => { gameBoardUpdate = u; };

            localGameController.JoinGame();
            localGameController.LaunchGame();
            localGameController.StartGameSetup();
            localGameController.ContinueGameSetup(53, 54);

            errorDetails.ShouldNotBeNull();
            errorDetails.Message.ShouldBe("Cannot build road segment. Locations 53 and/or 54 are outside of board range (0 - 53).");
            gameBoardUpdate.ShouldBeNull();
        }

        [Test]
        [Category("LocalGameController")]
        public void PlayerPlacesRoadWhereNoConnectionExistsDuringFirstSetupRound_MeaningfulErrorIsReceived()
        {
            var localGameController = CreateLocalGameControllerWithMainPlayerGoingFirstInSetup();

            ErrorDetails errorDetails = null;
            localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

            GameBoardUpdate gameBoardUpdate = null;
            localGameController.GameSetupUpdateEvent = (GameBoardUpdate u) => { gameBoardUpdate = u; };

            localGameController.JoinGame();
            localGameController.LaunchGame();
            localGameController.StartGameSetup();
            localGameController.ContinueGameSetup(28, 40);

            errorDetails.ShouldNotBeNull();
            errorDetails.Message.ShouldBe("Cannot build road segment. No direct connection between locations [28, 40].");
            gameBoardUpdate.ShouldBeNull();
        }

        [Test]
        [Category("LocalGameController")]
        public void PlayerPlacesSettlementOffGameBoardDuringSecondSetupRound_MeaningfulErrorIsReceived()
        {
            var localGameController = CreateLocalGameControllerWithMainPlayerGoingFirstInSetup();

            ErrorDetails errorDetails = null;
            localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

            localGameController.JoinGame();
            localGameController.LaunchGame();
            localGameController.StartGameSetup();
            localGameController.ContinueGameSetup(0, 1);

            GameBoardUpdate gameBoardUpdate = null;
            localGameController.GameSetupUpdateEvent = (GameBoardUpdate u) => { gameBoardUpdate = u; };

            localGameController.CompleteGameSetup(100, 101);

            errorDetails.ShouldNotBeNull();
            errorDetails.Message.ShouldBe("Cannot build settlement. Location 100 is outside of board range (0 - 53).");
            gameBoardUpdate.ShouldBeNull();
        }

        [Test]
        [Category("LocalGameController")]
        public void PlayerPlacesRoadOverEdgeOfGameBoardDuringSecondSetupRound_MeaningfulErrorIsReceived()
        {
            var localGameController = CreateLocalGameControllerWithMainPlayerGoingFirstInSetup();

            ErrorDetails errorDetails = null;
            localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

            localGameController.JoinGame();
            localGameController.LaunchGame();
            localGameController.StartGameSetup();
            localGameController.ContinueGameSetup(0, 1);

            GameBoardUpdate gameBoardUpdate = null;
            localGameController.GameSetupUpdateEvent = (GameBoardUpdate u) => { gameBoardUpdate = u; };

            localGameController.CompleteGameSetup(53, 54);

            errorDetails.ShouldNotBeNull();
            errorDetails.Message.ShouldBe("Cannot build road segment. Locations 53 and/or 54 are outside of board range (0 - 53).");
            gameBoardUpdate.ShouldBeNull();
        }

        [Test]
        [Category("LocalGameController")]
        public void PlayerPlacesRoadWhereNoConnectionExistsDuringSecondSetupRound_MeaningfulErrorIsReceived()
        {
            var localGameController = CreateLocalGameControllerWithMainPlayerGoingFirstInSetup();

            ErrorDetails errorDetails = null;
            localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

            localGameController.JoinGame();
            localGameController.LaunchGame();
            localGameController.StartGameSetup();
            localGameController.ContinueGameSetup(0, 1);

            GameBoardUpdate gameBoardUpdate = null;
            localGameController.GameSetupUpdateEvent = (GameBoardUpdate u) => { gameBoardUpdate = u; };

            localGameController.CompleteGameSetup(28, 40);

            errorDetails.ShouldNotBeNull();
            errorDetails.Message.ShouldBe("Cannot build road segment. No direct connection between locations [28, 40].");
            gameBoardUpdate.ShouldBeNull();
        }

        [Test]
        [Category("LocalGameController")]
        public void FinalisePlayerTurnOrder_CallOutOfSequence_MeaningfulErrorIsReceived()
        {
            var localGameController = new LocalGameControllerCreator().Create();
            ErrorDetails errorDetails = null;
            localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

            localGameController.FinalisePlayerTurnOrder();

            errorDetails.ShouldNotBeNull();
            errorDetails.Message.ShouldBe("Cannot call 'FinalisePlayerTurnOrder' until 'CompleteGameSetup' has completed.");
        }

        [Test]
        [Category("LocalGameController")]
        public void FinalisePlayerTurnOrder_RollsAscending_ReceiveTurnOrderForMainGameLoop()
        {
            // Arrange
            var gameSetupOrder = new[] { 12u, 10u, 8u, 6u };
            var gameTurnOrder = new[] { 6u, 8u, 10u, 12u };
            var mockDice = new MockDiceCreator()
              .AddExplicitDiceRollSequence(gameSetupOrder)
              .AddExplicitDiceRollSequence(gameTurnOrder)
              .Create();

            MockPlayer player;
            MockComputerPlayer firstOpponent, secondOpponent, thirdOpponent;
            this.CreateDefaultPlayerInstances(out player, out firstOpponent, out secondOpponent, out thirdOpponent);

            var localGameController = this.CreateLocalGameController(mockDice, player, firstOpponent, secondOpponent, thirdOpponent);

            localGameController.JoinGame();
            localGameController.LaunchGame();
            localGameController.StartGameSetup();
            localGameController.ContinueGameSetup(MainSettlementOneLocation, MainRoadOneEnd);
            localGameController.CompleteGameSetup(MainSettlementTwoLocation, MainRoadTwoEnd);

            // Act
            PlayerDataModel[] turnOrder = null;
            localGameController.TurnOrderFinalisedEvent = (PlayerDataModel[] p) => { turnOrder = p; };
            localGameController.FinalisePlayerTurnOrder();

            // Assert
            turnOrder.ShouldNotBeNull();
            turnOrder.Length.ShouldBe(4);

            this.AssertPlayerDataViewIsCorrect(player, turnOrder[3]);
            this.AssertPlayerDataViewIsCorrect(firstOpponent, turnOrder[2]);
            this.AssertPlayerDataViewIsCorrect(secondOpponent, turnOrder[1]);
            this.AssertPlayerDataViewIsCorrect(thirdOpponent, turnOrder[0]);
        }

        [Test]
        [Category("LocalGameController")]
        public void FinalisePlayerTurnOrder_RollsDescending_ReceiveTurnOrderForMainGameLoop()
        {
            // Arrange
            var gameSetupOrder = new[] { 12u, 10u, 8u, 6u };
            var gameTurnOrder = gameSetupOrder;
            var mockDice = new MockDiceCreator()
              .AddExplicitDiceRollSequence(gameSetupOrder)
              .AddExplicitDiceRollSequence(gameTurnOrder)
              .Create();

            MockPlayer player;
            MockComputerPlayer firstOpponent, secondOpponent, thirdOpponent;
            this.CreateDefaultPlayerInstances(out player, out firstOpponent, out secondOpponent, out thirdOpponent);

            var localGameController = this.CreateLocalGameController(mockDice, player, firstOpponent, secondOpponent, thirdOpponent);

            localGameController.JoinGame();
            localGameController.LaunchGame();
            localGameController.StartGameSetup();
            localGameController.ContinueGameSetup(MainSettlementOneLocation, MainRoadOneEnd);
            localGameController.CompleteGameSetup(MainSettlementTwoLocation, MainRoadTwoEnd);

            // Act
            PlayerDataModel[] turnOrder = null;
            localGameController.TurnOrderFinalisedEvent = (PlayerDataModel[] p) => { turnOrder = p; };
            localGameController.FinalisePlayerTurnOrder();

            // Assert
            turnOrder.ShouldNotBeNull();
            turnOrder.Length.ShouldBe(4);

            this.AssertPlayerDataViewIsCorrect(player, turnOrder[0]);
            this.AssertPlayerDataViewIsCorrect(firstOpponent, turnOrder[1]);
            this.AssertPlayerDataViewIsCorrect(secondOpponent, turnOrder[2]);
            this.AssertPlayerDataViewIsCorrect(thirdOpponent, turnOrder[3]);
        }

        [Test]
        [Category("LocalGameController")]
        public void StartGamePlayer_PlayerTurnNotFinalised_MeaningfulErrorIsReceived()
        {
            // Arrange
            var gameSetupOrder = new[] { 12u, 10u, 8u, 6u };
            var gameTurnOrder = gameSetupOrder;
            var mockDice = new MockDiceCreator()
              .AddExplicitDiceRollSequence(gameSetupOrder)
              .AddExplicitDiceRollSequence(gameTurnOrder)
              .Create();

            MockPlayer player;
            MockComputerPlayer firstOpponent, secondOpponent, thirdOpponent;
            this.CreateDefaultPlayerInstances(out player, out firstOpponent, out secondOpponent, out thirdOpponent);

            var localGameController = this.CreateLocalGameController(mockDice, player, firstOpponent, secondOpponent, thirdOpponent);

            ErrorDetails errorDetails = null;
            localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

            localGameController.JoinGame();
            localGameController.LaunchGame();
            localGameController.StartGameSetup();
            localGameController.ContinueGameSetup(MainSettlementOneLocation, MainRoadOneEnd);
            localGameController.CompleteGameSetup(MainSettlementTwoLocation, MainRoadTwoEnd);

            // Act
            localGameController.StartGamePlay();

            // Assert
            errorDetails.ShouldNotBeNull();
            errorDetails.Message.ShouldBe("Cannot call 'StartGamePlay' until 'FinalisePlayerTurnOrder' has completed.");
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
            TurnToken turnToken = null;
            localGameController.StartPlayerTurnEvent = (TurnToken t) => { turnToken = t; };
            localGameController.StartGamePlay();

            // Assert
            turnToken.ShouldNotBeNull();
        }

        [Test]
        [Category("LocalGameController")]
        [Category("Main Player Turn")]
        public void StartOfMainPlayerTurn_DiceRollReceived()
        {
            // Arrange
            MockDice mockDice = null;
            Guid id = Guid.Empty;
            MockPlayer player;
            MockComputerPlayer firstComputerPlayer, secondComputerPlayer, thirdComputerPlayer;
            var localGameController = this.CreateLocalGameControllerAndCompleteGameSetup(out mockDice, out player, out firstComputerPlayer, out secondComputerPlayer, out thirdComputerPlayer);
            mockDice.AddSequence(new[] { 8u });

            // Act
            var diceRoll = 0u;
            localGameController.DiceRollEvent = (uint d1, uint d2) => { diceRoll = d1 + d2; };
            localGameController.StartGamePlay();

            // Assert
            diceRoll.ShouldBe(8u);
        }

        [Test]
        [Category("LocalGameController")]
        [Category("Main Player Turn")]
        public void StartOfMainPlayerTurn_DoesNotRollSeven_ReceiveCollectedResourcesDetails()
        {
            var testInstances = LocalGameControllerTestCreator.CreateTestInstances();
            var localGameController = testInstances.LocalGameController;
            LocalGameControllerTestSetup.LaunchGameAndCompleteSetup(localGameController);
            testInstances.Dice.AddSequence(new[] { 8u });
            var playerId = testInstances.MainPlayer.Id;
            var firstOpponentId = testInstances.FirstOpponent.Id;

            Dictionary<Guid, ResourceCollection[]> actual = null;
            localGameController.ResourcesCollectedEvent = (Dictionary<Guid, ResourceCollection[]> r) => { actual = r; };
            localGameController.StartGamePlay();

            var expected = new Dictionary<Guid, ResourceCollection[]>
      {
        { playerId, new [] { new ResourceCollection(12u, ResourceClutch.OneBrick) } },
        { firstOpponentId, new[] { new ResourceCollection(43, ResourceClutch.OneGrain) } }
      };

            actual.ShouldContainExact(expected);
        }

        [Test]
        [Category("LocalGameController")]
        [Category("Main Player Turn")]
        public void StartOfMainPlayerTurn_RollsSevenReceiveResourceCardLossesForComputerPlayers()
        {
            // Arrange
            var testInstances = LocalGameControllerTestCreator.CreateTestInstances(new MockGameBoardWithNoResourcesCollected());
            var localGameController = testInstances.LocalGameController;
            var player = testInstances.MainPlayer;
            var firstOpponent = testInstances.FirstOpponent;
            var secondOpponent = testInstances.SecondOpponent;
            var thirdOpponent = testInstances.ThirdOpponent;
            LocalGameControllerTestSetup.LaunchGameAndCompleteSetup(localGameController);
            testInstances.Dice.AddSequence(new[] { 7u });

            var droppedResourcesForFirstOpponent = new ResourceClutch(1, 1, 1, 1, 0);
            var expectedDroppedResourcesForFirstOpponent = new ResourceClutch(1, 1, 1, 1, 0);
            var droppedResourcesForThirdOpponent = new ResourceClutch(1, 1, 1, 1, 1);
            var expectedDroppedResourcesForThirdOpponent = new ResourceClutch(1, 1, 1, 1, 1);

            firstOpponent.AddResources(new ResourceClutch(2, 2, 2, 1, 1));
            firstOpponent.DroppedResources = droppedResourcesForFirstOpponent;
            secondOpponent.AddResources(new ResourceClutch(2, 2, 1, 1, 1));
            thirdOpponent.AddResources(new ResourceClutch(2, 2, 2, 2, 1));
            thirdOpponent.DroppedResources = droppedResourcesForThirdOpponent;

            // Act
            ResourceUpdate resourcesLost = null;
            localGameController.ResourcesLostEvent = (ResourceUpdate r) => { resourcesLost = r; };
            localGameController.StartGamePlay();

            // Assert
            resourcesLost.Resources.Count.ShouldBe(2);
            resourcesLost.Resources.ShouldContainKeyAndValue(firstOpponent.Id, expectedDroppedResourcesForFirstOpponent);
            resourcesLost.Resources.ShouldContainKeyAndValue(thirdOpponent.Id, expectedDroppedResourcesForThirdOpponent);
        }

        [Test]
        [TestCase(6, 0)]
        [TestCase(7, 0)]
        [TestCase(8, 4)]
        [TestCase(9, 4)]
        [TestCase(10, 5)]
        [Category("LocalGameController")]
        [Category("Main Player Turn")]
        public void StartOfMainPlayerTurn_RollsSevenReceivesRobberEventNotificationWithDropResourceCardsCount(Int32 brickCount, Int32 expectedResourceDropCount)
        {
            // Arrange
            var testInstances = LocalGameControllerTestCreator.CreateTestInstances(new MockGameBoardWithNoResourcesCollected());
            var localGameController = testInstances.LocalGameController;
            var player = testInstances.MainPlayer;
            LocalGameControllerTestSetup.LaunchGameAndCompleteSetup(localGameController);
            testInstances.Dice.AddSequence(new[] { 7u });

            player.AddResources(new ResourceClutch(brickCount, 0, 0, 0, 0));

            // Act
            Int32 resourceDropCount = -1;
            localGameController.RobberEvent = (Int32 r) => { resourceDropCount = r; };
            localGameController.StartGamePlay();

            // Assert
            resourceDropCount.ShouldBe(expectedResourceDropCount);
        }

        [Test]
        [Category("LocalGameController")]
        [Category("Main Player Turn")]
        public void StartOfMainPlayerTurn_RollsSevenButDoesNotPassBackExpectedResources_MeaningfulErrorIsReceived()
        {
            // Arrange
            var testInstances = LocalGameControllerTestCreator.CreateTestInstances(new MockGameBoardWithNoResourcesCollected());
            var localGameController = testInstances.LocalGameController;
            var player = testInstances.MainPlayer;
            LocalGameControllerTestSetup.LaunchGameAndCompleteSetup(localGameController);
            testInstances.Dice.AddSequence(new[] { 7u });

            player.AddResources(new ResourceClutch(8, 0, 0, 0, 0));

            // Act
            ErrorDetails errorDetails = null;
            localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };
            localGameController.StartGamePlay();
            localGameController.SetRobberHex(0u);

            // Assert
            errorDetails.ShouldNotBeNull();
            errorDetails.Message.ShouldBe("Cannot set robber location until expected resources (4) have been dropped via call to DropResources method.");
        }

        [Test]
        [Category("LocalGameController")]
        [Category("Main Player Turn")]
        public void StartOfMainPlayerTurn_RollsSevenAndPassBackExpectedResources_PlayerResourcesUpdatedCorrectly()
        {
            // Arrange
            var testInstances = LocalGameControllerTestCreator.CreateTestInstances(new MockGameBoardWithNoResourcesCollected());
            var localGameController = testInstances.LocalGameController;
            var player = testInstances.MainPlayer;
            LocalGameControllerTestSetup.LaunchGameAndCompleteSetup(localGameController);
            testInstances.Dice.AddSequence(new[] { 7u });

            player.AddResources(new ResourceClutch(8, 0, 0, 0, 0));

            // Act
            localGameController.StartGamePlay();
            localGameController.DropResources(new ResourceClutch(4, 0, 0, 0, 0));

            // Assert
            player.BrickCount.ShouldBe(4);
            player.GrainCount.ShouldBe(0);
            player.LumberCount.ShouldBe(0);
            player.OreCount.ShouldBe(0);
            player.WoolCount.ShouldBe(0);
        }

        [Test]
        [Category("LocalGameController")]
        [Category("Main Player Turn")]
        public void StartOfMainPlayerTurn_SetRobberOnHexWithOneOpponent_ReturnListOfOpponentsAndResourceCardsToChooseFrom()
        {
            // Arrange
            var testInstances = LocalGameControllerTestCreator.CreateTestInstances(new MockGameBoardWithNoResourcesCollected());
            var localGameController = testInstances.LocalGameController;
            var firstOpponent = testInstances.FirstOpponent;
            LocalGameControllerTestSetup.LaunchGameAndCompleteSetup(localGameController);
            testInstances.Dice.AddSequence(new[] { 7u });

            firstOpponent.AddResources(new ResourceClutch(1, 1, 1, 1, 1));

            // Act
            Dictionary<Guid, Int32> robberingChoices = null;
            localGameController.RobbingChoicesEvent = (Dictionary<Guid, Int32> rc) => { robberingChoices = rc; };
            localGameController.StartGamePlay();
            localGameController.SetRobberHex(FirstSettlementOneLocation);

            // Assert
            robberingChoices.ShouldNotBeNull();
            robberingChoices.Count.ShouldBe(1);
            robberingChoices.ShouldContainKeyAndValue(firstOpponent.Id, 5);
        }

        [Test]
        [Category("LocalGameController")]
        [Category("Main Player Turn")]
        public void StartOfMainPlayerTurn_RollsSevenAndChoosesBlindResourceFromOpponent_PlayerAndOpponentResourcesUpdated()
        {
            // Arrange
            var testInstances = LocalGameControllerTestCreator.CreateTestInstances(new MockGameBoardWithNoResourcesCollected());
            var localGameController = testInstances.LocalGameController;
            LocalGameControllerTestSetup.LaunchGameAndCompleteSetup(localGameController);
            var player = testInstances.MainPlayer;
            var firstOpponent = testInstances.FirstOpponent;
            testInstances.Dice.AddSequence(new[] { 7u, 0u });

            firstOpponent.AddResources(new ResourceClutch(1, 0, 0, 0, 0));

            // Act
            ResourceTransactionList gainedResources = null;
            localGameController.ResourcesTransferredEvent = (ResourceTransactionList r) => { gainedResources = r; };
            localGameController.StartGamePlay();
            localGameController.SetRobberHex(7u);
            localGameController.ChooseResourceFromOpponent(firstOpponent.Id);

            // Assert
            var expectedResources = new ResourceTransactionList();
            expectedResources.Add(new ResourceTransaction(player.Id, firstOpponent.Id, ResourceClutch.OneBrick));
            gainedResources.ShouldBe(expectedResources);
            player.ResourcesCount.ShouldBe(1);
            player.BrickCount.ShouldBe(1);
            firstOpponent.ResourcesCount.ShouldBe(0);
        }

        /// <summary>
        /// Passing in an unknown player id causes an error to be raised.
        /// </summary>
        [Test]
        [Category("LocalGameController")]
        [Category("Main Player Turn")]
        public void StartOfMainPlayerTurn_RollsSevenAndPassesInUnknownPlayerId_MeaningfulErrorIsReceived()
        {
            // Arrange
            var testInstances = LocalGameControllerTestCreator.CreateTestInstances(new MockGameBoardWithNoResourcesCollected());
            var localGameController = testInstances.LocalGameController;
            var player = testInstances.MainPlayer;
            var firstOpponent = testInstances.FirstOpponent;
            var secondOpponent = testInstances.SecondOpponent;
            LocalGameControllerTestSetup.LaunchGameAndCompleteSetup(localGameController);
            testInstances.Dice.AddSequence(new[] { 7u, 0u });

            firstOpponent.AddResources(new ResourceClutch(1, 0, 0, 0, 0));

            // Act
            Boolean resourceTransactionsReceived = false;
            ErrorDetails errorDetails = null;
            localGameController.ResourcesTransferredEvent = (ResourceTransactionList r) => { resourceTransactionsReceived = true; };
            localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };
            localGameController.StartGamePlay();
            localGameController.SetRobberHex(3u);
            localGameController.ChooseResourceFromOpponent(secondOpponent.Id);

            // Assert
            resourceTransactionsReceived.ShouldBeFalse();
            player.ResourcesCount.ShouldBe(0);
            firstOpponent.ResourcesCount.ShouldBe(1);
            firstOpponent.BrickCount.ShouldBe(1);
            errorDetails.ShouldNotBeNull();
            errorDetails.Message.ShouldBe("Cannot pick resource card from invalid opponent.");
        }

        /// <summary>
        /// Passing in the player id when choosing the resource from an opponent causes an error to be raised.
        /// </summary>
        [Test]
        [Category("LocalGameController")]
        [Category("Main Player Turn")]
        public void StartOfMainPlayerTurn_ChooseResourceFromOpponentUsingPlayerId_MeaningfulErrorIsReceived()
        {
            var testInstances = LocalGameControllerTestCreator.CreateTestInstances();
            var localGameController = testInstances.LocalGameController;
            var player = testInstances.MainPlayer;
            LocalGameControllerTestSetup.LaunchGameAndCompleteSetup(localGameController);
            testInstances.Dice.AddSequence(new[] { 7u, 0u });

            // Act
            var resourceTransactionsReceived = false;
            localGameController.ResourcesTransferredEvent = (ResourceTransactionList r) => { resourceTransactionsReceived = true; };

            ErrorDetails errorDetails = null;
            localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

            localGameController.StartGamePlay();
            localGameController.SetRobberHex(3u);

            // Act
            localGameController.ChooseResourceFromOpponent(player.Id);

            // Assert
            resourceTransactionsReceived.ShouldBeFalse();
            player.ResourcesCount.ShouldBe(3);
            testInstances.FirstOpponent.ResourcesCount.ShouldBe(3);
            errorDetails.ShouldNotBeNull();
            errorDetails.Message.ShouldBe("Cannot pick resource card from invalid opponent.");
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
        /// The robber hex set by the player has no adjacent settlements so the returned robbing choices
        /// is null.
        /// </summary>
        [Test]
        [Category("LocalGameController")]
        [Category("Main Player Turn")]
        public void StartOfMainPlayerTurn_RobberLocationHasNoSettlements_ReturnedRobbingChoicesIsNull()
        {
            // Arrange
            MockDice mockDice = null;
            MockPlayer player;
            MockComputerPlayer firstOpponent, secondOpponent, thirdOpponent;
            var localGameController = this.CreateLocalGameControllerAndCompleteGameSetup(out mockDice, out player, out firstOpponent, out secondOpponent, out thirdOpponent);
            mockDice.AddSequence(new[] { 7u });

            // Act
            Dictionary<Guid, Int32> robbingChoices = new Dictionary<Guid, Int32>();
            localGameController.RobbingChoicesEvent = (Dictionary<Guid, Int32> rc) => { robbingChoices = rc; };
            localGameController.StartGamePlay();
            localGameController.SetRobberHex(0u);

            // Assert
            robbingChoices.ShouldBeNull();
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
            MockDice mockDice = null;
            MockPlayer player;
            MockComputerPlayer firstOpponent, secondOpponent, thirdOpponent;
            var localGameController = this.CreateLocalGameControllerAndCompleteGameSetup(out mockDice, out player, out firstOpponent, out secondOpponent, out thirdOpponent);
            mockDice.AddSequence(new[] { 7u, 0u });

            // Act
            ErrorDetails errorDetails = null;
            localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };
            localGameController.StartGamePlay();
            localGameController.SetRobberHex(0u);
            localGameController.ChooseResourceFromOpponent(Guid.NewGuid());

            // Assert
            errorDetails.ShouldNotBeNull();
            errorDetails.Message.ShouldBe("Cannot call 'ChooseResourceFromOpponent' when 'RobbingChoicesEvent' is not raised.");
        }

        /// <summary>
        /// The robber hex set by the player has only player settlements so the returned robbing choices is null.
        /// </summary>
        [Test]
        [Category("LocalGameController")]
        [Category("Main Player Turn")]
        public void StartOfMainPlayerTurn_RobberLocationHasOnlyPlayerSettlements_ReturnedRobbingChoicesIsNull()
        {
            // Arrange
            MockDice mockDice = null;
            MockPlayer player;
            MockComputerPlayer firstOpponent, secondOpponent, thirdOpponent;
            var localGameController = this.CreateLocalGameControllerAndCompleteGameSetup(out mockDice, out player, out firstOpponent, out secondOpponent, out thirdOpponent);
            mockDice.AddSequence(new[] { 7u });

            // Act
            Dictionary<Guid, Int32> robbingChoices = new Dictionary<Guid, Int32>();
            localGameController.RobbingChoicesEvent = (Dictionary<Guid, Int32> rc) => { robbingChoices = rc; };
            localGameController.StartGamePlay();
            localGameController.SetRobberHex(2u);

            // Assert
            robbingChoices.ShouldBeNull();
        }

        /// <summary>
        /// The robber hex set by the player has only player settlements so calling the CallingChooseResourceFromOpponent 
        /// method raises an error
        /// </summary>
        [Test]
        [Category("LocalGameController")]
        [Category("Main Player Turn")]
        public void StartOfMainPlayerTurn_RobberLocationHasOnlyPlayerSettlementsAndCallingChooseResourceFromOpponent_MeaningfulErrorIsReceived()
        {
            // Arrange
            MockDice mockDice = null;
            MockPlayer player;
            MockComputerPlayer firstOpponent, secondOpponent, thirdOpponent;
            var localGameController = this.CreateLocalGameControllerAndCompleteGameSetup(out mockDice, out player, out firstOpponent, out secondOpponent, out thirdOpponent);
            mockDice.AddSequence(new[] { 7u, 0u });

            // Act
            ErrorDetails errorDetails = null;
            localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };
            localGameController.StartGamePlay();
            localGameController.SetRobberHex(2u);
            localGameController.ChooseResourceFromOpponent(Guid.NewGuid());

            // Assert
            errorDetails.ShouldNotBeNull();
            errorDetails.Message.ShouldBe("Cannot call 'ChooseResourceFromOpponent' when 'RobbingChoicesEvent' is not raised.");
        }

        [Test]
        [Category("LocalGameController")]
        public void Load_HexDataOnly_ResourceProvidersLoadedCorrectly()
        {
            // Arrange
            var localGameController = new LocalGameControllerCreator().Create();

            GameBoard boardData = null;
            localGameController.GameLoadedEvent = (PlayerDataModel[] pd, GameBoard bd) => { boardData = bd; };

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
            Tuple<ResourceTypes?, UInt32>[] hexes = boardData.GetHexData();
            hexes.Length.ShouldBe(GameBoard.StandardBoardHexCount);
            hexes[0].ShouldBe(new Tuple<ResourceTypes?, UInt32>(ResourceTypes.Grain, 9));
            hexes[1].ShouldBe(new Tuple<ResourceTypes?, UInt32>(ResourceTypes.Lumber, 8));
            hexes[2].ShouldBe(new Tuple<ResourceTypes?, UInt32>(ResourceTypes.Brick, 5));
            hexes[3].ShouldBe(new Tuple<ResourceTypes?, UInt32>(ResourceTypes.Grain, 12));
            hexes[4].ShouldBe(new Tuple<ResourceTypes?, UInt32>(ResourceTypes.Lumber, 11));
            hexes[5].ShouldBe(new Tuple<ResourceTypes?, UInt32>(ResourceTypes.Ore, 3));
            hexes[6].ShouldBe(new Tuple<ResourceTypes?, UInt32>(ResourceTypes.Grain, 6));
            hexes[7].ShouldBe(new Tuple<ResourceTypes?, UInt32>(ResourceTypes.Ore, 10));
            hexes[8].ShouldBe(new Tuple<ResourceTypes?, UInt32>(ResourceTypes.Brick, 6));
            hexes[9].ShouldBe(new Tuple<ResourceTypes?, UInt32>(null, 0));
            hexes[10].ShouldBe(new Tuple<ResourceTypes?, UInt32>(ResourceTypes.Grain, 4));
            hexes[11].ShouldBe(new Tuple<ResourceTypes?, UInt32>(ResourceTypes.Wool, 11));
            hexes[12].ShouldBe(new Tuple<ResourceTypes?, UInt32>(ResourceTypes.Wool, 2));
            hexes[13].ShouldBe(new Tuple<ResourceTypes?, UInt32>(ResourceTypes.Wool, 4));
            hexes[14].ShouldBe(new Tuple<ResourceTypes?, UInt32>(ResourceTypes.Lumber, 3));
            hexes[15].ShouldBe(new Tuple<ResourceTypes?, UInt32>(ResourceTypes.Wool, 5));
            hexes[16].ShouldBe(new Tuple<ResourceTypes?, UInt32>(ResourceTypes.Lumber, 9));
            hexes[17].ShouldBe(new Tuple<ResourceTypes?, UInt32>(ResourceTypes.Brick, 10));
            hexes[18].ShouldBe(new Tuple<ResourceTypes?, UInt32>(ResourceTypes.Ore, 8));
        }

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

            PlayerDataModel[] playerDataViews = null;
            localGameController.GameLoadedEvent = (PlayerDataModel[] pd, GameBoard bd) => { playerDataViews = pd; };

            // Act
            var streamContent = "<game>" +
              "<players>" +
              "<player id=\"" + player.Id + "\" name=\"" + player.Name + "\" brick=\"" + player.BrickCount + "\" grain=\"" + player.GrainCount + "\" lumber=\"" + player.LumberCount + "\" ore=\"" + player.OreCount + "\" wool=\"" + player.WoolCount + "\" />" +
              "<player id=\"" + firstOpponent.Id + "\" name=\"" + firstOpponent.Name + "\" iscomputer=\"true\" brick=\"" + firstOpponent.BrickCount + "\" grain=\"" + firstOpponent.GrainCount + "\" lumber=\"" + firstOpponent.LumberCount + "\" ore=\"" + firstOpponent.OreCount + "\" wool=\"" + firstOpponent.WoolCount + "\" />" +
              "<player id=\"" + secondOpponent.Id + "\" name=\"" + secondOpponent.Name + "\" iscomputer=\"true\" brick=\"" + secondOpponent.BrickCount + "\" grain=\"" + secondOpponent.GrainCount + "\" lumber=\"" + secondOpponent.LumberCount + "\" ore=\"" + secondOpponent.OreCount + "\" wool=\"" + secondOpponent.WoolCount + "\" />" +
              "<player id=\"" + thirdOpponent.Id + "\" name=\"" + thirdOpponent.Name + "\" iscomputer=\"true\" brick=\"" + thirdOpponent.BrickCount + "\" grain=\"" + thirdOpponent.GrainCount + "\" lumber=\"" + thirdOpponent.LumberCount + "\" ore=\"" + thirdOpponent.OreCount + "\" wool=\"" + thirdOpponent.WoolCount + "\" />" +
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

            this.AssertPlayerDataViewIsCorrect(player, playerDataViews[0]);
            this.AssertPlayerDataViewIsCorrect(firstOpponent, playerDataViews[1]);
            this.AssertPlayerDataViewIsCorrect(secondOpponent, playerDataViews[2]);
            this.AssertPlayerDataViewIsCorrect(thirdOpponent, playerDataViews[3]);
        }

        //TODO: Replace with test for latest Load method
        [Test]
        [Category("LocalGameController")]
        public void Load_PlayerAndInfrastructureData_GameBoardIsAsExpected()
        {
            // Arrange
            Guid playerId = Guid.NewGuid(), firstOpponentId = Guid.NewGuid(), secondOpponentId = Guid.NewGuid(), thirdOpponentId = Guid.NewGuid();
            var localGameController = new LocalGameControllerCreator().Create();

            GameBoard boardData = null;
            localGameController.GameLoadedEvent = (PlayerDataModel[] pd, GameBoard bd) => { boardData = bd; };

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

        [Test]
        public void Scenario_AllPlayersCollectResourcesAsPartOfGameStart()
        {
            // Arrange
            var testInstances = LocalGameControllerTestCreator.CreateTestInstances(new MockGameBoardWithNoResourcesCollectedDuringGameSetup());
            var localGameController = testInstances.LocalGameController;
            var player = testInstances.MainPlayer;
            var firstOpponent = testInstances.FirstOpponent;
            var secondOpponent = testInstances.SecondOpponent;
            var thirdOpponent = testInstances.ThirdOpponent;
            LocalGameControllerTestSetup.LaunchGameAndCompleteSetup(localGameController);
            testInstances.Dice.AddSequence(new UInt32[] { 6u });

            TurnToken turnToken = null;
            localGameController.StartPlayerTurnEvent = (TurnToken t) => { turnToken = t; };

            Dictionary<Guid, ResourceCollection[]> collectedResources = null;
            localGameController.ResourcesCollectedEvent = (Dictionary<Guid, ResourceCollection[]> c) => { collectedResources = c; };

            UInt32 diceRoll = 0;
            localGameController.DiceRollEvent = (uint d1, uint d2) => { diceRoll = d1 + d2; };

            // Act
            localGameController.StartGamePlay();

            // Assert
            var expected = new Dictionary<Guid, ResourceCollection[]>
      {
        { firstOpponent.Id, new [] { new ResourceCollection(18, ResourceClutch.OneOre) } },
        { secondOpponent.Id, new [] { new ResourceCollection(25, ResourceClutch.OneLumber), new ResourceCollection(35, ResourceClutch.OneLumber) } },
        { thirdOpponent.Id, new [] { new ResourceCollection(31, ResourceClutch.OneOre) } },
      };

            diceRoll.ShouldBe(6u);

            collectedResources.ShouldNotBeNull();
            collectedResources.ShouldContainExact(expected);

            player.ResourcesCount.ShouldBe(0);
            firstOpponent.ResourcesCount.ShouldBe(1);
            firstOpponent.OreCount.ShouldBe(1); // Collected at turn start
            secondOpponent.ResourcesCount.ShouldBe(2);
            secondOpponent.LumberCount.ShouldBe(2); // 2 collected at turn start
            thirdOpponent.ResourcesCount.ShouldBe(1);
            thirdOpponent.OreCount.ShouldBe(1); // Collected at turn start
        }

        [Test]
        public void Scenario_AllPlayersCollectResourcesAsPartOfTurnStartAfterComputerPlayerCompletesTheirTurn()
        {
            // Arrange
            var testInstances = LocalGameControllerTestCreator.CreateTestInstances(new MockGameBoardWithResourcesCollectedAfterFirstTurn());
            var localGameController = testInstances.LocalGameController;
            var player = testInstances.MainPlayer;
            var firstOpponent = testInstances.FirstOpponent;
            var secondOpponent = testInstances.SecondOpponent;
            var thirdOpponent = testInstances.ThirdOpponent;
            LocalGameControllerTestSetup.LaunchGameAndCompleteSetup(localGameController);
            testInstances.Dice.AddSequence(new UInt32[] { 3u, 6u });

            TurnToken firstTurnToken = null;
            TurnToken secondTurnToken = null;
            localGameController.StartPlayerTurnEvent = (TurnToken t) =>
            {
                if (firstTurnToken == null)
                {
                    firstTurnToken = t;
                }
                else if (secondTurnToken == null)
                {
                    secondTurnToken = t;
                }
                else
                {
                    throw new Exception("Did not expect to start a third turn");
                }
            };

            localGameController.StartGamePlay();

            Dictionary<Guid, ResourceCollection[]> collectedResources = null;
            localGameController.ResourcesCollectedEvent = (Dictionary<Guid, ResourceCollection[]> c) => { collectedResources = c; };

            UInt32 diceRoll = 0;
            localGameController.DiceRollEvent = (uint d1, uint d2) => { diceRoll = d1 + d2; };

            // Act
            localGameController.EndTurn(firstTurnToken);

            // Assert
            var expected = new Dictionary<Guid, ResourceCollection[]>
      {
        { firstOpponent.Id, new [] { new ResourceCollection(18, ResourceClutch.OneOre) } },
        { secondOpponent.Id, new [] { new ResourceCollection(25, ResourceClutch.OneLumber), new ResourceCollection(35, ResourceClutch.OneLumber) } },
        { thirdOpponent.Id, new [] { new ResourceCollection(31, ResourceClutch.OneOre) } },
      };

            diceRoll.ShouldBe(6u);

            collectedResources.ShouldNotBeNull();
            collectedResources.ShouldContainExact(expected);

            player.ResourcesCount.ShouldBe(0);

            firstOpponent.ResourcesCount.ShouldBe(1);
            firstOpponent.OreCount.ShouldBe(1);

            secondOpponent.ResourcesCount.ShouldBe(2);
            secondOpponent.LumberCount.ShouldBe(2);

            thirdOpponent.ResourcesCount.ShouldBe(1);
            thirdOpponent.OreCount.ShouldBe(1);
        }

        private void AssertPlayerDataViewIsCorrect(IPlayer player, PlayerDataModel playerDataView)
        {
            playerDataView.Id.ShouldBe(player.Id);
            playerDataView.Name.ShouldBe(player.Name);
            playerDataView.PlayedDevelopmentCards.ShouldBeNull();
            playerDataView.HiddenDevelopmentCards.ShouldBe(0);
            playerDataView.ResourceCards.ShouldBe(player.ResourcesCount);
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
            LocalGameControllerTestCreator.PlayerSetup playerSetup = new LocalGameControllerTestCreator.PlayerSetup(player, firstOpponent, secondOpponent, thirdOpponent, mockPlayerPool);
            var localGameController = LocalGameControllerTestCreator.CreateTestInstances(playerSetup, null, null).LocalGameController;

            /*var mockPlayerPool = Substitute.For<IPlayerPool>();
            mockPlayerPool.CreatePlayer().Returns(player);
            mockPlayerPool.CreateComputerPlayer(Arg.Any<GameBoard>()).Returns(firstOpponent, secondOpponent, thirdOpponent);
            var localGameController = new LocalGameControllerCreator()
              .ChangePlayerPool(mockPlayerPool)
              .Create();*/

            PlayerDataModel[] playerDataViews = null;
            localGameController.GameJoinedEvent = (PlayerDataModel[] p) => { playerDataViews = p; };
            localGameController.JoinGame(gameOptions);

            playerDataViews.ShouldNotBeNull();
            playerDataViews.Length.ShouldBe(4);

            this.AssertPlayerDataViewIsCorrect(player, playerDataViews[0]);
            this.AssertPlayerDataViewIsCorrect(firstOpponent, playerDataViews[1]);
            this.AssertPlayerDataViewIsCorrect(secondOpponent, playerDataViews[2]);
            this.AssertPlayerDataViewIsCorrect(thirdOpponent, playerDataViews[3]);
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
