
namespace Jabberwocky.SoC.Library.GameEvents
{
    using System;

    public class GameErrorEvent : GameEvent
    {
        public string ErrorCode;
        public string ErrorMessage;
        public GameErrorEvent(Guid playerId, string errorCode, string errorMessage) : base(playerId)
        {
            this.ErrorCode = errorCode;
            this.ErrorMessage = errorMessage;
        }
    }
}
