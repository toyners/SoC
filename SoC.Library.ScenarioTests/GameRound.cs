using System.Collections.Generic;
using SoC.Library.ScenarioTests.PlayerTurn;

namespace SoC.Library.ScenarioTests
{
    internal class GameRound
    {
        public List<BasePlayerTurn> PlayerTurns = new List<BasePlayerTurn>();

        public bool IsComplete { get { return this.PlayerTurns.Count == 4; } }

        public void Add(BasePlayerTurn playerTurn)
        {
            this.PlayerTurns.Add(playerTurn);
        }
    }
}
