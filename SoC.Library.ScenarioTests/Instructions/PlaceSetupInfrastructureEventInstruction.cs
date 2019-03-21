
namespace SoC.Library.ScenarioTests.Instructions
{
    using System;
    using System.Collections.Generic;
    using Jabberwocky.SoC.Library.GameEvents;

    internal class PlaceSetupInfrastructureEventInstruction : EventInstruction
    {
        public PlaceSetupInfrastructureEventInstruction(string playerName) : base(playerName)
        {
        }

        public override GameEvent GetEvent(IDictionary<string, Guid> playerIdsByName)
        {
            return new PlaceSetupInfrastructureEvent();
        }
    }
}