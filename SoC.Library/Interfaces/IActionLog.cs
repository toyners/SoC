
using Jabberwocky.SoC.Library.GameEvents;
using Jabberwocky.SoC.Library.PlayerActions;

namespace Jabberwocky.SoC.Library.Interfaces
{
    public interface IActionLog
    {
        void AddGameEvent(GameEvent actualEvent);
        void AddPlayerAction(PlayerAction playerAction);
    }
}
