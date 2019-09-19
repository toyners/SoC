
namespace Jabberwocky.SoC.Library.Interfaces
{
    using Jabberwocky.SoC.Library.PlayerActions;

    public interface IEventStore
    {
        void EnqueuePlayerAction(PlayerAction playerAction);
    }
}
