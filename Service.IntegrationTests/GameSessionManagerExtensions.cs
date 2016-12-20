
namespace Service.IntegrationTests
{
  using System;
  using System.Diagnostics;
  using System.Threading;
  using Jabberwocky.SoC.Library;
  using Jabberwocky.SoC.Service;

  public static class GameSessionManagerExtensions
  {
    public static GameSessionManager CreateGameSessionManager(IGameManagerFactory gameManagerFactory, UInt32 maximumPlayerCount = 1)
    {
      var gameSessionManager = new GameSessionManager(gameManagerFactory, maximumPlayerCount);
      gameSessionManager.WaitUntilGameSessionManagerHasStarted();
      return gameSessionManager;
    }

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
