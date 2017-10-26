
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
    [Category("All")]
    [Category("GameBoardData")]
    [Category("CanPlaceRoad")]
    public void CanPlaceRoad_ConnectedToRoad_ReturnsValid()
    {
      var gameBoardData = new GameBoardData(BoardSizes.Standard);
      var playerId = Guid.NewGuid();
      gameBoardData.PlaceStartingInfrastructure(playerId, 0, 8);
      var result = gameBoardData.CanPlaceRoad(playerId, 9, 8);
      result.Status.ShouldBe(GameBoardData.VerificationStatus.Valid);
    }

    [Test]
    [Category("All")]
    [Category("GameBoardData")]
    [Category("CanPlaceRoad")]
    public void CanPlaceRoad_EmptyBoard_ReturnsStartingInfrastructureNotPresent()
    {
      // Arrange
      var gameBoardData = new GameBoardData(BoardSizes.Standard);

      // Act
      var result = gameBoardData.CanPlaceRoad(Guid.NewGuid(), 0, 1);

      // Assert
      result.Status.ShouldBe(GameBoardData.VerificationStatus.StartingInfrastructureNotPresent);
    }

    [Test]
    [Category("All")]
    [Category("GameBoardData")]
    [Category("PlaceRoad")]
    public void PlaceRoad_EmptyBoard_ThrowsMeaningfulException()
    {
      // Arrange
      var gameBoardData = new GameBoardData(BoardSizes.Standard);

      // Act
      Action action = () => { gameBoardData.PlaceRoad(Guid.NewGuid(), 0, 1); };

      // Assert
      action.ShouldThrow<GameBoardData.PlacementException>().Message.ShouldBe("Cannot place settlement before placing infrastructure using PlaceInfrastructure method.");
    }

    [Test]
    [Category("All")]
    [Category("GameBoardData")]
    [Category("CanPlaceRoad")]
    [TestCase(11u, 21u)]
    [TestCase(21u, 11u)]
    public void CanPlaceRoad_JoiningToOtherRoads_ReturnsValid(UInt32 roadStartLocation, UInt32 roadEndLocation)
    {
      // Arrange
      var gameBoardData = new GameBoardData(BoardSizes.Standard);
      var playerId = Guid.NewGuid();
      gameBoardData.PlaceStartingInfrastructure(playerId, 12, 11);

      // Act
      var result = gameBoardData.CanPlaceRoad(playerId, roadStartLocation, roadEndLocation);

      // Assert
      result.Status.ShouldBe(GameBoardData.VerificationStatus.Valid);
    }

    [Test]
    [Category("GameBoardData")]
    [Category("CanPlaceRoad")]
    [TestCase(53u, 54u)] // Hanging over the edge 
    [TestCase(54u, 53u)] // Hanging over the edge
    [TestCase(100u, 101u)]
    public void CanPlaceRoad_OffBoard_ReturnsRoadIsInvalid(UInt32 start, UInt32 end)
    {
      // Arrange
      var playerId = Guid.NewGuid();
      var gameBoardData = new GameBoardData(BoardSizes.Standard);
      gameBoardData.PlaceStartingInfrastructure(playerId, 0, 1);

      // Act
      var result = gameBoardData.CanPlaceRoad(playerId, start, end);

      // Assert
      result.Status.ShouldBe(GameBoardData.VerificationStatus.RoadIsOffBoard);
    }

    [Test]
    [Category("GameBoardData")]
    [Category("PlaceRoad")]
    [TestCase(53u, 54u)] // Hanging over the edge 
    [TestCase(54u, 53u)] // Hanging over the edge
    [TestCase(100u, 101u)]
    public void PlaceRoad_OffBoard_ThrowsMeaningfulException(UInt32 start, UInt32 end)
    {
      // Arrange
      var playerId = Guid.NewGuid();
      var gameBoardData = new GameBoardData(BoardSizes.Standard);
      gameBoardData.PlaceStartingInfrastructure(playerId, 0, 1);

      // Act
      Action action = () => { gameBoardData.PlaceRoad(playerId, start, end); };

      // Assert
      action.ShouldThrow<GameBoardData.PlacementException>().Message.ShouldBe("Cannot place road because board location is not valid (" + start + ", " + end + ").");
    }

    [Test]
    [Category("All")]
    [Category("GameBoardData")]
    [Category("CanPlaceRoad")]
    [TestCase(43u, 53u)]
    [TestCase(53u, 43u)]
    public void CanPlaceRoad_NoDirectConnection_ReturnsRoadIsInvalid(UInt32 start, UInt32 end)
    {
      // Arrange
      var playerId = Guid.NewGuid();
      var gameBoardData = new GameBoardData(BoardSizes.Standard);
      gameBoardData.PlaceStartingInfrastructure(playerId, 0, 1);

      // Act
      var result = gameBoardData.CanPlaceRoad(playerId, start, end);

      // Assert
      result.Status.ShouldBe(GameBoardData.VerificationStatus.NoDirectConnection);
    }

    [Test]
    [Category("All")]
    [Category("GameBoardData")]
    [Category("PlaceRoad")]
    [TestCase(43u, 53u)]
    [TestCase(53u, 43u)]
    public void PlaceRoad_NoDirectConnection_ThrowsMeaningfulException(UInt32 start, UInt32 end)
    {
      // Arrange
      var playerId = Guid.NewGuid();
      var gameBoardData = new GameBoardData(BoardSizes.Standard);
      gameBoardData.PlaceStartingInfrastructure(playerId, 0, 1);

      // Act
      Action action = () => { gameBoardData.CanPlaceRoad(playerId, start, end); };

      // Assert
      action.ShouldThrow<GameBoardData.PlacementException>().Message.ShouldBe("Cannot place road because no direct connection between locations (" + start + ", " + end + ").");
    }

    [Test]
    [Category("All")]
    [Category("GameBoardData")]
    [Category("CanPlaceRoad")]
    public void CanPlaceRoad_RoadAlreadyBuilt_ReturnsRoadIsOccupied()
    {
      var gameBoardData = new GameBoardData(BoardSizes.Standard);
      var playerId = Guid.NewGuid();
      gameBoardData.PlaceStartingInfrastructure(playerId, 0, 1);

      var result = gameBoardData.CanPlaceRoad(playerId, 0, 1);
      result.Status.ShouldBe(GameBoardData.VerificationStatus.RoadIsOccupied);
    }

    [Test]
    [Category("All")]
    [Category("GameBoardData")]
    [Category("PlaceRoad")]
    public void PlaceRoad_RoadAlreadyBuilt_ThrowsMeaningfulException()
    {
      // Arrange
      var gameBoardData = new GameBoardData(BoardSizes.Standard);
      var playerId = Guid.NewGuid();
      gameBoardData.PlaceStartingInfrastructure(playerId, 0, 1);

      // Act
      Action action = () => { gameBoardData.CanPlaceRoad(playerId, 0, 1); };

      // Assert
      action.ShouldThrow<GameBoardData.PlacementException>().Message.ShouldBe("Cannot place road because road already exists (0, 1).");
    }

    [Test]
    [Category("All")]
    [Category("GameBoardData")]
    [Category("CanPlaceRoad")]
    [TestCase(2u, 3u)]
    [TestCase(8u, 9u)]
    public void CanPlaceRoad_RoadNotConnectedToExistingInfrastructure_ReturnsNotConnected(UInt32 roadStartLocation, UInt32 roadEndLocation)
    {
      // Arrange
      var playerId = Guid.NewGuid();
      var gameBoardData = new GameBoardData(BoardSizes.Standard);
      gameBoardData.PlaceStartingInfrastructure(playerId, 0, 1);

      // Act
      var result = gameBoardData.CanPlaceRoad(playerId, roadStartLocation, roadEndLocation);

      // Assert
      result.Status.ShouldBe(GameBoardData.VerificationStatus.NotConnectedToExisting);
    }

    [Test]
    [Category("All")]
    [Category("GameBoardData")]
    [Category("PlaceRoad")]
    [TestCase(2u, 3u)]
    [TestCase(8u, 9u)]
    public void PlaceRoad_RoadNotConnectedToExistingInfrastructure_ThrowsMeaningfulException(UInt32 roadStartLocation, UInt32 roadEndLocation)
    {
      // Arrange
      var playerId = Guid.NewGuid();
      var gameBoardData = new GameBoardData(BoardSizes.Standard);
      gameBoardData.PlaceStartingInfrastructure(playerId, 0, 1);

      // Act
      Action action = () => { gameBoardData.CanPlaceRoad(playerId, roadStartLocation, roadEndLocation); };

      // Assert
      action.ShouldThrow<GameBoardData.PlacementException>().Message.ShouldBe("Cannot place road because it is not connected to an existing road segment.");
    }

    [Test]
    [Category("All")]
    [Category("GameBoardData")]
    [Category("CanPlaceSettlement")]
    public void CanPlaceSettlement_EmptyBoard_ReturnsStartingInfrastructureNotPresent()
    {
      // Arrange
      var playerId = Guid.NewGuid();
      var gameBoardData = new GameBoardData(BoardSizes.Standard);

      // Act
      var result = gameBoardData.CanPlaceSettlement(playerId, 0);

      // Assert
      result.Status.ShouldBe(GameBoardData.VerificationStatus.StartingInfrastructureNotPresent);
      result.LocationIndex.ShouldBe(0u);
      result.PlayerId.ShouldBe(Guid.Empty);
    }

    [Test]
    [Category("All")]
    [Category("GameBoardData")]
    [Category("CanPlaceSettlement")]
    public void CanPlaceSettlement_TryPlacingOnSettledLocation_ReturnsLocationIsOccupiedStatus()
    {
      // Arrange
      var playerId = Guid.NewGuid();
      var gameBoardData = new GameBoardData(BoardSizes.Standard);
      gameBoardData.PlaceStartingInfrastructure(playerId, 1, 2);
      
      // Act
      var result = gameBoardData.CanPlaceSettlement(playerId, 1);

      // Assert
      result.Status.ShouldBe(GameBoardData.VerificationStatus.LocationIsOccupied);
      result.LocationIndex.ShouldBe(1u);
      result.PlayerId.ShouldBe(playerId);
    }

    [Test]
    [Category("All")]
    [Category("GameBoardData")]
    [Category("CanPlaceSettlement")]
    public void CanPlaceSettlement_TryPlacingOnInvalidLocation_ReturnsLocationIsInvalidStatus()
    {
      // Arrange
      var playerId = Guid.NewGuid();
      var gameBoardData = new GameBoardData(BoardSizes.Standard);
      gameBoardData.PlaceStartingInfrastructure(playerId, 0, 1);

      // Act
      var result = gameBoardData.CanPlaceSettlement(playerId, 100);

      // Assert
      result.Status.ShouldBe(GameBoardData.VerificationStatus.LocationIsInvalid);
      result.LocationIndex.ShouldBe(0u);
      result.PlayerId.ShouldBe(Guid.Empty);
    }

    [Test]
    [Category("All")]
    [Category("GameBoardData")]
    [Category("CanPlaceSettlement")]
    [TestCase(19u)]
    [TestCase(21u)]
    [TestCase(31u)]
    public void CanPlaceSettlement_TryPlacingNextToSettledLocation_ReturnsTooCloseToSettlement(UInt32 newSettlementLocation)
    {
      // Arrange
      var playerId = Guid.NewGuid();
      var location = 20u;
      var gameBoardData = new GameBoardData(BoardSizes.Standard);
      gameBoardData.PlaceStartingInfrastructure(playerId, 20u, 21u);

      // Act
      var result = gameBoardData.CanPlaceSettlement(playerId, newSettlementLocation);

      // Assert
      result.Status.ShouldBe(GameBoardData.VerificationStatus.TooCloseToSettlement);
      result.LocationIndex.ShouldBe(location);
      result.PlayerId.ShouldBe(playerId);
    }

    [Test]
    [Category("All")]
    [Category("GameBoardData")]
    [Category("CanPlaceSettlement")]
    public void CanPlaceSettlement_PlaceOnRoad_ReturnsValid()
    {
      // Arrange
      var playerId = Guid.NewGuid();
      var gameBoardData = new GameBoardData(BoardSizes.Standard);
      gameBoardData.PlaceStartingInfrastructure(playerId, 20, 21);
      gameBoardData.PlaceRoad(playerId, 21, 22);

      // Act
      var result = gameBoardData.CanPlaceSettlement(playerId, 22);

      // Assert
      result.Status.ShouldBe(GameBoardData.VerificationStatus.Valid);
    }

    [Test]
    [Category("All")]
    [Category("GameBoardData")]
    [Category("CanPlaceSettlement")]
    public void CanPlaceSettlement_DontPlaceOnRoad_ReturnsNotConnectedToExisting()
    {
      // Arrange
      var playerId = Guid.NewGuid();
      var gameBoardData = new GameBoardData(BoardSizes.Standard);
      gameBoardData.PlaceStartingInfrastructure(playerId, 20, 21);

      // Act
      var result = gameBoardData.CanPlaceSettlement(playerId, 22);

      // Assert
      result.Status.ShouldBe(GameBoardData.VerificationStatus.NotConnectedToExisting);
    }

    [Test]
    [Category("All")]
    [Category("GameBoardData")]
    [Category("CanPlaceStartingInfrastructure")]
    public void CanPlaceStartingInfrastructure_EmptyBoard_ReturnsValid()
    {
      // Arrange
      var playerId = Guid.NewGuid();
      var locationIndex = 20u;
      var roadEndIndex = 21u;
      var gameBoardData = new GameBoardData(BoardSizes.Standard);

      // Act
      var result = gameBoardData.CanPlaceStartingInfrastructure(playerId, locationIndex, roadEndIndex);

      // Assert
      result.Status.ShouldBe(GameBoardData.VerificationStatus.Valid);
      result.LocationIndex.ShouldBe(0u);
      result.PlayerId.ShouldBe(Guid.Empty);
    }

    [Test]
    [Category("All")]
    [Category("GameBoardData")]
    [Category("CanPlaceStartingInfrastructure")]
    public void CanPlaceStartingInfrastructure_TryPlacingOnSettledLocation_ReturnsLocationIsOccupiedStatus()
    {
      var gameBoardData = new GameBoardData(BoardSizes.Standard);
      var firstPlayerId = Guid.NewGuid();
      var secondPlayerId = Guid.NewGuid();
      gameBoardData.PlaceStartingInfrastructure(firstPlayerId, 1, 2);

      var result = gameBoardData.CanPlaceStartingInfrastructure(secondPlayerId, 1, 0);
      result.Status.ShouldBe(GameBoardData.VerificationStatus.LocationIsOccupied);
      result.LocationIndex.ShouldBe(1u);
      result.PlayerId.ShouldBe(firstPlayerId);
    }

    [Test]
    [Category("All")]
    [Category("GameBoardData")]
    [Category("CanPlaceStartingInfrastructure")]
    public void CanPlaceStartingInfrastructure_TryPlacingOnInvalidLocation_ReturnsLocationIsInvalidStatus()
    {
      // Arrange
      var playerId = Guid.NewGuid();
      var gameBoardData = new GameBoardData(BoardSizes.Standard);

      // Act
      var result = gameBoardData.CanPlaceStartingInfrastructure(playerId, 100, 101);

      // Assert
      result.Status.ShouldBe(GameBoardData.VerificationStatus.LocationIsInvalid);
      result.LocationIndex.ShouldBe(0u);
      result.PlayerId.ShouldBe(Guid.Empty);
    }

    [Test]
    [Category("All")]
    [Category("GameBoardData")]
    [Category("CanPlaceStartingInfrastructure")]
    public void CanPlaceStartingInfrastructure_PlayerAlreadyPlacedStartingInfrastructure_ReturnsStartingInfrastructureAlreadyPresent()
    {
      var playerId = Guid.NewGuid();
      var locationOneIndex = 20u;
      var roadOneEndIndex = 21u;

      var gameBoardData = new GameBoardData(BoardSizes.Standard);
      gameBoardData.PlaceStartingInfrastructure(playerId, locationOneIndex, roadOneEndIndex);

      var locationTwoIndex = 0u;
      var roadTwoEndIndex = 1u;
      var result = gameBoardData.CanPlaceStartingInfrastructure(playerId, locationTwoIndex, roadTwoEndIndex);

      result.Status.ShouldBe(GameBoardData.VerificationStatus.StartingInfrastructureAlreadyPresent);
      result.LocationIndex.ShouldBe(0u);
      result.PlayerId.ShouldBe(Guid.Empty);
    }

    [Test]
    [Category("All")]
    [Category("GameBoardData")]
    [Category("CanPlaceStartingInfrastructure")]
    [TestCase(20u, 31u, 19u, 18u)]
    [TestCase(20u, 19u, 21u, 22u)]
    [TestCase(20u, 19u, 31u, 30u)]
    public void CanPlaceStartingInfrastructure_TryPlacingNextToSettledLocation_ReturnsCorrectVerificationResult(UInt32 firstSettlementLocation, UInt32 firstEndRoadLocation, UInt32 secondSettlementLocation, UInt32 secondEndRoadLocation)
    {
      var firstPlayerId = Guid.NewGuid();
      var secondPlayerId = Guid.NewGuid();
      var gameBoardData = new GameBoardData(BoardSizes.Standard);
      gameBoardData.PlaceStartingInfrastructure(firstPlayerId, firstSettlementLocation, firstEndRoadLocation);

      // Act
      var result = gameBoardData.CanPlaceStartingInfrastructure(secondPlayerId, secondSettlementLocation, secondEndRoadLocation);

      // Assert
      result.Status.ShouldBe(GameBoardData.VerificationStatus.TooCloseToSettlement);
      result.LocationIndex.ShouldBe(firstSettlementLocation);
      result.PlayerId.ShouldBe(firstPlayerId);
    }

    [Test]
    [Category("All")]
    [Category("GameBoardData")]
    [Category("CanPlaceStartingInfrastructure")]
    public void CanPlaceStartingInfrastructure_NoDirectConnectionBetweenSettlementAndRoadEnd_ReturnsNoDirectConnection()
    {
      // Arrange
      var playerId = Guid.NewGuid();
      var gameBoardData = new GameBoardData(BoardSizes.Standard);

      // Act
      var result = gameBoardData.CanPlaceStartingInfrastructure(playerId, 20u, 22u);

      // Assert
      result.Status.ShouldBe(GameBoardData.VerificationStatus.NoDirectConnection);
      result.LocationIndex.ShouldBe(0u);
      result.PlayerId.ShouldBe(Guid.Empty);
    }

    [Test]
    [Category("All")]
    [Category("GameBoardData")]
    [Category("CanPlaceStartingInfrastructure")]
    [TestCase(53u, 54u)] // Hanging over the edge 
    [TestCase(54u, 53u)] // Hanging over the edge
    [TestCase(100u, 101u)]
    public void CanPlaceStartingInfrastructure_RoadOffBoard_ReturnsRoadIsOffBoard(UInt32 settlementLocation, UInt32 roadEndLocation)
    {
      // Arrange
      var gameBoardData = new GameBoardData(BoardSizes.Standard);

      // Act
      var result = gameBoardData.CanPlaceStartingInfrastructure(Guid.NewGuid(), settlementLocation, roadEndLocation);

      // Assert
      result.Status.ShouldBe(GameBoardData.VerificationStatus.RoadIsOffBoard);
      result.LocationIndex.ShouldBe(0u);
      result.PlayerId.ShouldBe(Guid.Empty);
    }

    [Test]
    [Category("All")]
    [Category("GameBoardData")]
    [Category("PlaceSettlement")]
    public void PlaceSettlement_EmptyBoard_ThrowsMeaningfulException()
    {
      // Arrange
      var gameBoardData = new GameBoardData(BoardSizes.Standard);

      // Act
      Action action = () => { gameBoardData.PlaceSettlement(Guid.NewGuid(), 20u); };

      // Assert
      action.ShouldThrow<GameBoardData.PlacementException>().Message.ShouldBe("Cannot place settlement before placing infrastructure using PlaceInfrastructure method.");
    }

    [Test]
    [Category("All")]
    [Category("GameBoardData")]
    [Category("PlaceSettlement")]
    public void PlaceSettlement_TryPlacingOnSettledLocation_ThrowsMeaningfulException()
    {
      // Arrange
      var playerId = Guid.NewGuid();
      var gameBoardData = new GameBoardData(BoardSizes.Standard);
      gameBoardData.PlaceStartingInfrastructure(playerId, 1, 2);

      // Act
      Action action = () => { gameBoardData.PlaceSettlement(playerId, 1); };

      // Assert
      action.ShouldThrow<GameBoardData.PlacementException>().Message.ShouldBe("Cannot place settlement because location is already settled.");
    }

    [Test]
    [Category("All")]
    [Category("GameBoardData")]
    [Category("PlaceSettlement")]
    public void PlaceSettlement_TryPlacingOnInvalidLocation_ThrowsMeaningfulException()
    {
      // Arrange
      var playerId = Guid.NewGuid();
      var gameBoardData = new GameBoardData(BoardSizes.Standard);
      gameBoardData.PlaceStartingInfrastructure(playerId, 0, 1);

      // Act
      Action action = () => { gameBoardData.CanPlaceSettlement(playerId, 100); };

      // Assert
      action.ShouldThrow<GameBoardData.PlacementException>().Message.ShouldBe("Cannot place settlement because location is not on board.");
    }

    [Test]
    [Category("All")]
    [Category("GameBoardData")]
    [Category("PlaceSettlement")]
    [TestCase(19u)]
    [TestCase(21u)]
    [TestCase(31u)]
    public void PlaceSettlement_TryPlacingNextToSettledLocation_ThrowsMeaningfulException(UInt32 newSettlementLocation)
    {
      // Arrange
      var playerId = Guid.NewGuid();
      var location = 20u;
      var gameBoardData = new GameBoardData(BoardSizes.Standard);
      gameBoardData.PlaceStartingInfrastructure(playerId, location, 21u);

      // Act
      Action action = () => { gameBoardData.PlaceSettlement(playerId, newSettlementLocation); };

      // Assert
      action.ShouldThrow<GameBoardData.PlacementException>().Message.ShouldBe("Cannot place settlement because location (" + newSettlementLocation + ") is too close to exising settlement at location (" + location + ").");
    }

    [Test]
    [Category("All")]
    [Category("GameBoardData")]
    [Category("PlaceSettlement")]
    public void PlaceSettlement_DontPlaceOnRoad_ThrowsMeaningfulException()
    {
      // Arrange
      var playerId = Guid.NewGuid();
      var gameBoardData = new GameBoardData(BoardSizes.Standard);
      gameBoardData.PlaceStartingInfrastructure(playerId, 20, 21);

      // Act
      Action action = () => { gameBoardData.CanPlaceSettlement(playerId, 22); };

      // Assert
      action.ShouldThrow<GameBoardData.PlacementException>().Message.ShouldBe("Cannot place settlement because location is not on a road.");
    }

    [Test]
    [Category("All")]
    [Category("GameBoardData")]
    [Category("PlaceStartingInfrastructure")]
    public void PlaceStartingInfrastructure_TryPlacingOnSettledLocation_ThrowsMeaningfulException()
    {
      // Arrange
      var gameBoardData = new GameBoardData(BoardSizes.Standard);
      var firstPlayerId = Guid.NewGuid();
      var secondPlayerId = Guid.NewGuid();
      gameBoardData.PlaceStartingInfrastructure(firstPlayerId, 1, 2);

      // Act
      Action action = () => { gameBoardData.PlaceStartingInfrastructure(secondPlayerId, 1, 0); };

      // Assert
      action.ShouldThrow<GameBoardData.PlacementException>().Message.ShouldBe("Cannot place settlement because location is already settled.");
    }

    [Test]
    [Category("All")]
    [Category("GameBoardData")]
    [Category("PlaceStartingInfrastructure")]
    public void PlaceStartingInfrastructure_TryPlacingOnInvalidLocation_ThrowsMeaningfulException()
    {
      // Arrange
      var playerId = Guid.NewGuid();
      var gameBoardData = new GameBoardData(BoardSizes.Standard);

      // Act
      Action action = () => { gameBoardData.CanPlaceStartingInfrastructure(playerId, 100, 101); };

      // Assert
      action.ShouldThrow<GameBoardData.PlacementException>().Message.ShouldBe("Cannot place settlement because location is not on board.");
    }

    [Test]
    [Category("All")]
    [Category("GameBoardData")]
    [Category("PlaceStartingInfrastructure")]
    public void PlaceStartingInfrastructure_PlayerAlreadyPlacedStartingInfrastructure_ThrowsMeaningfulException()
    {
      var playerId = Guid.NewGuid();
      var gameBoardData = new GameBoardData(BoardSizes.Standard);
      gameBoardData.PlaceStartingInfrastructure(playerId, 20u, 21u);

      Action action = () => { gameBoardData.PlaceStartingInfrastructure(playerId, 10u, 11u); };

      action.ShouldThrow<GameBoardData.PlacementException>().Message.ShouldBe("Cannot place starting infrastructure more than once per player.");
    }

    [Test]
    [Category("All")]
    [Category("GameBoardData")]
    [Category("PlaceStartingInfrastructure")]
    [TestCase(20u, 31u, 19u, 18u)]
    [TestCase(20u, 19u, 21u, 22u)]
    [TestCase(20u, 19u, 31u, 30u)]
    public void PlaceStartingInfrastructure_TryPlacingNextToSettledLocation_ThrowsMeaningFulException(UInt32 firstSettlementLocation, UInt32 firstEndRoadLocation, UInt32 secondSettlementLocation, UInt32 secondEndRoadLocation)
    {
      var firstPlayerId = Guid.NewGuid();
      var secondPlayerId = Guid.NewGuid();
      var gameBoardData = new GameBoardData(BoardSizes.Standard);
      gameBoardData.PlaceStartingInfrastructure(firstPlayerId, firstSettlementLocation, firstEndRoadLocation);

      // Act
      Action action = () => { gameBoardData.CanPlaceStartingInfrastructure(secondPlayerId, secondSettlementLocation, secondEndRoadLocation); };

      // Assert
      action.ShouldThrow<GameBoardData.PlacementException>().Message.ShouldBe("Cannot place settlement because location (" + secondSettlementLocation + ") is too close to exising settlement at location (" + firstSettlementLocation + ").");
    }

    [Test]
    [Category("All")]
    [Category("GameBoardData")]
    [Category("PlaceStartingInfrastructure")]
    public void PlaceStartingInfrastructure_NoDirectConnectionBetweenSettlementAndRoadEnd_ThrowsMeaningfulException()
    {
      // Arrange
      var playerId = Guid.NewGuid();
      var gameBoardData = new GameBoardData(BoardSizes.Standard);

      // Act
      Action action = () => { gameBoardData.CanPlaceStartingInfrastructure(playerId, 20u, 22u); };

      // Assert
      action.ShouldThrow<GameBoardData.PlacementException>().Message.ShouldBe("Cannot place road because no direct connection between locations (20, 22).");
    }

    [Test]
    [Category("All")]
    [Category("GameBoardData")]
    [Category("PlaceStartingInfrastructure")]
    [TestCase(53u, 54u)] // Hanging over the edge 
    [TestCase(54u, 53u)] // Hanging over the edge
    [TestCase(100u, 101u)]
    public void PlaceStartingInfrastructure_RoadOffBoard_ThrowsMeaningfulException(UInt32 settlementLocation, UInt32 roadEndLocation)
    {
      // Arrange
      var gameBoardData = new GameBoardData(BoardSizes.Standard);

      // Act
      Action action = () => { gameBoardData.CanPlaceStartingInfrastructure(Guid.NewGuid(), settlementLocation, roadEndLocation); };

      // Assert
      action.ShouldThrow<GameBoardData.PlacementException>().Message.ShouldBe("Cannot place road because board location is not valid (" + settlementLocation + ", " + roadEndLocation + ").");
    }

    [Test]
    [Category("All")]
    [Category("GameBoardData")]
    [Category("PlaceStartingInfrastructure")]
    public void PlaceStartingInfrastructure_RoadFailsVerification_SettlementNotPlaced()
    {
      var playerId = Guid.NewGuid();
      var location = 20u;
      var gameBoardData = new GameBoardData(BoardSizes.Standard);

      // Road end not valid so will not be placed.
      try
      {
        gameBoardData.PlaceStartingInfrastructure(playerId, location, 22);
      }
      catch (GameBoardData.PlacementException pe)
      {
        // ignore it
      }

      // Check placing the settlement in the same location - will pass since nothing is there.
      var results = gameBoardData.CanPlaceSettlement(playerId, location);

      results.Status.ShouldBe(GameBoardData.VerificationStatus.Valid);
    }

    [Test]
    [Category("GameBoardData")]
    public void GetPathBetweenLocations_StartAndEndAreSame_ReturnsNull()
    {
      var gameBoardData = new GameBoardData(BoardSizes.Standard);
      var result = gameBoardData.GetPathBetweenLocations(0, 0);
      result.ShouldBeNull();
    }

    [Test]
    [Category("GameBoardData")]
    [TestCase(1u, 0u)]
    [TestCase(8u, 48u)]
    public void GetPathBetweenLocations_StartAndEndAreNeighbours_ReturnsOneStep(UInt32 endPoint, UInt32 stepIndex)
    {
      var gameBoardData = new GameBoardData(BoardSizes.Standard);
      var result = gameBoardData.GetPathBetweenLocations(0, endPoint);
      result.ShouldBe(new List<UInt32> { endPoint });
    }

    [Test]
    [Category("GameBoardData")]
    public void GetPathBetweenLocations_StartAndEndAreNeighbours()
    {
      var gameBoardData = new GameBoardData(BoardSizes.Standard);
      var result = gameBoardData.GetPathBetweenLocations(0, 10);
      result.ShouldBe(new List<UInt32> { 10, 2, 1 });
    }

    [Test]
    [Category("GameBoardData")]
    public void GetSettlementsForPlayers_EmptyBoard_ReturnsNull()
    {
      var gameBoardData = new GameBoardData(BoardSizes.Standard);
      var settlements = gameBoardData.GetSettlementsForPlayer(Guid.NewGuid());
      settlements.ShouldBeNull();
    }

    [Test]
    [Category("GameBoardData")]
    public void GetSettlementsForPlayers_PlayerHasNoSettlementsOnBoard_ReturnsNull()
    {
      var gameBoardData = new GameBoardData(BoardSizes.Standard);
      gameBoardData.PlaceSettlement(Guid.NewGuid(), 0);
      var settlements = gameBoardData.GetSettlementsForPlayer(Guid.NewGuid());
      settlements.ShouldBeNull();
    }

    [Test]
    [Category("GameBoardData")]
    public void PlaceStartingInfrastructure_SettlementAndRoadAreValid_NoMeaningfulExceptionThrown()
    {
      var playerId = Guid.NewGuid();
      var settlementIndex = 20u;
      var roadEndIndex = 21u;
      var gameBoardData = new GameBoardData(BoardSizes.Standard);

      // Act
      Action action = () => { gameBoardData.PlaceStartingInfrastructure(playerId, settlementIndex, roadEndIndex); };

      // Assert
      action.ShouldNotThrow();
      gameBoardData.CanPlaceSettlement(playerId, settlementIndex).Status.ShouldBe(GameBoardData.VerificationStatus.LocationIsOccupied);
      gameBoardData.CanPlaceRoad(playerId, settlementIndex, roadEndIndex).Status.ShouldBe(GameBoardData.VerificationStatus.RoadIsOccupied);
    }

    [Test]
    [Category("GameBoardData")]
    [TestCase(12u, 1, 0, 0, 1, 1)]
    [TestCase(45u, 0, 1, 0, 1, 0)]
    [TestCase(53u, 0, 1, 0, 0, 0)]
    [TestCase(20u, 0, 1, 1, 1, 0)]
    public void GetResourcesForLocation_StandardBoard_ReturnsExpectedResources(UInt32 location, Int32 expectedBrickCount, Int32 expectedGrainCount, Int32 expectedLumberCount, Int32 expectedOreCount, Int32 expectedWoolCount)
    {
      var gameBoardData = new GameBoardData(BoardSizes.Standard);
      var result = gameBoardData.GetResourcesForLocation(location);
      result.BrickCount.ShouldBe(expectedBrickCount);
      result.GrainCount.ShouldBe(expectedGrainCount);
      result.LumberCount.ShouldBe(expectedLumberCount);
      result.OreCount.ShouldBe(expectedOreCount);
      result.WoolCount.ShouldBe(expectedWoolCount);
    }

    [Test]
    [Category("GameBoardData")]
    public void GetResourcesForRoll_StandardBoard_ReturnsCorrectResourcesForMatchingNeighbouringLocations()
    {
      var player1_Id = Guid.NewGuid();
      var player2_Id = Guid.NewGuid();
      var player3_Id = Guid.NewGuid();

      var roll = 8u;
      var gameBoardData = new GameBoardData(BoardSizes.Standard);
      gameBoardData.PlaceSettlement(player1_Id, 12u);
      gameBoardData.PlaceSettlement(player1_Id, 53u);
      gameBoardData.PlaceSettlement(player2_Id, 43u);
      gameBoardData.PlaceSettlement(player3_Id, 39u);

      var result = gameBoardData.GetResourcesForRoll(roll);

      result.Count.ShouldBe(2);
      result.ShouldContainKeyAndValue(player1_Id, new ResourceClutch(1, 1, 0, 0, 0 ));
      result.ShouldContainKeyAndValue(player2_Id, new ResourceClutch(0, 1, 0, 0, 0 ));
    }

    [Test]
    [Category("GameBoardData")]
    [TestCase(5u, 42u, ResourceTypes.Brick)]
    [TestCase(2u, 23u, ResourceTypes.Grain)]
    [TestCase(11u, 27u, ResourceTypes.Lumber)]
    [TestCase(6u, 20u, ResourceTypes.Ore)]
    [TestCase(10u, 12u, ResourceTypes.Wool)]
    public void GetResourcesForRoll_StandardBoard_ReturnsCorrectResources(UInt32 diceRoll, UInt32 location, ResourceTypes expectedType)
    {
      var playerId = Guid.NewGuid();
      var gameBoardData = new GameBoardData(BoardSizes.Standard);
      gameBoardData.PlaceSettlement(playerId, location);

      var result = gameBoardData.GetResourcesForRoll(diceRoll);

      ResourceClutch expectedResourceCounts = default(ResourceClutch);
      switch (expectedType)
      {
        case ResourceTypes.Brick: expectedResourceCounts = new ResourceClutch(1, 0, 0, 0, 0); break;
        case ResourceTypes.Grain: expectedResourceCounts = new ResourceClutch(0, 1, 0, 0, 0); break;
        case ResourceTypes.Lumber: expectedResourceCounts = new ResourceClutch(0, 0, 1, 0, 0 ); break;
        case ResourceTypes.Ore: expectedResourceCounts = new ResourceClutch(0, 0, 0, 1, 0); break;
        case ResourceTypes.Wool: expectedResourceCounts = new ResourceClutch(0, 0, 0, 0, 1); break;
      }

      result.Count.ShouldBe(1);
      result.ShouldContainKeyAndValue(playerId, expectedResourceCounts);
    }

    [Test]
    [Category("All")]
    [Category("GameBoardData")]
    public void GetPlayersForLocation_OnePlayerOnHex_ReturnPlayerIds()
    {
      // Arrange
      var playerId = Guid.NewGuid();
      var gameBoardData = new GameBoardData(BoardSizes.Standard);
      gameBoardData.PlaceSettlement(playerId, 0u);

      // Act
      var results = gameBoardData.GetPlayersForHex(0);

      // Assert
      results.Length.ShouldBe(1);
      results.ShouldContain(playerId);
    }

    [Test]
    [Category("All")]
    [Category("GameBoardData")]
    public void GetPlayersForHex_MultiplePlayersOnHex_ReturnPlayerIds()
    {
      // Arrange
      var firstPlayerId = Guid.NewGuid();
      var secondPlayerId = Guid.NewGuid();
      var gameBoardData = new GameBoardData(BoardSizes.Standard);
      gameBoardData.PlaceSettlement(firstPlayerId, 0u);
      gameBoardData.PlaceSettlement(secondPlayerId, 2u);

      // Act
      var results = gameBoardData.GetPlayersForHex(0);

      // Assert
      results.Length.ShouldBe(2);
      results.ShouldContain(firstPlayerId);
      results.ShouldContain(secondPlayerId);
    }

    [Test]
    [Category("All")]
    [Category("GameBoardData")]
    public void GetPlayersForHex_MultiplePlayerSettlementsOnHex_ReturnPlayerIds()
    {
      // Arrange
      var playerId = Guid.NewGuid();
      var gameBoardData = new GameBoardData(BoardSizes.Standard);
      gameBoardData.PlaceSettlement(playerId, 0u);
      gameBoardData.PlaceSettlement(playerId, 2u);

      // Act
      var results = gameBoardData.GetPlayersForHex(0);

      // Assert
      results.Length.ShouldBe(1);
      results.ShouldContain(playerId);
    }

    [Test]
    [Category("All")]
    [Category("GameBoardData")]
    public void GetPlayersForHex_NoPlayerSettlementsOnHex_ReturnNull()
    {
      // Arrange
      var playerId = Guid.NewGuid();
      var gameBoardData = new GameBoardData(BoardSizes.Standard);

      // Act
      var results = gameBoardData.GetPlayersForHex(0);

      // Assert
      results.ShouldBeNull();
    }

    [Test]
    [Category("All")]
    [Category("GameBoardData")]
    public void GetHexInformation_StandardBoard_ReturnsResourceTypeArray()
    {
      // Arrange
      var gameBoard = new GameBoardData(BoardSizes.Standard);

      // Act
      var data = gameBoard.GetHexInformation();

      // Assert
      data[0].ShouldBe(new Tuple<ResourceTypes, UInt32>(ResourceTypes.None, 0));
      data[1].ShouldBe(new Tuple<ResourceTypes, UInt32>(ResourceTypes.Brick, 8));
      data[2].ShouldBe(new Tuple<ResourceTypes, UInt32>(ResourceTypes.Ore, 5));

      data[3].ShouldBe(new Tuple<ResourceTypes, UInt32>(ResourceTypes.Brick, 4));
      data[4].ShouldBe(new Tuple<ResourceTypes, UInt32>(ResourceTypes.Lumber, 3));
      data[5].ShouldBe(new Tuple<ResourceTypes, UInt32>(ResourceTypes.Wool, 10));
      data[6].ShouldBe(new Tuple<ResourceTypes, UInt32>(ResourceTypes.Grain, 2));

      data[7].ShouldBe(new Tuple<ResourceTypes, UInt32>(ResourceTypes.Lumber, 11));
      data[8].ShouldBe(new Tuple<ResourceTypes, UInt32>(ResourceTypes.Ore, 6));
      data[9].ShouldBe(new Tuple<ResourceTypes, UInt32>(ResourceTypes.Grain, 11));
      data[10].ShouldBe(new Tuple<ResourceTypes, UInt32>(ResourceTypes.Wool, 9));
      data[11].ShouldBe(new Tuple<ResourceTypes, UInt32>(ResourceTypes.Lumber, 6));

      data[12].ShouldBe(new Tuple<ResourceTypes, UInt32>(ResourceTypes.Wool, 12));
      data[13].ShouldBe(new Tuple<ResourceTypes, UInt32>(ResourceTypes.Brick, 5));
      data[14].ShouldBe(new Tuple<ResourceTypes, UInt32>(ResourceTypes.Lumber, 4));
      data[15].ShouldBe(new Tuple<ResourceTypes, UInt32>(ResourceTypes.Ore, 3));

      data[16].ShouldBe(new Tuple<ResourceTypes, UInt32>(ResourceTypes.Grain, 9));
      data[17].ShouldBe(new Tuple<ResourceTypes, UInt32>(ResourceTypes.Wool, 10));
      data[18].ShouldBe(new Tuple<ResourceTypes, UInt32>(ResourceTypes.Grain, 8));
    }

    [Test]
    [Category("All")]
    [Category("GameBoardData")]
    public void GetSettlementInformation_OneSettlement_ReturnsSettlementDetails()
    {
      // Arrange
      var gameBoard = new GameBoardData(BoardSizes.Standard);
      var playerId = Guid.NewGuid();
      var settlementLocation = 12u;
      gameBoard.PlaceSettlement(playerId, settlementLocation);

      // Act
      var settlements = gameBoard.GetSettlementInformation();

      // Assert
      settlements.Count.ShouldBe(1);
      settlements.ShouldContainKeyAndValue(settlementLocation, playerId);
    }

    [Test]
    [Category("All")]
    [Category("GameBoardData")]
    public void GetRoadInformation_OneRoad_ReturnsRoadDetails()
    {
      // Arrange
      var playerId = Guid.NewGuid();
      var gameBoard = new GameBoardData(BoardSizes.Standard);
      gameBoard.PlaceStartingInfrastructure(playerId, 12, 4);

      // Act
      var roads = gameBoard.GetRoadInformation();

      // Assert
      roads.Length.ShouldBe(1);
      roads[0].ShouldBe(new Tuple<UInt32, UInt32, Guid>(12, 4, playerId));
    }

    [Test]
    [Category("All")]
    [Category("GameBoardData")]
    public void GetProductionValuesForLocation_LocationWithThreeResourceProducers_ReturnsExpectedProductionValues()
    {
      // Arrange
      var gameBoard = new GameBoardData(BoardSizes.Standard);

      // Act
      var productionValues = gameBoard.GetProductionValuesForLocation(12u);

      // Assert
      productionValues.Length.ShouldBe(3);
      productionValues.ShouldContain(8u);
      productionValues.ShouldContain(5u);
      productionValues.ShouldContain(10u);
    }

    [Test]
    [Category("All")]
    [Category("GameBoardData")]
    public void GetProductionValuesForLocation_LocationWithTwoResourceProducers_ReturnsExpectedProductionValues()
    {
      // Arrange
      var gameBoard = new GameBoardData(BoardSizes.Standard);

      // Act
      var productionValues = gameBoard.GetProductionValuesForLocation(4u);

      // Assert
      productionValues.Length.ShouldBe(2);
      productionValues.ShouldContain(8u);
      productionValues.ShouldContain(5u);
    }

    [Test]
    [Category("All")]
    [Category("GameBoardData")]
    public void GetProductionValuesForLocation_LocationIsOnDesertOnly_ReturnsEmptyArray()
    {
      // Arrange
      var gameBoard = new GameBoardData(BoardSizes.Standard);

      // Act
      var productionValues = gameBoard.GetProductionValuesForLocation(0u);

      // Assert
      productionValues.Length.ShouldBe(0);
    }

    /// <summary>
    /// No roads on the board so no longest road details can be passed back. Returns false.
    /// </summary>
    [Test]
    [Category("All")]
    [Category("GameBoardData")]
    public void TryGetLongestRoadDetails_NoRoadsOnBoard_ReturnsFalse()
    {
      // Arrange
      Int32 roadLength;
      Guid longestRoadPlayerId;
      var gameBoard = new GameBoardData(BoardSizes.Standard);

      // Act
      var result = gameBoard.TryGetLongestRoadDetails(out longestRoadPlayerId, out roadLength);

      // Assert
      result.ShouldBeFalse();
      longestRoadPlayerId.ShouldBe(Guid.Empty);
      roadLength.ShouldBe(-1);
    }

    /// <summary>
    /// Roads placed by two players - first player road is two segments long, second player road is one segment long.
    /// Returns true to indicate that there is a longest road.
    /// </summary>
    [Test]
    [Category("All")]
    [Category("GameBoardData")]
    public void TryGetLongestRoadDetails_OnePlayerHasLongestRoad_ReturnsTrue()
    {
      // Arrange
      Int32 roadLength;
      Guid longestRoadPlayerId;
      var gameBoard = new GameBoardData(BoardSizes.Standard);

      var playerId = Guid.NewGuid();
      var opponentId = Guid.NewGuid();
      gameBoard.PlaceRoad(playerId, 0u, 1u);
      gameBoard.PlaceRoad(playerId, 1u, 2u);
      gameBoard.PlaceRoad(opponentId, 18u, 19u);

      // Act
      var result = gameBoard.TryGetLongestRoadDetails(out longestRoadPlayerId, out roadLength);

      // Assert
      result.ShouldBeTrue();
    }

    /// <summary>
    /// Roads placed by two players - first player road is two segments long, second player road is one segment long.
    /// Longest road details passed back identifying the first player has having the longest road.
    /// </summary>
    [Test]
    [Category("All")]
    [Category("GameBoardData")]
    public void TryGetLongestRoadDetails_OnePlayerHasLongestRoad_ReturnsLongestRoadDetails()
    {
      // Arrange
      Int32 roadLength;
      Guid longestRoadPlayerId;
      var gameBoard = new GameBoardData(BoardSizes.Standard);

      var playerId = Guid.NewGuid();
      var opponentId = Guid.NewGuid();
      gameBoard.PlaceRoad(playerId, 0u, 1u);
      gameBoard.PlaceRoad(playerId, 1u, 2u);
      gameBoard.PlaceRoad(opponentId, 18u, 19u);

      // Act
      var result = gameBoard.TryGetLongestRoadDetails(out longestRoadPlayerId, out roadLength);

      // Assert
      longestRoadPlayerId.ShouldBe(playerId);
      roadLength.ShouldBe(2);
    }

    /// <summary>
    /// Two players have the longest road. Return false
    /// </summary>
    [Test]
    [Category("All")]
    [Category("GameBoardData")]
    public void TryGetLongestRoadDetails_TwoPlayersHaveTheLongestRoad_ReturnsFalse()
    {
      // Arrange
      Int32 roadLength;
      Guid longestRoadPlayerId;
      var gameBoard = new GameBoardData(BoardSizes.Standard);

      var playerId = Guid.NewGuid();
      var opponentId = Guid.NewGuid();
      gameBoard.PlaceRoad(playerId, 0u, 1u);
      gameBoard.PlaceRoad(opponentId, 18u, 19u);

      // Act
      var result = gameBoard.TryGetLongestRoadDetails(out longestRoadPlayerId, out roadLength);

      // Assert
      result.ShouldBeFalse();
    }

    /// <summary>
    /// One player has multiple roads that are the longest. Returns false. 
    /// </summary>
    [Test]
    [Category("All")]
    [Category("GameBoardData")]
    public void TryGetLongestRoadDetails_OnePlayerHasTwoRoads_ReturnsFalse()
    {
      // Arrange
      Int32 roadLength;
      Guid longestRoadPlayerId;
      var gameBoard = new GameBoardData(BoardSizes.Standard);

      var playerId = Guid.NewGuid();
      gameBoard.PlaceRoad(playerId, 0u, 1u);
      gameBoard.PlaceRoad(playerId, 2u, 3u);

      // Act
      var result = gameBoard.TryGetLongestRoadDetails(out longestRoadPlayerId, out roadLength);

      // Assert
      result.ShouldBeFalse();
    }

    /// <summary>
    /// Road placed around hex in a cycle. Road segments have not been placed in a consecutive manner but rather in a
    /// haphazard manner. Longest road details must not count road segments more than once.
    /// </summary>
    [Test]
    [Category("All")]
    [Category("GameBoardData")]
    public void TryGetLongestRoadDetails_LongestRoadIsCycle_ReturnsLongestRoadDetails()
    {
      // Arrange
      Int32 roadLength;
      Guid longestRoadPlayerId;
      var gameBoard = new GameBoardData(BoardSizes.Standard);

      var playerId = Guid.NewGuid();
      gameBoard.PlaceRoad(playerId, 0u, 1u);
      gameBoard.PlaceRoad(playerId, 1u, 2u);
      gameBoard.PlaceRoad(playerId, 0u, 8u);
      gameBoard.PlaceRoad(playerId, 8u, 9u);
      gameBoard.PlaceRoad(playerId, 10u, 2u);
      gameBoard.PlaceRoad(playerId, 10u, 9u);

      // Act
      var result = gameBoard.TryGetLongestRoadDetails(out longestRoadPlayerId, out roadLength);

      // Assert
      longestRoadPlayerId.ShouldBe(playerId);
      roadLength.ShouldBe(6);
    }

    /// <summary>
    /// Road placed around two hexes in a figure-of-eight. Road segments have not been placed in a consecutive manner but 
    /// rather in a haphazard manner. Longest road details must not count road segments more than once.
    /// </summary>
    [Test]
    [Category("All")]
    [Category("GameBoardData")]
    public void TryGetLongestRoadDetails_LongestRoadIsFigureOfEight_ReturnsLongestRoadDetails()
    {
      // Arrange
      Int32 roadLength;
      Guid longestRoadPlayerId;
      var gameBoard = new GameBoardData(BoardSizes.Standard);

      var playerId = Guid.NewGuid();
      gameBoard.PlaceRoad(playerId, 0u, 1u);
      gameBoard.PlaceRoad(playerId, 1u, 2u);
      gameBoard.PlaceRoad(playerId, 0u, 8u);
      gameBoard.PlaceRoad(playerId, 8u, 9u);
      gameBoard.PlaceRoad(playerId, 10u, 2u);
      gameBoard.PlaceRoad(playerId, 10u, 9u);

      gameBoard.PlaceRoad(playerId, 8u, 7u);
      gameBoard.PlaceRoad(playerId, 7u, 17u);
      gameBoard.PlaceRoad(playerId, 8u, 7u);
      gameBoard.PlaceRoad(playerId, 19u, 9u);
      gameBoard.PlaceRoad(playerId, 19u, 18u);
      gameBoard.PlaceRoad(playerId, 18u, 17u);

      // Act
      var result = gameBoard.TryGetLongestRoadDetails(out longestRoadPlayerId, out roadLength);

      // Assert
      longestRoadPlayerId.ShouldBe(playerId);
      roadLength.ShouldBe(10);
    }

    /// <summary>
    /// Road contains a fork. One branch is longer. Longest road details must be for long branch. Longest road count
    /// must include common 'trunk' road segment.
    /// </summary>
    [Test]
    [Category("All")]
    [Category("GameBoardData")]
    public void TryGetLongestRoadDetails_LongestRoadContainsFork_ReturnsLongestRoadDetails()
    {
      // Arrange
      Int32 roadLength;
      Guid longestRoadPlayerId;
      var gameBoard = new GameBoardData(BoardSizes.Standard);

      var playerId = Guid.NewGuid();
      gameBoard.PlaceRoad(playerId, 23u, 22u);

      gameBoard.PlaceRoad(playerId, 22u, 21u);
      gameBoard.PlaceRoad(playerId, 21u, 20u);
      gameBoard.PlaceRoad(playerId, 20u, 19u);
      gameBoard.PlaceRoad(playerId, 19u, 18u);
      gameBoard.PlaceRoad(playerId, 18u, 17u);

      gameBoard.PlaceRoad(playerId, 22u, 33u);
      gameBoard.PlaceRoad(playerId, 33u, 32u);
      gameBoard.PlaceRoad(playerId, 32u, 42u);
      gameBoard.PlaceRoad(playerId, 42u, 41u);

      // Act
      var result = gameBoard.TryGetLongestRoadDetails(out longestRoadPlayerId, out roadLength);

      // Assert
      longestRoadPlayerId.ShouldBe(playerId);
      roadLength.ShouldBe(6);
    }

    /// <summary>
    /// Two roads are connected with a road segment creating a new longest road. Returns true
    /// </summary>
    [Test]
    [Category("All")]
    [Category("GameBoardData")]
    public void TryGetLongestRoadDetails_TwoRoadsAreConnectedWithRoadSegment_ReturnsTrue()
    {
      // Arrange
      Int32 roadLength;
      Guid longestRoadPlayerId;
      var gameBoard = new GameBoardData(BoardSizes.Standard);

      var playerId = Guid.NewGuid();

      gameBoard.PlaceRoad(playerId, 21u, 20u);
      gameBoard.PlaceRoad(playerId, 20u, 31u);
      gameBoard.PlaceRoad(playerId, 31u, 30u);
      gameBoard.PlaceRoad(playerId, 30u, 40u);

      gameBoard.PlaceRoad(playerId, 33u, 32u);
      gameBoard.PlaceRoad(playerId, 32u, 42u);
      gameBoard.PlaceRoad(playerId, 42u, 41u);
      gameBoard.PlaceRoad(playerId, 41u, 49u);

      gameBoard.PlaceRoad(playerId, 31u, 32u);

      // Act
      var result = gameBoard.TryGetLongestRoadDetails(out longestRoadPlayerId, out roadLength);

      // Assert
      result.ShouldBeTrue();
    }

    /// <summary>
    /// Two roads are connected with a road segment creating a new longest road. Returns longest road details
    /// </summary>
    [Test]
    [Category("All")]
    [Category("GameBoardData")]
    public void TryGetLongestRoadDetails_TwoRoadsAreConnectedWithRoadSegment_ReturnsLongestRoadDetails()
    {
      // Arrange
      Int32 roadLength;
      Guid longestRoadPlayerId;
      var gameBoard = new GameBoardData(BoardSizes.Standard);

      var playerId = Guid.NewGuid();

      gameBoard.PlaceRoad(playerId, 21u, 20u);
      gameBoard.PlaceRoad(playerId, 20u, 31u);
      gameBoard.PlaceRoad(playerId, 31u, 30u);
      gameBoard.PlaceRoad(playerId, 30u, 40u);

      gameBoard.PlaceRoad(playerId, 33u, 32u);
      gameBoard.PlaceRoad(playerId, 32u, 42u);
      gameBoard.PlaceRoad(playerId, 42u, 41u);
      gameBoard.PlaceRoad(playerId, 41u, 49u);

      gameBoard.PlaceRoad(playerId, 31u, 32u);

      // Act
      var result = gameBoard.TryGetLongestRoadDetails(out longestRoadPlayerId, out roadLength);

      // Assert
      longestRoadPlayerId.ShouldBe(playerId);
      roadLength.ShouldBe(6);
    }

    /// <summary>
    /// Road is in a 6 (or 9) figure i.e. only on end, other end is connected to the road. Road segments not placed sequentially.
    /// Returns longest road details.
    /// </summary>
    [Test]
    [Category("All")]
    [Category("GameBoardData")]
    public void TryGetLongestRoadDetails_OnePlayerHasLongestRoadWithOneEndPoint_ReturnsLongestRoadDetails()
    {
      // Arrange
      Int32 roadLength;
      Guid longestRoadPlayerId;
      var gameBoard = new GameBoardData(BoardSizes.Standard);

      var playerId = Guid.NewGuid();
      gameBoard.PlaceRoad(playerId, 12u, 11u);
      gameBoard.PlaceRoad(playerId, 11u, 21u);
      gameBoard.PlaceRoad(playerId, 21u, 20u);
      gameBoard.PlaceRoad(playerId, 20u, 19u);
      gameBoard.PlaceRoad(playerId, 19u, 9u);
      gameBoard.PlaceRoad(playerId, 10u, 9u);
      gameBoard.PlaceRoad(playerId, 11u, 10u);

      // Act
      var result = gameBoard.TryGetLongestRoadDetails(out longestRoadPlayerId, out roadLength);

      // Assert
      result.ShouldBeTrue();
      longestRoadPlayerId.ShouldBe(playerId);
      roadLength.ShouldBe(8);
    }
    #endregion
  }
}
