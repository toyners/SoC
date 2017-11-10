
namespace Jabberwocky.SoC.Library.GameBoards
{
  using System;
  using System.Collections.Generic;
  using System.Diagnostics;
  using System.Linq;
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
      RoadNotConnectedToExistingRoad,
      SettlementNotConnectedToExistingRoad,
      TooCloseToSettlement,
      RoadIsOffBoard,
      RoadIsOccupied,
      NoDirectConnection,
      StartingInfrastructureNotPresentWhenPlacingRoad,
      StartingInfrastructureNotCompleteWhenPlacingRoad,
      StartingInfrastructureNotPresentWhenPlacingSettlement,
      StartingInfrastructureNotCompleteWhenPlacingSettlement,
      StartingInfrastructureAlreadyPresent
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
    private Dictionary<Guid, List<RoadSegment>> roadSegmentsByPlayer;
    private Dictionary<UInt32, ResourceProducer[]> resourceProvidersByDiceRolls;
    private Dictionary<ResourceProducer, UInt32[]> locationsForResourceProvider;
    private Dictionary<UInt32, UInt32[]> locationsForHex;
    private Dictionary<UInt32, UInt32[]> hexesForLocations;
    private RoadNode[] roadNodes;
    private Location[] locations;
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
      this.roadSegmentsByPlayer = new Dictionary<Guid, List<RoadSegment>>();
      this.settlementsByPlayer = new Dictionary<Guid, List<UInt32>>();
      this.roadNodes = new RoadNode[StandardBoardLocationCount];

      this.locations = new Location[GameBoardData.StandardBoardLocationCount];
      for (var i = 0; i < GameBoardData.StandardBoardHexCount; i++)
      {
        this.locations[i] = new Location() { Index = i };
      }

      //this.ConnectLocationsVertically();
      //this.ConnectLocationsHorizontally();

      this.CreateHexes();

      this.connections = new Boolean[GameBoardData.StandardBoardLocationCount, GameBoardData.StandardBoardLocationCount];
      this.ConnectLocationsVerticallyOld();
      this.ConnectLocationsHorizontallyOld();

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
    public VerificationStatus CanPlaceCity(Guid playerId, UInt32 locationIndex)
    {
      throw new NotImplementedException();
    }

    public VerificationResults CanPlaceRoad(Guid playerId, UInt32 roadStartLocation, UInt32 roadEndLocation)
    {
      // Has the player placed their starting infrastructure
      switch (this.PlacedStartingInfrastructureStatus(playerId))
      {
        case StartingInfrastructureStatus.None: return new VerificationResults { Status = VerificationStatus.StartingInfrastructureNotPresentWhenPlacingRoad };
        case StartingInfrastructureStatus.Partial: return new VerificationResults { Status = VerificationStatus.StartingInfrastructureNotCompleteWhenPlacingRoad };
      }

      // Are both road locations on the board.
      if (!this.RoadLocationsOnBoard(roadStartLocation, roadEndLocation))      
      {
        return new VerificationResults { Status = VerificationStatus.RoadIsOffBoard };
      }

      // Is direct connection possible
      if (!this.DirectConnectionBetweenRoadLocations(roadStartLocation, roadEndLocation))
      {
        return new VerificationResults { Status = VerificationStatus.NoDirectConnection };
      }

      // Is there already a road built at the same location.
      if (this.RoadAlreadyPresent(roadStartLocation, roadEndLocation))
      {
        return new VerificationResults { Status = VerificationStatus.RoadIsOccupied };
      }

      // Does it connect to existing road
      if (!this.WillConnectToExistingRoad(playerId, roadStartLocation, roadEndLocation))
      {
        return new VerificationResults { Status = VerificationStatus.RoadNotConnectedToExistingRoad };
      }

      return new VerificationResults { Status = VerificationStatus.Valid };
    }

    private Boolean DirectConnectionBetweenRoadLocations(UInt32 roadStartLocation, UInt32 roadEndLocation)
    {
      return this.connections[roadStartLocation, roadEndLocation];
    }

    private Boolean RoadLocationsOnBoard(UInt32 roadStartLocation, UInt32 roadEndLocation)
    {
      var length = (UInt32)this.connections.GetLength(0); // TODO: Change to use Location array
      return roadStartLocation < length && roadEndLocation < length;
    }

    private List<Tuple<UInt32, UInt32, Guid>> builtRoadSegments = new List<Tuple<UInt32, UInt32, Guid>>();
    private Boolean RoadAlreadyPresent(UInt32 roadStartLocationIndex, UInt32 roadEndLocationIndex)
    {

      var roadSegment = builtRoadSegments.Where(r => (r.Item1 == roadStartLocationIndex && r.Item2 == roadEndLocationIndex)
      || (r.Item2 == roadStartLocationIndex && r.Item1 == roadEndLocationIndex)).FirstOrDefault();

      return roadSegment != null;
    }

    private Boolean WillConnectToExistingRoad(Guid playerId, UInt32 roadStartLocationIndex, UInt32 roadEndLocationIndex)
    {
      var roadSegment = builtRoadSegments.Where(r => (r.Item1 == roadStartLocationIndex || r.Item2 == roadStartLocationIndex
      || r.Item1 == roadEndLocationIndex || r.Item2 == roadEndLocationIndex) && r.Item3 == playerId).FirstOrDefault();

      return roadSegment != null;
    }

    public VerificationResults CanPlaceSettlement(Guid playerId, UInt32 locationIndex)
    {
      switch (this.PlacedStartingInfrastructureStatus(playerId))
      {
        case StartingInfrastructureStatus.None: return new VerificationResults { Status = VerificationStatus.StartingInfrastructureNotPresentWhenPlacingSettlement };
        case StartingInfrastructureStatus.Partial: return new VerificationResults { Status = VerificationStatus.StartingInfrastructureNotCompleteWhenPlacingSettlement };
      }

      if (!this.SettlementLocationOnBoard(locationIndex))
      {
        return new VerificationResults { Status = VerificationStatus.LocationIsInvalid };
      }

      if (this.SettlementLocationIsOccupied(locationIndex))
      {
        return new VerificationResults
        {
          Status = VerificationStatus.LocationIsOccupied,
          LocationIndex = locationIndex,
          PlayerId = this.settlements[locationIndex]
        };
      }

      Guid id;
      UInt32 index;
      if (this.TooCloseToSettlement(locationIndex, out id, out index))
      {
        return new VerificationResults
        {
          Status = VerificationStatus.TooCloseToSettlement,
          LocationIndex = index,
          PlayerId = id
        };
      }

      if (!this.SettlementIsOnRoad(playerId, locationIndex))
      {
        return new VerificationResults { Status = VerificationStatus.SettlementNotConnectedToExistingRoad };
      }

      return new VerificationResults { Status = VerificationStatus.Valid };
    }

    private Boolean SettlementIsOnRoad(Guid playerId, UInt32 locationIndex)
    {
      if (this.roadSegmentsByPlayer.ContainsKey(playerId))
      {
        return this.roadSegmentsByPlayer[playerId].FirstOrDefault(r => r.Location1 == locationIndex || r.Location2 == locationIndex) != null;
      }

      return false;
    }

    private Boolean TooCloseToSettlement(UInt32 locationIndex, out Guid id, out UInt32 index)
    {
      id = Guid.Empty;
      for (index = 0; index < this.connections.GetLength(1); index++)
      {
        if (this.connections[locationIndex, index] && this.settlements.ContainsKey(index))
        {
          id = this.settlements[index];
          return true;
        }
      }

      return false;
    }

    private Boolean SettlementLocationIsOccupied(UInt32 locationIndex)
    {
      return this.settlements.ContainsKey(locationIndex);
    }

    private Boolean SettlementLocationOnBoard(UInt32 settlementLocation)
    {
      return settlementLocation < (UInt32)this.connections.GetLength(0); // TODO: Change to use Location array
    }

    public VerificationResults CanPlaceStartingInfrastructure(Guid playerId, UInt32 settlementLocation, UInt32 roadEndLocation)
    {
      if (this.PlacedStartingInfrastructureStatus(playerId) == StartingInfrastructureStatus.Complete)
      {
        return new VerificationResults { Status = VerificationStatus.StartingInfrastructureAlreadyPresent };
      }

      if (!this.SettlementLocationOnBoard(settlementLocation))
      {
        return new VerificationResults { Status = VerificationStatus.LocationIsInvalid };
      }

      if (this.SettlementLocationIsOccupied(settlementLocation))
      {
        return new VerificationResults
        {
          Status = VerificationStatus.LocationIsOccupied,
          LocationIndex = settlementLocation,
          PlayerId = this.settlements[settlementLocation]
        };
      }

      Guid id;
      UInt32 index;
      if (this.TooCloseToSettlement(settlementLocation, out id, out index))
      {
        return new VerificationResults
        {
          Status = VerificationStatus.TooCloseToSettlement,
          LocationIndex = index,
          PlayerId = id
        };
      }

      // Verify #1 - Are both road locations on the board.
      if (!this.RoadLocationsOnBoard(settlementLocation, roadEndLocation))
      {
        return new VerificationResults { Status = VerificationStatus.RoadIsOffBoard };
      }

      // Verify #2 - Is direct connection possible
      if (!this.DirectConnectionBetweenRoadLocations(settlementLocation, roadEndLocation))
      {
        return new VerificationResults { Status = VerificationStatus.NoDirectConnection };
      }

      // Verify #3 - Is there already a road built at the same location.
      if (this.RoadAlreadyPresent(settlementLocation, roadEndLocation))
      {
        return new VerificationResults { Status = VerificationStatus.RoadIsOccupied };
      }

      // This is the first road segment on the board - No sense in checking for a connection
      // to an existing road segment

      return new VerificationResults { Status = VerificationStatus.Valid };
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
      foreach (var kv in this.roadSegmentsByPlayer)
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

    public void PlaceRoad(Guid playerId, UInt32 roadStartLocation, UInt32 roadEndLocation)
    {
      var verificationResults = this.CanPlaceRoad(playerId, roadStartLocation, roadEndLocation);
      this.ThrowExceptionOnBadVerificationResult(verificationResults);
      
      this.PlaceRoadOnBoard(playerId, roadStartLocation, roadEndLocation);
    }

    private enum StartingInfrastructureStatus
    {
      None,
      Partial,
      Complete
    }

    private StartingInfrastructureStatus PlacedStartingInfrastructureStatus(Guid playerId)
    {
      if (!this.settlementsByPlayer.ContainsKey(playerId) || 
        this.settlementsByPlayer[playerId] == null ||
        this.settlementsByPlayer[playerId].Count == 0)
      {
        return StartingInfrastructureStatus.None;
      }

      if (this.settlementsByPlayer[playerId].Count == 1)
      {
        return StartingInfrastructureStatus.Partial;
      }

      if (!this.roadSegmentsByPlayer.ContainsKey(playerId) ||
          this.roadSegmentsByPlayer[playerId] == null ||
          this.roadSegmentsByPlayer[playerId].Count == 0)
      {
        return StartingInfrastructureStatus.None;
      }

      return this.roadSegmentsByPlayer[playerId].Count == 1 ? StartingInfrastructureStatus.Partial : StartingInfrastructureStatus.Complete;
    }

    private void PlaceRoadOnBoard(Guid playerId, UInt32 roadStartLocationIndex, UInt32 roadEndLocationIndex)
    {
      this.builtRoadSegments.Add(new Tuple<uint, uint, Guid>(roadStartLocationIndex, roadEndLocationIndex, playerId));
      var newRoadSegment = new RoadSegment(roadStartLocationIndex, roadEndLocationIndex);
      this.roadSegments.Add(newRoadSegment);

      if (!this.roadSegmentsByPlayer.ContainsKey(playerId))
      {
        var roadSegmentList = new List<RoadSegment>();
        roadSegmentList.Add(newRoadSegment);
        this.roadSegmentsByPlayer.Add(playerId, roadSegmentList);
      }
      else
      {
        this.roadSegmentsByPlayer[playerId].Add(newRoadSegment);
      }

      var startRoadNode = this.roadNodes[roadStartLocationIndex];
      if (startRoadNode == null)
      {
        startRoadNode = new RoadNode();
        this.roadNodes[roadStartLocationIndex] = startRoadNode;
      }

      for (var i = 0; i < startRoadNode.Trails.Length; i++)
      {
        if (startRoadNode.Trails[i] == null)
        {
          startRoadNode.Trails[i] = new Tuple<UInt32, Guid>(roadEndLocationIndex, playerId);
          break;
        }
      }

      var endRoadNode = this.roadNodes[roadEndLocationIndex];
      if (endRoadNode == null)
      {
        endRoadNode = new RoadNode();
        this.roadNodes[roadEndLocationIndex] = endRoadNode;
      }

      for (var i = 0; i < endRoadNode.Trails.Length; i++)
      {
        if (endRoadNode.Trails[i] == null)
        {
          endRoadNode.Trails[i] = new Tuple<UInt32, Guid>(roadStartLocationIndex, playerId);
          break;
        }
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
      var verificationResults = this.CanPlaceSettlement(playerId, locationIndex);
      this.ThrowExceptionOnBadVerificationResult(verificationResults);

      this.PlaceSettlementOnBoard(playerId, locationIndex);
    }

    /// <summary>
    /// Place starting infrastructure (settlement and connecting road segment).
    /// </summary>
    /// <param name="playerId">Id of player placing the infrastructure.</param>
    /// <param name="settlementLocation">Location to place settlement. Also the starting location of the road segment.</param>
    /// <param name="roadEndLocation">End location of road segment.</param>
    public void PlaceStartingInfrastructure(Guid playerId, UInt32 settlementLocation, UInt32 roadEndLocation)
    {
      var verificationResults = this.CanPlaceStartingInfrastructure(playerId, settlementLocation, roadEndLocation);
      this.ThrowExceptionOnBadVerificationResult(verificationResults);

      this.PlaceSettlementOnBoard(playerId, settlementLocation);
      this.PlaceRoadOnBoard(playerId, settlementLocation, roadEndLocation);
    }

    private void PlaceSettlementOnBoard(Guid playerId, UInt32 settlementLocation)
    {
      if (this.settlementsByPlayer.ContainsKey(playerId))
      {
        this.settlementsByPlayer[playerId].Add(settlementLocation);
      }
      else
      {
        this.settlementsByPlayer.Add(playerId, new List<uint> { settlementLocation });
      }

      this.settlements.Add(settlementLocation, playerId);
    }

    private void ThrowExceptionOnBadVerificationResult(VerificationResults verificationResults)
    {
      switch (verificationResults.Status)
      {
        case VerificationStatus.LocationIsInvalid: throw new PlacementException("Cannot place settlement because location is not on board.");
        case VerificationStatus.LocationIsOccupied: throw new PlacementException("Cannot place settlement because location is already settled.");
        case VerificationStatus.NoDirectConnection: throw new PlacementException("Cannot place road because no direct connection between start location and end location.");
        case VerificationStatus.RoadNotConnectedToExistingRoad: throw new PlacementException("Cannot place road because it is not connected to an existing road segment.");
        case VerificationStatus.RoadIsOccupied: throw new PlacementException("Cannot place road because road already exists.");
        case VerificationStatus.RoadIsOffBoard: throw new PlacementException("Cannot place road because board location is not valid.");
        case VerificationStatus.SettlementNotConnectedToExistingRoad: throw new PlacementException("Cannot place settlement because location is not on a road.");
        case VerificationStatus.StartingInfrastructureAlreadyPresent: throw new PlacementException("Cannot place starting infrastructure more than once per player.");
        case VerificationStatus.StartingInfrastructureNotCompleteWhenPlacingRoad:
        case VerificationStatus.StartingInfrastructureNotPresentWhenPlacingRoad: throw new PlacementException("Cannot place road before placing all initial infrastructure.");
        case VerificationStatus.StartingInfrastructureNotCompleteWhenPlacingSettlement:
        case VerificationStatus.StartingInfrastructureNotPresentWhenPlacingSettlement: throw new PlacementException("Cannot place settlement before placing all initial infrastructure.");
        case VerificationStatus.TooCloseToSettlement: throw new PlacementException("Cannot place settlement because location is too close to exising settlement.");
      }
    }

    Dictionary<UInt32, HashSet<RoadSegment>> roadsByLocation = new Dictionary<UInt32, HashSet<RoadSegment>>();
    Guid[,] roadss = new Guid[1,1];
    public Boolean TryGetLongestRoadDetails(out Guid playerId, out Int32 roadLength)
    {
      var singleLongestRoad = false;
      playerId = Guid.Empty;
      roadLength = 0;

      // Get all road ends - Start from the starting settlements because most times they will be a genuine road end
      // (except in the case of a cycle). 
      // Start from road end and advance along path, store each in a set to ensure that cycles are ignored.
      // Find all road segments that are connected to the other end of this road segment
      // If there are multiple road segments connected to the other end then place a bookmark on the stack.
      // The bookmark has the location to start from and the set of the locations (paths) not currently explored and
      // also the visited set (maybe).

      foreach (var kv2 in this.roadSegmentsByPlayer)
      {
        var segments = kv2.Value;
        var roadEnds = new List<UInt32>();
        var startingBookmarks = new List<StartingBookmark>();
        var startingBookmarkIndex = -1;
        var settlementsPlacedByPlayer = this.settlementsByPlayer[kv2.Key];

        roadEnds.Add(settlementsPlacedByPlayer[0]);
        roadEnds.Add(settlementsPlacedByPlayer[1]);

        #region Finding road ends
        /*foreach (var segment in segments)
        {
          var ctl1 = segments.Where(s => s != segment && segment.Location1 == s.Location1 || segment.Location1 == s.Location2).ToList();
          var ctl2 = segments.Where(s => s != segment && segment.Location2 == s.Location1 || segment.Location2 == s.Location2).ToList();

          var connectedToLocation1 = ctl1.Count;
          var connectedToLocation2 = ctl2.Count;

          if (connectedToLocation1 != connectedToLocation2 && (connectedToLocation1 == 0 || connectedToLocation2 == 0))
          {
            if (connectedToLocation1 == 0)
            {
              roadEnds.Add(segment.Location1);
            }

            if (connectedToLocation2 == 0)
            {
              roadEnds.Add(segment.Location2);
            }
          }
        }

        if (roadEnds.Count == 0)
        {
          return TryGetLongestRoadDetailsForCycle(segments, settlementsPlacedByPlayer, out playerId, out roadLength);
        }*/
        #endregion

        var bookmarks = new List<Bookmark>();

        var visitedSet = new HashSet<RoadSegment>();
        
        //for (var index = 0; index < roadEnds.Count; index++)
        while (roadEnds.Count > 0)
        {
          var currentRoadEndLocation = roadEnds.First();
          roadEnds.RemoveAt(0);
          StartingBookmark startingBookmark = null;
          var workingRoadLength = 0;
          visitedSet.Clear();
          startingBookmarks.Clear();
          startingBookmarkIndex = -1;
          bookmarks.Clear();

          var connectingSegments = segments.Where(r => r.Location1 == currentRoadEndLocation || r.Location2 == currentRoadEndLocation).ToList();
          if (connectingSegments.Count > 1)
          {
            for (var i = 0; i < connectingSegments.Count; i++)
            {
              var connectingSegment = connectingSegments[i];
              var startingLocation = connectingSegment.Location1 == currentRoadEndLocation ? connectingSegment.Location2 : connectingSegment.Location1;
              startingBookmark = new StartingBookmark(startingLocation, connectingSegment);
              startingBookmarks.Add(startingBookmark);
            }

            startingBookmarkIndex = 0;
            var startingSegment = connectingSegments[0];
            currentRoadEndLocation = startingSegment.Location1 == currentRoadEndLocation ? startingSegment.Location2 : startingSegment.Location1;
            workingRoadLength = 1;
            visitedSet.Add(startingSegment);
          }

          while (true)
          {
            var segmentsContainingLocation = segments.Where(r => (r.Location1 == currentRoadEndLocation || r.Location2 == currentRoadEndLocation) && !visitedSet.Contains(r)).ToList();

            if (segmentsContainingLocation.Count == 0)
            {
              // No new segments connected to this location - At end of road.

              if (workingRoadLength > roadLength)
              {
                roadLength = workingRoadLength;
                playerId = kv2.Key;
                singleLongestRoad = true;
              }
              else if (workingRoadLength == roadLength)
              {
                singleLongestRoad = false;
              }

              if (bookmarks.Count > 0)
              {
                var bookmark = bookmarks.Last();
                bookmarks.RemoveAt(bookmarks.Count - 1);
                currentRoadEndLocation = bookmark.StartingLocation;
                workingRoadLength = bookmark.RoadLength;
                visitedSet = bookmark.Visited;
                continue;
              }

              if (startingBookmarks.Count > 0)
              {
                startingBookmarks[startingBookmarkIndex].WorkingRoadLength = workingRoadLength;

                if (startingBookmarkIndex < startingBookmarks.Count - 1)
                {
                  startingBookmarkIndex++;
                  currentRoadEndLocation = startingBookmarks[startingBookmarkIndex].StartingLocation;
                  workingRoadLength = 1;
                  visitedSet.Add(startingBookmarks[startingBookmarkIndex].VisitedSegment);
                  continue;
                }

                if (startingBookmarks.Count == 3)
                {
                  if (workingRoadLength < startingBookmarks[0].WorkingRoadLength && workingRoadLength < startingBookmarks[1].WorkingRoadLength)
                  {
                    workingRoadLength = startingBookmarks[0].WorkingRoadLength + startingBookmarks[1].WorkingRoadLength;
                  }
                  else if (startingBookmarks[0].WorkingRoadLength < startingBookmarks[1].WorkingRoadLength)
                  {
                    workingRoadLength += startingBookmarks[1].WorkingRoadLength;
                  }
                  else
                  {
                    workingRoadLength += startingBookmarks[0].WorkingRoadLength;
                  }
                }
                else if (startingBookmarks.Count == 2)
                {
                  workingRoadLength += startingBookmarks[0].WorkingRoadLength;
                }

                if (workingRoadLength > roadLength)
                {
                  roadLength = workingRoadLength;
                  playerId = kv2.Key;
                  singleLongestRoad = true;
                }
                else if (workingRoadLength == roadLength)
                {
                  singleLongestRoad = false;
                }
              }

              break;
            }

            var currentRoadSegment = segmentsContainingLocation[0];
            visitedSet.Add(currentRoadSegment);

            if (segmentsContainingLocation.Count > 1)
            {
              // Bookmark with next location, working road length, visited set
              for (var segmentIndex = 1; segmentIndex < segmentsContainingLocation.Count; segmentIndex++)
              {
                var alternateSegment = segmentsContainingLocation[segmentIndex];
                var startLocation = alternateSegment.Location1 == currentRoadEndLocation ? alternateSegment.Location2 : alternateSegment.Location1;
                var bookmark = new Bookmark(alternateSegment, startLocation, workingRoadLength + 1, visitedSet);
                bookmarks.Add(bookmark);
              }
            }

            // Move along road segment i.e. get other end
            currentRoadEndLocation = currentRoadSegment.Location1 == currentRoadEndLocation ? currentRoadSegment.Location2 : currentRoadSegment.Location1;
            workingRoadLength++;

            Int32 existingBookmarkIndex = -1;
            if ((existingBookmarkIndex = bookmarks.FindIndex(b => b.StartingLocation == currentRoadEndLocation)) != -1)
            {
              bookmarks.RemoveAt(existingBookmarkIndex);
            }

            if ((existingBookmarkIndex = startingBookmarks.FindIndex(b => b.StartingLocation == currentRoadEndLocation)) != -1)
            {
              startingBookmarks.RemoveAt(existingBookmarkIndex);
            }
          }
        }
      }

      if (!singleLongestRoad)
      {
        playerId = Guid.Empty;
        roadLength = -1;
      }

      return singleLongestRoad;
    }

    private class StartingBookmark
    {
      public UInt32 StartingLocation;
      public Int32 WorkingRoadLength;
      public RoadSegment VisitedSegment;

      public StartingBookmark(UInt32 startingLocation, RoadSegment roadSegment)
      {
        StartingLocation = startingLocation;
        VisitedSegment = roadSegment;
      }
    }

    private class Bookmark
    {
      public UInt32 StartingLocation;
      public Int32 RoadLength;
      public HashSet<RoadSegment> Visited;

      public Bookmark(RoadSegment segment, UInt32 startingLocation, Int32 roadLength, HashSet<RoadSegment> visited)
      {
        this.StartingLocation = startingLocation;
        this.RoadLength = roadLength;
        this.Visited = new HashSet<RoadSegment>(visited);
        this.Visited.Add(segment);
      }
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
      Connection connection = null;
      foreach (var setup in new[] {
            new HorizontalTrailSetup(0, 4, 8),
            new HorizontalTrailSetup(7, 5, 10),
            new HorizontalTrailSetup(16, 6, 11),
            new HorizontalTrailSetup(28, 5, 10),
            new HorizontalTrailSetup(39, 4, 8) })
      {
        var count = setup.TrailCount;
        var startIndex = setup.LocationIndexStart;
        Int32 endIndex = -1;
        while (count-- > 0)
        {
          endIndex = startIndex + setup.LocationIndexDiff;

          connection = new Connection();
          connection.Location1 = this.locations[startIndex];
          connection.Location2 = this.locations[endIndex];
          this.locations[startIndex].connections[2] = connection;
          this.locations[endIndex].connections[2] = connection;

          //this.connections[startIndex, endIndex] = this.connections[endIndex, startIndex] = true;
          startIndex += 2;
        }

        // First and last location in column only have two connections so move them towards 0th end.
        this.locations[setup.LocationIndexDiff].connections[1] = this.locations[setup.LocationIndexDiff].connections[2];
        this.locations[setup.LocationIndexDiff].connections[2] = null;

        this.locations[endIndex].connections[1] = connection;
        this.locations[endIndex].connections[2] = null;
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
        Connection connection = null;
        while (count-- > 0)
        {
          connection = new Connection();
          connection.Location1 = this.locations[startIndex];
          connection.Location2 = this.locations[endIndex];
          this.locations[startIndex].connections[0] = connection;
          this.locations[endIndex].connections[1] = connection;

          //this.connections[startIndex, endIndex] = this.connections[endIndex, startIndex] = true;
          startIndex++;
          endIndex++;
        }

        // Last location in column so change the connection end to be in the 0th slot. Otherwise
        // 0th slot will be null. 
        this.locations[endIndex - 1].connections[0] = connection;
        this.locations[endIndex - 1].connections[1] = null;
      }
    }

    private void ConnectLocationsHorizontallyOld()
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

    private void ConnectLocationsVerticallyOld()
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

    [DebuggerDisplay("Status = {Status}, PlayerId = {PlayerId}, LocationIndex = {LocationIndex}")]
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
      public Tuple<UInt32, Guid>[] Trails = new Tuple<UInt32, Guid>[3];
    }

    private class Connection
    {
      public Guid Owner;
      public Location Location1;
      public Location Location2;
    }

    private class Location
    {
      public Int32 Index;
      public Guid Owner;
      public Connection[] connections; 
    }

    public class PlacementException : Exception
    {
      public PlacementException() : base() { }
      public PlacementException(String message) : base(message) { }
    }
    #endregion
  }
}
