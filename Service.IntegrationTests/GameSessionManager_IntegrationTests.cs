
namespace Service.IntegrationTests
{
  using System;
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
      var gameSessionManager = new GameSessionManager();
      gameSessionManager.StartMatching();

      var client = new TestClient();

      gameSessionManager.AddClient(client);

      Thread.Sleep(5000);
      gameSessionManager.StopMatching();

      client.GameJoined.ShouldBe(true);
    }

    [Test]
    public void AddPlayer_AddEnoughPlayersToFillGame_AllPlayersHaveSameGameToken()
    {
      var gameSessionManager = new GameSessionManager(4);
      gameSessionManager.StartMatching();

      var client1 = new TestClient();
      var client2 = new TestClient();
      var client3 = new TestClient();
      var client4 = new TestClient();

      gameSessionManager.AddClient(client1);
      gameSessionManager.AddClient(client2);
      gameSessionManager.AddClient(client3);
      gameSessionManager.AddClient(client4);

      Thread.Sleep(500);
      gameSessionManager.StopMatching();

      (client1.GameToken != Guid.Empty &&
      client1.GameToken == client2.GameToken &&
      client2.GameToken == client3.GameToken &&
      client3.GameToken == client4.GameToken).ShouldBe(true);
    }
    #endregion 
  }
}
