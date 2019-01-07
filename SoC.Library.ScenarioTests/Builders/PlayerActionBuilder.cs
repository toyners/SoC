using System.Collections.Generic;
using Jabberwocky.SoC.Library.Enums;
using Jabberwocky.SoC.Library.GameActions;
using SoC.Library.ScenarioTests;
using SoC.Library.ScenarioTests.PlayerTurn;

namespace Jabberwocky.SoC.Library.ScenarioTests.Builders
{
    internal class PlayerActionBuilder
    {
        private BasePlayerTurn playerTurn;
        public List<RunnerAction> runnerActions = new List<RunnerAction>();
        public List<ComputerPlayerAction> playerActions = new List<ComputerPlayerAction>();
        public PlayerActionBuilder(BasePlayerTurn playerTurn)
        {
            this.playerTurn = playerTurn;
        }

        public BasePlayerTurn End()
        {
            return this.playerTurn;
        }

        public PlayerActionBuilder BuildCity(uint cityLocation)
        {
            this.playerActions.Add(new BuildCityAction(cityLocation));
            return this;
        }

        public PlayerActionBuilder BuildRoad(uint roadSegmentStart, uint roadSegmentEnd)
        {
            this.playerActions.Add(new BuildRoadSegmentAction(roadSegmentStart, roadSegmentEnd));
            return this;
        }

        public PlayerActionBuilder BuildSettlement(uint settlementLocation)
        {
            this.playerActions.Add(new BuildSettlementAction(settlementLocation));
            return this;
        }

        public PlayerActionBuilder BuyDevelopmentCard(DevelopmentCardTypes developmentCardType)
        {
            this.runnerActions.Add(new InsertDevelopmentCardAction { DevelopmentCardType = developmentCardType });
            this.playerActions.Add(new ComputerPlayerAction(ComputerPlayerActionTypes.BuyDevelopmentCard));
            return this;
        }
    }
}
