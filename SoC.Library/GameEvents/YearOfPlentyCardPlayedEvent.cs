
namespace Jabberwocky.SoC.Library.GameEvents
{
    using System;

    public class YearOfPlentyCardPlayedEvent : GameEvent
    {
        public YearOfPlentyCardPlayedEvent(Guid playerId, ResourceTypes firstResource, ResourceTypes secondResource)
            : base(playerId)
        {
            this.FirstResource = firstResource;
            this.SecondResource = secondResource;
        }

        public ResourceTypes FirstResource { get; }
        public ResourceTypes SecondResource { get; }
    }
}
