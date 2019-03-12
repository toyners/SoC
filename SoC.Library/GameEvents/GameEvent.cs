
namespace Jabberwocky.SoC.Library.GameEvents
{
    using System;
    using System.Diagnostics;

    [DebuggerDisplay("{GetType().Name}")]
    public class GameEvent
    {
        public enum GameTokenAction
        {
            New,
            Delete,
            Hold
        }

        #region Fields
        public readonly Guid PlayerId;
        #endregion

        #region Construction
        public GameEvent(Guid playerId)
        {
            this.PlayerId = playerId;
        }

        public GameEvent(GameToken token)
        {
            this.Token = token;
            this.TokenPhase = token != null ? GameTokenAction.New : GameTokenAction.Delete;
        }

        public GameEvent()
        {
            this.TokenPhase = GameTokenAction.Hold;
        }
        #endregion

        public GameToken Token { get; private set; }
        public GameTokenAction TokenPhase { get; private set; }

        #region Methods
        public override bool Equals(object obj)
        {
            if (obj == null || this.GetType() != obj.GetType())
                return false;

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
}
