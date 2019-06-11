
namespace Jabberwocky.SoC.Library.PlayerActions
{
    using System;

    public class LoseResourcesAction : PlayerAction
    {
        public readonly ResourceClutch Resources;

        public LoseResourcesAction(Guid playerId, ResourceClutch resources) : base(playerId)
        {
            this.Resources = resources;
        }
    }
}
