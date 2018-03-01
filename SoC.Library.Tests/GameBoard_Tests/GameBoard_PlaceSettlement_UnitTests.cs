
namespace Jabberwocky.SoC.Library.UnitTests.GameBoard_Tests
{
  using System;
  using GameBoards;
  using NUnit.Framework;
  using Shouldly;

  [TestFixture]
  [Category("All")]
  [Category("GameBoard")]
  [Category("GameBoard.PlaceSettlement")]
  public class GameBoard_PlaceSettlement_UnitTests : GameBoardTestBase
  {
    [Test]
    public void PlaceSettlement_EmptyBoard_ThrowsMeaningfulException()
    {
      // Arrange
      var gameBoardData = new GameBoard(BoardSizes.Standard);

      // Act
      Action action = () => { gameBoardData.PlaceSettlement(Guid.NewGuid(), 20u); };

      // Assert
      action.ShouldThrow<GameBoard.PlacementException>().Message.ShouldBe("Cannot place settlement before placing all initial infrastructure.");
    }

    [Test]
    public void PlaceSettlement_OnlyPlacedFirstStartingInfrastructure_ThrowsMeaningfulException()
    {
      // Arrange
      var playerId = Guid.NewGuid();
      var gameBoardData = new GameBoard(BoardSizes.Standard);
      gameBoardData.PlaceStartingInfrastructure(playerId, FirstPlayerSettlementLocation, FirstPlayerRoadEndLocation);

      // Act
      Action action = () => { gameBoardData.PlaceSettlement(playerId, 0); };

      // Assert
      action.ShouldThrow<GameBoard.PlacementException>().Message.ShouldBe("Cannot place settlement before placing all initial infrastructure.");
    }

    [Test]
    public void PlaceSettlement_TryPlacingOnSettledLocation_ThrowsMeaningfulException()
    {
      // Arrange
      var playerId = Guid.NewGuid();
      var gameBoardData = new GameBoard(BoardSizes.Standard);
      gameBoardData.PlaceStartingInfrastructure(playerId, FirstPlayerSettlementLocation, FirstPlayerRoadEndLocation);
      gameBoardData.PlaceStartingInfrastructure(playerId, SecondPlayerSettlementLocation, SecondPlayerRoadEndLocation);

      // Act
      Action action = () => { gameBoardData.PlaceSettlement(playerId, FirstPlayerSettlementLocation); };

      // Assert
      action.ShouldThrow<GameBoard.PlacementException>().Message.ShouldBe("Cannot place settlement because location is already settled.");
    }

    [Test]
    public void PlaceSettlement_TryPlacingOnInvalidLocation_ThrowsMeaningfulException()
    {
      // Arrange
      var playerId = Guid.NewGuid();
      var gameBoardData = new GameBoard(BoardSizes.Standard);
      gameBoardData.PlaceStartingInfrastructure(playerId, FirstPlayerSettlementLocation, FirstPlayerRoadEndLocation);
      gameBoardData.PlaceStartingInfrastructure(playerId, SecondPlayerSettlementLocation, SecondPlayerRoadEndLocation);

      // Act
      Action action = () => { gameBoardData.PlaceSettlement(playerId, 100); };

      // Assert
      action.ShouldThrow<GameBoard.PlacementException>().Message.ShouldBe("Cannot place settlement because location is not on board.");
    }

    [Test]
    [TestCase(4u)]
    [TestCase(11u)]
    [TestCase(13u)]
    public void PlaceSettlement_TryPlacingNextToSettledLocation_ThrowsMeaningfulException(UInt32 newSettlementLocation)
    {
      // Arrange
      var playerId = Guid.NewGuid();
      var gameBoardData = new GameBoard(BoardSizes.Standard);
      gameBoardData.PlaceStartingInfrastructure(playerId, FirstPlayerSettlementLocation, FirstPlayerRoadEndLocation);
      gameBoardData.PlaceStartingInfrastructure(playerId, SecondPlayerSettlementLocation, SecondPlayerRoadEndLocation);

      // Act
      Action action = () => { gameBoardData.PlaceSettlement(playerId, newSettlementLocation); };

      // Assert
      action.ShouldThrow<GameBoard.PlacementException>().Message.ShouldBe("Cannot place settlement because location is too close to exising settlement.");
    }

    [Test]
    public void PlaceSettlement_DontPlaceOnRoad_ThrowsMeaningfulException()
    {
      // Arrange
      var playerId = Guid.NewGuid();
      var gameBoardData = new GameBoard(BoardSizes.Standard);
      gameBoardData.PlaceStartingInfrastructure(playerId, FirstPlayerSettlementLocation, FirstPlayerRoadEndLocation);
      gameBoardData.PlaceStartingInfrastructure(playerId, SecondPlayerSettlementLocation, SecondPlayerRoadEndLocation);

      // Act
      Action action = () => { gameBoardData.PlaceSettlement(playerId, 10); };

      // Assert
      action.ShouldThrow<GameBoard.PlacementException>().Message.ShouldBe("Cannot place settlement because location is not on a road.");
    }
  }
}
