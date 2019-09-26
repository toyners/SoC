using System;
using System.Collections.Generic;
using Jabberwocky.SoC.Library.GameEvents;
using Jabberwocky.SoC.Library.Interfaces;

namespace SoC.Library.ScenarioTests
{
    internal class ScenarioEventSender : IEventSender
    {
        private IDictionary<Guid, IEventReceiver> eventReceiversByPlayerId = new Dictionary<Guid, IEventReceiver>();
        public ScenarioEventSender(IDictionary<Guid, IEventReceiver> eventReceiversByPlayerId)
        {
            this.eventReceiversByPlayerId = eventReceiversByPlayerId;
        }

        public void Send(GameEvent gameEvent, Guid playerId)
        {
            this.eventReceiversByPlayerId[playerId].Post(gameEvent);
        }

        /*public void SendEvent(GameEvent gameEvent, IEnumerable<IPlayer> players)
        {
            if (!this.CanSendEvents)
                return;

            foreach (var player in players)
                this.eventReceiversByPlayerId[player.Id].Post(gameEvent);
        }*/
    }
}
