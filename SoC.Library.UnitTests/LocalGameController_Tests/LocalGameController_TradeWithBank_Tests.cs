﻿
namespace Jabberwocky.SoC.Library.UnitTests.LocalGameController_Tests
{
  using System;
  using System.Collections.Generic;
  using GameBoards;
  using MockGameBoards;
  using NSubstitute;
  using NUnit.Framework;
  using Shouldly;

  [TestFixture]
  [Category("All")]
  [Category("LocalGameController")]
  [Category("LocalGameController.TradeWithBank")]
  public class LocalGameController_TradeWithBank_Tests
  {
    #region Methods
    [Test]
    public void TradeWithBank_TurnTokenIsNull_MeaningfulErrorIsReceived()
    {
      // Arrange
      var testInstances = this.TestSetup();
      var localGameController = testInstances.LocalGameController;

      var paymentResources = ResourceClutch.OneBrick * 4;
      var requestedResources = ResourceClutch.OneGrain;

      ErrorDetails errorDetails = null;
      localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

      localGameController.StartGamePlay();

      // Act
      localGameController.TradeWithBank(null, ResourceTypes.Grain, 0, ResourceTypes.Brick);

      // Assert
      errorDetails.ShouldNotBeNull();
      errorDetails.Message.ShouldBe("Turn token is null.");
    }

    [Test]
    public void TradeWithBank_TurnTokenNotCorrect_MeaningfulErrorIsReceived()
    {
      // Arrange
      var testInstances = this.TestSetup();
      var localGameController = testInstances.LocalGameController;

      ErrorDetails errorDetails = null;
      localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

      localGameController.StartGamePlay();

      // Act
      localGameController.TradeWithBank(new TurnToken(), ResourceTypes.Grain, 0, ResourceTypes.Brick);

      // Assert
      errorDetails.ShouldNotBeNull();
      errorDetails.Message.ShouldBe("Turn token not recognised.");
    }

    [Test]
    [TestCase(4, 1, 0, 0)]
    [TestCase(4, 1, 3, 0)]
    [TestCase(8, 1, 0, 4)]
    [TestCase(8, 1, 3, 4)]
    [TestCase(8, 2, 0, 0)]
    [TestCase(8, 2, 1, 0)]
    public void TradeWithBank_LegitmateResourcesForTransaction_ResourceTransactionCompleted(Int32 brickCount, Int32 receivingCount, Int32 otherCount, Int32 leftOverBrickCount)
    {
      // Arrange
      var bankId = Guid.NewGuid();
      var testInstances = this.TestSetupWithExplictGameBoard(bankId, new MockGameBoardWithNoResourcesCollected());
      var localGameController = testInstances.LocalGameController;

      var paymentResources = ResourceClutch.OneBrick * (receivingCount * 4);
      var requestedResources = ResourceClutch.OneGrain * receivingCount;

      var player = testInstances.MainPlayer;
      player.AddResources(ResourceClutch.OneBrick * brickCount);
      player.AddResources(ResourceClutch.OneWool * otherCount);

      TurnToken turnToken = null;
      localGameController.StartPlayerTurnEvent = (TurnToken t) => { turnToken = t; };

      ResourceTransactionList resources = null;
      localGameController.ResourcesTransferredEvent = (ResourceTransactionList r) => { resources = r; };

      localGameController.StartGamePlay();

      // Act
      localGameController.TradeWithBank(turnToken, ResourceTypes.Grain, receivingCount, ResourceTypes.Brick);

      // Assert
      resources.ShouldNotBeNull();

      var expected = new ResourceTransactionList();
      expected.Add(new ResourceTransaction(bankId, player.Id, paymentResources));
      expected.Add(new ResourceTransaction(player.Id, bankId, requestedResources));

      AssertToolBox.AssertThatTheResourceTransactionListIsAsExpected(resources, expected);

      player.ResourcesCount.ShouldBe(receivingCount + otherCount + leftOverBrickCount);
      player.BrickCount.ShouldBe(leftOverBrickCount);
      player.GrainCount.ShouldBe(receivingCount);
      player.WoolCount.ShouldBe(otherCount);
    }

    [Test]
    [TestCase(3, 1)]
    [TestCase(4, 2)]
    [TestCase(7, 2)]
    public void TradeWithBank_PaymentIsWrong_MeaningfulErrorIsReceived(Int32 paymentCount, Int32 receivingCount)
    {
      // Arrange
      var bankId = Guid.NewGuid();
      var testInstances = this.TestSetupWithExplictGameBoard(bankId, new MockGameBoardWithNoResourcesCollected());
      var localGameController = testInstances.LocalGameController;
      var player = testInstances.MainPlayer;

      player.AddResources(ResourceClutch.OneBrick * paymentCount);

      TurnToken turnToken = null;
      localGameController.StartPlayerTurnEvent = (TurnToken t) => { turnToken = t; };

      localGameController.ResourcesTransferredEvent = (ResourceTransactionList r) => { throw new Exception("ResourcesTransferredEvent should not be called."); };

      ErrorDetails errorDetails = null;
      localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

      localGameController.StartGamePlay();

      // Act
      localGameController.TradeWithBank(turnToken, ResourceTypes.Grain, receivingCount, ResourceTypes.Brick);

      // Assert
      errorDetails.ShouldNotBeNull();
      errorDetails.Message.ShouldBe("Cannot complete trade with bank: Need to pay " + (receivingCount * 4) + " brick for " + receivingCount + " grain. Only paying " + paymentCount + ".");
    }

    [Test]
    [TestCase(0)]
    [TestCase(-1)]
    public void TradeWithBank_RequestedCountIsWrong_MeaningfulErrorIsReceived(Int32 receivingCount)
    {
      // Arrange
      var testInstances = this.TestSetup();
      var localGameController = testInstances.LocalGameController;
      var player = testInstances.MainPlayer;

      player.AddResources(ResourceClutch.OneBrick * 4);

      TurnToken turnToken = null;
      localGameController.StartPlayerTurnEvent = (TurnToken t) => { turnToken = t; };

      localGameController.ResourcesTransferredEvent = (ResourceTransactionList r) => { throw new Exception("ResourcesTransferredEvent should not be called."); };

      ErrorDetails errorDetails = null;
      localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

      localGameController.StartGamePlay();

      // Act
      localGameController.TradeWithBank(turnToken, ResourceTypes.Grain, receivingCount, ResourceTypes.Brick);

      // Assert
      errorDetails.ShouldNotBeNull();
      errorDetails.Message.ShouldBe("Cannot complete trade with bank: Receiving count must be positive. Was " + receivingCount + ".");
    }

    [Test]
    public void Scenario_OpponentTradesWithBank()
    {
      // Arrange
      var bankId = Guid.NewGuid();
      var testInstances = this.TestSetupWithExplictGameBoard(bankId, new MockGameBoardWithNoResourcesCollected());
      var localGameController = testInstances.LocalGameController;

      testInstances.Dice.AddSequence(new[] { 8u });

      var givingResources = ResourceClutch.OneGrain * 4;
      var firstOpponent = testInstances.FirstOpponent;
      firstOpponent.AddResources(givingResources);

      var tradeWithBankAction = new TradeWithBankAction { GivingType = ResourceTypes.Grain, ReceivingCount = 1, ReceivingType = ResourceTypes.Wool };
      firstOpponent.AddTradeWithBankAction(tradeWithBankAction).EndTurn();

      TurnToken turnToken = null;
      localGameController.StartPlayerTurnEvent = (TurnToken t) => { turnToken = t; };

      var turn = 0;
      var playerActions = new Dictionary<String, List<GameEvent>>();
      var keys = new List<String>();
      localGameController.OpponentActionsEvent = (Guid g, List<GameEvent> e) =>
      {
        var key = turn + "-" + g.ToString();
        keys.Add(key);
        playerActions.Add(key, e);
      };

      localGameController.StartGamePlay();

      // Act
      localGameController.EndTurn(turnToken);

      // Assert
      var expectedTradeWithBankEvent = new TradeWithBankEvent(firstOpponent.Id, bankId, givingResources, ResourceClutch.OneWool);
      playerActions.Count.ShouldBe(1);
      keys.Count.ShouldBe(playerActions.Count);

      AssertToolBox.AssertThatPlayerActionsForTurnAreCorrect(playerActions[keys[0]], expectedTradeWithBankEvent);

      firstOpponent.ResourcesCount.ShouldBe(1);
      firstOpponent.WoolCount.ShouldBe(1);
    }

    private LocalGameControllerTestCreator.TestInstances TestSetup()
    {
      var testInstances = LocalGameControllerTestCreator.CreateTestInstances(null, null, new GameBoardData(BoardSizes.Standard));
      var localGameController = testInstances.LocalGameController;
      LocalGameControllerTestSetup.LaunchGameAndCompleteSetup(localGameController);

      testInstances.Dice.AddSequence(new[] { 8u }); // First turn roll i.e. no robber triggered

      return testInstances;
    }

    private LocalGameControllerTestCreator.TestInstances TestSetupWithExplictGameBoard(Guid bankId, GameBoardData gameBoard)
    {
      MockPlayer player;
      MockComputerPlayer firstOpponent, secondOpponent, thirdOpponent;
      LocalGameControllerTestCreator.CreateDefaultPlayerInstances(out player, out firstOpponent, out secondOpponent, out thirdOpponent);
      var playerPool = LocalGameControllerTestCreator.CreateMockPlayerPool(player, firstOpponent, secondOpponent, thirdOpponent);
      playerPool.GetBankId().Returns(bankId);

      var playerSetup = new LocalGameControllerTestCreator.PlayerSetup(player, firstOpponent, secondOpponent, thirdOpponent, playerPool);

      var testInstances = LocalGameControllerTestCreator.CreateTestInstances(playerSetup, null, gameBoard);
      var localGameController = testInstances.LocalGameController;
      LocalGameControllerTestSetup.LaunchGameAndCompleteSetup(localGameController);

      testInstances.Dice.AddSequence(new[] { 8u }); // First turn roll i.e. no robber triggered

      return testInstances;
    }
    #endregion 
  }
}
