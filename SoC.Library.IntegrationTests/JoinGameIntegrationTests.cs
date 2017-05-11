
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
    public void JoinEmptyGameOnLocalMachine()
    {
      var gameControllerFactory = new GameControllerFactory();
      var gameController = gameControllerFactory.Create(GameConnectionTypes.Local);
      Player player = null;
      gameController.GameJoinedEvent = (Player p) => { player = p; };

      gameController.StartJoiningGame(null);

      player.ShouldNotBeNull();
    }

    [Test]
    public void JoinEmptyGameOnRemoteServerMachine()
    {
      var gameControllerFactory = new GameControllerFactory();
      var gameController = gameControllerFactory.Create(GameConnectionTypes.Server);
      Player player = null;
      gameController.GameJoinedEvent = (Player p) => { player = p; };

      gameController.StartJoiningGame(null);

      player.ShouldNotBeNull();
    }
    #endregion 
  }
}
