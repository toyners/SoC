
namespace Jabberwocky.SoC.Library.GameActions
{
    using Enums;

    public class TradeWithBankAction : ComputerPlayerAction
    {
        public readonly ResourceTypes GivingType;
        public readonly ResourceTypes ReceivingType;
        public readonly int ReceivingCount;

        public TradeWithBankAction(ResourceTypes givingType, ResourceTypes receivingType, int receivingCount) : base(ComputerPlayerActionTypes.TradeWithBank)
        {
            this.GivingType = givingType;
            this.ReceivingType = receivingType;
            this.ReceivingCount = receivingCount;
        }
    }
}
