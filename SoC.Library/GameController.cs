
using System;
using Jabberwocky.SoC.Library.GameActions;
using Jabberwocky.SoC.Library.GameEvents;

namespace Jabberwocky.SoC.Library
{
    public class GameController
    {
        #region Fields
        private GameToken gameToken;
        private Guid gameId;
        #endregion

        #region Events
        public event Action<GameToken, PlayerAction> PlayerActionEvent;
        public event Action<GameEvent> GameEvent;
        public event Action<Exception> GameExceptionEvent;
        #endregion

        #region Methods
        public void AnswerDirectTradeOffer(ResourceClutch resourceClutch)
        {
            this.SendAction(new AnswerDirectTradeOfferAction(null, resourceClutch));
        }

        public void EndTurn()
        {
            this.SendAction(new EndOfTurnAction(this.gameId));
        }

        public void MakeDirectTradeOffer(ResourceClutch resourceClutch)
        {
            this.SendAction(new MakeDirectTradeOfferAction(this.gameId, resourceClutch));
        }

        public void PlaceStartingInfrastructure(uint settlementLocation, uint roadEndLocation)
        {
            this.SendAction(new PlaceInfrastructureAction(settlementLocation, roadEndLocation));
        }

        public void RequestState()
        {
            this.SendAction(new RequestStateAction(Guid.Empty));
        }

        internal void GameEventHandler(GameEvent gameEvent, GameToken gameToken)
        {
            this.gameToken = gameToken;
            if (gameEvent is GameJoinedEvent gameJoinedEvent)
                this.gameId = gameJoinedEvent.PlayerId;

            this.GameEvent.Invoke(gameEvent);
        }

        internal void GameExceptionHandler(Exception exception)
        {
            this.GameExceptionEvent.Invoke(exception);
        }

        private void SendAction(PlayerAction playerAction)
        {
            if (this.gameToken == null)
                throw new Exception($"No token for action {playerAction.GetType().Name}");

            this.PlayerActionEvent.Invoke(this.gameToken, playerAction);
        }
        #endregion
    }
}
