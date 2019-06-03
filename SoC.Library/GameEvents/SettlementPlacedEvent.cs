
namespace Jabberwocky.SoC.Library.GameEvents
{
    using System;

    public class SettlementPlacedEvent : GameEvent
    {
        public readonly uint Location;

        public SettlementPlacedEvent(Guid playerId, uint location) : base(playerId)
        {
            this.Location = location;
        }

        /*public override bool Equals(object obj)
        {
            if (!base.Equals(obj))
            {
                return false;
            }

            return this.Location == ((SettlementPlacedEvent)obj).Location;
        }

        public override string ToString()
        {
            return $"{base.ToString()} in location {this.Location}";
        }*/
    }
}
