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
        private readonly BasePlayerTurn playerTurn;
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

        public PlayerActionBuilder PlaceRobber(uint hexLocation)
        {
            this.playerActions.Add(new PlaceRobberAction(hexLocation, null, null, ResourceClutch.Zero));
            return this;
        }

        public PlayerActionBuilder PlaceRobber(uint hexLocation, string selectedPlayerName, ResourceTypes expectedSingleResource)
        {
            this.playerActions.Add(new PlaceRobberAction(hexLocation, selectedPlayerName, expectedSingleResource, ResourceClutch.Zero));
            return this;
        }

        public PlayerActionBuilder PlaceRobber(uint hexLocation, string selectedPlayerName, ResourceTypes expectedSingleResource, ResourceClutch resourcesToDrop)
        {
            this.playerActions.Add(new PlaceRobberAction(hexLocation, selectedPlayerName, expectedSingleResource, resourcesToDrop));
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
