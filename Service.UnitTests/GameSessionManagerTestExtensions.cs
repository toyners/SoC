
namespace Service.UnitTests
{
  using System;
  using System.Diagnostics;
  using System.Threading;
  using Jabberwocky.SoC.Library;
  using Jabberwocky.SoC.Service;
  using Jabberwocky.Toolkit.Logging;

  public static class GameSessionManagerTestExtensions
  {
    public const String GameSessionManagerUnitTestLoggingPath = @"C:\Projects\SOC_Logging\GameSessionManager_UnitTests\";
     
    /// <summary>
    /// Add mock clients to the game session manager.
    /// </summary>
    /// <param name="gameSessionManager">Game session manager instance.</param>
    /// <param name="testClients">Test clients to add to the game session manager.</param>
    public static void AddTestClients(this GameSessionManager gameSessionManager, params TestClient[] testClients)
    {
      foreach (var testClient in testClients)
      {
        gameSessionManager.AddPlayer(testClient, testClient.Username);
      }
    }

    /// <summary>
    /// Creates the game session manager and then starts it. Throws time out exception if the gsm is not started
    /// after 5 seconds.
    /// </summary>
    /// <param name="maximumPlayerCount"></param>
    /// <returns>Game Session Manager instance.</returns>
    public static GameSessionManager CreateGameSessionManagerForTest(UInt32 maximumPlayerCount)
    {
      return new GameSessionManager(maximumPlayerCount, GameSessionManagerUnitTestLoggingPath);
    }

    public static GameSessionManager AddGameManagerFactory(this GameSessionManager gameSessionManager, IGameManagerFactory gameManagerFactory)
    {
      gameSessionManager.GameManagerFactory = gameManagerFactory;
      return gameSessionManager;
    }

    public static GameSessionManager AddLoggerFactory(this GameSessionManager gameSessionManager, ILoggerFactory loggerFactory)
    {
      gameSessionManager.LoggerFactory = loggerFactory;
      return gameSessionManager;
    }

    public static GameSessionManager AddPlayerCardRepository(this GameSessionManager gameSessionManager, IPlayerCardRepository playerCardRepostory)
    {
      gameSessionManager.PlayerCardRepository = playerCardRepostory;
      return gameSessionManager;
    }

    public static GameSessionManager AddGameSessionTokenFactory(this GameSessionManager gameSessionManager, IGameSessionTokenFactory gameSessionTokenFactory)
    {
      gameSessionManager.GameSessionTokenFactory = gameSessionTokenFactory;
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

    public static GameSessionManager WaitUntilGameSessionManagerHasStarted(this GameSessionManager gameSessionManager)
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

      return gameSessionManager;
    }
  }
}
