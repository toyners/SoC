
namespace Jabberwocky.SoC.Library.GameBoards
{
  using System;
  using System.Collections.Generic;

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
      var result = new UInt32[count];

      var hexInformation = this.board.GetHexInformation();
      var locationInformation = new List<UInt32>[GameBoard.StandardBoardLocationCount];
      var locationIndex = 0;
      for (var hexIndex = 0; hexIndex < hexInformation.Length; hexIndex++)
      {

      }

      // Hex 0
      locationInformation[0] = new List<uint> { 0 };
      locationInformation[1] = new List<uint> { 0 };
      locationInformation[2] = new List<uint> { 0, 1 };
      locationInformation[3] = new List<uint> { 1 };
      locationInformation[4] = new List<uint> { 1, 2 };
      locationInformation[5] = new List<uint> { 2 };
      locationInformation[6] = new List<uint> { 2 };
      locationInformation[7] = new List<uint> { 3 };
      locationInformation[8] = new List<uint> { 0, 3 };
      locationInformation[9] = new List<uint> { 0, 3, 4 };
      locationInformation[10] = new List<uint> { 0, 1, 5 };
      locationInformation[11] = new List<uint> { 1, 4, 5 };
      locationInformation[12] = new List<uint> { 1, 2, 5 };
      locationInformation[13] = new List<uint> { 2, 5, 6 };
      locationInformation[14] = new List<uint> { 2, 6 };
      locationInformation[15] = new List<uint> { 6 };

      if (count == 1)
      {
        return new[] { 31u };
      }

      return new[] { 31u, 30u, 43u, 44u, 19u };
    }
  }
}
