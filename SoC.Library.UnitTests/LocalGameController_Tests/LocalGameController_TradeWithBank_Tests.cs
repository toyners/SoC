
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

      // Act
      localGameController.TradeWithBank(null, paymentResources, requestedResources);

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

      var paymentResources = ResourceClutch.OneBrick * 4;
      var requestedResources = ResourceClutch.OneGrain;

      ErrorDetails errorDetails = null;
      localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

      // Act
      localGameController.TradeWithBank(new TurnToken(), paymentResources, requestedResources);

      // Assert
      errorDetails.ShouldNotBeNull();
      errorDetails.Message.ShouldBe("Turn token not recognised.");
    }

    [Test]
    public void TradeWithBank_SwappingFourBrickForOneGrain_ResourceTransactionCompleted()
    {
      // Arrange
      var bankId = Guid.NewGuid();
      var testInstances = this.TestSetupWithExplictGameBoard(bankId, new MockGameBoardWithNoResourcesCollected());
      var localGameController = testInstances.LocalGameController;

      var player = testInstances.MainPlayer;

      TurnToken turnToken = null;
      localGameController.StartPlayerTurnEvent = (TurnToken t) => { turnToken = t; };

      ResourceTransactionList resources = null;
      localGameController.ResourcesTransferredEvent = (ResourceTransactionList r) => { resources = r; };

      var paymentResources = ResourceClutch.OneBrick * 4;
      var requestedResources = ResourceClutch.OneGrain;

      // Act
      localGameController.TradeWithBank(turnToken, paymentResources, requestedResources);

      // Assert
      resources.ShouldNotBeNull();

      var expected = new ResourceTransactionList();
      expected.Add(new ResourceTransaction(player.Id, bankId, requestedResources));

      AssertToolBox.AssertThatTheResourceTransactionListIsAsExpected(resources, expected);

      player.ResourcesCount.ShouldBe(1);
      player.GrainCount.ShouldBe(1);
    }

    [Test]
    public void TradeWithBank_SwappingFourBrickForOneGrain2_ResourceTransactionCompleted()
    {
      // Arrange
      var bankId = Guid.NewGuid();
      var testInstances = this.TestSetupWithExplictGameBoard(bankId, new MockGameBoardWithNoResourcesCollected());
      var localGameController = testInstances.LocalGameController;

      var player = testInstances.MainPlayer;

      TurnToken turnToken = null;
      localGameController.StartPlayerTurnEvent = (TurnToken t) => { turnToken = t; };

      ResourceTransactionList resources = null;
      localGameController.ResourcesTransferredEvent = (ResourceTransactionList r) => { resources = r; };

      var paymentResources = new ResourceClutch(4, 0, 0, 4, 0);
      var requestedResources = new ResourceClutch(0, 1, 0, 0, 1);

      // Act
      localGameController.TradeWithBank(turnToken, paymentResources, requestedResources);

      // Assert
      resources.ShouldNotBeNull();

      var expected = new ResourceTransactionList();
      expected.Add(new ResourceTransaction(player.Id, bankId, requestedResources));

      AssertToolBox.AssertThatTheResourceTransactionListIsAsExpected(resources, expected);

      player.ResourcesCount.ShouldBe(2);
      player.GrainCount.ShouldBe(1);
      player.WoolCount.ShouldBe(1);
    }

    [Test]
    [TestCase(3, 1)]
    [TestCase(4, 2)]
    public void TradeWithBank_PaymentIsWrong_MeaningfulErrorIsReceived(Int32 paymentAmount, Int32 requestedAmount)
    {
      // Arrange
      var testInstances = this.TestSetup();
      var localGameController = testInstances.LocalGameController;

      TurnToken turnToken = null;
      localGameController.StartPlayerTurnEvent = (TurnToken t) => { turnToken = t; };

      localGameController.ResourcesTransferredEvent = (ResourceTransactionList r) => { throw new Exception("ResourcesTransferredEvent should not be called."); };

      ErrorDetails errorDetails = null;
      localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

      var paymentResources = ResourceClutch.OneBrick * paymentAmount;
      var requestedResources = ResourceClutch.OneGrain * requestedAmount;

      // Act
      localGameController.TradeWithBank(turnToken, paymentResources, requestedResources);

      // Assert
      errorDetails.ShouldNotBeNull();
      errorDetails.Message.ShouldBe("Cannot complete trade with bank: Need to pay " + (requestedAmount * 4) + " brick for " + requestedAmount + " grain. Only paying " + paymentAmount);
    }

    private LocalGameControllerTestCreator.TestInstances TestSetup()
    {
      var testInstances = LocalGameControllerTestCreator.CreateTestInstances(null, null, new GameBoardData(BoardSizes.Standard));
      var localGameController = testInstances.LocalGameController;
      LocalGameControllerTestSetup.LaunchGameAndCompleteSetup(localGameController);

      testInstances.Dice.AddSequence(new[] { 8u }); // First turn roll i.e. no robber triggered

      localGameController.StartGamePlay();

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

      localGameController.StartGamePlay();

      return testInstances;
    }
    #endregion 
  }
}
