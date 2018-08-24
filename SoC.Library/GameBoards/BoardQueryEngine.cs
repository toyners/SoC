
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

    private UInt32 CalculateYield(UInt32 productionFactor)
    {
      switch (productionFactor)
      {
        case 2:
        case 12: return 3;
        case 3:
        case 11: return 6;
        case 4:
        case 10: return 8;
        case 5:
        case 9: return 11;
        case 6:
        case 8: return 14;
        case 0:
        case 7: return 0;
      }

      throw new Exception("Should not get here");
    }

    /// <summary>
    /// Get the first n locations with highest resource returns that are valid for settlement
    /// </summary>
    /// <returns></returns>
    public UInt32[] GetLocationsWithBestYield(Int32 count)
    {
      var result = new UInt32[count];
      var sorted = new List<UInt32>(new UInt32[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16,
        17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40,
        41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53 });

      var yieldsByLocation = new Dictionary<UInt32, UInt32>();

      sorted.Sort((firstLocation, secondLocation) => {

        if (!yieldsByLocation.TryGetValue(firstLocation, out var firstLocationYield))
        {
          //firstLocationYield = 0;
          foreach (var hexId in this.locationInformation[firstLocation])
          {
            firstLocationYield += this.CalculateYield(this.hexInformation[hexId].Item2);
          }

          yieldsByLocation.Add(firstLocation, firstLocationYield);
        }

        if (!yieldsByLocation.TryGetValue(secondLocation, out var secondLocationYield))
        {
          secondLocationYield = 0;
          foreach (var hexId in this.locationInformation[secondLocation])
          {
            secondLocationYield += this.CalculateYield(this.hexInformation[hexId].Item2);
          }

          yieldsByLocation.Add(secondLocation, secondLocationYield);
        }

        if (firstLocationYield == secondLocationYield)
        {
          return 0;
        }

        return (firstLocationYield < secondLocationYield ? 1 : -1);
      });

      return sorted.GetRange(0, count).ToArray();
    }
  }
}
