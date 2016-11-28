
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
    public void AddPlayer_AddPlayerToNonFullSession_PlayerAdded()
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
    public void AddPlayer_AddEnoughPlayersToFillGame_AllPlayersHaveSameGameToken()
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
    public void AddPlayer_AddEnoughPlayersToFillGame_AllPlayersAreInitialized()
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
    public void AddPlayer_AddEnoughPlayersToFillGame_FirstPlayerGetsToPlaceTown()
    {
      // Arrange
      var diceRollerFactory = this.CreateDiceRollerFactory(new List<UInt32> { 12, 8, 6, 4 });
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
      Assert.IsTrue(client1.TownPlaced);
      Assert.IsFalse(client2.TownPlaced);
      Assert.IsFalse(client3.TownPlaced);
      Assert.IsFalse(client4.TownPlaced);
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

    private GameSessionManager CreateGameSessionManager(IDiceRollerFactory diceRollerFactory, Int32 maximumPlayerCount = 1)
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
      private Int32 index;

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
