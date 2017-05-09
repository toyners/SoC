
namespace Jabberwocky.SoC.Library.GameBoards
{
  using System;

  /// <summary>
  /// Holds all locations, trails and resource providers
  /// </summary>
  public class GameBoardManager
  {
    public GameBoardManager(BoardSizes size)
    {

    }

    public GameBoardData Data { get; private set; }

    public void PlaceCity(Player player, Location location)
    {
      throw new NotImplementedException();
    }

    public void PlaceRoad(Player player, Location startLocation, Location endLocation)
    {
      throw new NotImplementedException();
    }

    public void PlaceTown(Player player, Location location)
    {
      throw new NotImplementedException();
    }

    public void PlaceStartingTown(Location location)
    {
      throw new NotImplementedException();
    }

    public void ProduceResources(UInt32 productionNumber)
    {
      throw new NotImplementedException();
    }
  }
}
