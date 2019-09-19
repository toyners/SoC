
namespace Jabberwocky.SoC.Library.Interfaces
{
    using Jabberwocky.SoC.Library.PlayerActions;

    public interface IEventReceiver
    {
        bool TryGetPlayerAction(out PlayerAction playerAction);
    }
}
