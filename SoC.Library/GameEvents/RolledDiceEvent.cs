
namespace Jabberwocky.SoC.Library.GameEvents
{
    using System;

    public class DiceRollEvent : GameEvent
    {
        public readonly uint Dice1;
        public readonly uint Dice2;

        public DiceRollEvent(Guid playerId, uint dice1, uint dice2) : base(playerId)
        {
            this.Dice1 = dice1;
            this.Dice2 = dice2;
        }

        public override bool Equals(object obj)
        {
            if (!base.Equals(obj))
            {
                return false;
            }

            var other = (DiceRollEvent)obj;
            return this.Dice1 == other.Dice1 && this.Dice2 == other.Dice2;
        }
    }
}
