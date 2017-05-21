
namespace Jabberwocky.SoC.Library.GameBoards
{
  using System;
  using System.Collections.Generic;

  /// <summary>
  /// Holds information on new settlements, cities and roads. Also any change in the Robber location.
  /// </summary>
  public class GameBoardUpdate
  {
    public Dictionary<PlayerBase, List<Location>> NewSettlements;
    public Dictionary<PlayerBase, List<Location>> NewCities;
    public Dictionary<PlayerBase, List<Trail>> NewRoads;
    public Location Robber;
  }
}
