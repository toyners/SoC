
namespace Jabberwocky.SoC.Library.GameBoards
{
  using System;
  using System.Collections.Generic;

  /// <summary>
  /// Holds information on new settlements, cities and roads. Also any change in the Robber location.
  /// </summary>
  public class GameBoardUpdate
  {
    public List<Tuple<uint, Guid>> NewSettlements;
    public List<Tuple<uint, uint, Guid>> NewRoads;
  }
}
