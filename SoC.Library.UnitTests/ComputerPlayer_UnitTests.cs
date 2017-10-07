
namespace Jabberwocky.SoC.Library.UnitTests
{
  using System;
  using GameBoards;
  using NUnit.Framework;
  using Shouldly;

  [TestFixture]
  public class ComputerPlayer_UnitTests
  {
    #region Methods
    [Test]
    [Category("ComputerPlayer")]
    public void ChooseSettlementLocation_GetBestLocationOnEmptyBoard_ReturnsBestLocation()
    {
      var gameBoardData = new GameBoardData(BoardSizes.Standard);
      var computerPlayer = new ComputerPlayer("ComputerPlayer");

      var location = computerPlayer.ChooseSettlementLocation(gameBoardData);

      location.ShouldBe(12u);
    }

    [Test]
    [Category("ComputerPlayer")]
    public void ChooseSettlementLocation_GetBestLocationOnBoardWithBestLocationUnavailable_ReturnsBestLocation()
    {
      var gameBoardData = new GameBoardData(BoardSizes.Standard);
      gameBoardData.PlaceSettlement(Guid.NewGuid(), 12);

      var computerPlayer = new ComputerPlayer("ComputerPlayer");

      var location = computerPlayer.ChooseSettlementLocation(gameBoardData);

      location.ShouldBe(31u);
    }

    [Test]
    [Category("ComputerPlayer")]
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

    [Test]
    [Category("ComputerPlayer")]
    public void ChooseRoad_BuildingTowardsNextBestSettlementLocation_ReturnsFirstRoadFragment()
    {
      throw new NotImplementedException();

      // TODO: Not sure about this - needs to be revisited
      /*var computerPlayer = new ComputerPlayer("ComputerPlayer");
      var gameBoardData = new GameBoardData(BoardSizes.Standard);
      gameBoardData.PlaceSettlement(computerPlayer.Id, 12);

      var road = computerPlayer.ChooseRoad(gameBoardData);
      var expectedRoad = new RoadSegment(11, 12);
      road.ShouldBe(expectedRoad);*/
    }

    [Test]
    [Category("ComputerPlayer")]
    public void ChooseRoad_BuildingTowardsNextBestSettlementLocationWithFirstRoadPlaced_ReturnsSecondRoadFragment()
    {
      throw new NotImplementedException();

      // TODO: Not sure about this - needs to be revisited
      /*var computerPlayer = new ComputerPlayer("ComputerPlayer");
      var gameBoardData = new GameBoardData(BoardSizes.Standard);
      gameBoardData.PlaceSettlement(computerPlayer.Id, 12);
      gameBoardData.PlaceRoad(computerPlayer.Id, new RoadSegment(12, 11));


      var road = computerPlayer.ChooseRoad(gameBoardData);
      var expectedRoad = new RoadSegment(11, 21);
      road.ShouldBe(expectedRoad);*/
    }
    #endregion 
  }
}
