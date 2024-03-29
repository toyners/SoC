﻿
namespace Jabberwocky.SoC.Library.UnitTests
{
  using System;
  using GameBoards;
  using Jabberwocky.SoC.Library.UnitTests.Extensions;
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
    [TestCase(ResourceTypes.Brick, new uint[] { 2, 3, 4, 10, 11, 12 }, 8)]
    [TestCase(ResourceTypes.Grain, new uint[] { 43, 44, 45, 51, 52, 53 }, 8)]
    [TestCase(ResourceTypes.Lumber, new uint[] { 24, 25, 26, 35, 36, 37 }, 6)]
    [TestCase(ResourceTypes.Ore, new uint[] { 18, 19, 20, 29, 30, 31 }, 6)]
    [TestCase(ResourceTypes.Wool, new uint[] { 22, 23, 24, 33, 34, 35 }, 9)]
    public void GetPossibleSettlementLocationsForBestReturningResourceOfType_EmptyBoard_ReturnsExpected(ResourceTypes resourceType, UInt32[] expectedLocations, int expectedProductionFactor)
    {
      var gameBoard = new GameBoard(BoardSizes.Standard);
      int actualProductionFactor = 0;
      var ai = new AI(gameBoard);
      var actualLocations = ai.GetLocationsForBestReturningResourceType(gameBoard, resourceType, out actualProductionFactor);

      actualProductionFactor.ShouldBe(expectedProductionFactor);
      actualLocations.ShouldContainExact(expectedLocations);
    }

    [Test]
    public void GetPossibleSettlementLocationsForBestReturningResourceOfType_HighestProductionFactorHexHasLastLocationAvailable_ReturnsLastLocation()
    {
      var firstOpponentId = Guid.NewGuid();
      var secondOpponentId = Guid.NewGuid();

      var gameBoard = new GameBoard(BoardSizes.Standard);
      gameBoard.PlaceStartingInfrastructure(firstOpponentId, 18, 17);
      gameBoard.PlaceStartingInfrastructure(secondOpponentId, 20, 21);

      var ai = new AI(gameBoard);

      int actualProductionFactor = 0;
      var actualLocations = ai.GetLocationsForBestReturningResourceType(gameBoard, ResourceTypes.Ore, out actualProductionFactor);

      actualProductionFactor.ShouldBe(6);
      actualLocations.ShouldContainExact(new[] { 30u });
    }

    [Test]
    public void GetPossibleSettlementLocationsForBestReturningResourceOfType_HighestProductionFactorHexFullyOccupied_ReturnsLocationsForNextBestProducingHex()
    {
      var firstOpponentId = Guid.NewGuid();
      var secondOpponentId = Guid.NewGuid();
      var thirdOpponentId = Guid.NewGuid();

      var gameBoard = new GameBoard(BoardSizes.Standard);
      gameBoard.PlaceStartingInfrastructure(firstOpponentId, 18, 17);
      gameBoard.PlaceStartingInfrastructure(secondOpponentId, 20, 21);
      gameBoard.PlaceStartingInfrastructure(thirdOpponentId, 30, 40);

      var ai = new AI(gameBoard);

      int actualProductionFactor = 0;
      var actualLocations = ai.GetLocationsForBestReturningResourceType(gameBoard, ResourceTypes.Ore, out actualProductionFactor);

      actualProductionFactor.ShouldBe(5);
      actualLocations.ShouldContainExact(new[] { 4u, 5u, 6u, 12u, 13u, 14u });
    }
    #endregion 
  }
}
