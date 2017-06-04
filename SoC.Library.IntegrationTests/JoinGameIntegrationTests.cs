
namespace SoC.Library.IntegrationTests
{
  using Jabberwocky.SoC.Library;
  using Jabberwocky.SoC.Library.Interfaces;
  using NUnit.Framework;
  using Shouldly;

  [TestFixture]
  public class JoinGameIntegrationTests
  {
    #region Methods
    [Test]
    public void StartGameUsingDefaultGameOptions()
    {
      var gameOptions = new GameOptions();
      var gameControllerSetup = new GameControllerSetup();
      var gameControllerFactory = new GameControllerFactory();
      var gameController = gameControllerFactory.Create(gameOptions, gameControllerSetup);
      IPlayer[] players = null;
      gameController.GameJoinedEvent = (IPlayer[] p) => { players = p; };
      gameController.StartJoiningGame(gameOptions);

      players.ShouldNotBeNull();
      players.Length.ShouldBe(4);
      players[0].ShouldBeOfType<PlayerData>();
      players[1].ShouldBeOfType<PlayerDataView>();
      players[2].ShouldBeOfType<PlayerDataView>();
      players[3].ShouldBeOfType<PlayerDataView>();
    }

    [Test]
    public void StartGameUsingNullGameOptions()
    {
      var gameControllerSetup = new GameControllerSetup();
      var gameControllerFactory = new GameControllerFactory();
      var gameController = gameControllerFactory.Create(null, gameControllerSetup);
      IPlayer[] players = null;
      gameController.GameJoinedEvent = (IPlayer[] p) => { players = p; };
      gameController.StartJoiningGame(null);

      players.ShouldNotBeNull();
      players.Length.ShouldBe(4);
      players[0].ShouldBeOfType<PlayerData>();
      players[1].ShouldBeOfType<PlayerDataView>();
      players[2].ShouldBeOfType<PlayerDataView>();
      players[3].ShouldBeOfType<PlayerDataView>();
    }
    #endregion 
  }
}
