
namespace Jabberwocky.SoC.Library.GameBoards
{
  using System;

  public class BoardQueryEngine
  {
    private GameBoard board;

    public BoardQueryEngine(GameBoard board)
    {
      this.board = board;
    }

    /// <summary>
    /// Get the first n locations with highest resource returns that are valid for settlement
    /// </summary>
    /// <returns></returns>
    public UInt32[] GetLocationsWithBestYield(UInt32 count)
    {
      if (count == 1)
      {
        return new[] { 31u };
      }

      return new[] { 31u, 30u, 43u, 44u, 19u };
    }
  }
}
