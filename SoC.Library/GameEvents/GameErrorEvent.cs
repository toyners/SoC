
namespace Jabberwocky.SoC.Library.GameEvents
{
    using System;

    public class GameErrorEvent : GameEvent
    {
        public GameErrorEvent(Guid playerId, int errorCode, string errorMessage) : base(playerId)
        {
            this.ErrorCode = errorCode;
            this.ErrorMessage = errorMessage;
        }

        public int ErrorCode { get; private set; }
        public string ErrorMessage { get; private set; }
    }
}
