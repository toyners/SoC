
namespace Jabberwocky.SoC.Library.UnitTests
{
  using System;
  using System.Collections.Generic;
  using GameBoards;
  using Interfaces;
  using NSubstitute;
  using NUnit.Framework;
  using Shouldly;

  [TestFixture]
  public class LocalGameController_UnitTests
  {
    #region Fields
    private const String PlayerName = "Player";
    private const String FirstOpponentName = "Bob";
    private const String SecondOpponentName = "Sally";
    private const String ThirdOpponentName = "Rich";

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
    public void JoinGame_NullGameOptions_OpponentDataPassedBack()
    {
      this.RunOpponentDataPassBackTests(null);
    }

    [Test]
    [Category("LocalGameController")]
    public void JoinGame_DefaultGameOptions_OpponentDataPassedBack()
    {
      this.RunOpponentDataPassBackTests(new GameOptions());
    }

    [Test]
    [Category("LocalGameController")]
    public void TryingToJoinGameMoreThanOnce_MeaningfulErrorIsRaised()
    {
      var firstOpponent = new MockComputerPlayer(FirstOpponentName);
      var secondOpponent = new MockComputerPlayer(SecondOpponentName);
      var thirdOpponent = new MockComputerPlayer(ThirdOpponentName);

      var mockPlayerFactory = this.CreateMockPlayerFactory(firstOpponent, secondOpponent, thirdOpponent);

      var localGameController = new LocalGameControllerCreator().ChangeComputerPlayerFactory(mockPlayerFactory).Create();
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

      var firstOpponent = new MockComputerPlayer(FirstOpponentName);
      var secondOpponent = new MockComputerPlayer(SecondOpponentName);
      var thirdOpponent = new MockComputerPlayer(ThirdOpponentName);

      var mockPlayerFactory = this.CreateMockPlayerFactory(firstOpponent, secondOpponent, thirdOpponent);

      var localGameController = new LocalGameControllerCreator().ChangeDice(mockDice).ChangeComputerPlayerFactory(mockPlayerFactory).Create();

      GameBoardData gameBoardData = null;
      localGameController.InitialBoardSetupEvent = (GameBoardData g) => { gameBoardData = g; };

      localGameController.JoinGame();
      localGameController.LaunchGame();

      gameBoardData.ShouldNotBeNull();
    }

    private IComputerPlayerFactory CreateMockPlayerFactory(IComputerPlayer firstOpponent, params IComputerPlayer[] otherOpponents)
    {
      var mockPlayerFactory = Substitute.For<IComputerPlayerFactory>();
      mockPlayerFactory.Create().Returns(firstOpponent, otherOpponents);

      return mockPlayerFactory;
    }

    [Test]
    [Category("LocalGameController")]
    public void CompleteSetupWithPlayerInFirstSlot_ExpectedPlacementsAreReturned()
    {
      var gameSetupOrder = new[] { 12u, 10u, 8u, 6u };
      var mockDice = new MockDiceCreator()
        .AddExplicitDiceRollSequence(gameSetupOrder)
        .Create();

      MockPlayer player;
      MockComputerPlayer firstOpponent, secondOpponent, thirdOpponent;
      this.CreateDefaultPlayerInstances(out player, out firstOpponent, out secondOpponent, out thirdOpponent);

      var localGameController = this.CreateLocalGameController(mockDice, player, firstOpponent, secondOpponent, thirdOpponent);

      GameBoardUpdate gameBoardUpdate = null;
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
      gameBoardUpdate.NewSettlements.ShouldContainKeyAndValue(FirstSettlementOneLocation, firstOpponent.Id);
      gameBoardUpdate.NewSettlements.ShouldContainKeyAndValue(FirstSettlementTwoLocation, firstOpponent.Id);
      gameBoardUpdate.NewSettlements.ShouldContainKeyAndValue(SecondSettlementOneLocation, secondOpponent.Id);
      gameBoardUpdate.NewSettlements.ShouldContainKeyAndValue(SecondSettlementTwoLocation, secondOpponent.Id);
      gameBoardUpdate.NewSettlements.ShouldContainKeyAndValue(ThirdSettlementOneLocation, thirdOpponent.Id);
      gameBoardUpdate.NewSettlements.ShouldContainKeyAndValue(ThirdSettlementTwoLocation, thirdOpponent.Id);

      gameBoardUpdate.NewRoads.Count.ShouldBe(6);
      gameBoardUpdate.NewRoads.ShouldContainKeyAndValue(firstRoadOne, firstOpponent.Id);
      gameBoardUpdate.NewRoads.ShouldContainKeyAndValue(firstRoadTwo, firstOpponent.Id);
      gameBoardUpdate.NewRoads.ShouldContainKeyAndValue(secondRoadOne, secondOpponent.Id);
      gameBoardUpdate.NewRoads.ShouldContainKeyAndValue(secondRoadTwo, secondOpponent.Id);
      gameBoardUpdate.NewRoads.ShouldContainKeyAndValue(thirdRoadOne, thirdOpponent.Id);
      gameBoardUpdate.NewRoads.ShouldContainKeyAndValue(thirdRoadTwo, thirdOpponent.Id);

      gameBoardUpdate = new GameBoardUpdate(); // Ensure that there is a state change for the gameBoardUpdate variable 

      localGameController.CompleteGameSetup(MainSettlementTwoLocation, mainRoadTwo);
      gameBoardUpdate.ShouldBeNull();
    }

    [Test]
    [Category("LocalGameController")]
    public void CompleteSetupWithPlayerInSecondSlot_ExpectedPlacementsAreReturned()
    {
      var gameSetupOrder = new[] { 10u, 12u, 8u, 6u };
      var mockDice = new MockDiceCreator()
        .AddExplicitDiceRollSequence(gameSetupOrder)
        .Create();

      MockPlayer player;
      MockComputerPlayer firstOpponent, secondOpponent, thirdOpponent;
      this.CreateDefaultPlayerInstances(out player, out firstOpponent, out secondOpponent, out thirdOpponent);

      var localGameController = this.CreateLocalGameController(mockDice, player, firstOpponent, secondOpponent, thirdOpponent);

      GameBoardUpdate gameBoardUpdate = null;
      localGameController.GameSetupUpdateEvent = (GameBoardUpdate u) => { gameBoardUpdate = u; };
      localGameController.InitialBoardSetupEvent = (GameBoardData d) => { };

      localGameController.JoinGame();
      localGameController.LaunchGame();

      localGameController.StartGameSetup();
      gameBoardUpdate.ShouldNotBeNull();
      gameBoardUpdate.NewSettlements.Count.ShouldBe(1);
      gameBoardUpdate.NewSettlements.ShouldContainKeyAndValue(FirstSettlementOneLocation, firstOpponent.Id);
      gameBoardUpdate.NewRoads.Count.ShouldBe(1);
      gameBoardUpdate.NewRoads.ShouldContainKeyAndValue(firstRoadOne, firstOpponent.Id);

      gameBoardUpdate = null; // Ensure that there is a state change for the gameBoardUpdate variable 
      localGameController.ContinueGameSetup(MainSettlementOneLocation, mainRoadOne);

      gameBoardUpdate.ShouldNotBeNull();
      gameBoardUpdate.NewSettlements.Count.ShouldBe(4);
      gameBoardUpdate.NewSettlements.ShouldContainKeyAndValue(SecondSettlementOneLocation, secondOpponent.Id);
      gameBoardUpdate.NewSettlements.ShouldContainKeyAndValue(SecondSettlementTwoLocation, secondOpponent.Id);
      gameBoardUpdate.NewSettlements.ShouldContainKeyAndValue(ThirdSettlementOneLocation, thirdOpponent.Id);
      gameBoardUpdate.NewSettlements.ShouldContainKeyAndValue(ThirdSettlementTwoLocation, thirdOpponent.Id);

      gameBoardUpdate.NewRoads.Count.ShouldBe(4);
      gameBoardUpdate.NewRoads.ShouldContainKeyAndValue(secondRoadOne, secondOpponent.Id);
      gameBoardUpdate.NewRoads.ShouldContainKeyAndValue(secondRoadTwo, secondOpponent.Id);
      gameBoardUpdate.NewRoads.ShouldContainKeyAndValue(thirdRoadOne, thirdOpponent.Id);
      gameBoardUpdate.NewRoads.ShouldContainKeyAndValue(thirdRoadTwo, thirdOpponent.Id);

      gameBoardUpdate = null; // Ensure that there is a state change for the gameBoardUpdate variable 
      localGameController.CompleteGameSetup(MainSettlementTwoLocation, mainRoadTwo);

      gameBoardUpdate.NewSettlements.Count.ShouldBe(1);
      gameBoardUpdate.NewSettlements.ShouldContainKeyAndValue(FirstSettlementTwoLocation, firstOpponent.Id);
      gameBoardUpdate.NewRoads.Count.ShouldBe(1);
      gameBoardUpdate.NewRoads.ShouldContainKeyAndValue(firstRoadTwo, firstOpponent.Id);
    }

    [Test]
    [Category("LocalGameController")]
    public void CompleteSetupWithPlayerInThirdSlot_ExpectedPlacementsAreReturned()
    {
      var gameSetupOrders = new[] { 8u, 12u, 10u, 6u };
      var mockDice = new MockDiceCreator()
        .AddExplicitDiceRollSequence(gameSetupOrders)
        .Create();

      MockPlayer player;
      MockComputerPlayer firstOpponent, secondOpponent, thirdOpponent;
      this.CreateDefaultPlayerInstances(out player, out firstOpponent, out secondOpponent, out thirdOpponent);

      var localGameController = this.CreateLocalGameController(mockDice, player, firstOpponent, secondOpponent, thirdOpponent);

      GameBoardUpdate gameBoardUpdate = null;
      localGameController.GameSetupUpdateEvent = (GameBoardUpdate u) => { gameBoardUpdate = u; };
      localGameController.InitialBoardSetupEvent = (GameBoardData d) => { };

      localGameController.JoinGame();
      localGameController.LaunchGame();

      localGameController.StartGameSetup();
      gameBoardUpdate.ShouldNotBeNull();
      gameBoardUpdate.NewSettlements.Count.ShouldBe(2);
      gameBoardUpdate.NewSettlements.ShouldContainKeyAndValue(FirstSettlementOneLocation, firstOpponent.Id);
      gameBoardUpdate.NewSettlements.ShouldContainKeyAndValue(SecondSettlementOneLocation, secondOpponent.Id);
      gameBoardUpdate.NewRoads.Count.ShouldBe(2);
      gameBoardUpdate.NewRoads.ShouldContainKeyAndValue(firstRoadOne, firstOpponent.Id);
      gameBoardUpdate.NewRoads.ShouldContainKeyAndValue(secondRoadOne, secondOpponent.Id);

      gameBoardUpdate = null; // Ensure that there is a state change for the gameBoardUpdate variable 
      localGameController.ContinueGameSetup(MainSettlementOneLocation, mainRoadOne);

      gameBoardUpdate.ShouldNotBeNull();
      gameBoardUpdate.NewSettlements.Count.ShouldBe(2);
      gameBoardUpdate.NewSettlements.ShouldContainKeyAndValue(ThirdSettlementOneLocation, thirdOpponent.Id);
      gameBoardUpdate.NewSettlements.ShouldContainKeyAndValue(ThirdSettlementTwoLocation, thirdOpponent.Id);

      gameBoardUpdate.NewRoads.Count.ShouldBe(2);
      gameBoardUpdate.NewRoads.ShouldContainKeyAndValue(thirdRoadOne, thirdOpponent.Id);
      gameBoardUpdate.NewRoads.ShouldContainKeyAndValue(thirdRoadTwo, thirdOpponent.Id);

      gameBoardUpdate = null; // Ensure that there is a state change for the gameBoardUpdate variable 
      localGameController.CompleteGameSetup(MainSettlementTwoLocation, mainRoadTwo);

      gameBoardUpdate.NewSettlements.Count.ShouldBe(2);
      gameBoardUpdate.NewSettlements.ShouldContainKeyAndValue(SecondSettlementTwoLocation, secondOpponent.Id);
      gameBoardUpdate.NewSettlements.ShouldContainKeyAndValue(FirstSettlementTwoLocation, firstOpponent.Id);
      gameBoardUpdate.NewRoads.Count.ShouldBe(2);
      gameBoardUpdate.NewRoads.ShouldContainKeyAndValue(secondRoadTwo, secondOpponent.Id);
      gameBoardUpdate.NewRoads.ShouldContainKeyAndValue(firstRoadTwo, firstOpponent.Id);
    }

    [Test]
    [Category("LocalGameController")]
    public void CompleteSetupWithPlayerInFourthSlot_ExpectedPlacementsAreReturned()
    {
      var gameSetupOrder = new[] { 6u, 12u, 10u, 8u };
      var mockDice = new MockDiceCreator()
        .AddExplicitDiceRollSequence(gameSetupOrder)
        .Create();

      MockPlayer player;
      MockComputerPlayer firstOpponent, secondOpponent, thirdOpponent;
      this.CreateDefaultPlayerInstances(out player, out firstOpponent, out secondOpponent, out thirdOpponent);

      var localGameController = this.CreateLocalGameController(mockDice, player, firstOpponent, secondOpponent, thirdOpponent);

      GameBoardUpdate gameBoardUpdate = null;
      localGameController.GameSetupUpdateEvent = (GameBoardUpdate u) => { gameBoardUpdate = u; };
      localGameController.InitialBoardSetupEvent = (GameBoardData d) => { };

      localGameController.JoinGame();
      localGameController.LaunchGame();

      localGameController.StartGameSetup();
      gameBoardUpdate.ShouldNotBeNull();
      gameBoardUpdate.NewSettlements.Count.ShouldBe(3);
      gameBoardUpdate.NewSettlements.ShouldContainKeyAndValue(FirstSettlementOneLocation, firstOpponent.Id);
      gameBoardUpdate.NewSettlements.ShouldContainKeyAndValue(SecondSettlementOneLocation, secondOpponent.Id);
      gameBoardUpdate.NewSettlements.ShouldContainKeyAndValue(ThirdSettlementOneLocation, thirdOpponent.Id);
      gameBoardUpdate.NewRoads.Count.ShouldBe(3);
      gameBoardUpdate.NewRoads.ShouldContainKeyAndValue(firstRoadOne, firstOpponent.Id);
      gameBoardUpdate.NewRoads.ShouldContainKeyAndValue(secondRoadOne, secondOpponent.Id);
      gameBoardUpdate.NewRoads.ShouldContainKeyAndValue(thirdRoadOne, thirdOpponent.Id);

      gameBoardUpdate = new GameBoardUpdate(); // Ensure that there is a state change for the gameBoardUpdate variable 
      localGameController.ContinueGameSetup(MainSettlementOneLocation, mainRoadOne);
      gameBoardUpdate.ShouldBeNull();

      gameBoardUpdate = null; // Ensure that there is a state change for the gameBoardUpdate variable 
      localGameController.CompleteGameSetup(MainSettlementTwoLocation, mainRoadTwo);

      gameBoardUpdate.ShouldNotBeNull();
      gameBoardUpdate.NewSettlements.Count.ShouldBe(3);
      gameBoardUpdate.NewSettlements.ShouldContainKeyAndValue(ThirdSettlementTwoLocation, thirdOpponent.Id);
      gameBoardUpdate.NewSettlements.ShouldContainKeyAndValue(SecondSettlementTwoLocation, secondOpponent.Id);
      gameBoardUpdate.NewSettlements.ShouldContainKeyAndValue(FirstSettlementTwoLocation, firstOpponent.Id);
      gameBoardUpdate.NewRoads.Count.ShouldBe(3);
      gameBoardUpdate.NewRoads.ShouldContainKeyAndValue(thirdRoadTwo, thirdOpponent.Id);
      gameBoardUpdate.NewRoads.ShouldContainKeyAndValue(secondRoadTwo, secondOpponent.Id);
      gameBoardUpdate.NewRoads.ShouldContainKeyAndValue(firstRoadTwo, firstOpponent.Id);
    }
    
    [Test]
    [Category("LocalGameController")]
    public void CompleteSetupWithPlayer_ExpectedResourcesAreReturned()
    {
      var mockDice = new MockDiceCreator()
        .AddRandomSequenceWithNoDuplicates(4)
        .Create();

      MockPlayer player;
      MockComputerPlayer firstOpponent, secondOpponent, thirdOpponent;
      this.CreateDefaultPlayerInstances(out player, out firstOpponent, out secondOpponent, out thirdOpponent);

      var localGameController = this.CreateLocalGameController(mockDice, player, firstOpponent, secondOpponent, thirdOpponent);

      ResourceUpdate resourceUpdate = null;
      localGameController.GameSetupResourcesEvent = (ResourceUpdate r) => { resourceUpdate = r; };

      localGameController.JoinGame();
      localGameController.LaunchGame();
      localGameController.StartGameSetup();
      localGameController.ContinueGameSetup(MainSettlementOneLocation, mainRoadOne);
      localGameController.CompleteGameSetup(MainSettlementTwoLocation, mainRoadTwo);

      resourceUpdate.ShouldNotBeNull();
      resourceUpdate.Resources.Count.ShouldBe(4);
      resourceUpdate.Resources.ShouldContainKey(player.Id);
      resourceUpdate.Resources.ShouldContainKey(firstOpponent.Id);
      resourceUpdate.Resources.ShouldContainKey(secondOpponent.Id);
      resourceUpdate.Resources.ShouldContainKey(thirdOpponent.Id);

      resourceUpdate.Resources[player.Id].BrickCount.ShouldBe(1u);
      resourceUpdate.Resources[player.Id].GrainCount.ShouldBe(1u);
      resourceUpdate.Resources[player.Id].LumberCount.ShouldBe(0u);
      resourceUpdate.Resources[player.Id].OreCount.ShouldBe(0u);
      resourceUpdate.Resources[player.Id].WoolCount.ShouldBe(1u);

      resourceUpdate.Resources[firstOpponent.Id].BrickCount.ShouldBe(0u);
      resourceUpdate.Resources[firstOpponent.Id].GrainCount.ShouldBe(1u);
      resourceUpdate.Resources[firstOpponent.Id].LumberCount.ShouldBe(1u);
      resourceUpdate.Resources[firstOpponent.Id].OreCount.ShouldBe(0u);
      resourceUpdate.Resources[firstOpponent.Id].WoolCount.ShouldBe(1u);

      resourceUpdate.Resources[secondOpponent.Id].BrickCount.ShouldBe(0u);
      resourceUpdate.Resources[secondOpponent.Id].GrainCount.ShouldBe(0u);
      resourceUpdate.Resources[secondOpponent.Id].LumberCount.ShouldBe(1u);
      resourceUpdate.Resources[secondOpponent.Id].OreCount.ShouldBe(1u);
      resourceUpdate.Resources[secondOpponent.Id].WoolCount.ShouldBe(1u);

      resourceUpdate.Resources[thirdOpponent.Id].BrickCount.ShouldBe(0u);
      resourceUpdate.Resources[thirdOpponent.Id].GrainCount.ShouldBe(1u);
      resourceUpdate.Resources[thirdOpponent.Id].LumberCount.ShouldBe(1u);
      resourceUpdate.Resources[thirdOpponent.Id].OreCount.ShouldBe(0u);
      resourceUpdate.Resources[thirdOpponent.Id].WoolCount.ShouldBe(1u);
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

      MockPlayer player;
      MockComputerPlayer firstOpponent, secondOpponent, thirdOpponent;
      this.CreateDefaultPlayerInstances(out player, out firstOpponent, out secondOpponent, out thirdOpponent);

      var localGameController = this.CreateLocalGameController(mockDice, player, firstOpponent, secondOpponent, thirdOpponent);

      ErrorDetails errorDetails = null;
      GameBoardUpdate gameBoardUpdate = null;
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

      MockPlayer player;
      MockComputerPlayer firstOpponent, secondOpponent, thirdOpponent;
      this.CreateDefaultPlayerInstances(out player, out firstOpponent, out secondOpponent, out thirdOpponent);

      var localGameController = this.CreateLocalGameController(mockDice, player, firstOpponent, secondOpponent, thirdOpponent);

      ErrorDetails exception = null;
      localGameController.ErrorRaisedEvent = (ErrorDetails e) => { exception = e; };

      localGameController.JoinGame();
      localGameController.LaunchGame();
      localGameController.StartGameSetup();

      GameBoardUpdate gameBoardUpdate = null;
      localGameController.GameSetupUpdateEvent = (GameBoardUpdate u) => { gameBoardUpdate = u; };

      localGameController.ContinueGameSetup(FirstSettlementOneLocation, new Road(0u, 1u));
      exception.ShouldNotBeNull();
      exception.Message.ShouldBe("Cannot place settlement: Location " + FirstSettlementOneLocation + " already owned by player " + firstOpponent.Id);
      gameBoardUpdate.ShouldBeNull();
    }

    [Test]
    [Category("LocalGameController")]
    public void PlayerSelectsSameLocationAsComputerPlayerDuringSecondSetupRound_MeaningfulErrorDetailsPassedBack()
    {
      var mockDice = Substitute.For<IDice>();
      mockDice.RollTwoDice().Returns(12u, 10u, 8u, 6u);

      MockPlayer player;
      MockComputerPlayer firstOpponent, secondOpponent, thirdOpponent;
      this.CreateDefaultPlayerInstances(out player, out firstOpponent, out secondOpponent, out thirdOpponent);

      var localGameController = this.CreateLocalGameController(mockDice, player, firstOpponent, secondOpponent, thirdOpponent);

      ErrorDetails exception = null;
      localGameController.ErrorRaisedEvent = (ErrorDetails e) => { exception = e; };

      localGameController.JoinGame();
      localGameController.LaunchGame();
      localGameController.StartGameSetup();
      localGameController.ContinueGameSetup(0, new Road(0, 1));

      GameBoardUpdate gameBoardUpdate = null;
      localGameController.GameSetupUpdateEvent = (GameBoardUpdate u) => { gameBoardUpdate = u; };

      localGameController.CompleteGameSetup(FirstSettlementOneLocation, new Road(0, 1));

      exception.ShouldNotBeNull();
      exception.Message.ShouldBe("Cannot place settlement: Location " + FirstSettlementOneLocation + " already owned by player " + firstOpponent.Id);
      gameBoardUpdate.ShouldBeNull();
    }

    [Test]
    [Category("LocalGameController")]
    public void PlayerSelectsLocationTooCloseToComputerPlayerDuringFirstSetupRound_MeaningfulErrorDetailsPassedBack()
    {
      var mockDice = Substitute.For<IDice>();
      mockDice.RollTwoDice().Returns(10u, 12u, 8u, 6u);

      MockPlayer player;
      MockComputerPlayer firstOpponent, secondOpponent, thirdOpponent;
      this.CreateDefaultPlayerInstances(out player, out firstOpponent, out secondOpponent, out thirdOpponent);

      var localGameController = this.CreateLocalGameController(mockDice, player, firstOpponent, secondOpponent, thirdOpponent);

      ErrorDetails exception = null;
      localGameController.ErrorRaisedEvent = (ErrorDetails e) => { exception = e; };
      localGameController.JoinGame();
      localGameController.LaunchGame();
      localGameController.StartGameSetup();

      GameBoardUpdate gameBoardUpdate = null;
      localGameController.GameSetupUpdateEvent = (GameBoardUpdate u) => { gameBoardUpdate = u; };

      localGameController.ContinueGameSetup(19u, new Road(0u, 1u));

      exception.ShouldNotBeNull();
      exception.Message.ShouldBe("Cannot place settlement: Too close to player " + firstOpponent.Id + " at location " + FirstSettlementOneLocation);
      gameBoardUpdate.ShouldBeNull();
    }

    [Test]
    [Category("LocalGameController")]
    public void PlayerSelectsLocationTooCloseToComputerPlayerDuringSecondSetupRound_MeaningfulErrorDetailsPassedBack()
    {
      var mockDice = Substitute.For<IDice>();
      mockDice.RollTwoDice().Returns(12u, 10u, 8u, 6u);

      MockPlayer player;
      MockComputerPlayer firstOpponent, secondOpponent, thirdOpponent;
      this.CreateDefaultPlayerInstances(out player, out firstOpponent, out secondOpponent, out thirdOpponent);

      var localGameController = this.CreateLocalGameController(mockDice, player, firstOpponent, secondOpponent, thirdOpponent);

      ErrorDetails exception = null;
      localGameController.ErrorRaisedEvent = (ErrorDetails e) => { exception = e; };
      localGameController.JoinGame();
      localGameController.LaunchGame();
      localGameController.StartGameSetup();
      localGameController.ContinueGameSetup(0, new Road(0, 1));

      GameBoardUpdate gameBoardUpdate = null;
      localGameController.GameSetupUpdateEvent = (GameBoardUpdate u) => { gameBoardUpdate = u; };

      localGameController.CompleteGameSetup(19, new Road(19, 18));

      exception.ShouldNotBeNull();
      exception.Message.ShouldBe("Cannot place settlement: Too close to player " + firstOpponent.Id + " at location " + FirstSettlementOneLocation);
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
    public void FinalisePlayerTurnOrder_CallOutOfSequence_MeaningfulErrorDetailsPassedBack()
    {
      var localGameController = this.CreateLocalGameController();
      ErrorDetails errorDetails = null;
      localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

      localGameController.FinalisePlayerTurnOrder();

      errorDetails.ShouldNotBeNull();
      errorDetails.Message.ShouldBe("Cannot call 'FinalisePlayerTurnOrder' until 'CompleteGameSetup' has completed.");
    }

    [Test]
    [Category("LocalGameController")]
    public void FinalisePlayerTurnOrder_RollsAscending_ReceiveTurnOrderForMainGameLoop()
    {
      // Arrange
      var gameSetupOrder = new[] { 12u, 10u, 8u, 6u };
      var gameTurnOrder = new[] { 6u, 8u, 10u, 12u };
      var mockDice = new MockDiceCreator()
        .AddExplicitDiceRollSequence(gameSetupOrder)
        .AddExplicitDiceRollSequence(gameTurnOrder)
        .Create();

      MockPlayer player;
      MockComputerPlayer firstOpponent, secondOpponent, thirdOpponent;
      this.CreateDefaultPlayerInstances(out player, out firstOpponent, out secondOpponent, out thirdOpponent);

      var localGameController = this.CreateLocalGameController(mockDice, player, firstOpponent, secondOpponent, thirdOpponent);

      localGameController.JoinGame();
      localGameController.LaunchGame();
      localGameController.StartGameSetup();
      localGameController.ContinueGameSetup(MainSettlementOneLocation, mainRoadOne);
      localGameController.CompleteGameSetup(MainSettlementTwoLocation, mainRoadTwo);

      // Act
      PlayerDataView[] turnOrder = null;
      localGameController.TurnOrderFinalisedEvent = (PlayerDataView[] p) => { turnOrder = p; };
      localGameController.FinalisePlayerTurnOrder();

      // Assert
      turnOrder.ShouldNotBeNull();
      turnOrder.Length.ShouldBe(4);

      this.AssertPlayerDataViewIsCorrect(player, turnOrder[3]);
      this.AssertPlayerDataViewIsCorrect(firstOpponent, turnOrder[2]);
      this.AssertPlayerDataViewIsCorrect(secondOpponent, turnOrder[1]);
      this.AssertPlayerDataViewIsCorrect(thirdOpponent, turnOrder[0]);
    }

    [Test]
    [Category("LocalGameController")]
    public void FinalisePlayerTurnOrder_RollsDescending_ReceiveTurnOrderForMainGameLoop()
    {
      // Arrange
      var gameSetupOrder = new[] { 12u, 10u, 8u, 6u };
      var gameTurnOrder = gameSetupOrder;
      var mockDice = new MockDiceCreator()
        .AddExplicitDiceRollSequence(gameSetupOrder)
        .AddExplicitDiceRollSequence(gameTurnOrder)
        .Create();

      MockPlayer player;
      MockComputerPlayer firstOpponent, secondOpponent, thirdOpponent;
      this.CreateDefaultPlayerInstances(out player, out firstOpponent, out secondOpponent, out thirdOpponent);

      var localGameController = this.CreateLocalGameController(mockDice, player, firstOpponent, secondOpponent, thirdOpponent);

      localGameController.JoinGame();
      localGameController.LaunchGame();
      localGameController.StartGameSetup();
      localGameController.ContinueGameSetup(MainSettlementOneLocation, mainRoadOne);
      localGameController.CompleteGameSetup(MainSettlementTwoLocation, mainRoadTwo);

      // Act
      PlayerDataView[] turnOrder = null;
      localGameController.TurnOrderFinalisedEvent = (PlayerDataView[] p) => { turnOrder = p; };
      localGameController.FinalisePlayerTurnOrder();

      // Assert
      turnOrder.ShouldNotBeNull();
      turnOrder.Length.ShouldBe(4);

      this.AssertPlayerDataViewIsCorrect(player, turnOrder[0]);
      this.AssertPlayerDataViewIsCorrect(firstOpponent, turnOrder[1]);
      this.AssertPlayerDataViewIsCorrect(secondOpponent, turnOrder[2]);
      this.AssertPlayerDataViewIsCorrect(thirdOpponent, turnOrder[3]);
    }

    [Test]
    [Category("LocalGameController")]
    public void StartOfMainPlayerTurn_TurnTokenReceived()
    {
      // Arrange
      MockDice mockDice = null;
      Guid id = Guid.Empty;
      MockPlayer player;
      MockComputerPlayer firstComputerPlayer, secondComputerPlayer, thirdComputerPlayer;
      var localGameController = this.CreateLocalGameControllerAndCompleteGameSetup(out mockDice, out player, out firstComputerPlayer, out secondComputerPlayer, out thirdComputerPlayer);

      mockDice.AddSequence(new[] { 8u });

      // Act
      var turnToken = Guid.Empty;
      localGameController.StartPlayerTurnEvent = (Guid t) => { turnToken = t; };
      localGameController.StartGamePlay();

      // Assert
      turnToken.ShouldNotBe(Guid.Empty);
    }

    [Test]
    [Category("LocalGameController")]
    public void StartOfMainPlayerTurn_DiceRollReceived()
    {
      // Arrange
      MockDice mockDice = null;
      Guid id = Guid.Empty;
      MockPlayer player;
      MockComputerPlayer firstComputerPlayer, secondComputerPlayer, thirdComputerPlayer;
      var localGameController = this.CreateLocalGameControllerAndCompleteGameSetup(out mockDice, out player, out firstComputerPlayer, out secondComputerPlayer, out thirdComputerPlayer);
      mockDice.AddSequence(new[] { 8u });

      // Act
      var diceRoll = 0u;
      localGameController.DiceRollEvent = (UInt32 d) => { diceRoll = d; };
      localGameController.StartGamePlay();

      // Assert
      diceRoll.ShouldBe(8u);
    }

    [Test]
    [Category("LocalGameController")]
    public void StartOfMainPlayerTurn_DoesNotRollSeven_ReceiveResourceDetails()
    {
      MockDice mockDice = null;
      Guid id = Guid.Empty;
      MockPlayer player;
      MockComputerPlayer firstOpponent, secondOpponent, thirdOpponent;
      var localGameController = this.CreateLocalGameControllerAndCompleteGameSetup(out mockDice, out player, out firstOpponent, out secondOpponent, out thirdOpponent);

      mockDice.AddSequence(new[] { 8u });

      ResourceUpdate resourceUpdate = null;
      localGameController.ResourcesCollectedEvent = (ResourceUpdate r) => { resourceUpdate = r; };
      localGameController.StartGamePlay();

      resourceUpdate.ShouldNotBeNull();
      resourceUpdate.Resources.Count.ShouldBe(2);
      resourceUpdate.Resources[id].BrickCount.ShouldBe(1u);
      resourceUpdate.Resources[id].GrainCount.ShouldBe(0u);
      resourceUpdate.Resources[id].LumberCount.ShouldBe(0u);
      resourceUpdate.Resources[id].OreCount.ShouldBe(0u);
      resourceUpdate.Resources[id].WoolCount.ShouldBe(0u);

      resourceUpdate.Resources[firstOpponent.Id].BrickCount.ShouldBe(0u);
      resourceUpdate.Resources[firstOpponent.Id].GrainCount.ShouldBe(1u);
      resourceUpdate.Resources[firstOpponent.Id].LumberCount.ShouldBe(0u);
      resourceUpdate.Resources[firstOpponent.Id].OreCount.ShouldBe(0u);
      resourceUpdate.Resources[firstOpponent.Id].WoolCount.ShouldBe(0u);
    }

    [Test]
    [Category("LocalGameController")]
    public void StartOfMainPlayerTurn_RollsSeven_ReceiveResourceCardLossesForComputerPlayers()
    {
      // Assert
      MockDice mockDice = null;
      Guid mainPlayerId;
      MockComputerPlayer firstComputerPlayer, secondComputerPlayer, thirdComputerPlayer;
      var localGameController = this.CreateLocalGameControllerAndCompleteGameSetup2(out mockDice, out mainPlayerId, out firstComputerPlayer, out secondComputerPlayer, out thirdComputerPlayer);
      mockDice.AddSequence(new[] { 7u });
      firstComputerPlayer.ResourceCards = 8;
      secondComputerPlayer.ResourceCards = 7;
      thirdComputerPlayer.ResourceCards = 9;

      // Act
      Dictionary<Guid, UInt32> resourcesLost = null;
      localGameController.ResourcesLostEvent = (Dictionary<Guid, UInt32> r) => { resourcesLost = r; };
      localGameController.StartGamePlay();

      // Assert
      resourcesLost.Count.ShouldBe(3);
      resourcesLost.ShouldContainKeyAndValue(firstComputerPlayer.Id, 4u);
      resourcesLost.ShouldContainKeyAndValue(secondComputerPlayer.Id, 0u);
      resourcesLost.ShouldContainKeyAndValue(thirdComputerPlayer.Id, 4u);
    }

    private LocalGameController CreateLocalGameControllerAndCompleteGameSetup(out MockDice mockDice, out MockPlayer player, out MockComputerPlayer firstOpponent, out MockComputerPlayer secondOpponent, out MockComputerPlayer thirdOpponent)
    {
      var gameSetupOrder = new[] { 12u, 10u, 8u, 6u };
      var gameTurnOrder = gameSetupOrder;
      mockDice = new MockDiceCreator()
        .AddExplicitDiceRollSequence(gameSetupOrder)
        .AddExplicitDiceRollSequence(gameTurnOrder)
        .Create();

      this.CreateDefaultPlayerInstances(out player, out firstOpponent, out secondOpponent, out thirdOpponent);

      var localGameController = this.CreateLocalGameController(mockDice, player, firstOpponent, secondOpponent, thirdOpponent);

      localGameController.JoinGame();
      localGameController.LaunchGame();
      localGameController.StartGameSetup();
      localGameController.ContinueGameSetup(MainSettlementOneLocation, mainRoadOne);
      localGameController.CompleteGameSetup(MainSettlementTwoLocation, mainRoadTwo);
      localGameController.FinalisePlayerTurnOrder();

      return localGameController;
    }

    private LocalGameController CreateLocalGameControllerAndCompleteGameSetup2(out MockDice mockDice, out Guid mainPlayerId, out MockComputerPlayer firstComputerPlayer, out MockComputerPlayer secondComputerPlayer, out MockComputerPlayer thirdComputerPlayer)
    {
      var gameSetupOrder = new[] { 12u, 10u, 8u, 6u };
      var gameTurnOrder = gameSetupOrder;
      mockDice = new MockDiceCreator()
        .AddExplicitDiceRollSequence(gameSetupOrder)
        .AddExplicitDiceRollSequence(gameTurnOrder)
        .Create();

      var gameBoardManager = new GameBoardManager(BoardSizes.Standard);

      firstComputerPlayer = this.CreateMockComputerPlayer(FirstOpponentName, FirstSettlementOneLocation, FirstSettlementTwoLocation, firstRoadOne, firstRoadTwo);
      secondComputerPlayer = this.CreateMockComputerPlayer(SecondOpponentName, SecondSettlementOneLocation, SecondSettlementTwoLocation, secondRoadOne, secondRoadTwo);
      thirdComputerPlayer = this.CreateMockComputerPlayer(ThirdOpponentName, ThirdSettlementOneLocation, ThirdSettlementTwoLocation, thirdRoadOne, thirdRoadTwo);

      var localGameController = this.CreateLocalGameController(mockDice, gameBoardManager, firstComputerPlayer, secondComputerPlayer, thirdComputerPlayer);

      var id = Guid.Empty;
      localGameController.GameJoinedEvent = (PlayerDataView[] p) => { id = p[0].Id; };

      localGameController.JoinGame();
      localGameController.LaunchGame();
      localGameController.StartGameSetup();
      localGameController.ContinueGameSetup(MainSettlementOneLocation, mainRoadOne);
      localGameController.CompleteGameSetup(MainSettlementTwoLocation, mainRoadTwo);
      localGameController.FinalisePlayerTurnOrder();

      mainPlayerId = id;

      return localGameController;
    }

    private void AssertPlayerDataViewIsCorrect(IPlayer player, PlayerDataView playerDataView)
    {
      playerDataView.Id.ShouldBe(player.Id);
      playerDataView.Name.ShouldBe(player.Name);
      playerDataView.DisplayedDevelopmentCards.ShouldBeNull();
      playerDataView.HiddenDevelopmentCards.ShouldBe(0u);
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

    private LocalGameController CreateLocalGameController(IDice dice, IPlayer firstPlayer, params IPlayer[] otherPlayers)
    {
      var mockComputerPlayerFactory = CreatePlayerFactory(firstPlayer, otherPlayers);

      return new LocalGameControllerCreator()
        .ChangeDice(dice)
        .ChangeComputerPlayerFactory(mockComputerPlayerFactory)
        .Create();
    }

    private IComputerPlayerFactory CreatePlayerFactory(IPlayer player, IPlayer[] otherPlayers)
    {
      var mockComputerPlayerFactory = Substitute.For<IComputerPlayerFactory>();
      mockComputerPlayerFactory.Create().Returns(player, otherPlayers);
      return mockComputerPlayerFactory;
    }

    private LocalGameController CreateLocalGameControllerWithMainPlayerGoingFirstInSetup()
    {
      var mockDice = Substitute.For<IDice>();
      mockDice.RollTwoDice().Returns(12u, 10u, 8u, 6u);

      MockPlayer player;
      MockComputerPlayer firstOpponent, secondOpponent, thirdOpponent;
      this.CreateDefaultPlayerInstances(out player, out firstOpponent, out secondOpponent, out thirdOpponent);

      return this.CreateLocalGameController(mockDice, player, firstOpponent, secondOpponent, thirdOpponent);
    }

    private MockComputerPlayer CreateMockComputerPlayer(String name, UInt32 settlementOneLocation, UInt32 settlementTwoLocation, Road roadOne, Road roadTwo)
    {
      var computerPlayer = new MockComputerPlayer(name);
      computerPlayer.SettlementLocations = new Queue<uint>(new uint[] { settlementOneLocation, settlementTwoLocation });
      computerPlayer.Roads = new Queue<Road>(new Road[] { roadOne, roadTwo });
      return computerPlayer;
    }

    private void CreateDefaultPlayerInstances(out MockPlayer player, out MockComputerPlayer firstOpponent, out MockComputerPlayer secondOpponent, out MockComputerPlayer thirdOpponent)
    {
      player = new MockPlayer(PlayerName);

      firstOpponent = new MockComputerPlayer(FirstOpponentName);
      firstOpponent.SettlementLocations = new Queue<UInt32>(new[] { FirstSettlementOneLocation, FirstSettlementTwoLocation });
      firstOpponent.Roads = new Queue<Road>(new[] { firstRoadOne, firstRoadTwo });

      secondOpponent = new MockComputerPlayer(SecondOpponentName);
      secondOpponent.SettlementLocations = new Queue<UInt32>(new[] { SecondSettlementOneLocation, SecondSettlementTwoLocation });
      secondOpponent.Roads = new Queue<Road>(new[] { secondRoadOne, secondRoadTwo });

      thirdOpponent = new MockComputerPlayer(ThirdOpponentName);
      thirdOpponent.SettlementLocations = new Queue<UInt32>(new[] { ThirdSettlementOneLocation, ThirdSettlementTwoLocation });
      thirdOpponent.Roads = new Queue<Road>(new[] { thirdRoadOne, thirdRoadTwo });
    }

    private void RunOpponentDataPassBackTests(GameOptions gameOptions)
    {
      var player = new MockPlayer(PlayerName);
      var firstOpponent = new MockComputerPlayer(FirstOpponentName);
      var secondOpponent = new MockComputerPlayer(SecondOpponentName);
      var thirdOpponent = new MockComputerPlayer(ThirdOpponentName);

      var mockComputerGameFactory = Substitute.For<IComputerPlayerFactory>();
      mockComputerGameFactory.Create().Returns(player, firstOpponent, secondOpponent, thirdOpponent);
      var localGameController = new LocalGameControllerCreator()
        .ChangeComputerPlayerFactory(mockComputerGameFactory)
        .Create();

      PlayerDataView[] playerDataViews = null;
      localGameController.GameJoinedEvent = (PlayerDataView[] p) => { playerDataViews = p; };
      localGameController.JoinGame(gameOptions);

      playerDataViews.ShouldNotBeNull();
      playerDataViews.Length.ShouldBe(4);

      this.AssertPlayerDataViewIsCorrect(player, playerDataViews[0]);
      this.AssertPlayerDataViewIsCorrect(firstOpponent, playerDataViews[1]);
      this.AssertPlayerDataViewIsCorrect(secondOpponent, playerDataViews[2]);
      this.AssertPlayerDataViewIsCorrect(thirdOpponent, playerDataViews[3]);
    }
    #endregion
  }

  public class LocalGameContext
  {
    public IComputerPlayer FirstComputerPlayer { get; private set; }
    public IComputerPlayer SecondComputerPlayer { get; private set; }
    public IComputerPlayer ThirdComputerPlayer { get; private set; }
    public MockDice Dice { get; private set; }
    public GameBoardData GameBoard { get; private set; }
    public LocalGameController GameController { get; private set; }

    public LocalGameContext(MockDice dice, IComputerPlayer firstComputerPlayer, IComputerPlayer secondComputerPlayer, IComputerPlayer thirdComputerPlayer)
    {
      this.Dice = dice;
      this.FirstComputerPlayer = firstComputerPlayer;
      this.SecondComputerPlayer = secondComputerPlayer;
      this.ThirdComputerPlayer = thirdComputerPlayer;

      var gameBoardManager = new GameBoardManager(BoardSizes.Standard);
      this.GameBoard = gameBoardManager.Data;

      var mockComputerPlayerFactory = Substitute.For<IComputerPlayerFactory>();
      mockComputerPlayerFactory.Create().Returns(firstComputerPlayer, secondComputerPlayer, thirdComputerPlayer);

      this.GameController = new LocalGameController(this.Dice, mockComputerPlayerFactory, gameBoardManager);
    }

    public void CompleteGameSetup(UInt32 firstSettlmentLocation, Road firstRoad, UInt32 secondSettlementLocation, Road secondRoad)
    {
      this.GameController.JoinGame();
      this.GameController.LaunchGame();
      this.GameController.StartGameSetup();
      this.GameController.ContinueGameSetup(firstSettlmentLocation, firstRoad);
      this.GameController.CompleteGameSetup(secondSettlementLocation, secondRoad);
      this.GameController.FinalisePlayerTurnOrder();
    }
  }
}
