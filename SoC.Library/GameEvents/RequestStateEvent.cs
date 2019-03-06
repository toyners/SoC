
namespace Jabberwocky.SoC.Library.GameEvents
{
    using System;

    public class RequestStateEvent : GameEvent
    {
        public RequestStateEvent(Guid playerId) : base(playerId)
        {
        }

        public ResourceClutch Resources { get; set; }
    }
}
