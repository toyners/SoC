
namespace Jabberwocky.SoC.Library.UnitTests.GameBoard_Tests
{
  using System;
  using GameBoards;
  using NUnit.Framework;
  using Shouldly;

  [TestFixture]
  [Category("All")]
  [Category("GameBoard")]
  [Category("GameBoard.PlaceStartingInfrastructure")]
  public class GameBoard_PlaceStartingInfrastructure_UnitTests : GameBoardTestBase
  {
    [Test]
    public void PlaceStartingInfrastructure_TryPlacingOnSettledLocation_ThrowsMeaningfulException()
    {
      // Arrange
      var gameBoardData = new GameBoard(BoardSizes.Standard);
      var firstPlayerId = Guid.NewGuid();
      var secondPlayerId = Guid.NewGuid();
      gameBoardData.PlaceStartingInfrastructure(firstPlayerId, 1, 2);

      // Act
      Action action = () => { gameBoardData.PlaceStartingInfrastructure(secondPlayerId, 1, 0); };

      // Assert
      action.ShouldThrow<GameBoard.PlacementException>().Message.ShouldBe("Cannot place settlement because location is already settled.");
    }

    [Test]
    public void PlaceStartingInfrastructure_TryPlacingOnInvalidLocation_ThrowsMeaningfulException()
    {
      // Arrange
      var playerId = Guid.NewGuid();
      var gameBoardData = new GameBoard(BoardSizes.Standard);

      // Act
      Action action = () => { gameBoardData.PlaceStartingInfrastructure(playerId, 100, 101); };

      // Assert
      action.ShouldThrow<GameBoard.PlacementException>().Message.ShouldBe("Cannot place settlement because location is not on board.");
    }

    [Test]
    public void PlaceStartingInfrastructure_PlayerAlreadyPlacedAllStartingInfrastructure_ThrowsMeaningfulException()
    {
      var playerId = Guid.NewGuid();
      var gameBoardData = new GameBoard(BoardSizes.Standard);
      gameBoardData.PlaceStartingInfrastructure(playerId, FirstPlayerSettlementLocation, FirstPlayerRoadEndLocation);
      gameBoardData.PlaceStartingInfrastructure(playerId, SecondPlayerSettlementLocation, SecondPlayerRoadEndLocation);

      Action action = () => { gameBoardData.PlaceStartingInfrastructure(playerId, 0u, 1u); };

      action.ShouldThrow<GameBoard.PlacementException>().Message.ShouldBe("Cannot place starting infrastructure more than once per player.");
    }

    [Test]
    [TestCase(20u, 31u, 19u, 18u)]
    [TestCase(20u, 19u, 21u, 22u)]
    [TestCase(20u, 19u, 31u, 30u)]
    public void PlaceStartingInfrastructure_TryPlacingNextToSettledLocation_ThrowsMeaningFulException(UInt32 firstSettlementLocation, UInt32 firstEndRoadLocation, UInt32 secondSettlementLocation, UInt32 secondEndRoadLocation)
    {
      var firstPlayerId = Guid.NewGuid();
      var secondPlayerId = Guid.NewGuid();
      var gameBoardData = new GameBoard(BoardSizes.Standard);
      gameBoardData.PlaceStartingInfrastructure(firstPlayerId, firstSettlementLocation, firstEndRoadLocation);

      // Act
      Action action = () => { gameBoardData.PlaceStartingInfrastructure(secondPlayerId, secondSettlementLocation, secondEndRoadLocation); };

      // Assert
      action.ShouldThrow<GameBoard.PlacementException>().Message.ShouldBe("Cannot place settlement because location is too close to exising settlement.");
    }

    [Test]
    public void PlaceStartingInfrastructure_NoDirectConnectionBetweenSettlementAndRoadEnd_ThrowsMeaningfulException()
    {
      // Arrange
      var playerId = Guid.NewGuid();
      var gameBoardData = new GameBoard(BoardSizes.Standard);

      // Act
      Action action = () => { gameBoardData.PlaceStartingInfrastructure(playerId, 20u, 22u); };

      // Assert
      action.ShouldThrow<GameBoard.PlacementException>().Message.ShouldBe("Cannot place road because no direct connection between start location and end location.");
    }

    [Test]
    [TestCase(53u, 54u, "Cannot place road because board location is not valid.")] // Hanging over the edge 
    [TestCase(54u, 53u, "Cannot place settlement because location is not on board.")] // Hanging over the edge
    [TestCase(100u, 101u, "Cannot place settlement because location is not on board.")]
    public void PlaceStartingInfrastructure_InfrastructureOffBoard_ThrowsMeaningfulException(UInt32 settlementLocation, UInt32 roadEndLocation, String expectedMessage)
    {
      // Arrange
      var gameBoardData = new GameBoard(BoardSizes.Standard);

      // Act
      Action action = () => { gameBoardData.PlaceStartingInfrastructure(Guid.NewGuid(), settlementLocation, roadEndLocation); };

      // Assert
      action.ShouldThrow<GameBoard.PlacementException>().Message.ShouldBe(expectedMessage);
    }

    [Test]
    public void PlaceStartingInfrastructure_RoadFailsVerification_SettlementNotPlaced()
    {
      var playerId = Guid.NewGuid();
      var location = 20u;
      var gameBoardData = new GameBoard(BoardSizes.Standard);

      // Road end not valid so will not be placed.
      try
      {
        gameBoardData.PlaceStartingInfrastructure(playerId, location, 22);
      }
      catch (GameBoard.PlacementException pe)
      {
        // ignore it
      }

      // Check placing the settlement in the same location with a correct road end - will pass since nothing is there.
      var results = gameBoardData.CanPlaceStartingInfrastructure(playerId, location, 21);

      results.Status.ShouldBe(GameBoard.VerificationStatus.Valid);
    }

    [Test]
    public void PlaceStartingInfrastructure_SettlementAndRoadAreValid_NoMeaningfulExceptionThrown()
    {
      var playerId = Guid.NewGuid();
      var gameBoardData = new GameBoard(BoardSizes.Standard);

      // Act
      Action action = () =>
      {
        gameBoardData.PlaceStartingInfrastructure(playerId, FirstPlayerSettlementLocation, FirstPlayerRoadEndLocation);
        gameBoardData.PlaceStartingInfrastructure(playerId, SecondPlayerSettlementLocation, SecondPlayerRoadEndLocation);
      };

      // Assert
      action.ShouldNotThrow();
      gameBoardData.CanPlaceSettlement(playerId, FirstPlayerSettlementLocation).Status.ShouldBe(GameBoard.VerificationStatus.LocationIsOccupied);
      gameBoardData.CanPlaceRoadSegment(playerId, FirstPlayerSettlementLocation, FirstPlayerRoadEndLocation).ShouldBe(PlacementStatusCodes.RoadIsOccupied);
    }
  }
}
