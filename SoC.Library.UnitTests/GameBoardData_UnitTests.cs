
namespace Jabberwocky.SoC.Library.UnitTests
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Text;
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
    public void Load_HexDataOnly_ResourceProvidersLoadedCorrectly()
    {
      // Arrange
      var playerId = Guid.NewGuid();
      var gameBoardData = new GameBoardData(BoardSizes.Standard);

      // Act
      var content = "<board><hexes>" + 
        "<resources>glbglogob gwwwlwlbo</resources>" + 
        "<production>9,8,5,12,11,3,6,10,6,0,4,11,2,4,3,5,9,10,8</production>" + 
        "</hexes></board>";

      var contentBytes = Encoding.UTF8.GetBytes(content);
      using (var memoryStream = new MemoryStream(contentBytes))
      {
        gameBoardData.Load(memoryStream);
      }

      // Assert
      Tuple<ResourceTypes, UInt32>[] hexes = gameBoardData.GetHexInformation();
      hexes.Length.ShouldBe(GameBoardData.StandardBoardHexCount);
      hexes[0].ShouldBe(new Tuple<ResourceTypes, UInt32>(ResourceTypes.Grain, 9));
      hexes[1].ShouldBe(new Tuple<ResourceTypes, UInt32>(ResourceTypes.Lumber, 8));
      hexes[2].ShouldBe(new Tuple<ResourceTypes, UInt32>(ResourceTypes.Brick, 5));
      hexes[3].ShouldBe(new Tuple<ResourceTypes, UInt32>(ResourceTypes.Grain, 12));
      hexes[4].ShouldBe(new Tuple<ResourceTypes, UInt32>(ResourceTypes.Lumber, 11));
      hexes[5].ShouldBe(new Tuple<ResourceTypes, UInt32>(ResourceTypes.Ore, 3));
      hexes[6].ShouldBe(new Tuple<ResourceTypes, UInt32>(ResourceTypes.Grain, 6));
      hexes[7].ShouldBe(new Tuple<ResourceTypes, UInt32>(ResourceTypes.Ore, 10));
      hexes[8].ShouldBe(new Tuple<ResourceTypes, UInt32>(ResourceTypes.Brick, 6));
      hexes[9].ShouldBe(new Tuple<ResourceTypes, UInt32>(ResourceTypes.None, 0));
      hexes[10].ShouldBe(new Tuple<ResourceTypes, UInt32>(ResourceTypes.Grain, 4));
      hexes[11].ShouldBe(new Tuple<ResourceTypes, UInt32>(ResourceTypes.Wool, 11));
      hexes[12].ShouldBe(new Tuple<ResourceTypes, UInt32>(ResourceTypes.Wool, 2));
      hexes[13].ShouldBe(new Tuple<ResourceTypes, UInt32>(ResourceTypes.Wool, 4));
      hexes[14].ShouldBe(new Tuple<ResourceTypes, UInt32>(ResourceTypes.Lumber, 3));
      hexes[15].ShouldBe(new Tuple<ResourceTypes, UInt32>(ResourceTypes.Wool, 5));
      hexes[16].ShouldBe(new Tuple<ResourceTypes, UInt32>(ResourceTypes.Lumber, 9));
      hexes[17].ShouldBe(new Tuple<ResourceTypes, UInt32>(ResourceTypes.Brick, 10));
      hexes[18].ShouldBe(new Tuple<ResourceTypes, UInt32>(ResourceTypes.Ore, 8));
    }

    [Test]
    [Category("All")]
    [Category("GameBoardData")]
    public void Load_HexAndInfrastructureData_SettlementsLoadedCorrectly()
    {
      // Arrange
      var playerId = Guid.NewGuid();
      var settlementLocation = 12u;
      var road = new Road(12u, 4u);
      var gameBoardData = new GameBoardData(BoardSizes.Standard);
      
      // Act
      var content = "<board><hexes>glbglogob gwwwlwlbo</hexes>" +
        "<settlements>" + 
        "<settlement playerid=\"" + playerId + "\" location=\"" + settlementLocation + "\" />" + 
        "</settlements>" +
        "<roads>" +
        "<road playerid=\"" + playerId + "\" start=\"" + road.Location1 + "\" end=\"" + road.Location2 + "\" />" + 
        "</roads>" +
        "</board>";

      var contentBytes = Encoding.UTF8.GetBytes(content);
      using (var memoryStream = new MemoryStream(contentBytes))
      {
        gameBoardData.Load(memoryStream);
      }

      var settlements = gameBoardData.GetSettlementInformation();
      var roads = gameBoardData.GetRoadInformation();

      // Assert
      settlements.Count.ShouldBe(1);
      settlements.ShouldContainKeyAndValue(settlementLocation, playerId);

      roads.Length.ShouldBe(1);
      roads[0].ShouldBe(new Tuple<Road, Guid>(road, playerId));
    }

    [Test]
    [Category("All")]
    [Category("GameBoardData")]
    [TestCase(3u, 8u, 1, 0)]
    [TestCase(20u, 6u, 0, 1)]
    public void Load_HexAndInfrastructureData_GetCorrectResourcesForRolls(UInt32 settlementLocation, UInt32 diceRoll, Int32 brickCount, Int32 oreCount)
    {
      // Arrange
      var playerId = Guid.NewGuid();
      var gameBoardData = new GameBoardData(BoardSizes.Standard);

      // Act
      var content = "<board><hexes>glbglogob gwwwlwlbo</hexes>" +
        "<settlements>" +
        "<settlement playerid=\"" + playerId + "\" location=\"" + settlementLocation + "\" />" +
        "</settlements>" +
        "</board>";

      var contentBytes = Encoding.UTF8.GetBytes(content);
      using (var memoryStream = new MemoryStream(contentBytes))
      {
        gameBoardData.Load(memoryStream);
      }

      // Assert
      var resourcesByPlayer = gameBoardData.GetResourcesForRoll(diceRoll);
      resourcesByPlayer.Count.ShouldBe(1);
      resourcesByPlayer.ShouldContainKeyAndValue(playerId, new ResourceClutch(brickCount, 0, 0, oreCount, 0));
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
      var gameBoard = new GameBoardData(BoardSizes.Standard);
      var playerId = Guid.NewGuid();
      var road = new Road(12u, 4u);
      gameBoard.PlaceRoad(playerId, road);

      // Act
      var roads = gameBoard.GetRoadInformation();

      // Assert
      roads.Length.ShouldBe(1);
      roads[0].ShouldBe(new Tuple<Road, Guid>(road, playerId));
    }
    #endregion 
  }
}
