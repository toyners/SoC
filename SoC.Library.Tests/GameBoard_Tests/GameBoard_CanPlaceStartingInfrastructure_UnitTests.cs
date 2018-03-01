
namespace Jabberwocky.SoC.Library.UnitTests.GameBoard_Tests
{
  using System;
  using GameBoards;
  using NUnit.Framework;
  using Shouldly;

  [TestFixture]
  [Category("All")]
  [Category("GameBoard")]
  [Category("GameBoard.CanPlaceStartingInfrastructure")]
  public class GameBoard_CanPlaceStartingInfrastructure_UnitTests : GameBoardTestBase
  {
    #region Methods
    [Test]
    public void CanPlaceStartingInfrastructure_EmptyBoard_ReturnsValid()
    {
      // Arrange
      var playerId = Guid.NewGuid();
      var locationIndex = 20u;
      var roadEndIndex = 21u;
      var gameBoardData = new GameBoard(BoardSizes.Standard);

      // Act
      var result = gameBoardData.CanPlaceStartingInfrastructure(playerId, locationIndex, roadEndIndex);

      // Assert
      result.Status.ShouldBe(GameBoard.VerificationStatus.Valid);
      result.LocationIndex.ShouldBe(0u);
      result.PlayerId.ShouldBe(Guid.Empty);
    }

    [Test]
    public void CanPlaceStartingInfrastructure_TryPlacingOnSettledLocation_ReturnsLocationIsOccupiedStatus()
    {
      var gameBoardData = new GameBoard(BoardSizes.Standard);
      var firstPlayerId = Guid.NewGuid();
      var secondPlayerId = Guid.NewGuid();
      gameBoardData.PlaceStartingInfrastructure(firstPlayerId, FirstPlayerSettlementLocation, FirstPlayerRoadEndLocation);

      var result = gameBoardData.CanPlaceStartingInfrastructure(secondPlayerId, FirstPlayerSettlementLocation, 4);
      result.Status.ShouldBe(GameBoard.VerificationStatus.LocationIsOccupied);
      result.LocationIndex.ShouldBe(FirstPlayerSettlementLocation);
      result.PlayerId.ShouldBe(firstPlayerId);
    }

    [Test]
    public void CanPlaceStartingInfrastructure_TryPlacingOnInvalidLocation_ReturnsLocationIsInvalidStatus()
    {
      // Arrange
      var playerId = Guid.NewGuid();
      var gameBoardData = new GameBoard(BoardSizes.Standard);

      // Act
      var result = gameBoardData.CanPlaceStartingInfrastructure(playerId, 100, 101);

      // Assert
      result.Status.ShouldBe(GameBoard.VerificationStatus.LocationForSettlementIsInvalid);
      result.LocationIndex.ShouldBe(0u);
      result.PlayerId.ShouldBe(Guid.Empty);
    }

    [Test]
    public void CanPlaceStartingInfrastructure_OnlyPlacedFirstStartingInfrastructure_ReturnsValid()
    {
      var playerId = Guid.NewGuid();
      var locationOneIndex = 20u;
      var roadOneEndIndex = 21u;

      var gameBoardData = new GameBoard(BoardSizes.Standard);
      gameBoardData.PlaceStartingInfrastructure(playerId, locationOneIndex, roadOneEndIndex);

      var locationTwoIndex = 0u;
      var roadTwoEndIndex = 1u;
      var result = gameBoardData.CanPlaceStartingInfrastructure(playerId, locationTwoIndex, roadTwoEndIndex);

      result.Status.ShouldBe(GameBoard.VerificationStatus.Valid);
    }

    [Test]
    public void CanPlaceStartingInfrastructure_PlayerAlreadyPlacedAllStartingInfrastructure_ReturnsStartingInfrastructureAlreadyPresent()
    {
      // Arrange
      var playerId = Guid.NewGuid();
      var gameBoardData = new GameBoard(BoardSizes.Standard);
      gameBoardData.PlaceStartingInfrastructure(playerId, FirstPlayerSettlementLocation, FirstPlayerRoadEndLocation);
      gameBoardData.PlaceStartingInfrastructure(playerId, SecondPlayerSettlementLocation, SecondPlayerRoadEndLocation);

      var locationThreeIndex = 0u;
      var roadThreeEndIndex = 1u;

      // Act
      var result = gameBoardData.CanPlaceStartingInfrastructure(playerId, locationThreeIndex, roadThreeEndIndex);

      // Assert
      result.Status.ShouldBe(GameBoard.VerificationStatus.StartingInfrastructureAlreadyPresent);
      result.LocationIndex.ShouldBe(0u);
      result.PlayerId.ShouldBe(Guid.Empty);
    }

    [Test]
    [TestCase(20u, 31u, 19u, 18u)]
    [TestCase(20u, 19u, 21u, 22u)]
    [TestCase(20u, 19u, 31u, 30u)]
    public void CanPlaceStartingInfrastructure_TryPlacingNextToSettledLocation_ReturnsTooCloseToSettlement(UInt32 firstSettlementLocation, UInt32 firstEndRoadLocation, UInt32 secondSettlementLocation, UInt32 secondEndRoadLocation)
    {
      // Arrange
      var firstPlayerId = Guid.NewGuid();
      var secondPlayerId = Guid.NewGuid();
      var gameBoardData = new GameBoard(BoardSizes.Standard);
      gameBoardData.PlaceStartingInfrastructure(firstPlayerId, firstSettlementLocation, firstEndRoadLocation);

      // Act
      var result = gameBoardData.CanPlaceStartingInfrastructure(secondPlayerId, secondSettlementLocation, secondEndRoadLocation);

      // Assert
      result.Status.ShouldBe(GameBoard.VerificationStatus.TooCloseToSettlement);
      result.LocationIndex.ShouldBe(firstSettlementLocation);
      result.PlayerId.ShouldBe(firstPlayerId);
    }

    [Test]
    public void CanPlaceStartingInfrastructure_NoDirectConnectionBetweenSettlementAndRoadEnd_ReturnsNoDirectConnection()
    {
      // Arrange
      var playerId = Guid.NewGuid();
      var gameBoardData = new GameBoard(BoardSizes.Standard);

      // Act
      var result = gameBoardData.CanPlaceStartingInfrastructure(playerId, 20u, 22u);

      // Assert
      result.Status.ShouldBe(GameBoard.VerificationStatus.NoDirectConnection);
      result.LocationIndex.ShouldBe(0u);
      result.PlayerId.ShouldBe(Guid.Empty);
    }

    [Test]
    [TestCase(53u, 54u, GameBoard.VerificationStatus.RoadIsOffBoard)] // Hanging over the edge 
    [TestCase(54u, 53u, GameBoard.VerificationStatus.LocationForSettlementIsInvalid)] // Hanging over the edge
    [TestCase(100u, 101u, GameBoard.VerificationStatus.LocationForSettlementIsInvalid)]
    public void CanPlaceStartingInfrastructure_RoadOffBoard_ReturnsRoadIsOffBoard(UInt32 settlementLocation, UInt32 roadEndLocation, GameBoard.VerificationStatus expectedStatus)
    {
      // Arrange
      var gameBoardData = new GameBoard(BoardSizes.Standard);

      // Act
      var result = gameBoardData.CanPlaceStartingInfrastructure(Guid.NewGuid(), settlementLocation, roadEndLocation);

      // Assert
      result.Status.ShouldBe(expectedStatus);
      result.LocationIndex.ShouldBe(0u);
      result.PlayerId.ShouldBe(Guid.Empty);
    }
    #endregion 
  }
}
