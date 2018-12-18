
namespace Jabberwocky.SoC.Library.UnitTests.LocalGameController_Tests
{
  using System;
  using Interfaces;
  using Jabberwocky.SoC.Library.UnitTests.Mock;

  public class LocalGameControllerTestBase
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
    [Obsolete("Deprecated. Use LocalGameControllerTestCreator class.")]
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

    [Obsolete("Deprecated. Use LocalGameControllerTestCreator class.")]
    protected LocalGameController CreateLocalGameController(INumberGenerator dice, IPlayer firstPlayer, params IPlayer[] otherPlayers)
    {
      var mockPlayerPool = LocalGameControllerTestCreator.CreateMockPlayerPool(firstPlayer, otherPlayers);

      var localGameController = new LocalGameControllerCreator()
        .ChangeDice(dice)
        .ChangePlayerPool(mockPlayerPool)
        .Create();

      localGameController.ErrorRaisedEvent = (ErrorDetails e) => { throw new Exception(e.Message); };

      return localGameController;
    }

    [Obsolete("Deprecated. Use LocalGameControllerTestCreator and LocalGameControllerTestSetup classes.")]
    protected LocalGameController CreateLocalGameControllerAndCompleteGameSetup(out MockDice mockDice, out MockPlayer player, out MockComputerPlayer firstOpponent, out MockComputerPlayer secondOpponent, out MockComputerPlayer thirdOpponent)
    {
      mockDice = this.CreateMockDice();

      this.CreateDefaultPlayerInstances(out player, out firstOpponent, out secondOpponent, out thirdOpponent);

      var localGameController = this.CreateLocalGameController(mockDice, player, firstOpponent, secondOpponent, thirdOpponent);

      localGameController.JoinGame();
      localGameController.LaunchGame();
      localGameController.StartGameSetup();
      localGameController.ContinueGameSetup(MainSettlementOneLocation, MainRoadOneEnd);
      localGameController.CompleteGameSetup(MainSettlementTwoLocation, MainRoadTwoEnd);

      return localGameController;
    }

    [Obsolete("Deprecated. Use LocalGameControllerTestCreator and LocalGameControllerTestSetup classes.")]
    protected void CompleteGameSetup(LocalGameController localGameController)
    {
      localGameController.JoinGame();
      localGameController.LaunchGame();
      localGameController.StartGameSetup();
      localGameController.ContinueGameSetup(MainSettlementOneLocation, MainRoadOneEnd);
      localGameController.CompleteGameSetup(MainSettlementTwoLocation, MainRoadTwoEnd);
    }

    [Obsolete("Deprecated. Use LocalGameControllerTestCreator class.")]
    protected LocalGameController CreateLocalGameController(INumberGenerator dice, IPlayerPool playerPool, IDevelopmentCardHolder developmentCardHolder)
    {
      var localGameControllerCreator = new LocalGameControllerCreator();

      if (dice != null)
      {
        localGameControllerCreator.ChangeDice(dice);
      }

      if (playerPool != null)
      {
        localGameControllerCreator.ChangePlayerPool(playerPool);
      }

      if (developmentCardHolder != null)
      {
        localGameControllerCreator.ChangeDevelopmentCardHolder(developmentCardHolder);
      }

      var localGameController = localGameControllerCreator.Create();
      localGameController.ErrorRaisedEvent = (ErrorDetails e) => { throw new Exception(e.Message); };

      return localGameController;
    }

    [Obsolete("Deprecated. Use LocalGameControllerTestCreator class.")]
    protected MockDice CreateMockDice()
    {
      var gameSetupOrder = new[] { 12u, 10u, 8u, 6u };
      var gameTurnOrder = gameSetupOrder;
      return new MockDiceCreator()
          .AddExplicitDiceRollSequence(gameSetupOrder)
          .AddExplicitDiceRollSequence(gameTurnOrder)
          .Create();
    }
    #endregion
  }
}
