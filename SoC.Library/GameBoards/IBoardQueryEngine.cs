
namespace Jabberwocky.SoC.Library.GameBoards
{
  using System;
  using System.Collections.Generic;

  public interface IBoardQueryEngine
  {
    uint[] GetLocationsWithBestYield(uint count);

    /// <summary>
    /// Returns possible paths to settlement candidates from existing player roads. Returns shortest routes only.
    /// Ordered by the settlement candidates.
    /// </summary>
    /// <param name="settlementCandidates"></param>
    /// <returns></returns>
    List<KeyValuePair<uint, List<uint>>> GetRoadPathCandidates(List<uint> settlementCandidates);
    List<uint> GetLongestRoadForPlayer(Guid id);
  }
}
