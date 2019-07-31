
namespace Jabberwocky.SoC.Library.GameEvents
{
    using System;

    public class LongestRoadBuiltEvent : LargestArmyChangedEvent
    {
        public readonly uint[] Locations;
        public LongestRoadBuiltEvent(Guid playerId, uint[] locations, Guid? previousPlayerWithLongestRoadId) 
            : base(playerId, previousPlayerWithLongestRoadId)
            => this.Locations = locations;
    }
}
