
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
    /// Creates the game session manager and then starts them. Throws time out exception if the gsm is not started
    /// after 5 seconds.
    /// </summary>
    /// <param name="gameManagerFactory"></param>
    /// <param name="maximumPlayerCount"></param>
    /// <returns></returns>
    public static GameSessionManager CreateGameSessionManagerForTest(IGameManagerFactory gameManagerFactory, UInt32 maximumPlayerCount = 1)
    {
      var gameSessionManager = new GameSessionManager(gameManagerFactory, maximumPlayerCount);
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
