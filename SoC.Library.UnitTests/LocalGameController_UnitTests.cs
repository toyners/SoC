
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
    private const UInt32 MainSettlementOneLocation = 12u;
    private const UInt32 FirstSettlementOneLocation = 18u;
    private const UInt32 SecondSettlementOneLocation = 25u;
    private const UInt32 ThirdSettlementOneLocation = 31u;

    private const UInt32 ThirdSettlementTwoLocation = 33u;
    private const UInt32 SecondSettlementTwoLocation = 35u;
    private const UInt32 FirstSettlementTwoLocation = 43u;
    private const UInt32 MainSettlementTwoLocation = 40u;

    private Road mainRoadOne = new Road(12, 4);
    private Road firstRoadOne = new Road(17, 18);
    private Road secondRoadOne = new Road(15, 25);
    private Road thirdRoadOne = new Road(30, 31);

    private Road thirdRoadTwo = new Road(32, 33);
    private Road secondRoadTwo = new Road(24, 35);
    private Road firstRoadTwo = new Road(43, 44);
    private Road mainRoadTwo = new Road(40, 39);
    #endregion

    #region Methods
    [Test]
    [Category("LocalGameController")]
    public void JoinGame_DefaultGameOptions_PlayerDataPassedBack()
    {
      var firstComputerPlayer = new ComputerPlayer(Guid.NewGuid());
      var secondComputerPlayer = new ComputerPlayer(Guid.NewGuid());
      var thirdComputerPlayer = new ComputerPlayer(Guid.NewGuid());

      var mockComputerGameFactory = Substitute.For<IComputerPlayerFactory>();
      mockComputerGameFactory.Create().Returns(firstComputerPlayer, secondComputerPlayer, thirdComputerPlayer);
      var localGameController = new LocalGameControllerCreator()
        .ChangeComputerPlayerFactory(mockComputerGameFactory)
        .Controller;

      PlayerDataView[] playerData = null;
      localGameController.GameJoinedEvent = (PlayerDataView[] p) => { playerData = p; };
      localGameController.JoinGame(new GameOptions());
      
      playerData.ShouldNotBeNull();
      playerData.Length.ShouldBe(4);
      playerData[0].ShouldBeOfType<PlayerDataView>();
      this.AssertPlayerDataViewIsCorrect(playerData[0]);

      playerData[1].ShouldBeOfType<PlayerDataView>();
      playerData[1].Id.ShouldBe(firstComputerPlayer.Id);
      this.AssertPlayerDataViewIsCorrect(playerData[1]);
      
      playerData[2].ShouldBeOfType<PlayerDataView>();
      playerData[2].Id.ShouldBe(secondComputerPlayer.Id);
      this.AssertPlayerDataViewIsCorrect(playerData[2]);

      playerData[3].ShouldBeOfType<PlayerDataView>();
      playerData[3].Id.ShouldBe(thirdComputerPlayer.Id);
      this.AssertPlayerDataViewIsCorrect(playerData[3]);
    }

    [Test]
    [Category("LocalGameController")]
    public void JoinGame_NullGameOptions_PlayerDataPassedBack()
    {
      var firstComputerPlayer = new ComputerPlayer(Guid.NewGuid());
      var secondComputerPlayer = new ComputerPlayer(Guid.NewGuid());
      var thirdComputerPlayer = new ComputerPlayer(Guid.NewGuid());

      var mockComputerGameFactory = Substitute.For<IComputerPlayerFactory>();
      mockComputerGameFactory.Create().Returns(firstComputerPlayer, secondComputerPlayer, thirdComputerPlayer);
      var localGameController = new LocalGameControllerCreator()
        .ChangeComputerPlayerFactory(mockComputerGameFactory)
        .Controller;

      PlayerDataView[] playerData = null;
      localGameController.GameJoinedEvent = (PlayerDataView[] p) => { playerData = p; };
      localGameController.JoinGame();

      playerData.ShouldNotBeNull();
      playerData.Length.ShouldBe(4);
      playerData[0].ShouldBeOfType<PlayerDataView>();
      this.AssertPlayerDataViewIsCorrect(playerData[0]);

      playerData[1].ShouldBeOfType<PlayerDataView>();
      playerData[1].Id.ShouldBe(firstComputerPlayer.Id);
      this.AssertPlayerDataViewIsCorrect(playerData[1]);

      playerData[2].ShouldBeOfType<PlayerDataView>();
      playerData[2].Id.ShouldBe(secondComputerPlayer.Id);
      this.AssertPlayerDataViewIsCorrect(playerData[2]);

      playerData[3].ShouldBeOfType<PlayerDataView>();
      playerData[3].Id.ShouldBe(thirdComputerPlayer.Id);
      this.AssertPlayerDataViewIsCorrect(playerData[3]);
    }

    [Test]
    [Category("LocalGameController")]
    public void TryingToJoinGameMoreThanOnce_MeaningfulErrorIsRaised()
    {
      var localGameController = this.CreateLocalGameController();
      ErrorDetails errorDetails = null;
      localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };
      localGameController.JoinGame();
      localGameController.JoinGame();

      errorDetails.ShouldNotBeNull();
      errorDetails.Message.ShouldBe("Cannot call 'JoinGame' more than once.");
    }

    [Test]
    [Category("LocalGameController")]
    public void LaunchGame_LaunchGameWithoutJoining_MeaningfulErrorIsRaised()
    {
      var localGameController = this.CreateLocalGameController();
      ErrorDetails errorDetails = null;
      localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };
      localGameController.LaunchGame();

      errorDetails.ShouldNotBeNull();
      errorDetails.Message.ShouldBe("Cannot call 'LaunchGame' without joining game.");
    }

    [Test]
    [Category("LocalGameController")]
    public void LaunchGame_GameLaunchedAfterJoining_InitialBoardPassedBack()
    {
      var mockDice = NSubstitute.Substitute.For<IDice>();
      mockDice.RollTwoDice().Returns(12u, 10u, 8u, 2u);
      var localGameController = this.CreateLocalGameController(mockDice, new ComputerPlayerFactory(), new GameBoardManager(BoardSizes.Standard));

      PlayerDataView[] playerData = null;
      GameBoardData gameBoardData = null;
      localGameController.GameJoinedEvent = (PlayerDataView[] p) => { playerData = p; };
      localGameController.InitialBoardSetupEvent = (GameBoardData g) => { gameBoardData = g; };

      localGameController.JoinGame();
      localGameController.LaunchGame();

      gameBoardData.ShouldNotBeNull();
    }

    [Test]
    [Category("LocalGameController")]
    public void CompleteSetupWithPlayerInFirstSlot_ExpectedPlacementsAreReturned()
    {
      var mockDice = Substitute.For<IDice>();
      mockDice.RollTwoDice().Returns(12u, 10u, 8u, 6u);

      var gameBoardManager = new GameBoardManager(BoardSizes.Standard);

      var firstComputerPlayer = this.CreateMockComputerPlayer(gameBoardManager.Data, FirstSettlementOneLocation, FirstSettlementTwoLocation, firstRoadOne, firstRoadTwo);
      var secondComputerPlayer = this.CreateMockComputerPlayer(gameBoardManager.Data, SecondSettlementOneLocation, SecondSettlementTwoLocation, secondRoadOne, secondRoadTwo);
      var thirdComputerPlayer = this.CreateMockComputerPlayer(gameBoardManager.Data, ThirdSettlementOneLocation, ThirdSettlementTwoLocation, thirdRoadOne, thirdRoadTwo);

      GameBoardUpdate gameBoardUpdate = null;
      var localGameController = this.CreateLocalGameController(mockDice, gameBoardManager, firstComputerPlayer, secondComputerPlayer, thirdComputerPlayer);
      localGameController.GameSetupUpdateEvent = (GameBoardUpdate u) => { gameBoardUpdate = u; };
      localGameController.InitialBoardSetupEvent = (GameBoardData d) => { };

      localGameController.JoinGame();
      localGameController.LaunchGame();

      gameBoardUpdate = new GameBoardUpdate(); // Ensure that there is a state change for the gameBoardUpdate variable 
      localGameController.StartGameSetup();
      gameBoardUpdate.ShouldBeNull();

      gameBoardUpdate = null; // Ensure that there is a state change for the gameBoardUpdate variable

      localGameController.ContinueGameSetup(MainSettlementOneLocation, mainRoadOne);
      gameBoardUpdate.ShouldNotBeNull();
      gameBoardUpdate.NewSettlements.Count.ShouldBe(6);
      gameBoardUpdate.NewSettlements.ShouldContainKeyAndValue(FirstSettlementOneLocation, firstComputerPlayer.Id);
      gameBoardUpdate.NewSettlements.ShouldContainKeyAndValue(FirstSettlementTwoLocation, firstComputerPlayer.Id);
      gameBoardUpdate.NewSettlements.ShouldContainKeyAndValue(SecondSettlementOneLocation, secondComputerPlayer.Id);
      gameBoardUpdate.NewSettlements.ShouldContainKeyAndValue(SecondSettlementTwoLocation, secondComputerPlayer.Id);
      gameBoardUpdate.NewSettlements.ShouldContainKeyAndValue(ThirdSettlementOneLocation, thirdComputerPlayer.Id);
      gameBoardUpdate.NewSettlements.ShouldContainKeyAndValue(ThirdSettlementTwoLocation, thirdComputerPlayer.Id);

      gameBoardUpdate.NewRoads.Count.ShouldBe(6);
      gameBoardUpdate.NewRoads.ShouldContainKeyAndValue(firstRoadOne, firstComputerPlayer.Id);
      gameBoardUpdate.NewRoads.ShouldContainKeyAndValue(firstRoadTwo, firstComputerPlayer.Id);
      gameBoardUpdate.NewRoads.ShouldContainKeyAndValue(secondRoadOne, secondComputerPlayer.Id);
      gameBoardUpdate.NewRoads.ShouldContainKeyAndValue(secondRoadTwo, secondComputerPlayer.Id);
      gameBoardUpdate.NewRoads.ShouldContainKeyAndValue(thirdRoadOne, thirdComputerPlayer.Id);
      gameBoardUpdate.NewRoads.ShouldContainKeyAndValue(thirdRoadTwo, thirdComputerPlayer.Id);

      gameBoardUpdate = new GameBoardUpdate(); // Ensure that there is a state change for the gameBoardUpdate variable 

      localGameController.CompleteGameSetup(MainSettlementTwoLocation, mainRoadTwo);
      gameBoardUpdate.ShouldBeNull();
    }

    [Test]
    [Category("LocalGameController")]
    public void CompleteSetupWithPlayerInSecondSlot_ExpectedPlacementsAreReturned()
    {
      var mockDice = Substitute.For<IDice>();
      mockDice.RollTwoDice().Returns(10u, 12u, 8u, 6u);

      var gameBoardManager = new GameBoardManager(BoardSizes.Standard);

      var firstComputerPlayer = this.CreateMockComputerPlayer(gameBoardManager.Data, FirstSettlementOneLocation, FirstSettlementTwoLocation, firstRoadOne, firstRoadTwo);
      var secondComputerPlayer = this.CreateMockComputerPlayer(gameBoardManager.Data, SecondSettlementOneLocation, SecondSettlementTwoLocation, secondRoadOne, secondRoadTwo);
      var thirdComputerPlayer = this.CreateMockComputerPlayer(gameBoardManager.Data, ThirdSettlementOneLocation, ThirdSettlementTwoLocation, thirdRoadOne, thirdRoadTwo);

      GameBoardUpdate gameBoardUpdate = null;
      var localGameController = this.CreateLocalGameController(mockDice, gameBoardManager, firstComputerPlayer, secondComputerPlayer, thirdComputerPlayer);
      localGameController.GameSetupUpdateEvent = (GameBoardUpdate u) => { gameBoardUpdate = u; };
      localGameController.InitialBoardSetupEvent = (GameBoardData d) => { };

      localGameController.JoinGame();
      localGameController.LaunchGame();

      localGameController.StartGameSetup();
      gameBoardUpdate.ShouldNotBeNull();
      gameBoardUpdate.NewSettlements.Count.ShouldBe(1);
      gameBoardUpdate.NewSettlements.ShouldContainKeyAndValue(FirstSettlementOneLocation, firstComputerPlayer.Id);
      gameBoardUpdate.NewRoads.Count.ShouldBe(1);
      gameBoardUpdate.NewRoads.ShouldContainKeyAndValue(firstRoadOne, firstComputerPlayer.Id);

      gameBoardUpdate = null; // Ensure that there is a state change for the gameBoardUpdate variable 
      localGameController.ContinueGameSetup(MainSettlementOneLocation, mainRoadOne);

      gameBoardUpdate.ShouldNotBeNull();
      gameBoardUpdate.NewSettlements.Count.ShouldBe(4);
      gameBoardUpdate.NewSettlements.ShouldContainKeyAndValue(SecondSettlementOneLocation, secondComputerPlayer.Id);
      gameBoardUpdate.NewSettlements.ShouldContainKeyAndValue(SecondSettlementTwoLocation, secondComputerPlayer.Id);
      gameBoardUpdate.NewSettlements.ShouldContainKeyAndValue(ThirdSettlementOneLocation, thirdComputerPlayer.Id);
      gameBoardUpdate.NewSettlements.ShouldContainKeyAndValue(ThirdSettlementTwoLocation, thirdComputerPlayer.Id);

      gameBoardUpdate.NewRoads.Count.ShouldBe(4);
      gameBoardUpdate.NewRoads.ShouldContainKeyAndValue(secondRoadOne, secondComputerPlayer.Id);
      gameBoardUpdate.NewRoads.ShouldContainKeyAndValue(secondRoadTwo, secondComputerPlayer.Id);
      gameBoardUpdate.NewRoads.ShouldContainKeyAndValue(thirdRoadOne, thirdComputerPlayer.Id);
      gameBoardUpdate.NewRoads.ShouldContainKeyAndValue(thirdRoadTwo, thirdComputerPlayer.Id);

      gameBoardUpdate = null; // Ensure that there is a state change for the gameBoardUpdate variable 
      localGameController.CompleteGameSetup(MainSettlementTwoLocation, mainRoadTwo);

      gameBoardUpdate.NewSettlements.Count.ShouldBe(1);
      gameBoardUpdate.NewSettlements.ShouldContainKeyAndValue(FirstSettlementTwoLocation, firstComputerPlayer.Id);
      gameBoardUpdate.NewRoads.Count.ShouldBe(1);
      gameBoardUpdate.NewRoads.ShouldContainKeyAndValue(firstRoadTwo, firstComputerPlayer.Id);
    }

    [Test]
    [Category("LocalGameController")]
    public void CompleteSetupWithPlayerInThirdSlot_ExpectedPlacementsAreReturned()
    {
      var mockDice = Substitute.For<IDice>();
      mockDice.RollTwoDice().Returns(8u, 12u, 10u, 6u);

      var gameBoardManager = new GameBoardManager(BoardSizes.Standard);

      var firstComputerPlayer = this.CreateMockComputerPlayer(gameBoardManager.Data, FirstSettlementOneLocation, FirstSettlementTwoLocation, firstRoadOne, firstRoadTwo);
      var secondComputerPlayer = this.CreateMockComputerPlayer(gameBoardManager.Data, SecondSettlementOneLocation, SecondSettlementTwoLocation, secondRoadOne, secondRoadTwo);
      var thirdComputerPlayer = this.CreateMockComputerPlayer(gameBoardManager.Data, ThirdSettlementOneLocation, ThirdSettlementTwoLocation, thirdRoadOne, thirdRoadTwo);

      GameBoardUpdate gameBoardUpdate = null;
      var localGameController = this.CreateLocalGameController(mockDice, gameBoardManager, firstComputerPlayer, secondComputerPlayer, thirdComputerPlayer);
      localGameController.GameSetupUpdateEvent = (GameBoardUpdate u) => { gameBoardUpdate = u; };
      localGameController.InitialBoardSetupEvent = (GameBoardData d) => { };

      localGameController.JoinGame();
      localGameController.LaunchGame();

      localGameController.StartGameSetup();
      gameBoardUpdate.ShouldNotBeNull();
      gameBoardUpdate.NewSettlements.Count.ShouldBe(2);
      gameBoardUpdate.NewSettlements.ShouldContainKeyAndValue(FirstSettlementOneLocation, firstComputerPlayer.Id);
      gameBoardUpdate.NewSettlements.ShouldContainKeyAndValue(SecondSettlementOneLocation, secondComputerPlayer.Id);
      gameBoardUpdate.NewRoads.Count.ShouldBe(2);
      gameBoardUpdate.NewRoads.ShouldContainKeyAndValue(firstRoadOne, firstComputerPlayer.Id);
      gameBoardUpdate.NewRoads.ShouldContainKeyAndValue(secondRoadOne, secondComputerPlayer.Id);

      gameBoardUpdate = null; // Ensure that there is a state change for the gameBoardUpdate variable 
      localGameController.ContinueGameSetup(MainSettlementOneLocation, mainRoadOne);

      gameBoardUpdate.ShouldNotBeNull();
      gameBoardUpdate.NewSettlements.Count.ShouldBe(2);
      gameBoardUpdate.NewSettlements.ShouldContainKeyAndValue(ThirdSettlementOneLocation, thirdComputerPlayer.Id);
      gameBoardUpdate.NewSettlements.ShouldContainKeyAndValue(ThirdSettlementTwoLocation, thirdComputerPlayer.Id);

      gameBoardUpdate.NewRoads.Count.ShouldBe(2);
      gameBoardUpdate.NewRoads.ShouldContainKeyAndValue(thirdRoadOne, thirdComputerPlayer.Id);
      gameBoardUpdate.NewRoads.ShouldContainKeyAndValue(thirdRoadTwo, thirdComputerPlayer.Id);

      gameBoardUpdate = null; // Ensure that there is a state change for the gameBoardUpdate variable 
      localGameController.CompleteGameSetup(MainSettlementTwoLocation, mainRoadTwo);

      gameBoardUpdate.NewSettlements.Count.ShouldBe(2);
      gameBoardUpdate.NewSettlements.ShouldContainKeyAndValue(SecondSettlementTwoLocation, secondComputerPlayer.Id);
      gameBoardUpdate.NewSettlements.ShouldContainKeyAndValue(FirstSettlementTwoLocation, firstComputerPlayer.Id);
      gameBoardUpdate.NewRoads.Count.ShouldBe(2);
      gameBoardUpdate.NewRoads.ShouldContainKeyAndValue(secondRoadTwo, secondComputerPlayer.Id);
      gameBoardUpdate.NewRoads.ShouldContainKeyAndValue(firstRoadTwo, firstComputerPlayer.Id);
    }

    [Test]
    [Category("LocalGameController")]
    public void CompleteSetupWithPlayerInFourthSlot_ExpectedPlacementsAreReturned()
    {
      var mockDice = Substitute.For<IDice>();
      mockDice.RollTwoDice().Returns(6u, 12u, 10u, 8u);

      var gameBoardManager = new GameBoardManager(BoardSizes.Standard);

      var firstComputerPlayer = this.CreateMockComputerPlayer(gameBoardManager.Data, FirstSettlementOneLocation, FirstSettlementTwoLocation, firstRoadOne, firstRoadTwo);
      var secondComputerPlayer = this.CreateMockComputerPlayer(gameBoardManager.Data, SecondSettlementOneLocation, SecondSettlementTwoLocation, secondRoadOne, secondRoadTwo);
      var thirdComputerPlayer = this.CreateMockComputerPlayer(gameBoardManager.Data, ThirdSettlementOneLocation, ThirdSettlementTwoLocation, thirdRoadOne, thirdRoadTwo);

      GameBoardUpdate gameBoardUpdate = null;
      var localGameController = this.CreateLocalGameController(mockDice, gameBoardManager, firstComputerPlayer, secondComputerPlayer, thirdComputerPlayer);
      localGameController.GameSetupUpdateEvent = (GameBoardUpdate u) => { gameBoardUpdate = u; };
      localGameController.InitialBoardSetupEvent = (GameBoardData d) => { };

      localGameController.JoinGame();
      localGameController.LaunchGame();

      localGameController.StartGameSetup();
      gameBoardUpdate.ShouldNotBeNull();
      gameBoardUpdate.NewSettlements.Count.ShouldBe(3);
      gameBoardUpdate.NewSettlements.ShouldContainKeyAndValue(FirstSettlementOneLocation, firstComputerPlayer.Id);
      gameBoardUpdate.NewSettlements.ShouldContainKeyAndValue(SecondSettlementOneLocation, secondComputerPlayer.Id);
      gameBoardUpdate.NewSettlements.ShouldContainKeyAndValue(ThirdSettlementOneLocation, thirdComputerPlayer.Id);
      gameBoardUpdate.NewRoads.Count.ShouldBe(3);
      gameBoardUpdate.NewRoads.ShouldContainKeyAndValue(firstRoadOne, firstComputerPlayer.Id);
      gameBoardUpdate.NewRoads.ShouldContainKeyAndValue(secondRoadOne, secondComputerPlayer.Id);
      gameBoardUpdate.NewRoads.ShouldContainKeyAndValue(thirdRoadOne, thirdComputerPlayer.Id);

      gameBoardUpdate = new GameBoardUpdate(); // Ensure that there is a state change for the gameBoardUpdate variable 
      localGameController.ContinueGameSetup(MainSettlementOneLocation, mainRoadOne);
      gameBoardUpdate.ShouldBeNull();

      gameBoardUpdate = null; // Ensure that there is a state change for the gameBoardUpdate variable 
      localGameController.CompleteGameSetup(MainSettlementTwoLocation, mainRoadTwo);

      gameBoardUpdate.ShouldNotBeNull();
      gameBoardUpdate.NewSettlements.Count.ShouldBe(3);
      gameBoardUpdate.NewSettlements.ShouldContainKeyAndValue(ThirdSettlementTwoLocation, thirdComputerPlayer.Id);
      gameBoardUpdate.NewSettlements.ShouldContainKeyAndValue(SecondSettlementTwoLocation, secondComputerPlayer.Id);
      gameBoardUpdate.NewSettlements.ShouldContainKeyAndValue(FirstSettlementTwoLocation, firstComputerPlayer.Id);
      gameBoardUpdate.NewRoads.Count.ShouldBe(3);
      gameBoardUpdate.NewRoads.ShouldContainKeyAndValue(thirdRoadTwo, thirdComputerPlayer.Id);
      gameBoardUpdate.NewRoads.ShouldContainKeyAndValue(secondRoadTwo, secondComputerPlayer.Id);
      gameBoardUpdate.NewRoads.ShouldContainKeyAndValue(firstRoadTwo, firstComputerPlayer.Id);
    }
    
    [Test]
    [Category("LocalGameController")]
    [TestCase(12u, 10u, 8u, 6u)]
    [TestCase(10u, 12u, 8u, 6u)]
    [TestCase(8u, 12u, 10u, 6u)]
    [TestCase(6u, 12u, 10u, 8u)]
    public void CompleteSetupWithPlayer_ExpectedResourcesAreReturned(UInt32 firstDiceRoll, UInt32 secondDiceRoll, UInt32 thirdDiceRoll, UInt32 fourthDiceRoll)
    {
      var mockDice = Substitute.For<IDice>();
      mockDice.RollTwoDice().Returns(firstDiceRoll, secondDiceRoll, thirdDiceRoll, fourthDiceRoll);

      var gameBoardManager = new GameBoardManager(BoardSizes.Standard);

      var firstComputerPlayer = this.CreateMockComputerPlayer(gameBoardManager.Data, FirstSettlementOneLocation, FirstSettlementTwoLocation, firstRoadOne, firstRoadTwo);
      var secondComputerPlayer = this.CreateMockComputerPlayer(gameBoardManager.Data, SecondSettlementOneLocation, SecondSettlementTwoLocation, secondRoadOne, secondRoadTwo);
      var thirdComputerPlayer = this.CreateMockComputerPlayer(gameBoardManager.Data, ThirdSettlementOneLocation, ThirdSettlementTwoLocation, thirdRoadOne, thirdRoadTwo);

      var localGameController = this.CreateLocalGameController(mockDice, gameBoardManager, firstComputerPlayer, secondComputerPlayer, thirdComputerPlayer);

      ResourceUpdate resourceUpdate = null;
      Guid mainPlayerId = Guid.Empty;
      localGameController.GameSetupResourcesEvent = (ResourceUpdate r) => { resourceUpdate = r; };
      localGameController.GameJoinedEvent = (PlayerDataView[] p) => { mainPlayerId = p[0].Id; };

      localGameController.JoinGame();
      localGameController.LaunchGame();
      localGameController.StartGameSetup();
      localGameController.ContinueGameSetup(MainSettlementOneLocation, mainRoadOne);
      localGameController.CompleteGameSetup(MainSettlementTwoLocation, mainRoadTwo);

      resourceUpdate.ShouldNotBeNull();
      resourceUpdate.Resources.Count.ShouldBe(4);
      resourceUpdate.Resources.ShouldContainKey(mainPlayerId);
      resourceUpdate.Resources.ShouldContainKey(firstComputerPlayer.Id);
      resourceUpdate.Resources.ShouldContainKey(secondComputerPlayer.Id);
      resourceUpdate.Resources.ShouldContainKey(thirdComputerPlayer.Id);

      resourceUpdate.Resources[mainPlayerId].BrickCount.ShouldBe(1u);
      resourceUpdate.Resources[mainPlayerId].GrainCount.ShouldBe(1u);
      resourceUpdate.Resources[mainPlayerId].LumberCount.ShouldBe(0u);
      resourceUpdate.Resources[mainPlayerId].OreCount.ShouldBe(0u);
      resourceUpdate.Resources[mainPlayerId].WoolCount.ShouldBe(1u);

      resourceUpdate.Resources[firstComputerPlayer.Id].BrickCount.ShouldBe(0u);
      resourceUpdate.Resources[firstComputerPlayer.Id].GrainCount.ShouldBe(1u);
      resourceUpdate.Resources[firstComputerPlayer.Id].LumberCount.ShouldBe(1u);
      resourceUpdate.Resources[firstComputerPlayer.Id].OreCount.ShouldBe(0u);
      resourceUpdate.Resources[firstComputerPlayer.Id].WoolCount.ShouldBe(1u);

      resourceUpdate.Resources[secondComputerPlayer.Id].BrickCount.ShouldBe(0u);
      resourceUpdate.Resources[secondComputerPlayer.Id].GrainCount.ShouldBe(0u);
      resourceUpdate.Resources[secondComputerPlayer.Id].LumberCount.ShouldBe(1u);
      resourceUpdate.Resources[secondComputerPlayer.Id].OreCount.ShouldBe(1u);
      resourceUpdate.Resources[secondComputerPlayer.Id].WoolCount.ShouldBe(1u);

      resourceUpdate.Resources[thirdComputerPlayer.Id].BrickCount.ShouldBe(0u);
      resourceUpdate.Resources[thirdComputerPlayer.Id].GrainCount.ShouldBe(1u);
      resourceUpdate.Resources[thirdComputerPlayer.Id].LumberCount.ShouldBe(1u);
      resourceUpdate.Resources[thirdComputerPlayer.Id].OreCount.ShouldBe(0u);
      resourceUpdate.Resources[thirdComputerPlayer.Id].WoolCount.ShouldBe(1u);
    }

    [Test]
    [Category("LocalGameController")]
    public void ContinueGameSetup_CallOutOfSequence_MeaningfulErrorDetailsPassedBack()
    {
      var localGameController = this.CreateLocalGameController();
      ErrorDetails errorDetails = null;
      localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

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
      localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

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

      var firstComputerPlayer = this.CreateMockComputerPlayer(gameBoardManager.Data, FirstSettlementOneLocation, FirstSettlementTwoLocation, firstRoadOne, firstRoadTwo);
      var secondComputerPlayer = this.CreateMockComputerPlayer(gameBoardManager.Data, SecondSettlementOneLocation, SecondSettlementTwoLocation, secondRoadOne, secondRoadTwo);
      var thirdComputerPlayer = this.CreateMockComputerPlayer(gameBoardManager.Data, ThirdSettlementOneLocation, ThirdSettlementTwoLocation, thirdRoadOne, thirdRoadTwo);

      ErrorDetails errorDetails = null;
      GameBoardUpdate gameBoardUpdate = null;
      var localGameController = this.CreateLocalGameController(mockDice, gameBoardManager, firstComputerPlayer, secondComputerPlayer, thirdComputerPlayer);
      localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };
      localGameController.GameSetupUpdateEvent = (GameBoardUpdate u) => { gameBoardUpdate = u; };

      localGameController.JoinGame();
      localGameController.LaunchGame();
      localGameController.StartGameSetup();
      localGameController.ContinueGameSetup(0u, new Road(1u, 2u));

      errorDetails.ShouldNotBeNull();
      errorDetails.Message.ShouldBe("Cannot place road at [1, 2]. No connection to a player owned road or settlement.");
      gameBoardUpdate.ShouldBeNull();
    }

    [Test]
    [Category("LocalGameController")]
    public void PlayerSelectsSameLocationAsComputerPlayerDuringFirstSetupRound_MeaningfulErrorDetailsPassedBack()
    {
      var mockDice = Substitute.For<IDice>();
      mockDice.RollTwoDice().Returns(10u, 12u, 8u, 6u);

      var gameBoardManager = new GameBoardManager(BoardSizes.Standard);

      var firstComputerPlayer = this.CreateMockComputerPlayer(gameBoardManager.Data, FirstSettlementOneLocation, FirstSettlementTwoLocation, firstRoadOne, firstRoadTwo);
      var secondComputerPlayer = this.CreateMockComputerPlayer(gameBoardManager.Data, SecondSettlementOneLocation, SecondSettlementTwoLocation, secondRoadOne, secondRoadTwo);
      var thirdComputerPlayer = this.CreateMockComputerPlayer(gameBoardManager.Data, ThirdSettlementOneLocation, ThirdSettlementTwoLocation, thirdRoadOne, thirdRoadTwo);

      ErrorDetails exception = null;
      var localGameController = this.CreateLocalGameController(mockDice, gameBoardManager, firstComputerPlayer, secondComputerPlayer, thirdComputerPlayer);
      localGameController.ErrorRaisedEvent = (ErrorDetails e) => { exception = e; };

      localGameController.JoinGame();
      localGameController.LaunchGame();
      localGameController.StartGameSetup();

      GameBoardUpdate gameBoardUpdate = null;
      localGameController.GameSetupUpdateEvent = (GameBoardUpdate u) => { gameBoardUpdate = u; };

      localGameController.ContinueGameSetup(FirstSettlementOneLocation, new Road(0u, 1u));
      exception.ShouldNotBeNull();
      exception.Message.ShouldBe("Cannot place settlement: Location " + FirstSettlementOneLocation + " already owned by player " + firstComputerPlayer.Id);
      gameBoardUpdate.ShouldBeNull();
    }

    [Test]
    [Category("LocalGameController")]
    public void PlayerSelectsSameLocationAsComputerPlayerDuringSecondSetupRound_MeaningfulErrorDetailsPassedBack()
    {
      var mockDice = Substitute.For<IDice>();
      mockDice.RollTwoDice().Returns(12u, 10u, 8u, 6u);

      var gameBoardManager = new GameBoardManager(BoardSizes.Standard);

      var firstComputerPlayer = this.CreateMockComputerPlayer(gameBoardManager.Data, FirstSettlementOneLocation, FirstSettlementTwoLocation, firstRoadOne, firstRoadTwo);
      var secondComputerPlayer = this.CreateMockComputerPlayer(gameBoardManager.Data, SecondSettlementOneLocation, SecondSettlementTwoLocation, secondRoadOne, secondRoadTwo);
      var thirdComputerPlayer = this.CreateMockComputerPlayer(gameBoardManager.Data, ThirdSettlementOneLocation, ThirdSettlementTwoLocation, thirdRoadOne, thirdRoadTwo);

      ErrorDetails exception = null;
      var localGameController = this.CreateLocalGameController(mockDice, gameBoardManager, firstComputerPlayer, secondComputerPlayer, thirdComputerPlayer);
      localGameController.ErrorRaisedEvent = (ErrorDetails e) => { exception = e; };

      localGameController.JoinGame();
      localGameController.LaunchGame();
      localGameController.StartGameSetup();
      localGameController.ContinueGameSetup(0, new Road(0, 1));

      GameBoardUpdate gameBoardUpdate = null;
      localGameController.GameSetupUpdateEvent = (GameBoardUpdate u) => { gameBoardUpdate = u; };

      localGameController.CompleteGameSetup(FirstSettlementOneLocation, new Road(0, 1));

      exception.ShouldNotBeNull();
      exception.Message.ShouldBe("Cannot place settlement: Location " + FirstSettlementOneLocation + " already owned by player " + firstComputerPlayer.Id);
      gameBoardUpdate.ShouldBeNull();
    }

    [Test]
    [Category("LocalGameController")]
    public void PlayerSelectsLocationTooCloseToComputerPlayerDuringFirstSetupRound_MeaningfulErrorDetailsPassedBack()
    {
      var mockDice = Substitute.For<IDice>();
      mockDice.RollTwoDice().Returns(10u, 12u, 8u, 6u);

      var gameBoardManager = new GameBoardManager(BoardSizes.Standard);

      var firstComputerPlayer = this.CreateMockComputerPlayer(gameBoardManager.Data, FirstSettlementOneLocation, FirstSettlementTwoLocation, firstRoadOne, firstRoadTwo);
      var secondComputerPlayer = this.CreateMockComputerPlayer(gameBoardManager.Data, SecondSettlementOneLocation, SecondSettlementTwoLocation, secondRoadOne, secondRoadTwo);
      var thirdComputerPlayer = this.CreateMockComputerPlayer(gameBoardManager.Data, ThirdSettlementOneLocation, ThirdSettlementTwoLocation, thirdRoadOne, thirdRoadTwo);

      ErrorDetails exception = null;
      var localGameController = this.CreateLocalGameController(mockDice, gameBoardManager, firstComputerPlayer, secondComputerPlayer, thirdComputerPlayer);
      localGameController.ErrorRaisedEvent = (ErrorDetails e) => { exception = e; };
      localGameController.JoinGame();
      localGameController.LaunchGame();
      localGameController.StartGameSetup();

      GameBoardUpdate gameBoardUpdate = null;
      localGameController.GameSetupUpdateEvent = (GameBoardUpdate u) => { gameBoardUpdate = u; };

      localGameController.ContinueGameSetup(19u, new Road(0u, 1u));

      exception.ShouldNotBeNull();
      exception.Message.ShouldBe("Cannot place settlement: Too close to player " + firstComputerPlayer.Id + " at location " + FirstSettlementOneLocation);
      gameBoardUpdate.ShouldBeNull();
    }

    [Test]
    [Category("LocalGameController")]
    public void PlayerSelectsLocationTooCloseToComputerPlayerDuringSecondSetupRound_MeaningfulErrorDetailsPassedBack()
    {
      var mockDice = Substitute.For<IDice>();
      mockDice.RollTwoDice().Returns(12u, 10u, 8u, 6u);

      var gameBoardManager = new GameBoardManager(BoardSizes.Standard);

      var firstComputerPlayer = this.CreateMockComputerPlayer(gameBoardManager.Data, FirstSettlementOneLocation, FirstSettlementTwoLocation, firstRoadOne, firstRoadTwo);
      var secondComputerPlayer = this.CreateMockComputerPlayer(gameBoardManager.Data, SecondSettlementOneLocation, SecondSettlementTwoLocation, secondRoadOne, secondRoadTwo);
      var thirdComputerPlayer = this.CreateMockComputerPlayer(gameBoardManager.Data, ThirdSettlementOneLocation, ThirdSettlementTwoLocation, thirdRoadOne, thirdRoadTwo);

      ErrorDetails exception = null;
      var localGameController = this.CreateLocalGameController(mockDice, gameBoardManager, firstComputerPlayer, secondComputerPlayer, thirdComputerPlayer);
      localGameController.ErrorRaisedEvent = (ErrorDetails e) => { exception = e; };
      localGameController.JoinGame();
      localGameController.LaunchGame();
      localGameController.StartGameSetup();
      localGameController.ContinueGameSetup(0, new Road(0, 1));

      GameBoardUpdate gameBoardUpdate = null;
      localGameController.GameSetupUpdateEvent = (GameBoardUpdate u) => { gameBoardUpdate = u; };

      localGameController.CompleteGameSetup(19, new Road(19, 18));

      exception.ShouldNotBeNull();
      exception.Message.ShouldBe("Cannot place settlement: Too close to player " + firstComputerPlayer.Id + " at location " + FirstSettlementOneLocation);
      gameBoardUpdate.ShouldBeNull();
    }

    [Test]
    [Category("LocalGameController")]
    public void PlayerPlacesRoadWithNoConnectionToAnySettlementsDuringSecondSetupRound_MeaningfulErrorDetailsPassedBack()
    {
      var localGameController = CreateLocalGameControllerWithMainPlayerGoingFirstInSetup();

      ErrorDetails errorDetails = null;
      localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

      localGameController.JoinGame();
      localGameController.LaunchGame();
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
      localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

      GameBoardUpdate gameBoardUpdate = null;
      localGameController.GameSetupUpdateEvent = (GameBoardUpdate u) => { gameBoardUpdate = u; };

      localGameController.JoinGame();
      localGameController.LaunchGame();
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
      localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

      GameBoardUpdate gameBoardUpdate = null;
      localGameController.GameSetupUpdateEvent = (GameBoardUpdate u) => { gameBoardUpdate = u; };

      localGameController.JoinGame();
      localGameController.LaunchGame();
      localGameController.StartGameSetup();
      localGameController.ContinueGameSetup(53, new Road(53, 54));

      errorDetails.ShouldNotBeNull();
      errorDetails.Message.ShouldBe("Cannot place road at [53, 54]. This is outside of board range (0 - 53).");
      gameBoardUpdate.ShouldBeNull();
    }

    [Test]
    [Category("LocalGameController")]
    public void PlayerPlacesRoadWhereNoConnectionExistsDuringFirstSetupRound_MeaningfulErrorDetailsPassedBack()
    {
      var localGameController = CreateLocalGameControllerWithMainPlayerGoingFirstInSetup();

      ErrorDetails errorDetails = null;
      localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

      GameBoardUpdate gameBoardUpdate = null;
      localGameController.GameSetupUpdateEvent = (GameBoardUpdate u) => { gameBoardUpdate = u; };

      localGameController.JoinGame();
      localGameController.LaunchGame();
      localGameController.StartGameSetup();
      localGameController.ContinueGameSetup(28, new Road(28, 40));

      errorDetails.ShouldNotBeNull();
      errorDetails.Message.ShouldBe("Cannot place road at [28, 40]. There is no direct connection between those points.");
      gameBoardUpdate.ShouldBeNull();
    }

    [Test]
    [Category("LocalGameController")]
    public void PlayerPlacesSettlementOffGameBoardDuringSecondSetupRound_MeaningfulErrorDetailsPassedBack()
    {
      var localGameController = CreateLocalGameControllerWithMainPlayerGoingFirstInSetup();

      ErrorDetails errorDetails = null;
      localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

      localGameController.JoinGame();
      localGameController.LaunchGame();
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
      localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

      localGameController.JoinGame();
      localGameController.LaunchGame();
      localGameController.StartGameSetup();
      localGameController.ContinueGameSetup(0, new Road(0, 1));

      GameBoardUpdate gameBoardUpdate = null;
      localGameController.GameSetupUpdateEvent = (GameBoardUpdate u) => { gameBoardUpdate = u; };

      localGameController.CompleteGameSetup(53, new Road(53, 54));

      errorDetails.ShouldNotBeNull();
      errorDetails.Message.ShouldBe("Cannot place road at [53, 54]. This is outside of board range (0 - 53).");
      gameBoardUpdate.ShouldBeNull();
    }

    [Test]
    [Category("LocalGameController")]
    public void PlayerPlacesRoadWhereNoConnectionExistsDuringSecondSetupRound_MeaningfulErrorDetailsPassedBack()
    {
      var localGameController = CreateLocalGameControllerWithMainPlayerGoingFirstInSetup();

      ErrorDetails errorDetails = null;
      localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

      localGameController.JoinGame();
      localGameController.LaunchGame();
      localGameController.StartGameSetup();
      localGameController.ContinueGameSetup(0, new Road(0, 1));

      GameBoardUpdate gameBoardUpdate = null;
      localGameController.GameSetupUpdateEvent = (GameBoardUpdate u) => { gameBoardUpdate = u; };

      localGameController.CompleteGameSetup(28, new Road(28, 40));

      errorDetails.ShouldNotBeNull();
      errorDetails.Message.ShouldBe("Cannot place road at [28, 40]. There is no direct connection between those points.");
      gameBoardUpdate.ShouldBeNull();
    }

    [Test]
    [Category("LocalGameController")]
    public void MainGameLoop_MainPlayerTurn_ReceiveResourceDetails()
    {
      var mockDice = Substitute.For<IDice>();
      var gameSetupOrder = new [] { 12u, 10u, 8u, 8u };
      var gameTurnOrder = new [] { 12u, 10u, 8u, 6u };
      var resourceRoll = new[] { 8u };
      mockDice = new MockDice(gameSetupOrder, gameTurnOrder, resourceRoll);

      var gameBoardManager = new GameBoardManager(BoardSizes.Standard);

      var firstComputerPlayer = this.CreateMockComputerPlayer(gameBoardManager.Data, FirstSettlementOneLocation, FirstSettlementTwoLocation, firstRoadOne, firstRoadTwo);
      var secondComputerPlayer = this.CreateMockComputerPlayer(gameBoardManager.Data, SecondSettlementOneLocation, SecondSettlementTwoLocation, secondRoadOne, secondRoadTwo);
      var thirdComputerPlayer = this.CreateMockComputerPlayer(gameBoardManager.Data, ThirdSettlementOneLocation, ThirdSettlementTwoLocation, thirdRoadOne, thirdRoadTwo);

      var localGameController = this.CreateLocalGameController(mockDice, gameBoardManager, firstComputerPlayer, secondComputerPlayer, thirdComputerPlayer);

      ResourceUpdate resourceUpdate = null;
      localGameController.StartPlayerTurnEvent = (ResourceUpdate r) => { resourceUpdate = r; };

      PlayerDataView[] playerData = null;
      localGameController.GameJoinedEvent = (PlayerDataView[] p) => { playerData = p; };

      localGameController.JoinGame();
      localGameController.LaunchGame();
      localGameController.StartGameSetup();
      localGameController.ContinueGameSetup(12, new Road(12, 11));
      localGameController.CompleteGameSetup(40, new Road(40, 39));

      var id = playerData[0].Id;
      resourceUpdate.ShouldNotBeNull();
      resourceUpdate.Resources.Count.ShouldBe(1);
      resourceUpdate.Resources[id].BrickCount.ShouldBe(1u);
      resourceUpdate.Resources[id].GrainCount.ShouldBe(0u);
      resourceUpdate.Resources[id].LumberCount.ShouldBe(0u);
      resourceUpdate.Resources[id].OreCount.ShouldBe(0u);
      resourceUpdate.Resources[id].WoolCount.ShouldBe(0u);
    }

    private void AssertPlayerDataViewIsCorrect(PlayerDataView playerDataView)
    {
      playerDataView.DisplayedDevelopmentCards.ShouldBeNull();
      playerDataView.HiddenDevelopmentCards.ShouldBe(0);
      playerDataView.ResourceCards.ShouldBe(0u);
    }

    private LocalGameController CreateLocalGameController()
    {
      return this.CreateLocalGameController(new Dice(), new ComputerPlayerFactory(), new GameBoardManager(BoardSizes.Standard));
    }

    private LocalGameController CreateLocalGameController(IDice diceRoller, IComputerPlayerFactory computerPlayerFactory, GameBoardManager gameBoardManager)
    {
      var localGameController = new LocalGameController(diceRoller, computerPlayerFactory, gameBoardManager);
      localGameController.GameJoinedEvent = (PlayerDataView[] players) => { };
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

      var firstComputerPlayer = this.CreateMockComputerPlayer(gameBoardManager.Data, FirstSettlementOneLocation, FirstSettlementTwoLocation, firstRoadOne, firstRoadTwo);
      var secondComputerPlayer = this.CreateMockComputerPlayer(gameBoardManager.Data, SecondSettlementOneLocation, SecondSettlementTwoLocation, secondRoadOne, secondRoadTwo);
      var thirdComputerPlayer = this.CreateMockComputerPlayer(gameBoardManager.Data, ThirdSettlementOneLocation, ThirdSettlementTwoLocation, thirdRoadOne, thirdRoadTwo);

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
