
namespace Jabberwocky.SoC.Library.GameActions
{
    using System;

    public class PlayerAction
    {
        public readonly Guid PlayerId;

        //public PlayerAction() { }

        public PlayerAction(Guid playerId)
        {
            this.PlayerId = playerId;
        }
    }
}
