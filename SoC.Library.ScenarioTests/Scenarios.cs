
namespace SoC.Library.ScenarioTests
{
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
                .WhenPlayer(Adam)
                    .ReceivesGameJoinedEvent().ThenDoNothing()
                    .ReceivesPlayerSetupEvent().ThenDoNothing()
                    .ReceivesInitialBoardSetupEvent(expectedGameBoardSetup).ThenDoNothing()
                    .ReceivesPlayerOrderEvent(playerOrder)
                .WhenPlayer(Babara)
                    .ReceivesGameJoinedEvent()
                    .ReceivesPlayerSetupEvent()
                    .ReceivesInitialBoardSetupEvent(expectedGameBoardSetup)
                    .ReceivesPlayerOrderEvent(playerOrder)
                .WhenPlayer(Charlie)
                    .ReceivesGameJoinedEvent()
                    .ReceivesPlayerSetupEvent()
                    .ReceivesInitialBoardSetupEvent(expectedGameBoardSetup)
                    .ReceivesPlayerOrderEvent(playerOrder)
                .WhenPlayer(Dana)
                    .ReceivesGameJoinedEvent()
                    .ReceivesPlayerSetupEvent()
                    .ReceivesInitialBoardSetupEvent(expectedGameBoardSetup)
                    .ReceivesPlayerOrderEvent(playerOrder)
                .WhenPlayer(Adam)
                    .ReceivesPlaceInfrastructureSetupEvent()
                    .ThenPlaceStartingInfrastructure(Adam_FirstSettlementLocation, Adam_FirstRoadEndLocation)
                    .VerifyAllPlayersReceivedInfrastructurePlacedEvent(Adam, Adam_FirstSettlementLocation, Adam_FirstRoadEndLocation)
                .WhenPlayer(Babara)
                    .ReceivesPlaceInfrastructureSetupEvent()
                    .ThenPlaceStartingInfrastructure(Babara_FirstSettlementLocation, Babara_FirstRoadEndLocation)
                    .VerifyAllPlayersReceivedInfrastructurePlacedEvent(Babara, Babara_FirstSettlementLocation, Babara_FirstRoadEndLocation)
                .WhenPlayer(Charlie)
                    .ReceivesPlaceInfrastructureSetupEvent()
                    .ThenPlaceStartingInfrastructure(Charlie_FirstSettlementLocation, Charlie_FirstRoadEndLocation)
                    .VerifyAllPlayersReceivedInfrastructurePlacedEvent(Charlie, Charlie_FirstSettlementLocation, Charlie_FirstRoadEndLocation)
                .WhenPlayer(Dana)
                    .ReceivesPlaceInfrastructureSetupEvent()
                    .ThenPlaceStartingInfrastructure(Dana_FirstSettlementLocation, Dana_FirstRoadEndLocation)
                    .VerifyAllPlayersReceivedInfrastructurePlacedEvent(Dana, Dana_FirstSettlementLocation, Dana_FirstRoadEndLocation)
                    .ReceivesPlaceInfrastructureSetupEvent()
                    .ThenPlaceStartingInfrastructure(Dana_SecondSettlementLocation, Dana_SecondRoadEndLocation)
                    .VerifyAllPlayersReceivedInfrastructurePlacedEvent(Dana, Dana_SecondSettlementLocation, Dana_SecondRoadEndLocation)
                .WhenPlayer(Charlie)
                    .ReceivesPlaceInfrastructureSetupEvent()
                    .ThenPlaceStartingInfrastructure(Charlie_SecondSettlementLocation, Charlie_SecondRoadEndLocation)
                    .VerifyAllPlayersReceivedInfrastructurePlacedEvent(Charlie, Charlie_SecondSettlementLocation, Charlie_SecondRoadEndLocation)
                .WhenPlayer(Babara)
                    .ReceivesPlaceInfrastructureSetupEvent()
                    .ThenPlaceStartingInfrastructure(Babara_SecondSettlementLocation, Babara_SecondRoadEndLocation)
                    .VerifyAllPlayersReceivedInfrastructurePlacedEvent(Babara, Babara_SecondSettlementLocation, Babara_SecondRoadEndLocation)
                .WhenPlayer(Adam)
                    .ReceivesPlaceInfrastructureSetupEvent()
                    .ThenPlaceStartingInfrastructure(Adam_SecondSettlementLocation, Adam_SecondRoadEndLocation)
                    .VerifyAllPlayersReceivedInfrastructurePlacedEvent(Adam, Adam_SecondSettlementLocation, Adam_SecondRoadEndLocation)
                    .ReceivesConfirmGameStartEvent()
                    .ThenQuitGame()
                .WhenPlayer(Babara)
                    .ReceivesConfirmGameStartEvent()
                    .ThenQuitGame()
                .WhenPlayer(Charlie)
                    .ReceivesConfirmGameStartEvent()
                    .ThenQuitGame()
                .WhenPlayer(Dana)
                    .ReceivesConfirmGameStartEvent()
                    .ThenQuitGame()
                .Run();
        }

        [Scenario]
        public void Scenario_AllPlayersCollectResourcesAsPartOfTurnStart(string[] args)
        {
            var firstTurnCollectedResources = CreateExpectedCollectedResources()
                .Add(Adam, Adam_FirstSettlementLocation, ResourceClutch.OneBrick)
                .Add(Babara, Babara_SecondSettlementLocation, ResourceClutch.OneGrain)
                .Build();

            var secondTurnCollectedResources = CreateExpectedCollectedResources()
                .Add(Babara, Babara_FirstSettlementLocation, ResourceClutch.OneOre)
                .Add(Charlie, Charlie_FirstSettlementLocation, ResourceClutch.OneLumber)
                .Add(Charlie, Charlie_SecondSettlementLocation, ResourceClutch.OneLumber)
                .Add(Dana, Dana_FirstSettlementLocation, ResourceClutch.OneOre)
                .Build();

            var thirdTurnCollectedResources = CreateExpectedCollectedResources()
                .Add(Charlie, Charlie_SecondSettlementLocation, ResourceClutch.OneOre)
                .Build();

            var fourTurnCollectedResources = CreateExpectedCollectedResources()
                .Add(Adam, Adam_FirstSettlementLocation, ResourceClutch.OneWool)
                .Add(Babara, Babara_SecondSettlementLocation, ResourceClutch.OneWool)
                .Build();

            this.CompletePlayerInfrastructureSetup(args)
                .WhenDiceRollEvent(Adam, 4, 4)
                .WhenResourceCollectedEvent(Adam, firstTurnCollectedResources)
                    .State(Adam).Resources(ResourceClutch.OneBrick).End()
                .WhenResourceCollectedEvent(Babara, firstTurnCollectedResources)
                    .State(Babara).Resources(ResourceClutch.OneGrain).End()
                .WhenResourceCollectedEvent(Charlie, firstTurnCollectedResources)
                    .State(Charlie).Resources(ResourceClutch.Zero).End()
                .WhenResourceCollectedEvent(Dana, firstTurnCollectedResources)
                    .State(Dana).Resources(ResourceClutch.Zero).End()
                .EndTurn(Adam)
                .WhenDiceRollEvent(Babara, 3, 3)
                .WhenResourceCollectedEvent(Adam, secondTurnCollectedResources)
                    .State(Adam).Resources(ResourceClutch.OneBrick).End()
                .WhenResourceCollectedEvent(Babara, secondTurnCollectedResources)
                    .State(Babara).Resources(ResourceClutch.OneGrain + ResourceClutch.OneOre).End()
                .WhenResourceCollectedEvent(Charlie, secondTurnCollectedResources)
                    .State(Charlie).Resources(ResourceClutch.OneLumber * 2).End()
                .WhenResourceCollectedEvent(Dana, secondTurnCollectedResources)
                    .State(Dana).Resources(ResourceClutch.OneOre).End()
                .EndTurn(Babara)
                .WhenDiceRollEvent(Charlie, 1, 2)
                .WhenResourceCollectedEvent(Adam, thirdTurnCollectedResources)
                    .State(Adam).Resources(ResourceClutch.OneBrick).End()
                .WhenResourceCollectedEvent(Babara, thirdTurnCollectedResources)
                    .State(Babara).Resources(ResourceClutch.OneGrain + ResourceClutch.OneOre).End()
                .WhenResourceCollectedEvent(Charlie, thirdTurnCollectedResources)
                    .State(Charlie).Resources((ResourceClutch.OneLumber * 2) + ResourceClutch.OneOre).End()
                .WhenResourceCollectedEvent(Dana, thirdTurnCollectedResources)
                    .State(Dana).Resources(ResourceClutch.OneOre).End()
                .EndTurn(Charlie)
                .WhenDiceRollEvent(Dana, 6, 4)
                .WhenResourceCollectedEvent(Adam, fourTurnCollectedResources)
                    .State(Adam).Resources(ResourceClutch.OneBrick + ResourceClutch.OneWool).End()
                .WhenResourceCollectedEvent(Babara, fourTurnCollectedResources)
                    .State(Babara).Resources(ResourceClutch.OneGrain + ResourceClutch.OneOre + ResourceClutch.OneWool).End()
                .WhenResourceCollectedEvent(Charlie, fourTurnCollectedResources)
                    .State(Charlie).Resources((ResourceClutch.OneLumber * 2) + ResourceClutch.OneOre).End()
                .WhenResourceCollectedEvent(Dana, fourTurnCollectedResources)
                    .State(Dana).Resources(ResourceClutch.OneOre).End()
                .EndTurn(Dana)
                .WhenPlayer(Adam)
                .ReceivesDiceRollEvent(1, 1)
                    .ThenQuitGame()
                .ReceivesDiceRollEvent(1, 1)
                    .ThenQuitGame()
                .ReceivesDiceRollEvent(1, 1)
                    .ThenQuitGame()
                .ReceivesDiceRollEvent(1, 1)
                    .ThenQuitGame()
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
                .WhenDiceRollEvent(Adam, 3, 3)
                    .EndTurn(Adam)
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

        [Scenario]
        public void Scenario_AllOtherPlayersQuit(string[] args)
        {
            this.CompletePlayerInfrastructureSetup(args)
                .WithNoResourceCollection()
                .WhenPlayer(Adam)
                    .ReceivesDiceRollEvent(3, 3)
                    .ThenEndTurn()
                .WhenPlayer(Babara)
                    .ReceivesDiceRollEvent(3, 3)
                    .ThenQuitGame()
                .WhenPlayer(Charlie)
                    .ReceivesDiceRollEvent(3, 3)
                    .ThenQuitGame()
                .WhenPlayer(Dana)
                    .ReceivesDiceRollEvent(3, 3)
                    .ThenQuitGame()
                .WhenPlayer(Adam)
                    .ReceivesPlayerWonEvent(Adam)
                    .ThenDoNothing()
                .Run();
        }

        private static CollectedResourcesBuilder CreateExpectedCollectedResources()
        {
            return new CollectedResourcesBuilder();
        }

        private ScenarioRunner CompletePlayerInfrastructureSetup(string[] args)
        {
            return ScenarioRunner.CreateScenarioRunner(args)
                .WithPlayer(Adam)
                .WithPlayer(Babara)
                .WithPlayer(Charlie)
                .WithPlayer(Dana)
                .WithTurnOrder(new[] { Adam, Babara, Charlie, Dana })
                .WhenPlayer(Adam)
                    .ReceivesPlaceInfrastructureSetupEvent()
                    .ThenPlaceStartingInfrastructure(Adam_FirstSettlementLocation, Adam_FirstRoadEndLocation)
                .WhenPlayer(Babara)
                    .ReceivesPlaceInfrastructureSetupEvent()
                    .ThenPlaceStartingInfrastructure(Babara_FirstSettlementLocation, Babara_FirstRoadEndLocation)
                .WhenPlayer(Charlie)
                    .ReceivesPlaceInfrastructureSetupEvent()
                    .ThenPlaceStartingInfrastructure(Charlie_FirstSettlementLocation, Charlie_FirstRoadEndLocation)
                .WhenPlayer(Dana)
                    .ReceivesPlaceInfrastructureSetupEvent()
                    .ThenPlaceStartingInfrastructure(Dana_FirstSettlementLocation, Dana_FirstRoadEndLocation)
                    .ReceivesPlaceInfrastructureSetupEvent()
                    .ThenPlaceStartingInfrastructure(Dana_SecondSettlementLocation, Dana_SecondRoadEndLocation)
                .WhenPlayer(Charlie)
                    .ReceivesPlaceInfrastructureSetupEvent()
                    .ThenPlaceStartingInfrastructure(Charlie_SecondSettlementLocation, Charlie_SecondRoadEndLocation)
                .WhenPlayer(Babara)
                    .ReceivesPlaceInfrastructureSetupEvent()
                    .ThenPlaceStartingInfrastructure(Babara_SecondSettlementLocation, Babara_SecondRoadEndLocation)
                .WhenPlayer(Adam)
                    .ReceivesPlaceInfrastructureSetupEvent()
                    .ThenPlaceStartingInfrastructure(Adam_SecondSettlementLocation, Adam_SecondRoadEndLocation)
                    .ReceivesConfirmGameStartEvent()
                    .ThenConfirmGameStart()
                .WhenPlayer(Babara)
                    .ReceivesConfirmGameStartEvent()
                    .ThenConfirmGameStart()
                .WhenPlayer(Charlie)
                    .ReceivesConfirmGameStartEvent()
                    .ThenConfirmGameStart()
                .WhenPlayer(Dana)
                    .ReceivesConfirmGameStartEvent()
                    .ThenConfirmGameStart();
        }
    }
}
