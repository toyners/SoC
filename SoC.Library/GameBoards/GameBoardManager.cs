
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
      this.Data = new GameBoard(size);
    }

    public GameBoard Data { get; private set; }

    public void PlaceCity(PlayerData player, Location location)
    {
      throw new NotImplementedException();
    }

    public void PlaceRoad(PlayerData player, Location startLocation, Location endLocation)
    {
      throw new NotImplementedException();
    }

    public void PlaceTown(PlayerData player, Location location)
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
