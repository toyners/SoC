
namespace Jabberwocky.SoC.Library.PlayerActions
{
    using System;

    public class TradeWithBankAction : PlayerAction
    {
        public readonly ResourceTypes GivingType;
        public readonly ResourceTypes ReceivingType;
        public readonly int ReceivingCount;

        public TradeWithBankAction(ResourceTypes givingType, ResourceTypes receivingType, int receivingCount) : base(Guid.Empty)
        {
            this.GivingType = givingType;
            this.ReceivingType = receivingType;
            this.ReceivingCount = receivingCount;
        }
    }
}
