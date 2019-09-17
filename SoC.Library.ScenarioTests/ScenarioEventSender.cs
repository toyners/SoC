using System;
using System.Collections.Generic;
using Jabberwocky.SoC.Library.GameEvents;
using Jabberwocky.SoC.Library.Interfaces;

namespace SoC.Library.ScenarioTests
{
    internal class ScenarioEventSender : IEventSender
    {
        private IDictionary<Guid, Action<GameEvent>> gameEventHandlersByPlayerId = new Dictionary<Guid, Action<GameEvent>>();
        public ScenarioEventSender(IDictionary<Guid, Action<GameEvent>> gameEventHandlersByPlayerId)
        {
            this.gameEventHandlersByPlayerId = gameEventHandlersByPlayerId;
        }

        public bool CanSendEvents { get; set; } = true;

        public void SendEvent(GameEvent gameEvent, Guid playerId)
        {
            if (!this.CanSendEvents)
                return;

            this.gameEventHandlersByPlayerId[playerId].Invoke(gameEvent);
        }

        public void SendEvent(GameEvent gameEvent, IEnumerable<IPlayer> players)
        {
            if (!this.CanSendEvents)
                return;

            foreach (var player in players)
                this.gameEventHandlersByPlayerId[player.Id].Invoke(gameEvent);
        }
    }
}
