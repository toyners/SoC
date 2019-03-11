using Jabberwocky.SoC.Library.GameActions;

namespace SoC.Library.ScenarioTests.ScenarioActions
{
    public class ScenarioVerifySnapshotAction : PlayerAction
    {
        private PlayerState playerState;
        public ScenarioVerifySnapshotAction(PlayerState playerState) : base(0)
        {
            this.playerState = playerState;
        }

        public void Verify()
        {
            this.playerState.Verify();
        }
    }
}
