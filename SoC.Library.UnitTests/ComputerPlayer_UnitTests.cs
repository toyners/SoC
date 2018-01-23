
namespace Jabberwocky.SoC.Library.UnitTests
{
  using System;
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
      var computerPlayer = new ComputerPlayer("ComputerPlayer");
      var gameBoardData = new GameBoardData(BoardSizes.Standard);
      gameBoardData.PlaceStartingInfrastructure(computerPlayer.Id, 0, 1);
      gameBoardData.PlaceStartingInfrastructure(computerPlayer.Id, 53, 52);

      var location = computerPlayer.ChooseSettlementLocation(gameBoardData);

      location.ShouldBe(12u);
    }

    [Test]
    public void ChooseSettlementLocation_GetBestLocationOnBoardWithBestLocationUnavailable_ReturnsBestLocation()
    {
      var computerPlayer = new ComputerPlayer("ComputerPlayer");
      var gameBoardData = new GameBoardData(BoardSizes.Standard);
      gameBoardData.PlaceStartingInfrastructure(computerPlayer.Id, 12, 11);
      gameBoardData.PlaceStartingInfrastructure(computerPlayer.Id, 0, 1);

      var location = computerPlayer.ChooseSettlementLocation(gameBoardData);

      location.ShouldBe(31u);
    }

    [Test]
    public void ChooseRoad_NoSettlementsForPlayer_ThrowsMeaningfulException()
    {
      var gameBoardData = new GameBoardData(BoardSizes.Standard);
      var computerPlayer = new ComputerPlayer("ComputerPlayer");

      Should.Throw<Exception>(() =>
      {
        UInt32 startRoadLocation, endRoadLocation;
        computerPlayer.ChooseRoad(gameBoardData, out startRoadLocation, out endRoadLocation);
      }).Message.ShouldBe("No settlements found for player with id " + computerPlayer.Id);
    }

    // [Test] - TODO Turned off until functionality can be completed
    public void ChooseRoad_BuildingTowardsNextBestSettlementLocation_ReturnsFirstRoadFragment()
    {
      var computerPlayer = new ComputerPlayer("ComputerPlayer");
      var gameBoardData = new GameBoardData(BoardSizes.Standard);
      gameBoardData.PlaceStartingInfrastructure(computerPlayer.Id, 0, 1);
      gameBoardData.PlaceStartingInfrastructure(computerPlayer.Id, 12, 11);

      UInt32 roadStartLocation, roadEndLocation;
      computerPlayer.ChooseRoad(gameBoardData, out roadStartLocation, out roadEndLocation);

      var tuple = new Tuple<UInt32, UInt32>(roadStartLocation, roadEndLocation);
      tuple.ShouldBeOneOf(new Tuple<UInt32, UInt32>(11, 21), new Tuple<UInt32, UInt32>(21, 11));
    }

    // [Test] - TODO Turned off until functionality can be completed
    public void ChooseRoad_BuildingTowardsNextBestSettlementLocationWithFirstRoadPlaced_ReturnsSecondRoadFragment()
    {
      var computerPlayer = new ComputerPlayer("ComputerPlayer");
      var gameBoardData = new GameBoardData(BoardSizes.Standard);
      gameBoardData.PlaceStartingInfrastructure(computerPlayer.Id, 0, 1);
      gameBoardData.PlaceStartingInfrastructure(computerPlayer.Id, 12, 11);
      gameBoardData.PlaceRoadSegment(computerPlayer.Id, 11, 21);

      UInt32 roadStartLocation, roadEndLocation;
      computerPlayer.ChooseRoad(gameBoardData, out roadStartLocation, out roadEndLocation);

      var tuple = new Tuple<UInt32, UInt32>(roadStartLocation, roadEndLocation);
      tuple.ShouldBeOneOf(new Tuple<UInt32, UInt32>(20, 21), new Tuple<UInt32, UInt32>(21, 20));
    }
    #endregion 
  }
}
