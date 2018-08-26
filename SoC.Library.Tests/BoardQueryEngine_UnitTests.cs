
namespace Jabberwocky.SoC.Library.UnitTests
{
  using System;
  using Jabberwocky.SoC.Library.GameBoards;
  using Jabberwocky.SoC.Library.UnitTests.Extensions;
  using NUnit.Framework;

  [TestFixture]
  [Category("All")]
  [Category("BoardQueryEngine")]
  public class BoardQueryEngine_UnitTests
  {
    [Test]
    public void GetLocationsWithBestYield_FirstLocationFromEmptyBoard_ReturnsExpectedLocation()
    {
      var gameBoard = new GameBoard(BoardSizes.Standard);
      var queryEngine = new BoardQueryEngine(gameBoard);

      var results = queryEngine.GetLocationsWithBestYield(1);

      results.ShouldContainExact(new[] { 12u });
    }

    [Test]
    public void GetLocationsWithBestYield_FirstFiveLocationsFromEmptyBoard_ReturnsExpectedLocations()
    {
      var gameBoard = new GameBoard(BoardSizes.Standard);
      var queryEngine = new BoardQueryEngine(gameBoard);

      var results = queryEngine.GetLocationsWithBestYield(5);

      results.ShouldContainExact(new[] { 12u, 31u, 35u, 44u, 19u });
    }

    [Test]
    [TestCase(31u, 20u, 43u, 44u, 19u, 18u, 12u)]
    [TestCase(30u, 29u, 43u, 44u, 19u, 18u, 12u)]
    [TestCase(43u, 51u, 31u, 30u, 19u, 18u, 12u)]
    [TestCase(44u, 45u, 31u, 30u, 19u, 18u, 12u)]
    [TestCase(19u, 20u, 31u, 30u, 43u, 44u, 12u)]
    [TestCase(18u, 29u, 31u, 30u, 43u, 44u, 12u)]
    public void GetLocationsWithBestYield_FiveLocationsWhenBestLocationIsTaken_ReturnsExpectedLocations(UInt32 settlementLocation, UInt32 roadEndLocation, UInt32 firstLocation, UInt32 secondLocation, UInt32 thirdLocation, UInt32 fourthLocation,  UInt32 fifthLocation)
    {
      var gameBoard = new GameBoard(BoardSizes.Standard);
      gameBoard.PlaceStartingInfrastructure(Guid.NewGuid(), settlementLocation, roadEndLocation);
      var queryEngine = new BoardQueryEngine(gameBoard);

      var results = queryEngine.GetLocationsWithBestYield(5);

      results.ShouldContainExact(new[] { firstLocation, secondLocation, thirdLocation, fourthLocation, fifthLocation });
    }
  }
}
