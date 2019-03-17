
namespace SoC.Library.ScenarioTests.ScenarioActions
{
    using System;
    using Jabberwocky.SoC.Library.PlayerActions;

    public class ScenarioVerifySnapshotAction : PlayerAction
    {
        private PlayerState playerState;
        public ScenarioVerifySnapshotAction(PlayerState playerState) : base(Guid.Empty)
        {
            this.playerState = playerState;
        }

        public void Verify()
        {
            this.playerState.Verify();
        }
    }
}
