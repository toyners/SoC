﻿
namespace Jabberwocky.SoC.Library.GameBoards
{
  using System;
  using System.Collections.Generic;

  /// <summary>
  /// Holds information on new settlements, cities and roads. Also any change in the Robber location.
  /// </summary>
  public class GameBoardUpdate
  {
    public Dictionary<PlayerDataBase, List<UInt32>> NewSettlements;
    public Dictionary<PlayerDataBase, List<Location>> NewCities;
    public Dictionary<PlayerDataBase, List<Trail>> NewRoads;
    public Location Robber;
  }
}
