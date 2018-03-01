
namespace Jabberwocky.SoC.Library.UnitTests.GameBoard_Tests
{
  using System;
  using System.Collections.Generic;
  using GameBoards;
  using NUnit.Framework;
  using Shouldly;

  [TestFixture]
  [Category("All")]
  [Category("GameBoard")]
  [Category("GameBoard.TryGetLongestRoadDetails")]
  public class GameBoard_TryGetLongestRoadDetails_UnitTests : GameBoardTestBase
  {
    #region Methods
    /// <summary>
    /// No roads on the board so no longest road details can be passed back. Returns false.
    /// </summary>
    [Test]
    
    public void TryGetLongestRoadDetails_NoRoadsOnBoard_ReturnsFalse()
    {
      // Arrange
      UInt32[] road;
      Guid longestRoadPlayerId;
      var gameBoard = new GameBoard(BoardSizes.Standard);

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
    public void TryGetLongestRoadDetails_OnePlayerHasLongestRoad_ReturnsTrue()
    {
      // Arrange
      var gameBoard = new GameBoard(BoardSizes.Standard);

      var playerId = Guid.NewGuid();
      var opponentId = Guid.NewGuid();
      gameBoard.PlaceStartingInfrastructure(playerId, FirstPlayerSettlementLocation, FirstPlayerRoadEndLocation);
      gameBoard.PlaceStartingInfrastructure(playerId, SecondPlayerSettlementLocation, SecondPlayerRoadEndLocation);
      gameBoard.PlaceRoadSegment(playerId, FirstPlayerRoadEndLocation, 10);
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
      var result1 = new List<UInt32> { FirstPlayerSettlementLocation, FirstPlayerRoadEndLocation, 10u, 2u };
      var result2 = new List<UInt32>(result1);
      result2.Reverse();
      this.RoadShouldBeSameAsOneOf(road, result1, result2);
    }

    /// <summary>
    /// Two players have the longest road. Return false
    /// </summary>
    [Test]
    public void TryGetLongestRoadDetails_TwoPlayersHaveTheLongestRoad_ReturnsFalse()
    {
      // Arrange
      var gameBoard = new GameBoard(BoardSizes.Standard);

      var playerId = Guid.NewGuid();
      var opponentId = Guid.NewGuid();
      gameBoard.PlaceStartingInfrastructure(playerId, FirstPlayerSettlementLocation, FirstPlayerRoadEndLocation);
      gameBoard.PlaceStartingInfrastructure(playerId, SecondPlayerSettlementLocation, SecondPlayerRoadEndLocation);
      gameBoard.PlaceRoadSegment(playerId, FirstPlayerRoadEndLocation, 10);
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
    public void TryGetLongestRoadDetails_OnePlayerHasTwoRoadsOfEqualLength_ReturnsLongestRoadDetails()
    {
      // Arrange      
      var gameBoard = new GameBoard(BoardSizes.Standard);

      var playerId = Guid.NewGuid();
      gameBoard.PlaceStartingInfrastructure(playerId, FirstPlayerSettlementLocation, FirstPlayerRoadEndLocation);
      gameBoard.PlaceStartingInfrastructure(playerId, SecondPlayerSettlementLocation, SecondPlayerRoadEndLocation);
      gameBoard.PlaceRoadSegment(playerId, FirstPlayerRoadEndLocation, 10);
      gameBoard.PlaceRoadSegment(playerId, 10, 2);

      gameBoard.PlaceRoadSegment(playerId, SecondPlayerRoadEndLocation, 14);
      gameBoard.PlaceRoadSegment(playerId, 14, 6);

      // Act
      UInt32[] road;
      Guid longestRoadPlayerId;
      var result = gameBoard.TryGetLongestRoadDetails(out longestRoadPlayerId, out road);

      // Assert
      result.ShouldBeTrue();
      longestRoadPlayerId.ShouldBe(playerId);
      var result1 = new List<UInt32> { FirstPlayerSettlementLocation, FirstPlayerRoadEndLocation, 10, 2 };
      var result2 = new List<UInt32>(result1);
      result2.Reverse();
      var result3 = new List<UInt32> { SecondPlayerSettlementLocation, SecondPlayerRoadEndLocation, 14, 6 };
      var result4 = new List<UInt32>(result3);
      result4.Reverse();

      this.RoadShouldBeSameAsOneOf(road, result1, result2, result3, result4);
    }

    /// <summary>
    /// Road placed around hex in a cycle. Road segments have not been placed in a consecutive manner but rather in a
    /// haphazard manner. Longest road details must not count road segments more than once.
    /// </summary>
    [Test]
    public void TryGetLongestRoadDetails_LongestRoadIsCycle_ReturnsLongestRoadDetails()
    {
      // Arrange
      var gameBoard = new GameBoard(BoardSizes.Standard);

      var playerId = Guid.NewGuid();
      gameBoard.PlaceStartingInfrastructure(playerId, FirstPlayerSettlementLocation, FirstPlayerRoadEndLocation);
      gameBoard.PlaceStartingInfrastructure(playerId, SecondPlayerSettlementLocation, SecondPlayerRoadEndLocation);

      gameBoard.PlaceRoadSegment(playerId, FirstPlayerSettlementLocation, 13);
      gameBoard.PlaceRoadSegment(playerId, FirstPlayerRoadEndLocation, 21);
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
      var result1 = new List<UInt32> { FirstPlayerSettlementLocation, FirstPlayerRoadEndLocation, 21u, 22u, 23u, 13u, FirstPlayerSettlementLocation };
      var result2 = new List<UInt32>(result1);
      result2.Reverse();
      this.RoadShouldBeSameAsOneOf(road, result1, result2);
    }

    /// <summary>
    /// Road placed around two hexes in a figure-of-eight. Road segments have not been placed in a consecutive manner but 
    /// rather in a haphazard manner. Longest road details must not count road segments more than once.
    /// </summary>
    [Test]
    public void TryGetLongestRoadDetails_LongestRoadIsFigureOfEight_ReturnsLongestRoadDetails()
    {
      // Arrange
      var gameBoard = new GameBoard(BoardSizes.Standard);

      var playerId = Guid.NewGuid();
      gameBoard.PlaceStartingInfrastructure(playerId, FirstPlayerSettlementLocation, FirstPlayerRoadEndLocation);
      gameBoard.PlaceStartingInfrastructure(playerId, SecondPlayerSettlementLocation, SecondPlayerRoadEndLocation);

      gameBoard.PlaceRoadSegment(playerId, FirstPlayerRoadEndLocation, 21);
      gameBoard.PlaceRoadSegment(playerId, 21, 22);
      gameBoard.PlaceRoadSegment(playerId, FirstPlayerSettlementLocation, 13);
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
      var result1 = new List<UInt32> { 21, 20, 31, 32, 33, 22, 21, 11, 12, 13, 23, 22 };
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
    [TestCase(new UInt32[] { 23, 22, 22, 21, 21, 20, 20, 19, 19, 18, 18, 17, 22, 33, 33, 32, 32, 42, 42, 41 })] // Longest branch first
    [TestCase(new UInt32[] { 23, 22, 22, 33, 33, 32, 32, 42, 42, 41, 22, 21, 21, 20, 20, 19, 19, 18, 18, 17 })] // Shortest branch first
    public void TryGetLongestRoadDetails_LongestRoadContainsFork_ReturnsLongestRoadDetails(UInt32[] locations)
    {
      // Arrange
      var gameBoard = new GameBoard(BoardSizes.Standard);

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
      var gameBoard = new GameBoard(BoardSizes.Standard);

      var playerId = Guid.NewGuid();
      gameBoard.PlaceStartingInfrastructure(playerId, 0, 1);

      gameBoard.PlaceStartingInfrastructure(playerId, 23, 22);

      this.BuildRoadBranch(gameBoard, playerId, firstBranch);
      this.BuildRoadBranch(gameBoard, playerId, secondBranch);
      this.BuildRoadBranch(gameBoard, playerId, lastBranch);

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
    public void TryGetLongestRoadDetails_TwoRoadsAreConnectedWithRoadSegment_ReturnsTrue()
    {
      // Arrange
      var gameBoard = new GameBoard(BoardSizes.Standard);

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
    public void TryGetLongestRoadDetails_OnePlayerHasLongestRoadWithOneEndPoint_ReturnsLongestRoadDetails()
    {
      // Arrange
      var gameBoard = new GameBoard(BoardSizes.Standard);

      var playerId = Guid.NewGuid();
      gameBoard.PlaceStartingInfrastructure(playerId, FirstPlayerSettlementLocation, FirstPlayerRoadEndLocation);
      gameBoard.PlaceStartingInfrastructure(playerId, SecondPlayerSettlementLocation, SecondPlayerRoadEndLocation);
      gameBoard.PlaceRoadSegment(playerId, FirstPlayerRoadEndLocation, 21u);
      gameBoard.PlaceRoadSegment(playerId, 21u, 20u);
      gameBoard.PlaceRoadSegment(playerId, 20u, 19u);
      gameBoard.PlaceRoadSegment(playerId, 19u, 9u);
      gameBoard.PlaceRoadSegment(playerId, 10u, 9u);
      gameBoard.PlaceRoadSegment(playerId, FirstPlayerRoadEndLocation, 10u);

      // Act
      UInt32[] road;
      Guid longestRoadPlayerId;
      var result = gameBoard.TryGetLongestRoadDetails(out longestRoadPlayerId, out road);

      // Assert
      result.ShouldBeTrue();
      longestRoadPlayerId.ShouldBe(playerId);
      var result1 = new List<UInt32> { FirstPlayerSettlementLocation, FirstPlayerRoadEndLocation, 21, 20, 19, 9, 10, FirstPlayerRoadEndLocation };
      var result2 = new List<UInt32>(result1);
      result2.Reverse();
      var result3 = new List<UInt32> { FirstPlayerSettlementLocation, FirstPlayerRoadEndLocation, 10, 9, 19, 20, 21, FirstPlayerRoadEndLocation };
      var result4 = new List<UInt32>(result3);
      result4.Reverse();
      this.RoadShouldBeSameAsOneOf(road, result1, result2, result3, result4);
    }

    /// <summary>
    /// Longest road is two segments long with settlement in the middle. Should still return the road as longest.
    /// </summary>
    [Test]
    public void TryGetLongestRoadDetails_StartingSettlementNotOnEndOfRoad_ReturnsLongestRoadDetails()
    {
      // Arrange
      var gameBoard = new GameBoard(BoardSizes.Standard);

      var playerId = Guid.NewGuid();
      gameBoard.PlaceStartingInfrastructure(playerId, FirstPlayerSettlementLocation, FirstPlayerRoadEndLocation);
      gameBoard.PlaceStartingInfrastructure(playerId, SecondPlayerSettlementLocation, SecondPlayerRoadEndLocation);
      gameBoard.PlaceRoadSegment(playerId, FirstPlayerSettlementLocation, 4);

      // Act
      UInt32[] road;
      Guid longestRoadPlayerId;
      var result = gameBoard.TryGetLongestRoadDetails(out longestRoadPlayerId, out road);

      // Assert
      result.ShouldBeTrue();
      longestRoadPlayerId.ShouldBe(playerId);
      var result1 = new List<UInt32> { FirstPlayerRoadEndLocation, FirstPlayerSettlementLocation, 4u };
      var result2 = new List<UInt32>(result1);
      result2.Reverse();
      this.RoadShouldBeSameAsOneOf(road, result1, result2);
    }

    /// <summary>
    /// Settlment has road leading away in all three directions. Two roads are same size. One road is longer.
    /// </summary>
    [Test]
    public void TryGetLongestRoadDetails_SettlementHasRoadsInAllDirections_ReturnsLongestRoadDetails()
    {
      // Arrange
      var gameBoard = new GameBoard(BoardSizes.Standard);

      var playerId = Guid.NewGuid();
      gameBoard.PlaceStartingInfrastructure(playerId, FirstPlayerSettlementLocation, FirstPlayerRoadEndLocation);
      gameBoard.PlaceStartingInfrastructure(playerId, SecondPlayerSettlementLocation, SecondPlayerRoadEndLocation);
      gameBoard.PlaceRoadSegment(playerId, FirstPlayerSettlementLocation, 4);
      gameBoard.PlaceRoadSegment(playerId, FirstPlayerRoadEndLocation, 10);
      gameBoard.PlaceRoadSegment(playerId, FirstPlayerSettlementLocation, 13);

      // Act
      UInt32[] road;
      Guid longestRoadPlayerId;
      var result = gameBoard.TryGetLongestRoadDetails(out longestRoadPlayerId, out road);

      // Assert
      result.ShouldBeTrue();
      longestRoadPlayerId.ShouldBe(playerId);
      var result1 = new List<UInt32> { 10u, FirstPlayerRoadEndLocation, FirstPlayerSettlementLocation, 4u };
      var result2 = new List<UInt32> { 10u, FirstPlayerRoadEndLocation, FirstPlayerSettlementLocation, 13u };
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
    public void TryGetLongestRoadDetails_SettlementBetweenTwoLoops_ReturnsLongestRoadDetails()
    {
      // Arrange
      var gameBoard = new GameBoard(BoardSizes.Standard);

      var playerId = Guid.NewGuid();
      gameBoard.PlaceStartingInfrastructure(playerId, FirstPlayerSettlementLocation, FirstPlayerRoadEndLocation);
      gameBoard.PlaceStartingInfrastructure(playerId, 0, 1);

      gameBoard.PlaceRoadSegment(playerId, FirstPlayerRoadEndLocation, 21);
      gameBoard.PlaceRoadSegment(playerId, FirstPlayerRoadEndLocation, 10);
      gameBoard.PlaceRoadSegment(playerId, 21, 20);
      gameBoard.PlaceRoadSegment(playerId, 20, 19);
      gameBoard.PlaceRoadSegment(playerId, 19, 9);
      gameBoard.PlaceRoadSegment(playerId, 9, 10);

      gameBoard.PlaceRoadSegment(playerId, FirstPlayerSettlementLocation, 13);
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
      var result1 = new List<UInt32> { FirstPlayerRoadEndLocation, 10u, 9u, 19u, 20u, 21u, FirstPlayerRoadEndLocation, FirstPlayerSettlementLocation, 13u, 23u, 24u, 25u, 15u, 14u, 13u };
      var result2 = new List<UInt32> { FirstPlayerRoadEndLocation, 10u, 9u, 19u, 20u, 21u, FirstPlayerRoadEndLocation, FirstPlayerSettlementLocation, 13u, 14u, 15u, 25u, 24u, 23u, 13u };
      var result3 = new List<UInt32> { FirstPlayerRoadEndLocation, 21u, 20u, 19u, 9u, 10u, FirstPlayerRoadEndLocation, FirstPlayerSettlementLocation, 13u, 23u, 24u, 25u, 15u, 14u, 13u };
      var result4 = new List<UInt32> { FirstPlayerRoadEndLocation, 21u, 20u, 19u, 9u, 10u, FirstPlayerRoadEndLocation, FirstPlayerSettlementLocation, 13u, 14u, 15u, 25u, 24u, 23u, 13u };
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
    public void TryGetLongestRoadDetails_StartingSettlementOnLongestRoadWithFork_ReturnsLongestRoadDetails()
    {
      // Arrange
      var gameBoard = new GameBoard(BoardSizes.Standard);

      var playerId = Guid.NewGuid();
      gameBoard.PlaceStartingInfrastructure(playerId, FirstPlayerSettlementLocation, FirstPlayerRoadEndLocation);
      gameBoard.PlaceStartingInfrastructure(playerId, 0, 1);

      gameBoard.PlaceRoadSegment(playerId, FirstPlayerRoadEndLocation, 10);
      gameBoard.PlaceRoadSegment(playerId, 10, 2);
      gameBoard.PlaceRoadSegment(playerId, 10, 9);
      gameBoard.PlaceRoadSegment(playerId, 9, 8);

      gameBoard.PlaceRoadSegment(playerId, FirstPlayerSettlementLocation, 4);

      // Act
      UInt32[] road;
      Guid longestRoadPlayerId;
      var result = gameBoard.TryGetLongestRoadDetails(out longestRoadPlayerId, out road);

      // Assert
      result.ShouldBeTrue();
      longestRoadPlayerId.ShouldBe(playerId);

      var result1 = new List<UInt32> { 4u, FirstPlayerSettlementLocation, FirstPlayerRoadEndLocation, 10u, 9u, 8u };
      var result2 = new List<UInt32>(result1);
      result2.Reverse();
      this.RoadShouldBeSameAsOneOf(road, result1, result2);
    }

    /// <summary>
    /// Road segments form a loop with a short branch. Settlement is on intersection between loop and branch.
    /// </summary>
    [Test]
    public void TryGetLongestRoadDetails_SettlementOnLoopIntersectionWithShortBranch_ReturnsLongestRoadDetails()
    {
      // Arrange
      var gameBoard = new GameBoard(BoardSizes.Standard);

      var playerId = Guid.NewGuid();
      gameBoard.PlaceStartingInfrastructure(playerId, FirstPlayerSettlementLocation, FirstPlayerRoadEndLocation);
      gameBoard.PlaceStartingInfrastructure(playerId, SecondPlayerSettlementLocation, SecondPlayerRoadEndLocation);

      gameBoard.PlaceRoadSegment(playerId, FirstPlayerSettlementLocation, 13);
      gameBoard.PlaceRoadSegment(playerId, FirstPlayerRoadEndLocation, 21);
      gameBoard.PlaceRoadSegment(playerId, 21, 22);
      gameBoard.PlaceRoadSegment(playerId, 22, 23);
      gameBoard.PlaceRoadSegment(playerId, 13, 23);

      gameBoard.PlaceRoadSegment(playerId, FirstPlayerSettlementLocation, 4);
      gameBoard.PlaceRoadSegment(playerId, 4, 3);
      gameBoard.PlaceRoadSegment(playerId, 3, 2);

      // Act
      UInt32[] road;
      Guid longestRoadPlayerId;
      var result = gameBoard.TryGetLongestRoadDetails(out longestRoadPlayerId, out road);

      // Assert
      result.ShouldBeTrue();
      longestRoadPlayerId.ShouldBe(playerId);

      var result1 = new List<UInt32> { FirstPlayerSettlementLocation, FirstPlayerRoadEndLocation, 21, 22, 23, 13, FirstPlayerSettlementLocation, 4, 3, 2 };
      var result2 = new List<UInt32>(result1);
      result2.Reverse();

      this.RoadShouldBeSameAsOneOf(road, result1, result2);
    }

    /// <summary>
    /// Road segments form a loop with a short branch. Settlement is on edge of loop.
    /// </summary>
    [Test]
    public void TryGetLongestRoadDetails_SettlementOnLoopWithShortBranch_ReturnsLongestRoadDetails()
    {
      // Arrange
      var gameBoard = new GameBoard(BoardSizes.Standard);

      var playerId = Guid.NewGuid();
      gameBoard.PlaceStartingInfrastructure(playerId, FirstPlayerSettlementLocation, 4);
      gameBoard.PlaceStartingInfrastructure(playerId, 0, 1);

      gameBoard.PlaceRoadSegment(playerId, FirstPlayerSettlementLocation, 13);
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

      var result1 = new List<UInt32> { 15, 14, 6, 5, 4, FirstPlayerSettlementLocation, 13, 14 };
      var result2 = new List<UInt32> { 15, 14, 13, FirstPlayerSettlementLocation, 4, 5, 6, 14 };
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
    [TestCase(new UInt32[] { FirstPlayerSettlementLocation, FirstPlayerRoadEndLocation, FirstPlayerRoadEndLocation, 10, FirstPlayerRoadEndLocation, 21, FirstPlayerSettlementLocation, 13, 13, 23, 13, 14, FirstPlayerSettlementLocation, 4, 4, 3, 4, 5, 21, 22, 22, 23, 14, 6, 6, 5, 2, 3, 2, 10 })]
    [TestCase(new UInt32[] { FirstPlayerSettlementLocation, 13, 13, 14, 23, 13, FirstPlayerSettlementLocation, 4, 3, 4, 4, 5, FirstPlayerSettlementLocation, FirstPlayerRoadEndLocation, FirstPlayerRoadEndLocation, 10, FirstPlayerRoadEndLocation, 21, 14, 6, 6, 5, 3, 2, 2, 10, 22, 21, 23, 22 })]
    [TestCase(new UInt32[] { FirstPlayerSettlementLocation, 4, 4, 3, 4, 5, FirstPlayerSettlementLocation, FirstPlayerRoadEndLocation, FirstPlayerRoadEndLocation, 10, 21, FirstPlayerRoadEndLocation, FirstPlayerSettlementLocation, 13, 23, 13, 13, 14, 3, 2, 10, 2, 22, 21, 22, 23, 6, 14, 6, 5 })]
    public void TryGetLongestRoadDetails_SettlementOnIntersectionOfThreeLoops_ReturnsLongestRoadDetails(UInt32[] locations)
    {
      // Arrange
      var gameBoard = new GameBoard(BoardSizes.Standard);

      var playerId = Guid.NewGuid();
      gameBoard.PlaceStartingInfrastructure(playerId, SecondPlayerSettlementLocation, SecondPlayerRoadEndLocation);

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
    [TestCase(new UInt32[] { 21, 20, 20, 19, 19, 18, 18, 29, 29, 28, 20, 31, 31, 30, 30, 29 })]
    [TestCase(new UInt32[] { 21, 20, 20, 31, 31, 30, 30, 29, 29, 28, 20, 19, 19, 18, 18, 29 })]
    public void TryGetLongestRoadDetails_TwoRoutesToSameDestinationAreEqual_ReturnsLongestRoadDetails(UInt32[] locations)
    {
      // Arrange
      var gameBoard = new GameBoard(BoardSizes.Standard);

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
    [TestCase(new UInt32[] { 21, 20, 20, 19, 19, 9, 9, 8, 8, 7, 7, 17, 17, 18, 18, 29, 29, 28, 20, 31, 31, 30, 30, 29 })]
    [TestCase(new UInt32[] { 21, 20, 20, 31, 31, 30, 30, 29, 29, 28, 20, 19, 19, 9, 9, 8, 8, 7, 7, 17, 17, 18, 18, 29 })]
    public void TryGetLongestRoadDetails_TwoRoutesToSameDestinationAreDifferent_ReturnsLongestRoadDetails(UInt32[] locations)
    {
      // Arrange
      var gameBoard = new GameBoard(BoardSizes.Standard);

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

    private void BuildRoadBranch(GameBoard gameBoard, Guid playerId, UInt32[] branch)
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
