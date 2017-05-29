
namespace Jabberwocky.SoC.Library.UnitTests
{
  using System;
  using System.Collections.Generic;
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

    [Test]
    public void GetPathBetweenLocations_StartAndEndAreSame_ReturnsNull()
    {
      var gameBoardData = new GameBoardData(BoardSizes.Standard);
      var result = gameBoardData.GetPathBetweenLocations(0, 0);
      result.ShouldBeNull();
    }

    [Test]
    [TestCase(1u, 0u)]
    [TestCase(8u, 48u)]
    public void GetPathBetweenLocations_StartAndEndAreNeighbours_ReturnsOneStep(UInt32 endPoint, UInt32 stepIndex)
    {
      var gameBoardData = new GameBoardData(BoardSizes.Standard);
      var result = gameBoardData.GetPathBetweenLocations(0, endPoint);
      result.ShouldBe(new List<UInt32> { stepIndex });
    }

    [Test]
    public void GetPathBetweenLocations_StartAndEndAreNeighbours()
    {
      var gameBoardData = new GameBoardData(BoardSizes.Standard);
      var result = gameBoardData.GetPathBetweenLocations(0, 10);
      result.ShouldBe(new List<UInt32> { 1, 2 });
    }

    [Test]
    public void GetSettlementsForPlayers_EmptyBoard_ReturnsNull()
    {
      var gameBoardData = new GameBoardData(BoardSizes.Standard);
      var settlements = gameBoardData.GetSettlementsForPlayer(Guid.NewGuid());
      settlements.ShouldBeNull();
    }

    [Test]
    public void GetSettlementsForPlayers_PlayerHasNoSettlementsOnBoard_ReturnsNull()
    {
      var gameBoardData = new GameBoardData(BoardSizes.Standard);
      gameBoardData.PlaceStartingSettlement(Guid.NewGuid(), 0);
      var settlements = gameBoardData.GetSettlementsForPlayer(Guid.NewGuid());
      settlements.ShouldBeNull();
    }
    #endregion 
  }
}
