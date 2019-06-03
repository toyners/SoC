using System;

namespace Jabberwocky.SoC.Library.PlayerActions
{
    public class PlaceCityAction : PlayerAction
    {
        public readonly uint CityLocation;

        public PlaceCityAction(Guid playerId, uint cityLocation) : base(playerId)
        {
            this.CityLocation = cityLocation;
        }
    }
}
