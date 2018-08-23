
namespace Jabberwocky.SoC.Library.GameBoards
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Reflection;

  public class BoardQueryEngine
  {
    private GameBoard board;
    private readonly List<UInt32>[] locationInformation;
    private readonly Tuple<ResourceTypes?, UInt32>[] hexInformation;

    public BoardQueryEngine(GameBoard board)
    {
      this.board = board;
      this.hexInformation = this.board.GetHexInformation();
      this.locationInformation = new List<UInt32>[GameBoard.StandardBoardLocationCount];

      var names = this.GetType().GetTypeInfo().Assembly.GetManifestResourceNames();

      using (var stream = this.GetType().GetTypeInfo().Assembly.GetManifestResourceStream("Jabberwocky.SoC.Library.GameBoards.Locations.txt"))
      {
        using (var streamReader = new StreamReader(stream))
        {
          var index = 0;
          while (!streamReader.EndOfStream)
          {
            this.locationInformation[index] = new List<UInt32>();
            foreach(var hexId in streamReader.ReadLine().Split(','))
            {
              this.locationInformation[index].Add(UInt32.Parse(hexId));
            }

            index++;
          }
        }
      }
    }

    /// <summary>
    /// Get the first n locations with highest resource returns that are valid for settlement
    /// </summary>
    /// <returns></returns>
    public UInt32[] GetLocationsWithBestYield(UInt32 count)
    {
      var result = new UInt32[count];

      if (count == 1)
      {
        return new[] { 31u };
      }

      return new[] { 31u, 30u, 43u, 44u, 19u };
    }
  }
}
