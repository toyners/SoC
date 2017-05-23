
namespace Jabberwocky.SoC.Library.UnitTests
{
  using System;
  using NSubstitute;
  using NUnit.Framework;
  using Shouldly;

  [TestFixture]
  public class ComputerPlayer_UnitTests
  {
    #region Methods
    [Test]
    public void ChooseSettlementLocation_GetBestLocation_ReturnsBestLocation()
    {
      var gameBoardData = new GameBoards.GameBoardData(BoardSizes.Standard);
      var computerPlayer = new ComputerPlayer();

      var location = computerPlayer.ChooseSettlementLocation(gameBoardData);

      location.ShouldBe(gameBoardData.Locations[19]);
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
