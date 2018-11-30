
namespace Jabberwocky.SoC.Library.GameEvents
{
    using System;

    public class CityBuiltEvent : SettlementBuiltEvent
    {
        public CityBuiltEvent(Guid playerId, uint location) : base(playerId, location)
        {
        }

        public override bool Equals(object obj)
        {
            if (!base.Equals(obj))
            {
                return false;
            }

            return this.Location == ((CityBuiltEvent)obj).Location;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
