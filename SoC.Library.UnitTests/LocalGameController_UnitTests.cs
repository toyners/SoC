
namespace Jabberwocky.SoC.Library.UnitTests
{
  using System;
  using GameBoards;
  using NSubstitute;
  using NUnit.Framework;
  using Shouldly;

  [TestFixture]
  public class LocalGameController_UnitTests
  {
    #region Methods
    [Test]
    public void StartJoiningGame_DefaultGameOptions_PlayerDataReturned()
    {
      var localGameController = new LocalGameController();

      PlayerBase[] players = null;
      localGameController.GameJoinedEvent = (PlayerBase[] p) => { players = p; };
      localGameController.StartJoiningGame(new GameOptions());

      players.ShouldNotBeNull();
      players.Length.ShouldBe(4);
      players[0].ShouldBeOfType<PlayerData>();
      players[1].ShouldBeOfType<PlayerView>();
      players[2].ShouldBeOfType<PlayerView>();
      players[3].ShouldBeOfType<PlayerView>();
    }

    [Test]
    public void StartJoiningGame_NullGameOptions_PlayerDataReturned()
    {
      var localGameController = new LocalGameController();

      PlayerBase[] players = null;
      localGameController.GameJoinedEvent = (PlayerBase[] p) => { players = p; };
      localGameController.StartJoiningGame(null);

      players.ShouldNotBeNull();
      players.Length.ShouldBe(4);
      players[0].ShouldBeOfType<PlayerData>();
      players[1].ShouldBeOfType<PlayerView>();
      players[2].ShouldBeOfType<PlayerView>();
      players[3].ShouldBeOfType<PlayerView>();
    }

    [Test]
    public void StartJoiningGame_LocalGameJoined_InitialBoardPassedBack()
    {
      var localGameController = new LocalGameController();

      localGameController.GameJoinedEvent = (PlayerBase[] p) => { };

      GameBoardData gameBoardData = null;
      localGameController.InitialBoardSetupEvent = (GameBoardData g) => { gameBoardData = g; };

      localGameController.StartJoiningGame(null);

      gameBoardData.ShouldNotBeNull();
    }

    [Test]
    public void StartJoiningGame_LocalGameJoined_TurnTokenPassedBack()
    {
      var localGameController = new LocalGameController();

      localGameController.GameJoinedEvent = (PlayerBase[] p) => { };

      Guid turnToken = Guid.Empty;
      localGameController.StartInitialTurnEvent = (Guid t) => { turnToken = t; };
      localGameController.StartJoiningGame(null);

      turnToken.ShouldNotBe(Guid.Empty);
    }
    #endregion 
  }
}
