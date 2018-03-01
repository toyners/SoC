
namespace Jabberwocky.SoC.Library.UnitTests.GameBoard_Tests
{
  using System;
  using System.Collections.Generic;
  using GameBoards;
  using LocalGameController_Tests;
  using NUnit.Framework;
  using Shouldly;

  [TestFixture]
  [Category("All")]
  [Category("GameBoard")]
  public class GameBoard_UnitTests : GameBoardTestBase
  {
    #region Methods
    [Test]
    public void GetPathBetweenLocations_StartAndEndAreSame_ReturnsNull()
    {
      var gameBoardData = new GameBoard(BoardSizes.Standard);
      var result = gameBoardData.GetPathBetweenLocations(0, 0);
      result.ShouldBeNull();
    }

    [Test]
    [TestCase(1u, 0u)]
    [TestCase(8u, 48u)]
    public void GetPathBetweenLocations_StartAndEndAreNeighbours_ReturnsOneStep(UInt32 endPoint, UInt32 stepIndex)
    {
      var gameBoardData = new GameBoard(BoardSizes.Standard);
      var result = gameBoardData.GetPathBetweenLocations(0, endPoint);
      result.ShouldBe(new List<UInt32> { endPoint });
    }

    [Test]
    public void GetPathBetweenLocations_StartAndEndAreNeighbours()
    {
      var gameBoardData = new GameBoard(BoardSizes.Standard);
      var result = gameBoardData.GetPathBetweenLocations(0, 10);
      result.ShouldBe(new List<UInt32> { 10, 2, 1 });
    }

    [Test]
    public void GetSettlementsForPlayers_EmptyBoard_ReturnsNull()
    {
      var gameBoardData = new GameBoard(BoardSizes.Standard);
      var settlements = gameBoardData.GetSettlementsForPlayer(Guid.NewGuid());
      settlements.ShouldBeNull();
    }

    [Test]
    public void GetSettlementsForPlayers_PlayerHasNoSettlementsOnBoard_ReturnsNull()
    {
      // Arrange
      var gameBoardData = new GameBoard(BoardSizes.Standard);
      gameBoardData.PlaceStartingInfrastructure(Guid.NewGuid(), FirstPlayerSettlementLocation, FirstPlayerRoadEndLocation);

      // Act
      var settlements = gameBoardData.GetSettlementsForPlayer(Guid.NewGuid());

      // Assert
      settlements.ShouldBeNull();
    }

    [Test]
    [TestCase(12u, 1, 0, 0, 1, 1)]
    [TestCase(45u, 0, 1, 0, 1, 0)]
    [TestCase(53u, 0, 1, 0, 0, 0)]
    [TestCase(20u, 0, 1, 1, 1, 0)]
    public void GetResourcesForLocation_StandardBoard_ReturnsExpectedResources(UInt32 location, Int32 expectedBrickCount, Int32 expectedGrainCount, Int32 expectedLumberCount, Int32 expectedOreCount, Int32 expectedWoolCount)
    {
      var gameBoardData = new GameBoard(BoardSizes.Standard);
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
      var gameBoardData = new GameBoard(BoardSizes.Standard);
      gameBoardData.PlaceStartingInfrastructure(player1_Id, 12, 11);
      gameBoardData.PlaceStartingInfrastructure(player1_Id, 53, 52);
      gameBoardData.PlaceStartingInfrastructure(player2_Id, 43, 42);
      gameBoardData.PlaceStartingInfrastructure(player3_Id, 39, 47);

      var result = gameBoardData.GetResourcesForRoll(roll);

      var expected = new Dictionary<Guid, ResourceCollection[]>();
      expected.Add(player1_Id, new ResourceCollection[] 
      {
        new ResourceCollection(12u, ResourceClutch.OneBrick),
        new ResourceCollection(53u, ResourceClutch.OneGrain)
      });

      expected.Add(player2_Id, new ResourceCollection[] { new ResourceCollection(43u, ResourceClutch.OneGrain) });

      result.ShouldContainExact(expected);
    }

    /// <summary>
    /// Verify that the collected resources are correct when all three hex locations are owned by the same
    /// player.
    /// </summary>
    [Test]
    public void GetResourcesForRoll_AllLocationsOnHexOwnedBySamePlayer_ReturnsCorrectResources()
    {
      var playerId = Guid.NewGuid();

      var roll = 8u;
      var gameBoardData = new GameBoard(BoardSizes.Standard);
      gameBoardData.PlaceStartingInfrastructure(playerId, 2, 1);
      gameBoardData.PlaceStartingInfrastructure(playerId, 11, 12);
      gameBoardData.PlaceRoadSegment(playerId, 12, 4);
      gameBoardData.PlaceSettlement(playerId, 4);

      var result = gameBoardData.GetResourcesForRoll(roll);

      var expected = new Dictionary<Guid, ResourceCollection[]>();
      expected.Add(playerId, new ResourceCollection[]
      {
        new ResourceCollection(4u, ResourceClutch.OneBrick),
        new ResourceCollection(2u, ResourceClutch.OneBrick),
        new ResourceCollection(11u, ResourceClutch.OneBrick)
      });

      result.ShouldContainExact(expected);
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
      var gameBoardData = new GameBoard(BoardSizes.Standard);
      gameBoardData.PlaceStartingInfrastructure(playerId, settlementLocation, roadEndLocation);
      var result = gameBoardData.GetResourcesForRoll(diceRoll);

      ResourceClutch expectedResources = default(ResourceClutch);
      switch (expectedType)
      {
        case ResourceTypes.Brick: expectedResources = ResourceClutch.OneBrick; break;
        case ResourceTypes.Grain: expectedResources = ResourceClutch.OneGrain; break;
        case ResourceTypes.Lumber: expectedResources = ResourceClutch.OneLumber; break;
        case ResourceTypes.Ore: expectedResources = ResourceClutch.OneOre; break;
        case ResourceTypes.Wool: expectedResources = ResourceClutch.OneWool; break;
      }

      result.Count.ShouldBe(1);
      result.ShouldContainKey(playerId);
      var actual = result[playerId];
      actual[0].Location.ShouldBe(settlementLocation);
      actual[0].Resources.ShouldBe(expectedResources);
    }

    [Test]
    public void GetPlayersForLocation_OnePlayerOnHex_ReturnPlayerIds()
    {
      // Arrange
      var playerId = Guid.NewGuid();
      var gameBoardData = new GameBoard(BoardSizes.Standard);
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
      var gameBoardData = new GameBoard(BoardSizes.Standard);
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
      var gameBoardData = new GameBoard(BoardSizes.Standard);
      gameBoardData.PlaceStartingInfrastructure(playerId, 0, 8);
      gameBoardData.PlaceStartingInfrastructure(playerId, SecondPlayerSettlementLocation, SecondPlayerRoadEndLocation);
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
      var gameBoardData = new GameBoard(BoardSizes.Standard);

      // Act
      var results = gameBoardData.GetPlayersForHex(0);

      // Assert
      results.ShouldBeNull();
    }

    [Test]
    public void GetHexInformation_StandardBoard_ReturnsResourceTypeArray()
    {
      // Arrange
      var gameBoard = new GameBoard(BoardSizes.Standard);

      // Act
      var data = gameBoard.GetHexInformation();

      // Assert
      data[0].ShouldBe(new Tuple<ResourceTypes?, UInt32>(null, 0));
      data[1].ShouldBe(new Tuple<ResourceTypes?, UInt32>(ResourceTypes.Brick, 8));
      data[2].ShouldBe(new Tuple<ResourceTypes?, UInt32>(ResourceTypes.Ore, 5));

      data[3].ShouldBe(new Tuple<ResourceTypes?, UInt32>(ResourceTypes.Brick, 4));
      data[4].ShouldBe(new Tuple<ResourceTypes?, UInt32>(ResourceTypes.Lumber, 3));
      data[5].ShouldBe(new Tuple<ResourceTypes?, UInt32>(ResourceTypes.Wool, 10));
      data[6].ShouldBe(new Tuple<ResourceTypes?, UInt32>(ResourceTypes.Grain, 2));

      data[7].ShouldBe(new Tuple<ResourceTypes?, UInt32>(ResourceTypes.Lumber, 11));
      data[8].ShouldBe(new Tuple<ResourceTypes?, UInt32>(ResourceTypes.Ore, 6));
      data[9].ShouldBe(new Tuple<ResourceTypes?, UInt32>(ResourceTypes.Grain, 11));
      data[10].ShouldBe(new Tuple<ResourceTypes?, UInt32>(ResourceTypes.Wool, 9));
      data[11].ShouldBe(new Tuple<ResourceTypes?, UInt32>(ResourceTypes.Lumber, 6));

      data[12].ShouldBe(new Tuple<ResourceTypes?, UInt32>(ResourceTypes.Wool, 12));
      data[13].ShouldBe(new Tuple<ResourceTypes?, UInt32>(ResourceTypes.Brick, 5));
      data[14].ShouldBe(new Tuple<ResourceTypes?, UInt32>(ResourceTypes.Lumber, 4));
      data[15].ShouldBe(new Tuple<ResourceTypes?, UInt32>(ResourceTypes.Ore, 3));

      data[16].ShouldBe(new Tuple<ResourceTypes?, UInt32>(ResourceTypes.Grain, 9));
      data[17].ShouldBe(new Tuple<ResourceTypes?, UInt32>(ResourceTypes.Wool, 10));
      data[18].ShouldBe(new Tuple<ResourceTypes?, UInt32>(ResourceTypes.Grain, 8));
    }

    [Test]
    public void GetSettlementInformation_OneSettlement_ReturnsSettlementDetails()
    {
      // Arrange
      var playerId = Guid.NewGuid();
      var gameBoard = new GameBoard(BoardSizes.Standard);
      var settlementLocation = 12u;
      gameBoard.PlaceStartingInfrastructure(playerId, FirstPlayerSettlementLocation, FirstPlayerRoadEndLocation);

      // Act
      var settlements = gameBoard.GetSettlementInformation();

      // Assert
      settlements.Count.ShouldBe(1);
      settlements.ShouldContainKeyAndValue(FirstPlayerSettlementLocation, playerId);
    }

    [Test]
    public void GetRoadInformation_OneRoad_ReturnsRoadDetails()
    {
      // Arrange
      var playerId = Guid.NewGuid();
      var gameBoard = new GameBoard(BoardSizes.Standard);
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
      var gameBoard = new GameBoard(BoardSizes.Standard);

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
      var gameBoard = new GameBoard(BoardSizes.Standard);

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
      var gameBoard = new GameBoard(BoardSizes.Standard);

      // Act
      var productionValues = gameBoard.GetProductionValuesForLocation(0u);

      // Assert
      productionValues.Length.ShouldBe(0);
    }
    #endregion
  }
}
