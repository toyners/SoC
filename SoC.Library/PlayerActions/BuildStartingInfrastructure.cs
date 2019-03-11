
namespace Jabberwocky.SoC.Library.GameActions
{
    public class BuildStartingInfrastructure : PlayerAction
    {
        public readonly uint SettlementLocation, RoadEnd;
        public BuildStartingInfrastructure(uint settlementLocation, uint roadEnd) : base(0)
        {
            this.SettlementLocation = settlementLocation;
            this.RoadEnd = roadEnd;
        }
    }
}
