
namespace Jabberwocky.SoC.Library.GameActions
{
    using System;

    public class DropResourcesAction : PlayerAction
    {
        public readonly ResourceClutch Resources;

        public DropResourcesAction(ResourceClutch resources) : base(Guid.Empty)
        {
            this.Resources = resources;
        }
    }
}
