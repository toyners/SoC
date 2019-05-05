
namespace Jabberwocky.SoC.Library.GameEvents
{
    using System;

    public class GameErrorEvent : GameEvent
    {
        public string ErrorCode;
        public GameErrorEvent(Guid playerId, string errorCode) : base(playerId)
        {
            this.ErrorCode = errorCode;
        }
    }
}
