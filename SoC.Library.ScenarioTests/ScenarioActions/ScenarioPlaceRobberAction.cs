
namespace SoC.Library.ScenarioTests.ScenarioActions
{
    using System;
    using Jabberwocky.SoC.Library;
    using Jabberwocky.SoC.Library.PlayerActions;

    internal class ScenarioPlaceRobberAction : PlayerAction
    {
        public readonly uint NewRobberHex;
        public readonly ResourceClutch ResourcesToDrop;

        #region Construction
        public ScenarioPlaceRobberAction(uint newRobberHex) : this(newRobberHex, ResourceClutch.Zero)
        {
        }

        public ScenarioPlaceRobberAction(uint newRobberHex, ResourceClutch resourcesToDrop) : base(Guid.Empty)
        {
            this.NewRobberHex = newRobberHex;
            this.ResourcesToDrop = resourcesToDrop;
        }
        #endregion
    }
}
