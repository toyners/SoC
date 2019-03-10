
namespace SoC.Library.ScenarioTests.Instructions
{
    using System;
    using System.Collections.Generic;
    using Jabberwocky.SoC.Library.GameEvents;
    using SoC.Library.ScenarioTests.ScenarioEvents;

    internal class ScenarioPlaceSetupInfrastructureEventInstruction : EventInstruction
    {
        public ScenarioPlaceSetupInfrastructureEventInstruction(string playerName) : base(playerName)
        {
        }

        public override GameEvent GetEvent(IDictionary<string, Guid> playerIdsByName)
        {
            return new ScenarioPlaceSetupInfrastructureEvent();
        }
    }
}
