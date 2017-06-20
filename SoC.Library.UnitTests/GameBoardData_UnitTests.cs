
namespace Jabberwocky.SoC.Library.UnitTests
{
  using System;
  using System.Collections.Generic;
  using GameBoards;
  using NUnit.Framework;
  using Shouldly;

  [TestFixture]
  public class GameBoardData_UnitTests
  {
    #region Methods
    [Test]
    public void CanPlaceSettlement_EmptyBoard_ReturnsNull()
    {
      var gameBoardData = new GameBoardData(BoardSizes.Standard);
      var result = gameBoardData.CanPlaceSettlement(0);
      result.ShouldBeNull();
    }

    [Test]
    public void CanPlaceSettlement_EmptyBoard_ReturnsEmptyOccupyingPlayer()
    {
      var gameBoardData = new GameBoardData(BoardSizes.Standard);
      Guid playerId = Guid.NewGuid();
      gameBoardData.CanPlaceSettlement(0, out playerId);
      playerId.ShouldBe(Guid.Empty);
    }

    [Test]
    public void Test()
    {
      var gameBoardData = new GameBoardData(BoardSizes.Standard);
      var connections = gameBoardData.GetBoardSnapshot();

      connections[0, 0].ShouldBeFalse();
      connections[0, 1].ShouldBeTrue();
      connections[1, 0].ShouldBeTrue();
    }

    [Test]
    public void CanPlaceSettlement_TryPlacingOnSettledLocation_ReturnsLocationIsOccupied()
    {
      var gameBoardData = new GameBoardData(BoardSizes.Standard);
      gameBoardData.PlaceStartingSettlement(Guid.NewGuid(), 0);
      Guid playerId;
      var result = gameBoardData.CanPlaceSettlement(0, out playerId);
      result.ShouldBe(GameBoardData.VerificationResults.LocationIsOccupied);
    }

    [Test]
    public void CanPlaceSettlement_TryPlacingOnSettledLocation_ReturnsOwnerOfOccupiedLocation()
    {
      var playerId = Guid.NewGuid();
      var gameBoardData = new GameBoardData(BoardSizes.Standard);
      gameBoardData.PlaceStartingSettlement(playerId, 0);
      Guid occupyingPlayerId = Guid.Empty;
      var result = gameBoardData.CanPlaceSettlement(0, out occupyingPlayerId);
      occupyingPlayerId.ShouldBe(playerId);
    }

    [Test]
    [TestCase(19u)]
    [TestCase(21u)]
    [TestCase(31u)]
    public void CanPlaceSettlement_TryPlacingNextToSettledLocation_ReturnsCorrectVerificationCode(UInt32 settlementIndex)
    {
      var playerId = Guid.NewGuid();
      var gameBoardData = new GameBoardData(BoardSizes.Standard);
      gameBoardData.PlaceStartingSettlement(playerId, 20);
      Guid occupyingPlayerId = Guid.Empty;
      var result = gameBoardData.CanPlaceSettlement(settlementIndex, out occupyingPlayerId);
      result.ShouldBe(GameBoardData.VerificationResults.TooCloseToSettlement);
    }

    [Test]
    [TestCase(19u)]
    [TestCase(21u)]
    [TestCase(31u)]
    public void CanPlaceSettlement_TryPlacingNextToSettledLocation_ReturnsOwnerOfOccupiedLocation(UInt32 settlementIndex)
    {
      var playerId = Guid.NewGuid();
      var gameBoardData = new GameBoardData(BoardSizes.Standard);
      gameBoardData.PlaceStartingSettlement(playerId, 20);
      Guid occupyingPlayerId = Guid.Empty;
      var result = gameBoardData.CanPlaceSettlement(settlementIndex, out occupyingPlayerId);
      occupyingPlayerId.ShouldBe(playerId);
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
      result.ShouldBe(new List<UInt32> { endPoint });
    }

    [Test]
    public void GetPathBetweenLocations_StartAndEndAreNeighbours()
    {
      var gameBoardData = new GameBoardData(BoardSizes.Standard);
      var result = gameBoardData.GetPathBetweenLocations(0, 10);
      result.ShouldBe(new List<UInt32> { 10, 2, 1 });
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
