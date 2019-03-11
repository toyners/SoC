
using System;
using Jabberwocky.SoC.Library.GameActions;
using Jabberwocky.SoC.Library.GameEvents;

namespace Jabberwocky.SoC.Library
{
    public class GameController
    {
        private TurnToken token;
        public event Action<TurnToken, PlayerAction> PlayerActionEvent;
        public event Action<GameEvent> GameEvent;
        public event Action<Exception> GameExceptionEvent;

        internal void GameEventHandler(GameEvent gameEvent)
        {
            if (gameEvent is PlaceSetupInfrastructureEvent placeSetupInfrastructureEventArgs)
                this.token = placeSetupInfrastructureEventArgs.TurnToken;
            
            this.GameEvent.Invoke(gameEvent);
        }

        internal void GameExceptionHandler(Exception exception)
        {
            this.GameExceptionEvent.Invoke(exception);
        }

        public void PlaceStartingInfrastructure(uint settlementLocation, uint roadEndLocation)
        {
            this.PlayerActionEvent.Invoke(this.token, new PlaceInfrastructureAction(settlementLocation, roadEndLocation));
        }

        public void EndTurn()
        {
            this.PlayerActionEvent.Invoke(this.token, new EndOfTurnAction());
        }

        public void RequestState()
        {
            this.PlayerActionEvent.Invoke(null, new RequestStateAction(Guid.Empty));
        }

        public void MakeDirectTradeOffer(ResourceClutch resourceClutch)
        {
            this.PlayerActionEvent.Invoke(this.token, new MakeDirectTradeOfferAction(Guid.Empty, resourceClutch));
        }
    }
}
