
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
    public enum VerificationStatus
    {
      Valid,
      LocationIsOccupied,
      LocationIsInvalid,
      NotConnectedToExisting,
      NoSettlementToUpgrade,
      TooCloseToSettlement,
      RoadConnectsToAnotherPlayer,
      RoadIsOffBoard,
      RoadIsOccupied,
      NoDirectConnection
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
    private Dictionary<Road, Guid> roads;

    // Location and resources bordering it
    private Dictionary<UInt32, ResourceProvider2[]> resourcesAtLocation;

    private Dictionary<UInt32, ResourceProvider2[]> resourceProvidersByDiceRolls;
    #endregion

    #region Construction
    public GameBoardData(BoardSizes size)
    {
      if (size == BoardSizes.Extended)
      {
        throw new Exception("Extended boards not implemented.");
      }

      this.settlements = new Dictionary<UInt32, Guid>();
      this.roads = new Dictionary<Road, Guid>();
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

      this.CreateResourceProviders2();
    }

    private void CreateResourceProviders2()
    {
      //d,b8,o5
      var brick8 = new ResourceProvider2(ResourceTypes.Brick);
      var ore5 = new ResourceProvider2(ResourceTypes.Ore);

      //b4,l3,w10,g2,
      var brick4 = new ResourceProvider2(ResourceTypes.Brick);
      var lumber3 = new ResourceProvider2(ResourceTypes.Lumber);
      var wool10_a = new ResourceProvider2(ResourceTypes.Wool);
      var grain2 = new ResourceProvider2(ResourceTypes.Grain);

      //l11,o6,g11,w9,l6,
      var lumber11 = new ResourceProvider2(ResourceTypes.Lumber);
      var ore6 = new ResourceProvider2(ResourceTypes.Ore);
      var grain11 = new ResourceProvider2(ResourceTypes.Grain);
      var wool9 = new ResourceProvider2(ResourceTypes.Wool);
      var lumber6 = new ResourceProvider2(ResourceTypes.Lumber);

      //w12,b5,l4,o3
      var wool12 = new ResourceProvider2(ResourceTypes.Wool);
      var brick5 = new ResourceProvider2(ResourceTypes.Brick);
      var lumber4 = new ResourceProvider2(ResourceTypes.Lumber);
      var ore3 = new ResourceProvider2(ResourceTypes.Ore);

      //g9,w10 (see above),g8
      var grain9 = new ResourceProvider2(ResourceTypes.Grain);
      var wool10_b = new ResourceProvider2(ResourceTypes.Wool);
      var grain8 = new ResourceProvider2(ResourceTypes.Grain);

      var tempResourceProvidersByDiceRolls = new List<ResourceProvider2>[13];

      tempResourceProvidersByDiceRolls[2] = new List<ResourceProvider2> { grain2 };
      tempResourceProvidersByDiceRolls[3] = new List<ResourceProvider2> { lumber3, ore3 };
      tempResourceProvidersByDiceRolls[4] = new List<ResourceProvider2> { brick4, lumber4 };
      tempResourceProvidersByDiceRolls[5] = new List<ResourceProvider2> { ore5, brick5 };
      tempResourceProvidersByDiceRolls[6] = new List<ResourceProvider2> { ore6, lumber6 };
      tempResourceProvidersByDiceRolls[8] = new List<ResourceProvider2> { brick8, grain8 };
      tempResourceProvidersByDiceRolls[9] = new List<ResourceProvider2> { wool9, grain9 };
      tempResourceProvidersByDiceRolls[10] = new List<ResourceProvider2> { wool10_a, wool10_b };
      tempResourceProvidersByDiceRolls[11] = new List<ResourceProvider2> { lumber11, grain11 };
      tempResourceProvidersByDiceRolls[12] = new List<ResourceProvider2> { wool12 };

      this.resourceProvidersByDiceRolls = new Dictionary<UInt32, ResourceProvider2[]>();

      for (UInt32 diceRoll = 2; diceRoll <= 12; diceRoll++) 
      {
        if (diceRoll == 7)
        {
          continue;
        }

        this.resourceProvidersByDiceRolls.Add(diceRoll, tempResourceProvidersByDiceRolls[diceRoll].ToArray());
      }

      this.locationsForResourceProvider = new Dictionary<ResourceProvider2, UInt32[]>();

      // Column 1
      var locationsForBrick8 = new UInt32[] { 2, 3, 4, 10, 11, 12 };
      this.locationsForResourceProvider.Add(brick8, locationsForBrick8);

      var locationsForOre5 = new UInt32[] { 4, 5, 6, 12, 13, 14 };
      this.locationsForResourceProvider.Add(ore5, locationsForOre5);

      // Column 2
      UInt32 lhs = 7;
      UInt32 rhs = 17;
      foreach (var resourceProvider in new[] { brick4, lumber3, wool10_a, grain2 })
      {
        var locations = new UInt32[] { lhs, lhs + 1, lhs + 2, rhs, rhs + 1, rhs + 2 };
        lhs = lhs + 2;
        rhs = rhs + 2;

        this.locationsForResourceProvider.Add(resourceProvider, locations);
      }

      // Column 3
      lhs = 16;
      rhs = 27;
      foreach (var resourceProvider in new[] { lumber11, ore6, grain11, wool9, lumber6 })
      {
        var locations = new UInt32[] { lhs, lhs + 1, lhs + 2, rhs, rhs + 1, rhs + 2 };
        lhs = lhs + 2;
        rhs = rhs + 2;

        this.locationsForResourceProvider.Add(resourceProvider, locations);
      }

      // Column 4
      lhs = 28;
      rhs = 38;
      foreach (var resourceProvider in new[] { wool12, brick5, lumber4, ore3 })
      {
        var locations = new UInt32[] { lhs, lhs + 1, lhs + 2, rhs, rhs + 1, rhs + 2 };
        lhs = lhs + 2;
        rhs = rhs + 2;

        this.locationsForResourceProvider.Add(resourceProvider, locations);
      }

      // Column 5
      lhs = 39;
      rhs = 47;
      foreach (var resourceProvider in new[] { grain9, wool10_b, grain8 })
      {
        var locations = new UInt32[] { lhs, lhs + 1, lhs + 2, rhs, rhs + 1, rhs + 2 };
        lhs = lhs + 2;
        rhs = rhs + 2;

        this.locationsForResourceProvider.Add(resourceProvider, locations);
      }
    }
    #endregion

    #region Methods
    public VerificationStatus CanPlaceCity(PlayerData player, Location location)
    {
      throw new NotImplementedException();
    }

    public VerificationResults CanPlaceRoad(Guid playerId, Road road)
    {
      // Verify #1 - Are both road locations on the board.
      var length = (UInt32)this.connections.GetLength(0);
      if (road.Location1 >= length || road.Location2 >= length)
      {
        return new VerificationResults { Status = VerificationStatus.RoadIsOffBoard };
      }

      // Verify #2 - Is direct connection possible
      if (!this.connections[road.Location1, road.Location2])
      {
        return new VerificationResults { Status = VerificationStatus.NoDirectConnection };
      }

      // Verify #3 - Is there already a road built
      if (this.roads.ContainsKey(road))
      {
        return new VerificationResults { Status = VerificationStatus.RoadIsOccupied };
      }

      // Verify #4 - Does it connect to existing infrastructure
      if (!this.settlements.ContainsKey(road.Location1) && !this.settlements.ContainsKey(road.Location2))
      {
        var isConnected = false;

        // Linear scan but the total number of possible roads is in the tens
        foreach (var existingRoad in this.roads.Keys)
        {
          if (road.Location1 == existingRoad.Location1 || road.Location1 == existingRoad.Location2 ||
              road.Location2 == existingRoad.Location1 || road.Location2 == existingRoad.Location2)
          {
            isConnected = true;
            break;
          }
        }
        
        if (!isConnected)
        {
          return new VerificationResults { Status = VerificationStatus.NotConnectedToExisting };
        }
      }

      // Verify #5 - Does it connect to another players settlement
      if (this.settlements.ContainsKey(road.Location1) && this.settlements[road.Location1] != playerId)
      {
        return new VerificationResults { Status = VerificationStatus.RoadConnectsToAnotherPlayer };
      }

      if (this.settlements.ContainsKey(road.Location2) && this.settlements[road.Location2] != playerId)
      {
        return new VerificationResults { Status = VerificationStatus.RoadConnectsToAnotherPlayer };
      }

      return new VerificationResults { Status = VerificationStatus.Valid };
    }

    public VerificationResults CanPlaceSettlement(UInt32 locationIndex)
    {
      if (locationIndex >= this.connections.GetLength(0))
      {
        return new VerificationResults
        {
          Status = VerificationStatus.LocationIsInvalid
        };
      }

      if (this.settlements.ContainsKey(locationIndex))
      {
        return new VerificationResults
        {
          Status = VerificationStatus.LocationIsOccupied,
          LocationIndex = locationIndex,
          PlayerId = this.settlements[locationIndex]
        };
      }

      for (UInt32 index = 0; index < this.connections.GetLength(1); index++)
      {
        if (this.connections[locationIndex, index] && this.settlements.ContainsKey(index))
        {
          return new VerificationResults
          {
            Status = VerificationStatus.TooCloseToSettlement,
            LocationIndex = index,
            PlayerId = this.settlements[index]
          };
        }
      }

      return new VerificationResults
      {
        Status = VerificationStatus.Valid,
        LocationIndex = 0u,
        PlayerId = Guid.Empty
      };
    }

    public VerificationResults CanPlaceStartingInfrastructure(Guid playerId, UInt32 settlementIndex, Road road)
    {
      var results = this.CanPlaceSettlement(settlementIndex);
      if (results.Status != VerificationStatus.Valid)
      {
        return results;
      }

      results = this.CanPlaceRoad(playerId, road);
      if (results.Status == VerificationStatus.NotConnectedToExisting)
      {
        if (road.Location1 == settlementIndex || road.Location2 == settlementIndex)
        {
          return new VerificationResults { Status = VerificationStatus.Valid };
        }
      }

      return results;
    }

    public List<UInt32> GetPathBetweenLocations(UInt32 startIndex, UInt32 endIndex)
    {
      if (startIndex == endIndex)
      {
        return null;
      }

      return PathFinder.GetPathBetweenPoints(startIndex, endIndex, this.connections);
    }

    public List<UInt32> GetSettlementsForPlayer(Guid playerId)
    {
      if (!this.settlementsByPlayer.ContainsKey(playerId))
      {
        return null;
      }

      return this.settlementsByPlayer[playerId];
    }

    public void PlaceStartingInfrastructure(Guid playerId, UInt32 settlementIndex, Road road)
    {
      this.PlaceSettlement(playerId, settlementIndex);
      this.PlaceRoad(playerId, road);
    }

    public void PlaceRoad(Guid playerId, Road road)
    {
      this.roads.Add(road, playerId);
    }

    public void PlaceSettlement(Guid playerId, UInt32 locationIndex)
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

    public ResourceCounts GetResourcesForLocation(UInt32 location)
    {
      var resourceCounts = new ResourceCounts();
      var resourceProviders = this.Locations[location].Providers;

      foreach (var resourceProvider in resourceProviders)
      {
        switch (resourceProvider.Type)
        {
          case ResourceTypes.Brick: resourceCounts.BrickCount++; break;
          case ResourceTypes.Grain: resourceCounts.GrainCount++; break;
          case ResourceTypes.Lumber: resourceCounts.LumberCount++; break;
          case ResourceTypes.Ore: resourceCounts.OreCount++; break;
          case ResourceTypes.Wool: resourceCounts.WoolCount++; break;
        }
      }

      return resourceCounts;
    }

    private Dictionary<ResourceProvider2, UInt32[]> locationsForResourceProvider;

    public Dictionary<Guid, ResourceCounts> GetResourcesForRoll(UInt32 diceRoll)
    {
      var resources = new Dictionary<Guid, ResourceCounts>();

      // Iterate over all the resource providers that match the dice roll
      foreach (var resourceProvider in this.resourceProvidersByDiceRolls[diceRoll])
      {
        // Iterate over all the locations bordering the resource provider
        foreach (var location in this.locationsForResourceProvider[resourceProvider])
        {
          if (!this.settlements.ContainsKey(location))
          {
            // No owner of the location
            continue;
          }

          var owner = this.settlements[location];

          ResourceCounts existingResourceCounts;

          // Got an owner - add to the resource count for the owner.
          if (resources.ContainsKey(owner))
          {
            existingResourceCounts = resources[owner];
          }
          else
          {
            existingResourceCounts = new ResourceCounts();
            resources.Add(owner, existingResourceCounts);
          }

          switch (resourceProvider.Type)
          {
            case ResourceTypes.Brick: existingResourceCounts.BrickCount += 1; break;
            case ResourceTypes.Grain: existingResourceCounts.GrainCount += 1; break;
            case ResourceTypes.Lumber: existingResourceCounts.LumberCount += 1; break;
            case ResourceTypes.Ore: existingResourceCounts.OreCount += 1; break;
            case ResourceTypes.Wool: existingResourceCounts.WoolCount += 1; break;
          }

          resources[owner] = existingResourceCounts;
        }
      }

      return resources;
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
    public struct ResourceCounts
    {
      public UInt32 BrickCount;
      public UInt32 GrainCount;
      public UInt32 LumberCount;
      public UInt32 OreCount;
      public UInt32 WoolCount;
    }

    private class ResourceProvider2
    {
      public readonly ResourceTypes Type;

      public ResourceProvider2(ResourceTypes type)
      {
        this.Type = type;
      }
    }

    public struct VerificationResults
    {
      public VerificationStatus Status;
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
