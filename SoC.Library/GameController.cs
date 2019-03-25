
namespace Jabberwocky.SoC.Library
{
    using System;
    using Jabberwocky.SoC.Library.GameEvents;
    using Jabberwocky.SoC.Library.PlayerActions;

    public class GameController
    {
        #region Fields
        private GameToken gameToken;
        private Guid playerId;
        private MakeDirectTradeOfferEvent lastMakeDirectTradeOfferEvent;
        private AnswerDirectTradeOfferEvent lasetAnswerDirectTradeOffEvent;
        #endregion

        #region Events
        public event Action<GameToken, PlayerAction> PlayerActionEvent;
        public event Action<GameEvent> GameEvent;
        public event Action<Exception> GameExceptionEvent;
        #endregion

        #region Methods
        public void AcceptDirectTradeOffer(Guid sellerId)
        {
            this.SendAction(new AcceptDirectTradeAction(this.playerId, sellerId));
        }

        public void AnswerDirectTradeOffer(ResourceClutch resourceClutch)
        {
            this.SendAction(new AnswerDirectTradeOfferAction(
                this.playerId, 
                this.lastMakeDirectTradeOfferEvent.BuyingPlayerId, 
                resourceClutch));
        }

        public void EndTurn()
        {
            this.SendAction(new EndOfTurnAction(this.playerId));
        }

        public void MakeDirectTradeOffer(ResourceClutch resourceClutch)
        {
            this.SendAction(new MakeDirectTradeOfferAction(this.playerId, resourceClutch));
        }

        public void PlaceSetupInfrastructure(uint settlementLocation, uint roadEndLocation)
        {
            this.SendAction(new PlaceSetupInfrastructureAction(this.playerId, settlementLocation, roadEndLocation));
        }

        public void RequestState()
        {
            this.SendAction(new RequestStateAction(this.playerId));
        }

        internal void GameEventHandler(GameEvent gameEvent, GameToken gameToken)
        {
            this.gameToken = gameToken;
            if (gameEvent is GameJoinedEvent gameJoinedEvent)
                this.playerId = gameJoinedEvent.PlayerId;

            if (gameEvent is MakeDirectTradeOfferEvent makeDirectTradeOfferEvent)
                this.lastMakeDirectTradeOfferEvent = makeDirectTradeOfferEvent;
        
            if (gameEvent is AnswerDirectTradeOfferEvent answerDirectTradeOfferEvent)
                this.lasetAnswerDirectTradeOffEvent = answerDirectTradeOfferEvent;

            this.GameEvent.Invoke(gameEvent);
        }

        internal void GameExceptionHandler(Exception exception)
        {
            this.GameExceptionEvent.Invoke(exception);
        }

        private void SendAction(PlayerAction playerAction)
        {
            this.PlayerActionEvent.Invoke(this.gameToken, playerAction);
        }
        #endregion
    }
}
