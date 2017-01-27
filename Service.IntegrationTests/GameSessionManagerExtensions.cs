
namespace Service.IntegrationTests
{
  using System;
  using System.Diagnostics;
  using System.Threading;
  using Jabberwocky.SoC.Library;
  using Jabberwocky.SoC.Service;

  public static class GameSessionManagerExtensions
  {
    /// <summary>
    /// Add mock clients to the game session manager.
    /// </summary>
    /// <param name="gameSessionManager">Game session manager instance.</param>
    /// <param name="mockClients">Mock clients to add to the game session manager.</param>
    public static void AddMockClients(this GameSessionManager gameSessionManager, params MockClient3[] mockClients)
    {
      foreach (var mockClient in mockClients)
      {
        gameSessionManager.AddPlayer(mockClient, mockClient.Username);
      }
    }

    /// <summary>
    /// Creates the game session manager and then starts it. Throws time out exception if the gsm is not started
    /// after 5 seconds.
    /// </summary>
    /// <param name="gameManagerFactory"></param>
    /// <param name="maximumPlayerCount"></param>
    /// <returns></returns>
    public static GameSessionManager CreateGameSessionManagerForTest(IGameManagerFactory gameManagerFactory, UInt32 maximumPlayerCount)
    {
      var gameSessionManager = new GameSessionManager(gameManagerFactory, maximumPlayerCount, new PlayerCardRepository());
      gameSessionManager.WaitUntilGameSessionManagerHasStarted();
      return gameSessionManager;
    }

    public static GameSessionManager CreateGameSessionManagerForTest(IGameManagerFactory gameManagerFactory, UInt32 maximumPlayerCount, IPlayerCardRepository playerCardRepository)
    {
      var gameSessionManager = new GameSessionManager(gameManagerFactory, maximumPlayerCount, playerCardRepository);
      gameSessionManager.WaitUntilGameSessionManagerHasStarted();
      return gameSessionManager;
    }

    /// <summary>
    /// Stops the game session manager (if not already stopped). Waits for a maximum of 5 seconds for the gsm to stop 
    /// before throwing a time out exception.
    /// </summary>
    /// <param name="gameSessionManager">Game session manager instance.</param>
    public static void WaitUntilGameSessionManagerHasStopped(this GameSessionManager gameSessionManager)
    {
      if (gameSessionManager.State == GameSessionManager.States.Stopped)
      {
        return;
      }

      gameSessionManager.Stop();

      var stopWatch = new Stopwatch();
      stopWatch.Start();

      // Wait until the game session manager is started before continuing. Set a limit of 5 seconds for this to happen.
      while (gameSessionManager.State != GameSessionManager.States.Stopped && stopWatch.ElapsedMilliseconds < 5000)
      {
        Thread.Sleep(50);
      }

      stopWatch.Stop();

      // Still not stopped.
      if (gameSessionManager.State != GameSessionManager.States.Stopped)
      {
        throw new Exception("GameSessionManager has not stopped.");
      }
    }

    private static void WaitUntilGameSessionManagerHasStarted(this GameSessionManager gameSessionManager)
    {
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
        throw new Exception("GameSessionManager has not started.");
      }
    }
  }
}
