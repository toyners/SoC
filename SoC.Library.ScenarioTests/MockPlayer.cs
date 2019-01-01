using Jabberwocky.SoC.Library;

namespace SoC.Library.ScenarioTests
{
    internal class MockPlayer : Player
    {
        internal MockPlayer(string name) : base(name) { }

        internal void SetVictoryPoints(uint victoryPoints)
        {
            this.VictoryPoints = victoryPoints;
        }
    }
}
