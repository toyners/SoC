
namespace Jabberwocky.SoC.Library.UnitTests
{
  using System;
  using Interfaces;
  using NSubstitute;
  using NUnit.Framework;
  using Shouldly;

  public class testbase
  {
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

    protected IPlayerPool CreatePlayerPool(IPlayer player, IPlayer[] otherPlayers)
    {
      var mockPlayerPool = Substitute.For<IPlayerPool>();
      mockPlayerPool.CreatePlayer(Arg.Any<Boolean>()).Returns(player, otherPlayers);
      return mockPlayerPool;
    }

  }

  [TestFixture]
  [Category("All")]
  [Category("LocalGameController")]
  [Category("LocalGameController.BuildSettlement")]
  public class LocalGameController_BuildSettlement_Tests : testbase
  {
    #region Methods
    [Test]
    public void BuildSettlement_ValidScenario_SettlementBuiltEventRaised()
    {
      // Arrange
      MockDice mockDice = null;
      MockPlayer player;
      MockComputerPlayer firstOpponent, secondOpponent, thirdOpponent;
      var localGameController = this.CreateLocalGameControllerAndCompleteGameSetup(out mockDice, out player, out firstOpponent, out secondOpponent, out thirdOpponent);

      mockDice.AddSequence(new[] { 8u });
      player.AddResources(new ResourceClutch(1, 1, 1, 0, 1));

      Boolean settlementBuilt = false;
      localGameController.SettlementBuiltEvent = () => { settlementBuilt = true; };

      TurnToken turnToken = null;
      localGameController.StartPlayerTurnEvent = (TurnToken t) => { turnToken = t; };
      localGameController.StartGamePlay();
      localGameController.BuildRoadSegment(turnToken, MainRoadOneEnd, 3);

      // Act
      localGameController.BuildSettlement(turnToken, 3);

      // Assert
      settlementBuilt.ShouldBeTrue();
    }

    [Test]
    [TestCase(1, 1, 1, 0, 1)]
    public void BuildSettlement_InsufficientResources_MeaningfulErrorIsReceived(Int32 brickCount, Int32 grainCount, Int32 lumberCount, Int32 oreCount, Int32 woolCount)
    {
      // Arrange
      MockDice mockDice = null;
      MockPlayer player;
      MockComputerPlayer firstOpponent, secondOpponent, thirdOpponent;
      var localGameController = this.CreateLocalGameControllerAndCompleteGameSetup(out mockDice, out player, out firstOpponent, out secondOpponent, out thirdOpponent);

      mockDice.AddSequence(new[] { 8u });
      player.AddResources(new ResourceClutch(brickCount, grainCount, lumberCount, oreCount, woolCount));

      Boolean settlementBuilt = false;
      localGameController.SettlementBuiltEvent = () => { settlementBuilt = true; };

      TurnToken turnToken = null;
      localGameController.StartPlayerTurnEvent = (TurnToken t) => { turnToken = t; };
      localGameController.StartGamePlay();
      localGameController.BuildRoadSegment(turnToken, MainRoadOneEnd, 3);

      // Act
      localGameController.BuildSettlement(turnToken, 3);

      // Assert
      settlementBuilt.ShouldBeTrue();
    }
    #endregion 
  }
}
