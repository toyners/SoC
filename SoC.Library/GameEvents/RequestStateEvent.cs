
namespace Jabberwocky.SoC.Library.GameEvents
{
    using System;

    public class RequestStateEvent : GameEvent
    {
        public RequestStateEvent(Guid playerId) : base(playerId)
        {
        }

        public int Cities { get; set; }
        public ResourceClutch Resources { get; set; }
        public int RoadSegments { get; set; }
        public int Settlements { get; set; }
        public uint VictoryPoints { get; set; }
    }
}
