
namespace Jabberwocky.SoC.Library.UnitTests
{
  using System;
  using Jabberwocky.SoC.Library.GameBoards;
  using Jabberwocky.SoC.Library.UnitTests.Extensions;
  using NUnit.Framework;
  using Shouldly;

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

      results.ShouldContainExact(new[] { 31u });
    }

    [Test]
    public void GetLocationsWithBestYield_FirstFiveLocationsFromEmptyBoard_ReturnsExpectedLocations()
    {
      var gameBoard = new GameBoard(BoardSizes.Standard);
      var queryEngine = new BoardQueryEngine(gameBoard);

      var results = queryEngine.GetLocationsWithBestYield(5);

      results.ShouldContainExact(new[] { 31u, 30u, 43u, 44u, 19u });
    }

    [Test]
    [TestCase(31u, 43u, 44u, 19u, 18u, 12u)]
    [TestCase(30u, 43u, 44u, 19u, 18u, 12u)]
    [TestCase(43u, 31u, 30u, 19u, 18u, 12u)]
    [TestCase(44u, 31u, 30u, 19u, 18u, 12u)]
    [TestCase(19u, 31u, 30u, 43u, 44u, 12u)]
    [TestCase(18u, 31u, 30u, 43u, 44u, 12u)]
    public void GetLocationsWithBestYield_FiveLocationsWhenBestLocationIsTaken_ReturnsExpectedLocations(UInt32 settlementLocation, UInt32 firstLocation, UInt32 secondLocation, UInt32 thirdLocation, UInt32 fourthLocation,  UInt32 fifthLocation)
    {
      var gameBoard = new GameBoard(BoardSizes.Standard);
      gameBoard.PlaceSettlement(Guid.NewGuid(), settlementLocation);
      var queryEngine = new BoardQueryEngine(gameBoard);

      var results = queryEngine.GetLocationsWithBestYield(5);

      results.ShouldContainExact(new[] { firstLocation, secondLocation, thirdLocation, fourthLocation, fifthLocation });
    }
  }
}
