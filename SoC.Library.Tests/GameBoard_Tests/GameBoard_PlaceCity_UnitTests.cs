
namespace Jabberwocky.SoC.Library.UnitTests.GameBoard_Tests
{
  using System;
  using GameBoards;
  using NUnit.Framework;
  using Shouldly;

  [TestFixture]
  [Category("All")]
  [Category("GameBoard")]
  [Category("GameBoard.PlaceCity")]
  public class GameBoard_PlaceCity_UnitTests : GameBoardTestBase
  {
    #region Methods
    [Test]
    public void PlaceCity_EmptyBoard_ThrowsMeaningfulException()
    {
      // Arrange
      var playerId = Guid.NewGuid();
      var gameBoardData = new GameBoard(BoardSizes.Standard);

      // Act
      Action action = () => { gameBoardData.PlaceCity(playerId, FirstPlayerSettlementLocation); };

      // Assert
      action.ShouldThrow<GameBoard.PlacementException>().Message.ShouldBe("Cannot place city before placing all initial infrastructure.");
    }

    [Test]
    public void PlaceCity_OnlyPlacedFirstStartingInfrastructure_ThrowsMeaningfulException()
    {
      // Arrange
      var playerId = Guid.NewGuid();
      var gameBoardData = new GameBoard(BoardSizes.Standard);
      gameBoardData.PlaceStartingInfrastructure(playerId, FirstPlayerSettlementLocation, FirstPlayerRoadEndLocation);

      // Act
      Action action = () => { gameBoardData.PlaceCity(playerId, FirstPlayerSettlementLocation); };

      // Assert
      action.ShouldThrow<GameBoard.PlacementException>().Message.ShouldBe("Cannot place city before placing all initial infrastructure.");
    }

    [Test]
    public void PlaceCity_TryPlacingOnExistingCity_ThrowsMeaningfulException()
    {
      // Arrange
      var playerId = Guid.NewGuid();
      var gameBoardData = new GameBoard(BoardSizes.Standard);
      gameBoardData.PlaceStartingInfrastructure(playerId, FirstPlayerSettlementLocation, FirstPlayerRoadEndLocation);
      gameBoardData.PlaceStartingInfrastructure(playerId, SecondPlayerSettlementLocation, SecondPlayerRoadEndLocation);
      gameBoardData.PlaceCity(playerId, FirstPlayerSettlementLocation);

      // Act
      Action action = () => { gameBoardData.PlaceCity(playerId, FirstPlayerSettlementLocation); };

      // Assert
      action.ShouldThrow<GameBoard.PlacementException>().Message.ShouldBe("Cannot place city on existing city.");
    }

    [Test]
    public void PlaceCity_TryPlacingOnInvalidLocation_ThrowsMeaningfulException()
    {
      // Arrange
      var playerId = Guid.NewGuid();
      var gameBoardData = new GameBoard(BoardSizes.Standard);
      gameBoardData.PlaceStartingInfrastructure(playerId, FirstPlayerSettlementLocation, FirstPlayerRoadEndLocation);
      gameBoardData.PlaceStartingInfrastructure(playerId, SecondPlayerSettlementLocation, SecondPlayerRoadEndLocation);

      // Act
      Action action = () => { gameBoardData.PlaceCity(playerId, 100); };

      // Assert
      action.ShouldThrow<GameBoard.PlacementException>().Message.ShouldBe("Cannot place city because location is not on board.");
    }

    [Test]
    public void PlaceCity_TryPlacingOnEmptyLocation_ThrowsMeaningfulException()
    {
      // Arrange
      var playerId = Guid.NewGuid();
      var gameBoardData = new GameBoard(BoardSizes.Standard);
      gameBoardData.PlaceStartingInfrastructure(playerId, FirstPlayerSettlementLocation, FirstPlayerRoadEndLocation);
      gameBoardData.PlaceStartingInfrastructure(playerId, SecondPlayerSettlementLocation, SecondPlayerRoadEndLocation);

      // Act
      Action action = () => { gameBoardData.PlaceCity(playerId, 0); };

      // Assert
      action.ShouldThrow<GameBoard.PlacementException>().Message.ShouldBe("Cannot place city because location is not settled.");
    }

    [Test]
    public void PlaceCity_TryPlacingOnOpponentSettlement_ThrowsMeaningfulException()
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
      Action action = () => { gameBoardData.PlaceCity(playerId, FirstOpponentSettlementLocation); };

      // Assert
      action.ShouldThrow<GameBoard.PlacementException>().Message.ShouldBe("Cannot place city because location is settled by an opponent.");
    }

    [Test]
    public void PlaceCity_TryPlacingOnOpponentCity_ThrowsMeaningfulException()
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
      Action action = () => { gameBoardData.PlaceCity(playerId, FirstOpponentSettlementLocation); };

      // Assert
      action.ShouldThrow<GameBoard.PlacementException>().Message.ShouldBe("Cannot place city because location is settled by an opponent.");
    }

    [Test]
    public void PlaceCity_TryPlacingOnEmptyLocationConnectedViaRoad_ThrowsMeaningfulException()
    {
      // Arrange
      var playerId = Guid.NewGuid();
      var gameBoardData = new GameBoard(BoardSizes.Standard);
      gameBoardData.PlaceStartingInfrastructure(playerId, FirstPlayerSettlementLocation, FirstPlayerRoadEndLocation);
      gameBoardData.PlaceStartingInfrastructure(playerId, SecondPlayerSettlementLocation, SecondPlayerRoadEndLocation);
      gameBoardData.PlaceRoadSegment(playerId, FirstPlayerRoadEndLocation, 10);

      // Act
      Action action = () => { gameBoardData.PlaceCity(playerId, 10); };

      // Assert
      action.ShouldThrow<GameBoard.PlacementException>().Message.ShouldBe("Cannot place city because location is not settled.");
    }
    #endregion 
  }
}
