
namespace Jabberwocky.SoC.Library.Interfaces
{
    using System;
    using System.Collections.Generic;
    using Jabberwocky.SoC.Library.GameEvents;

    public interface IEventSender
    {
        bool CanSendEvents { get; set; }
        void SendEvent(GameEvent gameEvent, Guid playerId);
        void SendEvent(GameEvent gameEvent, IEnumerable<IPlayer> players);
    }
}
