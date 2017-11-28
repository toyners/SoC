
namespace Jabberwocky.SoC.Library.UnitTests
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Text;
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

    private const UInt32 MainRoadOneEnd = 4;
    private const UInt32 FirstRoadOneEnd = 17;
    private const UInt32 SecondRoadOneEnd = 15;
    private const UInt32 ThirdRoadOneEnd = 30;

    private const UInt32 ThirdRoadTwoEnd = 32;
    private const UInt32 SecondRoadTwoEnd = 24;
    private const UInt32 FirstRoadTwoEnd = 44;
    private const UInt32 MainRoadTwoEnd = 39;

    //private RoadSegment mainRoadOne = new RoadSegment(12, 4);
    //private RoadSegment firstRoadOne = new RoadSegment(17, 18);
    //private RoadSegment secondRoadOne = new RoadSegment(15, 25);
    //private RoadSegment thirdRoadOne = new RoadSegment(30, 31);

    //private RoadSegment thirdRoadTwo = new RoadSegment(32, 33);
    //private RoadSegment secondRoadTwo = new RoadSegment(24, 35);
    //private RoadSegment firstRoadTwo = new RoadSegment(43, 44);
    //private RoadSegment mainRoadTwo = new RoadSegment(40, 39);
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
    public void TryingToJoinGameMoreThanOnce_MeaningfulErrorIsReceived()
    {
      var player = new MockPlayer(PlayerName);
      var opponents = new[] 
      {
        new MockComputerPlayer(FirstOpponentName),
        new MockComputerPlayer(SecondOpponentName),
        new MockComputerPlayer(ThirdOpponentName)
      };

      var mockPlayerFactory = this.CreatePlayerPool(player, opponents);

      var localGameController = new LocalGameControllerCreator().ChangePlayerPool(mockPlayerFactory).Create();
      ErrorDetails errorDetails = null;
      localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };
      localGameController.JoinGame();
      localGameController.JoinGame();

      errorDetails.ShouldNotBeNull();
      errorDetails.Message.ShouldBe("Cannot call 'JoinGame' more than once.");
    }

    [Test]
    [Category("LocalGameController")]
    public void LaunchGame_LaunchGameWithoutJoining_MeaningfulErrorIsReceived()
    {
      var localGameController = new LocalGameControllerCreator().Create();
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

      var player = new MockPlayer(PlayerName);
      var opponents = new[] 
      {
        new MockComputerPlayer(FirstOpponentName),
        new MockComputerPlayer(SecondOpponentName),
        new MockComputerPlayer(ThirdOpponentName)
      };

      var mockPlayerFactory = this.CreatePlayerPool(player, opponents);

      var localGameController = new LocalGameControllerCreator().ChangeDice(mockDice).ChangePlayerPool(mockPlayerFactory).Create();

      GameBoardData gameBoardData = null;
      localGameController.InitialBoardSetupEvent = (GameBoardData g) => { gameBoardData = g; };

      localGameController.JoinGame();
      localGameController.LaunchGame();

      gameBoardData.ShouldNotBeNull();
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

      localGameController.ContinueGameSetup(MainSettlementOneLocation, MainRoadOneEnd);
      gameBoardUpdate.ShouldNotBeNull();
      this.VerifyNewSettlements(gameBoardUpdate.NewSettlements,
        new Tuple<UInt32, Guid>(FirstSettlementOneLocation, firstOpponent.Id),
        new Tuple<UInt32, Guid>(FirstSettlementTwoLocation, firstOpponent.Id),
        new Tuple<UInt32, Guid>(SecondSettlementOneLocation, secondOpponent.Id),
        new Tuple<UInt32, Guid>(SecondSettlementTwoLocation, secondOpponent.Id),
        new Tuple<UInt32, Guid>(ThirdSettlementOneLocation, thirdOpponent.Id),
        new Tuple<UInt32, Guid>(ThirdSettlementTwoLocation, thirdOpponent.Id));

      this.VerifyNewRoads(gameBoardUpdate.NewRoads,
        new Tuple<UInt32, UInt32, Guid>(FirstSettlementOneLocation, FirstRoadOneEnd, firstOpponent.Id),
        new Tuple<UInt32, UInt32, Guid>(FirstSettlementTwoLocation, FirstRoadTwoEnd, firstOpponent.Id),
        new Tuple<UInt32, UInt32, Guid>(SecondSettlementOneLocation, SecondRoadOneEnd, secondOpponent.Id),
        new Tuple<UInt32, UInt32, Guid>(SecondSettlementTwoLocation, SecondRoadTwoEnd, secondOpponent.Id),
        new Tuple<UInt32, UInt32, Guid>(ThirdSettlementOneLocation, ThirdRoadOneEnd, thirdOpponent.Id),
        new Tuple<UInt32, UInt32, Guid>(ThirdSettlementTwoLocation, ThirdRoadTwoEnd, thirdOpponent.Id));

      gameBoardUpdate = new GameBoardUpdate(); // Ensure that there is a state change for the gameBoardUpdate variable 

      localGameController.CompleteGameSetup(MainSettlementTwoLocation, MainRoadTwoEnd);
      gameBoardUpdate.ShouldBeNull();
    }

    private void VerifyNewSettlements(List<Tuple<UInt32, Guid>> actualSettlements, params Tuple<UInt32, Guid>[] expectedSettlements)
    {
      actualSettlements.Count.ShouldBe(expectedSettlements.Length);

      foreach(var expectedSettlement in expectedSettlements)
      {
        var foundExpectedSettlement = false;
        for (var index = 0; index < actualSettlements.Count; index++)
        {
          var actualSettlement = actualSettlements[index];

          if (actualSettlement.Item1 == expectedSettlement.Item1 &&
              actualSettlement.Item2 == expectedSettlement.Item2)
          {
            actualSettlements.RemoveAt(index);
            index--;
            foundExpectedSettlement = true;
            break;
          }
        }

        if (!foundExpectedSettlement)
        {
          throw new Exception("Did not find expected settlement: " + expectedSettlement.ToString());
        }
      }
    }

    private void VerifyNewRoads(List<Tuple<UInt32, UInt32, Guid>> actualRoads, params Tuple<UInt32, UInt32, Guid>[] expectedRoads)
    {
      actualRoads.Count.ShouldBe(expectedRoads.Length);

      foreach (var expectedRoad in expectedRoads)
      {
        var foundExpectedRoad = false;
        for (var index = 0; index < actualRoads.Count; index++)
        {
          var actualRoad = actualRoads[index];

          if (actualRoad.Item1 == expectedRoad.Item1 &&
              actualRoad.Item2 == expectedRoad.Item2 &&
              actualRoad.Item3 == expectedRoad.Item3)
          {
            foundExpectedRoad = true;
            actualRoads.RemoveAt(index);
            index--;
            break;
          }
        }

        if (!foundExpectedRoad)
        {
          throw new Exception("Did not find expected road: " + expectedRoad);
        }
      }
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

      this.VerifyNewSettlements(gameBoardUpdate.NewSettlements,
        new Tuple<UInt32, Guid>(FirstSettlementOneLocation, firstOpponent.Id));

      this.VerifyNewRoads(gameBoardUpdate.NewRoads,
        new Tuple<UInt32, UInt32, Guid>(FirstSettlementOneLocation, FirstRoadOneEnd, firstOpponent.Id));

      gameBoardUpdate = null; // Ensure that there is a state change for the gameBoardUpdate variable 
      localGameController.ContinueGameSetup(MainSettlementOneLocation, MainRoadOneEnd);

      gameBoardUpdate.ShouldNotBeNull();

      this.VerifyNewSettlements(gameBoardUpdate.NewSettlements,
        new Tuple<UInt32, Guid>(SecondSettlementOneLocation, secondOpponent.Id),
        new Tuple<UInt32, Guid>(SecondSettlementTwoLocation, secondOpponent.Id),
        new Tuple<UInt32, Guid>(ThirdSettlementOneLocation, thirdOpponent.Id),
        new Tuple<UInt32, Guid>(ThirdSettlementTwoLocation, thirdOpponent.Id));

      this.VerifyNewRoads(gameBoardUpdate.NewRoads,
        new Tuple<UInt32, UInt32, Guid>(SecondSettlementOneLocation, SecondRoadOneEnd, secondOpponent.Id),
        new Tuple<UInt32, UInt32, Guid>(SecondSettlementTwoLocation, SecondRoadTwoEnd, secondOpponent.Id),
        new Tuple<UInt32, UInt32, Guid>(ThirdSettlementOneLocation, ThirdRoadOneEnd, thirdOpponent.Id),
        new Tuple<UInt32, UInt32, Guid>(ThirdSettlementTwoLocation, ThirdRoadTwoEnd, thirdOpponent.Id));

      gameBoardUpdate = null; // Ensure that there is a state change for the gameBoardUpdate variable 
      localGameController.CompleteGameSetup(MainSettlementTwoLocation, MainRoadTwoEnd);

      this.VerifyNewSettlements(gameBoardUpdate.NewSettlements,
        new Tuple<UInt32, Guid>(FirstSettlementTwoLocation, firstOpponent.Id));

      this.VerifyNewRoads(gameBoardUpdate.NewRoads,
        new Tuple<UInt32, UInt32, Guid>(FirstSettlementTwoLocation, FirstRoadTwoEnd, firstOpponent.Id));
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

      this.VerifyNewSettlements(gameBoardUpdate.NewSettlements,
        new Tuple<UInt32, Guid>(FirstSettlementOneLocation, firstOpponent.Id),
        new Tuple<UInt32, Guid>(SecondSettlementOneLocation, secondOpponent.Id));

      this.VerifyNewRoads(gameBoardUpdate.NewRoads,
        new Tuple<UInt32, UInt32, Guid>(FirstSettlementOneLocation, FirstRoadOneEnd, firstOpponent.Id),
        new Tuple<UInt32, UInt32, Guid>(SecondSettlementOneLocation, SecondRoadOneEnd, secondOpponent.Id));

      gameBoardUpdate = null; // Ensure that there is a state change for the gameBoardUpdate variable 
      localGameController.ContinueGameSetup(MainSettlementOneLocation, MainRoadOneEnd);

      gameBoardUpdate.ShouldNotBeNull();

      this.VerifyNewSettlements(gameBoardUpdate.NewSettlements,
        new Tuple<UInt32, Guid>(ThirdSettlementOneLocation, thirdOpponent.Id),
        new Tuple<UInt32, Guid>(ThirdSettlementTwoLocation, thirdOpponent.Id));

      this.VerifyNewRoads(gameBoardUpdate.NewRoads,
        new Tuple<UInt32, UInt32, Guid>(ThirdSettlementOneLocation, ThirdRoadOneEnd, thirdOpponent.Id),
        new Tuple<UInt32, UInt32, Guid>(ThirdSettlementTwoLocation, ThirdRoadTwoEnd, thirdOpponent.Id));

      gameBoardUpdate = null; // Ensure that there is a state change for the gameBoardUpdate variable 
      localGameController.CompleteGameSetup(MainSettlementTwoLocation, MainRoadTwoEnd);

      this.VerifyNewSettlements(gameBoardUpdate.NewSettlements,
        new Tuple<UInt32, Guid>(SecondSettlementTwoLocation, secondOpponent.Id),
        new Tuple<UInt32, Guid>(FirstSettlementTwoLocation, firstOpponent.Id));

      this.VerifyNewRoads(gameBoardUpdate.NewRoads,
        new Tuple<UInt32, UInt32, Guid>(SecondSettlementTwoLocation, SecondRoadTwoEnd, secondOpponent.Id),
        new Tuple<UInt32, UInt32, Guid>(FirstSettlementTwoLocation, FirstRoadTwoEnd, firstOpponent.Id));
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

      this.VerifyNewSettlements(gameBoardUpdate.NewSettlements,
        new Tuple<UInt32, Guid>(FirstSettlementOneLocation, firstOpponent.Id),
        new Tuple<UInt32, Guid>(SecondSettlementOneLocation, secondOpponent.Id),
        new Tuple<UInt32, Guid>(ThirdSettlementOneLocation, thirdOpponent.Id));

      this.VerifyNewRoads(gameBoardUpdate.NewRoads,
        new Tuple<UInt32, UInt32, Guid>(FirstSettlementOneLocation, FirstRoadOneEnd, firstOpponent.Id),
        new Tuple<UInt32, UInt32, Guid>(SecondSettlementOneLocation, SecondRoadOneEnd, secondOpponent.Id),
        new Tuple<UInt32, UInt32, Guid>(ThirdSettlementOneLocation, ThirdRoadOneEnd, thirdOpponent.Id));

      gameBoardUpdate = new GameBoardUpdate(); // Ensure that there is a state change for the gameBoardUpdate variable 
      localGameController.ContinueGameSetup(MainSettlementOneLocation, MainRoadOneEnd);
      gameBoardUpdate.ShouldBeNull();

      gameBoardUpdate = null; // Ensure that there is a state change for the gameBoardUpdate variable 
      localGameController.CompleteGameSetup(MainSettlementTwoLocation, MainRoadTwoEnd);

      gameBoardUpdate.ShouldNotBeNull();
      this.VerifyNewSettlements(gameBoardUpdate.NewSettlements,
        new Tuple<UInt32, Guid>(ThirdSettlementTwoLocation, thirdOpponent.Id),
        new Tuple<UInt32, Guid>(SecondSettlementTwoLocation, secondOpponent.Id),
        new Tuple<UInt32, Guid>(FirstSettlementTwoLocation, firstOpponent.Id));

      this.VerifyNewRoads(gameBoardUpdate.NewRoads,
        new Tuple<UInt32, UInt32, Guid>(ThirdSettlementTwoLocation, ThirdRoadTwoEnd, thirdOpponent.Id),
        new Tuple<UInt32, UInt32, Guid>(SecondSettlementTwoLocation, SecondRoadTwoEnd, secondOpponent.Id),
        new Tuple<UInt32, UInt32, Guid>(FirstSettlementTwoLocation, FirstRoadTwoEnd, firstOpponent.Id));
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
      localGameController.ContinueGameSetup(MainSettlementOneLocation, MainRoadOneEnd);
      localGameController.CompleteGameSetup(MainSettlementTwoLocation, MainRoadTwoEnd);

      resourceUpdate.ShouldNotBeNull();
      resourceUpdate.Resources.Count.ShouldBe(4);
      resourceUpdate.Resources.ShouldContainKey(player.Id);
      resourceUpdate.Resources.ShouldContainKey(firstOpponent.Id);
      resourceUpdate.Resources.ShouldContainKey(secondOpponent.Id);
      resourceUpdate.Resources.ShouldContainKey(thirdOpponent.Id);

      resourceUpdate.Resources[player.Id].BrickCount.ShouldBe(1);
      resourceUpdate.Resources[player.Id].GrainCount.ShouldBe(1);
      resourceUpdate.Resources[player.Id].LumberCount.ShouldBe(0);
      resourceUpdate.Resources[player.Id].OreCount.ShouldBe(0);
      resourceUpdate.Resources[player.Id].WoolCount.ShouldBe(1);

      resourceUpdate.Resources[firstOpponent.Id].BrickCount.ShouldBe(0);
      resourceUpdate.Resources[firstOpponent.Id].GrainCount.ShouldBe(1);
      resourceUpdate.Resources[firstOpponent.Id].LumberCount.ShouldBe(1);
      resourceUpdate.Resources[firstOpponent.Id].OreCount.ShouldBe(0);
      resourceUpdate.Resources[firstOpponent.Id].WoolCount.ShouldBe(1);

      resourceUpdate.Resources[secondOpponent.Id].BrickCount.ShouldBe(0);
      resourceUpdate.Resources[secondOpponent.Id].GrainCount.ShouldBe(0);
      resourceUpdate.Resources[secondOpponent.Id].LumberCount.ShouldBe(1);
      resourceUpdate.Resources[secondOpponent.Id].OreCount.ShouldBe(1);
      resourceUpdate.Resources[secondOpponent.Id].WoolCount.ShouldBe(1);

      resourceUpdate.Resources[thirdOpponent.Id].BrickCount.ShouldBe(0);
      resourceUpdate.Resources[thirdOpponent.Id].GrainCount.ShouldBe(1);
      resourceUpdate.Resources[thirdOpponent.Id].LumberCount.ShouldBe(1);
      resourceUpdate.Resources[thirdOpponent.Id].OreCount.ShouldBe(0);
      resourceUpdate.Resources[thirdOpponent.Id].WoolCount.ShouldBe(1);
    }

    [Test]
    [Category("LocalGameController")]
    public void ContinueGameSetup_CallOutOfSequence_MeaningfulErrorIsReceived()
    {
      var localGameController = new LocalGameControllerCreator().Create();
      ErrorDetails errorDetails = null;
      localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

      localGameController.ContinueGameSetup(0u, 1u);

      errorDetails.ShouldNotBeNull();
      errorDetails.Message.ShouldBe("Cannot call 'ContinueGameSetup' until 'StartGameSetup' has completed.");
    }

    [Test]
    [Category("LocalGameController")]
    public void CompleteGameSetup_CallOutOfSequence_MeaningfulErrorIsReceived()
    {
      var localGameController = new LocalGameControllerCreator().Create();
      ErrorDetails errorDetails = null;
      localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

      localGameController.CompleteGameSetup(0u, 1u);

      errorDetails.ShouldNotBeNull();
      errorDetails.Message.ShouldBe("Cannot call 'CompleteGameSetup' until 'ContinueGameSetup' has completed.");
    }

    [Test]
    [Category("LocalGameController")]
    public void PlayerSelectsSameLocationAsComputerPlayerDuringFirstSetupRound_MeaningfulErrorIsReceived()
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

      localGameController.ContinueGameSetup(FirstSettlementOneLocation, 1u);
      exception.ShouldNotBeNull();
      exception.Message.ShouldBe("Cannot place settlement: Location " + FirstSettlementOneLocation + " already owned by player " + firstOpponent.Id);
      gameBoardUpdate.ShouldBeNull();
    }

    [Test]
    [Category("LocalGameController")]
    public void PlayerSelectsSameLocationAsComputerPlayerDuringSecondSetupRound_MeaningfulErrorIsReceived()
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
      localGameController.ContinueGameSetup(0, 1);

      GameBoardUpdate gameBoardUpdate = null;
      localGameController.GameSetupUpdateEvent = (GameBoardUpdate u) => { gameBoardUpdate = u; };

      localGameController.CompleteGameSetup(FirstSettlementOneLocation, 1);

      exception.ShouldNotBeNull();
      exception.Message.ShouldBe("Cannot place settlement: Location " + FirstSettlementOneLocation + " already owned by player " + firstOpponent.Id);
      gameBoardUpdate.ShouldBeNull();
    }

    [Test]
    [Category("LocalGameController")]
    public void PlayerSelectsLocationTooCloseToComputerPlayerDuringFirstSetupRound_MeaningfulErrorIsReceived()
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

      localGameController.ContinueGameSetup(19u, 1u);

      exception.ShouldNotBeNull();
      exception.Message.ShouldBe("Cannot place settlement: Too close to player " + firstOpponent.Id + " at location " + FirstSettlementOneLocation);
      gameBoardUpdate.ShouldBeNull();
    }

    [Test]
    [Category("LocalGameController")]
    public void PlayerSelectsLocationTooCloseToComputerPlayerDuringSecondSetupRound_MeaningfulErrorIsReceived()
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
      localGameController.ContinueGameSetup(0, 1);

      GameBoardUpdate gameBoardUpdate = null;
      localGameController.GameSetupUpdateEvent = (GameBoardUpdate u) => { gameBoardUpdate = u; };

      localGameController.CompleteGameSetup(19, 18);

      exception.ShouldNotBeNull();
      exception.Message.ShouldBe("Cannot place settlement: Too close to player " + firstOpponent.Id + " at location " + FirstSettlementOneLocation);
      gameBoardUpdate.ShouldBeNull();
    }

    [Test]
    [Category("LocalGameController")]
    public void PlayerPlacesSettlementOffGameBoardDuringFirstSetupRound_MeaningfulErrorReceived()
    {
      var localGameController = CreateLocalGameControllerWithMainPlayerGoingFirstInSetup();

      ErrorDetails errorDetails = null;
      localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

      GameBoardUpdate gameBoardUpdate = null;
      localGameController.GameSetupUpdateEvent = (GameBoardUpdate u) => { gameBoardUpdate = u; };

      localGameController.JoinGame();
      localGameController.LaunchGame();
      localGameController.StartGameSetup();
      localGameController.ContinueGameSetup(100, 101);

      errorDetails.ShouldNotBeNull();
      errorDetails.Message.ShouldBe("Cannot place settlement at [100]. This is outside of board range (0 - 53).");
      gameBoardUpdate.ShouldBeNull();
    }

    [Test]
    [Category("LocalGameController")]
    public void PlayerPlacesRoadOverEdgeOfGameBoardDuringFirstSetupRound_MeaningfulErrorIsReceived()
    {
      var localGameController = CreateLocalGameControllerWithMainPlayerGoingFirstInSetup();

      ErrorDetails errorDetails = null;
      localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

      GameBoardUpdate gameBoardUpdate = null;
      localGameController.GameSetupUpdateEvent = (GameBoardUpdate u) => { gameBoardUpdate = u; };

      localGameController.JoinGame();
      localGameController.LaunchGame();
      localGameController.StartGameSetup();
      localGameController.ContinueGameSetup(53, 54);

      errorDetails.ShouldNotBeNull();
      errorDetails.Message.ShouldBe("Cannot place road at [53, 54]. This is outside of board range (0 - 53).");
      gameBoardUpdate.ShouldBeNull();
    }

    [Test]
    [Category("LocalGameController")]
    public void PlayerPlacesRoadWhereNoConnectionExistsDuringFirstSetupRound_MeaningfulErrorIsReceived()
    {
      var localGameController = CreateLocalGameControllerWithMainPlayerGoingFirstInSetup();

      ErrorDetails errorDetails = null;
      localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

      GameBoardUpdate gameBoardUpdate = null;
      localGameController.GameSetupUpdateEvent = (GameBoardUpdate u) => { gameBoardUpdate = u; };

      localGameController.JoinGame();
      localGameController.LaunchGame();
      localGameController.StartGameSetup();
      localGameController.ContinueGameSetup(28, 40);

      errorDetails.ShouldNotBeNull();
      errorDetails.Message.ShouldBe("Cannot place road at [28, 40]. There is no direct connection between those points.");
      gameBoardUpdate.ShouldBeNull();
    }

    [Test]
    [Category("LocalGameController")]
    public void PlayerPlacesSettlementOffGameBoardDuringSecondSetupRound_MeaningfulErrorIsReceived()
    {
      var localGameController = CreateLocalGameControllerWithMainPlayerGoingFirstInSetup();

      ErrorDetails errorDetails = null;
      localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

      localGameController.JoinGame();
      localGameController.LaunchGame();
      localGameController.StartGameSetup();
      localGameController.ContinueGameSetup(0, 1);

      GameBoardUpdate gameBoardUpdate = null;
      localGameController.GameSetupUpdateEvent = (GameBoardUpdate u) => { gameBoardUpdate = u; };

      localGameController.CompleteGameSetup(100, 101);

      errorDetails.ShouldNotBeNull();
      errorDetails.Message.ShouldBe("Cannot place settlement at [100]. This is outside of board range (0 - 53).");
      gameBoardUpdate.ShouldBeNull();
    }

    [Test]
    [Category("LocalGameController")]
    public void PlayerPlacesRoadOverEdgeOfGameBoardDuringSecondSetupRound_MeaningfulErrorIsReceived()
    {
      var localGameController = CreateLocalGameControllerWithMainPlayerGoingFirstInSetup();

      ErrorDetails errorDetails = null;
      localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

      localGameController.JoinGame();
      localGameController.LaunchGame();
      localGameController.StartGameSetup();
      localGameController.ContinueGameSetup(0, 1);

      GameBoardUpdate gameBoardUpdate = null;
      localGameController.GameSetupUpdateEvent = (GameBoardUpdate u) => { gameBoardUpdate = u; };

      localGameController.CompleteGameSetup(53, 54);

      errorDetails.ShouldNotBeNull();
      errorDetails.Message.ShouldBe("Cannot place road at [53, 54]. This is outside of board range (0 - 53).");
      gameBoardUpdate.ShouldBeNull();
    }

    [Test]
    [Category("LocalGameController")]
    public void PlayerPlacesRoadWhereNoConnectionExistsDuringSecondSetupRound_MeaningfulErrorIsReceived()
    {
      var localGameController = CreateLocalGameControllerWithMainPlayerGoingFirstInSetup();

      ErrorDetails errorDetails = null;
      localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

      localGameController.JoinGame();
      localGameController.LaunchGame();
      localGameController.StartGameSetup();
      localGameController.ContinueGameSetup(0, 1);

      GameBoardUpdate gameBoardUpdate = null;
      localGameController.GameSetupUpdateEvent = (GameBoardUpdate u) => { gameBoardUpdate = u; };

      localGameController.CompleteGameSetup(28, 40);

      errorDetails.ShouldNotBeNull();
      errorDetails.Message.ShouldBe("Cannot place road at [28, 40]. There is no direct connection between those points.");
      gameBoardUpdate.ShouldBeNull();
    }

    [Test]
    [Category("LocalGameController")]
    public void FinalisePlayerTurnOrder_CallOutOfSequence_MeaningfulErrorIsReceived()
    {
      var localGameController = new LocalGameControllerCreator().Create();
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
      localGameController.ContinueGameSetup(MainSettlementOneLocation, MainRoadOneEnd);
      localGameController.CompleteGameSetup(MainSettlementTwoLocation, MainRoadTwoEnd);

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
      localGameController.ContinueGameSetup(MainSettlementOneLocation, MainRoadOneEnd);
      localGameController.CompleteGameSetup(MainSettlementTwoLocation, MainRoadTwoEnd);

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
    [Category("All")]
    [Category("LocalGameController")]
    public void StartGamePlayer_PlayerTurnNotFinalised_MeaningfulErrorIsReceived()
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

      ErrorDetails errorDetails = null;
      localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

      localGameController.JoinGame();
      localGameController.LaunchGame();
      localGameController.StartGameSetup();
      localGameController.ContinueGameSetup(MainSettlementOneLocation, MainRoadOneEnd);
      localGameController.CompleteGameSetup(MainSettlementTwoLocation, MainRoadTwoEnd);

      // Act
      localGameController.StartGamePlay();

      // Assert
      errorDetails.ShouldNotBeNull();
      errorDetails.Message.ShouldBe("Cannot call 'StartGamePlay' until 'FinalisePlayerTurnOrder' has completed.");
    }

    [Test]
    [Category("LocalGameController")]
    [Category("Main Player Turn")]
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
    [Category("Main Player Turn")]
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
    [Category("Main Player Turn")]
    public void StartOfMainPlayerTurn_DoesNotRollSeven_ReceiveCollectedResourcesDetails()
    {
      MockDice mockDice = null;
      MockPlayer player;
      MockComputerPlayer firstOpponent, secondOpponent, thirdOpponent;
      var localGameController = this.CreateLocalGameControllerAndCompleteGameSetup(out mockDice, out player, out firstOpponent, out secondOpponent, out thirdOpponent);

      mockDice.AddSequence(new[] { 8u });

      ResourceUpdate resourceUpdate = null;
      localGameController.ResourcesCollectedEvent = (ResourceUpdate r) => { resourceUpdate = r; };
      localGameController.StartGamePlay();

      resourceUpdate.ShouldNotBeNull();
      resourceUpdate.Resources.Count.ShouldBe(2);
      resourceUpdate.Resources[player.Id].BrickCount.ShouldBe(1);
      resourceUpdate.Resources[player.Id].GrainCount.ShouldBe(0);
      resourceUpdate.Resources[player.Id].LumberCount.ShouldBe(0);
      resourceUpdate.Resources[player.Id].OreCount.ShouldBe(0);
      resourceUpdate.Resources[player.Id].WoolCount.ShouldBe(0);

      resourceUpdate.Resources[firstOpponent.Id].BrickCount.ShouldBe(0);
      resourceUpdate.Resources[firstOpponent.Id].GrainCount.ShouldBe(1);
      resourceUpdate.Resources[firstOpponent.Id].LumberCount.ShouldBe(0);
      resourceUpdate.Resources[firstOpponent.Id].OreCount.ShouldBe(0);
      resourceUpdate.Resources[firstOpponent.Id].WoolCount.ShouldBe(0);
    }

    [Test]
    [Category("LocalGameController")]
    [Category("Main Player Turn")]
    public void StartOfMainPlayerTurn_RollsSevenReceiveResourceCardLossesForComputerPlayers()
    {
      // Arrange
      var droppedResourcesForFirstOpponent = new ResourceClutch(1, 1, 1, 1, 0);
      var expectedDroppedResourcesForFirstOpponent = new ResourceClutch(1, 1, 1, 1, 0);
      var droppedResourcesForThirdOpponent = new ResourceClutch(1, 1, 1, 1, 1);
      var expectedDroppedResourcesForThirdOpponent = new ResourceClutch(1, 1, 1, 1, 1);

      MockDice mockDice = null;
      MockPlayer player;
      MockComputerPlayer firstOpponent, secondOpponent, thirdOpponent;
      var localGameController = this.CreateLocalGameControllerAndCompleteGameSetup(out mockDice, out player, out firstOpponent, out secondOpponent, out thirdOpponent);
      mockDice.AddSequence(new[] { 7u });

      //player.Resources = new ResourceBag();
      firstOpponent.AddResources(new ResourceClutch(2, 2, 2, 1, 1));
      firstOpponent.DroppedResources = droppedResourcesForFirstOpponent;
      secondOpponent.AddResources(new ResourceClutch(2, 2, 1, 1, 1));
      thirdOpponent.AddResources(new ResourceClutch(2, 2, 2, 2, 1));
      thirdOpponent.DroppedResources = droppedResourcesForThirdOpponent;

      // Act
      ResourceUpdate resourcesLost = null;
      localGameController.ResourcesLostEvent = (ResourceUpdate r) => { resourcesLost = r; };
      localGameController.StartGamePlay();

      // Assert
      resourcesLost.Resources.Count.ShouldBe(2);
      resourcesLost.Resources.ShouldContainKeyAndValue(firstOpponent.Id, expectedDroppedResourcesForFirstOpponent);
      resourcesLost.Resources.ShouldContainKeyAndValue(thirdOpponent.Id, expectedDroppedResourcesForThirdOpponent);
    }

    [Test]
    [TestCase(6, 0)]
    [TestCase(7, 0)]
    [TestCase(8, 4)]
    [TestCase(9, 4)]
    [TestCase(10, 5)]
    [Category("LocalGameController")]
    [Category("Main Player Turn")]
    public void StartOfMainPlayerTurn_RollsSevenReceivesRobberEventNotificationWithDropResourceCardsCount(Int32 brickCount, Int32 expectedResourceDropCount)
    {
      // Arrange
      MockDice mockDice = null;
      MockPlayer player;
      MockComputerPlayer firstOpponent, secondOpponent, thirdOpponent;
      var localGameController = this.CreateLocalGameControllerAndCompleteGameSetup(out mockDice, out player, out firstOpponent, out secondOpponent, out thirdOpponent);
      mockDice.AddSequence(new[] { 7u });

      player.AddResources(new ResourceClutch(brickCount, 0, 0, 0, 0));

      // Act
      Int32 resourceDropCount = -1;
      localGameController.RobberEvent = (Int32 r) => { resourceDropCount = r; };
      localGameController.StartGamePlay();

      // Assert
      resourceDropCount.ShouldBe(expectedResourceDropCount);
    }

    [Test]
    [Category("LocalGameController")]
    [Category("Main Player Turn")]
    public void StartOfMainPlayerTurn_RollsSevenButDoesNotPassBackExpectedResources_MeaningfulErrorIsReceived()
    {
      // Arrange
      MockDice mockDice = null;
      MockPlayer player;
      MockComputerPlayer firstOpponent, secondOpponent, thirdOpponent;
      var localGameController = this.CreateLocalGameControllerAndCompleteGameSetup(out mockDice, out player, out firstOpponent, out secondOpponent, out thirdOpponent);
      mockDice.AddSequence(new[] { 7u });

      player.AddResources(new ResourceClutch(8, 0, 0, 0, 0));
      
      // Act
      ErrorDetails errorDetails = null;
      localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };
      localGameController.StartGamePlay();
      localGameController.SetRobberLocation(0u);

      // Assert
      errorDetails.ShouldNotBeNull();
      errorDetails.Message.ShouldBe("Cannot set robber location until expected resources (4) have been dropped via call to DropResources method.");
    }

    [Test]
    [Category("LocalGameController")]
    [Category("Main Player Turn")]
    public void StartOfMainPlayerTurn_RollsSevenAndPassBackExpectedResources_PlayerResourcesUpdatedCorrectly()
    {
      // Arrange
      MockDice mockDice = null;
      MockPlayer player;
      MockComputerPlayer firstOpponent, secondOpponent, thirdOpponent;
      var localGameController = this.CreateLocalGameControllerAndCompleteGameSetup(out mockDice, out player, out firstOpponent, out secondOpponent, out thirdOpponent);
      mockDice.AddSequence(new[] { 7u });

      player.AddResources(new ResourceClutch(8, 0, 0, 0, 0));
      
      // Act
      localGameController.StartGamePlay();
      localGameController.DropResources(new ResourceClutch(4, 0, 0, 0, 0));

      // Assert
      player.BrickCount.ShouldBe(4);
      player.GrainCount.ShouldBe(0);
      player.LumberCount.ShouldBe(0);
      player.OreCount.ShouldBe(0);
      player.WoolCount.ShouldBe(0);
    }

    [Test]
    [Category("LocalGameController")]
    [Category("Main Player Turn")]
    public void StartOfMainPlayerTurn_SetRobberOnHexWithOneOpponent_ReturnListOfOpponentsAndResourceCardsToChooseFrom()
    {
      // Arrange
      MockDice mockDice = null;
      MockPlayer player;
      MockComputerPlayer firstOpponent, secondOpponent, thirdOpponent;
      var localGameController = this.CreateLocalGameControllerAndCompleteGameSetup(out mockDice, out player, out firstOpponent, out secondOpponent, out thirdOpponent);
      mockDice.AddSequence(new[] { 7u });

      firstOpponent.AddResources(new ResourceClutch(1, 1, 1, 1, 1));

      // Act
      Dictionary<Guid, Int32> robberingChoices = null;
      localGameController.RobbingChoicesEvent = (Dictionary<Guid, Int32> rc) => { robberingChoices = rc; };
      localGameController.StartGamePlay();
      localGameController.SetRobberLocation(FirstSettlementOneLocation);

      // Assert
      robberingChoices.ShouldNotBeNull();
      robberingChoices.Count.ShouldBe(1);
      robberingChoices.ShouldContainKeyAndValue(firstOpponent.Id, 5);
    }

    [Test]
    [Category("All")]
    [Category("LocalGameController")]
    [Category("Main Player Turn")]
    public void StartOfMainPlayerTurn_RollsSevenAndChoosesBlindResourceFromOpponent_PlayerAndOpponentResourcesUpdated()
    {
      // Arrange
      MockDice mockDice = null;
      MockPlayer player;
      MockComputerPlayer firstOpponent, secondOpponent, thirdOpponent;
      var localGameController = this.CreateLocalGameControllerAndCompleteGameSetup(out mockDice, out player, out firstOpponent, out secondOpponent, out thirdOpponent);
      mockDice.AddSequence(new[] { 7u });

      firstOpponent.AddResources(new ResourceClutch(1, 0, 0, 0, 0));

      // Act
      ResourceClutch resourceClutch = ResourceClutch.Zero;
      localGameController.ResourcesGainedEvent = (ResourceClutch rc) => { resourceClutch = rc; };
      localGameController.StartGamePlay();
      localGameController.SetRobberLocation(7u);
      localGameController.ChooseResourceFromOpponent(firstOpponent.Id, 0);

      // Assert
      resourceClutch.ShouldBe(new ResourceClutch(1, 0, 0, 0, 0));
      player.ResourcesCount.ShouldBe(1);
      player.BrickCount.ShouldBe(1);
      firstOpponent.ResourcesCount.ShouldBe(0);
    }

    /// <summary>
    /// Passing in an resource index that is out of the range causes an error to be raised.
    /// </summary>
    [Test]
    [Category("LocalGameController")]
    [Category("Main Player Turn")]
    [TestCase(-1)]
    [TestCase(1)]
    public void StartOfMainPlayerTurn_RollsSevenAndChoosesOutOfRangeResourceIndexFromOpponent_MeaningfulErrorIsReceived(Int32 index)
    {
      // Arrange
      MockDice mockDice = null;
      MockPlayer player;
      MockComputerPlayer firstOpponent, secondOpponent, thirdOpponent;
      var localGameController = this.CreateLocalGameControllerAndCompleteGameSetup(out mockDice, out player, out firstOpponent, out secondOpponent, out thirdOpponent);
      mockDice.AddSequence(new[] { 7u });

      firstOpponent.AddResources(new ResourceClutch(1, 0, 0, 0, 0));

      // Act
      ResourceClutch resourceClutch = ResourceClutch.Zero;
      ErrorDetails errorDetails = null;
      localGameController.ResourcesGainedEvent = (ResourceClutch rc) => { resourceClutch = rc; };
      localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };
      localGameController.StartGamePlay();
      localGameController.SetRobberLocation(3u);
      localGameController.ChooseResourceFromOpponent(firstOpponent.Id, index);

      // Assert
      resourceClutch.ShouldBe(ResourceClutch.Zero);
      player.ResourcesCount.ShouldBe(0);
      firstOpponent.ResourcesCount.ShouldBe(1);
      firstOpponent.BrickCount.ShouldBe(1);
      errorDetails.ShouldNotBeNull();
      errorDetails.Message.ShouldBe("Cannot pick resource card at position " + index + ". Resource card range is 0..0");
    }

    /// <summary>
    /// Passing in an unknown player id causes an error to be raised.
    /// </summary>
    [Test]
    [Category("LocalGameController")]
    [Category("Main Player Turn")]
    public void StartOfMainPlayerTurn_RollsSevenAndPassesInUnknownPlayerId_MeaningfulErrorIsReceived()
    {
      // Arrange
      MockDice mockDice = null;
      MockPlayer player;
      MockComputerPlayer firstOpponent, secondOpponent, thirdOpponent;
      var localGameController = this.CreateLocalGameControllerAndCompleteGameSetup(out mockDice, out player, out firstOpponent, out secondOpponent, out thirdOpponent);
      mockDice.AddSequence(new[] { 7u });

      firstOpponent.AddResources(new ResourceClutch(1, 0, 0, 0, 0));

      // Act
      ResourceClutch resourceClutch = ResourceClutch.Zero;
      ErrorDetails errorDetails = null;
      localGameController.ResourcesGainedEvent = (ResourceClutch rc) => { resourceClutch = rc; };
      localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };
      localGameController.StartGamePlay();
      localGameController.SetRobberLocation(3u);
      localGameController.ChooseResourceFromOpponent(secondOpponent.Id, 0);

      // Assert
      resourceClutch.ShouldBe(ResourceClutch.Zero);
      player.ResourcesCount.ShouldBe(0);
      firstOpponent.ResourcesCount.ShouldBe(1);
      firstOpponent.BrickCount.ShouldBe(1);
      errorDetails.ShouldNotBeNull();
      errorDetails.Message.ShouldBe("Cannot pick resource card from invalid opponent.");
    }

    /// <summary>
    /// Passing in the player id when choosing the resource from an opponent causes an error to be raised.
    /// </summary>
    [Test]
    [Category("All")]
    [Category("LocalGameController")]
    [Category("Main Player Turn")]
    public void StartOfMainPlayerTurn_ChooseResourceFromOpponentUsingPlayerId_MeaningfulErrorIsReceived()
    {
      // Arrange
      MockDice mockDice = null;
      MockPlayer player;
      MockComputerPlayer firstOpponent, secondOpponent, thirdOpponent;
      var localGameController = this.CreateLocalGameControllerAndCompleteGameSetup(out mockDice, out player, out firstOpponent, out secondOpponent, out thirdOpponent);
      mockDice.AddSequence(new[] { 7u });

      firstOpponent.AddResources(new ResourceClutch(1, 0, 0, 0, 0));

      // Act
      ResourceClutch resourceClutch = ResourceClutch.Zero;
      ErrorDetails errorDetails = null;
      localGameController.ResourcesGainedEvent = (ResourceClutch rc) => { resourceClutch = rc; };
      localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };
      localGameController.StartGamePlay();
      localGameController.SetRobberLocation(3u);
      localGameController.ChooseResourceFromOpponent(player.Id, 0);

      // Assert
      resourceClutch.ShouldBe(ResourceClutch.Zero);
      player.ResourcesCount.ShouldBe(0);
      firstOpponent.ResourcesCount.ShouldBe(1);
      firstOpponent.BrickCount.ShouldBe(1);
      errorDetails.ShouldNotBeNull();
      errorDetails.Message.ShouldBe("Cannot pick resource card from invalid opponent.");
    }

    [Test]
    [Category("LocalGameController")]
    [Category("Main Player Turn")]
    public void StartOfMainPlayerTurn_ChooseResourceFromOpponentCalledOutOfSequence_MeaningfulErrorIsReceived()
    {
      // Arrange
      MockDice mockDice = null;
      MockPlayer player;
      MockComputerPlayer firstOpponent, secondOpponent, thirdOpponent;
      var localGameController = this.CreateLocalGameControllerAndCompleteGameSetup(out mockDice, out player, out firstOpponent, out secondOpponent, out thirdOpponent);
      mockDice.AddSequence(new[] { 7u });

      firstOpponent.AddResources(new ResourceClutch(1, 0, 0, 0, 0));

      // Act
      ResourceClutch resourceClutch = ResourceClutch.Zero;
      ErrorDetails errorDetails = null;
      localGameController.ResourcesGainedEvent = (ResourceClutch rc) => { resourceClutch = rc; };
      localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };
      localGameController.ChooseResourceFromOpponent(firstOpponent.Id, 0);

      // Assert
      resourceClutch.ShouldBe(ResourceClutch.Zero);
      player.ResourcesCount.ShouldBe(0);
      firstOpponent.ResourcesCount.ShouldBe(1);
      firstOpponent.BrickCount.ShouldBe(1);
      errorDetails.ShouldNotBeNull();
      errorDetails.Message.ShouldBe("Cannot call 'ChooseResourceFromOpponent' until 'SetRobberLocation' has completed.");
    }

    /// <summary>
    /// The robber hex set by the player has no adjacent settlements so the returned robbing choices
    /// is null.
    /// </summary>
    [Test]
    [Category("LocalGameController")]
    [Category("Main Player Turn")]
    public void StartOfMainPlayerTurn_RobberLocationHasNoSettlements_ReturnedRobbingChoicesIsNull()
    {
      // Arrange
      MockDice mockDice = null;
      MockPlayer player;
      MockComputerPlayer firstOpponent, secondOpponent, thirdOpponent;
      var localGameController = this.CreateLocalGameControllerAndCompleteGameSetup(out mockDice, out player, out firstOpponent, out secondOpponent, out thirdOpponent);
      mockDice.AddSequence(new[] { 7u });

      // Act
      Dictionary<Guid, Int32> robbingChoices = new Dictionary<Guid, Int32>();
      localGameController.RobbingChoicesEvent = (Dictionary<Guid, Int32> rc) => { robbingChoices = rc; };
      localGameController.StartGamePlay();
      localGameController.SetRobberLocation(0u);

      // Assert
      robbingChoices.ShouldBeNull();
    }

    /// <summary>
    /// The robber hex set by the player has no adjacent settlements so calling the CallingChooseResourceFromOpponent 
    /// method raises an error
    /// </summary>
    [Test]
    [Category("All")]
    [Category("LocalGameController")]
    [Category("Main Player Turn")]
    public void StartOfMainPlayerTurn_RobberLocationHasNoSettlementsAndCallingChooseResourceFromOpponent_MeaningfulErrorIsReceived()
    {
      // Arrange
      MockDice mockDice = null;
      MockPlayer player;
      MockComputerPlayer firstOpponent, secondOpponent, thirdOpponent;
      var localGameController = this.CreateLocalGameControllerAndCompleteGameSetup(out mockDice, out player, out firstOpponent, out secondOpponent, out thirdOpponent);
      mockDice.AddSequence(new[] { 7u });

      // Act
      ErrorDetails errorDetails = null;
      localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };
      localGameController.StartGamePlay();
      localGameController.SetRobberLocation(0u);
      localGameController.ChooseResourceFromOpponent(Guid.NewGuid(), 0);

      // Assert
      errorDetails.ShouldNotBeNull();
      errorDetails.Message.ShouldBe("Cannot call 'ChooseResourceFromOpponent' when 'RobbingChoicesEvent' is not raised.");
    }

    /// <summary>
    /// The robber hex set by the player has only player settlements so the returned robbing choices is null.
    /// </summary>
    [Test]
    [Category("All")]
    [Category("LocalGameController")]
    [Category("Main Player Turn")]
    public void StartOfMainPlayerTurn_RobberLocationHasOnlyPlayerSettlements_ReturnedRobbingChoicesIsNull()
    {
      // Arrange
      MockDice mockDice = null;
      MockPlayer player;
      MockComputerPlayer firstOpponent, secondOpponent, thirdOpponent;
      var localGameController = this.CreateLocalGameControllerAndCompleteGameSetup(out mockDice, out player, out firstOpponent, out secondOpponent, out thirdOpponent);
      mockDice.AddSequence(new[] { 7u });

      // Act
      Dictionary<Guid, Int32> robbingChoices = new Dictionary<Guid, Int32>();
      localGameController.RobbingChoicesEvent = (Dictionary<Guid, Int32> rc) => { robbingChoices = rc; };
      localGameController.StartGamePlay();
      localGameController.SetRobberLocation(2u);

      // Assert
      robbingChoices.ShouldBeNull();
    }

    /// <summary>
    /// The robber hex set by the player has only player settlements so calling the CallingChooseResourceFromOpponent 
    /// method raises an error
    /// </summary>
    [Test]
    [Category("All")]
    [Category("LocalGameController")]
    [Category("Main Player Turn")]
    public void StartOfMainPlayerTurn_RobberLocationHasOnlyPlayerSettlementsAndCallingChooseResourceFromOpponent_MeaningfulErrorIsReceived()
    {
      // Arrange
      MockDice mockDice = null;
      MockPlayer player;
      MockComputerPlayer firstOpponent, secondOpponent, thirdOpponent;
      var localGameController = this.CreateLocalGameControllerAndCompleteGameSetup(out mockDice, out player, out firstOpponent, out secondOpponent, out thirdOpponent);
      mockDice.AddSequence(new[] { 7u });

      // Act
      ErrorDetails errorDetails = null;
      localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };
      localGameController.StartGamePlay();
      localGameController.SetRobberLocation(2u);
      localGameController.ChooseResourceFromOpponent(Guid.NewGuid(), 0);

      // Assert
      errorDetails.ShouldNotBeNull();
      errorDetails.Message.ShouldBe("Cannot call 'ChooseResourceFromOpponent' when 'RobbingChoicesEvent' is not raised.");
    }

    [Test]
    [Category("All")]
    [Category("LocalGameController")]
    [Category("LocalGameController.BuildRoad")]
    [Category("Main Player Turn")]
    public void MainPlayerTurn_BuildRoadWithRequiredResourcesAvailable_BuildCompleteEventRaised()
    {
      // Arrange
      MockDice mockDice = null;
      MockPlayer player;
      MockComputerPlayer firstOpponent, secondOpponent, thirdOpponent;
      var localGameController = this.CreateLocalGameControllerAndCompleteGameSetup(out mockDice, out player, out firstOpponent, out secondOpponent, out thirdOpponent);
      mockDice.AddSequence(new[] { 8u });
      player.AddResources(new ResourceClutch(1, 0, 1, 0, 0));

      var buildCompleted = false;
      localGameController.BuildCompletedEvent = () => { buildCompleted = true; };

      // Act
      localGameController.StartGamePlay();
      localGameController.BuildRoad(player.Id, 4u, 3u);

      // Assert
      buildCompleted.ShouldBeTrue();
    }

    [Test]
    [Category("All")]
    [Category("LocalGameController")]
    [Category("LocalGameController.BuildRoad")]
    [Category("Main Player Turn")]
    public void MainPlayerTurn_BuildRoadWithRequiredResourcesAvailable_PlayerResourcesUpdated()
    {
      // Arrange
      MockDice mockDice = null;
      MockPlayer player;
      MockComputerPlayer firstOpponent, secondOpponent, thirdOpponent;
      var localGameController = this.CreateLocalGameControllerAndCompleteGameSetup(out mockDice, out player, out firstOpponent, out secondOpponent, out thirdOpponent);
      mockDice.AddSequence(new[] { 8u });
      player.AddResources(new ResourceClutch(1, 0, 1, 0, 0));

      // Act
      localGameController.StartGamePlay();
      localGameController.BuildRoad(player.Id, 4u, 3u);

      // Assert
      player.BrickCount.ShouldBe(0);
      player.LumberCount.ShouldBe(0);
    }

    [Test]
    [Category("All")]
    [Category("LocalGameController")]
    [Category("LocalGameController.BuildRoad")]
    [Category("Main Player Turn")]
    public void MainPlayerTurn_FirstLongestRoadBuilt_LongestRoadEventRaised()
    {
      // Arrange
      MockDice mockDice = null;
      MockPlayer player;
      MockComputerPlayer firstOpponent, secondOpponent, thirdOpponent;
      var localGameController = this.CreateLocalGameControllerAndCompleteGameSetup(out mockDice, out player, out firstOpponent, out secondOpponent, out thirdOpponent);
      
      mockDice.AddSequence(new[] { 8u });
      player.AddResources(new ResourceClutch(1, 0, 1, 0, 0));

      Guid otherPlayerId = Guid.NewGuid(); // Set it to an id to register state change
      localGameController.LongestRoadBuiltEvent = (Guid pid) => { otherPlayerId = pid; };

      localGameController.StartGamePlay();

      // Act
      localGameController.BuildRoad(player.Id, 4u, 3u);

      // Assert
      otherPlayerId.ShouldBe(Guid.Empty);
    }

    [Test]
    [Category("All")]
    [Category("LocalGameController")]
    [Category("LocalGameController.BuildRoad")]
    [Category("Main Player Turn")]
    public void MainPlayerTurn_SubsequentLongestRoadBuilt_LongestRoadEventRaised()
    {
      // Arrange
      MockDice mockDice = null;
      MockPlayer player;
      MockComputerPlayer firstOpponent, secondOpponent, thirdOpponent;
      var localGameController = this.CreateLocalGameControllerAndCompleteGameSetup(out mockDice, out player, out firstOpponent, out secondOpponent, out thirdOpponent);

      mockDice.AddSequence(new[] { 8u });
      player.AddResources(new ResourceClutch(1, 0, 1, 0, 0));

      Guid otherPlayerId = Guid.Empty;
      localGameController.LongestRoadBuiltEvent = (Guid pid) => { otherPlayerId = pid; };

      localGameController.StartGamePlay();

      // Act
      localGameController.BuildRoad(player.Id, 4u, 3u);

      // Assert
      otherPlayerId.ShouldBe(firstOpponent.Id);
    }

    [Test]
    [Category("All")]
    [Category("LocalGameController")]
    [Category("LocalGameController.BuildRoad")]
    [Category("Main Player Turn")]
    [TestCase(new UInt32[] { 4, 3 })]
    [TestCase(new UInt32[] { 4, 3, 3, 2 })]
    [TestCase(new UInt32[] { 4, 3, 3, 2, 2, 1 })]
    public void MainPlayerTurn_AddToRoadShorterThanFiveSegments_LongestRoadEventNotRaised(UInt32[] roadLocations)
    {
      // Arrange
      MockDice mockDice = null;
      MockPlayer player;
      MockComputerPlayer firstOpponent, secondOpponent, thirdOpponent;
      var localGameController = this.CreateLocalGameControllerAndCompleteGameSetup(out mockDice, out player, out firstOpponent, out secondOpponent, out thirdOpponent);

      mockDice.AddSequence(new[] { 8u });
      var roadCount = roadLocations.Length / 2;
      var brickCount = roadCount;
      var lumberCount = roadCount;
      player.AddResources(new ResourceClutch(brickCount, 0, lumberCount, 0, 0));

      Boolean longestRoadBuiltEventRaised = false;
      localGameController.LongestRoadBuiltEvent = (Guid pid) => { longestRoadBuiltEventRaised = true; };

      localGameController.StartGamePlay();

      // Act
      for (var index = 0; index < roadLocations.Length; index += 2)
      {
        localGameController.BuildRoad(player.Id, roadLocations[index], roadLocations[index + 1]);
      }

      // Assert
      longestRoadBuiltEventRaised.ShouldBeFalse();
    }

    [Test]
    [Category("All")]
    [Category("LocalGameController")]
    [Category("LocalGameController.BuildRoad")]
    [Category("Main Player Turn")]
    [TestCase(0, 0, "Not enough resources to build road. Missing 1 brick and 1 lumber.")]
    [TestCase(1, 0, "Not enough resources to build road. Missing 1 lumber.")]
    [TestCase(0, 1, "Not enough resources to build road. Missing 1 brick.")]
    public void MainPlayerTurn_BuildRoadWithoutRequiredResourcesAvailable_MeaningfulErrorIsReceived(Int32 brickCount, Int32 lumberCount, String expectedErrorMessage)
    {
      // Arrange
      MockDice mockDice = null;
      MockPlayer player;
      MockComputerPlayer firstOpponent, secondOpponent, thirdOpponent;
      var localGameController = this.CreateLocalGameControllerAndCompleteGameSetup(out mockDice, out player, out firstOpponent, out secondOpponent, out thirdOpponent);
      mockDice.AddSequence(new[] { 8u });
      player.AddResources(new ResourceClutch(brickCount, 0, lumberCount, 0, 0));

      ErrorDetails errorDetails = null;
      localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

      // Act
      localGameController.StartGamePlay();
      localGameController.BuildRoad(player.Id, 4u, 3u);

      // Assert
      errorDetails.ShouldNotBeNull();
      errorDetails.Message.ShouldBe(expectedErrorMessage);
    }

    [Test]
    [Category("All")]
    [Category("LocalGameController")]
    public void Load_HexDataOnly_ResourceProvidersLoadedCorrectly()
    {
      // Arrange
      var localGameController = new LocalGameController(new Dice(), new PlayerPool(), new GameBoardManager(BoardSizes.Standard));

      GameBoardData boardData = null;
      localGameController.GameLoadedEvent = (PlayerDataView[] pd, GameBoardData bd) => { boardData = bd; };

      // Act
      var content = "<game><board><hexes>" +
        "<resources>glbglogob gwwwlwlbo</resources>" +
        "<production>9,8,5,12,11,3,6,10,6,0,4,11,2,4,3,5,9,10,8</production>" +
        "</hexes></board></game>";

      var contentBytes = Encoding.UTF8.GetBytes(content);
      using (var memoryStream = new MemoryStream(contentBytes))
      {
        localGameController.Load(memoryStream);
      }

      // Assert
      boardData.ShouldNotBeNull();
      Tuple<ResourceTypes, UInt32>[] hexes = boardData.GetHexInformation();
      hexes.Length.ShouldBe(GameBoardData.StandardBoardHexCount);
      hexes[0].ShouldBe(new Tuple<ResourceTypes, UInt32>(ResourceTypes.Grain, 9));
      hexes[1].ShouldBe(new Tuple<ResourceTypes, UInt32>(ResourceTypes.Lumber, 8));
      hexes[2].ShouldBe(new Tuple<ResourceTypes, UInt32>(ResourceTypes.Brick, 5));
      hexes[3].ShouldBe(new Tuple<ResourceTypes, UInt32>(ResourceTypes.Grain, 12));
      hexes[4].ShouldBe(new Tuple<ResourceTypes, UInt32>(ResourceTypes.Lumber, 11));
      hexes[5].ShouldBe(new Tuple<ResourceTypes, UInt32>(ResourceTypes.Ore, 3));
      hexes[6].ShouldBe(new Tuple<ResourceTypes, UInt32>(ResourceTypes.Grain, 6));
      hexes[7].ShouldBe(new Tuple<ResourceTypes, UInt32>(ResourceTypes.Ore, 10));
      hexes[8].ShouldBe(new Tuple<ResourceTypes, UInt32>(ResourceTypes.Brick, 6));
      hexes[9].ShouldBe(new Tuple<ResourceTypes, UInt32>(ResourceTypes.None, 0));
      hexes[10].ShouldBe(new Tuple<ResourceTypes, UInt32>(ResourceTypes.Grain, 4));
      hexes[11].ShouldBe(new Tuple<ResourceTypes, UInt32>(ResourceTypes.Wool, 11));
      hexes[12].ShouldBe(new Tuple<ResourceTypes, UInt32>(ResourceTypes.Wool, 2));
      hexes[13].ShouldBe(new Tuple<ResourceTypes, UInt32>(ResourceTypes.Wool, 4));
      hexes[14].ShouldBe(new Tuple<ResourceTypes, UInt32>(ResourceTypes.Lumber, 3));
      hexes[15].ShouldBe(new Tuple<ResourceTypes, UInt32>(ResourceTypes.Wool, 5));
      hexes[16].ShouldBe(new Tuple<ResourceTypes, UInt32>(ResourceTypes.Lumber, 9));
      hexes[17].ShouldBe(new Tuple<ResourceTypes, UInt32>(ResourceTypes.Brick, 10));
      hexes[18].ShouldBe(new Tuple<ResourceTypes, UInt32>(ResourceTypes.Ore, 8));
    }

    [Test]
    [Category("All")]
    [Category("LocalGameController")]
    public void Load_PlayerDataOnly_PlayerDataViewsAreAsExpected()
    {
      // Arrange
      MockPlayer player;
      MockComputerPlayer firstOpponent, secondOpponent, thirdOpponent;
      this.CreateDefaultPlayerInstances(out player, out firstOpponent, out secondOpponent, out thirdOpponent);
        
      player.AddResources(new ResourceClutch(1, 2, 3, 4, 5));
      firstOpponent.AddResources(new ResourceClutch(1, 1, 1, 1, 1));
      secondOpponent.AddResources(new ResourceClutch(2, 2, 2, 2, 2));
      thirdOpponent.AddResources(new ResourceClutch(3, 3, 3, 3, 3));

      var localGameController = new LocalGameController(new Dice(), new PlayerPool(), new GameBoardManager(BoardSizes.Standard));

      PlayerDataView[] playerDataViews = null;
      localGameController.GameLoadedEvent = (PlayerDataView[] pd, GameBoardData bd) => { playerDataViews = pd; };

      // Act
      var streamContent = "<game>" +
        "<players>" +
        "<player id=\"" + player.Id + "\" name=\"" + player.Name + "\" brick=\"" + player.BrickCount + "\" grain=\"" + player.GrainCount + "\" lumber=\"" + player.LumberCount + "\" ore=\"" + player.OreCount + "\" wool=\"" + player.WoolCount + "\" />" +
        "<player id=\"" + firstOpponent.Id + "\" name=\"" + firstOpponent.Name + "\" iscomputer=\"true\" brick=\"" + firstOpponent.BrickCount + "\" grain=\"" + firstOpponent.GrainCount + "\" lumber=\"" + firstOpponent.LumberCount + "\" ore=\"" + firstOpponent.OreCount + "\" wool=\"" + firstOpponent.WoolCount + "\" />" +
        "<player id=\"" + secondOpponent.Id + "\" name=\"" + secondOpponent.Name + "\" iscomputer=\"true\" brick=\"" + secondOpponent.BrickCount + "\" grain=\"" + secondOpponent.GrainCount + "\" lumber=\"" + secondOpponent.LumberCount + "\" ore=\"" + secondOpponent.OreCount + "\" wool=\"" + secondOpponent.WoolCount + "\" />" +
        "<player id=\"" + thirdOpponent.Id + "\" name=\"" + thirdOpponent.Name + "\" iscomputer=\"true\" brick=\"" + thirdOpponent.BrickCount + "\" grain=\"" + thirdOpponent.GrainCount + "\" lumber=\"" + thirdOpponent.LumberCount + "\" ore=\"" + thirdOpponent.OreCount + "\" wool=\"" + thirdOpponent.WoolCount + "\" />" +
        "</players>" +
        "</game>";
      var streamContentBytes = Encoding.UTF8.GetBytes(streamContent);
      using (var stream = new MemoryStream(streamContentBytes))
      {
        localGameController.Load(stream);
      }

      // Assert
      playerDataViews.ShouldNotBeNull();
      playerDataViews.Length.ShouldBe(4);

      this.AssertPlayerDataViewIsCorrect(player, playerDataViews[0]);
      this.AssertPlayerDataViewIsCorrect(firstOpponent, playerDataViews[1]);
      this.AssertPlayerDataViewIsCorrect(secondOpponent, playerDataViews[2]);
      this.AssertPlayerDataViewIsCorrect(thirdOpponent, playerDataViews[3]);
    }

    [Test]
    [Category("All")]
    [Category("LocalGameController")]
    public void Load_PlayerAndInfrastructureData_GameBoardIsAsExpected()
    {
      // Arrange
      Guid playerId = Guid.NewGuid(), firstOpponentId = Guid.NewGuid(), secondOpponentId = Guid.NewGuid(), thirdOpponentId = Guid.NewGuid();
      var localGameController = new LocalGameController(new Dice(), new PlayerPool(), new GameBoardManager(BoardSizes.Standard));

      GameBoardData boardData = null;
      localGameController.GameLoadedEvent = (PlayerDataView[] pd, GameBoardData bd) => { boardData = bd; };

      // Act
      var streamContent = "<game>" +
        "<players>" +
        "<player id=\"" + playerId + "\" name=\"" + PlayerName + "\" brick=\"5\" grain=\"\" />" +
        "<player id=\"" + firstOpponentId + "\" name=\"" + FirstOpponentName + "\" brick=\"6\" />" +
        "<player id=\"" + secondOpponentId + "\" name=\"" + SecondOpponentName + "\" brick=\"7\" />" +
        "<player id=\"" + thirdOpponentId + "\" name=\"" + ThirdOpponentName + "\" brick=\"8\" />" +
        "</players>" +
        "<settlements>" +
        "<settlement playerid=\"" + playerId + "\" location=\"" + MainSettlementOneLocation + "\" />" +
        "<settlement playerid=\"" + playerId + "\" location=\"" + MainSettlementTwoLocation + "\" />" +
        "<settlement playerid=\"" + firstOpponentId + "\" location=\"" + FirstSettlementOneLocation + "\" />" +
        "</settlements>" +
        "<roads>" +
        "<road playerid=\"" + playerId + "\" start=\"" + MainSettlementOneLocation + "\" end=\"" + MainRoadOneEnd + "\" />" +
        "</roads>" +
        "</game>";

      var streamContentBytes = Encoding.UTF8.GetBytes(streamContent);
      using (var stream = new MemoryStream(streamContentBytes))
      {
        localGameController.Load(stream);
      }

      // Assert
      boardData.ShouldNotBeNull();

      var settlements = boardData.GetSettlementInformation();
      settlements.Count.ShouldBe(3);
      settlements.ShouldContainKeyAndValue(MainSettlementOneLocation, playerId);
      settlements.ShouldContainKeyAndValue(MainSettlementTwoLocation, playerId);
      settlements.ShouldContainKeyAndValue(FirstSettlementOneLocation, firstOpponentId);

      var roads = boardData.GetRoadInformation();
      roads.Length.ShouldBe(1);
      roads[0].ShouldBe(new Tuple<UInt32, UInt32, Guid>(MainSettlementOneLocation, MainRoadOneEnd, playerId));
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
      localGameController.ContinueGameSetup(MainSettlementOneLocation, MainRoadOneEnd);
      localGameController.CompleteGameSetup(MainSettlementTwoLocation, MainRoadTwoEnd);
      localGameController.FinalisePlayerTurnOrder();

      return localGameController;
    }

    private LocalGameController CreateLocalGameControllerAndCompleteGameSetup(out MockDice mockDice, out GameBoardManager gameBoardManager, out MockPlayer player, out MockComputerPlayer firstOpponent, out MockComputerPlayer secondOpponent, out MockComputerPlayer thirdOpponent)
    {
      var gameSetupOrder = new[] { 12u, 10u, 8u, 6u };
      var gameTurnOrder = gameSetupOrder;
      mockDice = new MockDiceCreator()
        .AddExplicitDiceRollSequence(gameSetupOrder)
        .AddExplicitDiceRollSequence(gameTurnOrder)
        .Create();

      gameBoardManager = new GameBoardManager(BoardSizes.Standard);

      this.CreateDefaultPlayerInstances(out player, out firstOpponent, out secondOpponent, out thirdOpponent);

      var localGameController = this.CreateLocalGameController(mockDice, gameBoardManager, player, firstOpponent, secondOpponent, thirdOpponent);

      localGameController.JoinGame();
      localGameController.LaunchGame();
      localGameController.StartGameSetup();
      localGameController.ContinueGameSetup(MainSettlementOneLocation, MainRoadOneEnd);
      localGameController.CompleteGameSetup(MainSettlementTwoLocation, MainRoadTwoEnd);
      localGameController.FinalisePlayerTurnOrder();

      return localGameController;
    }

    private void AssertPlayerDataViewIsCorrect(IPlayer player, PlayerDataView playerDataView)
    {
      playerDataView.Id.ShouldBe(player.Id);
      playerDataView.Name.ShouldBe(player.Name);
      playerDataView.DisplayedDevelopmentCards.ShouldBeNull();
      playerDataView.HiddenDevelopmentCards.ShouldBe(0);
      playerDataView.ResourceCards.ShouldBe(player.ResourcesCount);
      playerDataView.IsComputer.ShouldBe(player.IsComputer);
    }

    private LocalGameController CreateLocalGameController(IDice dice, IPlayer firstPlayer, params IPlayer[] otherPlayers)
    {
      var mockPlayerPool = CreatePlayerPool(firstPlayer, otherPlayers);


      var localGameController = new LocalGameControllerCreator()
        .ChangeDice(dice)
        .ChangePlayerPool(mockPlayerPool)
        .Create();

      localGameController.ErrorRaisedEvent = (ErrorDetails e) => { throw new Exception(e.Message); };

      return localGameController;
    }

    private LocalGameController CreateLocalGameController(IDice dice, GameBoardManager gameBoardManager, IPlayer firstPlayer, params IPlayer[] otherPlayers)
    {
      var mockPlayerPool = CreatePlayerPool(firstPlayer, otherPlayers);

      return new LocalGameControllerCreator()
        .ChangeDice(dice)
        .ChangeGameBoardManager(gameBoardManager)
        .ChangePlayerPool(mockPlayerPool)
        .Create();
    }

    private IPlayerPool CreatePlayerPool(IPlayer player, IPlayer[] otherPlayers)
    {
      var mockPlayerPool = Substitute.For<IPlayerPool>();
      mockPlayerPool.CreatePlayer(Arg.Any<Boolean>()).Returns(player, otherPlayers);
      return mockPlayerPool;
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

    private void CreateDefaultPlayerInstances(out MockPlayer player, out MockComputerPlayer firstOpponent, out MockComputerPlayer secondOpponent, out MockComputerPlayer thirdOpponent)
    {
      player = new MockPlayer(PlayerName);

      firstOpponent = new MockComputerPlayer(FirstOpponentName);
      firstOpponent.AddInitialInfrastructureChoices(FirstSettlementOneLocation, FirstRoadOneEnd, FirstSettlementTwoLocation, FirstRoadTwoEnd);

      secondOpponent = new MockComputerPlayer(SecondOpponentName);
      secondOpponent.AddInitialInfrastructureChoices(SecondSettlementOneLocation, SecondRoadOneEnd, SecondSettlementTwoLocation, SecondRoadTwoEnd);

      thirdOpponent = new MockComputerPlayer(ThirdOpponentName);
      thirdOpponent.AddInitialInfrastructureChoices(ThirdSettlementOneLocation, ThirdRoadOneEnd, ThirdSettlementTwoLocation, ThirdRoadTwoEnd);
    }

    private void RunOpponentDataPassBackTests(GameOptions gameOptions)
    {
      var player = new MockPlayer(PlayerName);
      var firstOpponent = new MockComputerPlayer(FirstOpponentName);
      var secondOpponent = new MockComputerPlayer(SecondOpponentName);
      var thirdOpponent = new MockComputerPlayer(ThirdOpponentName);

      var mockPlayerPool = Substitute.For<IPlayerPool>();
      mockPlayerPool.CreatePlayer(true).Returns(player);
      mockPlayerPool.CreatePlayer(false).Returns(firstOpponent, secondOpponent, thirdOpponent);
      var localGameController = new LocalGameControllerCreator()
        .ChangePlayerPool(mockPlayerPool)
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
}
