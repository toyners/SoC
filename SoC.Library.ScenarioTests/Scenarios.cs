
namespace SoC.Library.ScenarioTests
{
    using System;
    using Jabberwocky.SoC.Library;
    using Jabberwocky.SoC.Library.GameBoards;
    using Jabberwocky.SoC.Library.GameEvents;
    using NUnit.Framework;

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
        [Scenario]
        public void AllPlayersCollectResourcesAsPartOfGameSetup()
        {
            throw new NotImplementedException();
        }

        [Test]
        [Scenario]
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

            var fourTurnCollectedResources = CreateExpectedCollectedResources()
                .Add(Adam, Adam_FirstSettlementLocation, ResourceClutch.OneWool)
                .Add(Babara, Babara_SecondSettlementLocation, ResourceClutch.OneWool)
                .Build();

            this.CompletePlayerInfrastructureSetup(new[] { System.Reflection.MethodBase.GetCurrentMethod().Name })
                .WhenPlayer(Adam)
                    .ReceivesDiceRollEvent(4, 4).ThenDoNothing()
                    .ReceivesResourceCollectedEvent(firstTurnCollectedResources)
                    .ThenVerifyPlayerState().Resources(ResourceClutch.OneBrick).End()
                    .ThenEndTurn()
                .WhenPlayer(Babara)
                    .ReceivesResourceCollectedEvent(firstTurnCollectedResources)
                    .ThenVerifyPlayerState().Resources(ResourceClutch.OneGrain).End()
                .WhenPlayer(Charlie)
                    .ReceivesResourceCollectedEvent(firstTurnCollectedResources)
                    .ThenVerifyPlayerState().Resources(ResourceClutch.Zero).End()
                .WhenPlayer(Dana)
                    .ReceivesResourceCollectedEvent(firstTurnCollectedResources)
                    .ThenVerifyPlayerState().Resources(ResourceClutch.Zero).End()

                .WhenPlayer(Babara)
                    .ReceivesDiceRollEvent(3, 3).ThenDoNothing()
                    .ReceivesResourceCollectedEvent(secondTurnCollectedResources)
                    .ThenVerifyPlayerState().Resources(ResourceClutch.OneGrain + ResourceClutch.OneOre).End()
                    .ThenEndTurn()
                .WhenPlayer(Adam)
                    .ReceivesResourceCollectedEvent(secondTurnCollectedResources)
                    .ThenVerifyPlayerState().Resources(ResourceClutch.OneBrick).End()
                .WhenPlayer(Charlie)
                    .ReceivesResourceCollectedEvent(secondTurnCollectedResources)
                    .ThenVerifyPlayerState().Resources(ResourceClutch.OneLumber * 2).End()
                .WhenPlayer(Dana)
                    .ReceivesResourceCollectedEvent(secondTurnCollectedResources)
                    .ThenVerifyPlayerState().Resources(ResourceClutch.OneOre).End()

                .WhenPlayer(Charlie)
                    .ReceivesDiceRollEvent(1, 2).ThenDoNothing()
                    .ReceivesResourceCollectedEvent(thirdTurnCollectedResources)
                    .ThenVerifyPlayerState().Resources((ResourceClutch.OneLumber * 2) + ResourceClutch.OneOre).End()
                    .ThenEndTurn()
                .WhenPlayer(Adam)
                    .ReceivesResourceCollectedEvent(thirdTurnCollectedResources)
                    .ThenVerifyPlayerState().Resources(ResourceClutch.OneBrick).End()
                .WhenPlayer(Babara)
                    .ReceivesResourceCollectedEvent(thirdTurnCollectedResources)
                    .ThenVerifyPlayerState().Resources(ResourceClutch.OneGrain + ResourceClutch.OneOre).End()
                .WhenPlayer(Dana)
                    .ReceivesResourceCollectedEvent(thirdTurnCollectedResources)
                    .ThenVerifyPlayerState().Resources(ResourceClutch.OneOre).End()

                .WhenPlayer(Dana)
                    .ReceivesDiceRollEvent(6, 4).ThenDoNothing()
                    .ReceivesResourceCollectedEvent(fourTurnCollectedResources)
                    .ThenVerifyPlayerState().Resources(ResourceClutch.OneOre).End()
                    .ThenEndTurn()
                .WhenPlayer(Adam)
                    .ReceivesResourceCollectedEvent(fourTurnCollectedResources)
                    .ThenVerifyPlayerState().Resources(ResourceClutch.OneBrick + ResourceClutch.OneWool).End()
                .WhenPlayer(Babara)
                    .ReceivesResourceCollectedEvent(fourTurnCollectedResources)
                    .ThenVerifyPlayerState().Resources(ResourceClutch.OneGrain + ResourceClutch.OneOre + ResourceClutch.OneWool).End()
                .WhenPlayer(Charlie)
                    .ReceivesResourceCollectedEvent(fourTurnCollectedResources)
                    .ThenVerifyPlayerState().Resources((ResourceClutch.OneLumber * 2) + ResourceClutch.OneOre).End()

                .WhenPlayer(Adam)
                    .ReceivesDiceRollEvent(1, 1)
                    .ThenQuitGame()
                .WhenPlayer(Babara)
                    .ReceivesDiceRollEvent(1, 1)
                    .ThenQuitGame()
                .WhenPlayer(Charlie)
                    .ReceivesDiceRollEvent(1, 1)
                    .ThenQuitGame()
                .Run();
        }

        [Scenario]
        [Test]
        public void AllPlayersCompleteSetup()
        {
            var expectedGameBoardSetup = new GameBoardSetup(new GameBoard(BoardSizes.Standard));
            var playerOrder = new[] { Adam, Babara, Charlie, Dana };
            ScenarioRunner.CreateScenarioRunner(new[] { "AllPlayersCompleteSetup" })
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
        public void AllOtherPlayersQuit(string[] args)
        {
            this.CompletePlayerInfrastructureSetup(args)
                .WithNoResourceCollection()
                .WhenPlayer(Adam)
                    .ReceivesDiceRollEvent(3, 3).ThenEndTurn()
                    .ReceivesPlayerQuitEvent(Babara).ThenDoNothing()
                    .ReceivesPlayerQuitEvent(Charlie).ThenDoNothing()
                    .ReceivesPlayerQuitEvent(Dana).ThenDoNothing()
                    .ReceivesPlayerWonEvent(Adam, 2).ThenDoNothing()
                .WhenPlayer(Babara)
                    .ReceivesDiceRollEvent(3, 3).ThenQuitGame()
                .WhenPlayer(Charlie)
                    .ReceivesPlayerQuitEvent(Babara).ThenDoNothing()
                    .ReceivesDiceRollEvent(3, 3).ThenQuitGame()
                .WhenPlayer(Dana)
                    .ReceivesPlayerQuitEvent(Babara).ThenDoNothing()
                    .ReceivesPlayerQuitEvent(Charlie).ThenDoNothing()
                    .ReceivesDiceRollEvent(3, 3).ThenQuitGame()
                .VerifyPlayer(Babara)
                    .DidNotReceiveEvent<GameWinEvent>()
                    .DidNotReceivePlayerQuitEvent(Babara)
                    .DidNotReceivePlayerQuitEvent(Charlie)
                    .DidNotReceivePlayerQuitEvent(Dana)
                .VerifyPlayer(Charlie)
                    .DidNotReceiveEvent<GameWinEvent>()
                    .DidNotReceivePlayerQuitEvent(Charlie)
                    .DidNotReceivePlayerQuitEvent(Dana)
                .VerifyPlayer(Dana)
                    .DidNotReceiveEvent<GameWinEvent>()
                    .DidNotReceivePlayerQuitEvent(Dana)
                .Run();
        }

        [Scenario]
        public void PlayerPlacesCity(string[] args)
        {
            throw new NotImplementedException();
        }

        [Scenario]
        public void PlayerPlacesCityAndWins(string[] args)
        {
            throw new NotImplementedException();
        }

        [Scenario]
        public void PlayerPlacesSettlement(string[] args)
        {
            this.CompletePlayerInfrastructureSetup(args)
                .WithNoResourceCollection()
                .WithInitialPlayerSetupFor(
                    Adam,
                    Resources(ResourceClutch.RoadSegment + ResourceClutch.Settlement))
                .WhenPlayer(Adam)
                    .ReceivesDiceRollEvent(3, 3)
                    .ThenPlaceRoadSegment(4, 3)
                    .ReceivesRoadSegmentPlacementEvent(4, 3)
                    .ThenPlaceSettlement(3)
                    .ReceivesSettlementPlacementEvent(3)
                    .ThenVerifyPlayerState()
                        .Resources(ResourceClutch.Zero)
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

        [Scenario]
        public void PlayerPlacesSettlementAndWins(string[] args)
        {
            this.CompletePlayerInfrastructureSetup(args)
                .WithNoResourceCollection()
                .WithInitialPlayerSetupFor(
                    Adam, 
                    Resources(ResourceClutch.RoadSegment + ResourceClutch.Settlement),
                    VictoryPoints(7)) // Account for placing infrastructure
                .WhenPlayer(Adam)
                    .ReceivesDiceRollEvent(3, 3)
                    .ThenPlaceRoadSegment(4, 3)
                    .ReceivesRoadSegmentPlacementEvent(4, 3)
                    .ThenPlaceSettlement(3)
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

        [Scenario]
        public void PlayerTriesToPlaceSettlementOnLocationOccupiedByPlayer(string[] args)
        {
            throw new NotImplementedException();
        }

        [Scenario]
        public void PlayerTriesToPlaceSettlementOnLocationOccupiedByOtherPlayer(string[] args)
        {
            this.CompletePlayerInfrastructureSetup(args)
                .WithNoResourceCollection()
                .WithInitialPlayerSetupFor(
                    Adam,
                    Resources((ResourceClutch.RoadSegment * 2) + ResourceClutch.Settlement))
                .WithInitialPlayerSetupFor(
                    Babara,
                    Resources(ResourceClutch.RoadSegment + ResourceClutch.Settlement))
                .WhenPlayer(Adam)
                    .ReceivesDiceRollEvent(3, 3).ThenPlaceRoadSegment(12, 13)
                    .ReceivesRoadSegmentPlacementEvent(12, 13).ThenPlaceRoadSegment(13, 14)
                    .ReceivesRoadSegmentPlacementEvent(13, 14).ThenPlaceSettlement(14)
                .WhenPlayer(Babara)
                    .ReceivesDiceRollEvent(3, 3).ThenPlaceRoadSegment(15, 14)
                    .ReceivesRoadSegmentPlacementEvent(Babara, 15, 14).ThenPlaceSettlement(14)
                // Receive error for placing settlement on occupied location
                .WhenPlayer(Charlie)
                .WhenPlayer(Dana)
                .Run();
        }

        [Scenario]
        public void PlayerTriesToPlaceSettlementOnUnconnectedLocation(string[] args)
        {
            throw new NotImplementedException();
        }

        [Scenario]
        public void PlayerTriesToPlaceSettlementWithNoSettlementsLeft(string[] args)
        {
            throw new NotImplementedException();
        }

        [Scenario]
        public void PlayerTriesToPlaceSettlementWithNotEnoughResources(string[] args)
        {
            throw new NotImplementedException();
        }

        [Scenario]
        public void PlayerTriesToPlaceRoadSegmentWithInvalidLocations(string[] args)
        {
            this.CompletePlayerInfrastructureSetup(args)
                .WithNoResourceCollection()
                .WithInitialPlayerSetupFor(Adam, Resources(ResourceClutch.RoadSegment), PlacedRoadSegments(Player.TotalRoadSegments))
                .WhenPlayer(Adam)
                    .ReceivesDiceRollEvent(3, 3)
                    .ThenPlaceRoadSegment(4, 0)
                    .ReceivesGameErrorEvent("", "").ThenDoNothing()
                .Run();
        }

        [Scenario]
        public void PlayerTriesToPlaceRoadSegmentWithNoRoadSegmentsLeft(string[] args)
        {
            this.CompletePlayerInfrastructureSetup(args)
                .WithNoResourceCollection()
                .WithInitialPlayerSetupFor(Adam, Resources(ResourceClutch.RoadSegment), PlacedRoadSegments(Player.TotalRoadSegments))
                .WhenPlayer(Adam)
                    .ReceivesDiceRollEvent(3, 3)
                    .ThenPlaceRoadSegment(4, 3)
                    .ReceivesGameErrorEvent("", "").ThenDoNothing()
                .Run();
        }

        [Scenario]
        public void PlayerTriesToPlaceRoadSegmentWithNotEnoughResources(string[] args)
        {
            this.CompletePlayerInfrastructureSetup(args)
                .WithNoResourceCollection()
                .WhenPlayer(Adam)
                    .ReceivesDiceRollEvent(3, 3)
                    .ThenPlaceRoadSegment(4, 3)
                    .ReceivesGameErrorEvent("", "").ThenDoNothing()
                .Run();
        }

        [Scenario]
        public void PlayerPlaysKnightCard(string[] args)
        {
            throw new NotImplementedException();
        }

        [Scenario]
        public void PlayerRollsSeven(string[] args)
        {
            throw new NotImplementedException();
        }

        [Scenario]
        public void PlayerRollsSevenAndAllPlayersWithMoreThanSevenResourcesLoseResources(string[] args)
        {
            throw new NotImplementedException();
        }

        [Scenario]
        public void PlayerRollsSevenAndSelectedHexHasNoPlayers(string[] args)
        {
            throw new NotImplementedException();
        }

        [Scenario]
        public void PlayerRollsSevenAndSelectedHexHasOnePlayer(string[] args)
        {
            throw new NotImplementedException();
        }

        [Scenario]
        public void PlayerRollsSevenAndGetsResourceFromSelectedPlayer(string[] args)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Passing in an id of a player that is not on the selected robber hex when choosing the resource 
        /// causes an error to be raised.
        /// </summary>
        [Scenario]
        public void PlayerRollsSevenAndSelectsInvalidPlayer(string[] args)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The robber hex set by the player has only player settlements so calling the CallingChooseResourceFromOpponent 
        /// method raises an error
        /// </summary>
        [Scenario]
        public void PlayerRollsSevenAndHasNoCompetitorsOnSelectedHex(string[] args)
        {
            throw new NotImplementedException();
        }

        [Scenario]
        public void PlayerQuitsDuringFirstRoundOfGameSetup(string[] args)
        {
            var expectedGameBoardSetup = new GameBoardSetup(new GameBoard(BoardSizes.Standard));
            var playerOrder = new[] { Adam, Babara, Charlie, Dana };
            ScenarioRunner.CreateScenarioRunner(args)
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

        [Scenario]
        public void PlayerQuitsDuringSecondRoundOfGameSetup(string[] args)
        {
            var expectedGameBoardSetup = new GameBoardSetup(new GameBoard(BoardSizes.Standard));
            var playerOrder = new[] { Adam, Babara, Charlie, Dana };
            ScenarioRunner.CreateScenarioRunner(args)
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
                    .DidNotReceiveEvent<ConfirmGameStartEvent>()
                .Run();
        }

        [Scenario]
        public void PlayerSendsIncorrectCommandDuringGameStartConfirmation(string[] args)
        {
            var expectedGameBoardSetup = new GameBoardSetup(new GameBoard(BoardSizes.Standard));
            var playerOrder = new[] { Adam, Babara, Charlie, Dana };
            ScenarioRunner.CreateScenarioRunner(args)
                .WithPlayer(Adam).WithPlayer(Babara).WithPlayer(Charlie).WithPlayer(Dana)
                .WithTurnOrder(playerOrder)
                .WhenPlayer(Adam)
                    .ReceivesPlaceInfrastructureSetupEvent()
                    .ThenPlaceStartingInfrastructure(Adam_FirstSettlementLocation, Adam_FirstRoadEndLocation)
                    .ReceivesPlaceInfrastructureSetupEvent()
                    .ThenPlaceStartingInfrastructure(Adam_SecondSettlementLocation, Adam_SecondRoadEndLocation)
                    .ReceivesConfirmGameStartEvent()
                    .ThenEndTurn()
                    .ReceivesGameErrorEvent("302", "Invalid action: Expected ConfirmGameStartAction or QuitGameAction")
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

        [Scenario]
        public void PlayerSendsIncorrectCommandDuringGameSetup(string[] args)
        {
            var expectedGameBoardSetup = new GameBoardSetup(new GameBoard(BoardSizes.Standard));
            var playerOrder = new[] { Adam, Babara, Charlie, Dana };
            ScenarioRunner.CreateScenarioRunner(args)
                .WithPlayer(Adam).WithPlayer(Babara).WithPlayer(Charlie).WithPlayer(Dana)
                .WithTurnOrder(playerOrder)
                .WhenPlayer(Adam)
                    .ReceivesPlaceInfrastructureSetupEvent().ThenEndTurn()
                    .ReceivesGameErrorEvent("301", "Invalid action: Expected PlaceSetupInfrastructureAction or QuitGameAction")
                .Run();
        }

        [Scenario]
        public void PlayerTradesOneResourceWithAnotherPlayer(string[] args)
        {
            var adamResources = ResourceClutch.OneWool;
            var babaraResources = ResourceClutch.OneGrain;

            this.CompletePlayerInfrastructureSetup(args)
                .WithNoResourceCollection()
                .WithInitialPlayerSetupFor(Adam, Resources(adamResources))
                .WithInitialPlayerSetupFor(Babara, Resources(babaraResources))
                .WhenPlayer(Adam)
                    .ReceivesDiceRollEvent(3, 3)
                    .ThenEndTurn()
                .WhenPlayer(Babara)
                    .ReceivesDiceRollEvent(3, 3)
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

        [Scenario]
        public void PlayerWithEightPointsGainsLargestArmyAndWins(string[] args)
        {
            throw new NotImplementedException();
        }

        [Scenario]
        public void PlayerWithEightPointsGainsLongestRoadAndWins(string[] args)
        {
            throw new NotImplementedException();
        }

        [Scenario]
        public void PlayerWithLargestArmyDoesNotRaiseEventWhenPlayingSubsequentKnight(string[] args)
        {
            throw new NotImplementedException();
        }

        [Scenario]
        public void PlayerWithLargestArmyDoesNotGetMoreVictoryPointsWhenPlayingSubsequentKnight(string[] args)
        {
            throw new NotImplementedException();
        }

        [Scenario]
        public void PlayerWithNinePointsGainsLargestArmyAndWins(string[] args)
        {
            throw new NotImplementedException();
        }

        [Scenario]
        public void PlayerWithNinePointsGainsLongestRoadAndWins(string[] args)
        {
            throw new NotImplementedException();
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

        internal static IPlayerSetupAction Resources(ResourceClutch resources) => new ResourceSetup(resources);

        private static IPlayerSetupAction VictoryPoints(uint value) => new VictoryPointSetup(value);

        private static IPlayerSetupAction PlacedRoadSegments(uint value) => new PlacedRoadSegmentSetup(value);
    }
}
