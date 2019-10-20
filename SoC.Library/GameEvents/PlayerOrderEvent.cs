
namespace Jabberwocky.SoC.Library.GameEvents
{
    using System;

    public class PlayerOrderEvent : GameEvent
    {
        public PlayerOrderEvent(Guid[] playerIds) : base(Guid.Empty) => this.PlayerIds = playerIds;
        
        public Guid[] PlayerIds { get; set; }
    }
}