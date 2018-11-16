
namespace Jabberwocky.SoC.Library.UnitTests
{
  using System;
  using Jabberwocky.SoC.Library.GameBoards;
  using Jabberwocky.SoC.Library.UnitTests.Extensions;
  using NUnit.Framework;

  [TestFixture]
  [Category("All")]
  [Category("GameBoardQuery")]
  public class GameBoardQuery_UnitTests
    {
    [Test]
    public void GetLocationsWithBestYield_FirstLocationFromEmptyBoard_ReturnsExpectedLocation()
    {
      var gameBoard = new GameBoard(BoardSizes.Standard);
      var queryEngine = new GameBoardQuery(gameBoard);

      var results = queryEngine.GetLocationsWithBestYield(1);

      results.ShouldContainExact(new[] { 12u });
    }

    [Test]
    public void GetLocationsWithBestYield_FirstFiveLocationsFromEmptyBoard_ReturnsExpectedLocations()
    {
      var gameBoard = new GameBoard(BoardSizes.Standard);
      var queryEngine = new GameBoardQuery(gameBoard);

      var results = queryEngine.GetLocationsWithBestYield(5);

      results.ShouldContainExact(new[] { 12u, 31u, 35u, 41u, 43u });
    }

    [Test]
    [TestCase(12u, 4u, 31u, 35u, 41u, 43u, 18u)]
    [TestCase(31u, 20u, 12u, 35u, 41u, 43u, 11u)]
    [TestCase(35u, 24u, 12u, 31u, 41u, 43u, 11u)]
    [TestCase(41u, 49u, 12u, 31u, 35u, 43u, 11u)]
    [TestCase(43u, 44u, 12u, 31u, 35u, 41u, 11u)]
    [TestCase(11u, 10u, 31u, 35u, 41u, 43u, 18u)]
    public void GetLocationsWithBestYield_FiveLocationsWhenLocationIsTaken_ReturnsExpectedLocations(UInt32 settlementLocation, UInt32 roadEndLocation, UInt32 firstLocation, UInt32 secondLocation, UInt32 thirdLocation, UInt32 fourthLocation,  UInt32 fifthLocation)
    {
      var gameBoard = new GameBoard(BoardSizes.Standard);
      gameBoard.PlaceStartingInfrastructure(Guid.NewGuid(), settlementLocation, roadEndLocation);
      var queryEngine = new GameBoardQuery(gameBoard);

      var results = queryEngine.GetLocationsWithBestYield(5);

      results.ShouldContainExact(new[] { firstLocation, secondLocation, thirdLocation, fourthLocation, fifthLocation });
    }
  }
}
