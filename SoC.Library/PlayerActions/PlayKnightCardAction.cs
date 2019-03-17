
namespace Jabberwocky.SoC.Library.PlayerActions
{
    using System;

    public class PlayKnightCardAction : PlayerAction
    {
        public readonly uint NewRobberHex;
        public readonly Guid? PlayerId;
        public PlayKnightCardAction(uint newRobberHex, Guid? playerId = null) : base(Guid.Empty)
        {
            this.NewRobberHex = newRobberHex;
            this.PlayerId = playerId;
        }
    }
}
