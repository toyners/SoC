
namespace Jabberwocky.SoC.Library.GameEvents
{
  using System;

  public class LongestRoadBuiltEvent : LargestArmyChangedEvent
  {
    public LongestRoadBuiltEvent(Guid playerId, Guid previousPlayerWithLongestRoadId) : base(playerId, previousPlayerWithLongestRoadId)
    {
    }
  }
}
