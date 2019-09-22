
namespace Jabberwocky.SoC.Library.Interfaces
{
    using Jabberwocky.SoC.Library.PlayerActions;

    public interface IPlayerActionReceiver
    {
        void Post(PlayerAction playerAction);
    }
}
