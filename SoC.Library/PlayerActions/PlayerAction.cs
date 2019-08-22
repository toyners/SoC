
namespace Jabberwocky.SoC.Library.PlayerActions
{
    using System;

    public abstract class PlayerAction
    {
        public PlayerAction(Guid initiatingPlayerId) => this.InitiatingPlayerId = initiatingPlayerId;

        public Guid InitiatingPlayerId { get; }
    }
}
