
using Jabberwocky.SoC.Library.Enums;

namespace Jabberwocky.SoC.Library.GameActions
{
    public class BuildCityAction : PlayerAction
    {
        public readonly uint CityLocation;

        public BuildCityAction(uint cityLocation) : base(ComputerPlayerActionTypes.BuildCity)
        {
            this.CityLocation = cityLocation;
        }
    }
}