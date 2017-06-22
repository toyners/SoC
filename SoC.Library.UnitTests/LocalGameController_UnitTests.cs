
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
    #region Fields
    UInt32 firstSettlementOneLocation = 18u;
    UInt32 secondSettlementOneLocation = 25u;
    UInt32 thirdSettlementOneLocation = 31u;

    UInt32 thirdSettlementTwoLocation = 33u;
    UInt32 secondSettlementTwoLocation = 35u;
    UInt32 firstSettlementTwoLocation = 43u;

    Road firstRoadOne = new Road(17u, 18u);
    Road secondRoadOne = new Road(15u, 25u);
    Road thirdRoadOne = new Road(30u, 31u);

    Road thirdRoadTwo = new Road(32u, 33u);
    Road secondRoadTwo = new Road(24u, 35u);
    Road firstRoadTwo = new Road(43u, 44u);
    #endregion

    #region Methods
    [Test]
    public void StartJoiningGame_DefaultGameOptions_PlayerDataPassedBack()
    {
      var localGameController = this.CreateLocalGameController();

      PlayerDataBase[] playerData = null;
      localGameController.GameJoinedEvent = (PlayerDataBase[] p) => { playerData = p; };
      localGameController.TryJoiningGame(new GameOptions());
      
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
      localGameController.TryJoiningGame();
      localGameController.Quit();

      playerData.ShouldNotBeNull();
      playerData.Length.ShouldBe(4);
      playerData[0].ShouldBeOfType<PlayerData>();
      playerData[1].ShouldBeOfType<PlayerDataView>();
      playerData[2].ShouldBeOfType<PlayerDataView>();
      playerData[3].ShouldBeOfType<PlayerDataView>();
    }

    [Test]
    public void TryingToJoinGameMoreThanOnceReturnsFalse()
    {
      var localGameController = this.CreateLocalGameController();
      var joinedCount = 0;
      localGameController.GameJoinedEvent = (PlayerDataBase[] p) => { joinedCount++; };
      localGameController.TryJoiningGame();
      var result = localGameController.TryJoiningGame();

      result.ShouldBeFalse();
      joinedCount.ShouldBe(1);
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

      localGameController.TryJoiningGame();
      localGameController.TryLaunchGame().ShouldBeTrue();
    }

    [Test]
    public void StartJoiningGame_GameLaunched_InitialBoardPassedBack()
    {
      var mockDice = NSubstitute.Substitute.For<IDice>();
      mockDice.RollTwoDice().Returns(12u, 10u, 8u, 2u);
      var localGameController = this.CreateLocalGameController(mockDice, new ComputerPlayerFactory(), new GameBoardManager(BoardSizes.Standard));

      PlayerDataBase[] playerData = null;
      GameBoardData gameBoardData = null;
      localGameController.GameJoinedEvent = (PlayerDataBase[] p) => { playerData = p; };
      localGameController.InitialBoardSetupEvent = (GameBoardData g) => { gameBoardData = g; };

      localGameController.TryJoiningGame();
      localGameController.TryLaunchGame();

      gameBoardData.ShouldNotBeNull();
    }

    [Test]
    public void CompleteSetupWithPlayerInFirstSlot()
    {
      var mockDice = Substitute.For<IDice>();
      mockDice.RollTwoDice().Returns(12u, 10u, 8u, 6u);

      var gameBoardManager = new GameBoardManager(BoardSizes.Standard);

      var firstMockComputerPlayer = this.CreateMockComputerPlayer(gameBoardManager.Data, firstSettlementOneLocation, firstSettlementTwoLocation, firstRoadOne, firstRoadTwo);
      var secondMockComputerPlayer = this.CreateMockComputerPlayer(gameBoardManager.Data, secondSettlementOneLocation, secondSettlementTwoLocation, secondRoadOne, secondRoadTwo);
      var thirdMockComputerPlayer = this.CreateMockComputerPlayer(gameBoardManager.Data, thirdSettlementOneLocation, thirdSettlementTwoLocation, thirdRoadOne, thirdRoadTwo);

      var mockComputerPlayerFactory = Substitute.For<IComputerPlayerFactory>();
      mockComputerPlayerFactory.Create().Returns(firstMockComputerPlayer, secondMockComputerPlayer, thirdMockComputerPlayer);

      GameBoardUpdate gameBoardUpdate = null;
      var localGameController = this.CreateLocalGameController(mockDice, mockComputerPlayerFactory, gameBoardManager);
      localGameController.GameSetupUpdateEvent = (GameBoardUpdate u) => { gameBoardUpdate = u; };
      localGameController.InitialBoardSetupEvent = (GameBoardData d) => { };

      localGameController.TryJoiningGame();
      localGameController.TryLaunchGame();

      gameBoardUpdate = new GameBoardUpdate(); // Ensure that there is a state change for the gameBoardUpdate variable 
      localGameController.StartGameSetup();
      gameBoardUpdate.ShouldBeNull();

      gameBoardUpdate = null; // Ensure that there is a state change for the gameBoardUpdate variable

      var chosenRoad = new Road(0u, 1u);
      localGameController.ContinueGameSetup(0u, chosenRoad);
      gameBoardUpdate.ShouldNotBeNull();
      gameBoardUpdate.NewSettlements.Count.ShouldBe(6);
      gameBoardUpdate.NewSettlements.ShouldContainKeyAndValue(firstSettlementOneLocation, firstMockComputerPlayer.Id);
      gameBoardUpdate.NewSettlements.ShouldContainKeyAndValue(firstSettlementTwoLocation, firstMockComputerPlayer.Id);
      gameBoardUpdate.NewSettlements.ShouldContainKeyAndValue(secondSettlementOneLocation, secondMockComputerPlayer.Id);
      gameBoardUpdate.NewSettlements.ShouldContainKeyAndValue(secondSettlementTwoLocation, secondMockComputerPlayer.Id);
      gameBoardUpdate.NewSettlements.ShouldContainKeyAndValue(thirdSettlementOneLocation, thirdMockComputerPlayer.Id);
      gameBoardUpdate.NewSettlements.ShouldContainKeyAndValue(thirdSettlementTwoLocation, thirdMockComputerPlayer.Id);

      gameBoardUpdate.NewRoads.Count.ShouldBe(6);
      gameBoardUpdate.NewRoads.ShouldContainKeyAndValue(firstRoadOne, firstMockComputerPlayer.Id);
      gameBoardUpdate.NewRoads.ShouldContainKeyAndValue(firstRoadTwo, firstMockComputerPlayer.Id);
      gameBoardUpdate.NewRoads.ShouldContainKeyAndValue(secondRoadOne, secondMockComputerPlayer.Id);
      gameBoardUpdate.NewRoads.ShouldContainKeyAndValue(secondRoadTwo, secondMockComputerPlayer.Id);
      gameBoardUpdate.NewRoads.ShouldContainKeyAndValue(thirdRoadOne, thirdMockComputerPlayer.Id);
      gameBoardUpdate.NewRoads.ShouldContainKeyAndValue(thirdRoadTwo, thirdMockComputerPlayer.Id);

      gameBoardUpdate = new GameBoardUpdate(); // Ensure that there is a state change for the gameBoardUpdate variable 

      chosenRoad = new Road(2u, 3u);
      localGameController.CompleteGameSetup(2u, chosenRoad);
      gameBoardUpdate.ShouldBeNull();
    }

    [Test]
    public void CompleteSetupWithPlayerInSecondSlot()
    {
      var mockDice = Substitute.For<IDice>();
      mockDice.RollTwoDice().Returns(10u, 12u, 8u, 6u);

      var gameBoardManager = new GameBoardManager(BoardSizes.Standard);

      var firstMockComputerPlayer = this.CreateMockComputerPlayer(gameBoardManager.Data, firstSettlementOneLocation, firstSettlementTwoLocation, firstRoadOne, firstRoadTwo);
      var secondMockComputerPlayer = this.CreateMockComputerPlayer(gameBoardManager.Data, secondSettlementOneLocation, secondSettlementTwoLocation, secondRoadOne, secondRoadTwo);
      var thirdMockComputerPlayer = this.CreateMockComputerPlayer(gameBoardManager.Data, thirdSettlementOneLocation, thirdSettlementTwoLocation, thirdRoadOne, thirdRoadTwo);

      var mockComputerPlayerFactory = Substitute.For<IComputerPlayerFactory>();
      mockComputerPlayerFactory.Create().Returns(firstMockComputerPlayer, secondMockComputerPlayer, thirdMockComputerPlayer);

      GameBoardUpdate gameBoardUpdate = null;
      var localGameController = this.CreateLocalGameController(mockDice, mockComputerPlayerFactory, gameBoardManager);
      localGameController.GameSetupUpdateEvent = (GameBoardUpdate u) => { gameBoardUpdate = u; };
      localGameController.InitialBoardSetupEvent = (GameBoardData d) => { };

      localGameController.TryJoiningGame();
      localGameController.TryLaunchGame();

      localGameController.StartGameSetup();
      gameBoardUpdate.ShouldNotBeNull();
      gameBoardUpdate.NewSettlements.Count.ShouldBe(1);
      gameBoardUpdate.NewSettlements.ShouldContainKeyAndValue(firstSettlementOneLocation, firstMockComputerPlayer.Id);
      gameBoardUpdate.NewRoads.Count.ShouldBe(1);
      gameBoardUpdate.NewRoads.ShouldContainKeyAndValue(firstRoadOne, firstMockComputerPlayer.Id);

      gameBoardUpdate = null; // Ensure that there is a state change for the gameBoardUpdate variable 
      localGameController.ContinueGameSetup(0u, new Road(0u, 1u));

      gameBoardUpdate.ShouldNotBeNull();
      gameBoardUpdate.NewSettlements.Count.ShouldBe(4);
      gameBoardUpdate.NewSettlements.ShouldContainKeyAndValue(secondSettlementOneLocation, secondMockComputerPlayer.Id);
      gameBoardUpdate.NewSettlements.ShouldContainKeyAndValue(secondSettlementTwoLocation, secondMockComputerPlayer.Id);
      gameBoardUpdate.NewSettlements.ShouldContainKeyAndValue(thirdSettlementOneLocation, thirdMockComputerPlayer.Id);
      gameBoardUpdate.NewSettlements.ShouldContainKeyAndValue(thirdSettlementTwoLocation, thirdMockComputerPlayer.Id);

      gameBoardUpdate.NewRoads.Count.ShouldBe(4);
      gameBoardUpdate.NewRoads.ShouldContainKeyAndValue(secondRoadOne, secondMockComputerPlayer.Id);
      gameBoardUpdate.NewRoads.ShouldContainKeyAndValue(secondRoadTwo, secondMockComputerPlayer.Id);
      gameBoardUpdate.NewRoads.ShouldContainKeyAndValue(thirdRoadOne, thirdMockComputerPlayer.Id);
      gameBoardUpdate.NewRoads.ShouldContainKeyAndValue(thirdRoadTwo, thirdMockComputerPlayer.Id);

      gameBoardUpdate = null; // Ensure that there is a state change for the gameBoardUpdate variable 
      localGameController.CompleteGameSetup(2u, new Road(2u, 3u));

      gameBoardUpdate.NewSettlements.Count.ShouldBe(1);
      gameBoardUpdate.NewSettlements.ShouldContainKeyAndValue(firstSettlementTwoLocation, firstMockComputerPlayer.Id);
      gameBoardUpdate.NewRoads.Count.ShouldBe(1);
      gameBoardUpdate.NewRoads.ShouldContainKeyAndValue(firstRoadTwo, firstMockComputerPlayer.Id);
    }

    [Test]
    public void PlayerSelectsSameLocationAsComputerplayerDuringSetupWithPlayerInSecondSlot_ErrorDetailsPassedBack()
    {
      var mockDice = Substitute.For<IDice>();
      mockDice.RollTwoDice().Returns(10u, 12u, 8u, 6u);

      var gameBoardManager = new GameBoardManager(BoardSizes.Standard);

      var firstMockComputerPlayer = this.CreateMockComputerPlayer(gameBoardManager.Data, firstSettlementOneLocation, firstSettlementTwoLocation, firstRoadOne, firstRoadTwo);
      var secondMockComputerPlayer = this.CreateMockComputerPlayer(gameBoardManager.Data, secondSettlementOneLocation, secondSettlementTwoLocation, secondRoadOne, secondRoadTwo);
      var thirdMockComputerPlayer = this.CreateMockComputerPlayer(gameBoardManager.Data, thirdSettlementOneLocation, thirdSettlementTwoLocation, thirdRoadOne, thirdRoadTwo);

      var mockComputerPlayerFactory = Substitute.For<IComputerPlayerFactory>();
      mockComputerPlayerFactory.Create().Returns(firstMockComputerPlayer, secondMockComputerPlayer, thirdMockComputerPlayer);

      Exception exception = null;
      var localGameController = this.CreateLocalGameController(mockDice, mockComputerPlayerFactory, gameBoardManager);
      localGameController.ExceptionRaisedEvent = (Exception e) => { exception = e; };

      localGameController.TryJoiningGame();
      localGameController.TryLaunchGame();
      localGameController.StartGameSetup();

      GameBoardUpdate gameBoardUpdate = null;
      localGameController.GameSetupUpdateEvent = (GameBoardUpdate u) => { gameBoardUpdate = u; };

      localGameController.ContinueGameSetup(firstSettlementOneLocation, new Road(0u, 1u));
      exception.ShouldNotBeNull();
      exception.Message.ShouldBe("Cannot place settlement: Location " + firstSettlementOneLocation + " already owned by player " + firstMockComputerPlayer.Id);
      gameBoardUpdate.ShouldBeNull();
    }

    [Test]
    public void PlayerSelectsLocationTooCloseToComputerplayerDuringSetupWithPlayerInSecondSlot_ErrorDetailsPassedBack()
    {
      var mockDice = Substitute.For<IDice>();
      mockDice.RollTwoDice().Returns(10u, 12u, 8u, 6u);

      var gameBoardManager = new GameBoardManager(BoardSizes.Standard);

      var firstMockComputerPlayer = this.CreateMockComputerPlayer(gameBoardManager.Data, firstSettlementOneLocation, firstSettlementTwoLocation, firstRoadOne, firstRoadTwo);
      var secondMockComputerPlayer = this.CreateMockComputerPlayer(gameBoardManager.Data, secondSettlementOneLocation, secondSettlementTwoLocation, secondRoadOne, secondRoadTwo);
      var thirdMockComputerPlayer = this.CreateMockComputerPlayer(gameBoardManager.Data, thirdSettlementOneLocation, thirdSettlementTwoLocation, thirdRoadOne, thirdRoadTwo);

      var mockComputerPlayerFactory = Substitute.For<IComputerPlayerFactory>();
      mockComputerPlayerFactory.Create().Returns(firstMockComputerPlayer, secondMockComputerPlayer, thirdMockComputerPlayer);

      Exception exception = null;
      var localGameController = this.CreateLocalGameController(mockDice, mockComputerPlayerFactory, gameBoardManager);
      localGameController.ExceptionRaisedEvent = (Exception e) => { exception = e; };
      localGameController.TryJoiningGame();
      localGameController.TryLaunchGame();
      localGameController.StartGameSetup();

      GameBoardUpdate gameBoardUpdate = null;
      localGameController.GameSetupUpdateEvent = (GameBoardUpdate u) => { gameBoardUpdate = u; };

      localGameController.ContinueGameSetup(19u, new Road(0u, 1u));

      exception.ShouldNotBeNull();
      exception.Message.ShouldBe("Cannot place settlement: Too close to player " + firstMockComputerPlayer.Id + " at location " + firstSettlementOneLocation);
      gameBoardUpdate.ShouldBeNull();
    }

    [Test]
    public void ContinueGameSetup_CallOutOfSequence_ExpectedExceptionPassedBack()
    {
      var localGameController = this.CreateLocalGameController();
      Exception exception = null;
      localGameController.ExceptionRaisedEvent = (Exception e) => { exception = e; };

      localGameController.ContinueGameSetup(0u, new Road(0u, 1u));

      exception.ShouldNotBeNull();
      exception.Message.ShouldBe("Cannot call 'ContinueGameSetup' until 'StartGameSetup' has completed.");
    }

    [Test]
    public void PlayerSelectsSameRoadAsComputerplayerDuringSetup_MeaningfulExceptionPassedBack()
    {
      /*var mockDice = Substitute.For<IDice>();
      mockDice.RollTwoDice().Returns(10u, 12u, 8u, 6u);

      var gameBoardManager = new GameBoardManager(BoardSizes.Standard);

      var firstMockComputerPlayer = this.CreateMockComputerPlayer(gameBoardManager.Data, firstSettlementOneLocation, firstSettlementTwoLocation, firstRoadOne, firstRoadTwo);
      var secondMockComputerPlayer = this.CreateMockComputerPlayer(gameBoardManager.Data, secondSettlementOneLocation, secondSettlementTwoLocation, secondRoadOne, secondRoadTwo);
      var thirdMockComputerPlayer = this.CreateMockComputerPlayer(gameBoardManager.Data, thirdSettlementOneLocation, thirdSettlementTwoLocation, thirdRoadOne, thirdRoadTwo);

      var mockComputerPlayerFactory = Substitute.For<IComputerPlayerFactory>();
      mockComputerPlayerFactory.Create().Returns(firstMockComputerPlayer, secondMockComputerPlayer, thirdMockComputerPlayer);

      Exception exception = null;
      var localGameController = this.CreateLocalGameController(mockDice, mockComputerPlayerFactory, gameBoardManager);
      localGameController.ExceptionRaisedEvent = (Exception e) => { exception = e; };

      localGameController.TryJoiningGame();
      localGameController.TryLaunchGame();
      localGameController.StartGameSetup();

      GameBoardUpdate gameBoardUpdate = null;
      localGameController.GameSetupUpdateEvent = (GameBoardUpdate u) => { gameBoardUpdate = u; };

      localGameController.ContinueGameSetup(firstSettlementOneLocation, new Road(0u, 1u));
      exception.ShouldNotBeNull();
      exception.Message.ShouldBe("Cannot place settlement: Location " + firstSettlementOneLocation + " already owned by player " + firstMockComputerPlayer.Id);
      gameBoardUpdate.ShouldBeNull();*/

      throw new NotImplementedException();
    }

    [Test]
    public void CompleteSetupWithPlayerInThirdSlot()
    {
      var mockDice = Substitute.For<IDice>();
      mockDice.RollTwoDice().Returns(8u, 12u, 10u, 6u);

      var gameBoardManager = new GameBoardManager(BoardSizes.Standard);

      var firstMockComputerPlayer = this.CreateMockComputerPlayer(gameBoardManager.Data, firstSettlementOneLocation, firstSettlementTwoLocation, firstRoadOne, firstRoadTwo);
      var secondMockComputerPlayer = this.CreateMockComputerPlayer(gameBoardManager.Data, secondSettlementOneLocation, secondSettlementTwoLocation, secondRoadOne, secondRoadTwo);
      var thirdMockComputerPlayer = this.CreateMockComputerPlayer(gameBoardManager.Data, thirdSettlementOneLocation, thirdSettlementTwoLocation, thirdRoadOne, thirdRoadTwo);

      var mockComputerPlayerFactory = Substitute.For<IComputerPlayerFactory>();
      mockComputerPlayerFactory.Create().Returns(firstMockComputerPlayer, secondMockComputerPlayer, thirdMockComputerPlayer);

      GameBoardUpdate gameBoardUpdate = null;
      var localGameController = this.CreateLocalGameController(mockDice, mockComputerPlayerFactory, gameBoardManager);
      localGameController.GameSetupUpdateEvent = (GameBoardUpdate u) => { gameBoardUpdate = u; };
      localGameController.InitialBoardSetupEvent = (GameBoardData d) => { };

      localGameController.TryJoiningGame();
      localGameController.TryLaunchGame();

      localGameController.StartGameSetup();
      gameBoardUpdate.ShouldNotBeNull();
      gameBoardUpdate.NewSettlements.Count.ShouldBe(2);
      gameBoardUpdate.NewSettlements.ShouldContainKeyAndValue(firstSettlementOneLocation, firstMockComputerPlayer.Id);
      gameBoardUpdate.NewSettlements.ShouldContainKeyAndValue(secondSettlementOneLocation, secondMockComputerPlayer.Id);
      gameBoardUpdate.NewRoads.Count.ShouldBe(2);
      gameBoardUpdate.NewRoads.ShouldContainKeyAndValue(firstRoadOne, firstMockComputerPlayer.Id);
      gameBoardUpdate.NewRoads.ShouldContainKeyAndValue(secondRoadOne, secondMockComputerPlayer.Id);

      gameBoardUpdate = null; // Ensure that there is a state change for the gameBoardUpdate variable 
      localGameController.ContinueGameSetup(0u, new Road(0u, 1u));

      gameBoardUpdate.ShouldNotBeNull();
      gameBoardUpdate.NewSettlements.Count.ShouldBe(2);
      gameBoardUpdate.NewSettlements.ShouldContainKeyAndValue(thirdSettlementOneLocation, thirdMockComputerPlayer.Id);
      gameBoardUpdate.NewSettlements.ShouldContainKeyAndValue(thirdSettlementTwoLocation, thirdMockComputerPlayer.Id);

      gameBoardUpdate.NewRoads.Count.ShouldBe(2);
      gameBoardUpdate.NewRoads.ShouldContainKeyAndValue(thirdRoadOne, thirdMockComputerPlayer.Id);
      gameBoardUpdate.NewRoads.ShouldContainKeyAndValue(thirdRoadTwo, thirdMockComputerPlayer.Id);

      gameBoardUpdate = null; // Ensure that there is a state change for the gameBoardUpdate variable 
      localGameController.CompleteGameSetup(2u, new Road(2u, 3u));

      gameBoardUpdate.NewSettlements.Count.ShouldBe(2);
      gameBoardUpdate.NewSettlements.ShouldContainKeyAndValue(secondSettlementTwoLocation, secondMockComputerPlayer.Id);
      gameBoardUpdate.NewSettlements.ShouldContainKeyAndValue(firstSettlementTwoLocation, firstMockComputerPlayer.Id);
      gameBoardUpdate.NewRoads.Count.ShouldBe(2);
      gameBoardUpdate.NewRoads.ShouldContainKeyAndValue(secondRoadTwo, secondMockComputerPlayer.Id);
      gameBoardUpdate.NewRoads.ShouldContainKeyAndValue(firstRoadTwo, firstMockComputerPlayer.Id);
    }

    [Test]
    public void CompleteSetupWithPlayerInFourthSlot()
    {
      var mockDice = Substitute.For<IDice>();
      mockDice.RollTwoDice().Returns(6u, 12u, 10u, 8u);

      var gameBoardManager = new GameBoardManager(BoardSizes.Standard);

      var firstMockComputerPlayer = this.CreateMockComputerPlayer(gameBoardManager.Data, firstSettlementOneLocation, firstSettlementTwoLocation, firstRoadOne, firstRoadTwo);
      var secondMockComputerPlayer = this.CreateMockComputerPlayer(gameBoardManager.Data, secondSettlementOneLocation, secondSettlementTwoLocation, secondRoadOne, secondRoadTwo);
      var thirdMockComputerPlayer = this.CreateMockComputerPlayer(gameBoardManager.Data, thirdSettlementOneLocation, thirdSettlementTwoLocation, thirdRoadOne, thirdRoadTwo);

      var mockComputerPlayerFactory = Substitute.For<IComputerPlayerFactory>();
      mockComputerPlayerFactory.Create().Returns(firstMockComputerPlayer, secondMockComputerPlayer, thirdMockComputerPlayer);

      GameBoardUpdate gameBoardUpdate = null;
      var localGameController = this.CreateLocalGameController(mockDice, mockComputerPlayerFactory, gameBoardManager);
      localGameController.GameSetupUpdateEvent = (GameBoardUpdate u) => { gameBoardUpdate = u; };
      localGameController.InitialBoardSetupEvent = (GameBoardData d) => { };

      localGameController.TryJoiningGame();
      localGameController.TryLaunchGame();

      localGameController.StartGameSetup();
      gameBoardUpdate.ShouldNotBeNull();
      gameBoardUpdate.NewSettlements.Count.ShouldBe(3);
      gameBoardUpdate.NewSettlements.ShouldContainKeyAndValue(firstSettlementOneLocation, firstMockComputerPlayer.Id);
      gameBoardUpdate.NewSettlements.ShouldContainKeyAndValue(secondSettlementOneLocation, secondMockComputerPlayer.Id);
      gameBoardUpdate.NewSettlements.ShouldContainKeyAndValue(thirdSettlementOneLocation, thirdMockComputerPlayer.Id);
      gameBoardUpdate.NewRoads.Count.ShouldBe(3);
      gameBoardUpdate.NewRoads.ShouldContainKeyAndValue(firstRoadOne, firstMockComputerPlayer.Id);
      gameBoardUpdate.NewRoads.ShouldContainKeyAndValue(secondRoadOne, secondMockComputerPlayer.Id);
      gameBoardUpdate.NewRoads.ShouldContainKeyAndValue(thirdRoadOne, thirdMockComputerPlayer.Id);

      gameBoardUpdate = new GameBoardUpdate(); // Ensure that there is a state change for the gameBoardUpdate variable 
      localGameController.ContinueGameSetup(0u, new Road(0u, 1u));
      gameBoardUpdate.ShouldBeNull();

      gameBoardUpdate = null; // Ensure that there is a state change for the gameBoardUpdate variable 
      localGameController.CompleteGameSetup(2u, new Road(2u, 3u));

      gameBoardUpdate.ShouldNotBeNull();
      gameBoardUpdate.NewSettlements.Count.ShouldBe(3);
      gameBoardUpdate.NewSettlements.ShouldContainKeyAndValue(thirdSettlementTwoLocation, thirdMockComputerPlayer.Id);
      gameBoardUpdate.NewSettlements.ShouldContainKeyAndValue(secondSettlementTwoLocation, secondMockComputerPlayer.Id);
      gameBoardUpdate.NewSettlements.ShouldContainKeyAndValue(firstSettlementTwoLocation, firstMockComputerPlayer.Id);
      gameBoardUpdate.NewRoads.Count.ShouldBe(3);
      gameBoardUpdate.NewRoads.ShouldContainKeyAndValue(thirdRoadTwo, thirdMockComputerPlayer.Id);
      gameBoardUpdate.NewRoads.ShouldContainKeyAndValue(secondRoadTwo, secondMockComputerPlayer.Id);
      gameBoardUpdate.NewRoads.ShouldContainKeyAndValue(firstRoadTwo, firstMockComputerPlayer.Id);
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

    private IComputerPlayer CreateMockComputerPlayer(GameBoardData gameBoardData, UInt32 firstSettlementOneLocation, UInt32 firstSettlementTwoLocation, Road firstRoadOne, Road firstRoadTwo)
    {
      var mockComputerPlayer = Substitute.For<IComputerPlayer>();
      var playerId = Guid.NewGuid();
      mockComputerPlayer.Id.Returns(playerId);
      mockComputerPlayer.ChooseSettlementLocation(gameBoardData)
        .Returns(firstSettlementOneLocation, firstSettlementTwoLocation);
      mockComputerPlayer.ChooseRoad(gameBoardData)
        .Returns(firstRoadOne, firstRoadTwo);

      return mockComputerPlayer;
    }
    #endregion 
  }
}
