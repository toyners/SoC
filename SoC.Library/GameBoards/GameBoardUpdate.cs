
namespace Jabberwocky.SoC.Library.GameBoards
{
  using System;
  using System.Collections.Generic;

  /// <summary>
  /// Holds information on new settlements, cities and roads. Also any change in the Robber location.
  /// </summary>
  public class GameBoardUpdate
  {
    public Dictionary<Player, List<Location>> NewSettlements;
    public Dictionary<Player, List<Location>> NewCities;
    public Dictionary<Player, List<Tuple<Location, Location>>> NewRoads;
    public Location Robber;
  }
}
