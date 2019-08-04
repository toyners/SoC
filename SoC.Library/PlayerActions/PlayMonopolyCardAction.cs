
namespace Jabberwocky.SoC.Library.PlayerActions
{
    using System;

    public class PlayMonopolyCardAction : PlayerAction
    {
        public PlayMonopolyCardAction(Guid initiatingPlayerId, ResourceTypes resourceType) : base(initiatingPlayerId)
            => this.ResourceType = resourceType;

        public ResourceTypes ResourceType { get; private set; }
    }
}
