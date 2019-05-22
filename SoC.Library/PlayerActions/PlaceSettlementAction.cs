
namespace Jabberwocky.SoC.Library.PlayerActions
{
    using System;

    public class PlaceSettlementAction : PlayerAction
    {
        public readonly uint SettlementLocation;

        public PlaceSettlementAction(Guid playerId, uint settlementLocation) : base(playerId)
        {
            this.SettlementLocation = settlementLocation;
        }
    }
}