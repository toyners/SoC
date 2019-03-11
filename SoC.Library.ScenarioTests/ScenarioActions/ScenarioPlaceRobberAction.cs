
using Jabberwocky.SoC.Library;
using Jabberwocky.SoC.Library.GameActions;

namespace SoC.Library.ScenarioTests.ScenarioActions
{
    internal class ScenarioPlaceRobberAction : PlayerAction
    {
        public readonly uint NewRobberHex;
        public readonly ResourceClutch ResourcesToDrop;

        #region Construction
        public ScenarioPlaceRobberAction(uint newRobberHex) : this(newRobberHex, ResourceClutch.Zero)
        {
        }

        public ScenarioPlaceRobberAction(uint newRobberHex, ResourceClutch resourcesToDrop) : base(0)
        {
            this.NewRobberHex = newRobberHex;
            this.ResourcesToDrop = resourcesToDrop;
        }
        #endregion
    }
}
