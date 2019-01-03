using Jabberwocky.SoC.Library;

namespace SoC.Library.ScenarioTests
{
    internal class ScenarioPlayer : Player
    {
        internal ScenarioPlayer(string name) : base(name) { }

        internal void SetVictoryPoints(uint victoryPoints)
        {
            this.VictoryPoints = victoryPoints;
        }
    }
}
