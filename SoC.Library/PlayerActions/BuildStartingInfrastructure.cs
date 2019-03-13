
namespace Jabberwocky.SoC.Library.GameActions
{
    public class BuildStartingInfrastructure : PlayerAction
    {
        public readonly uint SettlementLocation, RoadEnd;
        public BuildStartingInfrastructure(uint settlementLocation, uint roadEnd) : base()
        {
            this.SettlementLocation = settlementLocation;
            this.RoadEnd = roadEnd;
        }
    }
}
