
namespace Jabberwocky.SoC.Library.GameActions
{
    using System;

    public class SelectResourceFromPlayerAction : PlayerAction
    {
        public readonly Guid PlayerId;
        public SelectResourceFromPlayerAction(Guid playerId) : base(0)
        {
            this.PlayerId = playerId;
        }
    }
}
