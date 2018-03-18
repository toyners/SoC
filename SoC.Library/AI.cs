
namespace Jabberwocky.SoC.Library
{
  using System;
  using GameBoards;

  public class AI
  {
    public AI(GameBoard gameBoard)
    {

    }

    public UInt32[] GetLocationsForBestReturningResourceType(GameBoard gameBoard, ResourceTypes resourceType, out UInt32 productionFactor)
    {
      return gameBoard.GetLocationsForResourceTypeWithProductionFactors(resourceType, out productionFactor);
    }

    public UInt32[] GetRouteFromPlayerInfrastructureToLocation(GameBoard gameBoard, UInt32 location, Guid playerId)
    {
      throw new NotImplementedException();
    }

    public UInt32[] GetLocationsSharedByResourceProducers()
    {
      throw new NotImplementedException();
    }

    public Tuple<UInt32, UInt32> GetLocationsOnHexClosestToAnotherHex(UInt32 hex1, UInt32 hex2)
    {
      throw new NotImplementedException();
    }

    public GameBoard.LocationProduction GetLocationsForResourceType(GameBoard gameBoard, ResourceTypes resourceType)
    {
      throw new NotImplementedException();
    }

    public GameBoard.ResourceProducerLocation GetBestResourceProducerWithViableSettlementLocations(ResourceTypes resourceType)
    {
      throw new NotImplementedException();
    }

    // Starting location, resource type location, production factor, distance in steps, 
    public static Tuple<UInt32, UInt32, UInt32, UInt32>[] GetLocationsOfResourceTypeInOrderOfIncreasingDistanceToCandidateLocations(UInt32[] locations, ResourceTypes resourceType)
    {
      throw new NotImplementedException();
    }
  }

  public class LocationPairing
  {
    public readonly UInt32 Location1;
    public readonly UInt32 Location2;

    public LocationPairing(UInt32 location1, UInt32 location2)
    {
      this.Location1 = location1;
      this.Location2 = location2;
    }
  }
}
