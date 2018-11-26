
namespace SoC.Library.IntegrationTests
{
  using Jabberwocky.SoC.Library;
  using Jabberwocky.SoC.Library.Interfaces;
    using Jabberwocky.SoC.Library.PlayerData;
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
      PlayerDataModel[] playerData = null;
      gameController.GameJoinedEvent = (PlayerDataModel[] p) => { playerData = p; };
      gameController.JoinGame(gameOptions);

      playerData.ShouldNotBeNull();
      playerData.Length.ShouldBe(4);
      playerData[0].ShouldBeOfType<PlayerDataOld>();
      playerData[1].ShouldBeOfType<PlayerDataModel>();
      playerData[2].ShouldBeOfType<PlayerDataModel>();
      playerData[3].ShouldBeOfType<PlayerDataModel>();
    }

    [Test]
    public void StartGameUsingNullGameOptions()
    {
      var gameControllerSetup = new GameControllerSetup();
      var gameControllerFactory = new GameControllerFactory();
      var gameController = gameControllerFactory.Create(null, gameControllerSetup);
      PlayerDataModel[] playerData = null;
      gameController.GameJoinedEvent = (PlayerDataModel[] p) => { playerData = p; };
      gameController.JoinGame(null);

      playerData.ShouldNotBeNull();
      playerData.Length.ShouldBe(4);
      playerData[0].ShouldBeOfType<PlayerDataOld>();
      playerData[1].ShouldBeOfType<PlayerDataModel>();
      playerData[2].ShouldBeOfType<PlayerDataModel>();
      playerData[3].ShouldBeOfType<PlayerDataModel>();
    }
    #endregion 
  }
}
