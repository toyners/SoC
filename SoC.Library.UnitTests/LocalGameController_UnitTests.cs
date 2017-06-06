
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

      PlayerDataBase[] playerData = null;
      localGameController.GameJoinedEvent = (PlayerDataBase[] p) => { playerData = p; };
      localGameController.StartJoiningGame(new GameOptions());
      
      playerData.ShouldNotBeNull();
      playerData.Length.ShouldBe(4);
      playerData[0].ShouldBeOfType<PlayerData>();
      playerData[1].ShouldBeOfType<PlayerDataView>();
      playerData[2].ShouldBeOfType<PlayerDataView>();
      playerData[3].ShouldBeOfType<PlayerDataView>();
    }

    [Test]
    public void StartJoiningGame_NullGameOptions_PlayerDataPassedBack()
    {
      var localGameController = this.CreateLocalGameController();

      PlayerDataBase[] playerData = null;
      localGameController.GameJoinedEvent = (PlayerDataBase[] p) => { playerData = p; };
      localGameController.StartJoiningGame(null);
      localGameController.Quit();

      playerData.ShouldNotBeNull();
      playerData.Length.ShouldBe(4);
      playerData[0].ShouldBeOfType<PlayerData>();
      playerData[1].ShouldBeOfType<PlayerDataView>();
      playerData[2].ShouldBeOfType<PlayerDataView>();
      playerData[3].ShouldBeOfType<PlayerDataView>();
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
      var mockDice = NSubstitute.Substitute.For<IDice>();
      mockDice.RollTwoDice().Returns(12u, 10u, 8u, 2u);
      var localGameController = this.CreateLocalGameController(mockDice, new ComputerPlayerFactory(), new GameBoardManager(BoardSizes.Standard));

      localGameController.StartJoiningGame(null);
      localGameController.TryLaunchGame().ShouldBeTrue();
    }

    [Test]
    public void StartJoiningGame_GameLaunchedWithPlayerFirstInInitialSetupRound_InitialBoardPassedBack()
    {
      var mockDice = NSubstitute.Substitute.For<IDice>();
      mockDice.RollTwoDice().Returns(12u, 10u, 8u, 2u);
      var localGameController = this.CreateLocalGameController(mockDice, new ComputerPlayerFactory(), new GameBoardManager(BoardSizes.Standard));

      PlayerDataBase[] playerData = null;
      GameBoardData gameBoardData = null;
      GameBoardUpdate gameBoardUpdate = new GameBoardUpdate();
      localGameController.GameJoinedEvent = (PlayerDataBase[] p) => { playerData = p; };
      localGameController.InitialBoardSetupEvent = (GameBoardData g) => { gameBoardData = g; };
      localGameController.StartInitialSetupTurnEvent = (GameBoardUpdate u) => { gameBoardUpdate = u; };

      localGameController.StartJoiningGame(null);
      localGameController.TryLaunchGame();

      gameBoardData.ShouldNotBeNull();
      gameBoardUpdate.ShouldBeNull();
    }

    [Test]
    public void StartJoiningGame_GameLaunchedWithPlayerSecondInInitialSetupRound_CorrectBoardUpdatePassedBack()
    {
      var mockDice = NSubstitute.Substitute.For<IDice>();
      mockDice.RollTwoDice().Returns(10u, 12u, 8u, 6u);

      var gameBoardManager = new GameBoardManager(BoardSizes.Standard);
      var initialSetupSettlementLocation = 19u;
      var initialSetupRoundTrail = gameBoardManager.Data.Trails[10];

      var playerId = Guid.NewGuid();
      var mockComputerPlayer = Substitute.For<IComputerPlayer>();
      mockComputerPlayer.Id.Returns(playerId, Guid.Empty, Guid.Empty);
      mockComputerPlayer.ChooseSettlementLocation(gameBoardManager.Data).Returns(initialSetupSettlementLocation);
      mockComputerPlayer.ChooseRoad(gameBoardManager.Data).Returns(initialSetupRoundTrail);

      var mockComputerPlayerFactory = Substitute.For<IComputerPlayerFactory>();
      mockComputerPlayerFactory.Create().Returns(mockComputerPlayer);

      var localGameController = this.CreateLocalGameController(mockDice, mockComputerPlayerFactory, gameBoardManager);

      GameBoardUpdate gameBoardUpdate = null;
      localGameController.StartInitialSetupTurnEvent = (GameBoardUpdate u) => { gameBoardUpdate = u; };

      localGameController.StartJoiningGame(null);
      localGameController.TryLaunchGame();

      gameBoardUpdate.ShouldNotBeNull();
      gameBoardUpdate.NewSettlements.Count.ShouldBe(1);
      gameBoardUpdate.NewSettlements.ContainsValue(playerId);
      gameBoardUpdate.NewRoads.Count.ShouldBe(1);
      gameBoardUpdate.NewRoads.ContainsValue(playerId);
    }

    [Test]
    public void StartJoiningGame_GameLaunchedWithPlayerThirdInInitialSetupRound_CorrectBoardUpdatePassedBack()
    {
      var mockDice = NSubstitute.Substitute.For<IDice>();
      mockDice.RollTwoDice().Returns(8u, 10u, 12u, 6u);

      var gameBoardManager = new GameBoardManager(BoardSizes.Standard);
      var initialSetupSettlementLocation = 19u;
      var initialSetupRoundTrail = gameBoardManager.Data.Trails[10];

      var firstMockPlayerId = Guid.NewGuid();
      var firstMockComputerPlayer = Substitute.For<IComputerPlayer>();
      firstMockComputerPlayer.Id.Returns(firstMockPlayerId);
      firstMockComputerPlayer.ChooseSettlementLocation(gameBoardManager.Data).Returns(initialSetupSettlementLocation);
      firstMockComputerPlayer.ChooseRoad(gameBoardManager.Data).Returns(initialSetupRoundTrail);

      var secondMockPlayerId = Guid.NewGuid();
      var secondMockComputerPlayer = Substitute.For<IComputerPlayer>();
      secondMockComputerPlayer.Id.Returns(secondMockPlayerId);
      secondMockComputerPlayer.ChooseSettlementLocation(gameBoardManager.Data).Returns(24u);
      secondMockComputerPlayer.ChooseRoad(gameBoardManager.Data).Returns(gameBoardManager.Data.Trails[20]);

      var mockComputerPlayerFactory = Substitute.For<IComputerPlayerFactory>();
      mockComputerPlayerFactory.Create().Returns(firstMockComputerPlayer, secondMockComputerPlayer);

      var localGameController = this.CreateLocalGameController(mockDice, mockComputerPlayerFactory, gameBoardManager);

      Guid firstPlayerId = Guid.Empty;
      GameBoardUpdate gameBoardUpdate = null;
      localGameController.StartInitialSetupTurnEvent = (GameBoardUpdate u) => { gameBoardUpdate = u; };

      localGameController.StartJoiningGame(null);
      localGameController.TryLaunchGame();

      gameBoardUpdate.ShouldNotBeNull();
      gameBoardUpdate.NewSettlements.Count.ShouldBe(2);
      gameBoardUpdate.NewSettlements.ContainsValue(firstMockPlayerId);
      gameBoardUpdate.NewSettlements.ContainsValue(secondMockPlayerId);
      gameBoardUpdate.NewRoads.Count.ShouldBe(2);
      gameBoardUpdate.NewRoads.ContainsValue(firstMockPlayerId);
      gameBoardUpdate.NewRoads.ContainsValue(secondMockPlayerId);
    }

    private LocalGameController CreateLocalGameController()
    {
      return this.CreateLocalGameController(new Dice(), new ComputerPlayerFactory(), new GameBoardManager(BoardSizes.Standard));
    }

    private LocalGameController CreateLocalGameController(IDice diceRoller, IComputerPlayerFactory computerPlayerFactory, GameBoardManager gameBoardManager)
    {
      var localGameController = new LocalGameController(diceRoller, computerPlayerFactory, gameBoardManager);
      localGameController.GameJoinedEvent = (PlayerDataBase[] players) => { };
      return localGameController;
    }
    #endregion 
  }
}
