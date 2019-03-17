
namespace Jabberwocky.SoC.Library.PlayerActions
{
    using System;

    public class AnswerDirectTradeOfferAction : PlayerAction
    {
        public readonly ResourceClutch WantedResources;
        public readonly Guid InitialPlayerId;
        public AnswerDirectTradeOfferAction(Guid playerId, Guid initialPlayerId, ResourceClutch wantedResources) : base(playerId)
        {
            this.WantedResources = wantedResources;
            this.InitialPlayerId = initialPlayerId;
        }
    }
}
