using System;
using System.Collections.Generic;
using Jabberwocky.SoC.Library;
using SoC.Library.ScenarioTests.PlayerTurn;

namespace SoC.Library.ScenarioTests.Builders
{
    internal class PlayerStateBuilder
    {
        #region Fields
        private readonly BasePlayerTurn playerTurn;
        private readonly PlayerSnapshot playerSnapshot;
        #endregion

        #region Construction
        public PlayerStateBuilder(BasePlayerTurn playerTurn, PlayerSnapshot playerSnapshot)
        {
            this.playerTurn = playerTurn;
            this.playerSnapshot = playerSnapshot;
        }
        #endregion

        public PlayerStateBuilder HeldCards(params DevelopmentCardTypes[] cards)
        {
            this.playerSnapshot.HeldCards = new List<DevelopmentCardTypes>(cards);
            return this;
        }

        public BasePlayerTurn End()
        {
            return this.playerTurn;
        }

        public PlayerStateBuilder VictoryPoints(uint victoryPoints)
        {
            this.playerSnapshot.VictoryPoints = victoryPoints;
            return this;
        }
    }
}
