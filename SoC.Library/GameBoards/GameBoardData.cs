
namespace Jabberwocky.SoC.Library.GameBoards
{
  using System;
  using System.Collections.Generic;

  /// <summary>
  /// Holds data for all locations, trails, towns, cities, roads, resource providers and robber location.
  /// Provides methods for verifying locations for placing new settlements, cities and roads for a player.
  /// </summary>
  public class GameBoardData
  {
    public enum VerificationResults
    {
      Valid,
      LocationIsOccupied,
      NoConnectingRoad,
      NoSettlementToUpgrade,
      TooCloseToSettlement,
    }

    #region Fields
    public Dictionary<Guid, List<Trail>> Roads;

    public Location[] Locations;

    public Trail[] Trails;

    public ResourceProvider[] Providers;

    public const Int32 StandardBoardLocationCount = 54;

    public const Int32 StandardBoardTrailCount = 72;

    public const Int32 StandardBoardResourceProviderCount = 19;
    private Dictionary<UInt32, Guid> settlements;
    private Dictionary<Guid, List<UInt32>> settlementsByPlayer;
    private Boolean[,] connections;
    #endregion

    #region Construction
    public GameBoardData(BoardSizes size)
    {
      if (size == BoardSizes.Extended)
      {
        throw new Exception("Extended boards not implemented.");
      }

      this.settlements = new Dictionary<UInt32, Guid>();
      this.settlementsByPlayer = new Dictionary<Guid, List<UInt32>>();
      this.Roads = new Dictionary<Guid, List<Trail>>();

      this.CreateLocations();

      this.Trails = new Trail[StandardBoardTrailCount];

      this.connections = new Boolean[GameBoardData.StandardBoardLocationCount, GameBoardData.StandardBoardLocationCount];
      this.ConnectLocationsVertically();
      this.ConnectLocationsHorizontally();

      var index = this.StitchLocationsTogetherUsingVerticalTrails();

      this.StitchLocationsTogetherUsingHorizontalTrails(index);

      this.CreateResourceProviders();
    }
    #endregion

    #region Methods
    public VerificationResults CanPlaceCity(PlayerData player, Location location)
    {
      throw new NotImplementedException();
    }

    public VerificationResults CanPlaceRoad(PlayerData player, Location startLocation, Location endLocation)
    {
      throw new NotImplementedException();
    }

    public VerificationResults CanPlaceSettlement(UInt32 locationIndex, out Guid playerId)
    {
      playerId = Guid.Empty;
      if (this.settlements.ContainsKey(locationIndex))
      {
        playerId = this.settlements[locationIndex];
        return VerificationResults.LocationIsOccupied;
      }

      var neighbourCount = 0;
      for (UInt32 index = 0; index < this.connections.GetLength(1) && neighbourCount < 3; index++)
      {
        if (this.connections[locationIndex, index])
        {
          neighbourCount++;
          if (this.settlements.ContainsKey(index))
          {
            playerId = this.settlements[index];
            return VerificationResults.TooCloseToSettlement;
          }
        }
      }

      return VerificationResults.Valid;
    }

    public SettlementPlacementVerificationResults CanPlaceSettlement(UInt32 locationIndex)
    {
      if (this.settlements.ContainsKey(locationIndex))
      {
        return new SettlementPlacementVerificationResults
        {
          Status = VerificationResults.LocationIsOccupied,
          LocationIndex = locationIndex,
          PlayerId = this.settlements[locationIndex]
        };
      }

      var neighbourCount = 0;
      for (UInt32 index = 0; index < this.connections.GetLength(1) && neighbourCount < 3; index++)
      {
        if (this.connections[locationIndex, index])
        {
          neighbourCount++;
          if (this.settlements.ContainsKey(index))
          {
            return new SettlementPlacementVerificationResults
            {
              Status = VerificationResults.TooCloseToSettlement,
              LocationIndex = index,
              PlayerId = this.settlements[index]
            };
          }
        }
      }

      return new SettlementPlacementVerificationResults
      {
        Status = VerificationResults.Valid,
        LocationIndex = 0u,
        PlayerId = Guid.Empty
      };
    }

    public Boolean[,] GetBoardSnapshot()
    {
      var snapshot = new Boolean[this.connections.GetLength(0), this.connections.GetLength(1)];
      for (var x = 0u; x < this.connections.GetLength(0); x++)
      {
        for (var y = 0u; y < this.connections.GetLength(0); y++)
        {
          snapshot[x, y] = this.connections[x, y];
        }
      }

      return snapshot;
    }

    public List<UInt32> GetPathBetweenLocations(UInt32 startIndex, UInt32 endIndex)
    {
      if (startIndex == endIndex)
      {
        return null;
      }

      var connections = new Boolean[GameBoardData.StandardBoardLocationCount, GameBoardData.StandardBoardLocationCount];
      var locations = new List<Location>(this.Locations);
      foreach (var trail in this.Trails)
      {
        var x = locations.IndexOf(trail.Location1);
        var y = locations.IndexOf(trail.Location2);
        connections[x, y] = connections[y, x] = true;
      }

      return PathFinder.GetPathBetweenPoints(startIndex, endIndex, connections);
    }

    public List<UInt32> GetSettlementsForPlayer(Guid playerId)
    {
      if (!this.settlementsByPlayer.ContainsKey(playerId))
      {
        return null;
      }

      return this.settlementsByPlayer[playerId];
    }

    public void PlaceStartingRoad(Guid playerId, Road road)
    {
    }

    public void PlaceStartingSettlement(Guid playerId, UInt32 locationIndex)
    {
      if (this.settlementsByPlayer.ContainsKey(playerId))
      {
        this.settlementsByPlayer[playerId].Add(locationIndex);
      }
      else
      {
        this.settlementsByPlayer.Add(playerId, new List<uint> { locationIndex });
      }

      this.settlements.Add(locationIndex, playerId);
    }
    
    private void CreateLocations()
    {
      this.Locations = new Location[StandardBoardLocationCount];
      for (var index = 0; index < StandardBoardLocationCount; index++)
      {
        this.Locations[index] = new Location();
      }
    }

    private void CreateResourceProviders()
    {
      //d,b8,o5
      var desert = new ResourceProvider();
      var brick8 = new ResourceProvider(ResourceTypes.Brick, 8);
      var ore5 = new ResourceProvider(ResourceTypes.Ore, 5);

      //b4,l3,w10,g2,
      var brick4 = new ResourceProvider(ResourceTypes.Brick, 4);
      var lumber3 = new ResourceProvider(ResourceTypes.Lumber, 3);
      var wool10 = new ResourceProvider(ResourceTypes.Wool, 10);
      var grain2 = new ResourceProvider(ResourceTypes.Grain, 2);

      //l11,o6,g11,w9,l6,
      var lumber11 = new ResourceProvider(ResourceTypes.Lumber, 11);
      var ore6 = new ResourceProvider(ResourceTypes.Ore, 6);
      var grain11 = new ResourceProvider(ResourceTypes.Grain, 11);
      var wool9 = new ResourceProvider(ResourceTypes.Wool, 9);
      var lumber6 = new ResourceProvider(ResourceTypes.Lumber, 6);

      //w12,b5,l4,o3
      var wool12 = new ResourceProvider(ResourceTypes.Wool, 12);
      var brick5 = new ResourceProvider(ResourceTypes.Brick, 5);
      var lumber4 = new ResourceProvider(ResourceTypes.Lumber, 4);
      var ore3 = new ResourceProvider(ResourceTypes.Ore, 3);

      //g9,w10,g8
      var grain9 = new ResourceProvider(ResourceTypes.Grain, 9);
      var grain8 = new ResourceProvider(ResourceTypes.Grain, 8);

      // Load the resource provider array
      this.Providers = new ResourceProvider[StandardBoardResourceProviderCount];
      this.Providers[0] = desert;
      this.Providers[1] = brick8;
      this.Providers[2] = ore5;
      this.Providers[3] = brick4;
      this.Providers[4] = lumber3;
      this.Providers[5] = wool10;
      this.Providers[6] = grain2;
      this.Providers[7] = lumber11;
      this.Providers[8] = ore6;
      this.Providers[9] = grain11;
      this.Providers[10] = wool9;
      this.Providers[11] = lumber6;
      this.Providers[12] = wool12;
      this.Providers[13] = brick5;
      this.Providers[14] = lumber4;
      this.Providers[15] = ore3;
      this.Providers[16] = grain9;
      this.Providers[17] = wool10;
      this.Providers[18] = grain8;

      // Side 1
      this.Locations[0].Providers.Add(desert);
      this.Locations[1].Providers.Add(desert);
      this.Locations[2].Providers.Add(desert);
      this.Locations[2].Providers.Add(brick8);
      this.Locations[3].Providers.Add(brick8);
      this.Locations[4].Providers.Add(brick8);
      this.Locations[4].Providers.Add(ore5);
      this.Locations[5].Providers.Add(ore5);
      this.Locations[6].Providers.Add(ore5);

      // Side 2
      this.Locations[7].Providers.Add(brick4);
      this.Locations[8].Providers.Add(desert);
      this.Locations[8].Providers.Add(brick4);
      this.Locations[9].Providers.Add(brick4);
      this.Locations[9].Providers.Add(desert);
      this.Locations[9].Providers.Add(lumber3);
      this.Locations[10].Providers.Add(brick8);
      this.Locations[10].Providers.Add(desert);
      this.Locations[10].Providers.Add(lumber3);
      this.Locations[11].Providers.Add(brick8);
      this.Locations[11].Providers.Add(wool10);
      this.Locations[11].Providers.Add(lumber3);
      this.Locations[12].Providers.Add(brick8);
      this.Locations[12].Providers.Add(wool10);
      this.Locations[12].Providers.Add(ore5);
      this.Locations[13].Providers.Add(wool10);
      this.Locations[13].Providers.Add(ore5);
      this.Locations[13].Providers.Add(grain2);
      this.Locations[14].Providers.Add(ore5);
      this.Locations[14].Providers.Add(grain2);
      this.Locations[15].Providers.Add(grain2);

      // Side 3
      this.Locations[16].Providers.Add(lumber11);
      this.Locations[17].Providers.Add(lumber11);
      this.Locations[17].Providers.Add(brick4);
      this.Locations[18].Providers.Add(lumber11);
      this.Locations[18].Providers.Add(brick4);
      this.Locations[18].Providers.Add(ore6);
      this.Locations[19].Providers.Add(lumber3);
      this.Locations[19].Providers.Add(brick4);
      this.Locations[19].Providers.Add(ore6);
      this.Locations[20].Providers.Add(lumber3);
      this.Locations[20].Providers.Add(grain11);
      this.Locations[20].Providers.Add(ore6);
      this.Locations[21].Providers.Add(lumber3);
      this.Locations[21].Providers.Add(wool10);
      this.Locations[21].Providers.Add(grain11);
      this.Locations[22].Providers.Add(grain11);
      this.Locations[22].Providers.Add(wool10);
      this.Locations[22].Providers.Add(wool9);
      this.Locations[23].Providers.Add(grain2);
      this.Locations[23].Providers.Add(wool10);
      this.Locations[23].Providers.Add(wool9);
      this.Locations[24].Providers.Add(grain2);
      this.Locations[24].Providers.Add(lumber6);
      this.Locations[24].Providers.Add(wool9);
      this.Locations[25].Providers.Add(lumber6);
      this.Locations[25].Providers.Add(grain2);
      this.Locations[26].Providers.Add(lumber6);

      // Side 4
      this.Locations[27].Providers.Add(lumber11);
      this.Locations[28].Providers.Add(lumber11);
      this.Locations[28].Providers.Add(wool12);
      this.Locations[29].Providers.Add(lumber11);
      this.Locations[29].Providers.Add(wool12);
      this.Locations[29].Providers.Add(ore6);
      this.Locations[30].Providers.Add(wool12);
      this.Locations[30].Providers.Add(ore6);
      this.Locations[30].Providers.Add(brick5);
      this.Locations[31].Providers.Add(ore6);
      this.Locations[31].Providers.Add(brick5);
      this.Locations[31].Providers.Add(grain11);
      this.Locations[32].Providers.Add(brick5);
      this.Locations[32].Providers.Add(grain11);
      this.Locations[32].Providers.Add(lumber4);
      this.Locations[33].Providers.Add(grain11);
      this.Locations[33].Providers.Add(lumber4);
      this.Locations[33].Providers.Add(wool9);
      this.Locations[34].Providers.Add(lumber4);
      this.Locations[34].Providers.Add(wool9);
      this.Locations[34].Providers.Add(ore3);
      this.Locations[35].Providers.Add(wool9);
      this.Locations[35].Providers.Add(ore3);
      this.Locations[35].Providers.Add(lumber6);
      this.Locations[36].Providers.Add(lumber6);
      this.Locations[36].Providers.Add(ore3);
      this.Locations[37].Providers.Add(lumber6);

      // Side 5
      this.Locations[38].Providers.Add(wool12);
      this.Locations[39].Providers.Add(wool12);
      this.Locations[39].Providers.Add(grain9);
      this.Locations[40].Providers.Add(wool12);
      this.Locations[40].Providers.Add(grain9);
      this.Locations[40].Providers.Add(brick5);
      this.Locations[41].Providers.Add(grain9);
      this.Locations[41].Providers.Add(brick5);
      this.Locations[41].Providers.Add(wool10);
      this.Locations[42].Providers.Add(brick5);
      this.Locations[42].Providers.Add(wool10);
      this.Locations[42].Providers.Add(lumber4);
      this.Locations[43].Providers.Add(wool10);
      this.Locations[43].Providers.Add(lumber4);
      this.Locations[43].Providers.Add(grain8);
      this.Locations[44].Providers.Add(lumber4);
      this.Locations[44].Providers.Add(grain8);
      this.Locations[44].Providers.Add(ore3);
      this.Locations[45].Providers.Add(grain8);
      this.Locations[45].Providers.Add(ore3);
      this.Locations[46].Providers.Add(ore3);

      // Side 6
      this.Locations[47].Providers.Add(grain9);
      this.Locations[48].Providers.Add(grain9);
      this.Locations[49].Providers.Add(grain9);
      this.Locations[49].Providers.Add(wool10);
      this.Locations[50].Providers.Add(wool10);
      this.Locations[51].Providers.Add(wool10);
      this.Locations[51].Providers.Add(grain8);
      this.Locations[52].Providers.Add(grain8);
      this.Locations[53].Providers.Add(grain8);
    }

    private void StitchLocationsTogetherUsingHorizontalTrails(Int32 index)
    {
      // Add horizontal trails for columns
      foreach (var setup in new[] {
            new HorizontalTrailSetup(0, 4, 8),
            new HorizontalTrailSetup(7, 5, 10),
            new HorizontalTrailSetup(16, 6, 11),
            new HorizontalTrailSetup(28, 5, 10),
            new HorizontalTrailSetup(39, 4, 8) })
      {
        var count = setup.TrailCount;
        var locationIndex = setup.LocationIndexStart;
        while (count-- > 0)
        {
          var location1 = this.Locations[locationIndex];
          var location2 = this.Locations[locationIndex + setup.LocationIndexDiff];
          var trail = new Trail(location1, location2);
          this.Trails[index++] = trail;
          location1.AddTrail(trail);
          location2.AddTrail(trail);

          locationIndex += 2;
        }
      }
    }

    private Int32 StitchLocationsTogetherUsingVerticalTrails()
    {
      var index = 0;
      var locationIndex = 0;
      foreach (var trailCount in new[] { 6, 8, 10, 10, 8, 6 })
      {
        var count = trailCount;
        while (count-- > 0)
        {
          var location1 = this.Locations[locationIndex];
          var location2 = this.Locations[locationIndex + 1];
          var trail = new Trail(location1, location2);
          this.Trails[index++] = trail;
          location1.AddTrail(trail);
          location2.AddTrail(trail);

          locationIndex++;
        }

        locationIndex++;
      }

      return index;
    }

    private void ConnectLocationsHorizontally()
    {
      // Add horizontal trails for columns
      foreach (var setup in new[] {
            new HorizontalTrailSetup(0, 4, 8),
            new HorizontalTrailSetup(7, 5, 10),
            new HorizontalTrailSetup(16, 6, 11),
            new HorizontalTrailSetup(28, 5, 10),
            new HorizontalTrailSetup(39, 4, 8) })
      {
        var count = setup.TrailCount;
        var startIndex = setup.LocationIndexStart;
        while (count-- > 0)
        {
          var endIndex = startIndex + setup.LocationIndexDiff;
          this.connections[startIndex, endIndex] = this.connections[endIndex, startIndex] = true;
          startIndex += 2;
        }
      }
    }

    private void ConnectLocationsVertically()
    {
      var startIndex = -1;
      foreach (var trailCount in new[] { 6, 8, 10, 10, 8, 6 })
      {
        var count = trailCount;
        startIndex++;
        var endIndex = startIndex + 1;
        while (count-- > 0)
        {
          this.connections[startIndex, endIndex] = this.connections[endIndex, startIndex] = true;
          startIndex++;
          endIndex++;
        }
      }
    }
    #endregion

    #region Structs
    public struct SettlementPlacementVerificationResults
    {
      public VerificationResults Status;
      public Guid PlayerId;
      public UInt32 LocationIndex;
    }

    private struct HorizontalTrailSetup
    {
      public Int32 LocationIndexStart;
      public Int32 TrailCount;
      public Int32 LocationIndexDiff;

      public HorizontalTrailSetup(Int32 locationIndexStart, Int32 trailCount, Int32 locationIndexDiff)
      {
        this.LocationIndexStart = locationIndexStart;
        this.TrailCount = trailCount;
        this.LocationIndexDiff = locationIndexDiff;
      }
    }
    #endregion
  }
}
