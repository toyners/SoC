
namespace Jabberwocky.SoC.Library.UnitTests
{
  using System;
  using System.Collections.Generic;
  using GameBoards;
  using NUnit.Framework;
  using Shouldly;

  [TestFixture]
  [Category("All")]
  [Category("GameBoardData")]
  public class GameBoardData_UnitTests
  {
    private const UInt32 FirstSettlementLocation = 12;
    private const UInt32 FirstRoadEndLocation = 11;
    private const UInt32 SecondSettlementLocation = 25;
    private const UInt32 SecondRoadEndLocation = 15;

    #region Methods
    [Test]
    [Category("GameBoardData.CanPlaceRoad")]
    public void CanPlaceRoad_ConnectedToRoad_ReturnsValid()
    {
      // Arrange
      var playerId = Guid.NewGuid();
      var gameBoardData = new GameBoardData(BoardSizes.Standard);
      gameBoardData.PlaceStartingInfrastructure(playerId, FirstSettlementLocation, FirstRoadEndLocation);
      gameBoardData.PlaceStartingInfrastructure(playerId, SecondSettlementLocation, SecondRoadEndLocation);

      // Act
      var result = gameBoardData.CanPlaceRoad(playerId, 10, FirstRoadEndLocation);

      // Assert
      result.Status.ShouldBe(GameBoardData.VerificationStatus.Valid);
    }

    [Test]
    [Category("GameBoardData.CanPlaceRoad")]
    public void CanPlaceRoad_EmptyBoard_ReturnsStartingInfrastructureNotPresentWhenPlacingRoad()
    {
      // Arrange
      var gameBoardData = new GameBoardData(BoardSizes.Standard);

      // Act
      var result = gameBoardData.CanPlaceRoad(Guid.NewGuid(), 0, 1);

      // Assert
      result.Status.ShouldBe(GameBoardData.VerificationStatus.StartingInfrastructureNotPresentWhenPlacingRoad);
    }

    [Test]
    [Category("GameBoardData.CanPlaceRoad")]
    public void CanPlaceRoad_OnlyPlacedFirstStartingInfrastructure_StartingInfrastructureNotCompleteWhenPlacingRoad()
    {
      // Arrange
      var playerId = Guid.NewGuid();
      var gameBoardData = new GameBoardData(BoardSizes.Standard);
      gameBoardData.PlaceStartingInfrastructure(playerId, FirstSettlementLocation, FirstRoadEndLocation);

      // Act
      var result = gameBoardData.CanPlaceRoad(playerId, 10, FirstRoadEndLocation);

      // Assert
      result.Status.ShouldBe(GameBoardData.VerificationStatus.StartingInfrastructureNotCompleteWhenPlacingRoad);
    }

    [Test]
    [Category("GameBoardData.CanPlaceRoad")]
    [TestCase(10u, 11u)]
    [TestCase(11u, 10u)]
    public void CanPlaceRoad_JoiningToOtherRoads_ReturnsValid(UInt32 roadStartLocation, UInt32 roadEndLocation)
    {
      // Arrange
      var gameBoardData = new GameBoardData(BoardSizes.Standard);
      var playerId = Guid.NewGuid();
      gameBoardData.PlaceStartingInfrastructure(playerId, FirstSettlementLocation, FirstRoadEndLocation);
      gameBoardData.PlaceStartingInfrastructure(playerId, SecondSettlementLocation, SecondRoadEndLocation);

      // Act
      var result = gameBoardData.CanPlaceRoad(playerId, roadStartLocation, roadEndLocation);

      // Assert
      result.Status.ShouldBe(GameBoardData.VerificationStatus.Valid);
    }

    [Test]
    [Category("GameBoardData.CanPlaceRoad")]
    [TestCase(53u, 54u)] // Hanging over the edge 
    [TestCase(54u, 53u)] // Hanging over the edge
    [TestCase(100u, 101u)]
    public void CanPlaceRoad_OffBoard_ReturnsRoadIsInvalid(UInt32 start, UInt32 end)
    {
      // Arrange
      var playerId = Guid.NewGuid();
      var gameBoardData = new GameBoardData(BoardSizes.Standard);
      gameBoardData.PlaceStartingInfrastructure(playerId, FirstSettlementLocation, FirstRoadEndLocation);
      gameBoardData.PlaceStartingInfrastructure(playerId, SecondSettlementLocation, SecondRoadEndLocation);

      // Act
      var result = gameBoardData.CanPlaceRoad(playerId, start, end);

      // Assert
      result.Status.ShouldBe(GameBoardData.VerificationStatus.RoadIsOffBoard);
    }

    [Test]
    [Category("GameBoardData.CanPlaceRoad")]
    [TestCase(43u, 53u)]
    [TestCase(53u, 43u)]
    public void CanPlaceRoad_NoDirectConnection_ReturnsNoDirectConnection(UInt32 start, UInt32 end)
    {
      // Arrange
      var playerId = Guid.NewGuid();
      var gameBoardData = new GameBoardData(BoardSizes.Standard);
      gameBoardData.PlaceStartingInfrastructure(playerId, FirstSettlementLocation, FirstRoadEndLocation);
      gameBoardData.PlaceStartingInfrastructure(playerId, SecondSettlementLocation, SecondRoadEndLocation);

      // Act
      var result = gameBoardData.CanPlaceRoad(playerId, start, end);

      // Assert
      result.Status.ShouldBe(GameBoardData.VerificationStatus.NoDirectConnection);
    }

    [Test]
    [Category("GameBoardData.CanPlaceRoad")]
    public void CanPlaceRoad_RoadAlreadyBuilt_ReturnsRoadIsOccupied()
    {
      // Arrange
      var gameBoardData = new GameBoardData(BoardSizes.Standard);
      var playerId = Guid.NewGuid();
      gameBoardData.PlaceStartingInfrastructure(playerId, FirstSettlementLocation, FirstRoadEndLocation);
      gameBoardData.PlaceStartingInfrastructure(playerId, SecondSettlementLocation, SecondRoadEndLocation);

      var result = gameBoardData.CanPlaceRoad(playerId, FirstSettlementLocation, FirstRoadEndLocation);
      result.Status.ShouldBe(GameBoardData.VerificationStatus.RoadIsOccupied);
    }

    [Test]
    [Category("GameBoardData.CanPlaceRoad")]
    [TestCase(2u, 3u)]
    [TestCase(8u, 9u)]
    public void CanPlaceRoad_RoadNotConnectedToExistingInfrastructure_ReturnsRoadNotConnectedToExistingRoad(UInt32 roadStartLocation, UInt32 roadEndLocation)
    {
      // Arrange
      var playerId = Guid.NewGuid();
      var gameBoardData = new GameBoardData(BoardSizes.Standard);
      gameBoardData.PlaceStartingInfrastructure(playerId, FirstSettlementLocation, FirstRoadEndLocation);
      gameBoardData.PlaceStartingInfrastructure(playerId, SecondSettlementLocation, SecondRoadEndLocation);

      // Act
      var result = gameBoardData.CanPlaceRoad(playerId, roadStartLocation, roadEndLocation);

      // Assert
      result.Status.ShouldBe(GameBoardData.VerificationStatus.RoadNotConnectedToExistingRoad);
    }

    [Test]
    [Category("GameBoardData.CanPlaceSettlement")]
    public void CanPlaceSettlement_EmptyBoard_ReturnsStartingInfrastructureNotPresentWhenPlacingSettlement()
    {
      // Arrange
      var playerId = Guid.NewGuid();
      var gameBoardData = new GameBoardData(BoardSizes.Standard);

      // Act
      var result = gameBoardData.CanPlaceSettlement(playerId, 0);

      // Assert
      result.Status.ShouldBe(GameBoardData.VerificationStatus.StartingInfrastructureNotPresentWhenPlacingSettlement);
      result.LocationIndex.ShouldBe(0u);
      result.PlayerId.ShouldBe(Guid.Empty);
    }

    [Test]
    [Category("GameBoardData.CanPlaceSettlement")]
    public void CanPlaceSettlement_OnlyPlacedFirstStartingInfrastructure_ReturnsStartingInfrastructureNotPresentWhenPlacingSettlement()
    {
      // Arrange
      var playerId = Guid.NewGuid();
      var gameBoardData = new GameBoardData(BoardSizes.Standard);
      gameBoardData.PlaceStartingInfrastructure(playerId, FirstSettlementLocation, FirstRoadEndLocation);

      // Act
      var result = gameBoardData.CanPlaceSettlement(playerId, 0);

      // Assert
      result.Status.ShouldBe(GameBoardData.VerificationStatus.StartingInfrastructureNotCompleteWhenPlacingSettlement);
      result.LocationIndex.ShouldBe(0u);
      result.PlayerId.ShouldBe(Guid.Empty);
    }

    [Test]
    [Category("GameBoardData.CanPlaceSettlement")]
    public void CanPlaceSettlement_TryPlacingOnSettledLocation_ReturnsLocationIsOccupiedStatus()
    {
      // Arrange
      var playerId = Guid.NewGuid();
      var gameBoardData = new GameBoardData(BoardSizes.Standard);
      gameBoardData.PlaceStartingInfrastructure(playerId, FirstSettlementLocation, FirstRoadEndLocation);
      gameBoardData.PlaceStartingInfrastructure(playerId, SecondSettlementLocation, SecondRoadEndLocation);

      // Act
      var result = gameBoardData.CanPlaceSettlement(playerId, FirstSettlementLocation);

      // Assert
      result.Status.ShouldBe(GameBoardData.VerificationStatus.LocationIsOccupied);
      result.LocationIndex.ShouldBe(FirstSettlementLocation);
      result.PlayerId.ShouldBe(playerId);
    }

    [Test]
    [Category("GameBoardData.CanPlaceSettlement")]
    public void CanPlaceSettlement_TryPlacingOnInvalidLocation_ReturnsLocationIsInvalidStatus()
    {
      // Arrange
      var playerId = Guid.NewGuid();
      var gameBoardData = new GameBoardData(BoardSizes.Standard);
      gameBoardData.PlaceStartingInfrastructure(playerId, FirstSettlementLocation, FirstRoadEndLocation);
      gameBoardData.PlaceStartingInfrastructure(playerId, SecondSettlementLocation, SecondRoadEndLocation);

      // Act
      var result = gameBoardData.CanPlaceSettlement(playerId, 100);

      // Assert
      result.Status.ShouldBe(GameBoardData.VerificationStatus.LocationIsInvalid);
      result.LocationIndex.ShouldBe(0u);
      result.PlayerId.ShouldBe(Guid.Empty);
    }

    [Test]
    [Category("GameBoardData.CanPlaceSettlement")]
    [TestCase(4u)]
    [TestCase(11u)]
    [TestCase(13u)]
    public void CanPlaceSettlement_TryPlacingNextToSettledLocation_ReturnsTooCloseToSettlement(UInt32 newSettlementLocation)
    {
      // Arrange
      var playerId = Guid.NewGuid();
      var gameBoardData = new GameBoardData(BoardSizes.Standard);
      gameBoardData.PlaceStartingInfrastructure(playerId, FirstSettlementLocation, FirstRoadEndLocation);
      gameBoardData.PlaceStartingInfrastructure(playerId, SecondSettlementLocation, SecondRoadEndLocation);

      // Act
      var result = gameBoardData.CanPlaceSettlement(playerId, newSettlementLocation);

      // Assert
      result.Status.ShouldBe(GameBoardData.VerificationStatus.TooCloseToSettlement);
      result.LocationIndex.ShouldBe(FirstSettlementLocation);
      result.PlayerId.ShouldBe(playerId);
    }

    [Test]
    [Category("GameBoardData.CanPlaceSettlement")]
    public void CanPlaceSettlement_PlaceOnRoad_ReturnsValid()
    {
      // Arrange
      var playerId = Guid.NewGuid();
      var gameBoardData = new GameBoardData(BoardSizes.Standard);
      gameBoardData.PlaceStartingInfrastructure(playerId, FirstSettlementLocation, FirstRoadEndLocation);
      gameBoardData.PlaceStartingInfrastructure(playerId, SecondSettlementLocation, SecondRoadEndLocation);
      gameBoardData.PlaceRoadSegment(playerId, FirstRoadEndLocation, 10);

      // Act
      var result = gameBoardData.CanPlaceSettlement(playerId, 10);

      // Assert
      result.Status.ShouldBe(GameBoardData.VerificationStatus.Valid);
    }

    [Test]
    [Category("GameBoardData.CanPlaceSettlement")]
    public void CanPlaceSettlement_DontPlaceOnRoad_ReturnsNotConnectedToExisting()
    {
      // Arrange
      var playerId = Guid.NewGuid();
      var gameBoardData = new GameBoardData(BoardSizes.Standard);
      gameBoardData.PlaceStartingInfrastructure(playerId, FirstSettlementLocation, FirstRoadEndLocation);
      gameBoardData.PlaceStartingInfrastructure(playerId, SecondSettlementLocation, SecondRoadEndLocation);

      // Act
      var result = gameBoardData.CanPlaceSettlement(playerId, 10);

      // Assert
      result.Status.ShouldBe(GameBoardData.VerificationStatus.SettlementNotConnectedToExistingRoad);
    }

    [Test]
    [Category("GameBoardData.CanPlaceStartingInfrastructure")]
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
    [Category("GameBoardData.CanPlaceStartingInfrastructure")]
    public void CanPlaceStartingInfrastructure_TryPlacingOnSettledLocation_ReturnsLocationIsOccupiedStatus()
    {
      var gameBoardData = new GameBoardData(BoardSizes.Standard);
      var firstPlayerId = Guid.NewGuid();
      var secondPlayerId = Guid.NewGuid();
      gameBoardData.PlaceStartingInfrastructure(firstPlayerId, FirstSettlementLocation, FirstRoadEndLocation);

      var result = gameBoardData.CanPlaceStartingInfrastructure(secondPlayerId, FirstSettlementLocation, 4);
      result.Status.ShouldBe(GameBoardData.VerificationStatus.LocationIsOccupied);
      result.LocationIndex.ShouldBe(FirstSettlementLocation);
      result.PlayerId.ShouldBe(firstPlayerId);
    }

    [Test]
    [Category("GameBoardData.CanPlaceStartingInfrastructure")]
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
    [Category("GameBoardData.CanPlaceStartingInfrastructure")]
    public void CanPlaceStartingInfrastructure_OnlyPlacedFirstStartingInfrastructure_ReturnsValid()
    {
      var playerId = Guid.NewGuid();
      var locationOneIndex = 20u;
      var roadOneEndIndex = 21u;

      var gameBoardData = new GameBoardData(BoardSizes.Standard);
      gameBoardData.PlaceStartingInfrastructure(playerId, locationOneIndex, roadOneEndIndex);

      var locationTwoIndex = 0u;
      var roadTwoEndIndex = 1u;
      var result = gameBoardData.CanPlaceStartingInfrastructure(playerId, locationTwoIndex, roadTwoEndIndex);

      result.Status.ShouldBe(GameBoardData.VerificationStatus.Valid);
    }

    [Test]
    [Category("GameBoardData.CanPlaceStartingInfrastructure")]
    public void CanPlaceStartingInfrastructure_PlayerAlreadyPlacedAllStartingInfrastructure_ReturnsStartingInfrastructureAlreadyPresent()
    {
      // Arrange
      var playerId = Guid.NewGuid();
      var gameBoardData = new GameBoardData(BoardSizes.Standard);
      gameBoardData.PlaceStartingInfrastructure(playerId, FirstSettlementLocation, FirstRoadEndLocation);
      gameBoardData.PlaceStartingInfrastructure(playerId, SecondSettlementLocation, SecondRoadEndLocation);

      var locationThreeIndex = 0u;
      var roadThreeEndIndex = 1u;

      // Act
      var result = gameBoardData.CanPlaceStartingInfrastructure(playerId, locationThreeIndex, roadThreeEndIndex);

      // Assert
      result.Status.ShouldBe(GameBoardData.VerificationStatus.StartingInfrastructureAlreadyPresent);
      result.LocationIndex.ShouldBe(0u);
      result.PlayerId.ShouldBe(Guid.Empty);
    }

    [Test]
    [Category("GameBoardData.CanPlaceStartingInfrastructure")]
    [TestCase(20u, 31u, 19u, 18u)]
    [TestCase(20u, 19u, 21u, 22u)]
    [TestCase(20u, 19u, 31u, 30u)]
    public void CanPlaceStartingInfrastructure_TryPlacingNextToSettledLocation_ReturnsTooCloseToSettlement(UInt32 firstSettlementLocation, UInt32 firstEndRoadLocation, UInt32 secondSettlementLocation, UInt32 secondEndRoadLocation)
    {
      // Arrange
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
    [Category("GameBoardData.CanPlaceStartingInfrastructure")]
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
    [Category("GameBoardData.CanPlaceStartingInfrastructure")]
    [TestCase(53u, 54u, GameBoardData.VerificationStatus.RoadIsOffBoard)] // Hanging over the edge 
    [TestCase(54u, 53u, GameBoardData.VerificationStatus.LocationIsInvalid)] // Hanging over the edge
    [TestCase(100u, 101u, GameBoardData.VerificationStatus.LocationIsInvalid)]
    public void CanPlaceStartingInfrastructure_RoadOffBoard_ReturnsRoadIsOffBoard(UInt32 settlementLocation, UInt32 roadEndLocation, GameBoardData.VerificationStatus expectedStatus)
    {
      // Arrange
      var gameBoardData = new GameBoardData(BoardSizes.Standard);

      // Act
      var result = gameBoardData.CanPlaceStartingInfrastructure(Guid.NewGuid(), settlementLocation, roadEndLocation);

      // Assert
      result.Status.ShouldBe(expectedStatus);
      result.LocationIndex.ShouldBe(0u);
      result.PlayerId.ShouldBe(Guid.Empty);
    }

    [Test]
    [Category("GameBoardData.PlaceRoad")]
    public void PlaceRoad_EmptyBoard_ThrowsMeaningfulException()
    {
      // Arrange
      var gameBoardData = new GameBoardData(BoardSizes.Standard);

      // Act
      Action action = () => { gameBoardData.PlaceRoadSegment(Guid.NewGuid(), 0, 1); };

      // Assert
      action.ShouldThrow<GameBoardData.PlacementException>().Message.ShouldBe("Cannot place road before placing all initial infrastructure.");
    }

    [Test]
    [Category("GameBoardData.PlaceRoad")]
    public void PlaceRoad_OnlyPlacedFirstStartingInfrastructure_ThrowsMeaningfulException()
    {
      // Arrange
      var playerId = Guid.NewGuid();
      var gameBoardData = new GameBoardData(BoardSizes.Standard);
      gameBoardData.PlaceStartingInfrastructure(playerId, FirstSettlementLocation, FirstRoadEndLocation);

      // Act
      Action action = () => { gameBoardData.PlaceRoadSegment(playerId, 10, FirstRoadEndLocation); };

      // Assert
      action.ShouldThrow<GameBoardData.PlacementException>().Message.ShouldBe("Cannot place road before placing all initial infrastructure.");
    }

    [Test]
    [Category("GameBoardData.PlaceRoad")]
    [TestCase(53u, 54u)] // Hanging over the edge 
    [TestCase(54u, 53u)] // Hanging over the edge
    [TestCase(100u, 101u)]
    public void PlaceRoad_OffBoard_ThrowsMeaningfulException(UInt32 start, UInt32 end)
    {
      // Arrange
      var playerId = Guid.NewGuid();
      var gameBoardData = new GameBoardData(BoardSizes.Standard);
      gameBoardData.PlaceStartingInfrastructure(playerId, FirstSettlementLocation, FirstRoadEndLocation);
      gameBoardData.PlaceStartingInfrastructure(playerId, SecondSettlementLocation, SecondRoadEndLocation);

      // Act
      Action action = () => { gameBoardData.PlaceRoadSegment(playerId, start, end); };

      // Assert
      action.ShouldThrow<GameBoardData.PlacementException>().Message.ShouldBe("Cannot place road because board location is not valid.");
    }

    [Test]
    [Category("GameBoardData.PlaceRoad")]
    [TestCase(43u, 53u)]
    [TestCase(53u, 43u)]
    public void PlaceRoad_NoDirectConnection_ThrowsMeaningfulException(UInt32 start, UInt32 end)
    {
      // Arrange
      var playerId = Guid.NewGuid();
      var gameBoardData = new GameBoardData(BoardSizes.Standard);
      gameBoardData.PlaceStartingInfrastructure(playerId, FirstSettlementLocation, FirstRoadEndLocation);
      gameBoardData.PlaceStartingInfrastructure(playerId, SecondSettlementLocation, SecondRoadEndLocation);

      // Act
      Action action = () => { gameBoardData.PlaceRoadSegment(playerId, start, end); };

      // Assert
      action.ShouldThrow<GameBoardData.PlacementException>().Message.ShouldBe("Cannot place road because no direct connection between start location and end location.");
    }

    [Test]
    [Category("GameBoardData.PlaceRoad")]
    public void PlaceRoad_RoadAlreadyBuilt_ThrowsMeaningfulException()
    {
      // Arrange
      var gameBoardData = new GameBoardData(BoardSizes.Standard);
      var playerId = Guid.NewGuid();
      gameBoardData.PlaceStartingInfrastructure(playerId, FirstSettlementLocation, FirstRoadEndLocation);
      gameBoardData.PlaceStartingInfrastructure(playerId, SecondSettlementLocation, SecondRoadEndLocation);

      // Act
      Action action = () => { gameBoardData.PlaceRoadSegment(playerId, FirstSettlementLocation, FirstRoadEndLocation); };

      // Assert
      action.ShouldThrow<GameBoardData.PlacementException>().Message.ShouldBe("Cannot place road because road already exists.");
    }

    [Test]
    [Category("GameBoardData.PlaceRoad")]
    [TestCase(2u, 3u)]
    [TestCase(8u, 9u)]
    public void PlaceRoad_RoadNotConnectedToExistingInfrastructure_ThrowsMeaningfulException(UInt32 roadStartLocation, UInt32 roadEndLocation)
    {
      // Arrange
      var playerId = Guid.NewGuid();
      var gameBoardData = new GameBoardData(BoardSizes.Standard);
      gameBoardData.PlaceStartingInfrastructure(playerId, FirstSettlementLocation, FirstRoadEndLocation);
      gameBoardData.PlaceStartingInfrastructure(playerId, SecondSettlementLocation, SecondRoadEndLocation);

      // Act
      Action action = () => { gameBoardData.PlaceRoadSegment(playerId, roadStartLocation, roadEndLocation); };

      // Assert
      action.ShouldThrow<GameBoardData.PlacementException>().Message.ShouldBe("Cannot place road because it is not connected to an existing road segment.");
    }

    [Test]
    [Category("GameBoardData.PlaceSettlement")]
    public void PlaceSettlement_EmptyBoard_ThrowsMeaningfulException()
    {
      // Arrange
      var gameBoardData = new GameBoardData(BoardSizes.Standard);

      // Act
      Action action = () => { gameBoardData.PlaceSettlement(Guid.NewGuid(), 20u); };

      // Assert
      action.ShouldThrow<GameBoardData.PlacementException>().Message.ShouldBe("Cannot place settlement before placing all initial infrastructure.");
    }

    [Test]
    [Category("GameBoardData.PlaceSettlement")]
    public void PlaceSettlement_OnlyPlacedFirstStartingInfrastructure_ThrowsMeaningfulException()
    {
      // Arrange
      var playerId = Guid.NewGuid();
      var gameBoardData = new GameBoardData(BoardSizes.Standard);
      gameBoardData.PlaceStartingInfrastructure(playerId, FirstSettlementLocation, FirstRoadEndLocation);

      // Act
      Action action = () => { gameBoardData.PlaceSettlement(playerId, 0); };

      // Assert
      action.ShouldThrow<GameBoardData.PlacementException>().Message.ShouldBe("Cannot place settlement before placing all initial infrastructure.");
    }

    [Test]
    [Category("GameBoardData.PlaceSettlement")]
    public void PlaceSettlement_TryPlacingOnSettledLocation_ThrowsMeaningfulException()
    {
      // Arrange
      var playerId = Guid.NewGuid();
      var gameBoardData = new GameBoardData(BoardSizes.Standard);
      gameBoardData.PlaceStartingInfrastructure(playerId, FirstSettlementLocation, FirstRoadEndLocation);
      gameBoardData.PlaceStartingInfrastructure(playerId, SecondSettlementLocation, SecondRoadEndLocation);

      // Act
      Action action = () => { gameBoardData.PlaceSettlement(playerId, FirstSettlementLocation); };

      // Assert
      action.ShouldThrow<GameBoardData.PlacementException>().Message.ShouldBe("Cannot place settlement because location is already settled.");
    }

    [Test]
    [Category("GameBoardData.PlaceSettlement")]
    public void PlaceSettlement_TryPlacingOnInvalidLocation_ThrowsMeaningfulException()
    {
      // Arrange
      var playerId = Guid.NewGuid();
      var gameBoardData = new GameBoardData(BoardSizes.Standard);
      gameBoardData.PlaceStartingInfrastructure(playerId, FirstSettlementLocation, FirstRoadEndLocation);
      gameBoardData.PlaceStartingInfrastructure(playerId, SecondSettlementLocation, SecondRoadEndLocation);

      // Act
      Action action = () => { gameBoardData.PlaceSettlement(playerId, 100); };

      // Assert
      action.ShouldThrow<GameBoardData.PlacementException>().Message.ShouldBe("Cannot place settlement because location is not on board.");
    }

    [Test]
    [Category("GameBoardData.PlaceSettlement")]
    [TestCase(4u)]
    [TestCase(11u)]
    [TestCase(13u)]
    public void PlaceSettlement_TryPlacingNextToSettledLocation_ThrowsMeaningfulException(UInt32 newSettlementLocation)
    {
      // Arrange
      var playerId = Guid.NewGuid();
      var gameBoardData = new GameBoardData(BoardSizes.Standard);
      gameBoardData.PlaceStartingInfrastructure(playerId, FirstSettlementLocation, FirstRoadEndLocation);
      gameBoardData.PlaceStartingInfrastructure(playerId, SecondSettlementLocation, SecondRoadEndLocation);

      // Act
      Action action = () => { gameBoardData.PlaceSettlement(playerId, newSettlementLocation); };

      // Assert
      action.ShouldThrow<GameBoardData.PlacementException>().Message.ShouldBe("Cannot place settlement because location is too close to exising settlement.");
    }

    [Test]
    [Category("GameBoardData.PlaceSettlement")]
    public void PlaceSettlement_DontPlaceOnRoad_ThrowsMeaningfulException()
    {
      // Arrange
      var playerId = Guid.NewGuid();
      var gameBoardData = new GameBoardData(BoardSizes.Standard);
      gameBoardData.PlaceStartingInfrastructure(playerId, FirstSettlementLocation, FirstRoadEndLocation);
      gameBoardData.PlaceStartingInfrastructure(playerId, SecondSettlementLocation, SecondRoadEndLocation);

      // Act
      Action action = () => { gameBoardData.PlaceSettlement(playerId, 10); };

      // Assert
      action.ShouldThrow<GameBoardData.PlacementException>().Message.ShouldBe("Cannot place settlement because location is not on a road.");
    }

    [Test]
    [Category("GameBoardData.PlaceStartingInfrastructure")]
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
    [Category("GameBoardData.PlaceStartingInfrastructure")]
    public void PlaceStartingInfrastructure_TryPlacingOnInvalidLocation_ThrowsMeaningfulException()
    {
      // Arrange
      var playerId = Guid.NewGuid();
      var gameBoardData = new GameBoardData(BoardSizes.Standard);

      // Act
      Action action = () => { gameBoardData.PlaceStartingInfrastructure(playerId, 100, 101); };

      // Assert
      action.ShouldThrow<GameBoardData.PlacementException>().Message.ShouldBe("Cannot place settlement because location is not on board.");
    }

    [Test]
    [Category("GameBoardData.PlaceStartingInfrastructure")]
    public void PlaceStartingInfrastructure_PlayerAlreadyPlacedAllStartingInfrastructure_ThrowsMeaningfulException()
    {
      var playerId = Guid.NewGuid();
      var gameBoardData = new GameBoardData(BoardSizes.Standard);
      gameBoardData.PlaceStartingInfrastructure(playerId, FirstSettlementLocation, FirstRoadEndLocation);
      gameBoardData.PlaceStartingInfrastructure(playerId, SecondSettlementLocation, SecondRoadEndLocation);

      Action action = () => { gameBoardData.PlaceStartingInfrastructure(playerId, 0u, 1u); };

      action.ShouldThrow<GameBoardData.PlacementException>().Message.ShouldBe("Cannot place starting infrastructure more than once per player.");
    }

    [Test]
    [Category("GameBoardData.PlaceStartingInfrastructure")]
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
      Action action = () => { gameBoardData.PlaceStartingInfrastructure(secondPlayerId, secondSettlementLocation, secondEndRoadLocation); };

      // Assert
      action.ShouldThrow<GameBoardData.PlacementException>().Message.ShouldBe("Cannot place settlement because location is too close to exising settlement.");
    }

    [Test]
    [Category("GameBoardData.PlaceStartingInfrastructure")]
    public void PlaceStartingInfrastructure_NoDirectConnectionBetweenSettlementAndRoadEnd_ThrowsMeaningfulException()
    {
      // Arrange
      var playerId = Guid.NewGuid();
      var gameBoardData = new GameBoardData(BoardSizes.Standard);

      // Act
      Action action = () => { gameBoardData.PlaceStartingInfrastructure(playerId, 20u, 22u); };

      // Assert
      action.ShouldThrow<GameBoardData.PlacementException>().Message.ShouldBe("Cannot place road because no direct connection between start location and end location.");
    }

    [Test]
    [Category("GameBoardData.PlaceStartingInfrastructure")]
    [TestCase(53u, 54u, "Cannot place road because board location is not valid.")] // Hanging over the edge 
    [TestCase(54u, 53u, "Cannot place settlement because location is not on board.")] // Hanging over the edge
    [TestCase(100u, 101u, "Cannot place settlement because location is not on board.")]
    public void PlaceStartingInfrastructure_InfrastructureOffBoard_ThrowsMeaningfulException(UInt32 settlementLocation, UInt32 roadEndLocation, String expectedMessage)
    {
      // Arrange
      var gameBoardData = new GameBoardData(BoardSizes.Standard);

      // Act
      Action action = () => { gameBoardData.PlaceStartingInfrastructure(Guid.NewGuid(), settlementLocation, roadEndLocation); };

      // Assert
      action.ShouldThrow<GameBoardData.PlacementException>().Message.ShouldBe(expectedMessage);
    }

    [Test]
    [Category("GameBoardData.PlaceStartingInfrastructure")]
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

      // Check placing the settlement in the same location with a correct road end - will pass since nothing is there.
      var results = gameBoardData.CanPlaceStartingInfrastructure(playerId, location, 21);

      results.Status.ShouldBe(GameBoardData.VerificationStatus.Valid);
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
      // Arrange
      var gameBoardData = new GameBoardData(BoardSizes.Standard);
      gameBoardData.PlaceStartingInfrastructure(Guid.NewGuid(), FirstSettlementLocation, FirstRoadEndLocation);

      // Act
      var settlements = gameBoardData.GetSettlementsForPlayer(Guid.NewGuid());

      // Assert
      settlements.ShouldBeNull();
    }

    [Test]
    public void PlaceStartingInfrastructure_SettlementAndRoadAreValid_NoMeaningfulExceptionThrown()
    {
      var playerId = Guid.NewGuid();
      var gameBoardData = new GameBoardData(BoardSizes.Standard);

      // Act
      Action action = () =>
      {
        gameBoardData.PlaceStartingInfrastructure(playerId, FirstSettlementLocation, FirstRoadEndLocation);
        gameBoardData.PlaceStartingInfrastructure(playerId, SecondSettlementLocation, SecondRoadEndLocation);
      };

      // Assert
      action.ShouldNotThrow();
      gameBoardData.CanPlaceSettlement(playerId, FirstSettlementLocation).Status.ShouldBe(GameBoardData.VerificationStatus.LocationIsOccupied);
      gameBoardData.CanPlaceRoad(playerId, FirstSettlementLocation, FirstRoadEndLocation).Status.ShouldBe(GameBoardData.VerificationStatus.RoadIsOccupied);
    }

    [Test]
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
    public void GetResourcesForRoll_StandardBoard_ReturnsCorrectResourcesForMatchingNeighbouringLocations()
    {
      var player1_Id = Guid.NewGuid();
      var player2_Id = Guid.NewGuid();
      var player3_Id = Guid.NewGuid();

      var roll = 8u;
      var gameBoardData = new GameBoardData(BoardSizes.Standard);
      gameBoardData.PlaceStartingInfrastructure(player1_Id, 12, 11);
      gameBoardData.PlaceStartingInfrastructure(player1_Id, 53, 52);
      gameBoardData.PlaceStartingInfrastructure(player2_Id, 43, 42);
      gameBoardData.PlaceStartingInfrastructure(player3_Id, 39, 47);

      var result = gameBoardData.GetResourcesForRoll(roll);

      result.Count.ShouldBe(2);
      result.ShouldContainKeyAndValue(player1_Id, new ResourceClutch(1, 1, 0, 0, 0));
      result.ShouldContainKeyAndValue(player2_Id, new ResourceClutch(0, 1, 0, 0, 0));
    }

    [Test]
    [TestCase(5u, 42u, 41u, ResourceTypes.Brick)]
    [TestCase(2u, 23u, 22u, ResourceTypes.Grain)]
    [TestCase(11u, 27u, 28u, ResourceTypes.Lumber)]
    [TestCase(6u, 20u, 21u, ResourceTypes.Ore)]
    [TestCase(10u, 12u, 13u, ResourceTypes.Wool)]
    public void GetResourcesForRoll_StandardBoard_ReturnsCorrectResources(UInt32 diceRoll, UInt32 settlementLocation, UInt32 roadEndLocation, ResourceTypes expectedType)
    {
      var playerId = Guid.NewGuid();
      var gameBoardData = new GameBoardData(BoardSizes.Standard);
      gameBoardData.PlaceStartingInfrastructure(playerId, settlementLocation, roadEndLocation);

      var result = gameBoardData.GetResourcesForRoll(diceRoll);

      ResourceClutch expectedResourceCounts = default(ResourceClutch);
      switch (expectedType)
      {
        case ResourceTypes.Brick: expectedResourceCounts = new ResourceClutch(1, 0, 0, 0, 0); break;
        case ResourceTypes.Grain: expectedResourceCounts = new ResourceClutch(0, 1, 0, 0, 0); break;
        case ResourceTypes.Lumber: expectedResourceCounts = new ResourceClutch(0, 0, 1, 0, 0); break;
        case ResourceTypes.Ore: expectedResourceCounts = new ResourceClutch(0, 0, 0, 1, 0); break;
        case ResourceTypes.Wool: expectedResourceCounts = new ResourceClutch(0, 0, 0, 0, 1); break;
      }

      result.Count.ShouldBe(1);
      result.ShouldContainKeyAndValue(playerId, expectedResourceCounts);
    }

    [Test]
    public void GetPlayersForLocation_OnePlayerOnHex_ReturnPlayerIds()
    {
      // Arrange
      var playerId = Guid.NewGuid();
      var gameBoardData = new GameBoardData(BoardSizes.Standard);
      gameBoardData.PlaceStartingInfrastructure(playerId, 0, 8);

      // Act
      var results = gameBoardData.GetPlayersForHex(0);

      // Assert
      results.Length.ShouldBe(1);
      results.ShouldContain(playerId);
    }

    [Test]
    public void GetPlayersForHex_MultiplePlayersOnHex_ReturnPlayerIds()
    {
      // Arrange
      var firstPlayerId = Guid.NewGuid();
      var secondPlayerId = Guid.NewGuid();
      var gameBoardData = new GameBoardData(BoardSizes.Standard);
      gameBoardData.PlaceStartingInfrastructure(firstPlayerId, 0, 8);
      gameBoardData.PlaceStartingInfrastructure(secondPlayerId, 2, 1);

      // Act
      var results = gameBoardData.GetPlayersForHex(0);

      // Assert
      results.Length.ShouldBe(2);
      results.ShouldContain(firstPlayerId);
      results.ShouldContain(secondPlayerId);
    }

    [Test]
    public void GetPlayersForHex_MultiplePlayerSettlementsOnHex_ReturnPlayerIds()
    {
      // Arrange
      var playerId = Guid.NewGuid();
      var gameBoardData = new GameBoardData(BoardSizes.Standard);
      gameBoardData.PlaceStartingInfrastructure(playerId, 0, 8);
      gameBoardData.PlaceStartingInfrastructure(playerId, SecondSettlementLocation, SecondRoadEndLocation);
      gameBoardData.PlaceRoadSegment(playerId, 8, 9);
      gameBoardData.PlaceSettlement(playerId, 9);

      // Act
      var results = gameBoardData.GetPlayersForHex(0);

      // Assert
      results.Length.ShouldBe(1);
      results.ShouldContain(playerId);
    }

    [Test]
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
    public void GetSettlementInformation_OneSettlement_ReturnsSettlementDetails()
    {
      // Arrange
      var playerId = Guid.NewGuid();
      var gameBoard = new GameBoardData(BoardSizes.Standard);
      var settlementLocation = 12u;
      gameBoard.PlaceStartingInfrastructure(playerId, FirstSettlementLocation, FirstRoadEndLocation);

      // Act
      var settlements = gameBoard.GetSettlementInformation();

      // Assert
      settlements.Count.ShouldBe(1);
      settlements.ShouldContainKeyAndValue(FirstSettlementLocation, playerId);
    }

    [Test]
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
    [Category("GameBoardData.TryGetLongestRoadDetails")]
    public void TryGetLongestRoadDetails_NoRoadsOnBoard_ReturnsFalse()
    {
      // Arrange
      UInt32[] road;
      Guid longestRoadPlayerId;
      var gameBoard = new GameBoardData(BoardSizes.Standard);

      // Act
      var result = gameBoard.TryGetLongestRoadDetails(out longestRoadPlayerId, out road);

      // Assert
      result.ShouldBeFalse();
      longestRoadPlayerId.ShouldBe(Guid.Empty);
      road.ShouldBeNull();
    }

    /// <summary>
    /// Roads placed by two players - first player road is three segments long, second player road is two segment long.
    /// Returns true to indicate that there is a longest road.
    /// </summary>
    [Test]
    [Category("GameBoardData.TryGetLongestRoadDetails")]
    public void TryGetLongestRoadDetails_OnePlayerHasLongestRoad_ReturnsTrue()
    {
      // Arrange
      var gameBoard = new GameBoardData(BoardSizes.Standard);

      var playerId = Guid.NewGuid();
      var opponentId = Guid.NewGuid();
      gameBoard.PlaceStartingInfrastructure(playerId, FirstSettlementLocation, FirstRoadEndLocation);
      gameBoard.PlaceStartingInfrastructure(playerId, SecondSettlementLocation, SecondRoadEndLocation);
      gameBoard.PlaceRoadSegment(playerId, FirstRoadEndLocation, 10);
      gameBoard.PlaceRoadSegment(playerId, 10, 2);

      gameBoard.PlaceStartingInfrastructure(opponentId, 0, 1);
      gameBoard.PlaceStartingInfrastructure(opponentId, 53, 52);
      gameBoard.PlaceRoadSegment(opponentId, 1, 2);

      // Act
      UInt32[] road;
      Guid longestRoadPlayerId;
      var result = gameBoard.TryGetLongestRoadDetails(out longestRoadPlayerId, out road);

      // Assert
      result.ShouldBeTrue();
      longestRoadPlayerId.ShouldBe(playerId);
      var result1 = new List<UInt32> { FirstSettlementLocation, FirstRoadEndLocation, 10u, 2u };
      var result2 = new List<UInt32>(result1);
      result2.Reverse();
      this.RoadShouldBeSameAsOneOf(road, result1, result2);
    }

    /// <summary>
    /// Two players have the longest road. Return false
    /// </summary>
    [Test]
    [Category("GameBoardData.TryGetLongestRoadDetails")]
    public void TryGetLongestRoadDetails_TwoPlayersHaveTheLongestRoad_ReturnsFalse()
    {
      // Arrange
      var gameBoard = new GameBoardData(BoardSizes.Standard);

      var playerId = Guid.NewGuid();
      var opponentId = Guid.NewGuid();
      gameBoard.PlaceStartingInfrastructure(playerId, FirstSettlementLocation, FirstRoadEndLocation);
      gameBoard.PlaceStartingInfrastructure(playerId, SecondSettlementLocation, SecondRoadEndLocation);
      gameBoard.PlaceRoadSegment(playerId, FirstRoadEndLocation, 10);
      gameBoard.PlaceRoadSegment(playerId, 10, 2);

      gameBoard.PlaceStartingInfrastructure(opponentId, 0, 1);
      gameBoard.PlaceStartingInfrastructure(opponentId, 53, 52);
      gameBoard.PlaceRoadSegment(opponentId, 1, 2);
      gameBoard.PlaceRoadSegment(opponentId, 2, 3);

      // Act
      UInt32[] road;
      Guid longestRoadPlayerId;
      var result = gameBoard.TryGetLongestRoadDetails(out longestRoadPlayerId, out road);

      // Assert
      result.ShouldBeFalse();
    }

    /// <summary>
    /// One player has multiple roads that are the longest. Returns false. 
    /// </summary>
    [Test]
    [Category("GameBoardData.TryGetLongestRoadDetails")]
    public void TryGetLongestRoadDetails_OnePlayerHasTwoRoadsOfEqualLength_ReturnsLongestRoadDetails()
    {
      // Arrange      
      var gameBoard = new GameBoardData(BoardSizes.Standard);

      var playerId = Guid.NewGuid();
      gameBoard.PlaceStartingInfrastructure(playerId, FirstSettlementLocation, FirstRoadEndLocation);
      gameBoard.PlaceStartingInfrastructure(playerId, SecondSettlementLocation, SecondRoadEndLocation);
      gameBoard.PlaceRoadSegment(playerId, FirstRoadEndLocation, 10);
      gameBoard.PlaceRoadSegment(playerId, 10, 2);

      gameBoard.PlaceRoadSegment(playerId, SecondRoadEndLocation, 14);
      gameBoard.PlaceRoadSegment(playerId, 14, 6);

      // Act
      UInt32[] road;
      Guid longestRoadPlayerId;
      var result = gameBoard.TryGetLongestRoadDetails(out longestRoadPlayerId, out road);

      // Assert
      result.ShouldBeTrue();
      longestRoadPlayerId.ShouldBe(playerId);
      var result1 = new List<UInt32> { FirstSettlementLocation, FirstRoadEndLocation, 10, 2 };
      var result2 = new List<UInt32>(result1);
      result2.Reverse();
      var result3 = new List<UInt32> { SecondSettlementLocation, SecondRoadEndLocation, 14, 6 };
      var result4 = new List<UInt32>(result3);
      result4.Reverse();

      this.RoadShouldBeSameAsOneOf(road, result1, result2, result3, result4);
    }

    /// <summary>
    /// Road placed around hex in a cycle. Road segments have not been placed in a consecutive manner but rather in a
    /// haphazard manner. Longest road details must not count road segments more than once.
    /// </summary>
    [Test]
    [Category("GameBoardData.TryGetLongestRoadDetails")]
    public void TryGetLongestRoadDetails_LongestRoadIsCycle_ReturnsLongestRoadDetails()
    {
      // Arrange
      var gameBoard = new GameBoardData(BoardSizes.Standard);

      var playerId = Guid.NewGuid();
      gameBoard.PlaceStartingInfrastructure(playerId, FirstSettlementLocation, FirstRoadEndLocation);
      gameBoard.PlaceStartingInfrastructure(playerId, SecondSettlementLocation, SecondRoadEndLocation);

      gameBoard.PlaceRoadSegment(playerId, FirstSettlementLocation, 13);
      gameBoard.PlaceRoadSegment(playerId, FirstRoadEndLocation, 21);
      gameBoard.PlaceRoadSegment(playerId, 21, 22);
      gameBoard.PlaceRoadSegment(playerId, 22, 23);
      gameBoard.PlaceRoadSegment(playerId, 13, 23);

      // Act
      UInt32[] road;
      Guid longestRoadPlayerId;
      var result = gameBoard.TryGetLongestRoadDetails(out longestRoadPlayerId, out road);

      // Assert
      result.ShouldBeTrue();
      longestRoadPlayerId.ShouldBe(playerId);
      var result1 = new List<UInt32> { FirstSettlementLocation, FirstRoadEndLocation, 21u, 22u, 23u, 13u, FirstSettlementLocation };
      var result2 = new List<UInt32>(result1);
      result2.Reverse();
      this.RoadShouldBeSameAsOneOf(road, result1, result2);
    }

    /// <summary>
    /// Road placed around two hexes in a figure-of-eight. Road segments have not been placed in a consecutive manner but 
    /// rather in a haphazard manner. Longest road details must not count road segments more than once.
    /// </summary>
    [Test]
    [Category("GameBoardData.TryGetLongestRoadDetails")]
    public void TryGetLongestRoadDetails_LongestRoadIsFigureOfEight_ReturnsLongestRoadDetails()
    {
      // Arrange
      var gameBoard = new GameBoardData(BoardSizes.Standard);

      var playerId = Guid.NewGuid();
      gameBoard.PlaceStartingInfrastructure(playerId, FirstSettlementLocation, FirstRoadEndLocation);
      gameBoard.PlaceStartingInfrastructure(playerId, SecondSettlementLocation, SecondRoadEndLocation);

      gameBoard.PlaceRoadSegment(playerId, FirstRoadEndLocation, 21);
      gameBoard.PlaceRoadSegment(playerId, 21, 22);
      gameBoard.PlaceRoadSegment(playerId, FirstSettlementLocation, 13);
      gameBoard.PlaceRoadSegment(playerId, 13, 23);
      gameBoard.PlaceRoadSegment(playerId, 22, 23);

      gameBoard.PlaceRoadSegment(playerId, 21, 20);
      gameBoard.PlaceRoadSegment(playerId, 20, 31);
      gameBoard.PlaceRoadSegment(playerId, 31, 32);
      gameBoard.PlaceRoadSegment(playerId, 32, 33);
      gameBoard.PlaceRoadSegment(playerId, 22, 33);

      // Act
      UInt32[] road;
      Guid longestRoadPlayerId;
      var result = gameBoard.TryGetLongestRoadDetails(out longestRoadPlayerId, out road);

      // Assert
      longestRoadPlayerId.ShouldBe(playerId);
      var result1 = new List<UInt32> { 21, 20, 31, 32, 33, 22, 21, 11, 12, 13, 23, 22};
      var result2 = new List<UInt32>(result1);
      result2.Reverse();

      var result3 = new List<UInt32> { 21, 11, 12, 13, 23, 22, 21, 20, 31, 32, 33, 22 };
      var result4 = new List<UInt32>(result3);
      result4.Reverse();

      this.RoadShouldBeSameAsOneOf(road, result1, result2, result3, result4);
    }

    /// <summary>
    /// Road contains a fork. One branch is longer. Longest road details must include both long branch and short branch.
    /// Test cases used to vary the build order of the branches.
    /// </summary>
    [Test]
    [Category("GameBoardData.TryGetLongestRoadDetails")]
    [TestCase(new UInt32[] { 23, 22, 22, 21, 21, 20, 20, 19, 19, 18, 18, 17, 22, 33, 33, 32, 32, 42, 42, 41 })] // Longest branch first
    [TestCase(new UInt32[] { 23, 22, 22, 33, 33, 32, 32, 42, 42, 41, 22, 21, 21, 20, 20, 19, 19, 18, 18, 17 })] // Shortest branch first
    public void TryGetLongestRoadDetails_LongestRoadContainsFork_ReturnsLongestRoadDetails(UInt32[] locations)
    {
      // Arrange
      var gameBoard = new GameBoardData(BoardSizes.Standard);

      var playerId = Guid.NewGuid();
      gameBoard.PlaceStartingInfrastructure(playerId, 0, 1);

      gameBoard.PlaceStartingInfrastructure(playerId, locations[0], locations[1]);
      for (var index = 2; index < locations.Length; index += 2)
      {
        gameBoard.PlaceRoadSegment(playerId, locations[index], locations[index + 1]);
      }

      // Act
      UInt32[] road;
      Guid longestRoadPlayerId;
      var result = gameBoard.TryGetLongestRoadDetails(out longestRoadPlayerId, out road);

      // Assert
      longestRoadPlayerId.ShouldBe(playerId);
      var result1 = new List<UInt32> { 17u, 18u, 19u, 20u, 21u, 22u, 33u, 32u, 42u, 41u };
      var result2 = new List<UInt32>(result1);
      result2.Reverse();
      this.RoadShouldBeSameAsOneOf(road, result1, result2);
    }

    /// <summary>
    /// Road contains multiple branches. Tiny branch is ignored. Longest road details must include both long branch and short branch.
    /// Test cases used to vary the build order of the branches.
    /// </summary>
    [Test]
    [Category("GameBoardData.TryGetLongestRoadDetails")]
    [TestCase(new UInt32[] { 22, 21, 21, 20, 20, 19, 19, 18, 18, 17 }, // Longest branch
              new UInt32[] { 19, 9 }, // Tiny branch
              new UInt32[] { 22, 33, 33, 32, 32, 42, 42, 41 })] // Short branch
    [TestCase(new UInt32[] { 22, 21, 21, 20, 20, 19, 19, 18, 18, 17 }, // Longest branch
              new UInt32[] { 22, 33, 33, 32, 32, 42, 42, 41 }, // Short branch
              new UInt32[] { 19, 9 })] // Tiny Branch
    [TestCase(new UInt32[] { 22, 33, 33, 32, 32, 42, 42, 41 }, // Short branch
              new UInt32[] { 22, 21, 21, 20, 20, 19, 19, 18, 18, 17 }, // Longest branch
              new UInt32[] { 19, 9 })] // Tiny Branch
    public void TryGetLongestRoadDetails_LongestRoadContainsMultipleForks_ReturnsLongestRoadDetails(UInt32[] firstBranch, UInt32[] secondBranch, UInt32[] lastBranch)
    {
      // Arrange
      var gameBoard = new GameBoardData(BoardSizes.Standard);

      var playerId = Guid.NewGuid();
      gameBoard.PlaceStartingInfrastructure(playerId, 0, 1);

      gameBoard.PlaceStartingInfrastructure(playerId, 23, 22);

      this.BuidRoadBranch(gameBoard, playerId, firstBranch);
      this.BuidRoadBranch(gameBoard, playerId, secondBranch);
      this.BuidRoadBranch(gameBoard, playerId, lastBranch);

      // Act
      UInt32[] road;
      Guid longestRoadPlayerId;
      var result = gameBoard.TryGetLongestRoadDetails(out longestRoadPlayerId, out road);

      // Assert
      longestRoadPlayerId.ShouldBe(playerId);
      var result1 = new List<UInt32> { 17u, 18u, 19u, 20u, 21u, 22u, 33u, 32u, 42u, 41u };
      var result2 = new List<UInt32>(result1);
      result2.Reverse();
      this.RoadShouldBeSameAsOneOf(road, result1, result2);
    }

    /// <summary>
    /// Two roads are connected with a road segment creating a new longest road. Returns true
    /// </summary>
    [Test]
    [Category("GameBoardData.TryGetLongestRoadDetails")]
    public void TryGetLongestRoadDetails_TwoRoadsAreConnectedWithRoadSegment_ReturnsTrue()
    {
      // Arrange
      var gameBoard = new GameBoardData(BoardSizes.Standard);

      var playerId = Guid.NewGuid();
      gameBoard.PlaceStartingInfrastructure(playerId, 21, 20);
      gameBoard.PlaceStartingInfrastructure(playerId, 33, 32);

      gameBoard.PlaceRoadSegment(playerId, 20, 31);
      gameBoard.PlaceRoadSegment(playerId, 31, 30);
      gameBoard.PlaceRoadSegment(playerId, 30, 40);

      gameBoard.PlaceRoadSegment(playerId, 32, 42);
      gameBoard.PlaceRoadSegment(playerId, 42, 41);
      gameBoard.PlaceRoadSegment(playerId, 41, 49);

      gameBoard.PlaceRoadSegment(playerId, 31, 32);

      // Act
      UInt32[] road;
      Guid longestRoadPlayerId;
      var result = gameBoard.TryGetLongestRoadDetails(out longestRoadPlayerId, out road);

      // Assert
      result.ShouldBeTrue();
      longestRoadPlayerId.ShouldBe(playerId);

      var result1 = new List<UInt32> { 21, 20, 31, 32, 42, 41, 49 };
      var result2 = new List<UInt32> { 40, 30, 31, 32, 42, 41, 49 };
      var result3 = new List<UInt32>(result1);
      result3.Reverse();
      var result4 = new List<UInt32>(result2);
      result4.Reverse();

      this.RoadShouldBeSameAsOneOf(road, result1, result2, result3, result4);
    }

    /// <summary>
    /// Road is in a 6 (or 9) figure i.e. only one end, other end is connected to the road. Road segments not placed sequentially.
    /// Returns longest road details.
    /// </summary>
    [Test]
    [Category("GameBoardData.TryGetLongestRoadDetails")]
    public void TryGetLongestRoadDetails_OnePlayerHasLongestRoadWithOneEndPoint_ReturnsLongestRoadDetails()
    {
      // Arrange
      var gameBoard = new GameBoardData(BoardSizes.Standard);

      var playerId = Guid.NewGuid();
      gameBoard.PlaceStartingInfrastructure(playerId, FirstSettlementLocation, FirstRoadEndLocation);
      gameBoard.PlaceStartingInfrastructure(playerId, SecondSettlementLocation, SecondRoadEndLocation);
      gameBoard.PlaceRoadSegment(playerId, FirstRoadEndLocation, 21u);
      gameBoard.PlaceRoadSegment(playerId, 21u, 20u);
      gameBoard.PlaceRoadSegment(playerId, 20u, 19u);
      gameBoard.PlaceRoadSegment(playerId, 19u, 9u);
      gameBoard.PlaceRoadSegment(playerId, 10u, 9u);
      gameBoard.PlaceRoadSegment(playerId, FirstRoadEndLocation, 10u);

      // Act
      UInt32[] road;
      Guid longestRoadPlayerId;
      var result = gameBoard.TryGetLongestRoadDetails(out longestRoadPlayerId, out road);

      // Assert
      result.ShouldBeTrue();
      longestRoadPlayerId.ShouldBe(playerId);
      var result1 = new List<UInt32> { FirstSettlementLocation, FirstRoadEndLocation, 21, 20, 19, 9, 10, FirstRoadEndLocation };
      var result2 = new List<UInt32>(result1);
      result2.Reverse();
      var result3 = new List<UInt32> { FirstSettlementLocation, FirstRoadEndLocation, 10, 9, 19, 20, 21, FirstRoadEndLocation };
      var result4 = new List<UInt32>(result3);
      result4.Reverse();
      this.RoadShouldBeSameAsOneOf(road, result1, result2, result3, result4);
    }

    /// <summary>
    /// Longest road is two segments long with settlement in the middle. Should still return the road as longest.
    /// </summary>
    [Test]
    [Category("GameBoardData.TryGetLongestRoadDetails")]
    public void TryGetLongestRoadDetails_StartingSettlementNotOnEndOfRoad_ReturnsLongestRoadDetails()
    {
      // Arrange
      var gameBoard = new GameBoardData(BoardSizes.Standard);

      var playerId = Guid.NewGuid();
      gameBoard.PlaceStartingInfrastructure(playerId, FirstSettlementLocation, FirstRoadEndLocation);
      gameBoard.PlaceStartingInfrastructure(playerId, SecondSettlementLocation, SecondRoadEndLocation);
      gameBoard.PlaceRoadSegment(playerId, FirstSettlementLocation, 4);
      
      // Act
      UInt32[] road;
      Guid longestRoadPlayerId;
      var result = gameBoard.TryGetLongestRoadDetails(out longestRoadPlayerId, out road);

      // Assert
      result.ShouldBeTrue();
      longestRoadPlayerId.ShouldBe(playerId);
      var result1 = new List<UInt32> { FirstRoadEndLocation, FirstSettlementLocation, 4u };
      var result2 = new List<UInt32>(result1);
      result2.Reverse();
      this.RoadShouldBeSameAsOneOf(road, result1, result2);
    }

    /// <summary>
    /// Settlment has road leading away in all three directions. Two roads are same size. One road is longer.
    /// </summary>
    [Test]
    [Category("GameBoardData.TryGetLongestRoadDetails")]
    public void TryGetLongestRoadDetails_SettlementHasRoadsInAllDirections_ReturnsLongestRoadDetails()
    {
      // Arrange
      var gameBoard = new GameBoardData(BoardSizes.Standard);

      var playerId = Guid.NewGuid();
      gameBoard.PlaceStartingInfrastructure(playerId, FirstSettlementLocation, FirstRoadEndLocation);
      gameBoard.PlaceStartingInfrastructure(playerId, SecondSettlementLocation, SecondRoadEndLocation);
      gameBoard.PlaceRoadSegment(playerId, FirstSettlementLocation, 4);
      gameBoard.PlaceRoadSegment(playerId, FirstRoadEndLocation, 10);
      gameBoard.PlaceRoadSegment(playerId, FirstSettlementLocation, 13);

      // Act
      UInt32[] road;
      Guid longestRoadPlayerId;
      var result = gameBoard.TryGetLongestRoadDetails(out longestRoadPlayerId, out road);

      // Assert
      result.ShouldBeTrue();
      longestRoadPlayerId.ShouldBe(playerId);
      var result1 = new List<UInt32> { 10u, FirstRoadEndLocation, FirstSettlementLocation, 4u };
      var result2 = new List<UInt32> { 10u, FirstRoadEndLocation, FirstSettlementLocation, 13u };
      var result3 = new List<UInt32>(result1);
      result3.Reverse();
      var result4 = new List<UInt32>(result2);
      result2.Reverse();
      this.RoadShouldBeSameAsOneOf(road, result1, result2, result3, result4);
    }

    /// <summary>
    /// Settlement is between two closed loops.
    /// </summary>
    [Test]
    [Category("GameBoardData.TryGetLongestRoadDetails")]
    public void TryGetLongestRoadDetails_SettlementBetweenTwoLoops_ReturnsLongestRoadDetails()
    {
      // Arrange
      var gameBoard = new GameBoardData(BoardSizes.Standard);

      var playerId = Guid.NewGuid();
      gameBoard.PlaceStartingInfrastructure(playerId, FirstSettlementLocation, FirstRoadEndLocation);
      gameBoard.PlaceStartingInfrastructure(playerId, 0, 1);

      gameBoard.PlaceRoadSegment(playerId, FirstRoadEndLocation, 21);
      gameBoard.PlaceRoadSegment(playerId, FirstRoadEndLocation, 10);
      gameBoard.PlaceRoadSegment(playerId, 21, 20);
      gameBoard.PlaceRoadSegment(playerId, 20, 19);
      gameBoard.PlaceRoadSegment(playerId, 19, 9);
      gameBoard.PlaceRoadSegment(playerId, 9, 10);

      gameBoard.PlaceRoadSegment(playerId, FirstSettlementLocation, 13);
      gameBoard.PlaceRoadSegment(playerId, 13, 14);
      gameBoard.PlaceRoadSegment(playerId, 13, 23);
      gameBoard.PlaceRoadSegment(playerId, 23, 24);
      gameBoard.PlaceRoadSegment(playerId, 24, 25);
      gameBoard.PlaceRoadSegment(playerId, 25, 15);
      gameBoard.PlaceRoadSegment(playerId, 15, 14);

      // Act
      UInt32[] road;
      Guid longestRoadPlayerId;
      var result = gameBoard.TryGetLongestRoadDetails(out longestRoadPlayerId, out road);

      // Assert
      result.ShouldBeTrue();
      longestRoadPlayerId.ShouldBe(playerId);
      var result1 = new List<UInt32> { FirstRoadEndLocation, 10u, 9u, 19u, 20u, 21u, FirstRoadEndLocation, FirstSettlementLocation, 13u, 23u, 24u, 25u, 15u, 14u, 13u };
      var result2 = new List<UInt32> { FirstRoadEndLocation, 10u, 9u, 19u, 20u, 21u, FirstRoadEndLocation, FirstSettlementLocation, 13u, 14u, 15u, 25u, 24u, 23u, 13u };
      var result3 = new List<UInt32> { FirstRoadEndLocation, 21u, 20u, 19u, 9u, 10u, FirstRoadEndLocation, FirstSettlementLocation, 13u, 23u, 24u, 25u, 15u, 14u, 13u };
      var result4 = new List<UInt32> { FirstRoadEndLocation, 21u, 20u, 19u, 9u, 10u, FirstRoadEndLocation, FirstSettlementLocation, 13u, 14u, 15u, 25u, 24u, 23u, 13u };
      var result5 = new List<UInt32>(result1);
      var result6 = new List<UInt32>(result2);
      var result7 = new List<UInt32>(result3);
      var result8 = new List<UInt32>(result4);
      result5.Reverse();
      result6.Reverse();
      result7.Reverse();
      result8.Reverse();

      this.RoadShouldBeSameAsOneOf(road, result1, result2, result3, result4, result5, result6, result7, result8);
    }

    /// <summary>
    /// Settlement on longest fork.
    /// </summary>
    [Test]
    [Category("GameBoardData.TryGetLongestRoadDetails")]
    public void TryGetLongestRoadDetails_StartingSettlementOnLongestRoadWithFork_ReturnsLongestRoadDetails()
    {
      // Arrange
      var gameBoard = new GameBoardData(BoardSizes.Standard);

      var playerId = Guid.NewGuid();
      gameBoard.PlaceStartingInfrastructure(playerId, FirstSettlementLocation, FirstRoadEndLocation);
      gameBoard.PlaceStartingInfrastructure(playerId, 0, 1);

      gameBoard.PlaceRoadSegment(playerId, FirstRoadEndLocation, 10);
      gameBoard.PlaceRoadSegment(playerId, 10, 2);
      gameBoard.PlaceRoadSegment(playerId, 10, 9);
      gameBoard.PlaceRoadSegment(playerId, 9, 8);

      gameBoard.PlaceRoadSegment(playerId, FirstSettlementLocation, 4);

      // Act
      UInt32[] road;
      Guid longestRoadPlayerId;
      var result = gameBoard.TryGetLongestRoadDetails(out longestRoadPlayerId, out road);

      // Assert
      result.ShouldBeTrue();
      longestRoadPlayerId.ShouldBe(playerId);

      var result1 = new List<UInt32> { 4u, FirstSettlementLocation, FirstRoadEndLocation, 10u, 9u, 8u };
      var result2 = new List<UInt32>(result1);
      result2.Reverse();
      this.RoadShouldBeSameAsOneOf(road, result1, result2);
    }

    /// <summary>
    /// Road segments form a loop with a short branch. Settlement is on intersection between loop and branch.
    /// </summary>
    [Test]
    [Category("GameBoardData.TryGetLongestRoadDetails")]
    public void TryGetLongestRoadDetails_SettlementOnLoopIntersectionWithShortBranch_ReturnsLongestRoadDetails()
    {
      // Arrange
      var gameBoard = new GameBoardData(BoardSizes.Standard);

      var playerId = Guid.NewGuid();
      gameBoard.PlaceStartingInfrastructure(playerId, FirstSettlementLocation, FirstRoadEndLocation);
      gameBoard.PlaceStartingInfrastructure(playerId, SecondSettlementLocation, SecondRoadEndLocation);

      gameBoard.PlaceRoadSegment(playerId, FirstSettlementLocation, 13);
      gameBoard.PlaceRoadSegment(playerId, FirstRoadEndLocation, 21);
      gameBoard.PlaceRoadSegment(playerId, 21, 22);
      gameBoard.PlaceRoadSegment(playerId, 22, 23);
      gameBoard.PlaceRoadSegment(playerId, 13, 23);

      gameBoard.PlaceRoadSegment(playerId, FirstSettlementLocation, 4);
      gameBoard.PlaceRoadSegment(playerId, 4, 3);
      gameBoard.PlaceRoadSegment(playerId, 3, 2);

      // Act
      UInt32[] road;
      Guid longestRoadPlayerId;
      var result = gameBoard.TryGetLongestRoadDetails(out longestRoadPlayerId, out road);

      // Assert
      result.ShouldBeTrue();
      longestRoadPlayerId.ShouldBe(playerId);

      var result1 = new List<UInt32> { FirstSettlementLocation, FirstRoadEndLocation, 21, 22, 23, 13, FirstSettlementLocation, 4, 3, 2 };
      var result2 = new List<UInt32>(result1);
      result2.Reverse();

      this.RoadShouldBeSameAsOneOf(road, result1, result2);
    }

    /// <summary>
    /// Road segments form a loop with a short branch. Settlement is on edge of loop.
    /// </summary>
    [Test]
    [Category("GameBoardData.TryGetLongestRoadDetails")]
    public void TryGetLongestRoadDetails_SettlementOnLoopWithShortBranch_ReturnsLongestRoadDetails()
    {
      // Arrange
      var gameBoard = new GameBoardData(BoardSizes.Standard);

      var playerId = Guid.NewGuid();
      gameBoard.PlaceStartingInfrastructure(playerId, FirstSettlementLocation, 4);
      gameBoard.PlaceStartingInfrastructure(playerId, 0, 1);

      gameBoard.PlaceRoadSegment(playerId, FirstSettlementLocation, 13);
      gameBoard.PlaceRoadSegment(playerId, 13, 14);
      gameBoard.PlaceRoadSegment(playerId, 6, 14);
      gameBoard.PlaceRoadSegment(playerId, 5, 6);
      gameBoard.PlaceRoadSegment(playerId, 5, 4);

      gameBoard.PlaceRoadSegment(playerId, 14, 15);

      // Act
      UInt32[] road;
      Guid longestRoadPlayerId;
      var result = gameBoard.TryGetLongestRoadDetails(out longestRoadPlayerId, out road);

      // Assert
      result.ShouldBeTrue();
      longestRoadPlayerId.ShouldBe(playerId);

      var result1 = new List<UInt32> { 15, 14, 6, 5, 4, FirstSettlementLocation, 13, 14 };
      var result2 = new List<UInt32> { 15, 14, 13, FirstSettlementLocation, 4, 5, 6, 14 };
      var result3 = new List<UInt32>(result1);
      result3.Reverse();
      var result4 = new List<UInt32>(result2);
      result4.Reverse();

      this.RoadShouldBeSameAsOneOf(road, result1, result2, result3, result4);
    }

    /// <summary>
    /// Road segments form three single hex loops with settlement on the intersection. Uses test cases to build the road segments
    /// in different order.
    /// </summary>
    [Test]
    [Category("GameBoardData.TryGetLongestRoadDetails")]
    [TestCase(new UInt32[] { FirstSettlementLocation, FirstRoadEndLocation, FirstRoadEndLocation, 10, FirstRoadEndLocation, 21, FirstSettlementLocation, 13, 13, 23, 13, 14, FirstSettlementLocation, 4, 4, 3, 4, 5, 21, 22, 22, 23, 14, 6, 6, 5, 2, 3, 2, 10 })]
    [TestCase(new UInt32[] { FirstSettlementLocation, 13, 13, 14, 23, 13, FirstSettlementLocation, 4, 3, 4, 4, 5, FirstSettlementLocation, FirstRoadEndLocation, FirstRoadEndLocation, 10, FirstRoadEndLocation, 21, 14, 6, 6, 5, 3, 2, 2, 10, 22, 21, 23, 22 })]
    [TestCase(new UInt32[] { FirstSettlementLocation, 4, 4, 3, 4, 5, FirstSettlementLocation, FirstRoadEndLocation, FirstRoadEndLocation, 10, 21, FirstRoadEndLocation, FirstSettlementLocation, 13, 23, 13, 13, 14, 3, 2, 10, 2, 22, 21, 22, 23, 6, 14, 6, 5 })]
    public void TryGetLongestRoadDetails_SettlementOnIntersectionOfThreeLoops_ReturnsLongestRoadDetails(UInt32[] locations)
    {
      // Arrange
      var gameBoard = new GameBoardData(BoardSizes.Standard);

      var playerId = Guid.NewGuid();
      gameBoard.PlaceStartingInfrastructure(playerId, SecondSettlementLocation, SecondRoadEndLocation);

      gameBoard.PlaceStartingInfrastructure(playerId, locations[0], locations[1]);
      for (var index = 2; index < locations.Length; index += 2)
      {
        gameBoard.PlaceRoadSegment(playerId, locations[index], locations[index + 1]);
      }

      // Act
      UInt32[] road;
      Guid longestRoadPlayerId;
      var result = gameBoard.TryGetLongestRoadDetails(out longestRoadPlayerId, out road);

      // Assert
      result.ShouldBeTrue();
      longestRoadPlayerId.ShouldBe(playerId);

      var result1 = new List<UInt32> { 4, 12, 11, 10, 2, 3, 4, 5, 6, 14, 13, 23, 22, 21, 11 };
      var result2 = new List<UInt32>(result1);
      result2.Reverse();
      var result3 = new List<UInt32> { 4, 12, 11, 21, 22, 23, 13, 14, 6, 5, 4, 3, 2, 10, 11 };
      var result4 = new List<UInt32>(result3);
      result4.Reverse();
      var result5 = new List<UInt32> { 4, 12, 13, 23, 22, 21, 11, 10, 2, 3, 4, 5, 6, 14, 13 };
      var result6 = new List<UInt32>(result5);
      result6.Reverse();
      var result7 = new List<UInt32> { 4, 12, 13, 14, 6, 5, 4, 3, 2, 10, 11, 21, 22, 23, 13 };
      var result8 = new List<UInt32>(result7);
      result8.Reverse();
      var result9 = new List<UInt32> { 13, 12, 11, 10, 2, 3, 4, 5, 6, 14, 13, 23, 22, 21, 11 };
      var result10 = new List<UInt32>(result9);
      result10.Reverse();
      var result11 = new List<UInt32> { 13, 12, 11, 21, 22, 23, 13, 14, 6, 5, 4, 3, 2, 10, 11 };
      var result12 = new List<UInt32>(result11);
      result12.Reverse();
      var result13 = new List<UInt32> { 13, 12, 4, 3, 2, 10, 11, 21, 22, 23, 13, 14, 6, 5, 4 };
      var result14 = new List<UInt32>(result13);
      result14.Reverse();
      var result15 = new List<UInt32> { 13, 12, 4, 5, 6, 14, 13, 23, 22, 21, 11, 10, 2, 3, 4 };
      var result16 = new List<UInt32>(result15);
      result16.Reverse();
      var result17 = new List<UInt32> { 11, 12, 13, 23, 22, 21, 11, 10, 2, 3, 4, 5, 6, 14, 13 };
      var result18 = new List<UInt32>(result17);
      result18.Reverse();
      var result19 = new List<UInt32> { 11, 12, 13, 14, 6, 5, 4, 3, 2, 10, 11, 21, 22, 23, 13 };
      var result20 = new List<UInt32>(result19);
      result20.Reverse();
      var result21 = new List<UInt32> { 11, 12, 4, 3, 2, 10, 11, 21, 22, 23, 13, 14, 6, 5, 4 };
      var result22 = new List<UInt32>(result21);
      result22.Reverse();
      var result23 = new List<UInt32> { 11, 12, 4, 5, 6, 14, 13, 23, 22, 21, 11, 10, 2, 3, 4 };
      var result24 = new List<UInt32>(result23);
      result24.Reverse();

      this.RoadShouldBeSameAsOneOf(road, result1, result2, result3, result4, result5, result6,
        result7, result8, result9, result10, result11, result12, result13, result14,
        result15, result16, result17, result18, result19, result20, result21, result22,
        result23, result24);
    }

    /// <summary>
    /// Road segments form three single hex loops with settlement on the intersection. Uses test cases to build the road segments
    /// in different order.
    /// </summary>
    [Test]
    [Category("GameBoardData.TryGetLongestRoadDetails")]
    [TestCase(new UInt32[] { 21, 20, 20, 19, 19, 18, 18, 29, 29, 28, 20, 31, 31, 30, 30, 29 })]
    [TestCase(new UInt32[] { 21, 20, 20, 31, 31, 30, 30, 29, 29, 28, 20, 19, 19, 18, 18, 29 })]
    public void TryGetLongestRoadDetails_TwoRoutesToSameDestinationAreEqual_ReturnsLongestRoadDetails(UInt32[] locations)
    {
      // Arrange
      var gameBoard = new GameBoardData(BoardSizes.Standard);

      var playerId = Guid.NewGuid();
      gameBoard.PlaceStartingInfrastructure(playerId, 0, 1);

      gameBoard.PlaceStartingInfrastructure(playerId, locations[0], locations[1]);
      for (var index = 2; index < locations.Length; index += 2)
      {
        gameBoard.PlaceRoadSegment(playerId, locations[index], locations[index + 1]);
      }

      // Act
      UInt32[] road;
      Guid longestRoadPlayerId;
      var result = gameBoard.TryGetLongestRoadDetails(out longestRoadPlayerId, out road);

      // Assert
      result.ShouldBeTrue();
      longestRoadPlayerId.ShouldBe(playerId);

      var result1 = new List<UInt32> { 21, 20, 19, 18, 29, 30, 31, 20 };
      var result2 = new List<UInt32>(result1);
      result2.Reverse();
      var result3 = new List<UInt32> { 21, 20, 31, 30, 29, 18, 19, 20 };
      var result4 = new List<UInt32>(result3);
      result4.Reverse();

      this.RoadShouldBeSameAsOneOf(road, result1, result2, result3, result4);
    }

    /// <summary>
    /// Road segments form three single hex loops with settlement on the intersection. Uses test cases to build the road segments
    /// in different order.
    /// </summary>
    [Test]
    [Category("GameBoardData.TryGetLongestRoadDetails")]
    [TestCase(new UInt32[] { 21, 20, 20, 19, 19, 9, 9, 8, 8, 7, 7, 17, 17, 18, 18, 29, 29, 28, 20, 31, 31, 30, 30, 29 })]
    [TestCase(new UInt32[] { 21, 20, 20, 31, 31, 30, 30, 29, 29, 28, 20, 19, 19, 9, 9, 8, 8, 7, 7, 17, 17, 18, 18, 29 })]
    public void TryGetLongestRoadDetails_TwoRoutesToSameDestinationAreDifferent_ReturnsLongestRoadDetails(UInt32[] locations)
    {
      // Arrange
      var gameBoard = new GameBoardData(BoardSizes.Standard);

      var playerId = Guid.NewGuid();
      gameBoard.PlaceStartingInfrastructure(playerId, 0, 1);

      gameBoard.PlaceStartingInfrastructure(playerId, locations[0], locations[1]);
      for (var index = 2; index < locations.Length; index += 2)
      {
        gameBoard.PlaceRoadSegment(playerId, locations[index], locations[index + 1]);
      }

      // Act
      UInt32[] road;
      Guid longestRoadPlayerId;
      var result = gameBoard.TryGetLongestRoadDetails(out longestRoadPlayerId, out road);

      // Assert
      result.ShouldBeTrue();
      longestRoadPlayerId.ShouldBe(playerId);

      var result1 = new List<UInt32> { 21, 20, 19, 9, 8, 7, 17, 18, 29, 30, 31, 20 };
      var result2 = new List<UInt32>(result1);
      result2.Reverse();
      var result3 = new List<UInt32> { 21, 20, 31, 30, 29, 18, 17, 7, 8, 9, 19, 20 };
      var result4 = new List<UInt32>(result3);
      result4.Reverse();

      this.RoadShouldBeSameAsOneOf(road, result1, result2, result3, result4);
    }

    private void BuidRoadBranch(GameBoardData gameBoard, Guid playerId, UInt32[] branch)
    {
      for (var index = 0; index < branch.Length; index += 2)
      {
        gameBoard.PlaceRoadSegment(playerId, branch[index], branch[index + 1]);
      }
    }

    private void RoadShouldBeSameAsOneOf(UInt32[] actualRoad, params List<UInt32>[] possibleRoads)
    {
      var result = false;
      foreach (var possibleRoad in possibleRoads)
      {
        if (actualRoad.Length != possibleRoad.Count)
        {
          continue;
        }

        var index = 0;
        for (; index < actualRoad.Length; index++)
        {
          if (actualRoad[index] != possibleRoad[index])
          {
            break;
          }
        }

        if (index < actualRoad.Length)
        {
          continue;
        }

        result = true;
        break;
      }

      if (!result)
      {
        var actualRoadDetails = "(";
        foreach (var location in actualRoad)
        {
          actualRoadDetails += location + ", ";
        }

        actualRoadDetails = actualRoadDetails.Substring(0, actualRoadDetails.Length - 2) + ")";

        var possibleRoadDetails = "";
        foreach (var possibleRoad in possibleRoads)
        {
          possibleRoadDetails += "(";
          foreach (var location in possibleRoad)
          {
            possibleRoadDetails += location + ", ";
          }

          possibleRoadDetails = possibleRoadDetails.Substring(0, possibleRoadDetails.Length - 2) + ")\n";
        }

        throw new ShouldAssertException(String.Format("Actual road\n{0}\ndoes not match any of the possible roads\n{1}", actualRoadDetails, possibleRoadDetails));
      }
    }
    #endregion
  }
}
