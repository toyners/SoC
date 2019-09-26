
namespace Jabberwocky.SoC.Library.Interfaces
{
    using System;
    using Jabberwocky.SoC.Library.GameEvents;

    public interface IEventSender
    {
        void Send(GameEvent gameEvent, Guid playerId);
    }
}
