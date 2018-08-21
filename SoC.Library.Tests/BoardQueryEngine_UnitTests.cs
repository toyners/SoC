
namespace Jabberwocky.SoC.Library.UnitTests
{
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

      var results = queryEngine.GetLocationsWithBestYield(5);

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
  }
}
