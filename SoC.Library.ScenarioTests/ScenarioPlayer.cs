using System.Collections.Generic;
using Jabberwocky.SoC.Library;

namespace SoC.Library.ScenarioTests
{
    internal class ScenarioPlayer : Player
    {
        private LocalGameControllerScenarioRunner runner;
        internal ScenarioPlayer(string name, LocalGameControllerScenarioRunner runner) : base(name)
        {
            this.runner = runner;
        }

        internal ScenarioPlayer VictoryPoints(uint victoryPoints)
        {
            //this.VictoryPoints = victoryPoints;
            return this;
        }

        internal LocalGameControllerScenarioRunner End()
        {
            return this.runner;
        }
    }

    internal class PlayerSnapshot
    {
        public List<DevelopmentCardTypes> heldCards;
    }
}
