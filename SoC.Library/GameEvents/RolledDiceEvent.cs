
namespace Jabberwocky.SoC.Library.GameEvents
{
    using System;

    public class RolledDiceEvent : GameEvent
    {
        public readonly uint Dice1;
        public readonly uint Dice2;

        public RolledDiceEvent(Guid playerId, uint dice1, uint dice2) : base(playerId)
        {
            this.Dice1 = dice1;
            this.Dice2 = dice2;
        }
    }
}
