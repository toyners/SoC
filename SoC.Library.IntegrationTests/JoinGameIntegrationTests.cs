
namespace SoC.Library.IntegrationTests
{
  using Jabberwocky.SoC.Library;
  using Jabberwocky.SoC.Library.Enums;
  using NUnit.Framework;
  using Shouldly;

  [TestFixture]
  public class JoinGameIntegrationTests
  {
    #region Methods
    [Test]
    public void StartDefaultGameOnLocalMachine()
    {
      var gameControllerFactory = new GameControllerFactory();
      var gameController = gameControllerFactory.Create(GameConnectionTypes.Local);
      Player[] players = null;
      gameController.GameJoinedEvent = (Player[] p) => { players = p; };

      gameController.StartJoiningGame(null);

      players.ShouldNotBeNull();
      players.Length.ShouldBe(4);
    }

    [Test]
    public void StartSinglePlayerGameOnLocalMachine()
    {
      var gameControllerFactory = new GameControllerFactory();
      var gameController = gameControllerFactory.Create(GameConnectionTypes.Local);
      Player[] players = null;
      gameController.GameJoinedEvent = (Player[] p) => { players = p; };

      var gameFilter = new GameFilter { MaxPlayers = 1, MaxAIPlayers = 3 };
      gameController.StartJoiningGame(gameFilter);

      players.ShouldNotBeNull();
    }

    [Test]
    public void StartHotseatGameOnLocalServerMachine()
    {
      var gameControllerFactory = new GameControllerFactory();
      var gameController = gameControllerFactory.Create(GameConnectionTypes.Local);
      Player[] players = null;
      gameController.GameJoinedEvent = (Player[] p) => { players = p; };

      var gameFilter = new GameFilter { MaxPlayers = 2, MaxAIPlayers = 2 };
      gameController.StartJoiningGame(gameFilter);

      players.ShouldNotBeNull();
    }
    #endregion 
  }
}
