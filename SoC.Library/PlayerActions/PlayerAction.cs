
namespace Jabberwocky.SoC.Library.PlayerActions
{
    using System;

    public abstract class PlayerAction
    {
        public readonly Guid InitiatingPlayerId;

        public PlayerAction(Guid initiatingPlayerId)
        {
            this.InitiatingPlayerId = initiatingPlayerId;
        }
    }

    public class TokenConstraintedPlayerAction : PlayerAction
    {
        public TokenConstraintedPlayerAction(Guid playerId) : base(playerId)
        {
        }
    }
}
