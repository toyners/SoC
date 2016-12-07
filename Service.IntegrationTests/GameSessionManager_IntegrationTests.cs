
namespace Service.IntegrationTests
{
  using System;
  using System.Collections.Generic;
  using System.Diagnostics;
  using System.Threading;
  using Jabberwocky.SoC.Service;
  using NSubstitute;
  using NUnit.Framework;
  using Shouldly;

  [TestFixture]
  public class GameSessionManager_IntegrationTests
  {
    #region Methods
    [Test]
    public void AddClient_AddPlayerToNonFullSession_PlayerAdded()
    {
      // Arrange
      var gameSessionManager = this.CreateGameSessionManager(new DiceRollerFactory());
      var mockClient = new MockClient();

      // Act
      gameSessionManager.AddClient(mockClient);
      Thread.Sleep(1000);
      gameSessionManager.Stop();

      while (gameSessionManager.State != GameSessionManager.States.Stopped)
      {
        Thread.Sleep(500);
      }

      // Assert
      mockClient.GameJoined.ShouldBeTrue();
    }

    [Test]
    public void AddClient_AddEnoughPlayersToFillGame_AllPlayersHaveSameGameToken()
    {
      // Arrange
      Guid token = Guid.Empty;
      var gameSessionManager = this.CreateGameSessionManager(new DiceRollerFactory(), 4);

      var mockClient1 = new MockClient();
      var mockClient2 = new MockClient();
      var mockClient3 = new MockClient();
      var mockClient4 = new MockClient();

      // Act
      gameSessionManager.AddClient(mockClient1);
      gameSessionManager.AddClient(mockClient2);
      gameSessionManager.AddClient(mockClient3);
      gameSessionManager.AddClient(mockClient4);
      Thread.Sleep(1000);

      gameSessionManager.Stop();

      // Assert
      mockClient1.GameToken.ShouldNotBe(Guid.Empty);
      (mockClient1.GameToken == mockClient2.GameToken &&
       mockClient2.GameToken == mockClient3.GameToken &&
       mockClient3.GameToken == mockClient4.GameToken).ShouldBeTrue();
    }

    [Test]
    public void AddClient_AddEnoughPlayersToFillGame_AllPlayersAreInitialized()
    {
      // Arrange
      var gameSessionManager = this.CreateGameSessionManager(new DiceRollerFactory(), 4);

      var mockClient1 = new MockClient();
      var mockClient2 = new MockClient();
      var mockClient3 = new MockClient();
      var mockClient4 = new MockClient();

      // Act
      gameSessionManager.AddClient(mockClient1);
      gameSessionManager.AddClient(mockClient2);
      gameSessionManager.AddClient(mockClient3);
      gameSessionManager.AddClient(mockClient4);
      Thread.Sleep(1000);

      gameSessionManager.Stop();

      // Assert
      mockClient1.GameInitialized.ShouldBeTrue();
      mockClient2.GameInitialized.ShouldBeTrue();
      mockClient3.GameInitialized.ShouldBeTrue();
      mockClient4.GameInitialized.ShouldBeTrue();
    }

    [Test]
    public void AddClient_AddEnoughPlayersToFillGame_FirstPlayerGetsToPlaceFirstTown()
    {
      var diceRolls = new List<UInt32> { 12u, 8u, 6u, 4u };
      Boolean[] expectedResults = { true, false, false, false };
      GameIsFullSoPlayerGetsToPlaceFirstTown(diceRolls, expectedResults);
    }

    [Test]
    public void AddClient_AddEnoughPlayersToFillGame_SecondPlayerGetsToPlaceFirstTown()
    {
      var diceRolls = new List<UInt32> { 8u, 12u, 6u, 4u };
      Boolean[] expectedResults = { false, true, false, false };
      GameIsFullSoPlayerGetsToPlaceFirstTown(diceRolls, expectedResults);
    }

    [Test]
    public void AddClient_AddEnoughPlayersToFillGame_ThirdPlayerGetsToPlaceFirstTown()
    {
      var diceRolls = new List<UInt32> { 8u, 6u, 12u, 4u };
      Boolean[] expectedResults = { false, false, true, false };
      GameIsFullSoPlayerGetsToPlaceFirstTown(diceRolls, expectedResults);
    }

    [Test]
    public void AddClient_AddEnoughPlayersToFillGame_LastPlayerGetsToPlaceFirstTown()
    {
      var diceRolls = new List<UInt32> { 8u, 6u, 4u, 12u };
      Boolean[] expectedResults = { false, false, false, true };
      GameIsFullSoPlayerGetsToPlaceFirstTown(diceRolls, expectedResults);
    }

    private void GameIsFullSoPlayerGetsToPlaceFirstTown(List<UInt32> diceRolls, Boolean[] expectedResults)
    {
      // Arrange
      Guid gameToken = Guid.Empty;
      var index = 0;
      var diceRoller = Substitute.For<IDiceRoller>();
      diceRoller.RollTwoDice().Returns(x => { return diceRolls[index++]; });

      var diceRollerFactory = Substitute.For<IDiceRollerFactory>();
      diceRollerFactory.Create().Returns(diceRoller);
        
      var gameSessionManager = this.CreateGameSessionManager(diceRollerFactory, 4);

      var mockClient1 = new MockClient();
      var mockClient2 = new MockClient();
      var mockClient3 = new MockClient();
      var mockClient4 = new MockClient();

      // Act
      gameSessionManager.AddClient(mockClient1);
      gameSessionManager.AddClient(mockClient2);
      gameSessionManager.AddClient(mockClient3);
      gameSessionManager.AddClient(mockClient4);
      Thread.Sleep(1000);

      gameSessionManager.ConfirmGameInitialized(mockClient1.GameToken, mockClient1);
      gameSessionManager.ConfirmGameInitialized(mockClient1.GameToken, mockClient2);
      gameSessionManager.ConfirmGameInitialized(mockClient1.GameToken, mockClient3);
      gameSessionManager.ConfirmGameInitialized(mockClient1.GameToken, mockClient4);
      Thread.Sleep(10000);

      gameSessionManager.Stop();

      while (gameSessionManager.State != GameSessionManager.States.Stopped)
      {
        Thread.Sleep(50);
      }

      // Assert
      (mockClient1.TownPlaced == expectedResults[0]).ShouldBeTrue();
      (mockClient2.TownPlaced == expectedResults[1]).ShouldBeTrue();
      (mockClient3.TownPlaced == expectedResults[2]).ShouldBeTrue();
      (mockClient4.TownPlaced == expectedResults[3]).ShouldBeTrue();
    }

    private GameSessionManager CreateGameSessionManager(IDiceRollerFactory diceRollerFactory, UInt32 maximumPlayerCount = 1)
    {
      var gameSessionManager = new GameSessionManager(diceRollerFactory, maximumPlayerCount);
      gameSessionManager.Start();

      var stopWatch = new Stopwatch();
      stopWatch.Start();

      // Wait until the game session manager is started before continuing. Set a limit of 5 seconds for this to happen.
      while (gameSessionManager.State != GameSessionManager.States.Running && stopWatch.ElapsedMilliseconds < 5000)
      {
        Thread.Sleep(50);
      }

      stopWatch.Stop();

      // Still not started.
      if (gameSessionManager.State != GameSessionManager.States.Running)
      {
        throw new Exception("GameSessionManager has not started");
      }
      
      return gameSessionManager;
    }
    #endregion 
  }
}
