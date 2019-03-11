
using System;
using Jabberwocky.SoC.Library.GameActions;
using Jabberwocky.SoC.Library.GameEvents;

namespace Jabberwocky.SoC.Library
{
    public class GameController
    {
        private TurnToken turnToken;
        public event Action<TurnToken, ComputerPlayerAction> PlayerActionEvent;
        public event Action<GameEvent> GameEvent;
        public event Action<Exception> GameExceptionEvent;

        internal void GameEventHandler(GameEvent gameEvent)
        {
            if (gameEvent is PlaceSetupInfrastructureEvent placeSetupInfrastructureEventArgs)
                this.turnToken = placeSetupInfrastructureEventArgs.TurnToken;
            
            this.GameEvent.Invoke(gameEvent);
        }

        internal void GameExceptionHandler(Exception exception)
        {
            this.GameExceptionEvent.Invoke(exception);
        }

        public void PlaceStartingInfrastructure(uint settlementLocation, uint roadEndLocation)
        {
            this.PlayerActionEvent.Invoke(this.turnToken, new PlaceInfrastructureAction(settlementLocation, roadEndLocation));
        }

        public void EndTurn()
        {
            this.PlayerActionEvent.Invoke(this.turnToken, new EndOfTurnAction());
        }

        public void RequestState()
        {
            this.PlayerActionEvent.Invoke(null, new RequestStateAction(Guid.Empty));
        }

        public void MakeDirectTradeOffer(ResourceClutch resourceClutch)
        {
            this.PlayerActionEvent.Invoke(this.turnToken, new MakeDirectTradeOfferAction(Guid.Empty, resourceClutch));
        }
    }
}
