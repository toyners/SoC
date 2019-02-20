
using System;
using Jabberwocky.SoC.Library.GameActions;
using Jabberwocky.SoC.Library.GameEvents;

namespace Jabberwocky.SoC.Library
{
    public class GameController
    {
        public event Action<TurnToken, ComputerPlayerAction> PlayerActionEvent;
        public event Action<GameEvent> GameEvent;
        public event Action<Exception> GameExceptionEvent;

        internal void GameEventHandler(GameEvent gameEvent)
        {
            this.GameEvent.Invoke(gameEvent);
        }

        internal void GameExceptionHandler(Exception exception)
        {
            this.GameExceptionEvent.Invoke(exception);
        }

        public void SendAction(TurnToken turnToken, ComputerPlayerAction action)
        {
            this.PlayerActionEvent.Invoke(turnToken, action);
        }
    }
}
