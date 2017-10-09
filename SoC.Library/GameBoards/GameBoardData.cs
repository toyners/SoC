
namespace Jabberwocky.SoC.Library.GameBoards
{
  using System;
  using System.Collections.Generic;
  using System.Diagnostics;
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
    public const Int32 StandardBoardLocationCount = 54;
    public const Int32 StandardBoardTrailCount = 72;
    public const Int32 StandardBoardHexCount = 19;
    private ResourceProducer[] hexes;
    private Dictionary<UInt32, Guid> settlements;
    private Dictionary<Guid, List<UInt32>> settlementsByPlayer;
    private Boolean[,] connections;
    private List<RoadSegment> roadSegments;
    private Dictionary<Guid, List<RoadSegment>> roadsByPlayer;
    private Dictionary<UInt32, ResourceProducer[]> resourceProvidersByDiceRolls;
    private Dictionary<ResourceProducer, UInt32[]> locationsForResourceProvider;
    private Dictionary<UInt32, UInt32[]> locationsForHex;
    private Dictionary<UInt32, UInt32[]> hexesForLocations;
    private RoadNode[] roadNodes;
    #endregion

    #region Construction
    public GameBoardData(BoardSizes size)
    {
      if (size == BoardSizes.Extended)
      {
        throw new Exception("Extended boards not implemented.");
      }

      this.Length = StandardBoardHexCount;
      this.settlements = new Dictionary<UInt32, Guid>();
      this.roadSegments = new List<RoadSegment>();
      this.roadsByPlayer = new Dictionary<Guid, List<RoadSegment>>();
      this.settlementsByPlayer = new Dictionary<Guid, List<UInt32>>();
      this.roadNodes = new RoadNode[StandardBoardLocationCount];

      this.CreateHexes();

      this.connections = new Boolean[GameBoardData.StandardBoardLocationCount, GameBoardData.StandardBoardLocationCount];
      this.ConnectLocationsVertically();
      this.ConnectLocationsHorizontally();

      this.AssignResourceProvidersToDiceRolls();

      this.AssignLocationsToResourceProviders();

      this.AssignLocationsToHex();

      this.AssignHexesToLocation();
    }
    #endregion

    #region Properties
    public UInt32 Length { get; private set; }
    #endregion

    #region Methods
    public VerificationStatus CanPlaceCity(PlayerData player, Location location)
    {
      throw new NotImplementedException();
    }

    public VerificationResults CanPlaceRoad(Guid playerId, UInt32 roadStartLocation, UInt32 roadEndLocation)
    {
      // Verify #1 - Are both road locations on the board.
      var length = (UInt32)this.connections.GetLength(0);
      if (roadStartLocation >= length || roadEndLocation >= length)
      {
        return new VerificationResults { Status = VerificationStatus.RoadIsOffBoard };
      }

      // Verify #2 - Is direct connection possible
      if (!this.connections[roadStartLocation, roadEndLocation])
      {
        return new VerificationResults { Status = VerificationStatus.NoDirectConnection };
      }

      // Verify #3 - Is there already a road built
      var roadNode = this.roadNodes[roadStartLocation];
      if (roadNode != null)
      {
        foreach (var otherEnd in roadNode.OtherEnds)
        {
          if (otherEnd.Item1 == roadEndLocation)
          {
            return new VerificationResults { Status = VerificationStatus.RoadIsOccupied };
          }
        }
      }

      var newRoadSegment = new RoadSegment(roadStartLocation, roadEndLocation);
      /*if (this.roadSegments.Contains(newRoadSegment))
      {
        return new VerificationResults { Status = VerificationStatus.RoadIsOccupied };
      }*/

      // Verify #4 - Does it connect to existing infrastructure
      if (!this.settlements.ContainsKey(roadStartLocation) && !this.settlements.ContainsKey(roadEndLocation))
      {
        var isConnected = false;

        // Linear scan but the total number of possible roads is in the tens
        foreach (var existingRoad in this.roadSegments)
        {
          if (newRoadSegment.Location1 == existingRoad.Location1 || newRoadSegment.Location1 == existingRoad.Location2 ||
              newRoadSegment.Location2 == existingRoad.Location1 || newRoadSegment.Location2 == existingRoad.Location2)
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
      if (this.settlements.ContainsKey(newRoadSegment.Location1) && this.settlements[newRoadSegment.Location1] != playerId)
      {
        return new VerificationResults { Status = VerificationStatus.RoadConnectsToAnotherPlayer };
      }

      if (this.settlements.ContainsKey(newRoadSegment.Location2) && this.settlements[newRoadSegment.Location2] != playerId)
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

    public VerificationResults CanPlaceStartingInfrastructure(Guid playerId, UInt32 settlementIndex, UInt32 roadEndLocation)
    {
      var results = this.CanPlaceSettlement(settlementIndex);
      if (results.Status != VerificationStatus.Valid)
      {
        return results;
      }

      // TODO: Rework this to use call the individual verification for road place so that 
      // the NotConnectedToExisting verification is not called (since it makes no sense here)
      // Also check that there is no connecting 

      results = this.CanPlaceRoad(playerId, settlementIndex, roadEndLocation);
      /*if (results.Status == VerificationStatus.NotConnectedToExisting)
      {
        if (road.Location1 == settlementIndex || road.Location2 == settlementIndex)
        {
          return new VerificationResults { Status = VerificationStatus.Valid };
        }
      }*/

      return results;
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

    public List<UInt32> GetPathBetweenLocations(UInt32 startIndex, UInt32 endIndex)
    {
      if (startIndex == endIndex)
      {
        return null;
      }

      return PathFinder.GetPathBetweenPoints(startIndex, endIndex, this.connections);
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

    /// <summary>
    /// Returns the list of production values of the resource producers that are available
    /// to the location.
    /// </summary>
    /// <param name="location">Index of the location to get production values for.</param>
    /// <returns>Array of production values.</returns>
    public UInt32[] GetProductionValuesForLocation(UInt32 location)
    {
      var hexesForLocation = this.hexesForLocations[location];
      var productionValues = new List<UInt32>();

      foreach (var hexIndex in hexesForLocation)
      {
        var productionValue = this.hexes[hexIndex].Production;
        if (productionValue > 0)
        {
          productionValues.Add(this.hexes[hexIndex].Production);
        }
      }

      return productionValues.ToArray();
    }

    public ResourceClutch GetResourcesForLocation(UInt32 location)
    {
      var resourceClutch = new ResourceClutch();
      var hexesForLocation = this.hexesForLocations[location];

      foreach (var hexIndex in hexesForLocation)
      {
        switch (this.hexes[hexIndex].Type)
        {
          case ResourceTypes.Brick:
          resourceClutch.BrickCount++;
          break;
          case ResourceTypes.Grain:
          resourceClutch.GrainCount++;
          break;
          case ResourceTypes.Lumber:
          resourceClutch.LumberCount++;
          break;
          case ResourceTypes.Ore:
          resourceClutch.OreCount++;
          break;
          case ResourceTypes.Wool:
          resourceClutch.WoolCount++;
          break;
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
            case ResourceTypes.Brick:
            existingResourceCounts.BrickCount += 1;
            break;
            case ResourceTypes.Grain:
            existingResourceCounts.GrainCount += 1;
            break;
            case ResourceTypes.Lumber:
            existingResourceCounts.LumberCount += 1;
            break;
            case ResourceTypes.Ore:
            existingResourceCounts.OreCount += 1;
            break;
            case ResourceTypes.Wool:
            existingResourceCounts.WoolCount += 1;
            break;
          }

          resources[owner] = existingResourceCounts;
        }
      }

      return resources;
    }

    public Tuple<UInt32, UInt32, Guid>[] GetRoadInformation()
    {
      var data = new Tuple<UInt32, UInt32, Guid>[this.roadSegments.Count];
      var index = 0;
      foreach (var kv in this.roadsByPlayer)
      {
        foreach (var roadSegment in kv.Value)
        {
          data[index++] = new Tuple<UInt32, UInt32, Guid>(roadSegment.Location1, roadSegment.Location2, kv.Key);
        }
      }

      return data;
    }

    public List<UInt32> GetSettlementsForPlayer(Guid playerId)
    {
      if (!this.settlementsByPlayer.ContainsKey(playerId))
      {
        return null;
      }

      return this.settlementsByPlayer[playerId];
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

    public void PlaceRoad(Guid playerId, UInt32 locationIndex1, UInt32 locationIndex2)
    {
      this.PlaceTheRoad(playerId, locationIndex1, locationIndex2);
    }

    private void PlaceTheRoad(Guid playerId, UInt32 locationIndex1, UInt32 locationIndex2)
    {
      var newRoadSegment = new RoadSegment(locationIndex1, locationIndex2);
      this.roadSegments.Add(newRoadSegment);

      if (!this.roadsByPlayer.ContainsKey(playerId))
      {
        var roadSegmentList = new List<RoadSegment>();
        roadSegmentList.Add(newRoadSegment);
        this.roadsByPlayer.Add(playerId, roadSegmentList);
      }
      else
      {
        this.roadsByPlayer[playerId].Add(newRoadSegment);
      }

      /*List<List<RoadSegment>> roadsForPlayer = null;
      if (!this.roadsByPlayer.ContainsKey(playerId))
      {
        roadsForPlayer = new List<List<RoadSegment>>();
        var newRoad = new List<RoadSegment>();
        newRoad.Add(newRoadSegment);
        roadsForPlayer.Add(newRoad);
        this.roadsByPlayer.Add(playerId, roadsForPlayer);
        return;
      }

      roadsForPlayer = this.roadsByPlayer[playerId];
      foreach (var road in roadsForPlayer)
      {
        var firstRoadSegment = road[0];
        
        if (firstRoadSegment.IsConnected(newRoadSegment))
        {
          if (road.Count == 1)
          {
            // Second road segment added to road
            road.Add(newRoadSegment);
          }
          else
          {

          }
        }

        if (road[0].IsConnected(newRoadSegment))
        {
          road.Insert(0, newRoadSegment);
        }
        else if (road[road.Count - 1].IsConnected(newRoadSegment))
        {
          road.Add(newRoadSegment);
        }
      }

      // Create new road*/
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
    /// Place starting infrastructure (settlement and connecting road segment).
    /// </summary>
    /// <param name="playerId">Id of player placing the infrastructure.</param>
    /// <param name="settlementIndex">Location to place settlement. Also the starting location of the road segment.</param>
    /// <param name="endIndex">End location of road segment.</param>
    public void PlaceStartingInfrastructure(Guid playerId, UInt32 settlementIndex, UInt32 endIndex)
    {
      var verificationResults = this.CanPlaceStartingInfrastructure(playerId, settlementIndex, endIndex);
      if (verificationResults.Status != VerificationStatus.Valid)
      {

      }

      this.PlaceSettlement(playerId, settlementIndex);
      this.PlaceRoad(playerId, settlementIndex, endIndex);
    }

    Dictionary<UInt32, HashSet<RoadSegment>> roadsByLocation = new Dictionary<UInt32, HashSet<RoadSegment>>();
    Dictionary<Guid, List<RoadSegment>> roadSegmentsByPlayer = new Dictionary<Guid, List<RoadSegment>>();
    Guid[,] roadss = new Guid[1,1];
    public Boolean TryGetLongestRoadDetails(out Guid playerId, out Int32 roadLength)
    {

      playerId = Guid.Empty;
      roadLength = 0;
      foreach (var kv in roadSegmentsByPlayer)
      {
        // Got all road segments for the player. Identify the end points
        var playerSegments = kv.Value;
        RoadSegment endPoint = null;
        var endPoints = new HashSet<RoadSegment>();
        foreach (var roadSegment in playerSegments)
        {
          if (roadSegment.ConnectedToLocation1.Count > 0 && roadSegment.ConnectedToLocation2.Count > 0)
          {
            // This segment is in the middle of the road i.e. has two neighbours
          }
          else if (roadSegment.ConnectedToLocation1.Count > 0 || roadSegment.ConnectedToLocation2.Count > 0)
          {
            // This is a road end i.e. only one neighbour
            if (endPoint == null)
            {
              endPoint = roadSegment;
            }
            else
            {
              endPoints.Add(roadSegment);
            }
          }
        }

        while (endPoints.Count > 0)
        {
          // Follow road from endpoint through to other endpoint
          var roadSegment = endPoint;
          var visited = new HashSet<RoadSegment>();
          var length = 0;
          var atEndOfRoad = false;
          RoadSegment nextSegment = null;
          while (!atEndOfRoad)
          {
            // Get nextSegment from this roadSegment
            if (roadSegment.ConnectedToLocation2.Count == 0)
            {
              // Location 2 on this road segment has no neighbours
              if (roadSegment.ConnectedToLocation1.Count == 1)
              {
                // Only one neighbour - continue moving along the road
                nextSegment = roadSegment.ConnectedToLocation2[0];
                length++;
              }
              else if (roadSegment.ConnectedToLocation1.Count > 1)
              {

              }
              else
              {
                endPoints.Remove(roadSegment);
                atEndOfRoad = true;
              }
            }
          }
          
          if (length > roadLength)
          {
            playerId = kv.Key;
            roadLength = length;
          }
          else if (length == roadLength)
          {
            playerId = Guid.Empty;
          }
        }
      }

      throw new NotImplementedException();
    }

    private void TryGetLongestRoadDetails1()
    {
      /*playerId = Guid.Empty;
      roadLength = -1;
      var visited = new HashSet<Road>();
      var roadsByPlayer = new Dictionary<Guid, List<Road>>();

      if (this.roadSegments.Count == 0)
      {
        return false;
      }

      foreach (var kv1 in this.roadsByLocation)
      {
        var location = kv1.Key;
        var roadsForLocation = kv1.Value;

        foreach (var road in roadsForLocation)
        {
          if (visited.Contains(road))
          {
            continue;
          }

          var id = this.roadSegments[road];
          List<Road> workingRoadList = null;
          if (!roadsByPlayer.ContainsKey(id))
          {
            // Road of length one
            workingRoadList = new List<Road>();
            workingRoadList.Add(road);
            roadsByPlayer.Add(id, workingRoadList);
          }
          else
          {
            workingRoadList = roadsByPlayer[id];
            var lastRoadSegment = workingRoadList[workingRoadList.Count - 1];
            if (lastRoadSegment.IsConnected(road))
            {
              workingRoadList.Add(road);
            }
          }
        }
      }

      var gotSingleLongestRoad = false;
      foreach (var kv2 in roadsByPlayer)
      {
        if (kv2.Value.Count > roadLength)
        {
          playerId = kv2.Key;
          roadLength = kv2.Value.Count;
          gotSingleLongestRoad = true;
        }
        else if (kv2.Value.Count == roadLength && kv2.Key != playerId)
        {
          gotSingleLongestRoad = false;
        }
      }

      return gotSingleLongestRoad;*/
    }

    internal void ClearRoads()
    {
      this.roadSegments.Clear();
    }

    internal void ClearSettlements()
    {
      this.settlements.Clear();
      this.settlementsByPlayer.Clear();
    }

    internal void LoadHexProduction(XmlReader reader)
    {
      var productionValues = reader.ReadElementContentAsString().Split(',');
      var index = 0;
      foreach (var productionValue in productionValues)
      {
        this.hexes[index++].Production = UInt32.Parse(productionValue);
      }
    }

    internal void LoadHexResources(XmlReader reader)
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
      UInt32 lhs = 0;
      UInt32 rhs = 8;
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

    private void AssignHexesToLocation()
    {
      this.hexesForLocations = new Dictionary<uint, uint[]>();

      this.hexesForLocations.Add(0u, new[] { 0u });
      this.hexesForLocations.Add(1u, new[] { 0u });
      this.hexesForLocations.Add(2u, new[] { 0u, 1u });
      this.hexesForLocations.Add(3u, new[] { 1u });
      this.hexesForLocations.Add(4u, new[] { 1u, 2u });
      this.hexesForLocations.Add(5u, new[] { 2u });
      this.hexesForLocations.Add(6u, new[] { 2u });

      this.hexesForLocations.Add(7u, new[] { 3u });
      this.hexesForLocations.Add(8u, new[] { 0u, 3u });
      this.hexesForLocations.Add(9u, new[] { 0u, 3u, 4u });
      this.hexesForLocations.Add(10u, new[] { 0u, 1u, 4u });
      this.hexesForLocations.Add(11u, new[] { 1u, 4u, 5u });
      this.hexesForLocations.Add(12u, new[] { 1u, 2u, 5u });
      this.hexesForLocations.Add(13u, new[] { 2u, 5u, 6u });
      this.hexesForLocations.Add(14u, new[] { 2u, 6u });
      this.hexesForLocations.Add(15u, new[] { 6u });

      this.hexesForLocations.Add(16u, new[] { 7u });
      this.hexesForLocations.Add(17u, new[] { 3u, 7u });
      this.hexesForLocations.Add(18u, new[] { 3u, 7u, 8u });
      this.hexesForLocations.Add(19u, new[] { 3u, 4u, 8u });
      this.hexesForLocations.Add(20u, new[] { 4u, 8u, 9u });
      this.hexesForLocations.Add(21u, new[] { 4u, 5u, 9u });
      this.hexesForLocations.Add(22u, new[] { 5u, 9u, 10u });
      this.hexesForLocations.Add(23u, new[] { 5u, 6u, 10u });
      this.hexesForLocations.Add(24u, new[] { 6u, 10u, 11u });
      this.hexesForLocations.Add(25u, new[] { 6u, 11u });
      this.hexesForLocations.Add(26u, new[] { 11u });

      this.hexesForLocations.Add(27u, new[] { 7u });
      this.hexesForLocations.Add(28u, new[] { 7u, 12u });
      this.hexesForLocations.Add(29u, new[] { 7u, 8u, 12u });
      this.hexesForLocations.Add(30u, new[] { 8u, 12u, 13u });
      this.hexesForLocations.Add(31u, new[] { 8u, 9u, 13u });
      this.hexesForLocations.Add(32u, new[] { 9u, 13u, 14u });
      this.hexesForLocations.Add(33u, new[] { 9u, 10u, 14u });
      this.hexesForLocations.Add(34u, new[] { 10u, 14u, 15u });
      this.hexesForLocations.Add(35u, new[] { 10u, 11u, 15u });
      this.hexesForLocations.Add(36u, new[] { 11u, 15u });
      this.hexesForLocations.Add(37u, new[] { 11u });

      this.hexesForLocations.Add(38u, new[] { 12u });
      this.hexesForLocations.Add(39u, new[] { 12u, 16u });
      this.hexesForLocations.Add(40u, new[] { 12u, 13u, 16u });
      this.hexesForLocations.Add(41u, new[] { 13u, 16u, 17u });
      this.hexesForLocations.Add(42u, new[] { 13u, 14u, 17u });
      this.hexesForLocations.Add(43u, new[] { 14u, 17u, 18u });
      this.hexesForLocations.Add(44u, new[] { 14u, 15u, 18u });
      this.hexesForLocations.Add(45u, new[] { 15u, 18u });
      this.hexesForLocations.Add(46u, new[] { 12u });

      this.hexesForLocations.Add(47u, new[] { 16u });
      this.hexesForLocations.Add(48u, new[] { 16u });
      this.hexesForLocations.Add(49u, new[] { 16u, 17u });
      this.hexesForLocations.Add(50u, new[] { 17u });
      this.hexesForLocations.Add(51u, new[] { 17u, 18u });
      this.hexesForLocations.Add(52u, new[] { 18u });
      this.hexesForLocations.Add(53u, new[] { 18u });
    }

    private void AssignLocationsToHex()
    {
      this.locationsForHex = new Dictionary<UInt32, UInt32[]>();

      this.AddLocationsToHex(0u, 8u, 0u, 3u);
      this.AddLocationsToHex(7u, 17u, 3u, 4u);
      this.AddLocationsToHex(16u, 27u, 7u, 5u);
      this.AddLocationsToHex(28u, 38u, 12u, 4u);
      this.AddLocationsToHex(39u, 47u, 16u, 3u);
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
    [DebuggerDisplay("Type = {Type}, Production = {Production}")]
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

    private class RoadNode
    {
      public Tuple<UInt32, Guid>[] OtherEnds = new Tuple<UInt32, Guid>[3];
    }
    #endregion
  }
}
