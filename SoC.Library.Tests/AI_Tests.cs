
namespace Jabberwocky.SoC.Library.UnitTests
{
  using System;
  using System.Collections.Generic;
  using GameBoards;
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
      actualLocations.ShouldContainAll(expectedLocations);
    }

    #endregion 
  }

  public static class ShouldContainExtensions
  {
    public static void ShouldContainAll(this IEnumerable<UInt32> actualCollection, IEnumerable<UInt32> expectedCollection)
    {
      throw new NotImplementedException();
    }
  }
}
