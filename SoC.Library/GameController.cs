
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

        #region Properties
        public ResourceClutch Resources { get; private set; }
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

        public void SelectResourceFromPlayer(Guid selectedPlayerId)
        {
            this.SendAction(new SelectResourceFromPlayerAction(this.playerId, selectedPlayerId));
        }

        public void AnswerDirectTradeOffer(ResourceClutch resourceClutch)
        {
            this.SendAction(new AnswerDirectTradeOfferAction(
                this.playerId, 
                this.lastMakeDirectTradeOfferEvent.BuyingPlayerId, 
                resourceClutch));
        }

        public void BuyDevelopmentCard()
        {
            this.SendAction(new BuyDevelopmentCardAction(this.playerId));
        }

        public void ChooseResourcesToLose(ResourceClutch resourceClutch)
        {
            this.SendAction(new LoseResourcesAction(this.playerId, resourceClutch));
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

        public void PlaceCity(uint cityLocation)
        {
            this.SendAction(new PlaceCityAction(this.playerId, cityLocation));
        }

        public void PlaceRoadSegment(uint roadSegmentStartLocation, uint roadSegmentEndLocation)
        {
            this.SendAction(new PlaceRoadSegmentAction(this.playerId, roadSegmentStartLocation, roadSegmentEndLocation));
        }

        public void PlaceRobber(uint hex)
        {
            this.SendAction(new PlaceRobberAction(this.playerId, hex));
        }

        public void PlaceSettlement(uint settlementLocation)
        {
            this.SendAction(new PlaceSettlementAction(this.playerId, settlementLocation));
        }

        public void PlaceSetupInfrastructure(uint settlementLocation, uint roadEndLocation)
        {
            this.SendAction(new PlaceSetupInfrastructureAction(this.playerId, settlementLocation, roadEndLocation));
        }

        public void PlayKnightCard(uint hex)
        {
            this.SendAction(new PlayKnightCardAction(this.playerId, hex));
        }

        public void PlayRoadBuildingCard(uint firstRoadSegmentStartLocation, uint firstRoadSegmentEndLocation, uint secondRoadSegmentStartLocation, uint secondRoadSegmentEndLocation)
        {
            this.SendAction(new PlayRoadBuildingCardAction(this.playerId,
                firstRoadSegmentStartLocation, firstRoadSegmentEndLocation,
                secondRoadSegmentStartLocation, secondRoadSegmentEndLocation));
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
            else if (gameEvent is MakeDirectTradeOfferEvent makeDirectTradeOfferEvent)
                this.lastMakeDirectTradeOfferEvent = makeDirectTradeOfferEvent;
            else if (gameEvent is AnswerDirectTradeOfferEvent answerDirectTradeOfferEvent)
                this.lasetAnswerDirectTradeOffEvent = answerDirectTradeOfferEvent;
            else if (gameEvent is ResourcesCollectedEvent resourcesCollectedEvent && resourcesCollectedEvent.ResourcesCollectedByPlayerId.ContainsKey(this.playerId))
            {
                foreach (var resourceCollection in resourcesCollectedEvent.ResourcesCollectedByPlayerId[this.playerId])
                    this.Resources += resourceCollection.Resources;
            }

            this.GameEvent.Invoke(gameEvent);
        }

        private void SendAction(PlayerAction playerAction)
        {
            this.PlayerActionEvent.Invoke(playerAction);
        }
        #endregion
    }
}
