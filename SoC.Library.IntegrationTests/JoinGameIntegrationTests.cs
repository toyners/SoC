
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
      PlayerDataView[] playerData = null;
      gameController.GameJoinedEvent = (PlayerDataView[] p) => { playerData = p; };
      gameController.JoinGame(gameOptions);

      playerData.ShouldNotBeNull();
      playerData.Length.ShouldBe(4);
      playerData[0].ShouldBeOfType<PlayerData>();
      playerData[1].ShouldBeOfType<PlayerDataView>();
      playerData[2].ShouldBeOfType<PlayerDataView>();
      playerData[3].ShouldBeOfType<PlayerDataView>();
    }

    [Test]
    public void StartGameUsingNullGameOptions()
    {
      var gameControllerSetup = new GameControllerSetup();
      var gameControllerFactory = new GameControllerFactory();
      var gameController = gameControllerFactory.Create(null, gameControllerSetup);
      PlayerDataView[] playerData = null;
      gameController.GameJoinedEvent = (PlayerDataView[] p) => { playerData = p; };
      gameController.JoinGame(null);

      playerData.ShouldNotBeNull();
      playerData.Length.ShouldBe(4);
      playerData[0].ShouldBeOfType<PlayerData>();
      playerData[1].ShouldBeOfType<PlayerDataView>();
      playerData[2].ShouldBeOfType<PlayerDataView>();
      playerData[3].ShouldBeOfType<PlayerDataView>();
    }
    #endregion 
  }
}
