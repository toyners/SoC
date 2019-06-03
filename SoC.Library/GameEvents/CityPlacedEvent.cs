
namespace Jabberwocky.SoC.Library.GameEvents
{
    using System;

    public class CityPlacedEvent : SettlementPlacedEvent
    {
        public CityPlacedEvent(Guid playerId, uint location) : base(playerId, location)
        {
        }

        /*public override bool Equals(object obj)
        {
            if (!base.Equals(obj))
            {
                return false;
            }

            return this.Location == ((CityPlacedEvent)obj).Location;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }*/
    }
}
