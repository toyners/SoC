
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
    public void TradeWithBank()
    {
      // Arrange
      var testInstances = this.TestSetupWithExplictGameBoard(new MockGameBoardWithNoResourcesCollected());
      var localGameController = testInstances.LocalGameController;

      var player = testInstances.MainPlayer;

      var paymentResources = ResourceClutch.OneBrick * 4;
      var requestedResources = ResourceClutch.OneGrain;

      // Act
      localGameController.TradeWithBank(paymentResources, requestedResources);

      // Assert
      player.ResourcesCount.ShouldBe(1);
      player.GrainCount.ShouldBe(1);
    }

    private LocalGameControllerTestCreator.TestInstances TestSetupWithExplictGameBoard(GameBoardData gameBoard)
    {
      var testInstances = LocalGameControllerTestCreator.CreateTestInstances(null, null, gameBoard);
      var localGameController = testInstances.LocalGameController;
      LocalGameControllerTestSetup.LaunchGameAndCompleteSetup(localGameController);

      testInstances.Dice.AddSequence(new[] { 8u }); // First turn roll i.e. no robber triggered

      localGameController.StartGamePlay();

      return testInstances;
    }
    #endregion 
  }
}
