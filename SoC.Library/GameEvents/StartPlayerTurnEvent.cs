

namespace Jabberwocky.SoC.Library.GameEvents
{
    using System;
    using System.Collections.Generic;

    public class StartTurnEvent : GameEvent
    {
        public readonly uint Dice1, Dice2;
        public readonly Dictionary<Guid, ResourceCollection[]> CollectedResources;
        public StartTurnEvent(Guid playerId, uint dice1, uint dice2, Dictionary<Guid, ResourceCollection[]> collectedResources)
            : this(playerId, dice1, dice2)
        {
            this.CollectedResources = collectedResources;
        }

        public StartTurnEvent(Guid playerId, uint dice1, uint dice2)
            : base(playerId)
        {
            this.Dice1 = dice1;
            this.Dice2 = dice2;
        }
    }
}
