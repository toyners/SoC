
namespace Jabberwocky.SoC.Library.Interfaces
{
    using Jabberwocky.SoC.Library.GameEvents;

    public interface IEventReceiver
    {
        //bool TryGetPlayerAction(out PlayerAction playerAction);
        void Receive(GameEvent gameEvent);
    }
}
