
namespace Jabberwocky.SoC.Library.UnitTests
{
  using System;
  using GameBoards;
  using NSubstitute;
  using NUnit.Framework;
  using Shouldly;

  [TestFixture]
  public class GameBoardData_UnitTests
  {
    #region Methods
    [Test]
    public void CanPlaceSettlement_EmptyBoard_ReturnsValid()
    {
      var gameBoardData = new GameBoardData(BoardSizes.Standard);
      var result = gameBoardData.CanPlaceSettlement(0);
      result.ShouldBe(GameBoardData.VerificationResults.Valid);
    }

    [Test]
    public void CanPlaceSettlement_TryPlacingOnSettledLocation_ReturnsLocationIsOccupied()
    {
      var gameBoardData = new GameBoardData(BoardSizes.Standard);
      gameBoardData.PlaceStartingSettlement(Guid.NewGuid(), 0);
      var result = gameBoardData.CanPlaceSettlement(0);
      result.ShouldBe(GameBoardData.VerificationResults.LocationIsOccupied);
    }
    #endregion 
  }
}
