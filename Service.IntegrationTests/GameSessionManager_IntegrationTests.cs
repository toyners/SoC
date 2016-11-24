
namespace Service.IntegrationTests
{
  using System;
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
      var gameSessionManager = this.CreateGameSessionManager();
      gameSessionManager.StartMatching();

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
      var gameSessionManager = this.CreateGameSessionManager(4);
      gameSessionManager.StartMatching();

      var client1 = new TestClient();
      var client2 = new TestClient();
      var client3 = new TestClient();
      var client4 = new TestClient();

      // Act
      gameSessionManager.AddClient(client1);
      gameSessionManager.AddClient(client2);
      gameSessionManager.AddClient(client3);
      gameSessionManager.AddClient(client4);
      Thread.Sleep(500);

      gameSessionManager.StopMatching();

      // Assert
      (client1.GameToken != Guid.Empty &&
      client1.GameToken == client2.GameToken &&
      client2.GameToken == client3.GameToken &&
      client3.GameToken == client4.GameToken).ShouldBe(true);
    }

    private GameSessionManager CreateGameSessionManager(Int32 maximumPlayerCount = 1)
    {
      var gameSessionManager = new GameSessionManager(maximumPlayerCount);
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
      
      Thread.Sleep(500);

      return gameSessionManager;
    }
    #endregion 
  }
}
