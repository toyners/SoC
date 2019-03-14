
namespace Jabberwocky.SoC.Library.GameActions
{
    using System;

    public class AnswerDirectTradeOfferAction : PlayerAction
    {
        public readonly string PlayerName;
        public readonly ResourceClutch OfferedResources;
        public AnswerDirectTradeOfferAction(string playerName, ResourceClutch offeredResources) : base(Guid.Empty)
        {
            this.PlayerName = playerName;
            this.OfferedResources = offeredResources;
        }
    }
}
