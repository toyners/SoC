
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
    public Dictionary<UInt32, Guid> NewSettlements;
    public Dictionary<IPlayer, List<Location>> NewCities;
    public Dictionary<Road, Guid> NewRoads;
    public Location Robber;

    public static GameBoardUpdate operator+ (GameBoardUpdate a, GameBoardUpdate b)
    {
      throw new NotImplementedException();
    }
  }
}
