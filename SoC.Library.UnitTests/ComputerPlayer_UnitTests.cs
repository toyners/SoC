
namespace Jabberwocky.SoC.Library.UnitTests
{
  using System;
  using System.Collections.Generic;
  using GameBoards;
  using NSubstitute;
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

      location.ShouldBe(gameBoardData.Locations[12]);
    }

    [Test]
    public void ChooseSettlementLocation_GetBestLocationOnBoardWithBestLocationUnavailable_ReturnsBestLocation()
    {
      var gameBoardData = new GameBoardData(BoardSizes.Standard);
      gameBoardData.PlaceStartingSettlement(Guid.NewGuid(), 12);

      var computerPlayer = new ComputerPlayer(Guid.NewGuid());

      var location = computerPlayer.ChooseSettlementLocation(gameBoardData);

      location.ShouldBe(gameBoardData.Locations[31]);
    }

    [Test]
    public void ChooseRoad_GetBestRoadFromSettlement_ReturnsBestLocation()
    {
      var playerId = Guid.NewGuid();
      var gameBoardData = new GameBoardData(BoardSizes.Standard);
      gameBoardData.PlaceStartingSettlement(playerId, 12);

      var computerPlayer = new ComputerPlayer(playerId);

      var road = computerPlayer.ChooseRoad(gameBoardData);

      road.ShouldBe(gameBoardData.Trails[16]);
    }
    #endregion 
  }
}
