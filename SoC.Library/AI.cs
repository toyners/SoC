
namespace Jabberwocky.SoC.Library
{
  using System;
  using GameBoards;

  public static class AI
  {
    public static UInt32[] GetPossibleSettlementLocationsForBestReturningResourceType(GameBoard gameBoard, ResourceTypes resourceType, out Int32 productionFactor)
    {
      // Get locations for resources of type: return locations and their production factor
      // Order by production factor
      // Verify that the location is viable for settlement by using CanPlaceSettlement
      // Add to list

      var locations = gameBoard.GetLocationsForResourceTypeWithProductionFactors(resourceType);

      throw new NotImplementedException();
    }

    public static UInt32[] GetRouteFromPlayerInfrastructureToLocation(GameBoard gameBoard, UInt32 location, Guid playerId)
    {
      throw new NotImplementedException();
    }
  }
}
