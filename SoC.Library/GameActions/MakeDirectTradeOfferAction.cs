
namespace Jabberwocky.SoC.Library.GameActions
{
    using System;

    public class MakeDirectTradeOfferAction : ComputerPlayerAction
    {
        public readonly ResourceClutch WantedResources;
        public MakeDirectTradeOfferAction(Guid playerId, ResourceClutch wantedResources) : base(playerId)
        {
            this.WantedResources = wantedResources;
        }
    }
}
