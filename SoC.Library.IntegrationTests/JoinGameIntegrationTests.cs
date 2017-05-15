
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
    public void StartDefaultGameOnLocalServerMachine()
    {
      var gameOptions = new GameFilter { Connection = GameConnectionTypes.Local };
      var gameControllerFactory = new GameControllerFactory();
      var gameController = gameControllerFactory.Create(gameOptions);
      PlayerBase[] players = null;
      gameController.GameJoinedEvent = (PlayerBase[] p) => { players = p; };
      gameController.StartJoiningGame(null);

      players.ShouldNotBeNull();
      players.Length.ShouldBe(4);
      players[0].ShouldBeOfType<PlayerData>();
      players[1].ShouldBeOfType<PlayerView>();
      players[2].ShouldBeOfType<PlayerView>();
      players[3].ShouldBeOfType<PlayerView>();
    }
    #endregion 
  }
}
