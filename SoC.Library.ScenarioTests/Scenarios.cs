

namespace SoC.Library.ScenarioTests
{
    using Jabberwocky.SoC.Library;

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

        //[Scenario]
        public void Scenario_AllPlayersCompleteSetup(string[] args)
        {
            this.CompletePlayerSetup_Old(args)
                .WithNoResourceCollection()
                .PlayerSetupEvent()
                .InitialBoardSetupEvent()
                .PlayerInfrastructureSetup(Adam, Adam_FirstSettlementLocation, Adam_FirstRoadEndLocation)
                .PlayerInfrastructureSetup(Babara, Babara_FirstSettlementLocation, Babara_FirstRoadEndLocation)
                .PlayerInfrastructureSetup(Charlie, Charlie_FirstSettlementLocation, Charlie_FirstRoadEndLocation)
                .PlayerInfrastructureSetup(Dana, Dana_FirstSettlementLocation, Dana_FirstRoadEndLocation)
                .PlayerInfrastructureSetup(Dana, Dana_SecondSettlementLocation, Dana_SecondRoadEndLocation)
                .PlayerInfrastructureSetup(Charlie, Charlie_SecondSettlementLocation, Charlie_SecondRoadEndLocation)
                .PlayerInfrastructureSetup(Babara, Babara_SecondSettlementLocation, Babara_SecondRoadEndLocation)
                .PlayerInfrastructureSetup(Adam, Adam_SecondSettlementLocation, Adam_SecondRoadEndLocation)
                .Run();
        }

        //[Scenario]
        public void Scenario_PlayerTradesOneResourceWithPlayer(string[] args)
        {
            var adamResources = ResourceClutch.OneWool;
            var babaraResources = ResourceClutch.OneGrain;
            this.CompletePlayerInfrastructureSetup_Old(args)
                .WithNoResourceCollection()
                .WithStartingResourcesForPlayer(Adam, adamResources)
                .WithStartingResourcesForPlayer(Babara, babaraResources)
                .PlayerTurn(Adam, 3, 3).EndTurn()
                .PlayerTurn(Babara, 3, 3)
                    .MakeDirectTradeOffer(ResourceClutch.OneWool)
                    .MakeDirectTradeOfferEvent(Adam, Babara, ResourceClutch.OneWool)
                    .MakeDirectTradeOfferEvent(Charlie, Babara, ResourceClutch.OneWool)
                    .MakeDirectTradeOfferEvent(Dana, Babara, ResourceClutch.OneWool)
                    .AnswerDirectTradeOffer(Adam, ResourceClutch.OneGrain)
                    .AnswerDirectTradeOfferEvent(Babara, Adam, ResourceClutch.OneGrain)
                    .AnswerDirectTradeOfferEvent(Charlie, Adam, ResourceClutch.OneGrain)
                    .AnswerDirectTradeOfferEvent(Dana, Adam, ResourceClutch.OneGrain)
                    .TradeWithPlayerCompletedEvent(Adam, Babara, ResourceClutch.OneWool, Adam, ResourceClutch.OneGrain)
                    .TradeWithPlayerCompletedEvent(Babara, Babara, ResourceClutch.OneWool, Adam, ResourceClutch.OneGrain)
                    .TradeWithPlayerCompletedEvent(Charlie, Babara, ResourceClutch.OneWool, Adam, ResourceClutch.OneGrain)
                    .TradeWithPlayerCompletedEvent(Dana, Babara, ResourceClutch.OneWool, Adam, ResourceClutch.OneGrain)
                    .State(Adam)
                        .Resources(ResourceClutch.OneGrain)
                        .End()
                    .State(Babara)
                        .Resources(ResourceClutch.OneWool)
                        .End()
                    .EndTurn()
                .Run();
        }

        [Scenario]
        public void Scenario_PlayerTradesOneResourceWithPlayer2(string[] args)
        {
            var adamResources = ResourceClutch.OneWool;
            var babaraResources = ResourceClutch.OneGrain;

            this.CompletePlayerInfrastructureSetup(args)
                .WithPlayer(Adam)
                .WithPlayer(Babara)
                .WithPlayer(Charlie)
                .WithPlayer(Dana)
                .WithTurnOrder(Adam, Babara, Charlie, Dana)
                .WithNoResourceCollection()
                .WithStartingResourcesForPlayer(Adam, adamResources)
                .WithStartingResourcesForPlayer(Babara, babaraResources)
                .Label(Adam, "Round 1 - Adam")
                .WhenDiceRollEvent(Adam, 3, 3)
                .Label(Babara, "Round 1 - Babara")
                .WhenDiceRollEvent(Babara, 3, 3)
                    .MakeDirectTradeOffer(ResourceClutch.OneWool)
                .WhenMakeDirectTradeOfferEvent(Adam, Babara, ResourceClutch.OneWool)
                    .MakeDirectTradeOffer(ResourceClutch.OneGrain)
                
                //.MakeDirectTradeOfferEvent(Charlie, Babara, ResourceClutch.OneWool)
                //.MakeDirectTradeOfferEvent(Dana, Babara, ResourceClutch.OneWool)
                //    .AnswerDirectTradeOffer(Adam, ResourceClutch.OneGrain)
                .WhenAnswerDirectTradeOfferEvent(Babara, Adam, ResourceClutch.OneGrain)
                    .ConfirmDirectTrade()
                //.AnswerDirectTradeOfferEvent(Dana, Adam, ResourceClutch.OneGrain)
                //.TradeWithPlayerCompletedEvent(Adam, Babara, ResourceClutch.OneWool, Adam, ResourceClutch.OneGrain)
                //.TradeWithPlayerCompletedEvent(Babara, Babara, ResourceClutch.OneWool, Adam, ResourceClutch.OneGrain)
                //.TradeWithPlayerCompletedEvent(Charlie, Babara, ResourceClutch.OneWool, Adam, ResourceClutch.OneGrain)
                //.TradeWithPlayerCompletedEvent(Dana, Babara, ResourceClutch.OneWool, Adam, ResourceClutch.OneGrain)
                //.State(Adam)
                //    .Resources(ResourceClutch.OneGrain)
                //    .End()
                //.State(Babara)
                //    .Resources(ResourceClutch.OneWool)
                //    .End()
                .Run();
        }

        private LocalGameControllerScenarioRunner CompletePlayerSetup_Old(string[] args)
        {
            return LocalGameControllerScenarioRunner.LocalGameController(args)
                .WithPlayer(Adam)
                .WithPlayer(Babara)
                .WithPlayer(Charlie)
                .WithPlayer(Dana)
                .WithTurnOrder(Adam, Babara, Charlie, Dana);
        }

        private ScenarioRunner CompletePlayerSetup(string[] args)
        {
            return ScenarioRunner.CreateScenarioRunner(args)
                .WithPlayer(Adam)
                .WithPlayer(Babara)
                .WithPlayer(Charlie)
                .WithPlayer(Dana)
                .WithTurnOrder(Adam, Babara, Charlie, Dana);
        }

        private LocalGameControllerScenarioRunner CompletePlayerInfrastructureSetup_Old(string[] args)
        {
            return LocalGameControllerScenarioRunner.LocalGameController(args)
                .WithPlayer(Adam)
                .WithPlayer(Babara)
                .WithPlayer(Charlie)
                .WithPlayer(Dana)
                .WithTurnOrder(Adam, Babara, Charlie, Dana)
                .PlayerInfrastructureSetup(Adam, Adam_FirstSettlementLocation, Adam_FirstRoadEndLocation)
                .PlayerInfrastructureSetup(Babara, Babara_FirstSettlementLocation, Babara_FirstRoadEndLocation)
                .PlayerInfrastructureSetup(Charlie, Charlie_FirstSettlementLocation, Charlie_FirstRoadEndLocation)
                .PlayerInfrastructureSetup(Dana, Dana_FirstSettlementLocation, Dana_FirstRoadEndLocation)
                .PlayerInfrastructureSetup(Dana, Dana_SecondSettlementLocation, Dana_SecondRoadEndLocation)
                .PlayerInfrastructureSetup(Charlie, Charlie_SecondSettlementLocation, Charlie_SecondRoadEndLocation)
                .PlayerInfrastructureSetup(Babara, Babara_SecondSettlementLocation, Babara_SecondRoadEndLocation)
                .PlayerInfrastructureSetup(Adam, Adam_SecondSettlementLocation, Adam_SecondRoadEndLocation);
        }

        private ScenarioRunner CompletePlayerInfrastructureSetup(string[] args)
        {
            return ScenarioRunner.CreateScenarioRunner(args)
                .WithPlayer(Adam)
                .WithPlayer(Babara)
                .WithPlayer(Charlie)
                .WithPlayer(Dana)
                .WithTurnOrder(Adam, Babara, Charlie, Dana)
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
