
using Jabberwocky.SoC.Library.Enums;

namespace Jabberwocky.SoC.Library.GameActions
{
    public class BuildCityAction : PlayerAction
    {
        public readonly uint CityLocation;

        public BuildCityAction(uint cityLocation) : base()
        {
            this.CityLocation = cityLocation;
        }
    }
}