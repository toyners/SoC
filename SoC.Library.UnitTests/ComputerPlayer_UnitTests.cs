
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
        computerPlayer.ChooseRoad(gameBoardData);
      }).Message.ShouldBe("No settlements found for player with id " + computerPlayer.Id);
    }

    [Test]
    [Category("ComputerPlayer")]
    public void ChooseRoad_BuildingTowardsNextBestSettlementLocation_ReturnsFirstRoadFragment()
    {
      var computerPlayer = new ComputerPlayer("ComputerPlayer");
      var gameBoardData = new GameBoardData(BoardSizes.Standard);
      gameBoardData.PlaceSettlement(computerPlayer.Id, 12);


      var road = computerPlayer.ChooseRoad(gameBoardData);
      var expectedRoad = new Road(11, 12);
      road.ShouldBe(expectedRoad);
    }

    [Test]
    [Category("ComputerPlayer")]
    public void ChooseRoad_BuildingTowardsNextBestSettlementLocationWithFirstRoadPlaced_ReturnsSecondRoadFragment()
    {
      var computerPlayer = new ComputerPlayer("ComputerPlayer");
      var gameBoardData = new GameBoardData(BoardSizes.Standard);
      gameBoardData.PlaceSettlement(computerPlayer.Id, 12);
      gameBoardData.PlaceRoad(computerPlayer.Id, new Road(12, 11));


      var road = computerPlayer.ChooseRoad(gameBoardData);
      var expectedRoad = new Road(11, 21);
      road.ShouldBe(expectedRoad);

    }
    #endregion 
  }
}
