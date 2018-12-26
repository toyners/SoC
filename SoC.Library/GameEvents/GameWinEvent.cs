
namespace Jabberwocky.SoC.Library.GameEvents
{
    using System;

    public class GameWinEvent : GameEvent
    {
        public readonly uint VictoryPoints;

        public GameWinEvent(Guid playerId, uint victoryPoints) : base(playerId)
        {
            this.VictoryPoints = victoryPoints;
        }

        public override bool Equals(object obj)
        {
            if (!base.Equals(obj))
                return false;

            return this.VictoryPoints == ((GameWinEvent)obj).VictoryPoints;
        }
    }
}
