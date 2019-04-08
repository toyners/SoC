
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
                .WhenPlayerOrderEvent(Adam, playerOrder)
                .WhenPlayerOrderEvent(Babara, playerOrder)
                .WhenPlayerOrderEvent(Charlie, playerOrder)
                .WhenPlayerOrderEvent(Dana, playerOrder)
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
        public void Scenario_AllPlayersCollectResourcesAsPartOfTurnStart(string[] args)
        {
            var firstTurnCollectedResources = new Dictionary<string, ResourceCollection[]>()
            {
                { Adam, new ResourceCollection[] { new ResourceCollection(Adam_FirstSettlementLocation, ResourceClutch.OneBrick) } },
                { Babara, new ResourceCollection[] { new ResourceCollection(Babara_SecondSettlementLocation, ResourceClutch.OneGrain) } }
            };

            var secondTurnCollectedResources = new Dictionary<string, ResourceCollection[]>()
            {
                { Babara, new ResourceCollection[] {
                    new ResourceCollection(Babara_FirstSettlementLocation, ResourceClutch.OneOre)
                } },
                { Charlie, new ResourceCollection[] {
                    new ResourceCollection(Charlie_FirstSettlementLocation, ResourceClutch.OneLumber),
                    new ResourceCollection(Charlie_SecondSettlementLocation, ResourceClutch.OneLumber)
                } },
                { Dana, new ResourceCollection[] {
                    new ResourceCollection(Dana_FirstSettlementLocation, ResourceClutch.OneOre)
                } }
            };

            var thirdTurnCollectedResources = new Dictionary<string, ResourceCollection[]>()
            {
                { Charlie, new ResourceCollection[] {
                    new ResourceCollection(Charlie_SecondSettlementLocation, ResourceClutch.OneOre)
                } }
            };

            var fourTurnCollectedResources = new Dictionary<string, ResourceCollection[]>()
            {
                { Adam, new ResourceCollection[] {
                    new ResourceCollection(Adam_FirstSettlementLocation, ResourceClutch.OneWool)
                } },
                { Babara, new ResourceCollection[] {
                    new ResourceCollection(Babara_SecondSettlementLocation, ResourceClutch.OneWool)
                } }
            };

            this.CompletePlayerInfrastructureSetup(args)
                .WithNoResourceCollection()
                .WhenDiceRollEvent(Adam, 4, 4)
                .WhenResourceCollectedEvent(Adam, firstTurnCollectedResources)
                    .State(Adam).Resources(ResourceClutch.OneGrain).End()
                .WhenResourceCollectedEvent(Babara, firstTurnCollectedResources)
                    .State(Babara).Resources(ResourceClutch.OneGrain).End()
                .WhenResourceCollectedEvent(Charlie, firstTurnCollectedResources)
                    .State(Charlie).Resources(ResourceClutch.OneGrain).End()
                .WhenResourceCollectedEvent(Dana, firstTurnCollectedResources)
                    .State(Dana).Resources(ResourceClutch.OneGrain).End()
                .EndTurn()
                .WhenDiceRollEvent(Babara, 3, 3)
                .EndTurn()
                .WhenDiceRollEvent(Charlie, 1, 2)
                .EndTurn()
                .WhenDiceRollEvent(Dana, 6, 4)
                .EndTurn()
                .Run();
            /*var localGameController = this.CreateStandardLocalGameControllerScenarioRunner_Old()
                .PlayerTurn(FirstOpponentName, 4, 4)
                    .ResourceCollectedEvent(MainPlayerName,
                        new Tuple<uint, ResourceClutch>(MainPlayerFirstSettlementLocation, ResourceClutch.OneBrick))
                    .ResourceCollectedEvent(FirstOpponentName,
                        new Tuple<uint, ResourceClutch>(FirstOpponentSecondSettlementLocation, ResourceClutch.OneGrain))
                    .EndTurn()
                .PlayerTurn(SecondOpponentName, 3, 3)
                    .ResourceCollectedEvent(FirstOpponentName,
                            new Tuple<uint, ResourceClutch>(FirstOpponentFirstSettlementLocation, ResourceClutch.OneOre))
                    .ResourceCollectedEvent(SecondOpponentName,
                        new Tuple<uint, ResourceClutch>(SecondOpponentFirstSettlementLocation, ResourceClutch.OneLumber),
                        new Tuple<uint, ResourceClutch>(SecondOpponentSecondSettlementLocation, ResourceClutch.OneLumber))
                    .ResourceCollectedEvent(ThirdOpponentName,
                        new Tuple<uint, ResourceClutch>(ThirdOpponentFirstSettlementLocation, ResourceClutch.OneOre))
                    .EndTurn()
                .PlayerTurn(ThirdOpponentName, 1, 2)
                    .ResourceCollectedEvent(SecondOpponentName,
                        new Tuple<uint, ResourceClutch>(SecondOpponentSecondSettlementLocation, ResourceClutch.OneOre))
                    .EndTurn()
                .PlayerTurn(MainPlayerName, 6, 4)
                    .ResourceCollectedEvent(MainPlayerName,
                            new Tuple<uint, ResourceClutch>(MainPlayerFirstSettlementLocation, ResourceClutch.OneWool))
                    .ResourceCollectedEvent(FirstOpponentName,
                            new Tuple<uint, ResourceClutch>(FirstOpponentSecondSettlementLocation, ResourceClutch.OneWool))
                    .EndTurn()
                .Build()
                .Run_Old();*/
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
