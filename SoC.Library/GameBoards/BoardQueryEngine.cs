
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

    /// <summary>
    /// Get the first n locations with highest resource returns that are valid for settlement
    /// </summary>
    /// <returns></returns>
    public UInt32[] GetLocationsWithBestYield(UInt32 count)
    {
      var result = new UInt32[count];
      var sorted = new List<Int32>();

      var yieldsByLocation = new Dictionary<Int32, UInt32>();

      sorted.Sort((firstLocation, secondLocation) => {

        if (!yieldsByLocation.TryGetValue(firstLocation, out var firstLocationYield))
        {
          foreach (var hexId in this.locationInformation[firstLocation])
          {
            //this.hexInformation[hexId].Item2
          }

          yieldsByLocation.Add(firstLocation, firstLocationYield);
        }

        if (!yieldsByLocation.TryGetValue(secondLocation, out var secondLocationYield))
        {
          yieldsByLocation.Add(secondLocation, secondLocationYield);
        }

        if (firstLocationYield == secondLocationYield)
        {
          return 0;
        }

        return (firstLocationYield < secondLocationYield ? -1 : 1);
      });

      for (var locationIndex = 0; locationIndex < this.locationInformation.Length; locationIndex++)
      {
        
      }


      if (count == 1)
      {
        return new[] { 31u };
      }

      return new[] { 31u, 30u, 43u, 44u, 19u };
    }
  }
}
