
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
    private UInt32 firstSettlementOneLocation = 18u;
    private UInt32 secondSettlementOneLocation = 25u;
    private UInt32 thirdSettlementOneLocation = 31u;

    private UInt32 thirdSettlementTwoLocation = 33u;
    private UInt32 secondSettlementTwoLocation = 35u;
    private UInt32 firstSettlementTwoLocation = 43u;

    private Road firstRoadOne = new Road(17u, 18u);
    private Road secondRoadOne = new Road(15u, 25u);
    private Road thirdRoadOne = new Road(30u, 31u);

    private Road thirdRoadTwo = new Road(32u, 33u);
    private Road secondRoadTwo = new Road(24u, 35u);
    private Road firstRoadTwo = new Road(43u, 44u);
    #endregion

    #region Methods
    [Test]
    [Category("LocalGameController")]
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
    [Category("LocalGameController")]
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
    [Category("LocalGameController")]
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
    [Category("LocalGameController")]
    public void StartJoiningGame_TryLaunchingGameWithoutJoining_ReturnsFalse()
    {
      var localGameController = this.CreateLocalGameController();
      localGameController.TryLaunchGame().ShouldBeFalse();
    }

    [Test]
    [Category("LocalGameController")]
    public void StartJoiningGame_TryLaunchingGameAfterJoining_ReturnsTrue()
    {
      var mockDice = NSubstitute.Substitute.For<IDice>();
      mockDice.RollTwoDice().Returns(12u, 10u, 8u, 2u);
      var localGameController = this.CreateLocalGameController(mockDice, new ComputerPlayerFactory(), new GameBoardManager(BoardSizes.Standard));

      localGameController.TryJoiningGame();
      localGameController.TryLaunchGame().ShouldBeTrue();
    }

    [Test]
    [Category("LocalGameController")]
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
    [Category("LocalGameController")]
    public void CompleteSetupWithPlayerInFirstSlot()
    {
      var mockDice = Substitute.For<IDice>();
      mockDice.RollTwoDice().Returns(12u, 10u, 8u, 6u);

      var gameBoardManager = new GameBoardManager(BoardSizes.Standard);

      var firstComputerPlayer = this.CreateMockComputerPlayer(gameBoardManager.Data, firstSettlementOneLocation, firstSettlementTwoLocation, firstRoadOne, firstRoadTwo);
      var secondComputerPlayer = this.CreateMockComputerPlayer(gameBoardManager.Data, secondSettlementOneLocation, secondSettlementTwoLocation, secondRoadOne, secondRoadTwo);
      var thirdComputerPlayer = this.CreateMockComputerPlayer(gameBoardManager.Data, thirdSettlementOneLocation, thirdSettlementTwoLocation, thirdRoadOne, thirdRoadTwo);

      GameBoardUpdate gameBoardUpdate = null;
      var localGameController = this.CreateLocalGameController(mockDice, gameBoardManager, firstComputerPlayer, secondComputerPlayer, thirdComputerPlayer);
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
      gameBoardUpdate.NewSettlements.ShouldContainKeyAndValue(firstSettlementOneLocation, firstComputerPlayer.Id);
      gameBoardUpdate.NewSettlements.ShouldContainKeyAndValue(firstSettlementTwoLocation, firstComputerPlayer.Id);
      gameBoardUpdate.NewSettlements.ShouldContainKeyAndValue(secondSettlementOneLocation, secondComputerPlayer.Id);
      gameBoardUpdate.NewSettlements.ShouldContainKeyAndValue(secondSettlementTwoLocation, secondComputerPlayer.Id);
      gameBoardUpdate.NewSettlements.ShouldContainKeyAndValue(thirdSettlementOneLocation, thirdComputerPlayer.Id);
      gameBoardUpdate.NewSettlements.ShouldContainKeyAndValue(thirdSettlementTwoLocation, thirdComputerPlayer.Id);

      gameBoardUpdate.NewRoads.Count.ShouldBe(6);
      gameBoardUpdate.NewRoads.ShouldContainKeyAndValue(firstRoadOne, firstComputerPlayer.Id);
      gameBoardUpdate.NewRoads.ShouldContainKeyAndValue(firstRoadTwo, firstComputerPlayer.Id);
      gameBoardUpdate.NewRoads.ShouldContainKeyAndValue(secondRoadOne, secondComputerPlayer.Id);
      gameBoardUpdate.NewRoads.ShouldContainKeyAndValue(secondRoadTwo, secondComputerPlayer.Id);
      gameBoardUpdate.NewRoads.ShouldContainKeyAndValue(thirdRoadOne, thirdComputerPlayer.Id);
      gameBoardUpdate.NewRoads.ShouldContainKeyAndValue(thirdRoadTwo, thirdComputerPlayer.Id);

      gameBoardUpdate = new GameBoardUpdate(); // Ensure that there is a state change for the gameBoardUpdate variable 

      chosenRoad = new Road(2u, 3u);
      localGameController.CompleteGameSetup(2u, chosenRoad);
      gameBoardUpdate.ShouldBeNull();
    }

    [Test]
    [Category("LocalGameController")]
    public void CompleteSetupWithPlayerInSecondSlot()
    {
      var mockDice = Substitute.For<IDice>();
      mockDice.RollTwoDice().Returns(10u, 12u, 8u, 6u);

      var gameBoardManager = new GameBoardManager(BoardSizes.Standard);

      var firstComputerPlayer = this.CreateMockComputerPlayer(gameBoardManager.Data, firstSettlementOneLocation, firstSettlementTwoLocation, firstRoadOne, firstRoadTwo);
      var secondComputerPlayer = this.CreateMockComputerPlayer(gameBoardManager.Data, secondSettlementOneLocation, secondSettlementTwoLocation, secondRoadOne, secondRoadTwo);
      var thirdComputerPlayer = this.CreateMockComputerPlayer(gameBoardManager.Data, thirdSettlementOneLocation, thirdSettlementTwoLocation, thirdRoadOne, thirdRoadTwo);

      GameBoardUpdate gameBoardUpdate = null;
      var localGameController = this.CreateLocalGameController(mockDice, gameBoardManager, firstComputerPlayer, secondComputerPlayer, thirdComputerPlayer);
      localGameController.GameSetupUpdateEvent = (GameBoardUpdate u) => { gameBoardUpdate = u; };
      localGameController.InitialBoardSetupEvent = (GameBoardData d) => { };

      localGameController.TryJoiningGame();
      localGameController.TryLaunchGame();

      localGameController.StartGameSetup();
      gameBoardUpdate.ShouldNotBeNull();
      gameBoardUpdate.NewSettlements.Count.ShouldBe(1);
      gameBoardUpdate.NewSettlements.ShouldContainKeyAndValue(firstSettlementOneLocation, firstComputerPlayer.Id);
      gameBoardUpdate.NewRoads.Count.ShouldBe(1);
      gameBoardUpdate.NewRoads.ShouldContainKeyAndValue(firstRoadOne, firstComputerPlayer.Id);

      gameBoardUpdate = null; // Ensure that there is a state change for the gameBoardUpdate variable 
      localGameController.ContinueGameSetup(0u, new Road(0u, 1u));

      gameBoardUpdate.ShouldNotBeNull();
      gameBoardUpdate.NewSettlements.Count.ShouldBe(4);
      gameBoardUpdate.NewSettlements.ShouldContainKeyAndValue(secondSettlementOneLocation, secondComputerPlayer.Id);
      gameBoardUpdate.NewSettlements.ShouldContainKeyAndValue(secondSettlementTwoLocation, secondComputerPlayer.Id);
      gameBoardUpdate.NewSettlements.ShouldContainKeyAndValue(thirdSettlementOneLocation, thirdComputerPlayer.Id);
      gameBoardUpdate.NewSettlements.ShouldContainKeyAndValue(thirdSettlementTwoLocation, thirdComputerPlayer.Id);

      gameBoardUpdate.NewRoads.Count.ShouldBe(4);
      gameBoardUpdate.NewRoads.ShouldContainKeyAndValue(secondRoadOne, secondComputerPlayer.Id);
      gameBoardUpdate.NewRoads.ShouldContainKeyAndValue(secondRoadTwo, secondComputerPlayer.Id);
      gameBoardUpdate.NewRoads.ShouldContainKeyAndValue(thirdRoadOne, thirdComputerPlayer.Id);
      gameBoardUpdate.NewRoads.ShouldContainKeyAndValue(thirdRoadTwo, thirdComputerPlayer.Id);

      gameBoardUpdate = null; // Ensure that there is a state change for the gameBoardUpdate variable 
      localGameController.CompleteGameSetup(2u, new Road(2u, 3u));

      gameBoardUpdate.NewSettlements.Count.ShouldBe(1);
      gameBoardUpdate.NewSettlements.ShouldContainKeyAndValue(firstSettlementTwoLocation, firstComputerPlayer.Id);
      gameBoardUpdate.NewRoads.Count.ShouldBe(1);
      gameBoardUpdate.NewRoads.ShouldContainKeyAndValue(firstRoadTwo, firstComputerPlayer.Id);
    }

    [Test]
    [Category("LocalGameController")]
    public void CompleteSetupWithPlayerInThirdSlot()
    {
      var mockDice = Substitute.For<IDice>();
      mockDice.RollTwoDice().Returns(8u, 12u, 10u, 6u);

      var gameBoardManager = new GameBoardManager(BoardSizes.Standard);

      var firstComputerPlayer = this.CreateMockComputerPlayer(gameBoardManager.Data, firstSettlementOneLocation, firstSettlementTwoLocation, firstRoadOne, firstRoadTwo);
      var secondComputerPlayer = this.CreateMockComputerPlayer(gameBoardManager.Data, secondSettlementOneLocation, secondSettlementTwoLocation, secondRoadOne, secondRoadTwo);
      var thirdComputerPlayer = this.CreateMockComputerPlayer(gameBoardManager.Data, thirdSettlementOneLocation, thirdSettlementTwoLocation, thirdRoadOne, thirdRoadTwo);

      GameBoardUpdate gameBoardUpdate = null;
      var localGameController = this.CreateLocalGameController(mockDice, gameBoardManager, firstComputerPlayer, secondComputerPlayer, thirdComputerPlayer);
      localGameController.GameSetupUpdateEvent = (GameBoardUpdate u) => { gameBoardUpdate = u; };
      localGameController.InitialBoardSetupEvent = (GameBoardData d) => { };

      localGameController.TryJoiningGame();
      localGameController.TryLaunchGame();

      localGameController.StartGameSetup();
      gameBoardUpdate.ShouldNotBeNull();
      gameBoardUpdate.NewSettlements.Count.ShouldBe(2);
      gameBoardUpdate.NewSettlements.ShouldContainKeyAndValue(firstSettlementOneLocation, firstComputerPlayer.Id);
      gameBoardUpdate.NewSettlements.ShouldContainKeyAndValue(secondSettlementOneLocation, secondComputerPlayer.Id);
      gameBoardUpdate.NewRoads.Count.ShouldBe(2);
      gameBoardUpdate.NewRoads.ShouldContainKeyAndValue(firstRoadOne, firstComputerPlayer.Id);
      gameBoardUpdate.NewRoads.ShouldContainKeyAndValue(secondRoadOne, secondComputerPlayer.Id);

      gameBoardUpdate = null; // Ensure that there is a state change for the gameBoardUpdate variable 
      localGameController.ContinueGameSetup(0u, new Road(0u, 1u));

      gameBoardUpdate.ShouldNotBeNull();
      gameBoardUpdate.NewSettlements.Count.ShouldBe(2);
      gameBoardUpdate.NewSettlements.ShouldContainKeyAndValue(thirdSettlementOneLocation, thirdComputerPlayer.Id);
      gameBoardUpdate.NewSettlements.ShouldContainKeyAndValue(thirdSettlementTwoLocation, thirdComputerPlayer.Id);

      gameBoardUpdate.NewRoads.Count.ShouldBe(2);
      gameBoardUpdate.NewRoads.ShouldContainKeyAndValue(thirdRoadOne, thirdComputerPlayer.Id);
      gameBoardUpdate.NewRoads.ShouldContainKeyAndValue(thirdRoadTwo, thirdComputerPlayer.Id);

      gameBoardUpdate = null; // Ensure that there is a state change for the gameBoardUpdate variable 
      localGameController.CompleteGameSetup(2u, new Road(2u, 3u));

      gameBoardUpdate.NewSettlements.Count.ShouldBe(2);
      gameBoardUpdate.NewSettlements.ShouldContainKeyAndValue(secondSettlementTwoLocation, secondComputerPlayer.Id);
      gameBoardUpdate.NewSettlements.ShouldContainKeyAndValue(firstSettlementTwoLocation, firstComputerPlayer.Id);
      gameBoardUpdate.NewRoads.Count.ShouldBe(2);
      gameBoardUpdate.NewRoads.ShouldContainKeyAndValue(secondRoadTwo, secondComputerPlayer.Id);
      gameBoardUpdate.NewRoads.ShouldContainKeyAndValue(firstRoadTwo, firstComputerPlayer.Id);
    }

    [Test]
    [Category("LocalGameController")]
    public void CompleteSetupWithPlayerInFourthSlot()
    {
      var mockDice = Substitute.For<IDice>();
      mockDice.RollTwoDice().Returns(6u, 12u, 10u, 8u);

      var gameBoardManager = new GameBoardManager(BoardSizes.Standard);

      var firstComputerPlayer = this.CreateMockComputerPlayer(gameBoardManager.Data, firstSettlementOneLocation, firstSettlementTwoLocation, firstRoadOne, firstRoadTwo);
      var secondComputerPlayer = this.CreateMockComputerPlayer(gameBoardManager.Data, secondSettlementOneLocation, secondSettlementTwoLocation, secondRoadOne, secondRoadTwo);
      var thirdComputerPlayer = this.CreateMockComputerPlayer(gameBoardManager.Data, thirdSettlementOneLocation, thirdSettlementTwoLocation, thirdRoadOne, thirdRoadTwo);

      GameBoardUpdate gameBoardUpdate = null;
      var localGameController = this.CreateLocalGameController(mockDice, gameBoardManager, firstComputerPlayer, secondComputerPlayer, thirdComputerPlayer);
      localGameController.GameSetupUpdateEvent = (GameBoardUpdate u) => { gameBoardUpdate = u; };
      localGameController.InitialBoardSetupEvent = (GameBoardData d) => { };

      localGameController.TryJoiningGame();
      localGameController.TryLaunchGame();

      localGameController.StartGameSetup();
      gameBoardUpdate.ShouldNotBeNull();
      gameBoardUpdate.NewSettlements.Count.ShouldBe(3);
      gameBoardUpdate.NewSettlements.ShouldContainKeyAndValue(firstSettlementOneLocation, firstComputerPlayer.Id);
      gameBoardUpdate.NewSettlements.ShouldContainKeyAndValue(secondSettlementOneLocation, secondComputerPlayer.Id);
      gameBoardUpdate.NewSettlements.ShouldContainKeyAndValue(thirdSettlementOneLocation, thirdComputerPlayer.Id);
      gameBoardUpdate.NewRoads.Count.ShouldBe(3);
      gameBoardUpdate.NewRoads.ShouldContainKeyAndValue(firstRoadOne, firstComputerPlayer.Id);
      gameBoardUpdate.NewRoads.ShouldContainKeyAndValue(secondRoadOne, secondComputerPlayer.Id);
      gameBoardUpdate.NewRoads.ShouldContainKeyAndValue(thirdRoadOne, thirdComputerPlayer.Id);

      gameBoardUpdate = new GameBoardUpdate(); // Ensure that there is a state change for the gameBoardUpdate variable 
      localGameController.ContinueGameSetup(0u, new Road(0u, 1u));
      gameBoardUpdate.ShouldBeNull();

      gameBoardUpdate = null; // Ensure that there is a state change for the gameBoardUpdate variable 
      localGameController.CompleteGameSetup(2u, new Road(2u, 3u));

      gameBoardUpdate.ShouldNotBeNull();
      gameBoardUpdate.NewSettlements.Count.ShouldBe(3);
      gameBoardUpdate.NewSettlements.ShouldContainKeyAndValue(thirdSettlementTwoLocation, thirdComputerPlayer.Id);
      gameBoardUpdate.NewSettlements.ShouldContainKeyAndValue(secondSettlementTwoLocation, secondComputerPlayer.Id);
      gameBoardUpdate.NewSettlements.ShouldContainKeyAndValue(firstSettlementTwoLocation, firstComputerPlayer.Id);
      gameBoardUpdate.NewRoads.Count.ShouldBe(3);
      gameBoardUpdate.NewRoads.ShouldContainKeyAndValue(thirdRoadTwo, thirdComputerPlayer.Id);
      gameBoardUpdate.NewRoads.ShouldContainKeyAndValue(secondRoadTwo, secondComputerPlayer.Id);
      gameBoardUpdate.NewRoads.ShouldContainKeyAndValue(firstRoadTwo, firstComputerPlayer.Id);
    }

    [Test]
    [Category("LocalGameController")]
    public void ContinueGameSetup_CallOutOfSequence_MeaningfulErrorDetailsPassedBack()
    {
      var localGameController = this.CreateLocalGameController();
      ErrorDetails errorDetails = null;
      localGameController.ExceptionRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

      localGameController.ContinueGameSetup(0u, new Road(0u, 1u));

      errorDetails.ShouldNotBeNull();
      errorDetails.Message.ShouldBe("Cannot call 'ContinueGameSetup' until 'StartGameSetup' has completed.");
    }

    [Test]
    [Category("LocalGameController")]
    public void CompleteGameSetup_CallOutOfSequence_MeaningfulErrorDetailsPassedBack()
    {
      var localGameController = this.CreateLocalGameController();
      ErrorDetails errorDetails = null;
      localGameController.ExceptionRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

      localGameController.CompleteGameSetup(0u, new Road(0u, 1u));

      errorDetails.ShouldNotBeNull();
      errorDetails.Message.ShouldBe("Cannot call 'CompleteGameSetup' until 'ContinueGameSetup' has completed.");
    }

    [Test]
    [Category("LocalGameController")]
    public void PlayerPlacesRoadWithNoConnectionToAnySettlements_MeaningfulErrorDetailsPassedBack()
    {
      var mockDice = Substitute.For<IDice>();
      mockDice.RollTwoDice().Returns(12u, 10u, 8u, 6u);

      var gameBoardManager = new GameBoardManager(BoardSizes.Standard);

      var firstComputerPlayer = this.CreateMockComputerPlayer(gameBoardManager.Data, firstSettlementOneLocation, firstSettlementTwoLocation, firstRoadOne, firstRoadTwo);
      var secondComputerPlayer = this.CreateMockComputerPlayer(gameBoardManager.Data, secondSettlementOneLocation, secondSettlementTwoLocation, secondRoadOne, secondRoadTwo);
      var thirdComputerPlayer = this.CreateMockComputerPlayer(gameBoardManager.Data, thirdSettlementOneLocation, thirdSettlementTwoLocation, thirdRoadOne, thirdRoadTwo);

      ErrorDetails errorDetails = null;
      GameBoardUpdate gameBoardUpdate = null;
      var localGameController = this.CreateLocalGameController(mockDice, gameBoardManager, firstComputerPlayer, secondComputerPlayer, thirdComputerPlayer);
      localGameController.ExceptionRaisedEvent = (ErrorDetails e) => { errorDetails = e; };
      localGameController.GameSetupUpdateEvent = (GameBoardUpdate u) => { gameBoardUpdate = u; };

      localGameController.TryJoiningGame();
      localGameController.TryLaunchGame();
      localGameController.StartGameSetup();
      localGameController.ContinueGameSetup(0u, new Road(1u, 2u));

      errorDetails.ShouldNotBeNull();
      errorDetails.Message.ShouldBe("Cannot place road at [1, 2]. Not connected to a player owned road or settlement.");
      gameBoardUpdate.ShouldBeNull();
    }

    [Test]
    [Category("LocalGameController")]
    public void PlayerSelectsSameLocationAsComputerplayerDuringFirstSetupRound_MeaningfulErrorDetailsPassedBack()
    {
      var mockDice = Substitute.For<IDice>();
      mockDice.RollTwoDice().Returns(10u, 12u, 8u, 6u);

      var gameBoardManager = new GameBoardManager(BoardSizes.Standard);

      var firstComputerPlayer = this.CreateMockComputerPlayer(gameBoardManager.Data, firstSettlementOneLocation, firstSettlementTwoLocation, firstRoadOne, firstRoadTwo);
      var secondComputerPlayer = this.CreateMockComputerPlayer(gameBoardManager.Data, secondSettlementOneLocation, secondSettlementTwoLocation, secondRoadOne, secondRoadTwo);
      var thirdComputerPlayer = this.CreateMockComputerPlayer(gameBoardManager.Data, thirdSettlementOneLocation, thirdSettlementTwoLocation, thirdRoadOne, thirdRoadTwo);

      ErrorDetails exception = null;
      var localGameController = this.CreateLocalGameController(mockDice, gameBoardManager, firstComputerPlayer, secondComputerPlayer, thirdComputerPlayer);
      localGameController.ExceptionRaisedEvent = (ErrorDetails e) => { exception = e; };

      localGameController.TryJoiningGame();
      localGameController.TryLaunchGame();
      localGameController.StartGameSetup();

      GameBoardUpdate gameBoardUpdate = null;
      localGameController.GameSetupUpdateEvent = (GameBoardUpdate u) => { gameBoardUpdate = u; };

      localGameController.ContinueGameSetup(firstSettlementOneLocation, new Road(0u, 1u));
      exception.ShouldNotBeNull();
      exception.Message.ShouldBe("Cannot place settlement: Location " + firstSettlementOneLocation + " already owned by player " + firstComputerPlayer.Id);
      gameBoardUpdate.ShouldBeNull();
    }

    [Test]
    [Category("LocalGameController")]
    public void PlayerSelectsSameLocationAsComputerplayerDuringSecondSetupRound_MeaningfulErrorDetailsPassedBack()
    {
      var mockDice = Substitute.For<IDice>();
      mockDice.RollTwoDice().Returns(12u, 10u, 8u, 6u);

      var gameBoardManager = new GameBoardManager(BoardSizes.Standard);

      var firstComputerPlayer = this.CreateMockComputerPlayer(gameBoardManager.Data, firstSettlementOneLocation, firstSettlementTwoLocation, firstRoadOne, firstRoadTwo);
      var secondComputerPlayer = this.CreateMockComputerPlayer(gameBoardManager.Data, secondSettlementOneLocation, secondSettlementTwoLocation, secondRoadOne, secondRoadTwo);
      var thirdComputerPlayer = this.CreateMockComputerPlayer(gameBoardManager.Data, thirdSettlementOneLocation, thirdSettlementTwoLocation, thirdRoadOne, thirdRoadTwo);

      ErrorDetails exception = null;
      var localGameController = this.CreateLocalGameController(mockDice, gameBoardManager, firstComputerPlayer, secondComputerPlayer, thirdComputerPlayer);
      localGameController.ExceptionRaisedEvent = (ErrorDetails e) => { exception = e; };

      localGameController.TryJoiningGame();
      localGameController.TryLaunchGame();
      localGameController.StartGameSetup();
      localGameController.ContinueGameSetup(0, new Road(0, 1));

      GameBoardUpdate gameBoardUpdate = null;
      localGameController.GameSetupUpdateEvent = (GameBoardUpdate u) => { gameBoardUpdate = u; };

      localGameController.CompleteGameSetup(firstSettlementOneLocation, new Road(0, 1));

      exception.ShouldNotBeNull();
      exception.Message.ShouldBe("Cannot place settlement: Location " + firstSettlementOneLocation + " already owned by player " + firstComputerPlayer.Id);
      gameBoardUpdate.ShouldBeNull();
    }

    [Test]
    [Category("LocalGameController")]
    public void PlayerSelectsLocationTooCloseToComputerplayerDuringFirstSetupRound_MeaningfulErrorDetailsPassedBack()
    {
      var mockDice = Substitute.For<IDice>();
      mockDice.RollTwoDice().Returns(10u, 12u, 8u, 6u);

      var gameBoardManager = new GameBoardManager(BoardSizes.Standard);

      var firstComputerPlayer = this.CreateMockComputerPlayer(gameBoardManager.Data, firstSettlementOneLocation, firstSettlementTwoLocation, firstRoadOne, firstRoadTwo);
      var secondComputerPlayer = this.CreateMockComputerPlayer(gameBoardManager.Data, secondSettlementOneLocation, secondSettlementTwoLocation, secondRoadOne, secondRoadTwo);
      var thirdComputerPlayer = this.CreateMockComputerPlayer(gameBoardManager.Data, thirdSettlementOneLocation, thirdSettlementTwoLocation, thirdRoadOne, thirdRoadTwo);

      ErrorDetails exception = null;
      var localGameController = this.CreateLocalGameController(mockDice, gameBoardManager, firstComputerPlayer, secondComputerPlayer, thirdComputerPlayer);
      localGameController.ExceptionRaisedEvent = (ErrorDetails e) => { exception = e; };
      localGameController.TryJoiningGame();
      localGameController.TryLaunchGame();
      localGameController.StartGameSetup();

      GameBoardUpdate gameBoardUpdate = null;
      localGameController.GameSetupUpdateEvent = (GameBoardUpdate u) => { gameBoardUpdate = u; };

      localGameController.ContinueGameSetup(19u, new Road(0u, 1u));

      exception.ShouldNotBeNull();
      exception.Message.ShouldBe("Cannot place settlement: Too close to player " + firstComputerPlayer.Id + " at location " + firstSettlementOneLocation);
      gameBoardUpdate.ShouldBeNull();
    }

    [Test]
    [Category("LocalGameController")]
    public void PlayerSelectsLocationTooCloseToComputerplayerDuringSecondSetupRound_MeaningfulErrorDetailsPassedBack()
    {
      var mockDice = Substitute.For<IDice>();
      mockDice.RollTwoDice().Returns(12u, 10u, 8u, 6u);

      var gameBoardManager = new GameBoardManager(BoardSizes.Standard);

      var firstComputerPlayer = this.CreateMockComputerPlayer(gameBoardManager.Data, firstSettlementOneLocation, firstSettlementTwoLocation, firstRoadOne, firstRoadTwo);
      var secondComputerPlayer = this.CreateMockComputerPlayer(gameBoardManager.Data, secondSettlementOneLocation, secondSettlementTwoLocation, secondRoadOne, secondRoadTwo);
      var thirdComputerPlayer = this.CreateMockComputerPlayer(gameBoardManager.Data, thirdSettlementOneLocation, thirdSettlementTwoLocation, thirdRoadOne, thirdRoadTwo);

      ErrorDetails exception = null;
      var localGameController = this.CreateLocalGameController(mockDice, gameBoardManager, firstComputerPlayer, secondComputerPlayer, thirdComputerPlayer);
      localGameController.ExceptionRaisedEvent = (ErrorDetails e) => { exception = e; };
      localGameController.TryJoiningGame();
      localGameController.TryLaunchGame();
      localGameController.StartGameSetup();
      localGameController.ContinueGameSetup(0, new Road(0, 1));

      GameBoardUpdate gameBoardUpdate = null;
      localGameController.GameSetupUpdateEvent = (GameBoardUpdate u) => { gameBoardUpdate = u; };

      localGameController.CompleteGameSetup(19, new Road(19, 18));

      exception.ShouldNotBeNull();
      exception.Message.ShouldBe("Cannot place settlement: Too close to player " + firstComputerPlayer.Id + " at location " + firstSettlementOneLocation);
      gameBoardUpdate.ShouldBeNull();
    }

    [Test]
    [Category("LocalGameController")]
    public void PlayerPlacesRoadWithNoConnectionToAnySettlementsDuringSecondSetupRound_MeaningfulErrorDetailsPassedBack()
    {
      var localGameController = CreateLocalGameControllerWithMainPlayerGoingFirstInSetup();

      ErrorDetails errorDetails = null;
      localGameController.ExceptionRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

      localGameController.TryJoiningGame();
      localGameController.TryLaunchGame();
      localGameController.StartGameSetup();
      localGameController.ContinueGameSetup(0, new Road(0, 1));

      GameBoardUpdate gameBoardUpdate = null;
      localGameController.GameSetupUpdateEvent = (GameBoardUpdate u) => { gameBoardUpdate = u; };

      localGameController.CompleteGameSetup(6, new Road(4, 5));

      errorDetails.ShouldNotBeNull();
      errorDetails.Message.ShouldBe("Cannot place road at [4, 5]. No connection to a player owned road or settlement.");
      gameBoardUpdate.ShouldBeNull();
    }

    [Test]
    [Category("LocalGameController")]
    public void PlayerPlacesSettlementOffGameBoardDuringFirstSetupRound_MeaningfulErrorDetailsPassedBack()
    {
      var localGameController = CreateLocalGameControllerWithMainPlayerGoingFirstInSetup();

      ErrorDetails errorDetails = null;
      localGameController.ExceptionRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

      GameBoardUpdate gameBoardUpdate = null;
      localGameController.GameSetupUpdateEvent = (GameBoardUpdate u) => { gameBoardUpdate = u; };

      localGameController.TryJoiningGame();
      localGameController.TryLaunchGame();
      localGameController.StartGameSetup();
      localGameController.ContinueGameSetup(100, new Road(100, 101));

      errorDetails.ShouldNotBeNull();
      errorDetails.Message.ShouldBe("Cannot place settlement at [100]. This is outside of board range (0 - 53).");
      gameBoardUpdate.ShouldBeNull();
    }

    [Test]
    [Category("LocalGameController")]
    public void PlayerPlacesRoadOverEdgeOfGameBoardDuringFirstSetupRound_MeaningfulErrorDetailsPassedBack()
    {
      var localGameController = CreateLocalGameControllerWithMainPlayerGoingFirstInSetup();

      ErrorDetails errorDetails = null;
      localGameController.ExceptionRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

      GameBoardUpdate gameBoardUpdate = null;
      localGameController.GameSetupUpdateEvent = (GameBoardUpdate u) => { gameBoardUpdate = u; };

      localGameController.TryJoiningGame();
      localGameController.TryLaunchGame();
      localGameController.StartGameSetup();
      localGameController.ContinueGameSetup(53, new Road(53, 54));

      errorDetails.ShouldNotBeNull();
      errorDetails.Message.ShouldBe("Cannot place road at [53, 54]. This is outside of board range (0 - 53).");
      gameBoardUpdate.ShouldBeNull();
    }

    [Test]
    [Category("LocalGameController")]
    public void PlayerPlacesSettlementOffGameBoardDuringSecondSetupRound_MeaningfulErrorDetailsPassedBack()
    {
      var localGameController = CreateLocalGameControllerWithMainPlayerGoingFirstInSetup();

      ErrorDetails errorDetails = null;
      localGameController.ExceptionRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

      localGameController.TryJoiningGame();
      localGameController.TryLaunchGame();
      localGameController.StartGameSetup();
      localGameController.ContinueGameSetup(0, new Road(0, 1));

      GameBoardUpdate gameBoardUpdate = null;
      localGameController.GameSetupUpdateEvent = (GameBoardUpdate u) => { gameBoardUpdate = u; };

      localGameController.CompleteGameSetup(100, new Road(100, 101));

      errorDetails.ShouldNotBeNull();
      errorDetails.Message.ShouldBe("Cannot place settlement at [100]. This is outside of board range (0 - 53).");
      gameBoardUpdate.ShouldBeNull();
    }

    [Test]
    [Category("LocalGameController")]
    public void PlayerPlacesRoadOverEdgeOfGameBoardDuringSecondSetupRound_MeaningfulErrorDetailsPassedBack()
    {
      var localGameController = CreateLocalGameControllerWithMainPlayerGoingFirstInSetup();

      ErrorDetails errorDetails = null;
      localGameController.ExceptionRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

      localGameController.TryJoiningGame();
      localGameController.TryLaunchGame();
      localGameController.StartGameSetup();
      localGameController.ContinueGameSetup(0, new Road(0, 1));

      GameBoardUpdate gameBoardUpdate = null;
      localGameController.GameSetupUpdateEvent = (GameBoardUpdate u) => { gameBoardUpdate = u; };

      localGameController.CompleteGameSetup(53, new Road(53, 54));

      errorDetails.ShouldNotBeNull();
      errorDetails.Message.ShouldBe("Cannot place road at [53, 54]. This is outside of board range (0 - 53).");
      gameBoardUpdate.ShouldBeNull();
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

    private LocalGameController CreateLocalGameController(IDice dice, GameBoardManager gameBoardManager, IComputerPlayer firstComputerPlayer, params IComputerPlayer[] otherComputerPlayers)
    {
      var mockComputerPlayerFactory = Substitute.For<IComputerPlayerFactory>();
      mockComputerPlayerFactory.Create().Returns(firstComputerPlayer, otherComputerPlayers);

      var localGameController = new LocalGameController(dice, mockComputerPlayerFactory, gameBoardManager);
      return localGameController;
    }

    private LocalGameController CreateLocalGameControllerWithMainPlayerGoingFirstInSetup()
    {
      var mockDice = Substitute.For<IDice>();
      mockDice.RollTwoDice().Returns(12u, 10u, 8u, 6u);

      var gameBoardManager = new GameBoardManager(BoardSizes.Standard);

      var firstComputerPlayer = this.CreateMockComputerPlayer(gameBoardManager.Data, firstSettlementOneLocation, firstSettlementTwoLocation, firstRoadOne, firstRoadTwo);
      var secondComputerPlayer = this.CreateMockComputerPlayer(gameBoardManager.Data, secondSettlementOneLocation, secondSettlementTwoLocation, secondRoadOne, secondRoadTwo);
      var thirdComputerPlayer = this.CreateMockComputerPlayer(gameBoardManager.Data, thirdSettlementOneLocation, thirdSettlementTwoLocation, thirdRoadOne, thirdRoadTwo);

      return this.CreateLocalGameController(mockDice, gameBoardManager, firstComputerPlayer, secondComputerPlayer, thirdComputerPlayer);
    }

    private IComputerPlayer CreateMockComputerPlayer(GameBoardData gameBoardData, UInt32 settlementOneLocation, UInt32 settlementTwoLocation, Road roadOne, Road roadTwo)
    {
      var mockComputerPlayer = Substitute.For<IComputerPlayer>();
      var playerId = Guid.NewGuid();
      mockComputerPlayer.Id.Returns(playerId);
      mockComputerPlayer.ChooseSettlementLocation(gameBoardData)
        .Returns(settlementOneLocation, settlementTwoLocation);
      mockComputerPlayer.ChooseRoad(gameBoardData)
        .Returns(roadOne, roadTwo);

      return mockComputerPlayer;
    }
    #endregion 
  }
}
