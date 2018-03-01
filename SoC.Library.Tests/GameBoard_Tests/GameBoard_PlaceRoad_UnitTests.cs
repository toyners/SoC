
namespace Jabberwocky.SoC.Library.UnitTests.GameBoard_Tests
{
  using System;
  using GameBoards;
  using NUnit.Framework;
  using Shouldly;

  [TestFixture]
  [Category("All")]
  [Category("GameBoard")]
  [Category("GameBoard.PlaceRoad")]
  public class GameBoard_PlaceRoad_UnitTests : GameBoardTestBase
  {
    [Test]
    public void PlaceRoad_EmptyBoard_ThrowsMeaningfulException()
    {
      // Arrange
      var gameBoardData = new GameBoard(BoardSizes.Standard);

      // Act
      Action action = () => { gameBoardData.PlaceRoadSegment(Guid.NewGuid(), 0, 1); };

      // Assert
      action.ShouldThrow<GameBoard.PlacementException>().Message.ShouldBe("Cannot place road before placing all initial infrastructure.");
    }

    [Test]
    public void PlaceRoad_OnlyPlacedFirstStartingInfrastructure_ThrowsMeaningfulException()
    {
      // Arrange
      var playerId = Guid.NewGuid();
      var gameBoardData = new GameBoard(BoardSizes.Standard);
      gameBoardData.PlaceStartingInfrastructure(playerId, FirstPlayerSettlementLocation, FirstPlayerRoadEndLocation);

      // Act
      Action action = () => { gameBoardData.PlaceRoadSegment(playerId, 10, FirstPlayerRoadEndLocation); };

      // Assert
      action.ShouldThrow<GameBoard.PlacementException>().Message.ShouldBe("Cannot place road before placing all initial infrastructure.");
    }

    [Test]
    [TestCase(53u, 54u)] // Hanging over the edge 
    [TestCase(54u, 53u)] // Hanging over the edge
    [TestCase(100u, 101u)]
    public void PlaceRoad_OffBoard_ThrowsMeaningfulException(UInt32 start, UInt32 end)
    {
      // Arrange
      var playerId = Guid.NewGuid();
      var gameBoardData = new GameBoard(BoardSizes.Standard);
      gameBoardData.PlaceStartingInfrastructure(playerId, FirstPlayerSettlementLocation, FirstPlayerRoadEndLocation);
      gameBoardData.PlaceStartingInfrastructure(playerId, SecondPlayerSettlementLocation, SecondPlayerRoadEndLocation);

      // Act
      Action action = () => { gameBoardData.PlaceRoadSegment(playerId, start, end); };

      // Assert
      action.ShouldThrow<GameBoard.PlacementException>().Message.ShouldBe("Cannot place road because board location is not valid.");
    }

    [Test]
    [TestCase(43u, 53u)]
    [TestCase(53u, 43u)]
    public void PlaceRoad_NoDirectConnection_ThrowsMeaningfulException(UInt32 start, UInt32 end)
    {
      // Arrange
      var playerId = Guid.NewGuid();
      var gameBoardData = new GameBoard(BoardSizes.Standard);
      gameBoardData.PlaceStartingInfrastructure(playerId, FirstPlayerSettlementLocation, FirstPlayerRoadEndLocation);
      gameBoardData.PlaceStartingInfrastructure(playerId, SecondPlayerSettlementLocation, SecondPlayerRoadEndLocation);

      // Act
      Action action = () => { gameBoardData.PlaceRoadSegment(playerId, start, end); };

      // Assert
      action.ShouldThrow<GameBoard.PlacementException>().Message.ShouldBe("Cannot place road because no direct connection between start location and end location.");
    }

    [Test]
    public void PlaceRoad_RoadAlreadyBuilt_ThrowsMeaningfulException()
    {
      // Arrange
      var gameBoardData = new GameBoard(BoardSizes.Standard);
      var playerId = Guid.NewGuid();
      gameBoardData.PlaceStartingInfrastructure(playerId, FirstPlayerSettlementLocation, FirstPlayerRoadEndLocation);
      gameBoardData.PlaceStartingInfrastructure(playerId, SecondPlayerSettlementLocation, SecondPlayerRoadEndLocation);

      // Act
      Action action = () => { gameBoardData.PlaceRoadSegment(playerId, FirstPlayerSettlementLocation, FirstPlayerRoadEndLocation); };

      // Assert
      action.ShouldThrow<GameBoard.PlacementException>().Message.ShouldBe("Cannot place road because road already exists.");
    }

    [Test]
    [TestCase(2u, 3u)]
    [TestCase(8u, 9u)]
    public void PlaceRoad_RoadNotConnectedToExistingInfrastructure_ThrowsMeaningfulException(UInt32 roadStartLocation, UInt32 roadEndLocation)
    {
      // Arrange
      var playerId = Guid.NewGuid();
      var gameBoardData = new GameBoard(BoardSizes.Standard);
      gameBoardData.PlaceStartingInfrastructure(playerId, FirstPlayerSettlementLocation, FirstPlayerRoadEndLocation);
      gameBoardData.PlaceStartingInfrastructure(playerId, SecondPlayerSettlementLocation, SecondPlayerRoadEndLocation);

      // Act
      Action action = () => { gameBoardData.PlaceRoadSegment(playerId, roadStartLocation, roadEndLocation); };

      // Assert
      action.ShouldThrow<GameBoard.PlacementException>().Message.ShouldBe("Cannot place road because it is not connected to an existing road segment.");
    }
  }
}
