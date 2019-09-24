
namespace Jabberwocky.SoC.Library.Interfaces
{
    using Jabberwocky.SoC.Library.GameEvents;

    public interface IEventReceiver
    {
        void Post(GameEvent gameEvent);
    }
}
