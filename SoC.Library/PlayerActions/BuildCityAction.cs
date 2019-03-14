
namespace Jabberwocky.SoC.Library.GameActions
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