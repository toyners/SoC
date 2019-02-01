
namespace Jabberwocky.SoC.Library.GameActions
{
    using System;

    public class SelectResourceFromPlayerAction : ComputerPlayerAction
    {
        public readonly Guid PlayerId;
        public SelectResourceFromPlayerAction(Guid playerId) : base(0)
        {
            this.PlayerId = playerId;
        }
    }
}
