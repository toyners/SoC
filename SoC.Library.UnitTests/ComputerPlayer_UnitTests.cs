
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
    public void ChooseSettlementLocation_GetBestLocationOnEmptyBoard_ReturnsBestLocation()
    {
      var gameBoardData = new GameBoardData(BoardSizes.Standard);
      var computerPlayer = new ComputerPlayer(Guid.NewGuid());

      var location = computerPlayer.ChooseSettlementLocation(gameBoardData);

      location.ShouldBe(12u);
    }

    [Test]
    public void ChooseSettlementLocation_GetBestLocationOnBoardWithBestLocationUnavailable_ReturnsBestLocation()
    {
      var gameBoardData = new GameBoardData(BoardSizes.Standard);
      gameBoardData.PlaceStartingSettlement(Guid.NewGuid(), 12);

      var computerPlayer = new ComputerPlayer(Guid.NewGuid());

      var location = computerPlayer.ChooseSettlementLocation(gameBoardData);

      location.ShouldBe(31u);
    }

    [Test]
    public void ChooseRoad_NoSettlementsForPlayer_ThrowsMeaningfulException()
    {
      var playerId = Guid.NewGuid();
      var gameBoardData = new GameBoardData(BoardSizes.Standard);
      var computerPlayer = new ComputerPlayer(playerId);

      Should.Throw<Exception>(() =>
      {
        computerPlayer.ChooseRoad(gameBoardData);
      }).Message.ShouldBe("No settlements found for player with id " + playerId);
    }

    [Test]
    public void ChooseRoad_BuildingTowardsNextBestSettlementLocation_ReturnsFirstRoadTowardsBestLocation()
    {
      var playerId = Guid.NewGuid();
      var gameBoardData = new GameBoardData(BoardSizes.Standard);
      gameBoardData.PlaceStartingSettlement(playerId, 12);

      var computerPlayer = new ComputerPlayer(playerId);

      var road = computerPlayer.ChooseRoad(gameBoardData);
      var expectedRoad = new Road(11, 12);
      road.ShouldBe(expectedRoad);
    }
    #endregion 
  }
}
