

namespace Jabberwocky.SoC.Library.GameEvents
{
    using System;
    using System.Collections.Generic;

    public class StartPlayerTurnEvent : GameEvent
    {
        public readonly uint Dice1, Dice2;
        public readonly Dictionary<Guid, ResourceCollection[]> ResourcesCollected;
        public StartPlayerTurnEvent(Guid playerId, uint dice1, uint dice2, Dictionary<Guid, ResourceCollection[]> resourcesCollected)
            : this(playerId, dice1, dice2)
        {
            this.ResourcesCollected = resourcesCollected;
        }

        /*public StartPlayerTurnEvent(Guid playerId, uint dice1, uint dice2)
            : this(playerId, dice1, dice2)
        {
        }*/

        public StartPlayerTurnEvent(Guid playerId, uint dice1, uint dice2)
            : base(playerId)
        {
            this.Dice1 = dice1;
            this.Dice2 = dice2;
        }
    }
}
