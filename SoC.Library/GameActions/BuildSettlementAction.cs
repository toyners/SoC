
using Jabberwocky.SoC.Library.Enums;

namespace Jabberwocky.SoC.Library.GameActions
{
    public class BuildSettlementAction : ComputerPlayerAction
    {
        private uint settlementLocation;

        public BuildSettlementAction(uint settlementLocation) : base(ComputerPlayerActionTypes.BuildSettlement)
        {
            this.settlementLocation = settlementLocation;
        }
    }
}