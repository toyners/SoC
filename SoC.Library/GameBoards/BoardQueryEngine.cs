
namespace Jabberwocky.SoC.Library.GameBoards
{
  using System;
  using System.Collections.Generic;
  using System.Diagnostics;
  using System.IO;
  using System.Reflection;

  public class BoardQueryEngine : IBoardQueryEngine
  {
    #region Fields
    private GameBoard board;
    private readonly List<UInt32>[] locationInformation;
    private readonly Tuple<ResourceTypes?, UInt32>[] hexInformation;
    private readonly Int32[] locationsOrderedByBestYield;
    private readonly uint[][] neighboursOfLocation; //TODO: Push to board - this is static (maybe)
    #endregion

    #region Construction
    public BoardQueryEngine(GameBoard board)
    {
      this.board = board;
      this.hexInformation = this.board.GetHexData();
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

      this.locationsOrderedByBestYield = this.GetLocationsOrderedByBestYield();

      this.neighboursOfLocation = this.CreateNeighboursOfLocation();
    }
    #endregion

    #region Methods
    /// <summary>
    /// Get the first n locations with highest resource returns that are valid for settlement
    /// </summary>
    /// <returns></returns>
    public uint[] GetLocationsWithBestYield(int count)
    {
      var result = new uint[count];
      
      var queue = new Queue<uint>();
      var index = 0;
      while (queue.Count < count && index < this.locationsOrderedByBestYield.Length)
      {
        var location = this.locationsOrderedByBestYield[index++];
        if (location == -1)
        {
          continue;
        }

        var convertedLocation = (uint)location;
        if (this.board.SettlementLocationIsOccupied(convertedLocation) || this.board.TooCloseToSettlement(convertedLocation, out var id, out var i))
        {
          this.locationsOrderedByBestYield[index - 1] = -1;
          continue;
        }

        queue.Enqueue(convertedLocation);
      }

      return queue.ToArray();
    }

    private Int32 CalculateYield(UInt32 productionFactor)
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

    private int[] GetLocationsOrderedByBestYield()
    {
      var locations = new List<Int32>(new Int32[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16,
        17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40,
        41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53 });

      var yieldsByLocation = new Dictionary<Int32, Int32>();

      locations.Sort((firstLocation, secondLocation) => {

        if (!yieldsByLocation.TryGetValue(firstLocation, out var firstLocationYield))
        {
          foreach (var hexId in this.locationInformation[firstLocation])
          {
            firstLocationYield += this.CalculateYield(this.hexInformation[hexId].Item2);
          }

          yieldsByLocation.Add(firstLocation, firstLocationYield);
        }

        if (!yieldsByLocation.TryGetValue(secondLocation, out var secondLocationYield))
        {
          foreach (var hexId in this.locationInformation[secondLocation])
          {
            secondLocationYield += this.CalculateYield(this.hexInformation[hexId].Item2);
          }

          yieldsByLocation.Add(secondLocation, secondLocationYield);
        }

        if (firstLocationYield == secondLocationYield)
        {
          // if the yield is the same then order by location in ascending order
          return firstLocation.CompareTo(secondLocation);
        }

        return (firstLocationYield < secondLocationYield ? 1 : -1);
      });

      return locations.ToArray();
    }

    private uint[][] CreateNeighboursOfLocation()
    {
      var result = new uint[GameBoard.StandardBoardLocationCount][];

      using (var stream = this.GetType().GetTypeInfo().Assembly.GetManifestResourceStream("Jabberwocky.SoC.Library.GameBoards.NeighboursByLocation.txt"))
      {
        using (var streamReader = new StreamReader(stream))
        {
          while(!streamReader.EndOfStream)
          {
            var line = streamReader.ReadLine();
            var barIndex = line.IndexOf("|");
            var neighboursRaw = line.Substring(barIndex + 1).Split(',');
            var location = int.Parse(line.Substring(0, barIndex));
            result[location] = new uint[neighboursRaw.Length];

            var neighbourIndex = 0;
            foreach(var neighbourRaw in neighboursRaw)
            {
              result[location][neighbourIndex++] = uint.Parse(neighbourRaw);
            }
          }
        }
      }

      return result;
    }

    private void OutputScore(UInt32 location, List<UInt32> hexes)
    {
      var score = "" + location + " - ";
      Int32 yield = 0;
      foreach (var hexId in hexes)
      {
        score += "Hex: " + hexId + " has pf " +
          this.hexInformation[hexId].Item2 + " (" + this.CalculateYield(this.hexInformation[hexId].Item2) + ") ";

        yield += this.CalculateYield(this.hexInformation[hexId].Item2);
      }

      score += ". Total yield is " + yield;
      Debug.WriteLine(score);
    }

    public uint[] GetLocationsWithBestYield(uint count)
    {
      throw new NotImplementedException();
    }

    public List<KeyValuePair<uint, List<uint>>> GetRoadPathCandidates(List<uint> settlementCandidates)
    {
      throw new NotImplementedException();
    }

    public List<uint> GetLongestRoadForPlayer(Guid id)
    {
      throw new NotImplementedException();
    }

    public uint[] GetValidConnectedLocationsFrom(uint location)
    {
      return this.neighboursOfLocation[location];
    }

    public uint[] GetNeighbouringLocationsFrom(uint location)
    {
      return this.neighboursOfLocation[location];
    }
    #endregion
  }
}
