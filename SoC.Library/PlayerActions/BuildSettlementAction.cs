
using Jabberwocky.SoC.Library.Enums;

namespace Jabberwocky.SoC.Library.GameActions
{
    public class BuildSettlementAction : PlayerAction
    {
        public readonly uint SettlementLocation;

        public BuildSettlementAction(uint settlementLocation) : base()
        {
            this.SettlementLocation = settlementLocation;
        }
    }
}