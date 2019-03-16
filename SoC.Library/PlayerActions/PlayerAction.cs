
namespace Jabberwocky.SoC.Library.GameActions
{
    using System;

    public abstract class PlayerAction
    {
        public readonly Guid PlayerId;

        public PlayerAction(Guid playerId)
        {
            this.PlayerId = playerId;
        }
    }

    public class TokenConstraintedPlayerAction : PlayerAction
    {
        public TokenConstraintedPlayerAction(Guid playerId) : base(playerId)
        {
        }
    }
}
