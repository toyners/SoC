using System.Collections.Generic;
using Jabberwocky.SoC.Library;
using SoC.Library.ScenarioTests.PlayerTurn;

namespace SoC.Library.ScenarioTests.Builders
{
    internal class PlayerStateBuilder
    {
        private BasePlayerTurn playerTurn;
        public PlayerSnapshot playerSnapshot;
        public PlayerStateBuilder(BasePlayerTurn playerTurn)
        {
            this.playerTurn = playerTurn;
        }

        public PlayerStateBuilder HeldCards(params DevelopmentCardTypes[] cards)
        {
            playerSnapshot = new PlayerSnapshot();
            playerSnapshot.heldCards = new List<DevelopmentCardTypes>(cards);
            return this;
        }

        public BasePlayerTurn End()
        {
            return this.playerTurn;
        }
    }
}
