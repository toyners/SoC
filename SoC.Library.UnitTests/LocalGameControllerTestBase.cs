
namespace Jabberwocky.SoC.Library.UnitTests
{
  using System;
  using Interfaces;
  using NSubstitute;

  public class LocalGameControllerTestBase
  {
    #region Fields
    protected const String PlayerName = "Player";
    protected const String FirstOpponentName = "Bob";
    protected const String SecondOpponentName = "Sally";
    protected const String ThirdOpponentName = "Rich";

    protected const UInt32 MainSettlementOneLocation = 12u;
    protected const UInt32 FirstSettlementOneLocation = 18u;
    protected const UInt32 SecondSettlementOneLocation = 25u;
    protected const UInt32 ThirdSettlementOneLocation = 31u;

    protected const UInt32 ThirdSettlementTwoLocation = 33u;
    protected const UInt32 SecondSettlementTwoLocation = 35u;
    protected const UInt32 FirstSettlementTwoLocation = 43u;
    protected const UInt32 MainSettlementTwoLocation = 40u;

    protected const UInt32 MainRoadOneEnd = 4;
    protected const UInt32 FirstRoadOneEnd = 17;
    protected const UInt32 SecondRoadOneEnd = 15;
    protected const UInt32 ThirdRoadOneEnd = 30;

    protected const UInt32 ThirdRoadTwoEnd = 32;
    protected const UInt32 SecondRoadTwoEnd = 24;
    protected const UInt32 FirstRoadTwoEnd = 44;
    protected const UInt32 MainRoadTwoEnd = 39;
    #endregion

    #region Methods
    protected void CreateDefaultPlayerInstances(out MockPlayer player, out MockComputerPlayer firstOpponent, out MockComputerPlayer secondOpponent, out MockComputerPlayer thirdOpponent)
    {
      player = new MockPlayer(PlayerName);

      firstOpponent = new MockComputerPlayer(FirstOpponentName);
      firstOpponent.AddInitialInfrastructureChoices(FirstSettlementOneLocation, FirstRoadOneEnd, FirstSettlementTwoLocation, FirstRoadTwoEnd);

      secondOpponent = new MockComputerPlayer(SecondOpponentName);
      secondOpponent.AddInitialInfrastructureChoices(SecondSettlementOneLocation, SecondRoadOneEnd, SecondSettlementTwoLocation, SecondRoadTwoEnd);

      thirdOpponent = new MockComputerPlayer(ThirdOpponentName);
      thirdOpponent.AddInitialInfrastructureChoices(ThirdSettlementOneLocation, ThirdRoadOneEnd, ThirdSettlementTwoLocation, ThirdRoadTwoEnd);
    }

    protected LocalGameController CreateLocalGameController(IDice dice, IPlayer firstPlayer, params IPlayer[] otherPlayers)
    {
      var mockPlayerPool = CreatePlayerPool(firstPlayer, otherPlayers);

      var localGameController = new LocalGameControllerCreator()
        .ChangeDice(dice)
        .ChangePlayerPool(mockPlayerPool)
        .Create();

      localGameController.ErrorRaisedEvent = (ErrorDetails e) => { throw new Exception(e.Message); };

      return localGameController;
    }

    protected LocalGameController CreateLocalGameControllerAndCompleteGameSetup(out MockDice mockDice, out MockPlayer player, out MockComputerPlayer firstOpponent, out MockComputerPlayer secondOpponent, out MockComputerPlayer thirdOpponent)
    {
      var gameSetupOrder = new[] { 12u, 10u, 8u, 6u };
      var gameTurnOrder = gameSetupOrder;
      mockDice = new MockDiceCreator()
        .AddExplicitDiceRollSequence(gameSetupOrder)
        .AddExplicitDiceRollSequence(gameTurnOrder)
        .Create();

      this.CreateDefaultPlayerInstances(out player, out firstOpponent, out secondOpponent, out thirdOpponent);

      var localGameController = this.CreateLocalGameController(mockDice, player, firstOpponent, secondOpponent, thirdOpponent);

      localGameController.JoinGame();
      localGameController.LaunchGame();
      localGameController.StartGameSetup();
      localGameController.ContinueGameSetup(MainSettlementOneLocation, MainRoadOneEnd);
      localGameController.CompleteGameSetup(MainSettlementTwoLocation, MainRoadTwoEnd);
      localGameController.FinalisePlayerTurnOrder();

      return localGameController;
    }

    protected IPlayerPool CreatePlayerPool(IPlayer player, IPlayer[] otherPlayers)
    {
      var mockPlayerPool = Substitute.For<IPlayerPool>();
      mockPlayerPool.CreatePlayer(Arg.Any<Boolean>()).Returns(player, otherPlayers);
      return mockPlayerPool;
    }
    #endregion
  }
}
