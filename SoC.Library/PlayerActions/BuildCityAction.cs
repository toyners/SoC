
namespace Jabberwocky.SoC.Library.PlayerActions
{
    using System;

    public class BuildCityAction : PlayerAction
    {
        public readonly uint CityLocation;

        public BuildCityAction(uint cityLocation) : base(Guid.Empty)
        {
            this.CityLocation = cityLocation;
        }
    }
}