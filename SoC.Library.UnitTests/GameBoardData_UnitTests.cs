
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
      result.Status.ShouldBe(GameBoardData.VerificationStatus.NotConnectedToExisting);
    }

    [Test]
    [Category("GameBoardData")]
    public void CanPlaceRoad_RoadNotConnectedToPlacedSettlement_ReturnsNotConnected()
    {
      var gameBoardData = new GameBoardData(BoardSizes.Standard);
      gameBoardData.PlaceSettlement(Guid.NewGuid(), 0);

      var result = gameBoardData.CanPlaceRoad(Guid.NewGuid(), new Road(1u, 2u));
      result.Status.ShouldBe(GameBoardData.VerificationStatus.NotConnectedToExisting);
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
      result.Status.ShouldBe(GameBoardData.VerificationStatus.RoadConnectsToAnotherPlayer);
    }

    [Test]
    [Category("GameBoardData")]
    public void CanPlaceRoad_ConnectedToSettlement_ReturnsValid()
    {
      var gameBoardData = new GameBoardData(BoardSizes.Standard);
      var playerId = Guid.NewGuid();
      gameBoardData.PlaceSettlement(playerId, 0u);
      var result = gameBoardData.CanPlaceRoad(playerId, new Road(0u, 1u));
      result.Status.ShouldBe(GameBoardData.VerificationStatus.Valid);
    }

    [Test]
    [Category("GameBoardData")]
    [TestCase(53u, 54u)] // Hanging over the edge 
    [TestCase(54u, 53u)] // Hanging over the edge
    [TestCase(100u, 101u)]
    public void CanPlaceRoad_OffBoard_ReturnsRoadIsInvalid(UInt32 start, UInt32 end)
    {
      var gameBoardData = new GameBoardData(BoardSizes.Standard);
      var result = gameBoardData.CanPlaceRoad(Guid.NewGuid(), new Road(start, end));
      result.Status.ShouldBe(GameBoardData.VerificationStatus.RoadIsOffBoard);
    }

    [Test]
    [Category("GameBoardData")]
    [TestCase(43u, 53u)]
    [TestCase(53u, 43u)]
    public void CanPlaceRoad_NoDirectConnection_ReturnsRoadIsInvalid(UInt32 start, UInt32 end)
    {
      var gameBoardData = new GameBoardData(BoardSizes.Standard);
      var result = gameBoardData.CanPlaceRoad(Guid.NewGuid(), new Road(start, end));
      result.Status.ShouldBe(GameBoardData.VerificationStatus.NoDirectConnection);
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
      result.Status.ShouldBe(GameBoardData.VerificationStatus.Valid);
    }

    [Test]
    [Category("GameBoardData")]
    public void CanPlaceRoad_RoadAlreadyBuilt_ReturnsRoadIsOccupied()
    {
      var gameBoardData = new GameBoardData(BoardSizes.Standard);
      var playerId = Guid.NewGuid();
      var road = new Road(0, 1);
      gameBoardData.PlaceStartingInfrastructure(playerId, 0, road);

      var result = gameBoardData.CanPlaceRoad(playerId, road);
      result.Status.ShouldBe(GameBoardData.VerificationStatus.RoadIsOccupied);
    }

    [Test]
    [Category("GameBoardData")]
    public void CanPlaceRoad_JoiningToOtherRoads_ReturnsValid()
    {
      var gameBoardData = new GameBoardData(BoardSizes.Standard);
      var playerId = Guid.NewGuid();
      var road = new Road(11, 21);
      gameBoardData.PlaceStartingInfrastructure(playerId, 12, new Road(12, 11));

      gameBoardData.PlaceStartingInfrastructure(playerId, 20, new Road(20, 21));

      var result = gameBoardData.CanPlaceRoad(playerId, road);
      result.Status.ShouldBe(GameBoardData.VerificationStatus.Valid);
    }

    [Test]
    [Category("GameBoardData")]
    public void CanPlaceSettlement_EmptyBoard_ReturnsValid()
    {
      var gameBoardData = new GameBoardData(BoardSizes.Standard);
      var result = gameBoardData.CanPlaceSettlement(0);
      result.Status.ShouldBe(GameBoardData.VerificationStatus.Valid);
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
      result.Status.ShouldBe(GameBoardData.VerificationStatus.LocationIsOccupied);
      result.LocationIndex.ShouldBe(1u);
      result.PlayerId.ShouldBe(playerId);
    }

    [Test]
    [Category("GameBoardData")]
    public void CanPlaceSettlement_TryPlacingOnInvalidLocation_ReturnsLocationIsInvalidStatus()
    {
      var gameBoardData = new GameBoardData(BoardSizes.Standard);
      var result = gameBoardData.CanPlaceSettlement(100);
      result.Status.ShouldBe(GameBoardData.VerificationStatus.LocationIsInvalid);
      result.LocationIndex.ShouldBe(0u);
      result.PlayerId.ShouldBe(Guid.Empty);
    }

    [Test]
    [Category("GameBoardData")]
    [TestCase(19u)]
    [TestCase(21u)]
    [TestCase(31u)]
    public void CanPlaceSettlement_TryPlacingNextToSettledLocation_ReturnsCorrectVerificationResult(UInt32 newLocation)
    {
      var playerId = Guid.NewGuid();
      var location = 20u;
      var gameBoardData = new GameBoardData(BoardSizes.Standard);
      gameBoardData.PlaceSettlement(playerId, location);
      var result = gameBoardData.CanPlaceSettlement(newLocation);

      result.Status.ShouldBe(GameBoardData.VerificationStatus.TooCloseToSettlement);
      result.LocationIndex.ShouldBe(location);
      result.PlayerId.ShouldBe(playerId);
    }

    [Test]
    [Category("GameBoardData")]
    public void CanPlaceStartingInfrastructure_RoadFailsVerification_ReturnsNotConnectedToExisting()
    {
      var playerId = Guid.NewGuid();
      var location = 20u;
      var gameBoardData = new GameBoardData(BoardSizes.Standard);
      var results = gameBoardData.CanPlaceStartingInfrastructure(playerId, location, new Road(21, 22));

      results.Status.ShouldBe(GameBoardData.VerificationStatus.NotConnectedToExisting);
      results.LocationIndex.ShouldBe(0u);
      results.PlayerId.ShouldBe(Guid.Empty);
    }

    [Test]
    [Category("GameBoardData")]
    public void CanPlaceStartingInfrastructure_RoadFailsVerification_SettlementNotPlaced()
    {
      var playerId = Guid.NewGuid();
      var location = 20u;
      var gameBoardData = new GameBoardData(BoardSizes.Standard);
      gameBoardData.CanPlaceStartingInfrastructure(playerId, location, new Road(21, 22));
      var results = gameBoardData.CanPlaceSettlement(20);

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
    public void PlaceStartingInfrastructure_SettlementAndRoadAreValid_InfrastructurePlaced()
    {
      var playerId = Guid.NewGuid();
      var location = 20u;
      var road = new Road(21, 22);
      var gameBoardData = new GameBoardData(BoardSizes.Standard);
      gameBoardData.PlaceStartingInfrastructure(playerId, location, new Road(21, 22));

      gameBoardData.CanPlaceSettlement(location).Status.ShouldBe(GameBoardData.VerificationStatus.LocationIsOccupied);
      gameBoardData.CanPlaceRoad(playerId, road).Status.ShouldBe(GameBoardData.VerificationStatus.RoadIsOccupied);
    }

    [Test]
    [Category("GameBoardData")]
    [TestCase(12u, 1u, 0u, 0u, 1u, 1u)]
    [TestCase(45u, 0u, 1u, 0u, 1u, 0u)]
    [TestCase(53u, 0u, 1u, 0u, 0u, 0u)]
    [TestCase(20u, 0u, 1u, 1u, 1u, 0u)]
    public void GetResourcesByLocation_StandardBoard_ReturnsExpectedResources(UInt32 location, UInt32 expectedBrickCount, UInt32 expectedGrainCount, UInt32 expectedLumberCount, UInt32 expectedOreCount, UInt32 expectedWoolCount)
    {
      var gameBoardData = new GameBoardData(BoardSizes.Standard);
      var result = gameBoardData.GetResourcesByLocation(location);
      result.BrickCount.ShouldBe(expectedBrickCount);
      result.GrainCount.ShouldBe(expectedGrainCount);
      result.LumberCount.ShouldBe(expectedLumberCount);
      result.OreCount.ShouldBe(expectedOreCount);
      result.WoolCount.ShouldBe(expectedWoolCount);
    }
    #endregion 
  }
}
