
namespace Jabberwocky.SoC.Library.PlayerActions
{
    using System;

    public class AcceptDirectTradeAction : PlayerAction
    {
        public readonly Guid SellerId;

        public AcceptDirectTradeAction(Guid initiatingPlayerId, Guid sellerId) : base(initiatingPlayerId)
        {
            this.SellerId = sellerId;
        }
    }
}
