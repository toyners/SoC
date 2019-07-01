
namespace Jabberwocky.SoC.Library.PlayerActions
{
    using System;

    public class SelectResourceFromPlayerAction : PlayerAction
    {
        public Guid SelectedPlayerId;
        public SelectResourceFromPlayerAction(Guid initiatingPlayerId, Guid selectedPlayerId) : base(initiatingPlayerId)
            => this.SelectedPlayerId = selectedPlayerId;
    }
}
