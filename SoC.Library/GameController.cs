﻿
using System;
using Jabberwocky.SoC.Library.GameActions;
using Jabberwocky.SoC.Library.GameEvents;

namespace Jabberwocky.SoC.Library
{
    public class GameController
    {
        #region Fields
        private GameToken token;
        #endregion

        #region Construction
        internal void GameEventHandler(GameEvent gameEvent)
        {
            this.token = gameEvent.Token;
            this.GameEvent.Invoke(gameEvent);
        }
        #endregion

        #region Events
        public event Action<GameToken, PlayerAction> PlayerActionEvent;
        public event Action<GameEvent> GameEvent;
        public event Action<Exception> GameExceptionEvent;
        #endregion

        #region Methods
        public void EndTurn()
        {
            this.SendAction(new EndOfTurnAction());
        }

        public void MakeDirectTradeOffer(ResourceClutch resourceClutch)
        {
            this.SendAction(new MakeDirectTradeOfferAction(Guid.Empty, resourceClutch));
        }

        public void PlaceStartingInfrastructure(uint settlementLocation, uint roadEndLocation)
        {
            this.SendAction(new PlaceInfrastructureAction(settlementLocation, roadEndLocation));
        }

        public void RequestState()
        {
            this.SendAction(new RequestStateAction(Guid.Empty));
        }

        internal void GameExceptionHandler(Exception exception)
        {
            this.GameExceptionEvent.Invoke(exception);
        }

        private void SendAction(PlayerAction playerAction)
        {
            if (this.token == null)
                throw new Exception("No token");

            this.PlayerActionEvent.Invoke(this.token, playerAction);
        }
        #endregion
    }
}
