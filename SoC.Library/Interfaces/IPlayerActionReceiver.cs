
namespace Jabberwocky.SoC.Library.Interfaces
{
    using Jabberwocky.SoC.Library.PlayerActions;

    interface IPlayerActionReceiver
    {
        bool TryGetPlayerAction(out PlayerAction playerAction);
    }
}
