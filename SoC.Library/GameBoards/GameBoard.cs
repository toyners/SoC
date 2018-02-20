﻿
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
  public class GameBoard
  {
    #region Enums
    public enum VerificationStatus
    {
      Valid,
      LocationForCityIsInvalid,
      LocationForSettlementIsInvalid,
      LocationIsAlreadyCity,
      LocationIsOccupied,
      LocationIsNotSettled,
      LocationIsNotOwned,
      RoadNotConnectedToExistingRoad,
      SettlementNotConnectedToExistingRoad,
      TooCloseToSettlement,
      RoadIsOffBoard,
      RoadIsOccupied,
      NoDirectConnection,
      StartingInfrastructureNotPresentWhenPlacingCity,
      StartingInfrastructureNotCompleteWhenPlacingCity,
      StartingInfrastructureNotPresentWhenPlacingRoad,
      StartingInfrastructureNotCompleteWhenPlacingRoad,
      StartingInfrastructureNotPresentWhenPlacingSettlement,
      StartingInfrastructureNotCompleteWhenPlacingSettlement,
      StartingInfrastructureAlreadyPresent
    }

    private enum StartingInfrastructureStatus
    {
      None,
      Partial,
      Complete
    }
    #endregion

    #region Fields
    public const Int32 StandardBoardLocationCount = 54;
    public const Int32 StandardBoardTrailCount = 72;
    public const Int32 StandardBoardHexCount = 19;
    private Dictionary<UInt32, Guid> cities;
    private ResourceProducer[] hexes;
    private Dictionary<UInt32, Guid> settlements;
    private Dictionary<Guid, List<UInt32>> settlementsByPlayer;
    private Boolean[,] connections;
    private Dictionary<Guid, List<RoadSegment>> roadSegmentsByPlayer;
    private Dictionary<UInt32, ResourceProducer[]> resourceProvidersByDiceRolls;
    private Dictionary<ResourceProducer, UInt32[]> locationsForResourceProvider;
    private Dictionary<UInt32, UInt32[]> locationsForHex;
    private Dictionary<UInt32, UInt32[]> hexesForLocations;
    private RoadNode[] roadNodes;
    private Location[] locations;
    #endregion

    #region Construction
    public GameBoard(BoardSizes size)
    {
      if (size == BoardSizes.Extended)
      {
        throw new Exception("Extended boards not implemented.");
      }

      this.Length = StandardBoardLocationCount;
      this.cities = new Dictionary<UInt32, Guid>();
      this.settlements = new Dictionary<UInt32, Guid>();
      this.roadSegmentsByPlayer = new Dictionary<Guid, List<RoadSegment>>();
      this.settlementsByPlayer = new Dictionary<Guid, List<UInt32>>();
      this.roadNodes = new RoadNode[StandardBoardLocationCount];

      this.locations = new Location[GameBoard.StandardBoardLocationCount];
      for (var i = 0; i < GameBoard.StandardBoardHexCount; i++)
      {
        this.locations[i] = new Location() { Index = i };
      }

      this.CreateHexes();

      this.connections = new Boolean[GameBoard.StandardBoardLocationCount, GameBoard.StandardBoardLocationCount];
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
    public VerificationResults CanPlaceCity(Guid playerId, UInt32 location)
    {
      switch (this.PlacedStartingInfrastructureStatus(playerId))
      {
        case StartingInfrastructureStatus.None:
        return new VerificationResults { Status = VerificationStatus.StartingInfrastructureNotPresentWhenPlacingCity };
        case StartingInfrastructureStatus.Partial:
        return new VerificationResults { Status = VerificationStatus.StartingInfrastructureNotCompleteWhenPlacingCity };
      }

      if (!this.SettlementLocationOnBoard(location))
      {
        return new VerificationResults { Status = VerificationStatus.LocationForCityIsInvalid };
      }

      var owningPlayerId = this.GetOwningPlayerForLocation(location);
      if (owningPlayerId == Guid.Empty)
      {
        return new VerificationResults { Status = VerificationStatus.LocationIsNotSettled };
      }

      if (owningPlayerId != playerId)
      {
        return new VerificationResults
        {
          Status = VerificationStatus.LocationIsNotOwned,
          PlayerId = owningPlayerId,
          LocationIndex = location
        };
      }

      if (this.LocationHasPlayerCity(playerId, location))
      {
        return new VerificationResults
        {
          Status = VerificationStatus.LocationIsAlreadyCity,
          LocationIndex = location,
          PlayerId = playerId
        };
      }

      return new VerificationResults { Status = VerificationStatus.Valid };
    }

    public Boolean CanPlaceRobber(UInt32 hex)
    {
      return hex < StandardBoardHexCount;
    }

    private Guid GetOwningPlayerForLocation(UInt32 location)
    {
      if (!this.settlements.ContainsKey(location))
      {
        return Guid.Empty;
      }

      return this.settlements[location];
    }

    private Boolean LocationHasPlayerCity(Guid playerId, UInt32 location)
    {
      return this.cities.ContainsKey(location) && this.settlements[location] == playerId;
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

    public VerificationResults CanPlaceSettlement(Guid playerId, UInt32 locationIndex)
    {
      switch (this.PlacedStartingInfrastructureStatus(playerId))
      {
        case StartingInfrastructureStatus.None:
        return new VerificationResults { Status = VerificationStatus.StartingInfrastructureNotPresentWhenPlacingSettlement };
        case StartingInfrastructureStatus.Partial:
        return new VerificationResults { Status = VerificationStatus.StartingInfrastructureNotCompleteWhenPlacingSettlement };
      }

      if (!this.SettlementLocationOnBoard(locationIndex))
      {
        return new VerificationResults { Status = VerificationStatus.LocationForSettlementIsInvalid };
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
        return new VerificationResults
        {
          Status = VerificationStatus.SettlementNotConnectedToExistingRoad,
          LocationIndex = locationIndex
        };
      }

      return new VerificationResults { Status = VerificationStatus.Valid };
    }

    public void PlaceCity(Guid playerId, UInt32 location)
    {
      var verificationResults = this.CanPlaceCity(playerId, location);
      this.ThrowExceptionOnBadVerificationResult(verificationResults);

      this.PlaceCityOnBoard(playerId, location);
    }

    public void PlaceRoadSegment(Guid playerId, UInt32 roadStartLocation, UInt32 roadEndLocation)
    {
      var verificationResults = this.CanPlaceRoad(playerId, roadStartLocation, roadEndLocation);
      this.ThrowExceptionOnBadVerificationResult(verificationResults);

      this.PlaceRoadSegmentOnBoard(playerId, roadStartLocation, roadEndLocation);
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
      this.PlaceRoadSegmentOnBoard(playerId, settlementLocation, roadEndLocation);
    }

    internal void PlaceCityOnBoard(Guid playerId, UInt32 location)
    {
      this.cities.Add(location, playerId);
    }

    internal void PlaceRoadSegmentOnBoard(Guid playerId, UInt32 roadStartLocationIndex, UInt32 roadEndLocationIndex)
    {
      var newRoadSegment = new RoadSegment(roadStartLocationIndex, roadEndLocationIndex);

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
    }

    internal void PlaceSettlementOnBoard(Guid playerId, UInt32 settlementLocation)
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

    private Boolean DirectConnectionBetweenRoadLocations(UInt32 roadStartLocation, UInt32 roadEndLocation)
    {
      return this.connections[roadStartLocation, roadEndLocation];
    }

    private Boolean RoadLocationsOnBoard(UInt32 roadStartLocation, UInt32 roadEndLocation)
    {
      var length = (UInt32)this.connections.GetLength(0); // TODO: Change to use Location array
      return roadStartLocation < length && roadEndLocation < length;
    }

    private Boolean RoadAlreadyPresent(UInt32 roadStartLocationIndex, UInt32 roadEndLocationIndex)
    {
      foreach (var kv in this.roadSegmentsByPlayer)
      {
        var roadSegment = kv.Value.Where(r => (r.Location1 == roadStartLocationIndex && r.Location2 == roadEndLocationIndex)
        || (r.Location2 == roadStartLocationIndex && r.Location1 == roadEndLocationIndex)).FirstOrDefault();

        if (roadSegment != null)
        {
          return true;
        }
      }

      return false;
    }

    private Boolean WillConnectToExistingRoad(Guid playerId, UInt32 roadStartLocationIndex, UInt32 roadEndLocationIndex)
    {
      var roadSegment = this.roadSegmentsByPlayer[playerId].Where(r => (r.Location1 == roadStartLocationIndex || r.Location2 == roadStartLocationIndex ||
        r.Location1 == roadEndLocationIndex || r.Location2 == roadEndLocationIndex)).FirstOrDefault();

      return roadSegment != null;
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
        return new VerificationResults { Status = VerificationStatus.LocationForSettlementIsInvalid };
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

    public Tuple<ResourceTypes?, UInt32>[] GetHexInformation()
    {
      var data = new Tuple<ResourceTypes?, UInt32>[this.hexes.Length];
      for (var index = 0; index < this.hexes.Length; index++)
      {
        data[index] = new Tuple<ResourceTypes?, uint>(this.hexes[index].Type, this.hexes[index].Production);
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

    public virtual ResourceClutch GetResourcesForLocation(UInt32 location)
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

    public virtual Dictionary<Guid, ResourceCollection[]> GetResourcesForRoll(UInt32 diceRoll)
    {
      var workingResources = new Dictionary<Guid, List<ResourceCollection>>();

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

          List<ResourceCollection> resourceCollectionList;

          // Got an owner - add to the resource count for the owner.
          if (workingResources.ContainsKey(owner))
          {
            resourceCollectionList = workingResources[owner];
          }
          else
          {
            resourceCollectionList = new List<ResourceCollection>();
            workingResources.Add(owner, resourceCollectionList);
          }

          ResourceClutch resourceClutch = ResourceClutch.Zero;
          switch (resourceProvider.Type)
          {
            case ResourceTypes.Brick: resourceClutch = ResourceClutch.OneBrick; break;
            case ResourceTypes.Grain: resourceClutch = ResourceClutch.OneGrain; break;
            case ResourceTypes.Lumber: resourceClutch = ResourceClutch.OneLumber; break;
            case ResourceTypes.Ore: resourceClutch = ResourceClutch.OneOre; break;
            case ResourceTypes.Wool: resourceClutch = ResourceClutch.OneWool; break;
          }

          var resourceCollection = new ResourceCollection(location, resourceClutch);
          resourceCollectionList.Add(resourceCollection);
        }
      }

      var resources = new Dictionary<Guid, ResourceCollection[]>();

      foreach (var kv in workingResources)
      {
        resources.Add(kv.Key, kv.Value.ToArray());
      }

      return resources;
    }

    public Tuple<UInt32, UInt32, Guid>[] GetRoadInformation()
    {
      var count = 0;
      foreach (var kv in this.roadSegmentsByPlayer)
      {
        count += kv.Value.Count;
      }

      var data = new Tuple<UInt32, UInt32, Guid>[count];
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

    private void ThrowExceptionOnBadVerificationResult(VerificationResults verificationResults)
    {
      switch (verificationResults.Status)
      {
        case VerificationStatus.LocationForCityIsInvalid: throw new PlacementException("Cannot place city because location is not on board.");
        case VerificationStatus.LocationIsAlreadyCity: throw new PlacementException("Cannot place city on existing city.");
        case VerificationStatus.LocationForSettlementIsInvalid: throw new PlacementException("Cannot place settlement because location is not on board.");
        case VerificationStatus.LocationIsOccupied: throw new PlacementException("Cannot place settlement because location is already settled.");
        case VerificationStatus.LocationIsNotOwned: throw new PlacementException("Cannot place city because location is settled by an opponent.");
        case VerificationStatus.LocationIsNotSettled: throw new PlacementException("Cannot place city because location is not settled.");
        case VerificationStatus.NoDirectConnection: throw new PlacementException("Cannot place road because no direct connection between start location and end location.");
        case VerificationStatus.RoadNotConnectedToExistingRoad: throw new PlacementException("Cannot place road because it is not connected to an existing road segment.");
        case VerificationStatus.RoadIsOccupied: throw new PlacementException("Cannot place road because road already exists.");
        case VerificationStatus.RoadIsOffBoard: throw new PlacementException("Cannot place road because board location is not valid.");
        case VerificationStatus.SettlementNotConnectedToExistingRoad: throw new PlacementException("Cannot place settlement because location is not on a road.");
        case VerificationStatus.StartingInfrastructureAlreadyPresent: throw new PlacementException("Cannot place starting infrastructure more than once per player.");
        case VerificationStatus.StartingInfrastructureNotPresentWhenPlacingCity:
        case VerificationStatus.StartingInfrastructureNotCompleteWhenPlacingCity: throw new PlacementException("Cannot place city before placing all initial infrastructure.");        
        case VerificationStatus.StartingInfrastructureNotCompleteWhenPlacingRoad:
        case VerificationStatus.StartingInfrastructureNotPresentWhenPlacingRoad: throw new PlacementException("Cannot place road before placing all initial infrastructure.");
        case VerificationStatus.StartingInfrastructureNotCompleteWhenPlacingSettlement:
        case VerificationStatus.StartingInfrastructureNotPresentWhenPlacingSettlement: throw new PlacementException("Cannot place settlement before placing all initial infrastructure.");
        case VerificationStatus.TooCloseToSettlement: throw new PlacementException("Cannot place settlement because location is too close to exising settlement.");
      }
    }

    public Boolean TryGetLongestRoadDetails(out Guid playerId, out UInt32[] road)
    {
      playerId = Guid.Empty;
      road = null;
      List<UInt32> longestRoute = null;
      Boolean gotSingleLongestRoad = false;

      foreach (var playerRoadSegments in this.roadSegmentsByPlayer)
      {
        var roadSegments = playerRoadSegments.Value;
        var settlementsPlacedByPlayer = this.settlementsByPlayer[playerRoadSegments.Key];
        var roadEnds = new Queue<UInt32>();
        roadEnds.Enqueue(settlementsPlacedByPlayer[0]);
        roadEnds.Enqueue(settlementsPlacedByPlayer[1]);
        var visitedRoadEnds = new HashSet<UInt32>();

        while (roadEnds.Count > 0)
        {
          var currentLocation = roadEnds.Dequeue();
          var visitedRoadSegments = new HashSet<RoadSegment>();
          var workingRoute = new List<UInt32> { currentLocation };
          visitedRoadEnds.Add(currentLocation);
          var forkmarks = new Stack<Forkmark>();

          while (true)
          {
            var unvisitedSegmentsConnectedToLocation = roadSegments
              .Where(r => r.IsOnLocation(currentLocation) && !visitedRoadSegments.Contains(r))
              .ToList();

            if (unvisitedSegmentsConnectedToLocation.Count == 0)  
            {
              // No unvisited segments connected to this location so at end of route.
              // If this route end has not been discovered or processed then place it in the 
              // road end queue for later processing
              if (!visitedRoadEnds.Contains(currentLocation) && !roadEnds.Contains(currentLocation))
              {
                roadEnds.Enqueue(currentLocation);
              }

              // Set the new longest route if working route is longer
              if (longestRoute == null || workingRoute.Count > longestRoute.Count)
              {
                longestRoute = workingRoute;
                playerId = playerRoadSegments.Key;
                gotSingleLongestRoad = true;
              }
              else if (longestRoute.Count == workingRoute.Count && playerRoadSegments.Key != playerId)
              {
                gotSingleLongestRoad = false;
              }

              // Process a forkmark if we have any.
              if (forkmarks.Count > 0)
              {
                var forkmark = forkmarks.Pop();
                workingRoute = forkmark.WorkingRoute;
                currentLocation = forkmark.StartingLocation;
                visitedRoadSegments = forkmark.VisitedRoadSegments;
                continue;
              }

              break;
            }

            if (unvisitedSegmentsConnectedToLocation.Count > 1)
            {
              // At a fork in the route - mark other branches for processing later
              for (var index = 1; index < unvisitedSegmentsConnectedToLocation.Count; index++)
              {
                var otherRoadSegment = unvisitedSegmentsConnectedToLocation[index];
                var startLocation = otherRoadSegment.Location1 == currentLocation ? otherRoadSegment.Location2 : otherRoadSegment.Location1;
                var newForkMark = new Forkmark(otherRoadSegment, startLocation, 0, visitedRoadSegments, workingRoute);
                forkmarks.Push(newForkMark);
              }
            }

            var currentRoadSegment = unvisitedSegmentsConnectedToLocation[0];
            visitedRoadSegments.Add(currentRoadSegment);

            // Move along road segment i.e. get other end
            currentLocation = currentRoadSegment.Location1 == currentLocation ? currentRoadSegment.Location2 : currentRoadSegment.Location1;
            workingRoute.Add(currentLocation);
          }
        }
      }

      if (!gotSingleLongestRoad)
      {
        playerId = Guid.Empty;
        road = null;
      }
      else
      {
        road = longestRoute.ToArray();
      }

      return gotSingleLongestRoad;
    }

    internal void ClearRoads()
    {
      this.roadSegmentsByPlayer.Clear();
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
          this.hexes[index++].Type = null;
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
      this.hexes[0] = new ResourceProducer { Type = null, Production = 0u };
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
      public ResourceTypes? Type;
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

    private class Forkmark
    {
      public readonly UInt32 StartingLocation;
      public readonly Int32 RoadLength;
      public readonly HashSet<RoadSegment> VisitedRoadSegments;
      public readonly List<UInt32> WorkingRoute;

      public Forkmark(RoadSegment segment, UInt32 startingLocation, Int32 roadLength, HashSet<RoadSegment> visitedRoadSegments, List<UInt32> workingRoute)
      {
        this.StartingLocation = startingLocation;
        this.RoadLength = roadLength;
        this.VisitedRoadSegments = new HashSet<RoadSegment>(visitedRoadSegments);
        this.VisitedRoadSegments.Add(segment);
        this.WorkingRoute = new List<UInt32>(workingRoute);
        this.WorkingRoute.Add(startingLocation);
      }
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