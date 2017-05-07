
namespace Service.UnitTests
{
  using System;
  using System.Diagnostics;
  using System.Threading;
  using Jabberwocky.SoC.Library;
  using Jabberwocky.SoC.Library.Interfaces;
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
    public static void AddTestClients(this Jabberwocky.SoC.Service.GameSessionManager gameSessionManager, params TestClient[] testClients)
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
    public static Jabberwocky.SoC.Service.GameSessionManager CreateGameSessionManagerForTest(UInt32 maximumPlayerCount)
    {
      return new Jabberwocky.SoC.Service.GameSessionManager(maximumPlayerCount, GameSessionManagerUnitTestLoggingPath);
    }

    public static Jabberwocky.SoC.Service.GameSessionManager AddGameManagerFactory(this Jabberwocky.SoC.Service.GameSessionManager gameSessionManager, IGameSessionManager gameManagerFactory)
    {
      gameSessionManager.GameManagerFactory = gameManagerFactory;
      return gameSessionManager;
    }

    public static Jabberwocky.SoC.Service.GameSessionManager AddLoggerFactory(this Jabberwocky.SoC.Service.GameSessionManager gameSessionManager, ILoggerFactory loggerFactory)
    {
      gameSessionManager.LoggerFactory = loggerFactory;
      return gameSessionManager;
    }

    public static Jabberwocky.SoC.Service.GameSessionManager AddPlayerCardRepository(this Jabberwocky.SoC.Service.GameSessionManager gameSessionManager, IPlayerCardRepository playerCardRepostory)
    {
      gameSessionManager.PlayerCardRepository = playerCardRepostory;
      return gameSessionManager;
    }

    public static Jabberwocky.SoC.Service.GameSessionManager AddGameSessionTokenFactory(this Jabberwocky.SoC.Service.GameSessionManager gameSessionManager, IGameSessionTokenFactory gameSessionTokenFactory)
    {
      gameSessionManager.GameSessionTokenFactory = gameSessionTokenFactory;
      return gameSessionManager;
    }

    /// <summary>
    /// Stops the game session manager (if not already stopped). Waits for a maximum of 5 seconds for the gsm to stop 
    /// before throwing a time out exception.
    /// </summary>
    /// <param name="gameSessionManager">Game session manager instance.</param>
    public static void WaitUntilGameSessionManagerHasStopped(this Jabberwocky.SoC.Service.GameSessionManager gameSessionManager)
    {
      if (gameSessionManager.State == Jabberwocky.SoC.Service.GameSessionManager.States.Stopped)
      {
        return;
      }

      gameSessionManager.Stop();

      var stopWatch = new Stopwatch();
      stopWatch.Start();

      // Wait until the game session manager is stopped before continuing. Set a limit of 5 seconds for this to happen.
      while (gameSessionManager.State != Jabberwocky.SoC.Service.GameSessionManager.States.Stopped && stopWatch.ElapsedMilliseconds < 5000)
      {
        Thread.Sleep(50);
      }

      stopWatch.Stop();

      // Still not stopped.
      if (gameSessionManager.State != Jabberwocky.SoC.Service.GameSessionManager.States.Stopped)
      {
        throw new Exception("GameSessionManager has not stopped.");
      }
    }

    public static Jabberwocky.SoC.Service.GameSessionManager WaitUntilGameSessionManagerHasStarted(this Jabberwocky.SoC.Service.GameSessionManager gameSessionManager)
    {
      gameSessionManager.Start();

      var stopWatch = new Stopwatch();
      stopWatch.Start();

      // Wait until the game session manager is started before continuing. Set a limit of 5 seconds for this to happen.
      while (gameSessionManager.State != Jabberwocky.SoC.Service.GameSessionManager.States.Running && stopWatch.ElapsedMilliseconds < 5000)
      {
        Thread.Sleep(50);
      }

      stopWatch.Stop();

      // Still not started.
      if (gameSessionManager.State != Jabberwocky.SoC.Service.GameSessionManager.States.Running)
      {
        throw new Exception("GameSessionManager has not started.");
      }

      return gameSessionManager;
    }
  }
}
