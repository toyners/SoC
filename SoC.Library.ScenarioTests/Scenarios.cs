
namespace SoC.Library.ScenarioTests
{
    using System;
    using System.Collections.Generic;
    using Jabberwocky.SoC.Library;
    using Jabberwocky.SoC.Library.GameBoards;

    public class Scenarios
    {
        const string Adam = "Adam";
        const string Babara = "Barbara";
        const string Charlie = "Charlie";
        const string Dana = "Dana";

        const uint Adam_FirstSettlementLocation = 12u;
        const uint Babara_FirstSettlementLocation = 18u;
        const uint Charlie_FirstSettlementLocation = 25u;
        const uint Dana_FirstSettlementLocation = 31u;

        const uint Dana_SecondSettlementLocation = 33u;
        const uint Charlie_SecondSettlementLocation = 35u;
        const uint Babara_SecondSettlementLocation = 43u;
        const uint Adam_SecondSettlementLocation = 40u;

        const uint Adam_FirstRoadEndLocation = 4;
        const uint Babara_FirstRoadEndLocation = 17;
        const uint Charlie_FirstRoadEndLocation = 15;
        const uint Dana_FirstRoadEndLocation = 30;

        const uint Dana_SecondRoadEndLocation = 32;
        const uint Charlie_SecondRoadEndLocation = 24;
        const uint Babara_SecondRoadEndLocation = 44;
        const uint Adam_SecondRoadEndLocation = 39;

        [Scenario]
        public void Scenario_AllPlayersCompleteSetup(string[] args)
        {
            var expectedGameBoardSetup = new GameBoardSetup(new GameBoard(BoardSizes.Standard));
            var playerOrder = new[] { Adam, Babara, Charlie, Dana };
            ScenarioRunner.CreateScenarioRunner(args)
                .WithPlayer(Adam)
                .WithPlayer(Babara)
                .WithPlayer(Charlie)
                .WithPlayer(Dana)
                .WithTurnOrder(playerOrder)
                .WhenPlayerOrderEvent(Adam, playerOrder)
                .WhenGameJoinedEvent(Adam)
                .WhenGameJoinedEvent(Babara)
                .WhenGameJoinedEvent(Charlie)
                .WhenGameJoinedEvent(Dana)
                .WhenPlayerSetupEvent(Adam)
                .WhenPlayerSetupEvent(Babara)
                .WhenPlayerSetupEvent(Charlie)
                .WhenPlayerSetupEvent(Dana)
                .WhenInitialBoardSetupEvent(Adam, expectedGameBoardSetup)
                .WhenInitialBoardSetupEvent(Babara, expectedGameBoardSetup)
                .WhenInitialBoardSetupEvent(Charlie, expectedGameBoardSetup)
                .WhenInitialBoardSetupEvent(Dana, expectedGameBoardSetup)
                .WhenPlaceInfrastructureSetupEvent(Adam)
                    .PlaceStartingInfrastructure(Adam_FirstSettlementLocation, Adam_FirstRoadEndLocation)
                .WhenPlaceInfrastructureSetupEvent(Babara)
                    .PlaceStartingInfrastructure(Babara_FirstSettlementLocation, Babara_FirstRoadEndLocation)
                .WhenPlaceInfrastructureSetupEvent(Charlie)
                    .PlaceStartingInfrastructure(Charlie_FirstSettlementLocation, Charlie_FirstRoadEndLocation)
                .WhenPlaceInfrastructureSetupEvent(Dana)
                    .PlaceStartingInfrastructure(Dana_FirstSettlementLocation, Dana_FirstRoadEndLocation)
                .WhenPlaceInfrastructureSetupEvent(Dana)
                    .PlaceStartingInfrastructure(Dana_SecondSettlementLocation, Dana_SecondRoadEndLocation)
                .WhenPlaceInfrastructureSetupEvent(Charlie)
                    .PlaceStartingInfrastructure(Charlie_SecondSettlementLocation, Charlie_SecondRoadEndLocation)
                .WhenPlaceInfrastructureSetupEvent(Babara)
                    .PlaceStartingInfrastructure(Babara_SecondSettlementLocation, Babara_SecondRoadEndLocation)
                .WhenPlaceInfrastructureSetupEvent(Adam)
                    .PlaceStartingInfrastructure(Adam_SecondSettlementLocation, Adam_SecondRoadEndLocation)
                .Run();
        }

        [Scenario]
        public void Scenario_PlayerTradesOneResourceWithPlayer(string[] args)
        {
            var adamResources = ResourceClutch.OneWool;
            var babaraResources = ResourceClutch.OneGrain;

            this.CompletePlayerInfrastructureSetup(args)
                .WithNoResourceCollection()
                .WithStartingResourcesForPlayer(Adam, adamResources)
                .WithStartingResourcesForPlayer(Babara, babaraResources)
                .Label(Adam, "Round 1 - Adam")
                .WhenDiceRollEvent(Adam, 3, 3)
                    .EndTurn()
                .Label(Babara, "Round 1 - Babara")
                .WhenDiceRollEvent(Babara, 3, 3)
                    .MakeDirectTradeOffer(ResourceClutch.OneWool)
                .WhenMakeDirectTradeOfferEvent(Adam, Babara, ResourceClutch.OneWool)
                    .AnswerDirectTradeOffer(ResourceClutch.OneGrain)
                .WhenMakeDirectTradeOfferEvent(Charlie, Babara, ResourceClutch.OneWool)
                .WhenMakeDirectTradeOfferEvent(Dana, Babara, ResourceClutch.OneWool)
                .WhenAnswerDirectTradeOfferEvent(Babara, Adam, ResourceClutch.OneGrain)
                    .AcceptTrade(Adam)
                .WhenAnswerDirectTradeOfferEvent(Charlie, Adam, ResourceClutch.OneGrain)
                .WhenAnswerDirectTradeOfferEvent(Dana, Adam, ResourceClutch.OneGrain)
                .WhenAcceptDirectTradeEvent(Adam, Babara, ResourceClutch.OneWool, Adam, ResourceClutch.OneGrain)
                    .State(Adam)
                        .Resources(ResourceClutch.OneGrain)
                        .End()
                .WhenAcceptDirectTradeEvent(Babara, Babara, ResourceClutch.OneWool, Adam, ResourceClutch.OneGrain)
                    .State(Babara)
                        .Resources(ResourceClutch.OneWool)
                        .End()
                .WhenAcceptDirectTradeEvent(Charlie, Babara, ResourceClutch.OneWool, Adam, ResourceClutch.OneGrain)
                .WhenAcceptDirectTradeEvent(Dana, Babara, ResourceClutch.OneWool, Adam, ResourceClutch.OneGrain)
                .Run();
        }

        private ScenarioRunner CompletePlayerInfrastructureSetup(string[] args)
        {
            return ScenarioRunner.CreateScenarioRunner(args)
                .WithPlayer(Adam)
                .WithPlayer(Babara)
                .WithPlayer(Charlie)
                .WithPlayer(Dana)
                .WithTurnOrder(new[] { Adam, Babara, Charlie, Dana })
                .WhenPlaceInfrastructureSetupEvent(Adam)
                    .PlaceStartingInfrastructure(Adam_FirstSettlementLocation, Adam_FirstRoadEndLocation)
                .WhenPlaceInfrastructureSetupEvent(Babara)
                    .PlaceStartingInfrastructure(Babara_FirstSettlementLocation, Babara_FirstRoadEndLocation)
                .WhenPlaceInfrastructureSetupEvent(Charlie)
                    .PlaceStartingInfrastructure(Charlie_FirstSettlementLocation, Charlie_FirstRoadEndLocation)
                .WhenPlaceInfrastructureSetupEvent(Dana)
                    .PlaceStartingInfrastructure(Dana_FirstSettlementLocation, Dana_FirstRoadEndLocation)
                .WhenPlaceInfrastructureSetupEvent(Dana)
                    .PlaceStartingInfrastructure(Dana_SecondSettlementLocation, Dana_SecondRoadEndLocation)
                .WhenPlaceInfrastructureSetupEvent(Charlie)
                    .PlaceStartingInfrastructure(Charlie_SecondSettlementLocation, Charlie_SecondRoadEndLocation)
                .WhenPlaceInfrastructureSetupEvent(Babara)
                    .PlaceStartingInfrastructure(Babara_SecondSettlementLocation, Babara_SecondRoadEndLocation)
                .WhenPlaceInfrastructureSetupEvent(Adam)
                    .PlaceStartingInfrastructure(Adam_SecondSettlementLocation, Adam_SecondRoadEndLocation);
        }
    }
}
