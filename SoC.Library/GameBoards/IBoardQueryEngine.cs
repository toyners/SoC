
namespace Jabberwocky.SoC.Library.GameBoards
{
  using System;
  using System.Collections.Generic;

  public interface IBoardQueryEngine
  {
    uint[] GetLocationsWithBestYield(int count);

    /// <summary>
    /// Returns possible paths to settlement candidates from existing player roads. Returns shortest routes only.
    /// Ordered by the settlement candidates and then all other paths after that.
    /// </summary>
    /// <param name="settlementCandidates"></param>
    /// <returns></returns>
    List<KeyValuePair<uint, List<uint>>> GetRoadPathCandidates(List<uint> settlementCandidates);

    /// <summary>
    /// Get the longest road for a player.
    /// </summary>
    /// <param name="id">Id of player</param>
    /// <returns>Locations of the road.</returns>
    List<uint> GetLongestRoadForPlayer(Guid id);

    uint[] GetValidConnectedLocationsFrom(uint location);
    uint[] GetNeighbouringLocationsFrom(uint location);
  }
}
