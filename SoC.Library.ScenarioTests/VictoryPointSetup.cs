
namespace SoC.Library.ScenarioTests
{
    internal class VictoryPointSetup : IPlayerSetupAction
    {
        private uint victoryPoints;
        public VictoryPointSetup(uint victoryPoints) => this.victoryPoints = victoryPoints;
        public void Process(ScenarioPlayer player) => player.SetVictoryPoints(this.victoryPoints);
    }
}
