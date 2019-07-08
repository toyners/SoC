
namespace Jabberwocky.SoC.Library.PlayerActions
{
    using System;

    public class PlayKnightCardAction : PlayerAction
    {
        public readonly uint NewRobberHex;
        public PlayKnightCardAction(Guid playerId, uint newRobberHex) : base(playerId)
        {
            this.NewRobberHex = newRobberHex;
        }
    }
}
