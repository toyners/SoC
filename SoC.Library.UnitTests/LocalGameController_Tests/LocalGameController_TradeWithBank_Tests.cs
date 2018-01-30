
namespace Jabberwocky.SoC.Library.UnitTests.LocalGameController_Tests
{
  using System;
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
    [TestCase(4, 1, 0)]
    [TestCase(4, 1, 3)]
    [TestCase(8, 2, 0)]
    [TestCase(8, 2, 1)]
    public void TradeWithBank_LegitmateResourcesForTransaction_ResourceTransactionCompleted(Int32 paymentCount, Int32 receivingCount, Int32 otherCount)
    {
      // Arrange
      var bankId = Guid.NewGuid();
      var testInstances = this.TestSetupWithExplictGameBoard(bankId, new MockGameBoardWithNoResourcesCollected());
      var localGameController = testInstances.LocalGameController;

      var paymentResources = ResourceClutch.OneBrick * paymentCount;
      var requestedResources = ResourceClutch.OneGrain * receivingCount;

      var player = testInstances.MainPlayer;
      player.AddResources(ResourceClutch.OneBrick * paymentCount);

      TurnToken turnToken = null;
      localGameController.StartPlayerTurnEvent = (TurnToken t) => { turnToken = t; };

      ResourceTransactionList resources = null;
      localGameController.ResourcesTransferredEvent = (ResourceTransactionList r) => { resources = r; };

      localGameController.StartGamePlay();

      // Act
      localGameController.TradeWithBank(turnToken, ResourceTypes.Grain, 0, ResourceTypes.Brick);

      // Assert
      resources.ShouldNotBeNull();

      var expected = new ResourceTransactionList();
      expected.Add(new ResourceTransaction(bankId, player.Id, paymentResources));
      expected.Add(new ResourceTransaction(player.Id, bankId, requestedResources));

      AssertToolBox.AssertThatTheResourceTransactionListIsAsExpected(resources, expected);

      player.ResourcesCount.ShouldBe(receivingCount + otherCount);
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
      var testInstances = this.TestSetup();
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
      errorDetails.Message.ShouldBe("Cannot complete trade with bank: Need to pay " + (receivingCount * 4) + " brick for " + receivingCount + " grain. Only paying " + paymentCount);
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
      errorDetails.Message.ShouldBe("Cannot complete trade with bank: Receiving count must be positive. Was " + receivingCount);
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
