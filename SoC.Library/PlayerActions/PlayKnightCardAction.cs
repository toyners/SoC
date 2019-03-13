
using System;

namespace Jabberwocky.SoC.Library.GameActions
{
    public class PlayKnightCardAction : PlayerAction
    {
        public readonly uint NewRobberHex;
        public readonly Guid? PlayerId;
        public PlayKnightCardAction(uint newRobberHex, Guid? playerId = null) : base()
        {
            this.NewRobberHex = newRobberHex;
            this.PlayerId = playerId;
        }
    }
}
