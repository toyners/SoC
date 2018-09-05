
namespace Jabberwocky.SoC.Library.GameBoards
{
  using System.Collections.Generic;

  public interface IBoardQueryEngine
  {
    uint[] GetLocationsWithBestYield(uint count);
    List<KeyValuePair<uint, List<uint>>> GetRoadSegmentCandidates(List<uint> settlementCandidates);
  }
}
