
namespace Jabberwocky.SoC.Library.UnitTests
{
  using System;
  using System.Collections.Generic;
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
      var gameBoardData = new GameBoards.GameBoardData(BoardSizes.Standard);
      var computerPlayer = new ComputerPlayer();

      var location = computerPlayer.ChooseSettlementLocation(gameBoardData);

      location.ShouldBe(gameBoardData.Locations[12]);
    }

    [Test]
    public void ChooseSettlementLocation_GetBestLocationOnBoardWithBestLocationUnavailable_ReturnsBestLocation()
    {
      var gameBoardData = new GameBoards.GameBoardData(BoardSizes.Standard);
      gameBoardData.Settlements = new Dictionary<Location, Guid>
      {
        { gameBoardData.Locations[12], Guid.NewGuid() }
      };

      var computerPlayer = new ComputerPlayer();

      var location = computerPlayer.ChooseSettlementLocation(gameBoardData);

      location.ShouldBe(gameBoardData.Locations[31]);
    }

    [Test]
    public void ChooseRoad_GetBestLocation_ReturnsBestLocation()
    {
      var gameBoardData = new GameBoards.GameBoardData(BoardSizes.Standard);
      var computerPlayer = new ComputerPlayer();

      var road = computerPlayer.ChooseRoad(gameBoardData);

      road.ShouldBe(gameBoardData.Trails[16]);
    }
    #endregion 
  }
}
