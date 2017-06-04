
namespace Jabberwocky.SoC.Library.GameBoards
{
  using System;
  using System.Collections.Generic;
  using Interfaces;

  /// <summary>
  /// Holds information on new settlements, cities and roads. Also any change in the Robber location.
  /// </summary>
  public class GameBoardUpdate
  {
    public Dictionary<IPlayer, List<UInt32>> NewSettlements;
    public Dictionary<IPlayer, List<Location>> NewCities;
    public Dictionary<IPlayer, List<Trail>> NewRoads;
    public Location Robber;
  }
}
