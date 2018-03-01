
namespace Jabberwocky.SoC.Library.UnitTests.GameBoard_Tests
{
  using System;
  using GameBoards;
  using NUnit.Framework;
  using Shouldly;

  [TestFixture]
  [Category("All")]
  [Category("GameBoard")]
  [Category("GameBoard.CanPlaceSettlement")]
  public class GameBoard_CanPlaceSettlement_UnitTests : GameBoardTestBase
  {
    #region Methods
    [Test]
    public void CanPlaceSettlement_EmptyBoard_ReturnsStartingInfrastructureNotPresentWhenPlacingSettlement()
    {
      // Arrange
      var playerId = Guid.NewGuid();
      var gameBoardData = new GameBoard(BoardSizes.Standard);

      // Act
      var result = gameBoardData.CanPlaceSettlement(playerId, 0);

      // Assert
      result.Status.ShouldBe(GameBoard.VerificationStatus.StartingInfrastructureNotPresentWhenPlacingSettlement);
      result.LocationIndex.ShouldBe(0u);
      result.PlayerId.ShouldBe(Guid.Empty);
    }

    [Test]
    public void CanPlaceSettlement_OnlyPlacedFirstStartingInfrastructure_ReturnsStartingInfrastructureNotPresentWhenPlacingSettlement()
    {
      // Arrange
      var playerId = Guid.NewGuid();
      var gameBoardData = new GameBoard(BoardSizes.Standard);
      gameBoardData.PlaceStartingInfrastructure(playerId, FirstPlayerSettlementLocation, FirstPlayerRoadEndLocation);

      // Act
      var result = gameBoardData.CanPlaceSettlement(playerId, 0);

      // Assert
      result.Status.ShouldBe(GameBoard.VerificationStatus.StartingInfrastructureNotCompleteWhenPlacingSettlement);
      result.LocationIndex.ShouldBe(0u);
      result.PlayerId.ShouldBe(Guid.Empty);
    }

    [Test]
    public void CanPlaceSettlement_TryPlacingOnSettledLocation_ReturnsLocationIsOccupiedStatus()
    {
      // Arrange
      var playerId = Guid.NewGuid();
      var gameBoardData = new GameBoard(BoardSizes.Standard);
      gameBoardData.PlaceStartingInfrastructure(playerId, FirstPlayerSettlementLocation, FirstPlayerRoadEndLocation);
      gameBoardData.PlaceStartingInfrastructure(playerId, SecondPlayerSettlementLocation, SecondPlayerRoadEndLocation);

      // Act
      var result = gameBoardData.CanPlaceSettlement(playerId, FirstPlayerSettlementLocation);

      // Assert
      result.Status.ShouldBe(GameBoard.VerificationStatus.LocationIsOccupied);
      result.LocationIndex.ShouldBe(FirstPlayerSettlementLocation);
      result.PlayerId.ShouldBe(playerId);
    }

    [Test]
    public void CanPlaceSettlement_TryPlacingOnInvalidLocation_ReturnsLocationIsInvalidStatus()
    {
      // Arrange
      var playerId = Guid.NewGuid();
      var gameBoardData = new GameBoard(BoardSizes.Standard);
      gameBoardData.PlaceStartingInfrastructure(playerId, FirstPlayerSettlementLocation, FirstPlayerRoadEndLocation);
      gameBoardData.PlaceStartingInfrastructure(playerId, SecondPlayerSettlementLocation, SecondPlayerRoadEndLocation);

      // Act
      var result = gameBoardData.CanPlaceSettlement(playerId, 100);

      // Assert
      result.Status.ShouldBe(GameBoard.VerificationStatus.LocationForSettlementIsInvalid);
      result.LocationIndex.ShouldBe(0u);
      result.PlayerId.ShouldBe(Guid.Empty);
    }

    [Test]
    [TestCase(4u)]
    [TestCase(11u)]
    [TestCase(13u)]
    public void CanPlaceSettlement_TryPlacingNextToSettledLocation_ReturnsTooCloseToSettlement(UInt32 newSettlementLocation)
    {
      // Arrange
      var playerId = Guid.NewGuid();
      var gameBoardData = new GameBoard(BoardSizes.Standard);
      gameBoardData.PlaceStartingInfrastructure(playerId, FirstPlayerSettlementLocation, FirstPlayerRoadEndLocation);
      gameBoardData.PlaceStartingInfrastructure(playerId, SecondPlayerSettlementLocation, SecondPlayerRoadEndLocation);

      // Act
      var result = gameBoardData.CanPlaceSettlement(playerId, newSettlementLocation);

      // Assert
      result.Status.ShouldBe(GameBoard.VerificationStatus.TooCloseToSettlement);
      result.LocationIndex.ShouldBe(FirstPlayerSettlementLocation);
      result.PlayerId.ShouldBe(playerId);
    }

    [Test]
    public void CanPlaceSettlement_PlaceOnRoad_ReturnsValid()
    {
      // Arrange
      var playerId = Guid.NewGuid();
      var gameBoardData = new GameBoard(BoardSizes.Standard);
      gameBoardData.PlaceStartingInfrastructure(playerId, FirstPlayerSettlementLocation, FirstPlayerRoadEndLocation);
      gameBoardData.PlaceStartingInfrastructure(playerId, SecondPlayerSettlementLocation, SecondPlayerRoadEndLocation);
      gameBoardData.PlaceRoadSegment(playerId, FirstPlayerRoadEndLocation, 10);

      // Act
      var result = gameBoardData.CanPlaceSettlement(playerId, 10);

      // Assert
      result.Status.ShouldBe(GameBoard.VerificationStatus.Valid);
    }

    [Test]
    public void CanPlaceSettlement_DontPlaceOnRoad_ReturnsNotConnectedToExisting()
    {
      // Arrange
      var playerId = Guid.NewGuid();
      var gameBoardData = new GameBoard(BoardSizes.Standard);
      gameBoardData.PlaceStartingInfrastructure(playerId, FirstPlayerSettlementLocation, FirstPlayerRoadEndLocation);
      gameBoardData.PlaceStartingInfrastructure(playerId, SecondPlayerSettlementLocation, SecondPlayerRoadEndLocation);

      // Act
      var result = gameBoardData.CanPlaceSettlement(playerId, 10);

      // Assert
      result.Status.ShouldBe(GameBoard.VerificationStatus.SettlementNotConnectedToExistingRoad);
    }
    #endregion
  }
}
