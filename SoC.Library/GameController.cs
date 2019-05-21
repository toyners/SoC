
namespace Jabberwocky.SoC.Library
{
    using System;
    using Jabberwocky.SoC.Library.GameEvents;
    using Jabberwocky.SoC.Library.PlayerActions;

    public class GameController
    {
        #region Fields
        private Guid playerId;
        private MakeDirectTradeOfferEvent lastMakeDirectTradeOfferEvent;
        private AnswerDirectTradeOfferEvent lasetAnswerDirectTradeOffEvent;
        #endregion

        #region Events
        public event Action<PlayerAction> PlayerActionEvent;
        public event Action<GameEvent> GameEvent;
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

        public void ConfirmStart()
        {
            this.SendAction(new ConfirmGameStartAction(this.playerId));
        }

        public void EndTurn()
        {
            this.SendAction(new EndOfTurnAction(this.playerId));
        }

        public void MakeDirectTradeOffer(ResourceClutch resourceClutch)
        {
            this.SendAction(new MakeDirectTradeOfferAction(this.playerId, resourceClutch));
        }

        public void PlaceRoadSegment(uint roadSegmentStartLocation, uint roadSegmentEndLocation)
        {
            this.SendAction(new PlaceRoadSegmentAction(this.playerId, roadSegmentStartLocation, roadSegmentEndLocation));
        }

        public void PlaceSettlement(uint v)
        {
            throw new NotImplementedException();
        }

        public void PlaceSetupInfrastructure(uint settlementLocation, uint roadEndLocation)
        {
            this.SendAction(new PlaceSetupInfrastructureAction(this.playerId, settlementLocation, roadEndLocation));
        }

        public void RequestState()
        {
            this.SendAction(new RequestStateAction(this.playerId));
        }

        public void QuitGame()
        {
            this.SendAction(new QuitGameAction(this.playerId));
        }

        internal void GameEventHandler(GameEvent gameEvent)
        {
            if (gameEvent is GameJoinedEvent gameJoinedEvent)
                this.playerId = gameJoinedEvent.PlayerId;

            if (gameEvent is MakeDirectTradeOfferEvent makeDirectTradeOfferEvent)
                this.lastMakeDirectTradeOfferEvent = makeDirectTradeOfferEvent;
        
            if (gameEvent is AnswerDirectTradeOfferEvent answerDirectTradeOfferEvent)
                this.lasetAnswerDirectTradeOffEvent = answerDirectTradeOfferEvent;

            this.GameEvent.Invoke(gameEvent);
        }

        private void SendAction(PlayerAction playerAction)
        {
            this.PlayerActionEvent.Invoke(playerAction);
        }
        #endregion
    }
}
