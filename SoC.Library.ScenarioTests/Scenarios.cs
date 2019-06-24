
namespace SoC.Library.ScenarioTests
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Jabberwocky.SoC.Library;
    using Jabberwocky.SoC.Library.GameBoards;
    using Jabberwocky.SoC.Library.GameEvents;
    using NUnit.Framework;
    using SoC.Library.ScenarioTests.ScenarioEvents;
    using static SoC.Library.ScenarioTests.InfrastructureSetupBuilder;

    [TestFixture]
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

        [Test]
        public void AllPlayersCollectResourcesAsPartOfGameSetup()
        {
            var expectedAdamResources = ResourceClutch.OneBrick + ResourceClutch.OneGrain + ResourceClutch.OneWool;
            var expectedBabaraResources = ResourceClutch.OneGrain + ResourceClutch.OneLumber + ResourceClutch.OneWool;
            var expectedCharlieResources = ResourceClutch.OneLumber;
            var expectedDanaResources = ResourceClutch.OneLumber + ResourceClutch.OneGrain + ResourceClutch.OneWool;
            var gameSetupCollectedResources = CreateExpectedCollectedResources()
                .Add(Adam, Adam_SecondSettlementLocation, expectedAdamResources)
                .Add(Babara, Babara_SecondSettlementLocation, expectedBabaraResources)
                .Add(Charlie, 37, expectedCharlieResources)
                .Add(Dana, Dana_SecondSettlementLocation, expectedDanaResources)
                .Build();

            ScenarioRunner.CreateScenarioRunner(new[] { MethodBase.GetCurrentMethod().Name })
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
                    .ThenPlaceStartingInfrastructure(37, 36)
                .WhenPlayer(Babara)
                    .ReceivesPlaceInfrastructureSetupEvent()
                    .ThenPlaceStartingInfrastructure(Babara_SecondSettlementLocation, Babara_SecondRoadEndLocation)
                .WhenPlayer(Adam)
                    .ReceivesPlaceInfrastructureSetupEvent()
                    .ThenPlaceStartingInfrastructure(Adam_SecondSettlementLocation, Adam_SecondRoadEndLocation)
                    .ReceivesConfirmGameStartEvent().ThenConfirmGameStart()
                    .ReceivesAll()
                        .ReceivesResourceCollectedEvent(gameSetupCollectedResources)
                        .ReceivesStartTurnEvent(1, 2)
                    .ReceivesAllEnd().ThenVerifyPlayerState()
                        .Resources(expectedAdamResources)
                        .End()
                .WhenPlayer(Babara)
                    .ReceivesConfirmGameStartEvent().ThenConfirmGameStart()
                    .ReceivesAll()
                        .ReceivesResourceCollectedEvent(gameSetupCollectedResources)
                        .ReceivesStartTurnEvent(Adam, 1, 2)
                    .ReceivesAllEnd().ThenVerifyPlayerState()
                        .Resources(expectedBabaraResources)
                        .End()
                .WhenPlayer(Charlie)
                    .ReceivesConfirmGameStartEvent().ThenConfirmGameStart()
                    .ReceivesAll()
                        .ReceivesResourceCollectedEvent(gameSetupCollectedResources)
                        .ReceivesStartTurnEvent(Adam, 1, 2)
                    .ReceivesAllEnd().ThenVerifyPlayerState()
                        .Resources(expectedCharlieResources)
                        .End()
                .WhenPlayer(Dana)
                    .ReceivesConfirmGameStartEvent().ThenConfirmGameStart()
                    .ReceivesAll()
                        .ReceivesResourceCollectedEvent(gameSetupCollectedResources)
                        .ReceivesStartTurnEvent(Adam, 1, 2)
                    .ReceivesAllEnd().ThenVerifyPlayerState()
                        .Resources(expectedDanaResources)
                        .End()
                .Run();
        }

        [Test]
        public void AllPlayersCollectResourcesAsPartOfTurnStart()
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

            var fourthTurnCollectedResources = CreateExpectedCollectedResources()
                .Add(Adam, Adam_FirstSettlementLocation, ResourceClutch.OneWool)
                .Add(Babara, Babara_SecondSettlementLocation, ResourceClutch.OneWool)
                .Build();

            this.CompletePlayerInfrastructureSetup(new[] { MethodBase.GetCurrentMethod().Name })
                // Zero away the resources collected at start of the game
                .WithInitialPlayerSetupFor(Adam, Resources(new ResourceClutch(-1, -1, 0, 0, -1)))
                .WithInitialPlayerSetupFor(Babara, Resources(new ResourceClutch(0, -1, -1, 0, -1)))
                .WithInitialPlayerSetupFor(Charlie, Resources(new ResourceClutch(0, 0, -1, -1, -1)))
                .WithInitialPlayerSetupFor(Dana, Resources(new ResourceClutch(0, -1, -1, 0, -1)))
                .WhenPlayer(Adam)
                    .ReceivesStartTurnWithResourcesCollectedEvent(4, 4, firstTurnCollectedResources)
                    .ThenVerifyPlayerState().Resources(ResourceClutch.OneBrick).End()
                    .ThenEndTurn()
                .WhenPlayer(Babara)
                    .ReceivesStartTurnWithResourcesCollectedEvent(Adam, 4, 4, firstTurnCollectedResources)
                    .ThenVerifyPlayerState().Resources(ResourceClutch.OneGrain).End()
                .WhenPlayer(Charlie)
                    .ReceivesStartTurnWithResourcesCollectedEvent(Adam, 4, 4, firstTurnCollectedResources)
                    .ThenVerifyPlayerState().Resources(ResourceClutch.Zero).End()
                .WhenPlayer(Dana)
                    .ReceivesStartTurnWithResourcesCollectedEvent(Adam, 4, 4, firstTurnCollectedResources)
                    .ThenVerifyPlayerState().Resources(ResourceClutch.Zero).End()

                .WhenPlayer(Babara)
                    .ReceivesStartTurnWithResourcesCollectedEvent(3, 3, secondTurnCollectedResources)
                    .ThenVerifyPlayerState().Resources(ResourceClutch.OneGrain + ResourceClutch.OneOre).End()
                    .ThenEndTurn()
                .WhenPlayer(Adam)
                    .ReceivesStartTurnWithResourcesCollectedEvent(Babara, 3, 3, secondTurnCollectedResources)
                    .ThenVerifyPlayerState().Resources(ResourceClutch.OneBrick).End()
                .WhenPlayer(Charlie)
                    .ReceivesStartTurnWithResourcesCollectedEvent(Babara, 3, 3, secondTurnCollectedResources)
                    .ThenVerifyPlayerState().Resources(ResourceClutch.OneLumber * 2).End()
                .WhenPlayer(Dana)
                    .ReceivesStartTurnWithResourcesCollectedEvent(Babara, 3, 3, secondTurnCollectedResources)
                    .ThenVerifyPlayerState().Resources(ResourceClutch.OneOre).End()

                .WhenPlayer(Charlie)
                    .ReceivesStartTurnWithResourcesCollectedEvent(1, 2, thirdTurnCollectedResources)
                    .ThenVerifyPlayerState().Resources((ResourceClutch.OneLumber * 2) + ResourceClutch.OneOre).End()
                    .ThenEndTurn()
                .WhenPlayer(Adam)
                    .ReceivesStartTurnWithResourcesCollectedEvent(Charlie, 1, 2, thirdTurnCollectedResources)
                    .ThenVerifyPlayerState().Resources(ResourceClutch.OneBrick).End()
                .WhenPlayer(Babara)
                    .ReceivesStartTurnWithResourcesCollectedEvent(Charlie, 1, 2, thirdTurnCollectedResources)
                    .ThenVerifyPlayerState().Resources(ResourceClutch.OneGrain + ResourceClutch.OneOre).End()
                .WhenPlayer(Dana)
                    .ReceivesStartTurnWithResourcesCollectedEvent(Charlie, 1, 2, thirdTurnCollectedResources)
                    .ThenVerifyPlayerState().Resources(ResourceClutch.OneOre).End()

                .WhenPlayer(Dana)
                    .ReceivesStartTurnWithResourcesCollectedEvent(6, 4, fourthTurnCollectedResources)
                    .ThenVerifyPlayerState().Resources(ResourceClutch.OneOre).End()
                    .ThenEndTurn()
                .WhenPlayer(Adam)
                    .ReceivesStartTurnWithResourcesCollectedEvent(Dana, 6, 4, fourthTurnCollectedResources)
                    .ThenVerifyPlayerState().Resources(ResourceClutch.OneBrick + ResourceClutch.OneWool).End()
                .WhenPlayer(Babara)
                    .ReceivesStartTurnWithResourcesCollectedEvent(Dana, 6, 4, fourthTurnCollectedResources)
                    .ThenVerifyPlayerState().Resources(ResourceClutch.OneGrain + ResourceClutch.OneOre + ResourceClutch.OneWool).End()
                .WhenPlayer(Charlie)
                    .ReceivesStartTurnWithResourcesCollectedEvent(Dana, 6, 4, fourthTurnCollectedResources)
                    .ThenVerifyPlayerState().Resources((ResourceClutch.OneLumber * 2) + ResourceClutch.OneOre).End()

                .WhenPlayer(Adam)
                    .ReceivesStartTurnEvent(1, 1)
                    .ThenQuitGame()
                .WhenPlayer(Babara)
                    .ReceivesStartTurnEvent(1, 1)
                    .ThenQuitGame()
                .WhenPlayer(Charlie)
                    .ReceivesStartTurnEvent(1, 1)
                    .ThenQuitGame()
                .Run();
        }

        [Test]
        public void AllPlayersCompleteSetup()
        {
            var expectedGameBoardSetup = new GameBoardSetup(new GameBoard(BoardSizes.Standard));
            var playerOrder = new[] { Adam, Babara, Charlie, Dana };
            ScenarioRunner.CreateScenarioRunner(new[] { MethodBase.GetCurrentMethod().Name })
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

        [Test]
        public void AllOtherPlayersQuit()
        {
            this.CompletePlayerInfrastructureSetup(new[] { MethodBase.GetCurrentMethod().Name })
                .WithNoResourceCollection()
                .WhenPlayer(Adam)
                    .ReceivesStartTurnEvent(3, 3).ThenEndTurn()
                    .ReceivesPlayerQuitEvent(Babara).ThenDoNothing()
                    .ReceivesPlayerQuitEvent(Charlie).ThenDoNothing()
                    .ReceivesPlayerQuitEvent(Dana).ThenDoNothing()
                    .ReceivesPlayerWonEvent(Adam, 2).ThenDoNothing()
                .WhenPlayer(Babara)
                    .ReceivesStartTurnEvent(3, 3).ThenQuitGame()
                .WhenPlayer(Charlie)
                    .ReceivesPlayerQuitEvent(Babara).ThenDoNothing()
                    .ReceivesStartTurnEvent(3, 3).ThenQuitGame()
                .WhenPlayer(Dana)
                    .ReceivesPlayerQuitEvent(Babara).ThenDoNothing()
                    .ReceivesPlayerQuitEvent(Charlie).ThenDoNothing()
                    .ReceivesStartTurnEvent(3, 3).ThenQuitGame()
                .VerifyPlayer(Babara)
                    .DidNotReceiveEventOfType<GameWinEvent>()
                    .DidNotReceivePlayerQuitEvent(Babara)
                    .DidNotReceivePlayerQuitEvent(Charlie)
                    .DidNotReceivePlayerQuitEvent(Dana)
                .VerifyPlayer(Charlie)
                    .DidNotReceiveEventOfType<GameWinEvent>()
                    .DidNotReceivePlayerQuitEvent(Charlie)
                    .DidNotReceivePlayerQuitEvent(Dana)
                .VerifyPlayer(Dana)
                    .DidNotReceiveEventOfType<GameWinEvent>()
                    .DidNotReceivePlayerQuitEvent(Dana)
                .Run();
        }

        [Test]
        public void PlayerPlacesCity()
        {
            this.CompletePlayerInfrastructureSetup(new[] { MethodBase.GetCurrentMethod().Name })
                .WithNoResourceCollection()
                .WithInitialPlayerSetupFor(
                    Adam,
                    Resources(ResourceClutch.RoadSegment + ResourceClutch.Settlement + ResourceClutch.City))
                .WhenPlayer(Adam)
                    .ReceivesStartTurnEvent(3, 3).ThenPlaceRoadSegment(4, 3)
                    .ReceivesRoadSegmentPlacementEvent(4, 3).ThenPlaceSettlement(3)
                    .ReceivesSettlementPlacementEvent(3).ThenPlaceCity(3)
                    .ReceivesCityPlacementEvent(3)
                    .ThenVerifyPlayerState()
                        .Resources(ResourceClutch.Zero)
                        .VictoryPoints(4)
                        .Settlements(Player.TotalSettlements - 2)
                        .Cities(Player.TotalCities - 1)
                        .End()
                .WhenPlayer(Babara)
                    .ReceivesRoadSegmentPlacementEvent(Adam, 4, 3).ThenDoNothing()
                    .ReceivesSettlementPlacementEvent(Adam, 3).ThenDoNothing()
                    .ReceivesCityPlacementEvent(Adam, 3).ThenDoNothing()
                .WhenPlayer(Charlie)
                    .ReceivesRoadSegmentPlacementEvent(Adam, 4, 3).ThenDoNothing()
                    .ReceivesSettlementPlacementEvent(Adam, 3).ThenDoNothing()
                    .ReceivesCityPlacementEvent(Adam, 3).ThenDoNothing()
                .WhenPlayer(Dana)
                    .ReceivesRoadSegmentPlacementEvent(Adam, 4, 3).ThenDoNothing()
                    .ReceivesSettlementPlacementEvent(Adam, 3).ThenDoNothing()
                    .ReceivesCityPlacementEvent(Adam, 3).ThenDoNothing()
                .Run();
        }

        [Test]
        public void PlayerPlacesCityAndWins()
        {
            this.CompletePlayerInfrastructureSetup(new[] { MethodBase.GetCurrentMethod().Name })
                .WithNoResourceCollection()
                .WithInitialPlayerSetupFor(
                    Adam,
                    Resources(ResourceClutch.RoadSegment + ResourceClutch.Settlement + ResourceClutch.City),
                    VictoryPoints(6))
                .WhenPlayer(Adam)
                    .ReceivesStartTurnEvent(3, 3).ThenPlaceRoadSegment(4, 3)
                    .ReceivesRoadSegmentPlacementEvent(4, 3).ThenPlaceSettlement(3)
                    .ReceivesSettlementPlacementEvent(3).ThenPlaceCity(3)
                    .ReceivesCityPlacementEvent(3).ThenDoNothing()
                    .ReceivesPlayerWonEvent(10)
                .WhenPlayer(Babara)
                    .ReceivesPlayerWonEvent(Adam, 10).ThenDoNothing()
                .WhenPlayer(Charlie)
                    .ReceivesPlayerWonEvent(Adam, 10).ThenDoNothing()
                .WhenPlayer(Dana)
                    .ReceivesPlayerWonEvent(Adam, 10).ThenDoNothing()
                .WhenPlayer(Adam)
                    .ThenVerifyPlayerState()
                        .Resources(ResourceClutch.Zero)
                        .VictoryPoints(10)
                        .Cities(Player.TotalCities - 1)
                        .End()
                .Run();
        }

        [Test]
        public void PlayerTriesToPlaceCityOnLocationOccupiedByPlayer()
        {
            this.CompletePlayerInfrastructureSetup(new[] { MethodBase.GetCurrentMethod().Name })
                .WithNoResourceCollection()
                .WithInitialPlayerSetupFor(
                    Adam,
                    Resources(ResourceClutch.City * 2))
                .WhenPlayer(Adam)
                    .ReceivesStartTurnEvent(3, 3).ThenPlaceCity(Adam_FirstSettlementLocation)
                    .ReceivesCityPlacementEvent(Adam_FirstSettlementLocation).ThenPlaceCity(Adam_FirstSettlementLocation)
                    .ReceivesGameErrorEvent("908", $"Location ({Adam_FirstSettlementLocation}) already occupied by you").ThenDoNothing()
                .VerifyPlayer(Babara)
                    .DidNotReceiveEventOfType<GameErrorEvent>()
                .VerifyPlayer(Charlie)
                    .DidNotReceiveEventOfType<GameErrorEvent>()
                .VerifyPlayer(Dana)
                    .DidNotReceiveEventOfType<GameErrorEvent>()
                .Run();
        }

        [Test]
        public void PlayerTriesToPlaceCityOnInvalidLocation()
        {
            this.CompletePlayerInfrastructureSetup(new[] { MethodBase.GetCurrentMethod().Name })
                .WithNoResourceCollection()
                .WithInitialPlayerSetupFor(
                    Adam,
                    Resources(ResourceClutch.City))
                .WhenPlayer(Adam)
                    .ReceivesStartTurnEvent(3, 3).ThenPlaceCity(100)
                    .ReceivesGameErrorEvent("915", $"Location (100) is invalid").ThenDoNothing()
                .VerifyPlayer(Babara)
                    .DidNotReceiveEventOfType<GameErrorEvent>()
                .VerifyPlayer(Charlie)
                    .DidNotReceiveEventOfType<GameErrorEvent>()
                .VerifyPlayer(Dana)
                    .DidNotReceiveEventOfType<GameErrorEvent>()
                .Run();
        }

        [Test]
        public void PlayerTriesToPlaceCityOnLocationOccupiedByOtherPlayer()
        {
            this.CompletePlayerInfrastructureSetup(new[] { MethodBase.GetCurrentMethod().Name })
                .WithNoResourceCollection()
                .WithInitialPlayerSetupFor(
                    Adam,
                    Resources((ResourceClutch.RoadSegment * 2) + ResourceClutch.Settlement))
                .WithInitialPlayerSetupFor(
                    Charlie,
                    Resources(ResourceClutch.RoadSegment + ResourceClutch.City))
                .WhenPlayer(Adam)
                    .ReceivesStartTurnEvent(3, 3).ThenPlaceRoadSegment(12, 13)
                    .ReceivesRoadSegmentPlacementEvent(12, 13).ThenPlaceRoadSegment(13, 14)
                    .ReceivesRoadSegmentPlacementEvent(13, 14).ThenPlaceSettlement(14)
                    .ReceivesSettlementPlacementEvent(14).ThenEndTurn()
                .WhenPlayer(Babara)
                    .ReceivesStartTurnEvent(3, 3).ThenEndTurn()
                .WhenPlayer(Charlie)
                    .ReceivesStartTurnEvent(3, 3).ThenPlaceRoadSegment(15, 14)
                    .ReceivesRoadSegmentPlacementEvent(15, 14).ThenPlaceCity(14)
                    .ReceivesGameErrorEvent("908", "Location (14) already occupied by Adam").ThenDoNothing()
                .VerifyPlayer(Adam)
                    .DidNotReceiveEventOfType<GameErrorEvent>()
                .VerifyPlayer(Babara)
                    .DidNotReceiveEventOfType<GameErrorEvent>()
                .VerifyPlayer(Dana)
                    .DidNotReceiveEventOfType<GameErrorEvent>()
                .Run();
        }

        [Test]
        public void PlayerTriesToPlaceCityOnLocationWithoutSettlement()
        {
            this.CompletePlayerInfrastructureSetup(new[] { MethodBase.GetCurrentMethod().Name })
                .WithNoResourceCollection()
                .WithInitialPlayerSetupFor(
                    Adam,
                    Resources(ResourceClutch.RoadSegment + ResourceClutch.City))
                .WhenPlayer(Adam)
                    .ReceivesStartTurnEvent(3, 3).ThenPlaceRoadSegment(3, 4)
                    .ReceivesRoadSegmentPlacementEvent(3, 4).ThenPlaceCity(3)
                    .ReceivesGameErrorEvent("914", "Location (3) not an settlement").ThenDoNothing()
                .VerifyPlayer(Babara)
                    .DidNotReceiveEventOfType<GameErrorEvent>()
                .VerifyPlayer(Charlie)
                    .DidNotReceiveEventOfType<GameErrorEvent>()
                .VerifyPlayer(Dana)
                    .DidNotReceiveEventOfType<GameErrorEvent>()
                .Run();
        }

        [Test]
        public void PlayerTriesToPlaceCityWithNotEnoughResources()
        {
            this.CompletePlayerInfrastructureSetup(new[] { MethodBase.GetCurrentMethod().Name })
                .WithNoResourceCollection()
                .WhenPlayer(Adam)
                    .ReceivesStartTurnEvent(3, 3).ThenPlaceCity(Adam_FirstSettlementLocation)
                    .ReceivesGameErrorEvent("913", "Not enough resources for placing city").ThenDoNothing()
                .VerifyPlayer(Babara)
                    .DidNotReceiveEventOfType<GameErrorEvent>()
                .VerifyPlayer(Charlie)
                    .DidNotReceiveEventOfType<GameErrorEvent>()
                .VerifyPlayer(Dana)
                    .DidNotReceiveEventOfType<GameErrorEvent>()
                .Run();
        }

        [Test]
        public void PlayerPlacesSettlement()
        {
            this.CompletePlayerInfrastructureSetup(new[] { MethodBase.GetCurrentMethod().Name })
                .WithNoResourceCollection()
                .WithInitialPlayerSetupFor(
                    Adam,
                    Resources(ResourceClutch.RoadSegment + ResourceClutch.Settlement))
                .WhenPlayer(Adam)
                    .ReceivesStartTurnEvent(3, 3)
                    .ThenPlaceRoadSegment(4, 3)
                    .ReceivesRoadSegmentPlacementEvent(4, 3)
                    .ThenPlaceSettlement(3)
                    .ReceivesSettlementPlacementEvent(3)
                    .ThenVerifyPlayerState()
                        .Resources(ResourceClutch.Zero)
                        .Settlements(Player.TotalSettlements - 3)
                        .VictoryPoints(3)
                        .End()
                .WhenPlayer(Babara)
                    .ReceivesRoadSegmentPlacementEvent(Adam, 4, 3).ThenDoNothing()
                    .ReceivesSettlementPlacementEvent(Adam, 3).ThenDoNothing()
                .WhenPlayer(Charlie)
                    .ReceivesRoadSegmentPlacementEvent(Adam, 4, 3).ThenDoNothing()
                    .ReceivesSettlementPlacementEvent(Adam, 3).ThenDoNothing()
                .WhenPlayer(Dana)
                    .ReceivesRoadSegmentPlacementEvent(Adam, 4, 3).ThenDoNothing()
                    .ReceivesSettlementPlacementEvent(Adam, 3).ThenDoNothing()
                .Run();
        }

        [Test]
        public void PlayerPlacesSettlementAndWins()
        {
            this.CompletePlayerInfrastructureSetup(new[] { MethodBase.GetCurrentMethod().Name })
                .WithNoResourceCollection()
                .WithInitialPlayerSetupFor(
                    Adam, 
                    Resources(ResourceClutch.RoadSegment + ResourceClutch.Settlement),
                    VictoryPoints(7)) // Account for placing infrastructure
                .WhenPlayer(Adam)
                    .ReceivesStartTurnEvent(3, 3).ThenPlaceRoadSegment(4, 3)
                    .ReceivesRoadSegmentPlacementEvent(4, 3).ThenPlaceSettlement(3)
                    .ReceivesSettlementPlacementEvent(3)
                .WhenPlayer(Babara)
                    .ReceivesRoadSegmentPlacementEvent(Adam, 4, 3).ThenDoNothing()
                    .ReceivesSettlementPlacementEvent(Adam, 3).ThenDoNothing()
                .WhenPlayer(Charlie)
                    .ReceivesRoadSegmentPlacementEvent(Adam, 4, 3).ThenDoNothing()
                    .ReceivesSettlementPlacementEvent(Adam, 3).ThenDoNothing()
                .WhenPlayer(Dana)
                    .ReceivesRoadSegmentPlacementEvent(Adam, 4, 3).ThenDoNothing()
                    .ReceivesSettlementPlacementEvent(Adam, 3).ThenDoNothing()
                .VerifyAllPlayersReceivedGameWonEvent(Adam, 10)
                .WhenPlayer(Adam)
                    .ThenVerifyPlayerState()
                        .Resources(ResourceClutch.Zero)
                        .VictoryPoints(10)
                        .End()
                .Run();
        }

        [Test]
        public void PlayerTriesToPlaceSettlementOnInvalidLocation()
        {
            this.CompletePlayerInfrastructureSetup(new[] { MethodBase.GetCurrentMethod().Name })
                .WithNoResourceCollection()
                .WithInitialPlayerSetupFor(
                    Adam,
                    Resources(ResourceClutch.Settlement))
                .WhenPlayer(Adam)
                    .ReceivesStartTurnEvent(3, 3).ThenPlaceSettlement(100)
                    .ReceivesGameErrorEvent("915", $"Location (100) is invalid").ThenDoNothing()
                .VerifyPlayer(Babara)
                    .DidNotReceiveEventOfType<GameErrorEvent>()
                .VerifyPlayer(Charlie)
                    .DidNotReceiveEventOfType<GameErrorEvent>()
                .VerifyPlayer(Dana)
                    .DidNotReceiveEventOfType<GameErrorEvent>()
                .Run();
        }

        [Test]
        public void PlayerTriesToPlaceSettlementOnLocationOccupiedByPlayer()
        {
            this.CompletePlayerInfrastructureSetup(new[] { MethodBase.GetCurrentMethod().Name })
                .WithNoResourceCollection()
                .WithInitialPlayerSetupFor(
                    Adam,
                    Resources(ResourceClutch.RoadSegment + (ResourceClutch.Settlement * 2)))
                .WhenPlayer(Adam)
                    .ReceivesStartTurnEvent(3, 3).ThenPlaceRoadSegment(4, 3)
                    .ReceivesRoadSegmentPlacementEvent(4, 3).ThenPlaceSettlement(3)
                    .ReceivesSettlementPlacementEvent(3).ThenPlaceSettlement(3)
                    .ReceivesGameErrorEvent("908", "Location (3) already occupied by you").ThenDoNothing()
                .VerifyPlayer(Babara)
                    .DidNotReceiveEventOfType<GameErrorEvent>()
                .VerifyPlayer(Charlie)
                    .DidNotReceiveEventOfType<GameErrorEvent>()
                .VerifyPlayer(Dana)
                    .DidNotReceiveEventOfType<GameErrorEvent>()
                .Run();
        }

        [Test]
        public void PlayerTriesToPlaceSettlementOnLocationOccupiedByOtherPlayer()
        {
            this.CompletePlayerInfrastructureSetup(new[] { MethodBase.GetCurrentMethod().Name })
                .WithNoResourceCollection()
                .WithInitialPlayerSetupFor(
                    Adam,
                    Resources((ResourceClutch.RoadSegment * 2) + ResourceClutch.Settlement))
                .WithInitialPlayerSetupFor(
                    Charlie,
                    Resources(ResourceClutch.RoadSegment + ResourceClutch.Settlement))
                .WhenPlayer(Adam)
                    .ReceivesStartTurnEvent(3, 3).ThenPlaceRoadSegment(12, 13)
                    .ReceivesRoadSegmentPlacementEvent(12, 13).ThenPlaceRoadSegment(13, 14)
                    .ReceivesRoadSegmentPlacementEvent(13, 14).ThenPlaceSettlement(14)
                    .ReceivesSettlementPlacementEvent(14).ThenEndTurn()
                .WhenPlayer(Babara)
                    .ReceivesStartTurnEvent(3, 3).ThenEndTurn()
                .WhenPlayer(Charlie)
                    .ReceivesStartTurnEvent(3, 3).ThenPlaceRoadSegment(15, 14)
                    .ReceivesRoadSegmentPlacementEvent(15, 14).ThenPlaceSettlement(14)
                    .ReceivesGameErrorEvent("908", "Location (14) already occupied by Adam").ThenDoNothing()
                .VerifyPlayer(Adam)
                    .DidNotReceiveEventOfType<GameErrorEvent>()
                .VerifyPlayer(Babara)
                    .DidNotReceiveEventOfType<GameErrorEvent>()
                .VerifyPlayer(Dana)
                    .DidNotReceiveEventOfType<GameErrorEvent>()
                .Run();
        }

        [Test]
        public void PlayerTriesToPlaceSettlementOnUnconnectedLocation()
        {
            this.CompletePlayerInfrastructureSetup(new[] { MethodBase.GetCurrentMethod().Name })
                .WithNoResourceCollection()
                .WithInitialPlayerSetupFor(
                    Adam,
                    Resources(ResourceClutch.Settlement))
                .WhenPlayer(Adam)
                    .ReceivesStartTurnEvent(3, 3).ThenPlaceSettlement(3)
                    .ReceivesGameErrorEvent("909", "Location (3) is not connected to your road system").ThenDoNothing()
                .VerifyPlayer(Babara)
                    .DidNotReceiveEventOfType<GameErrorEvent>()
                .VerifyPlayer(Charlie)
                    .DidNotReceiveEventOfType<GameErrorEvent>()
                .VerifyPlayer(Dana)
                    .DidNotReceiveEventOfType<GameErrorEvent>()
                .Run();
        }

        [Test]
        public void PlayerTriesToPlaceSettlementWithNoSettlementsLeft()
        {
            this.CompletePlayerInfrastructureSetup(new[] { MethodBase.GetCurrentMethod().Name })
                .WithNoResourceCollection()
                .WithInitialPlayerSetupFor(
                    Adam,
                    Resources(ResourceClutch.Settlement), PlacedSettlements(Player.TotalSettlements - 2))
                .WhenPlayer(Adam)
                    .ReceivesStartTurnEvent(3, 3).ThenPlaceSettlement(3)
                    .ReceivesGameErrorEvent("911", "No settlements to place").ThenDoNothing()
                .VerifyPlayer(Babara)
                    .DidNotReceiveEventOfType<GameErrorEvent>()
                .VerifyPlayer(Charlie)
                    .DidNotReceiveEventOfType<GameErrorEvent>()
                .VerifyPlayer(Dana)
                    .DidNotReceiveEventOfType<GameErrorEvent>()
                .Run();
        }

        [Test]
        public void PlayerTriesToPlaceSettlementWithNotEnoughResources()
        {
            this.CompletePlayerInfrastructureSetup(new[] { MethodBase.GetCurrentMethod().Name })
                .WithNoResourceCollection()
                .WhenPlayer(Adam)
                    .ReceivesStartTurnEvent(3, 3).ThenPlaceSettlement(3)
                    .ReceivesGameErrorEvent("912", "Not enough resources for placing settlement").ThenDoNothing()
                .VerifyPlayer(Babara)
                    .DidNotReceiveEventOfType<GameErrorEvent>()
                .VerifyPlayer(Charlie)
                    .DidNotReceiveEventOfType<GameErrorEvent>()
                .VerifyPlayer(Dana)
                    .DidNotReceiveEventOfType<GameErrorEvent>()
                .Run();
        }

        [Test]
        public void PlayerTriesToPlaceRoadSegmentWithInvalidLocations()
        {
            this.CompletePlayerInfrastructureSetup(new[] { MethodBase.GetCurrentMethod().Name })
                .WithNoResourceCollection()
                .WithInitialPlayerSetupFor(Adam, Resources(ResourceClutch.RoadSegment))
                .WhenPlayer(Adam)
                    .ReceivesStartTurnEvent(3, 3)
                    .ThenPlaceRoadSegment(4, 55)
                    .ReceivesGameErrorEvent("903", "Locations (4, 55) invalid for placing road segment").ThenDoNothing()
                .VerifyPlayer(Babara)
                    .DidNotReceiveEventOfType<GameErrorEvent>()
                .VerifyPlayer(Charlie)
                    .DidNotReceiveEventOfType<GameErrorEvent>()
                .VerifyPlayer(Dana)
                    .DidNotReceiveEventOfType<GameErrorEvent>()
                .Run();
        }

        [Test]
        public void PlayerTriesToPlaceRoadSegmentWithUnconnectedLocations()
        {
            this.CompletePlayerInfrastructureSetup(new[] { MethodBase.GetCurrentMethod().Name })
                .WithNoResourceCollection()
                .WithInitialPlayerSetupFor(Adam, Resources(ResourceClutch.RoadSegment))
                .WhenPlayer(Adam)
                    .ReceivesStartTurnEvent(3, 3)
                    .ThenPlaceRoadSegment(4, 0)
                    .ReceivesGameErrorEvent("904", "Locations (4, 0) not connected when placing road segment").ThenDoNothing()
                .VerifyPlayer(Babara)
                    .DidNotReceiveEventOfType<GameErrorEvent>()
                .VerifyPlayer(Charlie)
                    .DidNotReceiveEventOfType<GameErrorEvent>()
                .VerifyPlayer(Dana)
                    .DidNotReceiveEventOfType<GameErrorEvent>()
                .Run();
        }

        [Test]
        public void PlayerTriesToPlaceRoadSegmentWithNoRoadSegmentsLeft()
        {
            this.CompletePlayerInfrastructureSetup(new[] { MethodBase.GetCurrentMethod().Name })
                .WithNoResourceCollection()
                .WithInitialPlayerSetupFor(Adam, Resources(ResourceClutch.RoadSegment), PlacedRoadSegments(Player.TotalRoadSegments - 2))
                .WhenPlayer(Adam)
                    .ReceivesStartTurnEvent(3, 3)
                    .ThenPlaceRoadSegment(4, 3)
                    .ReceivesGameErrorEvent("905", "No road segments to place").ThenDoNothing()
                .VerifyPlayer(Babara)
                    .DidNotReceiveEventOfType<GameErrorEvent>()
                .VerifyPlayer(Charlie)
                    .DidNotReceiveEventOfType<GameErrorEvent>()
                .VerifyPlayer(Dana)
                    .DidNotReceiveEventOfType<GameErrorEvent>()
                .Run();
        }

        [Test]
        public void PlayerTriesToPlaceRoadSegmentWithNotEnoughResources()
        {
            this.CompletePlayerInfrastructureSetup(new[] { MethodBase.GetCurrentMethod().Name })
                .WithNoResourceCollection()
                .WhenPlayer(Adam)
                    .ReceivesStartTurnEvent(3, 3)
                    .ThenPlaceRoadSegment(4, 3)
                    .ReceivesGameErrorEvent("906", "Not enough resources for placing road segment").ThenDoNothing()
                .VerifyPlayer(Babara)
                    .DidNotReceiveEventOfType<GameErrorEvent>()
                .VerifyPlayer(Charlie)
                    .DidNotReceiveEventOfType<GameErrorEvent>()
                .VerifyPlayer(Dana)
                    .DidNotReceiveEventOfType<GameErrorEvent>()
                .Run();
        }

        [Test]
        public void PlayerTriesToPlaceRoadSegmentOnOccupiedLocations()
        {
            this.CompletePlayerInfrastructureSetup(new[] { MethodBase.GetCurrentMethod().Name })
                .WithNoResourceCollection()
                .WithInitialPlayerSetupFor(Adam, Resources(ResourceClutch.RoadSegment))
                .WhenPlayer(Adam)
                    .ReceivesStartTurnEvent(3, 3)
                    .ThenPlaceRoadSegment(4, 12)
                    .ReceivesGameErrorEvent("907", "Cannot place road segment on existing road segment (4, 12)").ThenDoNothing()
                .VerifyPlayer(Babara)
                    .DidNotReceiveEventOfType<GameErrorEvent>()
                .VerifyPlayer(Charlie)
                    .DidNotReceiveEventOfType<GameErrorEvent>()
                .VerifyPlayer(Dana)
                    .DidNotReceiveEventOfType<GameErrorEvent>()
                .Run();
        }

        [Test]
        public void PlayerTriesToPlaceRoadSegmentOnLocationsNotConnectedToExistingRoadSystem()
        {
            this.CompletePlayerInfrastructureSetup(new[] { MethodBase.GetCurrentMethod().Name })
                .WithNoResourceCollection()
                .WithInitialPlayerSetupFor(Adam, Resources(ResourceClutch.RoadSegment))
                .WhenPlayer(Adam)
                    .ReceivesStartTurnEvent(3, 3)
                    .ThenPlaceRoadSegment(3, 2)
                    .ReceivesGameErrorEvent("910", "Cannot place road segment because locations (3, 2) are not connected to existing road").ThenDoNothing()
                .VerifyPlayer(Babara)
                    .DidNotReceiveEventOfType<GameErrorEvent>()
                .VerifyPlayer(Charlie)
                    .DidNotReceiveEventOfType<GameErrorEvent>()
                .VerifyPlayer(Dana)
                    .DidNotReceiveEventOfType<GameErrorEvent>()
                .Run();
        }

        [Test]
        public void PlayerPlaysKnightCard()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void PlayerRollsSevenAndAllPlayersWithMoreThanSevenResourcesLoseResources()
        {
            var adamsInitialResources = new ResourceClutch(1, 2, 2, 2, 2); // 9 resources
            var babarasInitialResources = new ResourceClutch(2, 2, 2, 2, 2); // 10 resources
            var charliesInitialResources = new ResourceClutch(1, 1, 1, 1, 2); // 6 resources
            var danasInitialResources = new ResourceClutch(1, 1, 2, 2, 2); // 8 resources

            var adamsLostResources = new ResourceClutch(0, 1, 1, 1, 1);
            var babarasLostResources = new ResourceClutch(1, 1, 1, 1, 1);
            var danasLostResources = new ResourceClutch(0, 1, 1, 1, 1);

            var adamsFinalResources = adamsInitialResources - adamsLostResources;
            var babarasFinalResources = babarasInitialResources - babarasLostResources;
            var danasFinalResources = danasInitialResources - danasLostResources;

            this.CompletePlayerInfrastructureSetup(new[] { MethodBase.GetCurrentMethod().Name })
                .WithNoResourceCollection()
                .WithInitialPlayerSetupFor(
                    Adam,
                    Resources(adamsInitialResources))
                .WithInitialPlayerSetupFor(
                    Babara,
                    Resources(babarasInitialResources))
                .WithInitialPlayerSetupFor(
                    Charlie,
                    Resources(charliesInitialResources))
                .WithInitialPlayerSetupFor(
                    Dana,
                    Resources(danasInitialResources))
                .WhenPlayer(Adam)
                    .ReceivesStartTurnEvent(3, 4).ThenDoNothing()
                    .ReceivesChooseLostResourcesEvent(4).ThenChooseResourcesToLose(adamsLostResources)
                    .ReceivesAll()
                        .ReceivesResourcesLostEvent(Adam, adamsLostResources)
                        .ReceivesResourcesLostEvent(Babara, babarasLostResources)
                        .ReceivesResourcesLostEvent(Dana, danasLostResources)
                    .ReceivesAllEnd()
                    .ThenVerifyPlayerState()
                        .Resources(adamsFinalResources)
                        .End()
                .WhenPlayer(Babara)
                    .ReceivesChooseLostResourcesEvent(5).ThenChooseResourcesToLose(babarasLostResources)
                    .ReceivesAll()
                        .ReceivesResourcesLostEvent(Adam, adamsLostResources)
                        .ReceivesResourcesLostEvent(Babara, babarasLostResources)
                        .ReceivesResourcesLostEvent(Dana, danasLostResources)
                    .ReceivesAllEnd()
                    .ThenVerifyPlayerState()
                        .Resources(babarasFinalResources)
                        .End()
                .WhenPlayer(Charlie)
                    .ReceivesAll()
                        .ReceivesResourcesLostEvent(Adam, adamsLostResources)
                        .ReceivesResourcesLostEvent(Babara, babarasLostResources)
                        .ReceivesResourcesLostEvent(Dana, danasLostResources)
                    .ReceivesAllEnd()
                    .ThenVerifyPlayerState()
                        .Resources(charliesInitialResources)
                        .End()
                .WhenPlayer(Dana)
                    .ReceivesChooseLostResourcesEvent(4).ThenChooseResourcesToLose(danasLostResources)
                    .ReceivesAll()
                        .ReceivesResourcesLostEvent(Adam, adamsLostResources)
                        .ReceivesResourcesLostEvent(Babara, babarasLostResources)
                        .ReceivesResourcesLostEvent(Dana, danasLostResources)
                    .ReceivesAllEnd()
                    .ThenVerifyPlayerState()
                        .Resources(danasFinalResources)
                        .End()
                .VerifyPlayer(Charlie)
                    .DidNotReceiveEventOfType<ChooseLostResourcesEvent>()
                .Run();
        }

        [Test]
        public void PlayerRollsSevenAndPlayerSendsTooManyResources()
        {
            var adamsInitialResources = new ResourceClutch(1, 2, 2, 2, 2); // 9 resources
            var adamsLostResources = new ResourceClutch(1, 1, 1, 1, 1);

            this.CompletePlayerInfrastructureSetup(new[] { MethodBase.GetCurrentMethod().Name })
                .WithNoResourceCollection()
                .WithInitialPlayerSetupFor(
                    Adam,
                    Resources(adamsInitialResources))
                .WhenPlayer(Adam)
                    .ReceivesStartTurnEvent(3, 4).ThenDoNothing()
                    .ReceivesChooseLostResourcesEvent(4).ThenChooseResourcesToLose(adamsLostResources)
                    .ReceivesGameErrorEvent("916", "Expected 4 resources but received 5")
                .VerifyPlayer(Babara)
                    .DidNotReceiveEventOfType<GameErrorEvent>()
                .VerifyPlayer(Charlie)
                    .DidNotReceiveEventOfType<GameErrorEvent>()
                .VerifyPlayer(Dana)
                    .DidNotReceiveEventOfType<GameErrorEvent>()
                .Run();
        }
        
        [Test]
        public void PlayerRollsSevenAndPlayerSendsTooLittleResources()
        {
            var adamsInitialResources = new ResourceClutch(1, 2, 2, 2, 2); // 9 resources
            var adamsLostResources = new ResourceClutch(0, 0, 1, 1, 1);

            this.CompletePlayerInfrastructureSetup(new[] { MethodBase.GetCurrentMethod().Name })
                .WithNoResourceCollection()
                .WithInitialPlayerSetupFor(
                    Adam,
                    Resources(adamsInitialResources))
                .WhenPlayer(Adam)
                    .ReceivesStartTurnEvent(3, 4).ThenDoNothing()
                    .ReceivesChooseLostResourcesEvent(4).ThenChooseResourcesToLose(adamsLostResources)
                    .ReceivesGameErrorEvent("916", "Expected 4 resources but received 3")
                .VerifyPlayer(Babara)
                    .DidNotReceiveEventOfType<GameErrorEvent>()
                .VerifyPlayer(Charlie)
                    .DidNotReceiveEventOfType<GameErrorEvent>()
                .VerifyPlayer(Dana)
                    .DidNotReceiveEventOfType<GameErrorEvent>()
                .Run();
        }

        [Test]
        public void PlayerRollsSevenAndPlayerSendsResourcesResultingInNegativeResources()
        {
            var adamsInitialResources = new ResourceClutch(1, 2, 2, 2, 2); // 9 resources
            var adamsLostResources = new ResourceClutch(2, 0, 0, 1, 1);

            this.CompletePlayerInfrastructureSetup(new[] { MethodBase.GetCurrentMethod().Name })
                .WithNoResourceCollection()
                .WithInitialPlayerSetupFor(
                    Adam,
                    Resources(adamsInitialResources))
                .WhenPlayer(Adam)
                    .ReceivesStartTurnEvent(3, 4).ThenDoNothing()
                    .ReceivesChooseLostResourcesEvent(4).ThenChooseResourcesToLose(adamsLostResources)
                    .ReceivesGameErrorEvent("917", "Resources sent results in negative counts")
                .VerifyPlayer(Babara)
                    .DidNotReceiveEventOfType<GameErrorEvent>()
                .VerifyPlayer(Charlie)
                    .DidNotReceiveEventOfType<GameErrorEvent>()
                .VerifyPlayer(Dana)
                    .DidNotReceiveEventOfType<GameErrorEvent>()
                .Run();
        }

        [Test]
        public void PlayerRollsSevenAndAllPlayersWithMoreThanSeventResourcesMustSendsResourcesBeforeRobberCanBePlaced()
        {
            var adamsInitialResources = new ResourceClutch(1, 2, 2, 2, 2); // 9 resources
            var babarasInitialResources = new ResourceClutch(2, 2, 2, 2, 2); // 10 resources
            var charliesInitialResources = new ResourceClutch(1, 1, 1, 1, 2); // 6 resources
            var danasInitialResources = new ResourceClutch(1, 1, 2, 2, 2); // 8 resources

            var adamsLostResources = new ResourceClutch(0, 1, 1, 1, 1);
            var babarasLostResources = new ResourceClutch(1, 1, 1, 1, 1);
            var danasLostResources = new ResourceClutch(0, 1, 1, 1, 1);

            var adamsFinalResources = adamsInitialResources - adamsLostResources;
            var babarasFinalResources = babarasInitialResources - babarasLostResources;
            var danasFinalResources = danasInitialResources - danasLostResources;

            this.CompletePlayerInfrastructureSetup(new[] { MethodBase.GetCurrentMethod().Name })
                .WithNoResourceCollection()
                .WithInitialPlayerSetupFor(
                    Adam,
                    Resources(adamsInitialResources))
                .WithInitialPlayerSetupFor(
                    Babara,
                    Resources(babarasInitialResources))
                .WithInitialPlayerSetupFor(
                    Charlie,
                    Resources(charliesInitialResources))
                .WithInitialPlayerSetupFor(
                    Dana,
                    Resources(danasInitialResources))
                .WhenPlayer(Adam)
                    .ReceivesStartTurnEvent(3, 4).ThenDoNothing()
                    .ReceivesChooseLostResourcesEvent(4).ThenChooseResourcesToLose(adamsLostResources)
                    .ReceivesAll()
                        .ReceivesResourcesLostEvent(Adam, adamsLostResources)
                        .ReceivesResourcesLostEvent(Babara, babarasLostResources)
                        .ReceivesResourcesLostEvent(Dana, danasLostResources)
                    .ReceivesAllEnd().ThenDoNothing()
                    .ReceivesPlaceRobberEvent()
                .WhenPlayer(Babara)
                    .ReceivesChooseLostResourcesEvent(5).ThenChooseResourcesToLose(babarasLostResources)
                    .ReceivesAll()
                        .ReceivesResourcesLostEvent(Adam, adamsLostResources)
                        .ReceivesResourcesLostEvent(Babara, babarasLostResources)
                        .ReceivesResourcesLostEvent(Dana, danasLostResources)
                    .ReceivesAllEnd()
                    .ThenVerifyPlayerState()
                        .Resources(babarasFinalResources)
                        .End()
                .WhenPlayer(Charlie)
                    .ReceivesAll()
                        .ReceivesResourcesLostEvent(Adam, adamsLostResources)
                        .ReceivesResourcesLostEvent(Babara, babarasLostResources)
                        .ReceivesResourcesLostEvent(Dana, danasLostResources)
                    .ReceivesAllEnd()
                    .ThenVerifyPlayerState()
                        .Resources(charliesInitialResources)
                        .End()
                .WhenPlayer(Dana)
                    .ReceivesChooseLostResourcesEvent(4).ThenChooseResourcesToLose(danasLostResources)
                    .ReceivesAll()
                        .ReceivesResourcesLostEvent(Adam, adamsLostResources)
                        .ReceivesResourcesLostEvent(Babara, babarasLostResources)
                        .ReceivesResourcesLostEvent(Dana, danasLostResources)
                    .ReceivesAllEnd()
                    .ThenVerifyPlayerState()
                        .Resources(danasFinalResources)
                        .End()
                .VerifyPlayer(Charlie)
                    .DidNotReceiveEventOfType<ChooseLostResourcesEvent>()
                .Run();
        }

        [Test]
        public void PlayerRollsSevenAndNewHexHasNoPlayers()
        {
            this.CompletePlayerInfrastructureSetup(new[] { MethodBase.GetCurrentMethod().Name })
                .WithNoResourceCollection()
                .WhenPlayer(Adam)
                    .ReceivesStartTurnEvent(3, 4).ThenDoNothing()
                    .ReceivesPlaceRobberEvent().ThenPlaceRobber(4)
                .WhenPlayer(Babara)
                    .ReceivesRobberPlacedEvent(Adam, 4).ThenDoNothing()
                .WhenPlayer(Charlie)
                    .ReceivesRobberPlacedEvent(Adam, 4).ThenDoNothing()
                .WhenPlayer(Dana)
                    .ReceivesRobberPlacedEvent(Adam, 4).ThenDoNothing()
                .VerifyPlayer(Babara)
                    .DidNotReceiveEventOfType<ChooseLostResourcesEvent>()
                .VerifyPlayer(Charlie)
                    .DidNotReceiveEventOfType<ChooseLostResourcesEvent>()
                .VerifyPlayer(Dana)
                    .DidNotReceiveEventOfType<ChooseLostResourcesEvent>()
                .Run();
        }

        [Test]
        public void PlayerRollsSevenAndNewHexIsSameAsCurrentHex()
        {
            throw new NotImplementedException();
            /*this.CompletePlayerInfrastructureSetup(new[] { MethodBase.GetCurrentMethod().Name })
                .WithNoResourceCollection()
                .WhenPlayer(Adam)
                    .ReceivesStartTurnEvent(3, 4).ThenDoNothing()
                    .ReceivesPlaceRobberEvent().ThenPlaceRobber()
                .VerifyPlayer(Babara)
                    .DidNotReceiveEventOfType<GameErrorEvent>()
                .VerifyPlayer(Charlie)
                    .DidNotReceiveEventOfType<GameErrorEvent>()
                .VerifyPlayer(Dana)
                    .DidNotReceiveEventOfType<GameErrorEvent>()
                .Run();*/
        }

        [Test]
        public void PlayerRollsSevenAndNewHexHasOnePlayer()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void PlayerRollsSevenAndNewHexHasOnePlayerWhichIsRollingPlayer()
        {
            this.CompletePlayerInfrastructureSetup(new[] { MethodBase.GetCurrentMethod().Name })
                .WithNoResourceCollection()
                .WhenPlayer(Adam)
                    .ReceivesStartTurnEvent(3, 4).ThenDoNothing()
                    .ReceivesPlaceRobberEvent().ThenPlaceRobber(2)
                .WhenPlayer(Babara)
                    .ReceivesRobberPlacedEvent(Adam, 2).ThenDoNothing()
                .WhenPlayer(Charlie)
                    .ReceivesRobberPlacedEvent(Adam, 2).ThenDoNothing()
                .WhenPlayer(Dana)
                    .ReceivesRobberPlacedEvent(Adam, 2).ThenDoNothing()
                .VerifyPlayer(Adam)
                    .DidNotReceiveEventOfType<ChooseLostResourcesEvent>()
                .VerifyPlayer(Babara)
                    .DidNotReceiveEventOfType<ChooseLostResourcesEvent>()
                .VerifyPlayer(Charlie)
                    .DidNotReceiveEventOfType<ChooseLostResourcesEvent>()
                .VerifyPlayer(Dana)
                    .DidNotReceiveEventOfType<ChooseLostResourcesEvent>()
                .Run();
        }

        [Test]
        public void PlayerRollsSevenAndNewHexHasMultiplePlayers()
        {
            var infrastructureSetupBuilder = new InfrastructureSetupBuilder();
            infrastructureSetupBuilder.Add(Adam, Adam_FirstSettlementLocation, Adam_FirstRoadEndLocation)
                .Add(Babara, 21, 11)
                .Add(Charlie, Charlie_FirstSettlementLocation, Charlie_FirstRoadEndLocation)
                .Add(Dana, Dana_FirstSettlementLocation, Dana_FirstRoadEndLocation)
                .Add(Dana, 35, 24)
                .Add(Charlie, 33, 32)
                .Add(Babara, Babara_SecondSettlementLocation, Babara_SecondRoadEndLocation)
                .Add(Adam, Adam_SecondSettlementLocation, Adam_SecondRoadEndLocation);

            var robbingChoices = new Dictionary<string, int>()
            {
                { Babara, 3 },
                { Charlie, 3 },
                { Dana, 3 }
            };

            this.CompletePlayerInfrastructureSetup(infrastructureSetupBuilder.Build(), new[] { MethodBase.GetCurrentMethod().Name })
                .WhenPlayer(Adam)
                    .ReceivesStartTurnEvent(3, 4).ThenDoNothing()
                    .ReceivesPlaceRobberEvent().ThenPlaceRobber(9)
                    .ReceivesRobbingChoicesEvent(robbingChoices).ThenDoNothing()
                .WhenPlayer(Babara)
                    .ReceivesRobberPlacedEvent(Adam, 9).ThenDoNothing()
                .WhenPlayer(Charlie)
                    .ReceivesRobberPlacedEvent(Adam, 9).ThenDoNothing()
                .WhenPlayer(Dana)
                    .ReceivesRobberPlacedEvent(Adam, 9).ThenDoNothing()
                .VerifyPlayer(Adam)
                    .DidNotReceiveEventOfType<ChooseLostResourcesEvent>()
                .VerifyPlayer(Babara)
                    .DidNotReceiveEventOfType<ChooseLostResourcesEvent>()
                .VerifyPlayer(Charlie)
                    .DidNotReceiveEventOfType<ChooseLostResourcesEvent>()
                .VerifyPlayer(Dana)
                    .DidNotReceiveEventOfType<ChooseLostResourcesEvent>()
                .Run();
        }

        [Test]
        public void PlayerRollsSevenAndNewHexHasMultiplePlayersIncludingRollingPlayer()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Passing in an id of a player that is not on the selected robber hex when choosing the resource 
        /// causes an error to be raised.
        /// </summary>
        [Test]
        public void PlayerRollsSevenAndSelectsInvalidPlayer()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void PlayerQuitsDuringFirstRoundOfGameSetup()
        {
            var expectedGameBoardSetup = new GameBoardSetup(new GameBoard(BoardSizes.Standard));
            var playerOrder = new[] { Adam, Babara, Charlie, Dana };
            ScenarioRunner.CreateScenarioRunner(new[] { MethodBase.GetCurrentMethod().Name })
                .WithPlayer(Adam).WithPlayer(Babara).WithPlayer(Charlie).WithPlayer(Dana)
                .WithTurnOrder(playerOrder)
                .WhenPlayer(Adam)
                    .ReceivesPlaceInfrastructureSetupEvent().ThenQuitGame()
                .WhenPlayer(Babara)
                    .ReceivesPlayerQuitEvent(Adam).ThenDoNothing()
                .WhenPlayer(Charlie)
                    .ReceivesPlayerQuitEvent(Adam).ThenDoNothing()
                .WhenPlayer(Dana)
                    .ReceivesPlayerQuitEvent(Adam).ThenDoNothing()
                .Run();
        }

        [Test]
        public void PlayerQuitsDuringSecondRoundOfGameSetup()
        {
            var expectedGameBoardSetup = new GameBoardSetup(new GameBoard(BoardSizes.Standard));
            var playerOrder = new[] { Adam, Babara, Charlie, Dana };
            ScenarioRunner.CreateScenarioRunner(new[] { MethodBase.GetCurrentMethod().Name })
                .WithPlayer(Adam).WithPlayer(Babara).WithPlayer(Charlie).WithPlayer(Dana)
                .WithTurnOrder(playerOrder)
                .WhenPlayer(Adam)
                    .ReceivesPlaceInfrastructureSetupEvent().ThenPlaceStartingInfrastructure(Adam_FirstSettlementLocation, Adam_FirstRoadEndLocation)
                    .ReceivesPlaceInfrastructureSetupEvent().ThenQuitGame()
                .WhenPlayer(Babara)
                    .ReceivesPlaceInfrastructureSetupEvent().ThenPlaceStartingInfrastructure(Babara_FirstSettlementLocation, Babara_FirstRoadEndLocation)
                    .ReceivesPlaceInfrastructureSetupEvent().ThenPlaceStartingInfrastructure(Babara_SecondSettlementLocation, Babara_SecondRoadEndLocation)
                    .ReceivesPlayerQuitEvent(Adam).ThenDoNothing()
                .WhenPlayer(Charlie)
                    .ReceivesPlaceInfrastructureSetupEvent().ThenPlaceStartingInfrastructure(Charlie_FirstSettlementLocation, Charlie_FirstRoadEndLocation)
                    .ReceivesPlaceInfrastructureSetupEvent().ThenPlaceStartingInfrastructure(Charlie_SecondSettlementLocation, Charlie_SecondRoadEndLocation)
                    .ReceivesPlayerQuitEvent(Adam).ThenDoNothing()
                .WhenPlayer(Dana)
                    .ReceivesPlaceInfrastructureSetupEvent().ThenPlaceStartingInfrastructure(Dana_FirstSettlementLocation, Dana_FirstRoadEndLocation)
                    .ReceivesPlaceInfrastructureSetupEvent().ThenPlaceStartingInfrastructure(Dana_SecondSettlementLocation, Dana_SecondRoadEndLocation)
                    .ReceivesPlayerQuitEvent(Adam).ThenDoNothing()
                .VerifyPlayer(Adam)
                    .DidNotReceiveEventOfType<ConfirmGameStartEvent>()
                .Run();
        }

        [Test]
        public void PlayerSendsIncorrectCommandDuringGameStartConfirmation()
        {
            var expectedGameBoardSetup = new GameBoardSetup(new GameBoard(BoardSizes.Standard));
            var playerOrder = new[] { Adam, Babara, Charlie, Dana };
            ScenarioRunner.CreateScenarioRunner(new[] { MethodBase.GetCurrentMethod().Name })
                .WithPlayer(Adam).WithPlayer(Babara).WithPlayer(Charlie).WithPlayer(Dana)
                .WithTurnOrder(playerOrder)
                .WhenPlayer(Adam)
                    .ReceivesPlaceInfrastructureSetupEvent()
                    .ThenPlaceStartingInfrastructure(Adam_FirstSettlementLocation, Adam_FirstRoadEndLocation)
                    .ReceivesPlaceInfrastructureSetupEvent()
                    .ThenPlaceStartingInfrastructure(Adam_SecondSettlementLocation, Adam_SecondRoadEndLocation)
                    .ReceivesConfirmGameStartEvent()
                    .ThenEndTurn()
                    .ReceivesGameErrorEvent("999", "Received action type EndOfTurnAction. Expected one of ConfirmGameStartAction, QuitGameAction")
                .WhenPlayer(Babara)
                    .ReceivesPlaceInfrastructureSetupEvent()
                    .ThenPlaceStartingInfrastructure(Babara_FirstSettlementLocation, Babara_FirstRoadEndLocation)
                    .ReceivesPlaceInfrastructureSetupEvent()
                    .ThenPlaceStartingInfrastructure(Babara_SecondSettlementLocation, Babara_SecondRoadEndLocation)
                .WhenPlayer(Charlie)
                    .ReceivesPlaceInfrastructureSetupEvent()
                    .ThenPlaceStartingInfrastructure(Charlie_FirstSettlementLocation, Charlie_FirstRoadEndLocation)
                    .ReceivesPlaceInfrastructureSetupEvent()
                    .ThenPlaceStartingInfrastructure(Charlie_SecondSettlementLocation, Charlie_SecondRoadEndLocation)
                .WhenPlayer(Dana)
                    .ReceivesPlaceInfrastructureSetupEvent()
                    .ThenPlaceStartingInfrastructure(Dana_FirstSettlementLocation, Dana_FirstRoadEndLocation)
                    .ReceivesPlaceInfrastructureSetupEvent()
                    .ThenPlaceStartingInfrastructure(Dana_SecondSettlementLocation, Dana_SecondRoadEndLocation)
                .Run();
        }

        [Test]
        public void PlayerSendsIncorrectCommandDuringGameSetup()
        {
            var expectedGameBoardSetup = new GameBoardSetup(new GameBoard(BoardSizes.Standard));
            var playerOrder = new[] { Adam, Babara, Charlie, Dana };
            ScenarioRunner.CreateScenarioRunner(new[] { MethodBase.GetCurrentMethod().Name })
                .WithPlayer(Adam).WithPlayer(Babara).WithPlayer(Charlie).WithPlayer(Dana)
                .WithTurnOrder(playerOrder)
                .WhenPlayer(Adam)
                    .ReceivesPlaceInfrastructureSetupEvent().ThenEndTurn()
                    .ReceivesGameErrorEvent("999", "Received action type EndOfTurnAction. Expected one of PlaceSetupInfrastructureAction, QuitGameAction")
                .Run();
        }

        [Test]
        public void PlayerTradesOneResourceWithAnotherPlayer()
        {
            var adamResources = ResourceClutch.OneWool;
            var babaraResources = ResourceClutch.OneGrain;

            this.CompletePlayerInfrastructureSetup(new[] { MethodBase.GetCurrentMethod().Name })
                .WithNoResourceCollection()
                .WithInitialPlayerSetupFor(Adam, Resources(adamResources))
                .WithInitialPlayerSetupFor(Babara, Resources(babaraResources))
                .WhenPlayer(Adam)
                    .ReceivesStartTurnEvent(3, 3)
                    .ThenEndTurn()
                .WhenPlayer(Babara)
                    .ReceivesStartTurnEvent(3, 3)
                    .ThenMakeDirectTradeOffer(ResourceClutch.OneWool)
                .WhenPlayer(Adam)
                    .ReceivesMakeDirectTradeOfferEvent(Babara, ResourceClutch.OneWool)
                    .ThenAnswerDirectTradeOffer(ResourceClutch.OneGrain)
                .WhenPlayer(Charlie)
                    .ReceivesMakeDirectTradeOfferEvent(Babara, ResourceClutch.OneWool).ThenDoNothing()
                .WhenPlayer(Dana)
                    .ReceivesMakeDirectTradeOfferEvent(Babara, ResourceClutch.OneWool).ThenDoNothing()
                .WhenPlayer(Babara)
                    .ReceivesAnswerDirectTradeOfferEvent(Adam, ResourceClutch.OneGrain)
                    .ThenAcceptTradeOffer(Adam)
                .WhenPlayer(Charlie)
                    .ReceivesAnswerDirectTradeOfferEvent(Adam, ResourceClutch.OneGrain).ThenDoNothing()
                .WhenPlayer(Dana)
                    .ReceivesAnswerDirectTradeOfferEvent(Adam, ResourceClutch.OneGrain).ThenDoNothing()
                .WhenPlayer(Adam)
                    .ReceivesAcceptDirectTradeEvent(Babara, ResourceClutch.OneWool, Adam, ResourceClutch.OneGrain)
                    .ThenVerifyPlayerState()
                        .Resources(ResourceClutch.OneGrain)
                        .End()
                .WhenPlayer(Babara)
                    .ReceivesAcceptDirectTradeEvent(Babara, ResourceClutch.OneWool, Adam, ResourceClutch.OneGrain)
                    .ThenVerifyPlayerState()
                        .Resources(ResourceClutch.OneWool)
                        .End()
                .WhenPlayer(Charlie)
                    .ReceivesAcceptDirectTradeEvent(Babara, ResourceClutch.OneWool, Adam, ResourceClutch.OneGrain)
                    .ThenDoNothing()
                .WhenPlayer(Dana)
                    .ReceivesAcceptDirectTradeEvent(Babara, ResourceClutch.OneWool, Adam, ResourceClutch.OneGrain)
                    .ThenDoNothing()
                .Run();
        }

        [Test]
        public void PlayerWithEightPointsGainsLargestArmyAndWins()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void PlayerWithEightPointsGainsLongestRoadAndWins()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void PlayerWithLargestArmyDoesNotRaiseEventWhenPlayingSubsequentKnight()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void PlayerWithLargestArmyDoesNotGetMoreVictoryPointsWhenPlayingSubsequentKnight()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void PlayerWithNinePointsGainsLargestArmyAndWins()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void PlayerWithNinePointsGainsLongestRoadAndWins()
        {
            throw new NotImplementedException();
        }

        private static CollectedResourcesBuilder CreateExpectedCollectedResources()
        {
            return new CollectedResourcesBuilder();
        }

        private ScenarioRunner CompletePlayerInfrastructureSetup(string[] args = null)
        {
            var infrastructureSetupBuilder = new InfrastructureSetupBuilder();
            infrastructureSetupBuilder.Add(Adam, Adam_FirstSettlementLocation, Adam_FirstRoadEndLocation)
                .Add(Babara, Babara_FirstSettlementLocation, Babara_FirstRoadEndLocation)
                .Add(Charlie, Charlie_FirstSettlementLocation, Charlie_FirstRoadEndLocation)
                .Add(Dana, Dana_FirstSettlementLocation, Dana_FirstRoadEndLocation)
                .Add(Dana, Dana_SecondSettlementLocation, Dana_SecondRoadEndLocation)
                .Add(Charlie, Charlie_SecondSettlementLocation, Charlie_SecondRoadEndLocation)
                .Add(Babara, Babara_SecondSettlementLocation, Babara_SecondRoadEndLocation)
                .Add(Adam, Adam_SecondSettlementLocation, Adam_SecondRoadEndLocation);
            
            return this.CompletePlayerInfrastructureSetup(infrastructureSetupBuilder.Build(), args);
        }

        private ScenarioRunner CompletePlayerInfrastructureSetup(InfrastructureSetup infrastructureSetup, string[] args = null)
        {
            var actionNotRecognisedError = new ScenarioGameErrorEvent(null, "999", null);
            var scenarioRunner = ScenarioRunner.CreateScenarioRunner(args)
                .WithPlayer(infrastructureSetup.PlayerOneName)
                .WithPlayer(infrastructureSetup.PlayerTwoName)
                .WithPlayer(infrastructureSetup.PlayerThreeName)
                .WithPlayer(infrastructureSetup.PlayerFourName)
                .WithTurnOrder(infrastructureSetup.PlayerOrder);

            foreach (var setupLocation in infrastructureSetup.SetupLocations)
            {
                scenarioRunner.WhenPlayer(setupLocation.PlayerName)
                    .ReceivesPlaceInfrastructureSetupEvent()
                    .ThenPlaceStartingInfrastructure(setupLocation.SettlementLocation, setupLocation.RoadEndLocation);
            }

            foreach (var playerName in infrastructureSetup.PlayerOrder)
            {
                scenarioRunner
                    .WhenPlayer(playerName)
                        .ReceivesConfirmGameStartEvent()
                        .ThenConfirmGameStart()
                    .VerifyPlayer(playerName)
                        .DidNotReceiveEvent(actionNotRecognisedError);
            }

            return scenarioRunner;
        }

        internal static IPlayerSetupAction Resources(ResourceClutch resources) => new ResourceSetup(resources);

        private static IPlayerSetupAction VictoryPoints(uint value) => new VictoryPointSetup(value);

        private static IPlayerSetupAction PlacedRoadSegments(int value) => new PlacedRoadSegmentSetup(value);

        private static IPlayerSetupAction PlacedSettlements(int value) => new PlacedSettlementsSetup(value);
    }
}
