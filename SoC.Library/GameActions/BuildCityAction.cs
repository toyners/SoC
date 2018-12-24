
using Jabberwocky.SoC.Library.Enums;

namespace Jabberwocky.SoC.Library.GameActions
{
    public class BuildCityAction : ComputerPlayerAction
    {
        public uint CityLocation;

        public BuildCityAction(uint cityLocation) : base(ComputerPlayerActionTypes.BuildCity)
        {
            this.CityLocation = cityLocation;
        }
    }
}