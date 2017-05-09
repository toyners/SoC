
namespace Jabberwocky.SoC.Library.GameBoards
{
  using System;

  /// <summary>
  /// Holds data for all locations, trails, settlements, cities, roads, resource providers and robber location.
  /// Provides methods for verifying locations for placing new settlements, cities and roads for a player.
  /// </summary>
  public class GameBoardData
  {
    public Boolean CanPlaceCity(Player player, Location location)
    {
      throw new NotImplementedException();
    }

    public Boolean CanPlaceRoad(Player player, Location startLocation, Location endLocation)
    {
      throw new NotImplementedException();
    }

    public Boolean CanPlaceSettlement(Player player, Location location)
    {
      throw new NotImplementedException();
    }
  }
}
