
namespace Jabberwocky.SoC.Library.UnitTests
{
  using System;
  using System.Collections.Generic;
  using GameActions;
  using GameBoards;
  using Interfaces;
  using NSubstitute;
  using NUnit.Framework;
  using Shouldly;

  [TestFixture]
  [Category("All")]
  [Category("ComputerPlayer")]
  public class ComputerPlayer_UnitTests
  {
    #region Methods
    [Test]
    public void ChooseSettlementLocation_GetBestLocationOnEmptyBoard_ReturnsBestLocation()
    {
      var gameBoard = new GameBoard(BoardSizes.Standard);
      var computerPlayer = new ComputerPlayer("ComputerPlayer", gameBoard, this.CreateMockNumberGenerator());
      gameBoard.PlaceStartingInfrastructure(computerPlayer.Id, 0, 1);
      gameBoard.PlaceStartingInfrastructure(computerPlayer.Id, 53, 52);

      var location = computerPlayer.ChooseSettlementLocation();

      location.ShouldBe(12u);
    }

    [Test]
    public void ChooseSettlementLocation_GetBestLocationOnBoardWithBestLocationUnavailable_ReturnsBestLocation()
    {
      var gameBoard = new GameBoard(BoardSizes.Standard);
      var computerPlayer = new ComputerPlayer("ComputerPlayer", gameBoard, this.CreateMockNumberGenerator());
      gameBoard.PlaceStartingInfrastructure(computerPlayer.Id, 12, 11);
      gameBoard.PlaceStartingInfrastructure(computerPlayer.Id, 0, 1);

      var location = computerPlayer.ChooseSettlementLocation();

      location.ShouldBe(31u);
    }

    [Test]
    public void ChooseInitialInfrastructure_RoadBuilderStrategyWithFirstSelection_ReturnFirstChoiceLocation()
    {
      var gameBoard = new GameBoard(BoardSizes.Standard);
      var computerPlayer = new ComputerPlayer("Bob", gameBoard, this.CreateMockNumberGenerator());

      var settlementLocation = 0u;
      var roadEndLocation = 0u;
      computerPlayer.ChooseInitialInfrastructure(out settlementLocation, out roadEndLocation);

      settlementLocation.ShouldBe(35u);
      roadEndLocation.ShouldBe(34u);
    }

    /// <summary>
    /// Another player has taken either the first, second or third best road builder location Lumber 6. Road builder strategy will
    /// take what is best from the remaining viable choices.
    /// </summary>
    [Test]
    [TestCase(24u, 35u, 36u, 46u)]
    [TestCase(36u, 46u, 24u, 35u)]
    [TestCase(35u, 34u, 25u, 24u)]
    public void ChooseInitialInfrastructure_RoadBuilderAlphaStrategyWithSecondSelectionAndBestLumberHexIsOccupied_ReturnBestPossibleLocationOnHex(UInt32 firstSettlementLocation, UInt32 firstRoadEndLocation, UInt32 expectedSettlementLocation, UInt32 expectedRoadEndLocation)
    {
      var gameBoard = new GameBoard(BoardSizes.Standard);
      var computerPlayer = new ComputerPlayer("Bob", gameBoard, this.CreateMockNumberGenerator());

      gameBoard.PlaceStartingInfrastructure(Guid.NewGuid(), firstSettlementLocation, firstRoadEndLocation);

      var settlementLocation = 0u;
      var roadEndLocation = 0u;
      computerPlayer.ChooseInitialInfrastructure(out settlementLocation, out roadEndLocation);

      settlementLocation.ShouldBe(expectedSettlementLocation);
      roadEndLocation.ShouldBe(expectedRoadEndLocation);
    }

    /// <summary>
    /// Two other players take positions on the lumber 6 hex. Take what is left.
    /// </summary>
    /// <param name="infrastructureData"></param>
    /// <param name="expectedSettlementLocation"></param>
    /// <param name="expectedRoadEndLocation"></param>
    [Test]
    [TestCase(new UInt32[] { 35, 34, 25, 24 }, 37u, 36u)]
    [TestCase(new UInt32[] { 24, 23, 36, 46 }, 26u, 25u)]
    public void ChooseInitialInfrastructure_RoadBuilderAlphaStrategyWithThirdSelectionAndBestLumberHexIsOccupied_ReturnBestPossibleLocationOnHex(UInt32[] infrastructureData, UInt32 expectedSettlementLocation, UInt32 expectedRoadEndLocation)
    {
      var gameBoard = new GameBoard(BoardSizes.Standard);
      var computerPlayer = new ComputerPlayer("Bob", gameBoard, this.CreateMockNumberGenerator());

      this.PlaceInfrastructure(gameBoard, infrastructureData);

      var settlementLocation = 0u;
      var roadEndLocation = 0u;
      computerPlayer.ChooseInitialInfrastructure(out settlementLocation, out roadEndLocation);

      settlementLocation.ShouldBe(expectedSettlementLocation);
      roadEndLocation.ShouldBe(expectedRoadEndLocation);
    }

    [Test]
    [TestCase(new UInt32[] { 35, 34, 25, 24, 37, 36 }, 42u, 43u)]
    [TestCase(new UInt32[] { 24, 23, 36, 46, 26, 25 }, 42u, 43u)]
    public void ChooseInitialInfrastructure_ChangeToRoadBuilderBetaStrategyWhenInFourthSlotAndBestLumberHexIsFull_ReturnBestPossibleLocationOnHex(UInt32[] infrastructureData, UInt32 expectedSettlementLocation, UInt32 expectedRoadEndLocation)
    {
      var gameBoard = new GameBoard(BoardSizes.Standard);
      var computerPlayer = new ComputerPlayer("Bob", gameBoard, this.CreateMockNumberGenerator());

      this.PlaceInfrastructure(gameBoard, infrastructureData);

      var settlementLocation = 0u;
      var roadEndLocation = 0u;
      computerPlayer.ChooseInitialInfrastructure(out settlementLocation, out roadEndLocation);

      settlementLocation.ShouldBe(expectedSettlementLocation);
      roadEndLocation.ShouldBe(expectedRoadEndLocation);
    }

    [Test]
    [TestCase(new UInt32[] { 35, 34, 25, 24, 37, 36, 42, 43 }, 11u, 21u)]
    [TestCase(new UInt32[] { 35, 34, 25, 24, 37, 36, 42, 43, 11, 21 }, 4u, 12u)]
    [TestCase(new UInt32[] { 35, 34, 25, 24, 37, 36, 42, 43, 11, 21, 4, 12 }, 2u, 10u)]
    [TestCase(new UInt32[] { 35, 34, 25, 24, 37, 36, 42, 43, 11, 21, 4, 12, 2, 10 }, 31u, 30u)]
    public void ChooseInitialInfrastructure_ChangeToRoadBuilderBetaStrategyWhenInFifthSlot_ReturnBestPossibleLocationOnHex(UInt32[] infrastructureData, UInt32 expectedSettlementLocation, UInt32 expectedRoadEndLocation)
    {
      var gameBoard = new GameBoard(BoardSizes.Standard);
      var computerPlayer = new ComputerPlayer("Bob", gameBoard, this.CreateMockNumberGenerator());

      this.PlaceInfrastructure(gameBoard, infrastructureData);

      var settlementLocation = 0u;
      var roadEndLocation = 0u;
      computerPlayer.ChooseInitialInfrastructure(out settlementLocation, out roadEndLocation);

      settlementLocation.ShouldBe(expectedSettlementLocation);
      roadEndLocation.ShouldBe(expectedRoadEndLocation);
    }

    /* // [Test]
    public void ChooseRoad_NoSettlementsForPlayer_ThrowsMeaningfulException()
    {
      var gameBoard = new GameBoard(BoardSizes.Standard);
      var computerPlayer = new ComputerPlayer("ComputerPlayer", gameBoard);

      /*Should.Throw<Exception>(() =>
      {
        UInt32 startRoadLocation, endRoadLocation;
        computerPlayer.ChooseRoad(gameBoardData, out startRoadLocation, out endRoadLocation);
      }).Message.ShouldBe("No settlements found for player with id " + computerPlayer.Id);*/

    //throw new NotImplementedException();
    //}

    // [Test]
    /*public void ChooseRoad_BuildingTowardsNextBestSettlementLocation_ReturnsFirstRoadFragment()
    {
      var gameBoard = new GameBoard(BoardSizes.Standard);
      var computerPlayer = new ComputerPlayer("ComputerPlayer", gameBoard);
      gameBoard.PlaceStartingInfrastructure(computerPlayer.Id, 12, 4);
      gameBoard.PlaceStartingInfrastructure(computerPlayer.Id, 40, 39);

      computerPlayer.AddResources(ResourceClutch.RoadSegment);
      computerPlayer.BuildInitialPlayerActions(null);
      var firstAction = computerPlayer.GetPlayerAction();
      var endAction = computerPlayer.GetPlayerAction();

      firstAction.ShouldBeOfType<BuildRoadSegmentAction>();
      var buildRoadSegmentsAction = (BuildRoadSegmentAction)firstAction;
      
      endAction.ShouldBeNull();

      UInt32 roadStartLocation, roadEndLocation;
      //computerPlayer.ChooseRoad(gameBoard, out roadStartLocation, out roadEndLocation);

      //var tuple = new Tuple<UInt32, UInt32>(roadStartLocation, roadEndLocation);
      //tuple.ShouldBeOneOf(new Tuple<UInt32, UInt32>(11, 21), new Tuple<UInt32, UInt32>(21, 11));
      throw new NotImplementedException();
    }

    // [Test] - TODO Turned off until functionality can be completed
    public void ChooseRoad_BuildingTowardsNextBestSettlementLocationWithFirstRoadPlaced_ReturnsSecondRoadFragment()
    {
      var gameBoard = new GameBoard(BoardSizes.Standard);
      var computerPlayer = new ComputerPlayer("ComputerPlayer", gameBoard);
      gameBoard.PlaceStartingInfrastructure(computerPlayer.Id, 0, 1);
      gameBoard.PlaceStartingInfrastructure(computerPlayer.Id, 12, 11);
      gameBoard.PlaceRoadSegment(computerPlayer.Id, 11, 21);

      UInt32 roadStartLocation, roadEndLocation;
      //computerPlayer.ChooseRoad(gameBoardData, out roadStartLocation, out roadEndLocation);

      //var tuple = new Tuple<UInt32, UInt32>(roadStartLocation, roadEndLocation);
      //tuple.ShouldBeOneOf(new Tuple<UInt32, UInt32>(20, 21), new Tuple<UInt32, UInt32>(21, 20));
      throw new NotImplementedException();
    }*/

    private INumberGenerator CreateMockNumberGenerator(params Int32[] values)
    {
      var mockNumberGenerator = Substitute.For<INumberGenerator>();
      var index = 0;
      if (values != null && values.Length > 0)
      {
        mockNumberGenerator.GetRandomNumberBetweenZeroAndMaximum(100).Returns(x => 
        {
          return values[index++];
        });
      }
      
      return mockNumberGenerator;
    }

    private void PlaceInfrastructure(GameBoard gameBoard, UInt32[] data)
    {
      var playerIds = new List<Guid>();
      var playerId = Guid.Empty;
      var playerIdIndex = 0;
      for (var i = 0; i < data.Length; i += 2)
      {
        if (playerIds.Count < 4)
        {
          playerId = Guid.NewGuid();
          playerIds.Add(playerId);
        }
        else if (playerIdIndex == playerIds.Count)
        {
          playerIdIndex = 0;
          playerId = playerIds[playerIdIndex++];
        }
        else
        {
          playerId = playerIds[playerIdIndex++];
        }

        gameBoard.PlaceStartingInfrastructure(playerId, data[i], data[i + 1]);
      }
    }
    #endregion 
  }
}
