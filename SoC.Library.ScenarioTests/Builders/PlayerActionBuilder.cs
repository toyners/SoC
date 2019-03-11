using System;
using System.Collections.Generic;
using Jabberwocky.SoC.Library.Enums;
using Jabberwocky.SoC.Library.GameActions;
using SoC.Library.ScenarioTests;
using SoC.Library.ScenarioTests.PlayerTurn;
using SoC.Library.ScenarioTests.ScenarioActions;

namespace Jabberwocky.SoC.Library.ScenarioTests.Builders
{
    internal class PlayerActionBuilder
    {
        private readonly GameTurn playerTurn;
        private readonly IList<RunnerAction> runnerActions = new List<RunnerAction>();
        private readonly List<PlayerAction> playerActions = new List<PlayerAction>();

        public PlayerActionBuilder(GameTurn playerTurn)
        {
            this.playerTurn = playerTurn;
        }

        public GameTurn End()
        {
            //this.playerTurn.PlayerActions = this.playerActions;
            this.playerTurn.RunnerActions = this.runnerActions;
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
            this.playerActions.Add(new PlayerAction(ComputerPlayerActionTypes.BuyDevelopmentCard));
            return this;
        }

        public PlayerActionBuilder ChooseResourceFromOpponent(string opponentName, ResourceTypes resourceType)
        {
            var scenarioSelectResourceFromPlayerAction = new ScenarioSelectResourceFromPlayerAction(opponentName, resourceType);
            this.playerActions.Add(scenarioSelectResourceFromPlayerAction);
            return this;
        }

        public PlayerActionBuilder PlaceRobber(uint hexLocation)
        {
            this.playerActions.Add(new ScenarioPlaceRobberAction(hexLocation));
            return this;
        }

        public PlayerActionBuilder PlaceRobber(uint hexLocation, string selectedPlayerName, ResourceTypes expectedSingleResource)
        {
            this.playerActions.Add(new ScenarioPlaceRobberAction(hexLocation));
            this.playerActions.Add(new ScenarioSelectResourceFromPlayerAction(selectedPlayerName, expectedSingleResource));
            return this;
        }

        public PlayerActionBuilder PlaceRobber(uint hexLocation, string selectedPlayerName, ResourceTypes expectedSingleResource, ResourceClutch resourcesToDrop)
        {
            this.playerActions.Add(new ScenarioPlaceRobberAction(hexLocation, resourcesToDrop));
            this.playerActions.Add(new ScenarioSelectResourceFromPlayerAction(selectedPlayerName, expectedSingleResource));
            return this;
        }

        public PlayerActionBuilder PlayKnightCard(uint hexLocation)
        {
            this.playerActions.Add(new PlayKnightCardAction(hexLocation));
            return this;
        }

        public PlayerActionBuilder PlayKnightCardAndCollectFrom(uint hexLocation, string selectedPlayerName, ResourceTypes expectedSingleResource)
        {
            this.playerActions.Add(new ScenarioPlayKnightCardAction(hexLocation, selectedPlayerName, expectedSingleResource));
            return this;
        }
    }
}
