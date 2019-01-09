using System;
using System.Collections.Generic;
using Jabberwocky.SoC.Library;
using SoC.Library.ScenarioTests.PlayerTurn;

namespace SoC.Library.ScenarioTests.Builders
{
    internal class PlayerStateBuilder
    {
        private readonly BasePlayerTurn playerTurn;
        public PlayerSnapshot playerSnapshot = new PlayerSnapshot();
        public PlayerStateBuilder(BasePlayerTurn playerTurn)
        {
            this.playerTurn = playerTurn;
        }

        public PlayerStateBuilder HeldCards(params DevelopmentCardTypes[] cards)
        {
            this.playerSnapshot.heldCards = new List<DevelopmentCardTypes>(cards);
            return this;
        }

        public BasePlayerTurn End()
        {
            return this.playerTurn;
        }

        public PlayerStateBuilder VictoryPoints(int victoryPoints)
        {
            this.playerSnapshot.VictoryPoints = victoryPoints;
            return this;
        }
    }
}
