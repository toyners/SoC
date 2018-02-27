
namespace Jabberwocky.SoC.Library.UnitTests
{
  using System;
  using GameActions;
  using GameBoards;
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
      var computerPlayer = new ComputerPlayer("ComputerPlayer", gameBoard);
      gameBoard.PlaceStartingInfrastructure(computerPlayer.Id, 0, 1);
      gameBoard.PlaceStartingInfrastructure(computerPlayer.Id, 53, 52);

      var location = computerPlayer.ChooseSettlementLocation();

      location.ShouldBe(12u);
    }

    [Test]
    public void ChooseSettlementLocation_GetBestLocationOnBoardWithBestLocationUnavailable_ReturnsBestLocation()
    {
      var gameBoard = new GameBoard(BoardSizes.Standard);
      var computerPlayer = new ComputerPlayer("ComputerPlayer", gameBoard);
      gameBoard.PlaceStartingInfrastructure(computerPlayer.Id, 12, 11);
      gameBoard.PlaceStartingInfrastructure(computerPlayer.Id, 0, 1);

      var location = computerPlayer.ChooseSettlementLocation();

      location.ShouldBe(31u);
    }

    // [Test]
    public void ChooseRoad_NoSettlementsForPlayer_ThrowsMeaningfulException()
    {
      var gameBoard = new GameBoard(BoardSizes.Standard);
      var computerPlayer = new ComputerPlayer("ComputerPlayer", gameBoard);

      /*Should.Throw<Exception>(() =>
      {
        UInt32 startRoadLocation, endRoadLocation;
        computerPlayer.ChooseRoad(gameBoardData, out startRoadLocation, out endRoadLocation);
      }).Message.ShouldBe("No settlements found for player with id " + computerPlayer.Id);*/

      throw new NotImplementedException();
    }

    // [Test]
    public void ChooseRoad_BuildingTowardsNextBestSettlementLocation_ReturnsFirstRoadFragment()
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
    }
    #endregion 
  }
}
