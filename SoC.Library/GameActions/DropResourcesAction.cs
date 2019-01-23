using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jabberwocky.SoC.Library.Enums;

namespace Jabberwocky.SoC.Library.GameActions
{
    public class DropResourcesAction : ComputerPlayerAction
    {
        public readonly ResourceClutch Resources;

        public DropResourcesAction(ResourceClutch resources) : base(0)
        {
            this.Resources = resources;
        }
    }
}
