
namespace Jabberwocky.SoC.Library.GameEvents
{
    using System;
    using System.Diagnostics;

    [DebuggerDisplay("{GetType().Name}")]
    public class GameEvent
    {
        #region Fields
        public readonly Guid PlayerId;
        #endregion

        #region Construction
        public GameEvent(Guid playerId)
        {
            this.PlayerId = playerId;
        }
        #endregion

        #region Methods
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (this.GetType() != obj.GetType())
            {
                return false;
            }

            return this.PlayerId == ((GameEvent)obj).PlayerId;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return this.GetType().ToString();
        }
        #endregion
    }

    public class BuyDevelopmentCardEvent : GameEvent
    {
        public BuyDevelopmentCardEvent(Guid playerId) : base(playerId) { }
    }

    public class PlayKnightCardEvent : GameEvent
    {
        public PlayKnightCardEvent(Guid playerId) : base(playerId) { }
    }

    public class GameWinEvent : GameEvent
    {
        public GameWinEvent(Guid playerId) : base(playerId) { }
    }
}
