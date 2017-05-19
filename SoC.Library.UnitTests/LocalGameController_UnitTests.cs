
namespace Jabberwocky.SoC.Library.UnitTests
{
  using System;
  using GameBoards;
  using Interfaces;
  using NSubstitute;
  using NUnit.Framework;
  using Shouldly;

  [TestFixture]
  public class LocalGameController_UnitTests
  {
    #region Methods
    [Test]
    public void StartJoiningGame_DefaultGameOptions_PlayerDataPassedBack()
    {
      var localGameController = this.CreateLocalGameController();

      PlayerBase[] players = null;
      localGameController.GameJoinedEvent = (PlayerBase[] p) => { players = p; };
      localGameController.StartJoiningGame(new GameOptions());
      //localGameController.Quit();
      
      players.ShouldNotBeNull();
      players.Length.ShouldBe(4);
      players[0].ShouldBeOfType<PlayerData>();
      players[1].ShouldBeOfType<PlayerView>();
      players[2].ShouldBeOfType<PlayerView>();
      players[3].ShouldBeOfType<PlayerView>();
    }

    [Test]
    public void StartJoiningGame_NullGameOptions_PlayerDataPassedBack()
    {
      var localGameController = this.CreateLocalGameController();

      PlayerBase[] players = null;
      localGameController.GameJoinedEvent = (PlayerBase[] p) => { players = p; };
      localGameController.StartJoiningGame(null);
      localGameController.Quit();

      players.ShouldNotBeNull();
      players.Length.ShouldBe(4);
      players[0].ShouldBeOfType<PlayerData>();
      players[1].ShouldBeOfType<PlayerView>();
      players[2].ShouldBeOfType<PlayerView>();
      players[3].ShouldBeOfType<PlayerView>();
    }

    [Test]
    public void StartJoiningGame_TryLaunchingGameWithoutJoining_ReturnsFalse()
    {
      var localGameController = this.CreateLocalGameController();
      localGameController.TryLaunchGame().ShouldBeFalse();
    }

    [Test]
    public void StartJoiningGame_TryLaunchingGameAfterJoining_ReturnsTrue()
    {
      var localGameController = this.CreateLocalGameController();
      localGameController.StartJoiningGame(null);
      localGameController.TryLaunchGame().ShouldBeTrue();
    }

    [Test]
    public void StartJoiningGame_GameLaunchedWithPlayerFirstInInitialSetupRound_InitialBoardPassedBack()
    {
      var mockDice = NSubstitute.Substitute.For<IDice>();
      mockDice.RollTwoDice().Returns(12u, 10u, 8u, 2u);
      var localGameController = this.CreateLocalGameController(mockDice);

      PlayerBase[] players = null;
      GameBoardData gameBoardData = null;
      localGameController.GameJoinedEvent = (PlayerBase[] p) => { players = p; };
      localGameController.InitialBoardSetupEvent = (GameBoardData g) => { gameBoardData = g; };

      localGameController.StartJoiningGame(null);
      localGameController.TryLaunchGame();

      gameBoardData.ShouldNotBeNull();
      gameBoardData.Settlements.Count.ShouldBe(0);
    }

    [Test]
    public void StartJoiningGame_GameLaunched_TurnTokenPassedBack()
    {
      var localGameController = this.CreateLocalGameController();

      Guid turnToken = Guid.Empty;
      localGameController.StartInitialTurnEvent = (Guid t) => { turnToken = t; };
      localGameController.StartJoiningGame(null);
      localGameController.Quit();

      turnToken.ShouldNotBe(Guid.Empty);
    }

    [Test]
    public void InitialOrderSetupTest()
    {
      var player1 = new PlayerData();
      var player2 = new PlayerView();
      var player3 = new PlayerView();
      var player4 = new PlayerView();

      var players = new PlayerBase[] { player1, player2, player3, player4 };
      var mockDice = NSubstitute.Substitute.For<IDice>();
      mockDice.RollTwoDice().Returns(12u, 10u, 8u, 6u);
      var setupOrder = SetupOrderCreator.Create(players, mockDice);

      setupOrder.ShouldBe(new PlayerBase[] { null });
    }

    private static class SetupOrderCreator
    {
      public static PlayerBase[] Create(PlayerBase[] players, IDice dice)
      {
        throw new NotImplementedException();
      }
    }

    private LocalGameController CreateLocalGameController()
    {
      return this.CreateLocalGameController(new Dice());
    }

    private LocalGameController CreateLocalGameController(IDice diceRoller)
    {
      var localGameController = new LocalGameController(diceRoller);
      localGameController.GameJoinedEvent = (PlayerBase[] players) => { };
      return localGameController;
    }
    #endregion 
  }
}
