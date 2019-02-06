
namespace Jabberwocky.SoC.Library.UnitTests.LocalGameController_Tests
{
    using System;
    using GameBoards;
    using Interfaces;
    using NSubstitute;
    using Jabberwocky.Toolkit.Object;
    using Jabberwocky.SoC.Library.UnitTests.Mock;

    public static class LocalGameControllerTestCreator
    {
        #region Fields
        public const String PlayerName = "Player";
        public const String FirstOpponentName = "Bob";
        public const String SecondOpponentName = "Sally";
        public const String ThirdOpponentName = "Rich";

        public const UInt32 MainSettlementOneLocation = 12u;
        public const UInt32 FirstSettlementOneLocation = 18u;
        public const UInt32 SecondSettlementOneLocation = 25u;
        public const UInt32 ThirdSettlementOneLocation = 31u;

        public const UInt32 ThirdSettlementTwoLocation = 33u;
        public const UInt32 SecondSettlementTwoLocation = 35u;
        public const UInt32 FirstSettlementTwoLocation = 43u;
        public const UInt32 MainSettlementTwoLocation = 40u;

        public const UInt32 MainRoadOneEnd = 4;
        public const UInt32 FirstRoadOneEnd = 17;
        public const UInt32 SecondRoadOneEnd = 15;
        public const UInt32 ThirdRoadOneEnd = 30;

        public const UInt32 ThirdRoadTwoEnd = 32;
        public const UInt32 SecondRoadTwoEnd = 24;
        public const UInt32 FirstRoadTwoEnd = 44;
        public const UInt32 MainRoadTwoEnd = 39;
        #endregion

        #region Methods
        public static void CreateDefaultPlayerInstances(out MockPlayer player, out MockComputerPlayer firstOpponent, out MockComputerPlayer secondOpponent, out MockComputerPlayer thirdOpponent)
        {
            player = new MockPlayer(PlayerName);

            firstOpponent = new MockComputerPlayer(FirstOpponentName);
            firstOpponent.AddInitialInfrastructureChoices(FirstSettlementOneLocation, FirstRoadOneEnd, FirstSettlementTwoLocation, FirstRoadTwoEnd);

            secondOpponent = new MockComputerPlayer(SecondOpponentName);
            secondOpponent.AddInitialInfrastructureChoices(SecondSettlementOneLocation, SecondRoadOneEnd, SecondSettlementTwoLocation, SecondRoadTwoEnd);

            thirdOpponent = new MockComputerPlayer(ThirdOpponentName);
            thirdOpponent.AddInitialInfrastructureChoices(ThirdSettlementOneLocation, ThirdRoadOneEnd, ThirdSettlementTwoLocation, ThirdRoadTwoEnd);
        }

        public static IPlayerPool CreateMockPlayerPool(IPlayer player, params IPlayer[] otherPlayers)
        {
            var mockPlayerPool = Substitute.For<IPlayerPool>();
            mockPlayerPool.CreatePlayer().Returns(player);

            var index = 0;
            mockPlayerPool.CreateComputerPlayer(Arg.Any<GameBoard>(), Arg.Any<LocalGameController>(), Arg.Any<INumberGenerator>()).Returns(x =>
              {
                  if (index >= otherPlayers.Length)
                  {
                      throw new Exception("No more computer players to create.");
                  }

                  return otherPlayers[index++];
              });
            return mockPlayerPool;
        }

        public static TestInstances CreateTestInstances()
        {
            return LocalGameControllerTestCreator.CreateTestInstances(null, null, null, null);
        }

        public static TestInstances CreateTestInstances(MockDice mockNumGenerator)
        {
            return LocalGameControllerTestCreator.CreateTestInstances(mockNumGenerator, null, null, null);
        }

        public static TestInstances CreateTestInstances(IDevelopmentCardHolder developmentCardHolder)
        {
            return LocalGameControllerTestCreator.CreateTestInstances(null, null, developmentCardHolder, null);
        }

        public static TestInstances CreateTestInstances(GameBoard gameBoardData)
        {
            return LocalGameControllerTestCreator.CreateTestInstances(null, null, null, gameBoardData);
        }

        public static TestInstances CreateTestInstances(MockDice mockNumGenerator, PlayerSetup playerSetup, IDevelopmentCardHolder developmentCardHolder, GameBoard gameBoard)
        {
            MockDice mockNumberGenerator = null;
            if (mockNumGenerator != null)
                mockNumberGenerator = mockNumGenerator;
            else
                mockNumberGenerator = LocalGameControllerTestCreator.CreateMockNumberGenerator();

            MockPlayer player;
            MockComputerPlayer firstOpponent, secondOpponent, thirdOpponent;
            IPlayerPool playerPool = null;
            if (playerSetup != null)
            {
                player = playerSetup.Player;
                firstOpponent = playerSetup.FirstOpponent;
                secondOpponent = playerSetup.SecondOpponent;
                thirdOpponent = playerSetup.ThirdOpponent;
                playerPool = playerSetup.PlayerPool;
            }
            else
            {
                LocalGameControllerTestCreator.CreateDefaultPlayerInstances(out player, out firstOpponent, out secondOpponent, out thirdOpponent);
                playerPool = LocalGameControllerTestCreator.CreateMockPlayerPool(player, firstOpponent, secondOpponent, thirdOpponent);
            }

            if (developmentCardHolder == null)
            {
                developmentCardHolder = new DevelopmentCardHolder();
            }

            if (gameBoard == null)
            {
                gameBoard = new GameBoard(BoardSizes.Standard);
            }

            var localGameController = new LocalGameController(mockNumberGenerator, playerPool, gameBoard, developmentCardHolder);
            localGameController.ErrorRaisedEvent = (ErrorDetails e) => { throw new Exception(e.Message); };

            var testInstances = new TestInstances(localGameController, player, firstOpponent, secondOpponent, thirdOpponent, mockNumberGenerator);

            return testInstances;
        }

        private static MockDice CreateMockNumberGenerator()
        {
            var gameSetupOrder = new[] { 12u, 10u, 8u, 6u };
            var gameTurnOrder = gameSetupOrder;
            return new MockDiceCreator()
                .AddExplicitDiceRollSequence(gameSetupOrder)
                .AddExplicitDiceRollSequence(gameTurnOrder)
                .Create();
        }
        #endregion

        #region Structures
        public struct TestInstances
        {
            public readonly LocalGameController LocalGameController;
            public readonly MockPlayer MainPlayer;
            public readonly MockComputerPlayer FirstOpponent;
            public readonly MockComputerPlayer SecondOpponent;
            public readonly MockComputerPlayer ThirdOpponent;
            public readonly MockDice Dice;

            public TestInstances(LocalGameController localGameController, MockPlayer player, MockComputerPlayer firstOpponent, MockComputerPlayer secondOpponent, MockComputerPlayer thirdOpponent, MockDice dice)
            {
                this.MainPlayer = player;
                this.FirstOpponent = firstOpponent;
                this.SecondOpponent = secondOpponent;
                this.ThirdOpponent = thirdOpponent;
                this.Dice = dice;
                this.LocalGameController = localGameController;
            }
        }

        public class PlayerSetup
        {
            public MockPlayer Player;
            public MockComputerPlayer FirstOpponent;
            public MockComputerPlayer SecondOpponent;
            public MockComputerPlayer ThirdOpponent;
            public IPlayerPool PlayerPool;

            public PlayerSetup(MockPlayer player, MockComputerPlayer firstOpponent, MockComputerPlayer secondOpponent, MockComputerPlayer thirdOpponent, IPlayerPool playerPool)
            {
                player.VerifyThatObjectIsNotNull();
                firstOpponent.VerifyThatObjectIsNotNull();
                secondOpponent.VerifyThatObjectIsNotNull();
                thirdOpponent.VerifyThatObjectIsNotNull();
                playerPool.VerifyThatObjectIsNotNull();

                this.Player = player;
                this.FirstOpponent = firstOpponent;
                this.SecondOpponent = secondOpponent;
                this.ThirdOpponent = thirdOpponent;
                this.PlayerPool = playerPool;
            }
        }
        #endregion
    }
}
