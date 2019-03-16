
namespace Jabberwocky.SoC.Library.GameActions
{
    using System;

    public class AnswerDirectTradeOfferAction : TokenConstraintedPlayerAction
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
