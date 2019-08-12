
namespace Jabberwocky.SoC.Library.UnitTests.GameBoard_Tests
{
  using System;
  using GameBoards;
  using NUnit.Framework;
  using Shouldly;

  [TestFixture]
  [Category("All")]
  [Category("GameBoard")]
  [Category("GameBoard.CanPlaceRoad")]
  public class GameBoard_CanPlaceRoad_UnitTests : GameBoardTestBase
  {
    #region Methods
    [Test]
    public void CanPlaceRoad_ConnectedToRoad_ReturnsValid()
    {
      // Arrange
      var playerId = Guid.NewGuid();
      var gameBoardData = new GameBoard(BoardSizes.Standard);
      gameBoardData.PlaceStartingInfrastructure(playerId, FirstPlayerSettlementLocation, FirstPlayerRoadEndLocation);
      gameBoardData.PlaceStartingInfrastructure(playerId, SecondPlayerSettlementLocation, SecondPlayerRoadEndLocation);

      // Act
      var result = gameBoardData.CanPlaceRoad(playerId, 10, FirstPlayerRoadEndLocation);

      // Assert
      result.ShouldBe(PlacementStatusCodes.Success);
    }

    [Test]
    public void CanPlaceRoad_EmptyBoard_ReturnsStartingInfrastructureNotPresentWhenPlacingRoad()
    {
      // Arrange
      var gameBoardData = new GameBoard(BoardSizes.Standard);

      // Act
      var result = gameBoardData.CanPlaceRoad(Guid.NewGuid(), 0, 1);

      // Assert
      result.ShouldBe(PlacementStatusCodes.StartingInfrastructureNotPresentWhenPlacingRoad);
    }

    [Test]
    public void CanPlaceRoad_OnlyPlacedFirstStartingInfrastructure_StartingInfrastructureNotCompleteWhenPlacingRoad()
    {
      // Arrange
      var playerId = Guid.NewGuid();
      var gameBoardData = new GameBoard(BoardSizes.Standard);
      gameBoardData.PlaceStartingInfrastructure(playerId, FirstPlayerSettlementLocation, FirstPlayerRoadEndLocation);

      // Act
      var result = gameBoardData.CanPlaceRoad(playerId, 10, FirstPlayerRoadEndLocation);

      // Assert
      result.ShouldBe(PlacementStatusCodes.StartingInfrastructureNotCompleteWhenPlacingRoad);
    }

    [Test]
    [TestCase(10u, 11u)]
    [TestCase(11u, 10u)]
    public void CanPlaceRoad_JoiningToOtherRoads_ReturnsValid(UInt32 roadStartLocation, UInt32 roadEndLocation)
    {
      // Arrange
      var gameBoardData = new GameBoard(BoardSizes.Standard);
      var playerId = Guid.NewGuid();
      gameBoardData.PlaceStartingInfrastructure(playerId, FirstPlayerSettlementLocation, FirstPlayerRoadEndLocation);
      gameBoardData.PlaceStartingInfrastructure(playerId, SecondPlayerSettlementLocation, SecondPlayerRoadEndLocation);

      // Act
      var result = gameBoardData.CanPlaceRoad(playerId, roadStartLocation, roadEndLocation);

      // Assert
      result.ShouldBe(PlacementStatusCodes.Success);
    }

    [Test]
    [TestCase(53u, 54u)] // Hanging over the edge 
    [TestCase(54u, 53u)] // Hanging over the edge
    [TestCase(100u, 101u)]
    public void CanPlaceRoad_OffBoard_ReturnsRoadIsInvalid(UInt32 start, UInt32 end)
    {
      // Arrange
      var playerId = Guid.NewGuid();
      var gameBoardData = new GameBoard(BoardSizes.Standard);
      gameBoardData.PlaceStartingInfrastructure(playerId, FirstPlayerSettlementLocation, FirstPlayerRoadEndLocation);
      gameBoardData.PlaceStartingInfrastructure(playerId, SecondPlayerSettlementLocation, SecondPlayerRoadEndLocation);

      // Act
      var result = gameBoardData.CanPlaceRoad(playerId, start, end);

      // Assert
      result.ShouldBe(PlacementStatusCodes.RoadIsOffBoard);
    }

    [Test]
    [TestCase(43u, 53u)]
    [TestCase(53u, 43u)]
    public void CanPlaceRoad_NoDirectConnection_ReturnsNoDirectConnection(UInt32 start, UInt32 end)
    {
      // Arrange
      var playerId = Guid.NewGuid();
      var gameBoardData = new GameBoard(BoardSizes.Standard);
      gameBoardData.PlaceStartingInfrastructure(playerId, FirstPlayerSettlementLocation, FirstPlayerRoadEndLocation);
      gameBoardData.PlaceStartingInfrastructure(playerId, SecondPlayerSettlementLocation, SecondPlayerRoadEndLocation);

      // Act
      var result = gameBoardData.CanPlaceRoad(playerId, start, end);

      // Assert
      result.ShouldBe(PlacementStatusCodes.NoDirectConnection);
    }

    [Test]
    public void CanPlaceRoad_RoadAlreadyBuilt_ReturnsRoadIsOccupied()
    {
      // Arrange
      var gameBoardData = new GameBoard(BoardSizes.Standard);
      var playerId = Guid.NewGuid();
      gameBoardData.PlaceStartingInfrastructure(playerId, FirstPlayerSettlementLocation, FirstPlayerRoadEndLocation);
      gameBoardData.PlaceStartingInfrastructure(playerId, SecondPlayerSettlementLocation, SecondPlayerRoadEndLocation);

      var result = gameBoardData.CanPlaceRoad(playerId, FirstPlayerSettlementLocation, FirstPlayerRoadEndLocation);
      result.ShouldBe(PlacementStatusCodes.RoadIsOccupied);
    }

    [Test]
    [TestCase(2u, 3u)]
    [TestCase(8u, 9u)]
    public void CanPlaceRoad_RoadNotConnectedToExistingInfrastructure_ReturnsRoadNotConnectedToExistingRoad(UInt32 roadStartLocation, UInt32 roadEndLocation)
    {
      // Arrange
      var playerId = Guid.NewGuid();
      var gameBoardData = new GameBoard(BoardSizes.Standard);
      gameBoardData.PlaceStartingInfrastructure(playerId, FirstPlayerSettlementLocation, FirstPlayerRoadEndLocation);
      gameBoardData.PlaceStartingInfrastructure(playerId, SecondPlayerSettlementLocation, SecondPlayerRoadEndLocation);

      // Act
      var result = gameBoardData.CanPlaceRoad(playerId, roadStartLocation, roadEndLocation);

      // Assert
      result.ShouldBe(PlacementStatusCodes.RoadNotConnectedToExistingRoad);
    }
    #endregion
  }
}
