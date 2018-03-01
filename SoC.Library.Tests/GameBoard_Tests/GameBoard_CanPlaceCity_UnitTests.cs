
namespace Jabberwocky.SoC.Library.UnitTests.GameBoard_Tests
{
  using System;
  using GameBoards;
  using NUnit.Framework;
  using Shouldly;

  [TestFixture]
  [Category("All")]
  [Category("GameBoard")]
  [Category("GameBoard.CanPlaceCity")]
  public class GameBoard_CanPlaceCity_UnitTests : GameBoardTestBase
  {
    #region Methods
    [Test]
    public void CanPlaceCity_EmptyBoard_ReturnsStartingInfrastructureNotPresentWhenPlacingCity()
    {
      // Arrange
      var playerId = Guid.NewGuid();
      var gameBoardData = new GameBoard(BoardSizes.Standard);

      // Act
      var result = gameBoardData.CanPlaceCity(playerId, FirstPlayerSettlementLocation);

      // Assert
      result.Status.ShouldBe(GameBoard.VerificationStatus.StartingInfrastructureNotPresentWhenPlacingCity);
      result.LocationIndex.ShouldBe(0u);
      result.PlayerId.ShouldBe(Guid.Empty);
    }

    [Test]
    public void CanPlaceCity_OnlyPlacedFirstStartingInfrastructure_ReturnsStartingInfrastructureNotCompleteWhenPlacingCity()
    {
      // Arrange
      var playerId = Guid.NewGuid();
      var gameBoardData = new GameBoard(BoardSizes.Standard);
      gameBoardData.PlaceStartingInfrastructure(playerId, FirstPlayerSettlementLocation, FirstPlayerRoadEndLocation);

      // Act
      var result = gameBoardData.CanPlaceCity(playerId, FirstPlayerSettlementLocation);

      // Assert
      result.Status.ShouldBe(GameBoard.VerificationStatus.StartingInfrastructureNotCompleteWhenPlacingCity);
      result.LocationIndex.ShouldBe(0u);
      result.PlayerId.ShouldBe(Guid.Empty);
    }

    [Test]
    public void CanPlaceCity_TryPlacingOnExistingCity_ReturnsLocationIsOccupiedStatus()
    {
      // Arrange
      var playerId = Guid.NewGuid();
      var gameBoardData = new GameBoard(BoardSizes.Standard);
      gameBoardData.PlaceStartingInfrastructure(playerId, FirstPlayerSettlementLocation, FirstPlayerRoadEndLocation);
      gameBoardData.PlaceStartingInfrastructure(playerId, SecondPlayerSettlementLocation, SecondPlayerRoadEndLocation);
      gameBoardData.PlaceCity(playerId, FirstPlayerSettlementLocation);

      // Act
      var result = gameBoardData.CanPlaceCity(playerId, FirstPlayerSettlementLocation);

      // Assert
      result.Status.ShouldBe(GameBoard.VerificationStatus.LocationIsAlreadyCity);
      result.LocationIndex.ShouldBe(FirstPlayerSettlementLocation);
      result.PlayerId.ShouldBe(playerId);
    }

    [Test]
    public void CanPlaceCity_TryPlacingOnInvalidLocation_ReturnsLocationIsInvalidStatus()
    {
      // Arrange
      var playerId = Guid.NewGuid();
      var gameBoardData = new GameBoard(BoardSizes.Standard);
      gameBoardData.PlaceStartingInfrastructure(playerId, FirstPlayerSettlementLocation, FirstPlayerRoadEndLocation);
      gameBoardData.PlaceStartingInfrastructure(playerId, SecondPlayerSettlementLocation, SecondPlayerRoadEndLocation);

      // Act
      var result = gameBoardData.CanPlaceCity(playerId, 100);

      // Assert
      result.Status.ShouldBe(GameBoard.VerificationStatus.LocationForCityIsInvalid);
      result.LocationIndex.ShouldBe(0u);
      result.PlayerId.ShouldBe(Guid.Empty);
    }

    [Test]
    public void CanPlaceCity_TryPlacingOnEmptyLocation_ReturnsLocationIsNotSettledStatus()
    {
      // Arrange
      var playerId = Guid.NewGuid();
      var gameBoardData = new GameBoard(BoardSizes.Standard);
      gameBoardData.PlaceStartingInfrastructure(playerId, FirstPlayerSettlementLocation, FirstPlayerRoadEndLocation);
      gameBoardData.PlaceStartingInfrastructure(playerId, SecondPlayerSettlementLocation, SecondPlayerRoadEndLocation);

      // Act
      var result = gameBoardData.CanPlaceCity(playerId, 0);

      // Assert
      result.Status.ShouldBe(GameBoard.VerificationStatus.LocationIsNotSettled);
      result.LocationIndex.ShouldBe(0u);
      result.PlayerId.ShouldBe(Guid.Empty);
    }

    [Test]
    public void CanPlaceCity_TryPlacingOnOpponentSettlement_ReturnsLocationIsNotOwnedStatus()
    {
      // Arrange
      var playerId = Guid.NewGuid();
      var gameBoardData = new GameBoard(BoardSizes.Standard);
      gameBoardData.PlaceStartingInfrastructure(playerId, FirstPlayerSettlementLocation, FirstPlayerRoadEndLocation);
      gameBoardData.PlaceStartingInfrastructure(playerId, SecondPlayerSettlementLocation, SecondPlayerRoadEndLocation);

      var opponentId = Guid.NewGuid();
      gameBoardData.PlaceStartingInfrastructure(opponentId, FirstOpponentSettlementLocation, FirstOpponentRoadEndLocation);
      gameBoardData.PlaceStartingInfrastructure(opponentId, SecondOpponentSettlementLocation, SecondOpponentRoadEndLocation);

      // Act
      var result = gameBoardData.CanPlaceCity(playerId, FirstOpponentSettlementLocation);

      // Assert
      result.Status.ShouldBe(GameBoard.VerificationStatus.LocationIsNotOwned);
      result.LocationIndex.ShouldBe(FirstOpponentSettlementLocation);
      result.PlayerId.ShouldBe(opponentId);
    }

    [Test]
    public void CanPlaceCity_TryPlacingOnOpponentCity_ReturnsLocationIsNotOwnedStatus()
    {
      // Arrange
      var playerId = Guid.NewGuid();
      var gameBoardData = new GameBoard(BoardSizes.Standard);
      gameBoardData.PlaceStartingInfrastructure(playerId, FirstPlayerSettlementLocation, FirstPlayerRoadEndLocation);
      gameBoardData.PlaceStartingInfrastructure(playerId, SecondPlayerSettlementLocation, SecondPlayerRoadEndLocation);

      var opponentId = Guid.NewGuid();
      gameBoardData.PlaceStartingInfrastructure(opponentId, FirstOpponentSettlementLocation, FirstOpponentRoadEndLocation);
      gameBoardData.PlaceStartingInfrastructure(opponentId, SecondOpponentSettlementLocation, SecondOpponentRoadEndLocation);
      gameBoardData.PlaceCity(opponentId, FirstOpponentSettlementLocation);

      // Act
      var result = gameBoardData.CanPlaceCity(playerId, FirstOpponentSettlementLocation);

      // Assert
      result.Status.ShouldBe(GameBoard.VerificationStatus.LocationIsNotOwned);
      result.LocationIndex.ShouldBe(FirstOpponentSettlementLocation);
      result.PlayerId.ShouldBe(opponentId);
    }

    [Test]
    public void CanPlaceCity_TryPlacingOnEmptyLocationConnectedViaRoad_ReturnsLocationIsNotSettledStatus()
    {
      // Arrange
      var playerId = Guid.NewGuid();
      var gameBoardData = new GameBoard(BoardSizes.Standard);
      gameBoardData.PlaceStartingInfrastructure(playerId, FirstPlayerSettlementLocation, FirstPlayerRoadEndLocation);
      gameBoardData.PlaceStartingInfrastructure(playerId, SecondPlayerSettlementLocation, SecondPlayerRoadEndLocation);
      gameBoardData.PlaceRoadSegment(playerId, FirstPlayerRoadEndLocation, 10);

      // Act
      var result = gameBoardData.CanPlaceCity(playerId, 10);

      // Assert
      result.Status.ShouldBe(GameBoard.VerificationStatus.LocationIsNotSettled);
      result.LocationIndex.ShouldBe(0u);
      result.PlayerId.ShouldBe(Guid.Empty);
    }

    [Test]
    public void CanPlaceCity_OnPlayerSettlement_ReturnsValidStatus()
    {
      // Arrange
      var playerId = Guid.NewGuid();
      var gameBoardData = new GameBoard(BoardSizes.Standard);
      gameBoardData.PlaceStartingInfrastructure(playerId, FirstPlayerSettlementLocation, FirstPlayerRoadEndLocation);
      gameBoardData.PlaceStartingInfrastructure(playerId, SecondPlayerSettlementLocation, SecondPlayerRoadEndLocation);

      // Act
      var result = gameBoardData.CanPlaceCity(playerId, FirstPlayerSettlementLocation);

      // Assert
      result.Status.ShouldBe(GameBoard.VerificationStatus.Valid);
      result.LocationIndex.ShouldBe(0u);
      result.PlayerId.ShouldBe(Guid.Empty);
    }
    #endregion 
  }
}
