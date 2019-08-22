
namespace Jabberwocky.SoC.Library.PlayerActions
{
    using System;

    public class PlayYearOfPlentyCardAction : PlayerAction
    {
        public PlayYearOfPlentyCardAction(Guid initiatingPlayerId, ResourceTypes firstResource, ResourceTypes secondResource) : base(initiatingPlayerId)
        {
            this.FirstResource = firstResource;
            this.SecondResource = secondResource;
        }

        public ResourceTypes FirstResource { get; }
        public ResourceTypes SecondResource { get; }
    }
}
