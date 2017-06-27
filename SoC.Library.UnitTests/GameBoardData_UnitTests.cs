
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
    [Category("GameBoardData")]
    public void CanPlaceRoad_EmptyBoard_ReturnsNotConnected()
    {
      var gameBoardData = new GameBoardData(BoardSizes.Standard);
      var result = gameBoardData.CanPlaceRoad(Guid.NewGuid(), new Road(0u, 1u));
      result.Status.ShouldBe(GameBoardData.VerificationResults.NotConnectedToExisting);
    }

    [Test]
    [Category("GameBoardData")]
    public void CanPlaceRoad_RoadNotConnectedToPlacedSettlement_ReturnsNotConnected()
    {
      var gameBoardData = new GameBoardData(BoardSizes.Standard);
      gameBoardData.PlaceSettlement(Guid.NewGuid(), 0);

      var result = gameBoardData.CanPlaceRoad(Guid.NewGuid(), new Road(1u, 2u));
      result.Status.ShouldBe(GameBoardData.VerificationResults.NotConnectedToExisting);
    }

    [Test]
    [Category("GameBoardData")]
    public void CanPlaceRoad_RoadWouldConnectToAnotherPlayersSettlement_ReturnsRoadConnectsToAnotherPlayer()
    {
      var gameBoardData = new GameBoardData(BoardSizes.Standard);
      var playerOneId = Guid.NewGuid();
      gameBoardData.PlaceSettlement(playerOneId, 0);
      gameBoardData.PlaceRoad(playerOneId, new Road(0, 9));

      var playerTwoId = Guid.NewGuid();
      gameBoardData.PlaceSettlement(playerTwoId, 8);

      var result = gameBoardData.CanPlaceRoad(playerOneId, new Road(9, 8));
      result.Status.ShouldBe(GameBoardData.VerificationResults.RoadConnectsToAnotherPlayer);
    }

    [Test]
    [Category("GameBoardData")]
    public void CanPlaceRoad_ConnectedToSettlement_ReturnsValid()
    {
      var gameBoardData = new GameBoardData(BoardSizes.Standard);
      var playerId = Guid.NewGuid();
      gameBoardData.PlaceSettlement(playerId, 0u);
      var result = gameBoardData.CanPlaceRoad(playerId, new Road(0u, 1u));
      result.Status.ShouldBe(GameBoardData.VerificationResults.Valid);
    }

    [Test]
    [Category("GameBoardData")]
    public void CanPlaceRoad_OverEdgeOfBoard_ReturnsRoadIsInvalid()
    {
      var gameBoardData = new GameBoardData(BoardSizes.Standard);
      var result = gameBoardData.CanPlaceRoad(Guid.NewGuid(), new Road(52, 53));
      result.Status.ShouldBe(GameBoardData.VerificationResults.RoadIsInvalid);
    }

    [Test]
    [Category("GameBoardData")]
    public void CanPlaceRoad_ConnectedToRoad_ReturnsValid()
    {
      var gameBoardData = new GameBoardData(BoardSizes.Standard);
      var playerId = Guid.NewGuid();
      gameBoardData.PlaceSettlement(playerId, 0);
      gameBoardData.PlaceRoad(playerId, new Road(0, 9));
      var result = gameBoardData.CanPlaceRoad(playerId, new Road(9, 8));
      result.Status.ShouldBe(GameBoardData.VerificationResults.Valid);
    }

    [Test]
    [Category("GameBoardData")]
    public void CanPlaceSettlement_EmptyBoard_ReturnsValid()
    {
      var gameBoardData = new GameBoardData(BoardSizes.Standard);
      var result = gameBoardData.CanPlaceSettlement(0);
      result.Status.ShouldBe(GameBoardData.VerificationResults.Valid);
      result.LocationIndex.ShouldBe(0u);
      result.PlayerId.ShouldBe(Guid.Empty);
    }

    [Test]
    [Category("GameBoardData")]
    public void CanPlaceSettlement_TryPlacingOnSettledLocation_ReturnsLocationIsOccupiedStatus()
    {
      var gameBoardData = new GameBoardData(BoardSizes.Standard);
      var playerId = Guid.NewGuid();
      gameBoardData.PlaceSettlement(playerId, 1);
      
      var result = gameBoardData.CanPlaceSettlement(1);
      result.Status.ShouldBe(GameBoardData.VerificationResults.LocationIsOccupied);
      result.LocationIndex.ShouldBe(1u);
      result.PlayerId.ShouldBe(playerId);
    }

    [Test]
    [Category("GameBoardData")]
    public void CanPlaceSettlement_TryPlacingOnInvalidLocation_ReturnsLocationIsInvalidStatus()
    {
      var gameBoardData = new GameBoardData(BoardSizes.Standard);
      var result = gameBoardData.CanPlaceSettlement(100);
      result.Status.ShouldBe(GameBoardData.VerificationResults.LocationIsInvalid);
    }

    [Test]
    [Category("GameBoardData")]
    [TestCase(19u)]
    [TestCase(21u)]
    [TestCase(31u)]
    public void CanPlaceSettlement_TryPlacingNextToSettledLocation_ReturnsCorrectVerificationCode(UInt32 newLocation)
    {
      var playerId = Guid.NewGuid();
      var location = 20u;
      var gameBoardData = new GameBoardData(BoardSizes.Standard);
      gameBoardData.PlaceSettlement(playerId, location);
      var result = gameBoardData.CanPlaceSettlement(newLocation);

      result.Status.ShouldBe(GameBoardData.VerificationResults.TooCloseToSettlement);
      result.LocationIndex.ShouldBe(location);
      result.PlayerId.ShouldBe(playerId);
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
    #endregion 
  }
}
