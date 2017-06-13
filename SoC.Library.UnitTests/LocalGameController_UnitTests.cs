
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
      localGameController.TryJoiningGame(null);
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
      localGameController.TryJoiningGame(null);
      var result = localGameController.TryJoiningGame(null);

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

      localGameController.TryJoiningGame(null);
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

      localGameController.TryJoiningGame(null);
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
      var initialSetupRoundTrail = new Road(0u, 1u); //      gameBoardManager.Data.Trails[10];

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

      localGameController.TryJoiningGame(null);
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
      var initialSetupRoundTrail = new Road(0u, 1u); // gameBoardManager.Data.Trails[10];
      var secondRoadOne = new Road(0u, 1u); // gameBoardManager.Data.Trails[20]

      var firstMockPlayerId = Guid.NewGuid();
      var firstMockComputerPlayer = Substitute.For<IComputerPlayer>();
      firstMockComputerPlayer.Id.Returns(firstMockPlayerId);
      firstMockComputerPlayer.ChooseSettlementLocation(gameBoardManager.Data).Returns(initialSetupSettlementLocation);
      firstMockComputerPlayer.ChooseRoad(gameBoardManager.Data).Returns(initialSetupRoundTrail);

      var secondMockPlayerId = Guid.NewGuid();
      var secondMockComputerPlayer = Substitute.For<IComputerPlayer>();
      secondMockComputerPlayer.Id.Returns(secondMockPlayerId);
      secondMockComputerPlayer.ChooseSettlementLocation(gameBoardManager.Data).Returns(24u);
      secondMockComputerPlayer.ChooseRoad(gameBoardManager.Data).Returns(secondRoadOne);

      var mockComputerPlayerFactory = Substitute.For<IComputerPlayerFactory>();
      mockComputerPlayerFactory.Create().Returns(firstMockComputerPlayer, secondMockComputerPlayer);

      var localGameController = this.CreateLocalGameController(mockDice, mockComputerPlayerFactory, gameBoardManager);

      Guid firstPlayerId = Guid.Empty;
      GameBoardUpdate gameBoardUpdate = null;
      localGameController.StartInitialSetupTurnEvent = (GameBoardUpdate u) => { gameBoardUpdate = u; };

      localGameController.TryJoiningGame(null);
      localGameController.TryLaunchGame();

      gameBoardUpdate.ShouldNotBeNull();
      gameBoardUpdate.NewSettlements.Count.ShouldBe(2);
      gameBoardUpdate.NewSettlements.ContainsValue(firstMockPlayerId);
      gameBoardUpdate.NewSettlements.ContainsValue(secondMockPlayerId);
      gameBoardUpdate.NewRoads.Count.ShouldBe(2);
      gameBoardUpdate.NewRoads.ContainsValue(firstMockPlayerId);
      gameBoardUpdate.NewRoads.ContainsValue(secondMockPlayerId);
    }

    [Test]
    public void CompleteSetupWithPlayerInFirstSlot()
    {
      var mockDice = Substitute.For<IDice>();
      mockDice.RollTwoDice().Returns(12u, 10u, 8u, 6u);

      var gameBoardManager = new GameBoardManager(BoardSizes.Standard);
      var firstSettlementOneLocation = 18u;
      var secondSettlementOneLocation = 25u;
      var thirdSettlementOneLocation = 31u;

      var thirdSettlementTwoLocation = 33u;
      var secondSettlementTwoLocation = 35u;
      var firstSettlementTwoLocation = 43u;

      var firstRoadOne = new Road(17u, 18u);
      var secondRoadOne = new Road(15u, 25u);
      var thirdRoadOne = new Road(30u, 31u);

      var thirdRoadTwo = new Road(32u, 33u);
      var secondRoadTwo = new Road(24u, 35u);
      var firstRoadTwo = new Road(43u, 44u);

      var firstMockPlayerId = Guid.NewGuid();
      var firstMockComputerPlayer = Substitute.For<IComputerPlayer>();
      firstMockComputerPlayer.Id.Returns(firstMockPlayerId);
      firstMockComputerPlayer.ChooseSettlementLocation(gameBoardManager.Data)
        .Returns(firstSettlementOneLocation, firstSettlementTwoLocation);
      firstMockComputerPlayer.ChooseRoad(gameBoardManager.Data).Returns(firstRoadOne, firstRoadTwo);

      var secondMockPlayerId = Guid.NewGuid();
      var secondMockComputerPlayer = Substitute.For<IComputerPlayer>();
      secondMockComputerPlayer.Id.Returns(secondMockPlayerId);
      secondMockComputerPlayer.ChooseSettlementLocation(gameBoardManager.Data)
        .Returns(secondSettlementOneLocation, secondSettlementTwoLocation);
      secondMockComputerPlayer.ChooseRoad(gameBoardManager.Data).Returns(secondRoadOne, secondRoadTwo);

      var thirdMockPlayerId = Guid.NewGuid();
      var thirdMockComputerPlayer = Substitute.For<IComputerPlayer>();
      thirdMockComputerPlayer.Id.Returns(thirdMockPlayerId);
      thirdMockComputerPlayer.ChooseSettlementLocation(gameBoardManager.Data)
        .Returns(thirdSettlementOneLocation, thirdSettlementTwoLocation);
      thirdMockComputerPlayer.ChooseRoad(gameBoardManager.Data).Returns(thirdRoadOne, thirdRoadTwo);

      var mockComputerPlayerFactory = Substitute.For<IComputerPlayerFactory>();
      mockComputerPlayerFactory.Create().Returns(firstMockComputerPlayer, secondMockComputerPlayer, thirdMockComputerPlayer);

      GameBoardUpdate gameBoardUpdate = null;
      var localGameController = this.CreateLocalGameController(mockDice, mockComputerPlayerFactory, gameBoardManager);
      localGameController.GameSetupUpdateEvent = (GameBoardUpdate u) => { gameBoardUpdate = u; };
      localGameController.InitialBoardSetupEvent = (GameBoardData d) => { };

      localGameController.TryJoiningGame(null);
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

      gameBoardUpdate.NewRoads2.Count.ShouldBe(6);
      gameBoardUpdate.NewRoads2.ShouldContainKeyAndValue(firstRoadOne, firstMockComputerPlayer.Id);
      gameBoardUpdate.NewRoads2.ShouldContainKeyAndValue(firstRoadTwo, firstMockComputerPlayer.Id);
      gameBoardUpdate.NewRoads2.ShouldContainKeyAndValue(secondRoadOne, secondMockComputerPlayer.Id);
      gameBoardUpdate.NewRoads2.ShouldContainKeyAndValue(secondRoadTwo, secondMockComputerPlayer.Id);
      gameBoardUpdate.NewRoads2.ShouldContainKeyAndValue(thirdRoadOne, thirdMockComputerPlayer.Id);
      gameBoardUpdate.NewRoads2.ShouldContainKeyAndValue(thirdRoadTwo, thirdMockComputerPlayer.Id);

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

      var firstSettlementOneLocation = 18u;
      var secondSettlementOneLocation = 25u;
      var thirdSettlementOneLocation = 31u;

      var thirdSettlementTwoLocation = 33u;
      var secondSettlementTwoLocation = 35u;
      var firstSettlementTwoLocation = 43u;

      var firstRoadOne = new Road(17u, 18u);
      var secondRoadOne = new Road(15u, 25u);
      var thirdRoadOne = new Road(30u, 31u);

      var thirdRoadTwo = new Road(32u, 33u);
      var secondRoadTwo = new Road(24u, 35u);
      var firstRoadTwo = new Road(43u, 44u);

      var firstMockPlayerId = Guid.NewGuid();
      var firstMockComputerPlayer = Substitute.For<IComputerPlayer>();
      firstMockComputerPlayer.Id.Returns(firstMockPlayerId);
      firstMockComputerPlayer.ChooseSettlementLocation(gameBoardManager.Data)
        .Returns(firstSettlementOneLocation, firstSettlementTwoLocation);
      firstMockComputerPlayer.ChooseRoad(gameBoardManager.Data).Returns(firstRoadOne, firstRoadTwo);

      var secondMockPlayerId = Guid.NewGuid();
      var secondMockComputerPlayer = Substitute.For<IComputerPlayer>();
      secondMockComputerPlayer.Id.Returns(secondMockPlayerId);
      secondMockComputerPlayer.ChooseSettlementLocation(gameBoardManager.Data)
        .Returns(secondSettlementOneLocation, secondSettlementTwoLocation);
      secondMockComputerPlayer.ChooseRoad(gameBoardManager.Data).Returns(secondRoadOne, secondRoadTwo);

      var thirdMockPlayerId = Guid.NewGuid();
      var thirdMockComputerPlayer = Substitute.For<IComputerPlayer>();
      thirdMockComputerPlayer.Id.Returns(thirdMockPlayerId);
      thirdMockComputerPlayer.ChooseSettlementLocation(gameBoardManager.Data)
        .Returns(thirdSettlementOneLocation, thirdSettlementTwoLocation);
      thirdMockComputerPlayer.ChooseRoad(gameBoardManager.Data).Returns(thirdRoadOne, thirdRoadTwo);

      var mockComputerPlayerFactory = Substitute.For<IComputerPlayerFactory>();
      mockComputerPlayerFactory.Create().Returns(firstMockComputerPlayer, secondMockComputerPlayer, thirdMockComputerPlayer);

      GameBoardUpdate gameBoardUpdate = null;
      var localGameController = this.CreateLocalGameController(mockDice, mockComputerPlayerFactory, gameBoardManager);
      localGameController.GameSetupUpdateEvent = (GameBoardUpdate u) => { gameBoardUpdate = u; };
      localGameController.InitialBoardSetupEvent = (GameBoardData d) => { };

      localGameController.TryJoiningGame(null);
      localGameController.TryLaunchGame();

      localGameController.StartGameSetup();
      gameBoardUpdate.ShouldNotBeNull();
      gameBoardUpdate.NewSettlements.Count.ShouldBe(1);
      gameBoardUpdate.NewSettlements.ShouldContainKeyAndValue(firstSettlementOneLocation, firstMockComputerPlayer.Id);
      gameBoardUpdate.NewRoads2.Count.ShouldBe(1);
      gameBoardUpdate.NewRoads2.ShouldContainKeyAndValue(firstRoadOne, firstMockComputerPlayer.Id);

      gameBoardUpdate = null; // Ensure that there is a state change for the gameBoardUpdate variable 
      localGameController.ContinueGameSetup(0u, new Road(0u, 1u));

      gameBoardUpdate.ShouldNotBeNull();
      gameBoardUpdate.NewSettlements.Count.ShouldBe(4);
      gameBoardUpdate.NewSettlements.ShouldContainKeyAndValue(secondSettlementOneLocation, secondMockComputerPlayer.Id);
      gameBoardUpdate.NewSettlements.ShouldContainKeyAndValue(secondSettlementTwoLocation, secondMockComputerPlayer.Id);
      gameBoardUpdate.NewSettlements.ShouldContainKeyAndValue(thirdSettlementOneLocation, thirdMockComputerPlayer.Id);
      gameBoardUpdate.NewSettlements.ShouldContainKeyAndValue(thirdSettlementTwoLocation, thirdMockComputerPlayer.Id);

      gameBoardUpdate.NewRoads2.Count.ShouldBe(4);
      gameBoardUpdate.NewRoads2.ShouldContainKeyAndValue(secondRoadOne, secondMockComputerPlayer.Id);
      gameBoardUpdate.NewRoads2.ShouldContainKeyAndValue(secondRoadTwo, secondMockComputerPlayer.Id);
      gameBoardUpdate.NewRoads2.ShouldContainKeyAndValue(thirdRoadOne, thirdMockComputerPlayer.Id);
      gameBoardUpdate.NewRoads2.ShouldContainKeyAndValue(thirdRoadTwo, thirdMockComputerPlayer.Id);

      gameBoardUpdate = null; // Ensure that there is a state change for the gameBoardUpdate variable 
      localGameController.CompleteGameSetup(2u, new Road(2u, 3u));

      gameBoardUpdate.NewSettlements.Count.ShouldBe(1);
      gameBoardUpdate.NewSettlements.ShouldContainKeyAndValue(firstSettlementTwoLocation, firstMockComputerPlayer.Id);
      gameBoardUpdate.NewRoads2.Count.ShouldBe(1);
      gameBoardUpdate.NewRoads2.ShouldContainKeyAndValue(firstRoadTwo, firstMockComputerPlayer.Id);
    }

    [Test]
    public void CompleteSetupWithPlayerInThirdSlot()
    {
      var mockDice = Substitute.For<IDice>();
      mockDice.RollTwoDice().Returns(8u, 12u, 10u, 6u);

      var gameBoardManager = new GameBoardManager(BoardSizes.Standard);

      var firstSettlementOneLocation = 18u;
      var secondSettlementOneLocation = 25u;
      var thirdSettlementOneLocation = 31u;

      var thirdSettlementTwoLocation = 33u;
      var secondSettlementTwoLocation = 35u;
      var firstSettlementTwoLocation = 43u;

      var firstRoadOne = new Road(17u, 18u);
      var secondRoadOne = new Road(15u, 25u);
      var thirdRoadOne = new Road(30u, 31u);

      var thirdRoadTwo = new Road(32u, 33u);
      var secondRoadTwo = new Road(24u, 35u);
      var firstRoadTwo = new Road(43u, 44u);

      var firstMockPlayerId = Guid.NewGuid();
      var firstMockComputerPlayer = Substitute.For<IComputerPlayer>();
      firstMockComputerPlayer.Id.Returns(firstMockPlayerId);
      firstMockComputerPlayer.ChooseSettlementLocation(gameBoardManager.Data)
        .Returns(firstSettlementOneLocation, firstSettlementTwoLocation);
      firstMockComputerPlayer.ChooseRoad(gameBoardManager.Data).Returns(firstRoadOne, firstRoadTwo);

      var secondMockPlayerId = Guid.NewGuid();
      var secondMockComputerPlayer = Substitute.For<IComputerPlayer>();
      secondMockComputerPlayer.Id.Returns(secondMockPlayerId);
      secondMockComputerPlayer.ChooseSettlementLocation(gameBoardManager.Data)
        .Returns(secondSettlementOneLocation, secondSettlementTwoLocation);
      secondMockComputerPlayer.ChooseRoad(gameBoardManager.Data).Returns(secondRoadOne, secondRoadTwo);

      var thirdMockPlayerId = Guid.NewGuid();
      var thirdMockComputerPlayer = Substitute.For<IComputerPlayer>();
      thirdMockComputerPlayer.Id.Returns(thirdMockPlayerId);
      thirdMockComputerPlayer.ChooseSettlementLocation(gameBoardManager.Data)
        .Returns(thirdSettlementOneLocation, thirdSettlementTwoLocation);
      thirdMockComputerPlayer.ChooseRoad(gameBoardManager.Data).Returns(thirdRoadOne, thirdRoadTwo);

      var mockComputerPlayerFactory = Substitute.For<IComputerPlayerFactory>();
      mockComputerPlayerFactory.Create().Returns(firstMockComputerPlayer, secondMockComputerPlayer, thirdMockComputerPlayer);

      GameBoardUpdate gameBoardUpdate = null;
      var localGameController = this.CreateLocalGameController(mockDice, mockComputerPlayerFactory, gameBoardManager);
      localGameController.GameSetupUpdateEvent = (GameBoardUpdate u) => { gameBoardUpdate = u; };
      localGameController.InitialBoardSetupEvent = (GameBoardData d) => { };

      localGameController.TryJoiningGame(null);
      localGameController.TryLaunchGame();

      localGameController.StartGameSetup();
      gameBoardUpdate.ShouldNotBeNull();
      gameBoardUpdate.NewSettlements.Count.ShouldBe(2);
      gameBoardUpdate.NewSettlements.ShouldContainKeyAndValue(firstSettlementOneLocation, firstMockComputerPlayer.Id);
      gameBoardUpdate.NewSettlements.ShouldContainKeyAndValue(secondSettlementOneLocation, secondMockComputerPlayer.Id);
      gameBoardUpdate.NewRoads2.Count.ShouldBe(2);
      gameBoardUpdate.NewRoads2.ShouldContainKeyAndValue(firstRoadOne, firstMockComputerPlayer.Id);
      gameBoardUpdate.NewRoads2.ShouldContainKeyAndValue(secondRoadOne, secondMockComputerPlayer.Id);

      gameBoardUpdate = null; // Ensure that there is a state change for the gameBoardUpdate variable 
      localGameController.ContinueGameSetup(0u, new Road(0u, 1u));

      gameBoardUpdate.ShouldNotBeNull();
      gameBoardUpdate.NewSettlements.Count.ShouldBe(2);
      gameBoardUpdate.NewSettlements.ShouldContainKeyAndValue(thirdSettlementOneLocation, thirdMockComputerPlayer.Id);
      gameBoardUpdate.NewSettlements.ShouldContainKeyAndValue(thirdSettlementTwoLocation, thirdMockComputerPlayer.Id);

      gameBoardUpdate.NewRoads2.Count.ShouldBe(2);
      gameBoardUpdate.NewRoads2.ShouldContainKeyAndValue(thirdRoadOne, thirdMockComputerPlayer.Id);
      gameBoardUpdate.NewRoads2.ShouldContainKeyAndValue(thirdRoadTwo, thirdMockComputerPlayer.Id);

      gameBoardUpdate = null; // Ensure that there is a state change for the gameBoardUpdate variable 
      localGameController.CompleteGameSetup(2u, new Road(2u, 3u));

      gameBoardUpdate.NewSettlements.Count.ShouldBe(2);
      gameBoardUpdate.NewSettlements.ShouldContainKeyAndValue(secondSettlementTwoLocation, secondMockComputerPlayer.Id);
      gameBoardUpdate.NewSettlements.ShouldContainKeyAndValue(firstSettlementTwoLocation, firstMockComputerPlayer.Id);
      gameBoardUpdate.NewRoads2.Count.ShouldBe(2);
      gameBoardUpdate.NewRoads2.ShouldContainKeyAndValue(secondRoadTwo, secondMockComputerPlayer.Id);
      gameBoardUpdate.NewRoads2.ShouldContainKeyAndValue(firstRoadTwo, firstMockComputerPlayer.Id);
    }

    [Test]
    public void CompleteSetupWithPlayerInFourthSlot()
    {
      var mockDice = Substitute.For<IDice>();
      mockDice.RollTwoDice().Returns(6u, 12u, 10u, 8u);

      var gameBoardManager = new GameBoardManager(BoardSizes.Standard);

      var firstSettlementOneLocation = 18u;
      var secondSettlementOneLocation = 25u;
      var thirdSettlementOneLocation = 31u;

      var thirdSettlementTwoLocation = 33u;
      var secondSettlementTwoLocation = 35u;
      var firstSettlementTwoLocation = 43u;

      var firstRoadOne = new Road(17u, 18u);
      var secondRoadOne = new Road(15u, 25u);
      var thirdRoadOne = new Road(30u, 31u);

      var thirdRoadTwo = new Road(32u, 33u);
      var secondRoadTwo = new Road(24u, 35u);
      var firstRoadTwo = new Road(43u, 44u);

      var firstMockPlayerId = Guid.NewGuid();
      var firstMockComputerPlayer = Substitute.For<IComputerPlayer>();
      firstMockComputerPlayer.Id.Returns(firstMockPlayerId);
      firstMockComputerPlayer.ChooseSettlementLocation(gameBoardManager.Data)
        .Returns(firstSettlementOneLocation, firstSettlementTwoLocation);
      firstMockComputerPlayer.ChooseRoad(gameBoardManager.Data).Returns(firstRoadOne, firstRoadTwo);

      var secondMockPlayerId = Guid.NewGuid();
      var secondMockComputerPlayer = Substitute.For<IComputerPlayer>();
      secondMockComputerPlayer.Id.Returns(secondMockPlayerId);
      secondMockComputerPlayer.ChooseSettlementLocation(gameBoardManager.Data)
        .Returns(secondSettlementOneLocation, secondSettlementTwoLocation);
      secondMockComputerPlayer.ChooseRoad(gameBoardManager.Data).Returns(secondRoadOne, secondRoadTwo);

      var thirdMockPlayerId = Guid.NewGuid();
      var thirdMockComputerPlayer = Substitute.For<IComputerPlayer>();
      thirdMockComputerPlayer.Id.Returns(thirdMockPlayerId);
      thirdMockComputerPlayer.ChooseSettlementLocation(gameBoardManager.Data)
        .Returns(thirdSettlementOneLocation, thirdSettlementTwoLocation);
      thirdMockComputerPlayer.ChooseRoad(gameBoardManager.Data).Returns(thirdRoadOne, thirdRoadTwo);

      var mockComputerPlayerFactory = Substitute.For<IComputerPlayerFactory>();
      mockComputerPlayerFactory.Create().Returns(firstMockComputerPlayer, secondMockComputerPlayer, thirdMockComputerPlayer);

      GameBoardUpdate gameBoardUpdate = null;
      var localGameController = this.CreateLocalGameController(mockDice, mockComputerPlayerFactory, gameBoardManager);
      localGameController.GameSetupUpdateEvent = (GameBoardUpdate u) => { gameBoardUpdate = u; };
      localGameController.InitialBoardSetupEvent = (GameBoardData d) => { };

      localGameController.TryJoiningGame(null);
      localGameController.TryLaunchGame();

      localGameController.StartGameSetup();
      gameBoardUpdate.ShouldNotBeNull();
      gameBoardUpdate.NewSettlements.Count.ShouldBe(3);
      gameBoardUpdate.NewSettlements.ShouldContainKeyAndValue(firstSettlementOneLocation, firstMockComputerPlayer.Id);
      gameBoardUpdate.NewSettlements.ShouldContainKeyAndValue(secondSettlementOneLocation, secondMockComputerPlayer.Id);
      gameBoardUpdate.NewSettlements.ShouldContainKeyAndValue(thirdSettlementOneLocation, thirdMockComputerPlayer.Id);
      gameBoardUpdate.NewRoads2.Count.ShouldBe(3);
      gameBoardUpdate.NewRoads2.ShouldContainKeyAndValue(firstRoadOne, firstMockComputerPlayer.Id);
      gameBoardUpdate.NewRoads2.ShouldContainKeyAndValue(secondRoadOne, secondMockComputerPlayer.Id);
      gameBoardUpdate.NewRoads2.ShouldContainKeyAndValue(thirdRoadOne, thirdMockComputerPlayer.Id);

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
      gameBoardUpdate.NewRoads2.Count.ShouldBe(3);
      gameBoardUpdate.NewRoads2.ShouldContainKeyAndValue(thirdRoadTwo, thirdMockComputerPlayer.Id);
      gameBoardUpdate.NewRoads2.ShouldContainKeyAndValue(secondRoadTwo, secondMockComputerPlayer.Id);
      gameBoardUpdate.NewRoads2.ShouldContainKeyAndValue(firstRoadTwo, firstMockComputerPlayer.Id);
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
