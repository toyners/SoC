
namespace Jabberwocky.SoC.Library.GameBoards
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Xml;

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
    [Obsolete]
    public Location[] Locations;
    [Obsolete]
    public Trail[] Trails;
    [Obsolete]
    public OldResourceProvider[] Providers;

    public const Int32 StandardBoardLocationCount = 54;
    public const Int32 StandardBoardTrailCount = 72;
    public const Int32 StandardBoardHexCount = 19;
    private ResourceProducer[] hexes;
    private Dictionary<UInt32, Guid> settlements;
    private Dictionary<Guid, List<UInt32>> settlementsByPlayer;
    private Boolean[,] connections;
    private Dictionary<Road, Guid> roads;
    private Dictionary<UInt32, ResourceProducer[]> resourceProvidersByDiceRolls;
    private Dictionary<ResourceProducer, UInt32[]> locationsForResourceProvider;
    private Dictionary<UInt32, UInt32[]> locationsForHex;
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

      this.CreateHexes();

      this.CreateLocations();

      this.Trails = new Trail[StandardBoardTrailCount];

      this.connections = new Boolean[GameBoardData.StandardBoardLocationCount, GameBoardData.StandardBoardLocationCount];
      this.ConnectLocationsVertically();
      this.ConnectLocationsHorizontally();

      var index = this.StitchLocationsTogetherUsingVerticalTrails();

      this.StitchLocationsTogetherUsingHorizontalTrails(index);

      this.ObsoleteCreateResourceProviders();

      this.AssignResourceProvidersToDiceRolls();

      this.AssignLocationsToResourceProviders();

      this.CreateLocationsForHex();
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

    /// <summary>
    /// Get list of player ids that have settlements on the hex. Duplicates are ignored.
    /// </summary>
    /// <param name="hex">Index of location hex.</param>
    /// <returns>List of player ids.</returns>
    public Guid[] GetPlayersForHex(UInt32 hex)
    {
      List<Guid> players = null;

      // Get players for each settlement
      foreach (var locationIndex in this.locationsForHex[hex])
      {
        if (!this.settlements.ContainsKey(locationIndex))
        {
          continue;
        }

        if (players == null)
        {
          players = new List<Guid>();
        }

        var playerId = this.settlements[locationIndex];
        if (players.Contains(playerId))
        {
          continue;
        }

        players.Add(playerId);
      }

      return players != null ? players.ToArray() : null;
    }

    public ResourceClutch GetResourcesForLocation(UInt32 location)
    {
      var resourceClutch = new ResourceClutch();
      var resourceProviders = this.Locations[location].Providers;

      foreach (var resourceProvider in resourceProviders)
      {
        switch (resourceProvider.Type)
        {
          case ResourceTypes.Brick: resourceClutch.BrickCount++; break;
          case ResourceTypes.Grain: resourceClutch.GrainCount++; break;
          case ResourceTypes.Lumber: resourceClutch.LumberCount++; break;
          case ResourceTypes.Ore: resourceClutch.OreCount++; break;
          case ResourceTypes.Wool: resourceClutch.WoolCount++; break;
        }
      }

      return resourceClutch;
    }

    public Dictionary<Guid, ResourceClutch> GetResourcesForRoll(UInt32 diceRoll)
    {
      var resources = new Dictionary<Guid, ResourceClutch>();

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

          ResourceClutch existingResourceCounts;

          // Got an owner - add to the resource count for the owner.
          if (resources.ContainsKey(owner))
          {
            existingResourceCounts = resources[owner];
          }
          else
          {
            existingResourceCounts = new ResourceClutch();
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

    /// <summary>
    /// Load the board data from stream.
    /// </summary>
    /// <param name="stream">Stream containing board data.</param>
    public void Load(Stream stream)
    {
      try
      {
        using (var reader = XmlReader.Create(stream, new XmlReaderSettings { CloseInput = false, IgnoreWhitespace = true, IgnoreComments = true }))
        {
          this.roads.Clear();
          this.settlements.Clear();

          while (!reader.EOF)
          {
            if (reader.Name == "board" && reader.NodeType == XmlNodeType.EndElement)
            {
              break;
            }

            if (reader.Name == "resources" && reader.NodeType == XmlNodeType.Element)
            {
              this.LoadHexResources(reader);
            }

            if (reader.Name == "production" && reader.NodeType == XmlNodeType.Element)
            {
              this.LoadHexProduction(reader);
            }

            if (reader.Name == "settlement")
            {
              var playerId = Guid.Parse(reader.GetAttribute("playerid"));
              var location = UInt32.Parse(reader.GetAttribute("location"));

              this.settlements.Add(location, playerId);
            }

            if (reader.Name == "road")
            {
              var playerId = Guid.Parse(reader.GetAttribute("playerid"));
              var start = UInt32.Parse(reader.GetAttribute("start"));
              var end = UInt32.Parse(reader.GetAttribute("end"));

              this.roads.Add(new Road(start, end), playerId);
            }

            reader.Read();
          }
        }
      }
      catch (Exception e)
      {
        throw new Exception("Ëxception thrown during board loading.", e);
      }
    }

    private void AddLocationsToHex(UInt32 lhs, UInt32 rhs, UInt32 hexIndex, UInt32 count)
    {
      var lastIndex = hexIndex + count - 1;
      for (; hexIndex <= lastIndex; hexIndex++)
      {
        var locations = new UInt32[6];
        locations[0] = lhs;
        locations[1] = lhs + 1;
        locations[2] = lhs + 2;
        locations[3] = rhs;
        locations[4] = rhs + 1;
        locations[5] = rhs + 2;

        lhs += 2;
        rhs += 2;

        this.locationsForHex.Add(hexIndex, locations);
      }
    }

    private void CreateHexes()
    {
      this.hexes = new ResourceProducer[StandardBoardHexCount];
      //d,b8,o5
      this.hexes[0] = new ResourceProducer { Type = ResourceTypes.None, Production = 0u };
      this.hexes[1] = new ResourceProducer { Type = ResourceTypes.Brick, Production = 8u };
      this.hexes[2] = new ResourceProducer { Type = ResourceTypes.Ore, Production = 5u };

      //b4,l3,w10,g2,
      this.hexes[3] = new ResourceProducer { Type = ResourceTypes.Brick, Production = 4u };
      this.hexes[4] = new ResourceProducer { Type = ResourceTypes.Lumber, Production = 3u };
      this.hexes[5] = new ResourceProducer { Type = ResourceTypes.Wool, Production = 10u };
      this.hexes[6] = new ResourceProducer { Type = ResourceTypes.Grain, Production = 2u };

      //l11,o6,g11,w9,l6,
      this.hexes[7] = new ResourceProducer { Type = ResourceTypes.Lumber, Production = 11u };
      this.hexes[8] = new ResourceProducer { Type = ResourceTypes.Ore, Production = 6u };
      this.hexes[9] = new ResourceProducer { Type = ResourceTypes.Grain, Production = 11u };
      this.hexes[10] = new ResourceProducer { Type = ResourceTypes.Wool, Production = 9u };
      this.hexes[11] = new ResourceProducer { Type = ResourceTypes.Lumber, Production = 6u };

      //w12,b5,l4,o3
      this.hexes[12] = new ResourceProducer { Type = ResourceTypes.Wool, Production = 12u };
      this.hexes[13] = new ResourceProducer { Type = ResourceTypes.Brick, Production = 5u };
      this.hexes[14] = new ResourceProducer { Type = ResourceTypes.Lumber, Production = 4u };
      this.hexes[15] = new ResourceProducer { Type = ResourceTypes.Ore, Production = 3u };

      //g9,w10 (see above),g8
      this.hexes[16] = new ResourceProducer { Type = ResourceTypes.Grain, Production = 9u };
      this.hexes[17] = new ResourceProducer { Type = ResourceTypes.Wool, Production = 10u };
      this.hexes[18] = new ResourceProducer { Type = ResourceTypes.Grain, Production = 8u };
    }

    private void CreateLocations()
    {
      this.Locations = new Location[StandardBoardLocationCount];
      for (var index = 0; index < StandardBoardLocationCount; index++)
      {
        this.Locations[index] = new Location();
      }
    }

    private void LoadHexResources(XmlReader reader)
    {
      var resources = reader.ReadElementContentAsString();
      var index = 0;
      foreach (var resource in resources)
      {
        switch (resource)
        {
          case 'b':
          this.hexes[index++].Type = ResourceTypes.Brick;
          break;
          case 'g':
          this.hexes[index++].Type = ResourceTypes.Grain;
          break;
          case 'l':
          this.hexes[index++].Type = ResourceTypes.Lumber;
          break;
          case 'o':
          this.hexes[index++].Type = ResourceTypes.Ore;
          break;
          case 'w':
          this.hexes[index++].Type = ResourceTypes.Wool;
          break;
          case ' ':
          this.hexes[index++].Type = ResourceTypes.None;
          break;
        }
      }
    }

    private void LoadHexProduction(XmlReader reader)
    {
      var productionValues = reader.ReadElementContentAsString().Split(',');
      var index = 0;
      foreach (var productionValue in productionValues)
      {
        this.hexes[index++].Production = UInt32.Parse(productionValue);
      }
    }

    private void ObsoleteCreateResourceProviders()
    {
      //d,b8,o5
      var desert = new OldResourceProvider();
      var brick8 = new OldResourceProvider(ResourceTypes.Brick, 8);
      var ore5 = new OldResourceProvider(ResourceTypes.Ore, 5);

      //b4,l3,w10,g2,
      var brick4 = new OldResourceProvider(ResourceTypes.Brick, 4);
      var lumber3 = new OldResourceProvider(ResourceTypes.Lumber, 3);
      var wool10 = new OldResourceProvider(ResourceTypes.Wool, 10);
      var grain2 = new OldResourceProvider(ResourceTypes.Grain, 2);

      //l11,o6,g11,w9,l6,
      var lumber11 = new OldResourceProvider(ResourceTypes.Lumber, 11);
      var ore6 = new OldResourceProvider(ResourceTypes.Ore, 6);
      var grain11 = new OldResourceProvider(ResourceTypes.Grain, 11);
      var wool9 = new OldResourceProvider(ResourceTypes.Wool, 9);
      var lumber6 = new OldResourceProvider(ResourceTypes.Lumber, 6);

      //w12,b5,l4,o3
      var wool12 = new OldResourceProvider(ResourceTypes.Wool, 12);
      var brick5 = new OldResourceProvider(ResourceTypes.Brick, 5);
      var lumber4 = new OldResourceProvider(ResourceTypes.Lumber, 4);
      var ore3 = new OldResourceProvider(ResourceTypes.Ore, 3);

      //g9,w10,g8
      var grain9 = new OldResourceProvider(ResourceTypes.Grain, 9);
      var grain8 = new OldResourceProvider(ResourceTypes.Grain, 8);

      // Load the resource provider array
      this.Providers = new OldResourceProvider[StandardBoardHexCount];
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

    public Tuple<ResourceTypes, UInt32>[] GetHexInformation()
    {
      var data = new Tuple<ResourceTypes, UInt32>[this.hexes.Length];
      for (var index = 0; index < this.hexes.Length; index++)
      {
        data[index] = new Tuple<ResourceTypes, uint>(this.hexes[index].Type, this.hexes[index].Production);
      }

      return data;
    }

    public Dictionary<UInt32, Guid> GetSettlementInformation()
    {
      var data = new Dictionary<UInt32, Guid>(this.settlements.Count);
      foreach (var kv in this.settlements)
      {
        data.Add(kv.Key, kv.Value);
      }

      return data;
    }

    public Tuple<Road, Guid>[] GetRoadInformation()
    {
      var data = new Tuple<Road, Guid>[this.roads.Count];
      var index = 0;
      foreach (var kv in this.roads)
      {
        data[index++] = new Tuple<Road, Guid>(kv.Key, kv.Value);
      }

      return data;
    }

    private void AssignResourceProvidersToDiceRolls()
    {
      var tempResourceProvidersByDiceRolls = new List<ResourceProducer>[13];

      tempResourceProvidersByDiceRolls[2] = new List<ResourceProducer>();
      tempResourceProvidersByDiceRolls[3] = new List<ResourceProducer>();
      tempResourceProvidersByDiceRolls[4] = new List<ResourceProducer>();
      tempResourceProvidersByDiceRolls[5] = new List<ResourceProducer>();
      tempResourceProvidersByDiceRolls[6] = new List<ResourceProducer>();
      tempResourceProvidersByDiceRolls[8] = new List<ResourceProducer>();
      tempResourceProvidersByDiceRolls[9] = new List<ResourceProducer>();
      tempResourceProvidersByDiceRolls[10] = new List<ResourceProducer>();
      tempResourceProvidersByDiceRolls[11] = new List<ResourceProducer>();
      tempResourceProvidersByDiceRolls[12] = new List<ResourceProducer>();

      foreach (var hex in this.hexes)
      {
        if (hex.Production == 0)
        {
          continue;
        }

        tempResourceProvidersByDiceRolls[hex.Production].Add(hex);
      }

      this.resourceProvidersByDiceRolls = new Dictionary<UInt32, ResourceProducer[]>();

      for (UInt32 diceRoll = 2; diceRoll <= 12; diceRoll++)
      {
        if (diceRoll == 7)
        {
          continue;
        }

        this.resourceProvidersByDiceRolls.Add(diceRoll, tempResourceProvidersByDiceRolls[diceRoll].ToArray());
      }
    }

    private void AssignLocationsToResourceProviders()
    {
      this.locationsForResourceProvider = new Dictionary<ResourceProducer, UInt32[]>();

      // Column 1
      UInt32 lhs = 2;
      UInt32 rhs = 10;
      Int32 hexIndex = 0;
      for (; hexIndex < 3; hexIndex++)
      {
        var resourceProvider = this.hexes[hexIndex];
        var locations = new UInt32[] { lhs, lhs + 1, lhs + 2, rhs, rhs + 1, rhs + 2 };
        lhs = lhs + 2;
        rhs = rhs + 2;

        this.locationsForResourceProvider.Add(resourceProvider, locations);
      }

      // Column 2
      lhs = 7;
      rhs = 17;
      for (; hexIndex < 7; hexIndex++)
      {
        var resourceProvider = this.hexes[hexIndex];
        var locations = new UInt32[] { lhs, lhs + 1, lhs + 2, rhs, rhs + 1, rhs + 2 };
        lhs = lhs + 2;
        rhs = rhs + 2;

        this.locationsForResourceProvider.Add(resourceProvider, locations);
      }

      // Column 3
      lhs = 16;
      rhs = 27;
      for (; hexIndex < 12; hexIndex++)
      {
        var resourceProvider = this.hexes[hexIndex];
        var locations = new UInt32[] { lhs, lhs + 1, lhs + 2, rhs, rhs + 1, rhs + 2 };
        lhs = lhs + 2;
        rhs = rhs + 2;

        this.locationsForResourceProvider.Add(resourceProvider, locations);
      }

      // Column 4
      lhs = 28;
      rhs = 38;
      for (; hexIndex < 16; hexIndex++)
      {
        var resourceProvider = this.hexes[hexIndex];
        var locations = new UInt32[] { lhs, lhs + 1, lhs + 2, rhs, rhs + 1, rhs + 2 };
        lhs = lhs + 2;
        rhs = rhs + 2;

        this.locationsForResourceProvider.Add(resourceProvider, locations);
      }

      // Column 5
      lhs = 39;
      rhs = 47;
      for (; hexIndex < 19; hexIndex++)
      {
        var resourceProvider = this.hexes[hexIndex];
        var locations = new UInt32[] { lhs, lhs + 1, lhs + 2, rhs, rhs + 1, rhs + 2 };
        lhs = lhs + 2;
        rhs = rhs + 2;

        this.locationsForResourceProvider.Add(resourceProvider, locations);
      }
    }

    private void CreateLocationsForHex()
    {
      this.locationsForHex = new Dictionary<UInt32, UInt32[]>();

      this.AddLocationsToHex(0u, 8u, 0u, 3u);
      this.AddLocationsToHex(7u, 17u, 3u, 4u);
      this.AddLocationsToHex(16u, 27u, 7u, 5u);
      this.AddLocationsToHex(28u, 38u, 12u, 4u);
      this.AddLocationsToHex(39u, 47u, 16u, 3u);
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
    private class ResourceProducer
    {
      public ResourceTypes Type;
      public UInt32 Production;
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
