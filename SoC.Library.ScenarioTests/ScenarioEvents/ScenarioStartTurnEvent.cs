using System;
using System.Collections.Generic;
using Jabberwocky.SoC.Library;
using Jabberwocky.SoC.Library.GameEvents;

namespace SoC.Library.ScenarioTests.ScenarioEvents
{
    public class ScenarioStartTurnEvent : GameEvent
    {
        public readonly uint Dice1, Dice2;
        public readonly Dictionary<string, ResourceCollection[]> CollectedResources;

        public ScenarioStartTurnEvent(Guid playerId, uint dice1, uint dice2, Dictionary<string, ResourceCollection[]> collectedResources)
            : base(playerId)
        {
            this.Dice1 = dice1;
            this.Dice2 = dice2;
            this.CollectedResources = collectedResources;
        }
    }
}
