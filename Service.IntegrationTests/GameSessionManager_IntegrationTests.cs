
namespace Service.IntegrationTests
{
  using System;
  using System.Collections.Generic;
  using System.Diagnostics;
  using System.Threading;
  using Jabberwocky.SoC.Service;
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

      var client = new TestClient();

      // Act
      gameSessionManager.AddClient(client);
      Thread.Sleep(1000);

      gameSessionManager.StopMatching();

      // Assert
      client.GameJoined.ShouldBe(true);
    }

    [Test]
    public void AddClient_AddEnoughPlayersToFillGame_AllPlayersHaveSameGameToken()
    {
      // Arrange
      var gameSessionManager = this.CreateGameSessionManager(new DiceRollerFactory(), 4);

      var client1 = new TestClient();
      var client2 = new TestClient();
      var client3 = new TestClient();
      var client4 = new TestClient();

      // Act
      gameSessionManager.AddClient(client1);
      gameSessionManager.AddClient(client2);
      gameSessionManager.AddClient(client3);
      gameSessionManager.AddClient(client4);
      Thread.Sleep(1000);

      gameSessionManager.StopMatching();

      // Assert
      (client1.GameToken != Guid.Empty &&
      client1.GameToken == client2.GameToken &&
      client2.GameToken == client3.GameToken &&
      client3.GameToken == client4.GameToken).ShouldBe(true);
    }

    [Test]
    public void AddClient_AddEnoughPlayersToFillGame_AllPlayersAreInitialized()
    {
      // Arrange
      var gameSessionManager = this.CreateGameSessionManager(new DiceRollerFactory(), 4);

      var client1 = new TestClient();
      var client2 = new TestClient();
      var client3 = new TestClient();
      var client4 = new TestClient();

      // Act
      gameSessionManager.AddClient(client1);
      gameSessionManager.AddClient(client2);
      gameSessionManager.AddClient(client3);
      gameSessionManager.AddClient(client4);
      Thread.Sleep(1000);

      gameSessionManager.StopMatching();

      // Assert
      Assert.IsTrue(client1.GameInitialized);
      Assert.IsTrue(client2.GameInitialized);
      Assert.IsTrue(client3.GameInitialized);
      Assert.IsTrue(client4.GameInitialized);
    }

    [Test]
    public void AddClient_AddEnoughPlayersToFillGame_FirstPlayerGetsToPlaceFirstTown()
    {
      var diceRolls = new List<UInt32> { 12, 8, 6, 4 };
      var expectedResults = new Boolean[] { true, false, false, false };
      GameIsFullSoPlayerGetsToPlaceFirstTown(diceRolls, expectedResults);
    }

    [Test]
    public void AddClient_AddEnoughPlayersToFillGame_SecondPlayerGetsToPlaceFirstTown()
    {
      var diceRolls = new List<UInt32> { 8, 12, 6, 4 };
      var expectedResults = new Boolean[] { false, true, false, false };
      GameIsFullSoPlayerGetsToPlaceFirstTown(diceRolls, expectedResults);
    }

    [Test]
    public void AddClient_AddEnoughPlayersToFillGame_ThirdPlayerGetsToPlaceFirstTown()
    {
      var diceRolls = new List<UInt32> { 8, 6, 12, 4 };
      var expectedResults = new Boolean[] { false, false, true, false };
      GameIsFullSoPlayerGetsToPlaceFirstTown(diceRolls, expectedResults);
    }

    [Test]
    public void AddClient_AddEnoughPlayersToFillGame_LastPlayerGetsToPlaceFirstTown()
    {
      var diceRolls = new List<UInt32> { 8, 6, 4, 12 };
      var expectedResults = new Boolean[] { false, false, false, true };
      GameIsFullSoPlayerGetsToPlaceFirstTown(diceRolls, expectedResults);
    }

    private void GameIsFullSoPlayerGetsToPlaceFirstTown(List<UInt32> diceRolls, Boolean[] expectedResults)
    {
      // Arrange
      var diceRollerFactory = this.CreateDiceRollerFactory(diceRolls);
      var gameSessionManager = this.CreateGameSessionManager(diceRollerFactory, 4);

      var client1 = new TestClient();
      var client2 = new TestClient();
      var client3 = new TestClient();
      var client4 = new TestClient();

      // Act
      gameSessionManager.AddClient(client1);
      gameSessionManager.AddClient(client2);
      gameSessionManager.AddClient(client3);
      gameSessionManager.AddClient(client4);
      Thread.Sleep(1000);

      gameSessionManager.StopMatching();

      // Assert
      Assert.AreEqual(client1.TownPlaced, expectedResults[0]);
      Assert.AreEqual(client2.TownPlaced, expectedResults[1]);
      Assert.AreEqual(client3.TownPlaced, expectedResults[2]);
      Assert.AreEqual(client4.TownPlaced, expectedResults[3]);
    }

    private TestDiceRollerFactory CreateDiceRollerFactory(params List<UInt32>[] numbers)
    {
      var diceRollers = new List<TestDiceRoller>();
      foreach (var numberSet in numbers)
      {
        diceRollers.Add(new TestDiceRoller(numberSet));
      }

      return new TestDiceRollerFactory(diceRollers);
    }

    private GameSessionManager CreateGameSessionManager(IDiceRollerFactory diceRollerFactory, UInt32 maximumPlayerCount = 1)
    {
      var gameSessionManager = new GameSessionManager(diceRollerFactory, maximumPlayerCount);
      gameSessionManager.StartMatching();

      var stopWatch = new Stopwatch();
      stopWatch.Start();

      while (!gameSessionManager.IsProcessing && stopWatch.ElapsedMilliseconds < 5000)
      {
        Thread.Sleep(500);
      }

      stopWatch.Stop();
      if (!gameSessionManager.IsProcessing)
      {
        throw new Exception("GameSessionManager has not started");
      }
      
      return gameSessionManager;
    }

    public class TestDiceRollerFactory : IDiceRollerFactory
    {
      private Queue<TestDiceRoller> diceRollers;

      public TestDiceRollerFactory(List<TestDiceRoller> diceRollers)
      {
        this.diceRollers = new Queue<TestDiceRoller>(diceRollers);
      }

      public IDiceRoller Create()
      {
        if (this.diceRollers.Count == 0)
        {
          throw new Exception("No more dice rollers in dice roller factory.");
        }

        return this.diceRollers.Dequeue();
      }
    }

    public class TestDiceRoller : IDiceRoller
    {
      private Queue<UInt32> numbers;

      public TestDiceRoller(List<UInt32> numbers)
      {
        this.numbers = new Queue<UInt32>(numbers);
      } 

      public UInt32 RollTwoDice()
      {
        if (this.numbers.Count == 0)
        {
          throw new Exception("No more numbers in dice roller.");
        }

        return this.numbers.Dequeue();
      }
    }
    #endregion 
  }
}
