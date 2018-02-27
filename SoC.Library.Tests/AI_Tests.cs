
namespace Jabberwocky.SoC.Library.UnitTests
{
  using System;
  using GameBoards;
  using LocalGameController_Tests;
  using NUnit.Framework;
  using Shouldly;

  [TestFixture]
  [Category("All")]
  [Category("ArtificialIntelligence")]
  public class AI_Tests
  {
    #region Methods
    [Test]
    [TestCase(ResourceTypes.Brick, new uint[] { 2, 3, 4, 10, 11, 12 }, 8u)]
    [TestCase(ResourceTypes.Grain, new uint[] { 43, 44, 45, 51, 52, 53 }, 8u)]
    [TestCase(ResourceTypes.Lumber, new uint[] { 24, 25, 26, 35, 36, 37 }, 6u)]
    [TestCase(ResourceTypes.Ore, new uint[] { 18, 19, 20, 29, 30, 31 }, 6u)]
    [TestCase(ResourceTypes.Wool, new uint[] { 22, 23, 24, 33, 34, 35 }, 9u)]
    public void GetPossibleSettlementLocationsForBestReturningResourceOfType_EmptyBoard_ReturnsExpected(ResourceTypes resourceType, UInt32[] expectedLocations, UInt32 expectedProductionFactor)
    {
      var gameBoard = new GameBoard(BoardSizes.Standard);
      UInt32 actualProductionFactor = 0;
      var actualLocations = AI.GetPossibleSettlementLocationsForBestReturningResourceType(gameBoard, resourceType, out actualProductionFactor);

      actualProductionFactor.ShouldBe(expectedProductionFactor);
      actualLocations.ShouldContainExact(expectedLocations);
    }

    [Test]
    public void GetPossibleSettlementLocationsForBestReturningResourceOfType_HighestProductionFactorHexFullyOccupied_ReturnsLocationsForNextBestProducingHex()
    {
      var gameBoard = new GameBoard(BoardSizes.Standard);
      UInt32 actualProductionFactor = 0;
      var actualLocations = AI.GetPossibleSettlementLocationsForBestReturningResourceType(gameBoard, ResourceTypes.Ore, out actualProductionFactor);

      actualProductionFactor.ShouldBe(5u);
      actualLocations.ShouldContainExact(new[] { 4u, 5u, 6u, 12u, 13u, 14u });
    }

    #endregion 
  }
}
