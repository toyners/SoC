
namespace Jabberwocky.SoC.Library.PlayerActions
{
    using System;

    public class PlayKnightCardAction : PlayerAction
    {
        public PlayKnightCardAction(Guid playerId, uint newRobberHex) : base(playerId)
            => this.NewRobberHex = newRobberHex;

        public uint NewRobberHex { get; private set; }
    }
}
