
namespace Jabberwocky.SoC.Library.PlayerActions
{
    using System;

    public class SelectResourceFromPlayerAction : PlayerAction
    {
        public Guid SelectedPlayerId;
        public SelectResourceFromPlayerAction(Guid selectedPlayerId) : base(Guid.Empty)
            => this.SelectedPlayerId = selectedPlayerId;
    }
}
