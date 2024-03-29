﻿
namespace SoC.Library.ScenarioTests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Jabberwocky.SoC.Library;
    using Jabberwocky.SoC.Library.Enums;
    using Jabberwocky.SoC.Library.GameBoards;
    using Jabberwocky.SoC.Library.GameEvents;
    using NUnit.Framework;
    using SoC.Library.ScenarioTests.PlayerSetupActions;
    using SoC.Library.ScenarioTests.ScenarioEvents;
    using static SoC.Library.ScenarioTests.InfrastructureSetupBuilder;

    [TestFixture]
    [Category("A_Scenarios")]
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

        private string logDirectory;

        [SetUp]
        public void Setup()
        {
            this.logDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            if (!Directory.Exists(this.logDirectory))
            {
                Directory.CreateDirectory(this.logDirectory);

                var greenTickFilePath = $"{this.logDirectory}\\green_tick.png";
                using (var stream = new MemoryStream())
                {
                    Properties.Resources.GreenTick.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                    File.WriteAllBytes(greenTickFilePath, stream.ToArray());
                }

                var redCrossFilePath = $"{this.logDirectory}\\red_cross.png";
                using (var stream = new MemoryStream())
                {
                    Properties.Resources.RedCross.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                    File.WriteAllBytes(redCrossFilePath, stream.ToArray());
                }
            }
        }

        [Test]
        public void AllPlayersCollectResourcesAsPartOfGameSetup()
        {
            try
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

                ScenarioRunner.CreateScenarioRunner()
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
                            .EndPlayerVerification()
                    .WhenPlayer(Babara)
                        .ReceivesConfirmGameStartEvent().ThenConfirmGameStart()
                        .ReceivesAll()
                            .ReceivesResourceCollectedEvent(gameSetupCollectedResources)
                            .ReceivesStartTurnEvent(Adam, 1, 2)
                        .ReceivesAllEnd().ThenVerifyPlayerState()
                            .Resources(expectedBabaraResources)
                            .EndPlayerVerification()
                    .WhenPlayer(Charlie)
                        .ReceivesConfirmGameStartEvent().ThenConfirmGameStart()
                        .ReceivesAll()
                            .ReceivesResourceCollectedEvent(gameSetupCollectedResources)
                            .ReceivesStartTurnEvent(Adam, 1, 2)
                        .ReceivesAllEnd().ThenVerifyPlayerState()
                            .Resources(expectedCharlieResources)
                            .EndPlayerVerification()
                    .WhenPlayer(Dana)
                        .ReceivesConfirmGameStartEvent().ThenConfirmGameStart()
                        .ReceivesAll()
                            .ReceivesResourceCollectedEvent(gameSetupCollectedResources)
                            .ReceivesStartTurnEvent(Adam, 1, 2)
                        .ReceivesAllEnd().ThenVerifyPlayerState()
                            .Resources(expectedDanaResources)
                            .EndPlayerVerification()
                    .Run(this.logDirectory);
            }
            finally
            {
                this.AttachReports();
            }
        }

        [Test]
        public void AllPlayersCollectResourcesAsPartOfTurnStart()
        {
            try
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

                this.CompletePlayerInfrastructureSetup()
                    // Zero away the resources collected at start of the game
                    .WithInitialPlayerSetupFor(Adam, Resources(new ResourceClutch(-1, -1, 0, 0, -1)))
                    .WithInitialPlayerSetupFor(Babara, Resources(new ResourceClutch(0, -1, -1, 0, -1)))
                    .WithInitialPlayerSetupFor(Charlie, Resources(new ResourceClutch(0, 0, -1, -1, -1)))
                    .WithInitialPlayerSetupFor(Dana, Resources(new ResourceClutch(0, -1, -1, 0, -1)))
                    .WhenPlayer(Adam)
                        .ReceivesStartTurnWithResourcesCollectedEvent(4, 4, firstTurnCollectedResources)
                        .ThenVerifyPlayerState().Resources(ResourceClutch.OneBrick).EndPlayerVerification()
                        .ThenEndTurn()
                    .WhenPlayer(Babara)
                        .ReceivesStartTurnWithResourcesCollectedEvent(Adam, 4, 4, firstTurnCollectedResources)
                        .ThenVerifyPlayerState().Resources(ResourceClutch.OneGrain).EndPlayerVerification()
                    .WhenPlayer(Charlie)
                        .ReceivesStartTurnWithResourcesCollectedEvent(Adam, 4, 4, firstTurnCollectedResources)
                        .ThenVerifyPlayerState().Resources(ResourceClutch.Zero).EndPlayerVerification()
                    .WhenPlayer(Dana)
                        .ReceivesStartTurnWithResourcesCollectedEvent(Adam, 4, 4, firstTurnCollectedResources)
                        .ThenVerifyPlayerState().Resources(ResourceClutch.Zero).EndPlayerVerification()

                    .WhenPlayer(Babara)
                        .ReceivesStartTurnWithResourcesCollectedEvent(3, 3, secondTurnCollectedResources)
                        .ThenVerifyPlayerState().Resources(ResourceClutch.OneGrain + ResourceClutch.OneOre).EndPlayerVerification()
                        .ThenEndTurn()
                    .WhenPlayer(Adam)
                        .ReceivesStartTurnWithResourcesCollectedEvent(Babara, 3, 3, secondTurnCollectedResources)
                        .ThenVerifyPlayerState().Resources(ResourceClutch.OneBrick).EndPlayerVerification()
                    .WhenPlayer(Charlie)
                        .ReceivesStartTurnWithResourcesCollectedEvent(Babara, 3, 3, secondTurnCollectedResources)
                        .ThenVerifyPlayerState().Resources(ResourceClutch.OneLumber * 2).EndPlayerVerification()
                    .WhenPlayer(Dana)
                        .ReceivesStartTurnWithResourcesCollectedEvent(Babara, 3, 3, secondTurnCollectedResources)
                        .ThenVerifyPlayerState().Resources(ResourceClutch.OneOre).EndPlayerVerification()

                    .WhenPlayer(Charlie)
                        .ReceivesStartTurnWithResourcesCollectedEvent(1, 2, thirdTurnCollectedResources)
                        .ThenVerifyPlayerState().Resources((ResourceClutch.OneLumber * 2) + ResourceClutch.OneOre).EndPlayerVerification()
                        .ThenEndTurn()
                    .WhenPlayer(Adam)
                        .ReceivesStartTurnWithResourcesCollectedEvent(Charlie, 1, 2, thirdTurnCollectedResources)
                        .ThenVerifyPlayerState().Resources(ResourceClutch.OneBrick).EndPlayerVerification()
                    .WhenPlayer(Babara)
                        .ReceivesStartTurnWithResourcesCollectedEvent(Charlie, 1, 2, thirdTurnCollectedResources)
                        .ThenVerifyPlayerState().Resources(ResourceClutch.OneGrain + ResourceClutch.OneOre).EndPlayerVerification()
                    .WhenPlayer(Dana)
                        .ReceivesStartTurnWithResourcesCollectedEvent(Charlie, 1, 2, thirdTurnCollectedResources)
                        .ThenVerifyPlayerState().Resources(ResourceClutch.OneOre).EndPlayerVerification()

                    .WhenPlayer(Dana)
                        .ReceivesStartTurnWithResourcesCollectedEvent(6, 4, fourthTurnCollectedResources)
                        .ThenVerifyPlayerState().Resources(ResourceClutch.OneOre).EndPlayerVerification()
                        .ThenEndTurn()
                    .WhenPlayer(Adam)
                        .ReceivesStartTurnWithResourcesCollectedEvent(Dana, 6, 4, fourthTurnCollectedResources)
                        .ThenVerifyPlayerState().Resources(ResourceClutch.OneBrick + ResourceClutch.OneWool).EndPlayerVerification()
                    .WhenPlayer(Babara)
                        .ReceivesStartTurnWithResourcesCollectedEvent(Dana, 6, 4, fourthTurnCollectedResources)
                        .ThenVerifyPlayerState().Resources(ResourceClutch.OneGrain + ResourceClutch.OneOre + ResourceClutch.OneWool).EndPlayerVerification()
                    .WhenPlayer(Charlie)
                        .ReceivesStartTurnWithResourcesCollectedEvent(Dana, 6, 4, fourthTurnCollectedResources)
                        .ThenVerifyPlayerState().Resources((ResourceClutch.OneLumber * 2) + ResourceClutch.OneOre).EndPlayerVerification()

                    .WhenPlayer(Adam)
                        .ReceivesStartTurnEvent(1, 1)
                        .ThenQuitGame()
                    .WhenPlayer(Babara)
                        .ReceivesStartTurnEvent(1, 1)
                        .ThenQuitGame()
                    .WhenPlayer(Charlie)
                        .ReceivesStartTurnEvent(1, 1)
                        .ThenQuitGame()
                    .Run(this.logDirectory);
            }
            finally
            {
                this.AttachReports();
            }
        }

        [Test]
        public void AllPlayersCompleteSetup()
        {
            try
            {
                var expectedGameBoardSetup = new GameBoardSetup(new GameBoard(BoardSizes.Standard));
                var playerOrder = new[] { Adam, Babara, Charlie, Dana };
                ScenarioRunner.CreateScenarioRunner()
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
                    .Run(this.logDirectory);
            }
            finally
            {
                this.AttachReports();
            }
        }

        [Test]
        public void AllOtherPlayersQuit()
        {
            try
            {
                this.CompletePlayerInfrastructureSetup()
                    .WithCustomResourceCollection()
                    .WhenPlayer(Adam)
                        .ReceivesStartTurnEvent(3, 3).ThenEndTurn()
                        .ReceivesPlayerQuitEvent(Babara).ThenDoNothing()
                        .ReceivesPlayerQuitEvent(Charlie).ThenDoNothing()
                        .ReceivesPlayerQuitEvent(Dana).ThenDoNothing()
                        .ReceivesPlayerWonEvent(2, Adam).ThenDoNothing()
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
                    .Run(this.logDirectory);
            }
            finally
            {
                this.AttachReports();
            }
        }

        [Test]
        public void PlayerBuysDevelopmentCard()
        {
            try
            {
                this.CompletePlayerInfrastructureSetup()
                    .WithCustomResourceCollection()
                    .WithInitialPlayerSetupFor(Adam, Resources(ResourceClutch.DevelopmentCard))
                    .WhenPlayer(Adam)
                        .ReceivesStartTurnEvent(3, 3).ThenBuyDevelopmentCard()
                        .ReceivesDevelopmentCardBoughtEvent(DevelopmentCardTypes.RoadBuilding)
                        .ThenVerifyPlayerState()
                            .HeldCardsByType(DevelopmentCardTypes.RoadBuilding, 1)
                            .Resources(ResourceClutch.Zero)
                        .EndPlayerVerification()
                    .WhenPlayer(Babara)
                        .ReceivesDevelopmentCardBoughtEvent(Adam)
                    .WhenPlayer(Charlie)
                        .ReceivesDevelopmentCardBoughtEvent(Adam)
                    .WhenPlayer(Dana)
                        .ReceivesDevelopmentCardBoughtEvent(Adam)
                .Run(this.logDirectory);
            }
            finally
            {
                this.AttachReports();
            }
        }

        [Test]
        public void PlayerBuysDevelopmentCardWithoutEnoughResources()
        {
            try
            {
                this.CompletePlayerInfrastructureSetup()
                    .WithCustomResourceCollection()
                    .WhenPlayer(Adam)
                        .ReceivesStartTurnEvent(3, 3).ThenBuyDevelopmentCard()
                        .ReceivesGameErrorEvent(ErrorCodes.NotEnoughResourcesForDevelopmentCard, "Not enough resources for buying development card")
                    .Run(this.logDirectory);
            }
            finally
            {
                this.AttachReports();
            }
        }

        [Test]
        public void PlayerBuysDevelopmentCardAndUsesItOnSubsequentTurn()
        {
            try
            {
                this.CompletePlayerInfrastructureSetup()
                    .WithInitialPlayerSetupFor(Adam, Resources(ResourceClutch.DevelopmentCard))
                    .WhenPlayer(Adam)
                        .ReceivesStartTurnEvent(3, 3).ThenBuyDevelopmentCard()
                        .ReceivesDevelopmentCardBoughtEvent(DevelopmentCardTypes.Knight).ThenEndTurn()
                        .ReceivesStartTurnEvent(3, 3).ThenPlayKnightCard(4)
                        .ReceivesKnightCardPlayedEvent(4)
                    .WhenPlayer(Babara)
                        .ReceivesStartTurnEvent(3, 3).ThenEndTurn()
                    .WhenPlayer(Charlie)
                        .ReceivesStartTurnEvent(3, 3).ThenEndTurn()
                    .WhenPlayer(Dana)
                        .ReceivesStartTurnEvent(3, 3).ThenEndTurn()
                    .Run(this.logDirectory);
            }
            finally
            {
                this.AttachReports();
            }
        }

        [Test]
        public void PlayerPlacesCity()
        {
            try
            {
                this.CompletePlayerInfrastructureSetup()
                    .WithCustomResourceCollection()
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
                            .EndPlayerVerification()
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
                    .Run(this.logDirectory);
            }
            finally
            {
                this.AttachReports();
            }
        }

        [Test]
        public void PlayerPlacesCityAndWins()
        {
            try {
                this.CompletePlayerInfrastructureSetup()
                    .WithCustomResourceCollection()
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
                        .ReceivesPlayerWonEvent(10, Adam).ThenDoNothing()
                    .WhenPlayer(Charlie)
                        .ReceivesPlayerWonEvent(10, Adam).ThenDoNothing()
                    .WhenPlayer(Dana)
                        .ReceivesPlayerWonEvent(10, Adam).ThenDoNothing()
                    .WhenPlayer(Adam)
                        .ThenVerifyPlayerState()
                            .Resources(ResourceClutch.Zero)
                            .VictoryPoints(10)
                            .Cities(Player.TotalCities - 1)
                            .EndPlayerVerification()
                    .Run(this.logDirectory);
            }
            finally
            {
                this.AttachReports();
            }
        }

        [Test]
        public void PlayerTriesToPlaceCityOnLocationOccupiedByPlayer()
        {
            try
            {
                this.CompletePlayerInfrastructureSetup()
                    .WithCustomResourceCollection()
                    .WithInitialPlayerSetupFor(
                        Adam,
                        Resources(ResourceClutch.City * 2))
                    .WhenPlayer(Adam)
                        .ReceivesStartTurnEvent(3, 3).ThenPlaceCity(Adam_FirstSettlementLocation)
                        .ReceivesCityPlacementEvent(Adam_FirstSettlementLocation).ThenPlaceCity(Adam_FirstSettlementLocation)
                        .ReceivesGameErrorEvent(ErrorCodes.LocationAlreadyOccupiedByPlayer, $"Location ({Adam_FirstSettlementLocation}) already occupied by you").ThenDoNothing()
                    .VerifyPlayer(Babara)
                        .DidNotReceiveEventOfType<GameErrorEvent>()
                    .VerifyPlayer(Charlie)
                        .DidNotReceiveEventOfType<GameErrorEvent>()
                    .VerifyPlayer(Dana)
                        .DidNotReceiveEventOfType<GameErrorEvent>()
                    .Run(this.logDirectory);
            }
            finally
            {
                this.AttachReports();
            }
        }

        [Test]
        public void PlayerTriesToPlaceCityOnInvalidLocation()
        {
            try
            {
                this.CompletePlayerInfrastructureSetup()
                    .WithCustomResourceCollection()
                    .WithInitialPlayerSetupFor(
                        Adam,
                        Resources(ResourceClutch.City))
                    .WhenPlayer(Adam)
                        .ReceivesStartTurnEvent(3, 3).ThenPlaceCity(100)
                        .ReceivesGameErrorEvent(ErrorCodes.LocationIsInvalid, $"Location (100) is invalid").ThenDoNothing()
                    .VerifyPlayer(Babara)
                        .DidNotReceiveEventOfType<GameErrorEvent>()
                    .VerifyPlayer(Charlie)
                        .DidNotReceiveEventOfType<GameErrorEvent>()
                    .VerifyPlayer(Dana)
                        .DidNotReceiveEventOfType<GameErrorEvent>()
                    .Run(this.logDirectory);
            }
            finally
            {
                this.AttachReports();
            }
        }

        [Test]
        public void PlayerTriesToPlaceCityOnLocationOccupiedByOtherPlayer()
        {
            try
            {
                this.CompletePlayerInfrastructureSetup()
                    .WithCustomResourceCollection()
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
                        .ReceivesGameErrorEvent(ErrorCodes.LocationAlreadyOccupiedByPlayer, "Location (14) already occupied by Adam").ThenDoNothing()
                    .VerifyPlayer(Adam)
                        .DidNotReceiveEventOfType<GameErrorEvent>()
                    .VerifyPlayer(Babara)
                        .DidNotReceiveEventOfType<GameErrorEvent>()
                    .VerifyPlayer(Dana)
                        .DidNotReceiveEventOfType<GameErrorEvent>()
                    .Run(this.logDirectory);
            }
            finally
            {
                this.AttachReports();
            }
        }

        [Test]
        public void PlayerTriesToPlaceCityOnLocationWithoutSettlement()
        {
            try
            {
                this.CompletePlayerInfrastructureSetup()
                    .WithCustomResourceCollection()
                    .WithInitialPlayerSetupFor(
                        Adam,
                        Resources(ResourceClutch.RoadSegment + ResourceClutch.City))
                    .WhenPlayer(Adam)
                        .ReceivesStartTurnEvent(3, 3).ThenPlaceRoadSegment(3, 4)
                        .ReceivesRoadSegmentPlacementEvent(3, 4).ThenPlaceCity(3)
                        .ReceivesGameErrorEvent(ErrorCodes.LocationDoesNotHaveSettlement, "Location (3) not an settlement").ThenDoNothing()
                    .VerifyPlayer(Babara)
                        .DidNotReceiveEventOfType<GameErrorEvent>()
                    .VerifyPlayer(Charlie)
                        .DidNotReceiveEventOfType<GameErrorEvent>()
                    .VerifyPlayer(Dana)
                        .DidNotReceiveEventOfType<GameErrorEvent>()
                    .Run(this.logDirectory);
            }
            finally
            {
                this.AttachReports();
            }
        }

        [Test]
        public void PlayerTriesToPlaceCityWithoutEnoughResources()
        {
            try
            {
                this.CompletePlayerInfrastructureSetup()
                    .WithCustomResourceCollection()
                    .WhenPlayer(Adam)
                        .ReceivesStartTurnEvent(3, 3).ThenPlaceCity(Adam_FirstSettlementLocation)
                        .ReceivesGameErrorEvent(ErrorCodes.NotEnoughResourcesForCity, "Not enough resources for placing city").ThenDoNothing()
                    .VerifyPlayer(Babara)
                        .DidNotReceiveEventOfType<GameErrorEvent>()
                    .VerifyPlayer(Charlie)
                        .DidNotReceiveEventOfType<GameErrorEvent>()
                    .VerifyPlayer(Dana)
                        .DidNotReceiveEventOfType<GameErrorEvent>()
                    .Run(this.logDirectory);
            }
            finally
            {
                this.AttachReports();
            }
        }

        [Test]
        public void PlayerPlacesSettlement()
        {
            try
            {
                this.CompletePlayerInfrastructureSetup()
                    .WithCustomResourceCollection()
                    .WithInitialPlayerSetupFor(
                        Adam,
                        Resources(ResourceClutch.RoadSegment + ResourceClutch.Settlement))
                    .WhenPlayer(Adam)
                        .ReceivesStartTurnEvent(3, 3).ThenPlaceRoadSegment(4, 3)
                        .ReceivesRoadSegmentPlacementEvent(4, 3).ThenPlaceSettlement(3)
                        .ReceivesSettlementPlacementEvent(3)
                        .ThenVerifyPlayerState()
                            .Resources(ResourceClutch.Zero)
                            .Settlements(Player.TotalSettlements - 3)
                            .VictoryPoints(3)
                            .EndPlayerVerification()
                    .WhenPlayer(Babara)
                        .ReceivesRoadSegmentPlacementEvent(Adam, 4, 3).ThenDoNothing()
                        .ReceivesSettlementPlacementEvent(Adam, 3).ThenDoNothing()
                    .WhenPlayer(Charlie)
                        .ReceivesRoadSegmentPlacementEvent(Adam, 4, 3).ThenDoNothing()
                        .ReceivesSettlementPlacementEvent(Adam, 3).ThenDoNothing()
                    .WhenPlayer(Dana)
                        .ReceivesRoadSegmentPlacementEvent(Adam, 4, 3).ThenDoNothing()
                        .ReceivesSettlementPlacementEvent(Adam, 3).ThenDoNothing()
                    .Run(this.logDirectory);
            }
            finally
            {
                this.AttachReports();
            }
        }

        [Test]
        public void PlayerPlacesSettlementAndWins()
        {
            try
            {
                this.CompletePlayerInfrastructureSetup()
                    .WithCustomResourceCollection()
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
                            .EndPlayerVerification()
                    .Run(this.logDirectory);
            }
            finally
            {
                this.AttachReports();
            }
        }

        [Test]
        public void PlayerTriesToPlaceSettlementOnInvalidLocation()
        {
            try
            {
                this.CompletePlayerInfrastructureSetup()
                    .WithCustomResourceCollection()
                    .WithInitialPlayerSetupFor(
                        Adam,
                        Resources(ResourceClutch.Settlement))
                    .WhenPlayer(Adam)
                        .ReceivesStartTurnEvent(3, 3).ThenPlaceSettlement(100)
                        .ReceivesGameErrorEvent(ErrorCodes.LocationIsInvalid, $"Location (100) is invalid").ThenDoNothing()
                    .VerifyPlayer(Babara)
                        .DidNotReceiveEventOfType<GameErrorEvent>()
                    .VerifyPlayer(Charlie)
                        .DidNotReceiveEventOfType<GameErrorEvent>()
                    .VerifyPlayer(Dana)
                        .DidNotReceiveEventOfType<GameErrorEvent>()
                    .Run(this.logDirectory);
            }
            finally
            {
                this.AttachReports();
            }
        }

        [Test]
        public void PlayerTriesToPlaceSettlementOnLocationOccupiedByPlayer()
        {
            try
            {
                this.CompletePlayerInfrastructureSetup()
                    .WithCustomResourceCollection()
                    .WithInitialPlayerSetupFor(
                        Adam,
                        Resources(ResourceClutch.RoadSegment + (ResourceClutch.Settlement * 2)))
                    .WhenPlayer(Adam)
                        .ReceivesStartTurnEvent(3, 3).ThenPlaceRoadSegment(4, 3)
                        .ReceivesRoadSegmentPlacementEvent(4, 3).ThenPlaceSettlement(3)
                        .ReceivesSettlementPlacementEvent(3).ThenPlaceSettlement(3)
                        .ReceivesGameErrorEvent(ErrorCodes.LocationAlreadyOccupiedByPlayer, "Location (3) already occupied by you").ThenDoNothing()
                    .VerifyPlayer(Babara)
                        .DidNotReceiveEventOfType<GameErrorEvent>()
                    .VerifyPlayer(Charlie)
                        .DidNotReceiveEventOfType<GameErrorEvent>()
                    .VerifyPlayer(Dana)
                        .DidNotReceiveEventOfType<GameErrorEvent>()
                    .Run(this.logDirectory);
            }
            finally
            {
                this.AttachReports();
            }
        }

        [Test]
        public void PlayerTriesToPlaceSettlementOnLocationOccupiedByOtherPlayer()
        {
            try
            {
                this.CompletePlayerInfrastructureSetup()
                    .WithCustomResourceCollection()
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
                        .ReceivesGameErrorEvent(ErrorCodes.LocationAlreadyOccupiedByPlayer, "Location (14) already occupied by Adam").ThenDoNothing()
                    .VerifyPlayer(Adam)
                        .DidNotReceiveEventOfType<GameErrorEvent>()
                    .VerifyPlayer(Babara)
                        .DidNotReceiveEventOfType<GameErrorEvent>()
                    .VerifyPlayer(Dana)
                        .DidNotReceiveEventOfType<GameErrorEvent>()
                    .Run(this.logDirectory);
            }
            finally
            {
                this.AttachReports();
            }
        }

        [Test]
        public void PlayerTriesToPlaceSettlementOnUnconnectedLocation()
        {
            try
            {
                this.CompletePlayerInfrastructureSetup()
                    .WithCustomResourceCollection()
                    .WithInitialPlayerSetupFor(
                        Adam,
                        Resources(ResourceClutch.Settlement))
                    .WhenPlayer(Adam)
                        .ReceivesStartTurnEvent(3, 3).ThenPlaceSettlement(3)
                        .ReceivesGameErrorEvent(ErrorCodes.LocationNotConnectedToRoadSystem, "Location (3) is not connected to your road system").ThenDoNothing()
                    .VerifyPlayer(Babara)
                        .DidNotReceiveEventOfType<GameErrorEvent>()
                    .VerifyPlayer(Charlie)
                        .DidNotReceiveEventOfType<GameErrorEvent>()
                    .VerifyPlayer(Dana)
                        .DidNotReceiveEventOfType<GameErrorEvent>()
                    .Run(this.logDirectory);
            }
            finally
            {
                this.AttachReports();
            }
        }

        [Test]
        public void PlayerTriesToPlaceSettlementWithNoSettlementsLeft()
        {
            try
            {
                this.CompletePlayerInfrastructureSetup()
                    .WithCustomResourceCollection()
                    .WithInitialPlayerSetupFor(
                        Adam,
                        Resources(ResourceClutch.Settlement), PlacedSettlements(Player.TotalSettlements - 2))
                    .WhenPlayer(Adam)
                        .ReceivesStartTurnEvent(3, 3).ThenPlaceSettlement(3)
                        .ReceivesGameErrorEvent(ErrorCodes.NoSettlements, "No settlements to place").ThenDoNothing()
                    .VerifyPlayer(Babara)
                        .DidNotReceiveEventOfType<GameErrorEvent>()
                    .VerifyPlayer(Charlie)
                        .DidNotReceiveEventOfType<GameErrorEvent>()
                    .VerifyPlayer(Dana)
                        .DidNotReceiveEventOfType<GameErrorEvent>()
                    .Run(this.logDirectory);
            }
            finally
            {
                this.AttachReports();
            }
        }

        [Test]
        public void PlayerTriesToPlaceSettlementWithNotEnoughResources()
        {
            try
            {
                this.CompletePlayerInfrastructureSetup()
                    .WithCustomResourceCollection()
                    .WhenPlayer(Adam)
                        .ReceivesStartTurnEvent(3, 3).ThenPlaceSettlement(3)
                        .ReceivesGameErrorEvent(ErrorCodes.NotEnoughResourcesForSettlement, "Not enough resources for placing settlement").ThenDoNothing()
                    .VerifyPlayer(Babara)
                        .DidNotReceiveEventOfType<GameErrorEvent>()
                    .VerifyPlayer(Charlie)
                        .DidNotReceiveEventOfType<GameErrorEvent>()
                    .VerifyPlayer(Dana)
                        .DidNotReceiveEventOfType<GameErrorEvent>()
                    .Run(this.logDirectory);
            }
            finally
            {
                this.AttachReports();
            }
        }

        [Test]
        public void PlayerTriesToPlaceRoadSegmentWithNoRoadSegmentsLeft()
        {
            try
            {
                this.CompletePlayerInfrastructureSetup()
                    .WithCustomResourceCollection()
                    .WithInitialPlayerSetupFor(Adam, Resources(ResourceClutch.RoadSegment), PlacedRoadSegments(Player.TotalRoadSegments - 2))
                    .WhenPlayer(Adam)
                        .ReceivesStartTurnEvent(3, 3)
                        .ThenPlaceRoadSegment(4, 3)
                        .ReceivesGameErrorEvent(ErrorCodes.NotEnoughRoadSegments, "Not enough road segments to place").ThenDoNothing()
                    .VerifyPlayer(Babara)
                        .DidNotReceiveEventOfType<GameErrorEvent>()
                    .VerifyPlayer(Charlie)
                        .DidNotReceiveEventOfType<GameErrorEvent>()
                    .VerifyPlayer(Dana)
                        .DidNotReceiveEventOfType<GameErrorEvent>()
                    .Run(this.logDirectory);
            }
            finally
            {
                this.AttachReports();
            }
        }

        [Test]
        public void PlayerTriesToPlaceRoadSegmentWithoutEnoughResources()
        {
            try
            {
                this.CompletePlayerInfrastructureSetup()
                    .WithCustomResourceCollection()
                    .WhenPlayer(Adam)
                        .ReceivesStartTurnEvent(3, 3)
                        .ThenPlaceRoadSegment(4, 3)
                        .ReceivesGameErrorEvent(ErrorCodes.NotEnoughResourcesForRoadSegment, "Not enough resources for placing road segment").ThenDoNothing()
                    .VerifyPlayer(Babara)
                        .DidNotReceiveEventOfType<GameErrorEvent>()
                    .VerifyPlayer(Charlie)
                        .DidNotReceiveEventOfType<GameErrorEvent>()
                    .VerifyPlayer(Dana)
                        .DidNotReceiveEventOfType<GameErrorEvent>()
                    .Run(this.logDirectory);
            }
            finally
            {
                this.AttachReports();
            }
        }

        [Test]
        [TestCase(4u, 12u, ErrorCodes.LocationsInvalidForRoadSegment, "Cannot place road segment on existing road segment (4, 12)")]
        [TestCase(3u, 2u, ErrorCodes.LocationNotConnectedToRoadSystem, "Cannot place road segment because locations (3, 2) are not connected to existing road")]
        [TestCase(4u, 55u, ErrorCodes.LocationsInvalidForRoadSegment, "Locations (4, 55) invalid for placing road segment")]
        [TestCase(4u, 0u, ErrorCodes.LocationsNotDirectlyConnected, "Locations (4, 0) not directly connected when placing road segment")]
        public void PlayerTriesToPlaceRoadSegmentButLocationsAreInvalid(uint roadSegmentStartLocation, uint roadSegmentEndLocation, ErrorCodes errorCode, string errorMessage)
        {
            try
            {
                this.CompletePlayerInfrastructureSetup()
                    .WithCustomResourceCollection()
                    .WithInitialPlayerSetupFor(Adam, Resources(ResourceClutch.RoadSegment))
                    .WhenPlayer(Adam)
                        .ReceivesStartTurnEvent(3, 3).ThenPlaceRoadSegment(roadSegmentStartLocation, roadSegmentEndLocation)
                        .ReceivesGameErrorEvent(errorCode, errorMessage).ThenDoNothing()
                    .VerifyPlayer(Babara)
                        .DidNotReceiveEventOfType<GameErrorEvent>()
                    .VerifyPlayer(Charlie)
                        .DidNotReceiveEventOfType<GameErrorEvent>()
                    .VerifyPlayer(Dana)
                        .DidNotReceiveEventOfType<GameErrorEvent>()
                    .Run(this.logDirectory);
            }
            finally
            {
                this.AttachReports();
            }
        }

        [Test]
        public void PlayerPlaysKnightCardAndGetsResourceFromSelectedPlayer()
        {
            try
            {
                var robbingChoices = new Dictionary<string, int>()
                {
                    { Babara, 4 },
                    { Dana, 4 }
                };
                var robbedResource = ResourceClutch.OneLumber;
                this.CompletePlayerInfrastructureSetup()
                    .WithInitialPlayerSetupFor(Adam, KnightCard())
                    .WhenPlayer(Adam)
                        .ReceivesStartTurnEvent(3, 3)
                        .ThenVerifyPlayerState()
                            .HeldCardsByType(DevelopmentCardTypes.Knight, 1)
                            .PlayedKnightCards(0)
                        .EndPlayerVerification()
                        .ThenPlayKnightCard(8)
                        .ReceivesRobbingChoicesEvent(robbingChoices).ThenSelectRobbedPlayer(Babara)
                        .ReceivesResourcesRobbedEvent(Babara, ResourceTypes.Lumber)
                        .ThenVerifyPlayerState()
                            .Resources(ResourceClutch.OneBrick + ResourceClutch.OneGrain + robbedResource + ResourceClutch.OneWool)
                            .HeldCardsByType(DevelopmentCardTypes.Knight, 0)
                            .PlayedKnightCards(1)
                        .EndPlayerVerification()
                        .ThenPlaceRoadSegment(4, 3)
                    .WhenPlayer(Babara)
                        .ReceivesKnightCardPlayedEvent(Adam, 8).ThenDoNothing()
                        .ReceivesResourcesStolenEvent(robbedResource)
                        .ThenVerifyPlayerState()
                            .Resources(ResourceClutch.OneGrain + ResourceClutch.OneOre + ResourceClutch.OneWool)
                        .EndPlayerVerification()
                        .ReceivesRoadSegmentPlacementEvent(Adam, 4, 3)
                    .WhenPlayer(Charlie)
                        .ReceivesKnightCardPlayedEvent(Adam, 8).ThenDoNothing()
                        .ReceivesResourcesStolenEvent(Babara, robbedResource)
                        .ReceivesRoadSegmentPlacementEvent(Adam, 4, 3)
                    .WhenPlayer(Dana)
                        .ReceivesKnightCardPlayedEvent(Adam, 8).ThenDoNothing()
                        .ReceivesResourcesStolenEvent(Babara, robbedResource)
                        .ReceivesRoadSegmentPlacementEvent(Adam, 4, 3)
                    .Run(this.logDirectory);
            }
            finally
            {
                this.AttachReports();
            }
        }

        [Test]
        public void PlayersPlayKnightCardsAndLargestArmyEventFiresCorrectly()
        {
            try
            {
                this.CompletePlayerInfrastructureSetup()
                    .WithInitialPlayerSetupFor(Adam, KnightCard(5))
                    .WithInitialPlayerSetupFor(Babara, KnightCard(4))
                    .WhenPlayer(Adam)
                        .ReceivesStartTurnEvent(3, 3) // Turn 1
                            .ThenPlayKnightCard(4).ReceivesKnightCardPlayedEvent(4)
                        .ThenEndTurn()
                        .ReceivesStartTurnEvent(3, 3) // 2
                            .ThenPlayKnightCard(4).ReceivesKnightCardPlayedEvent(4)
                        .ThenEndTurn()
                        .ReceivesStartTurnEvent(3, 3) // 3
                            .ThenPlayKnightCard(4).ReceivesKnightCardPlayedEvent(4)
                            .ReceivesLargestArmyChangedEvent()
                            .ThenVerifyPlayerState()
                                .PlayedKnightCards(3)
                                .HeldCardsByType(DevelopmentCardTypes.Knight, 2)
                                .VictoryPoints(4)
                            .EndPlayerVerification()
                        .ThenEndTurn()
                        .ReceivesStartTurnEvent(3, 3).ThenEndTurn() // 4
                        .ReceivesLargestArmyChangedEvent(Babara, Adam)
                        .ThenVerifyPlayerState()
                            .VictoryPoints(2)
                        .EndPlayerVerification()
                        .ReceivesStartTurnEvent(3, 3) // 5
                            .ThenPlayKnightCard(0).ReceivesKnightCardPlayedEvent(0)
                        .ThenEndTurn()
                        .ReceivesStartTurnEvent(3, 3) // 6
                            .ThenPlayKnightCard(4)
                            .ReceivesKnightCardPlayedEvent(4)
                            .ReceivesLargestArmyChangedEvent(Adam, Babara)
                            .ThenVerifyPlayerState()
                                .PlayedKnightCards(5)
                                .HeldCardsByType(DevelopmentCardTypes.Knight, 0)
                                .VictoryPoints(4)
                            .EndPlayerVerification()
                        .ThenEndTurn()
                    .WhenPlayer(Babara)
                        .ReceivesStartTurnEvent(3, 3) // 1
                            .ThenPlayKnightCard(0).ReceivesKnightCardPlayedEvent(0)
                        .ThenEndTurn()
                        .ReceivesStartTurnEvent(3, 3) // 2
                            .ThenPlayKnightCard(0).ReceivesKnightCardPlayedEvent(0)
                        .ThenEndTurn()
                        .ReceivesLargestArmyChangedEvent(Adam)
                        .ReceivesStartTurnEvent(3, 3) // 3
                            .ThenPlayKnightCard(0).ReceivesKnightCardPlayedEvent(0)
                        .ThenEndTurn()
                        .ReceivesStartTurnEvent(3, 3) // 4
                            .ThenPlayKnightCard(4)
                            .ReceivesLargestArmyChangedEvent(Babara, Adam)
                            .ThenVerifyPlayerState()
                                .PlayedKnightCards(4)
                                .HeldCardsByType(DevelopmentCardTypes.Knight, 0)
                                .VictoryPoints(4)
                            .EndPlayerVerification()
                        .ThenEndTurn()
                        .ReceivesStartTurnEvent(3, 3).ThenEndTurn() // 5
                        .ReceivesLargestArmyChangedEvent(Adam, Babara)
                        .ThenVerifyPlayerState()
                            .VictoryPoints(2)
                        .EndPlayerVerification()
                        .ReceivesStartTurnEvent(3, 3).ThenQuitGame() // 6
                    .WhenPlayer(Charlie)
                        .ReceivesStartTurnEvent(3, 3).ThenEndTurn()
                        .ReceivesStartTurnEvent(3, 3).ThenEndTurn()
                        .ReceivesLargestArmyChangedEvent(Adam, null).ThenDoNothing()
                        .ReceivesStartTurnEvent(3, 3).ThenEndTurn()
                        .ReceivesLargestArmyChangedEvent(Babara, Adam).ThenDoNothing()
                        .ReceivesStartTurnEvent(3, 3).ThenEndTurn()
                        .ReceivesStartTurnEvent(3, 3).ThenEndTurn()
                        .ReceivesLargestArmyChangedEvent(Adam, Babara).ThenDoNothing()
                        .ReceivesStartTurnEvent(3, 3).ThenQuitGame()
                    .WhenPlayer(Dana)
                        .ReceivesStartTurnEvent(3, 3).ThenEndTurn()
                        .ReceivesStartTurnEvent(3, 3).ThenEndTurn()
                        .ReceivesLargestArmyChangedEvent(Adam, null).ThenDoNothing()
                        .ReceivesStartTurnEvent(3, 3).ThenEndTurn()
                        .ReceivesLargestArmyChangedEvent(Babara, Adam).ThenDoNothing()
                        .ReceivesStartTurnEvent(3, 3).ThenEndTurn()
                        .ReceivesStartTurnEvent(3, 3).ThenEndTurn()
                        .ReceivesLargestArmyChangedEvent(Adam, Babara).ThenDoNothing()
                        .ReceivesStartTurnEvent(3, 3).ThenQuitGame()
                    .Run(this.logDirectory);
            }
            finally
            {
                this.AttachReports();
            }
        }

        [Test]
        public void PlayerPlaysKnightCardThatIsNotOwned()
        {
            try
            {
                this.CompletePlayerInfrastructureSetup()
                    .WithCustomResourceCollection()
                    .WhenPlayer(Adam)
                        .ReceivesStartTurnEvent(3, 3).ThenPlayKnightCard(0)
                        .ReceivesGameErrorEvent(ErrorCodes.NoDevelopmentCardCanBePlayed, "No Knight card owned that can be played this turn")
                    .VerifyPlayer(Babara)
                        .DidNotReceiveEventOfType<KnightCardPlayedEvent>()
                        .DidNotReceiveEventOfType<GameErrorEvent>()
                    .VerifyPlayer(Charlie)
                        .DidNotReceiveEventOfType<KnightCardPlayedEvent>()
                        .DidNotReceiveEventOfType<GameErrorEvent>()
                    .VerifyPlayer(Dana)
                        .DidNotReceiveEventOfType<KnightCardPlayedEvent>()
                        .DidNotReceiveEventOfType<GameErrorEvent>()
                    .Run(this.logDirectory);
            }
            finally
            {
                this.AttachReports();
            }
        }

        [Test]
        public void PlayerPlaysRoadBuildingCardThatIsNotOwned()
        {
            try
            {
                this.CompletePlayerInfrastructureSetup()
                    .WithCustomResourceCollection()
                    .WhenPlayer(Adam)
                        .ReceivesStartTurnEvent(3, 3).ThenPlayRoadBuildingCard(4, 3, 3, 2)
                        .ReceivesGameErrorEvent(ErrorCodes.NoDevelopmentCardCanBePlayed, "No Road building card owned that can be played this turn")
                    .VerifyPlayer(Babara)
                        .DidNotReceiveEventOfType<RoadBuildingCardPlayedEvent>()
                        .DidNotReceiveEventOfType<GameErrorEvent>()
                    .VerifyPlayer(Charlie)
                        .DidNotReceiveEventOfType<RoadBuildingCardPlayedEvent>()
                        .DidNotReceiveEventOfType<GameErrorEvent>()
                    .VerifyPlayer(Dana)
                        .DidNotReceiveEventOfType<RoadBuildingCardPlayedEvent>()
                        .DidNotReceiveEventOfType<GameErrorEvent>()
                    .Run(this.logDirectory);
            }
            finally
            {
                this.AttachReports();
            }
        }

        [Test]
        [TestCase(1)]
        [TestCase(0)]
        public void PlayerPlaysRoadBuildingCardButDoesNotHaveEnoughRoadSegments(int remainingRoadSegments)
        {
            try
            {
                this.CompletePlayerInfrastructureSetup()
                    .WithCustomResourceCollection()
                    .WithInitialPlayerSetupFor(Adam, PlacedRoadSegments(Player.TotalRoadSegments - (remainingRoadSegments + 2)), RoadBuildingCard(1))
                    .WhenPlayer(Adam)
                        .ReceivesStartTurnEvent(3, 3).ThenPlayRoadBuildingCard(4, 3, 3, 2)
                        .ReceivesGameErrorEvent(ErrorCodes.NotEnoughRoadSegments, "Not enough road segments to place")
                    .VerifyPlayer(Babara)
                        .DidNotReceiveEventOfType<RoadBuildingCardPlayedEvent>()
                        .DidNotReceiveEventOfType<GameErrorEvent>()
                    .VerifyPlayer(Charlie)
                        .DidNotReceiveEventOfType<RoadBuildingCardPlayedEvent>()
                        .DidNotReceiveEventOfType<GameErrorEvent>()
                    .VerifyPlayer(Dana)
                        .DidNotReceiveEventOfType<RoadBuildingCardPlayedEvent>()
                        .DidNotReceiveEventOfType<GameErrorEvent>()
                    .Run(this.logDirectory);
            }
            finally
            {
                this.AttachReports();
            }
        }

        [Test]
        [TestCase(3u, 2u, 2u, 1u, ErrorCodes.LocationNotConnectedToRoadSystem, "Cannot place road segment because locations (3, 2) are not connected to existing road")]
        [TestCase(4u, 3u, 2u, 1u, ErrorCodes.LocationNotConnectedToRoadSystem, "Cannot place road segment because locations (2, 1) are not connected to existing road")]
        [TestCase(4u, 12u, 4u, 3u, ErrorCodes.LocationsInvalidForRoadSegment, "Cannot place road segment on existing road segment (4, 12)")]
        [TestCase(4u, 3u, 4u, 12u, ErrorCodes.LocationsInvalidForRoadSegment, "Cannot place road segment on existing road segment (4, 12)")]
        [TestCase(4u, 3u, 4u, 3u, ErrorCodes.LocationsInvalidForRoadSegment, "Cannot place road segment on existing road segment (4, 3)")]
        [TestCase(4u, 54u, 4u, 3u, ErrorCodes.LocationsInvalidForRoadSegment, "Locations (4, 54) invalid for placing road segment")]
        [TestCase(4u, 3u, 4u, 54u, ErrorCodes.LocationsInvalidForRoadSegment, "Locations (4, 54) invalid for placing road segment")]
        [TestCase(4u, 0u, 4u, 3u, ErrorCodes.LocationsNotDirectlyConnected, "Locations (4, 0) not directly connected when placing road segment")]
        [TestCase(4u, 3u, 4u, 0u, ErrorCodes.LocationsNotDirectlyConnected, "Locations (4, 0) not directly connected when placing road segment")]
        public void PlayerPlaysRoadBuildingCardButLocationsAreInvalid(uint firstRoadSegmentStartLocation, uint firstRoadSegmentEndLocation, uint secondRoadSegmentStartLocation, uint secondRoadSegmentEndLocation, ErrorCodes expectedCode, string expectedMessage)
        {
            try
            {
                this.CompletePlayerInfrastructureSetup()
                    .WithCustomResourceCollection()
                    .WithInitialPlayerSetupFor(Adam, RoadBuildingCard(1))
                    .WhenPlayer(Adam)
                        .ReceivesStartTurnEvent(3, 3).ThenPlayRoadBuildingCard(firstRoadSegmentStartLocation, firstRoadSegmentEndLocation, secondRoadSegmentStartLocation, secondRoadSegmentEndLocation)
                        .ReceivesGameErrorEvent(expectedCode, expectedMessage)
                    .VerifyPlayer(Babara)
                        .DidNotReceiveEventOfType<RoadBuildingCardPlayedEvent>()
                        .DidNotReceiveEventOfType<GameErrorEvent>()
                    .VerifyPlayer(Charlie)
                        .DidNotReceiveEventOfType<RoadBuildingCardPlayedEvent>()
                        .DidNotReceiveEventOfType<GameErrorEvent>()
                    .VerifyPlayer(Dana)
                        .DidNotReceiveEventOfType<RoadBuildingCardPlayedEvent>()
                        .DidNotReceiveEventOfType<GameErrorEvent>()
                    .Run(this.logDirectory);
            }
            finally
            {
                this.AttachReports();
            }
        }

        [Test]
        public void PlayerPlaysRoadBuildingCard()
        {
            try
            {
                this.CompletePlayerInfrastructureSetup()
                    .WithCustomResourceCollection()
                    .WithInitialPlayerSetupFor(Adam, RoadBuildingCard(1))
                    .WhenPlayer(Adam)
                        .ReceivesStartTurnEvent(3, 3).ThenPlayRoadBuildingCard(4, 3, 3, 2)
                        .ReceivesRoadBuildingCardPlayedEvent(4, 3, 3, 2)
                    .VerifyPlayer(Babara)
                        .ReceivesRoadBuildingCardPlayedEvent(4, 3, 3, 2, Adam)
                    .VerifyPlayer(Charlie)
                        .ReceivesRoadBuildingCardPlayedEvent(4, 3, 3, 2, Adam)
                    .VerifyPlayer(Dana)
                        .ReceivesRoadBuildingCardPlayedEvent(4, 3, 3, 2, Adam)
                    .Run(this.logDirectory);
            }
            finally
            {
                this.AttachReports();
            }
        }

        [Test]
        public void PlayerPlaysRoadBuildingCardAndWinsWithLongestRoad()
        {
            try
            {
                this.CompletePlayerInfrastructureSetup()
                    .WithCustomResourceCollection()
                    .WithInitialPlayerSetupFor(Adam, Resources(ResourceClutch.RoadSegment * 2), RoadBuildingCard(1),
                        VictoryPoints(6))
                    .WhenPlayer(Adam)
                        .ReceivesStartTurnEvent(3, 3).ThenPlaceRoadSegment(4, 3)
                        .ReceivesRoadSegmentPlacementEvent(4, 3).ThenPlaceRoadSegment(3, 2)
                        .ReceivesRoadSegmentPlacementEvent(3, 2).ThenPlayRoadBuildingCard(2, 1, 1, 0)
                        .ReceivesRoadBuildingCardPlayedEvent(2, 1, 1, 0)
                        .ReceivesLongestRoadChangedEvent(new uint[] { 12, 4, 3, 2, 1, 0 })
                        .ReceivesPlayerWonEvent(10)
                    .VerifyPlayer(Babara)
                        .ReceivesRoadBuildingCardPlayedEvent(2, 1, 1, 0, Adam)
                        .ReceivesLongestRoadChangedEvent(new uint[] { 12, 4, 3, 2, 1, 0 }, Adam)
                        .ReceivesPlayerWonEvent(10, Adam)
                    .VerifyPlayer(Charlie)
                        .ReceivesRoadBuildingCardPlayedEvent(2, 1, 1, 0, Adam)
                        .ReceivesLongestRoadChangedEvent(new uint[] { 12, 4, 3, 2, 1, 0 }, Adam)
                        .ReceivesPlayerWonEvent(10, Adam)
                    .VerifyPlayer(Dana)
                        .ReceivesRoadBuildingCardPlayedEvent(2, 1, 1, 0, Adam)
                        .ReceivesLongestRoadChangedEvent(new uint[] { 12, 4, 3, 2, 1, 0 }, Adam)
                        .ReceivesPlayerWonEvent(10, Adam)
                    .Run(this.logDirectory);
            }
            finally
            {
                this.AttachReports();
            }
        }

        [Test]
        public void PlayerPlaysRoadBuildingCardAndWinsWithLongestRoadAfterOtherPlayerHasLongestRoad()
        {
            try
            {
                this.CompletePlayerInfrastructureSetup()
                    .WithCustomResourceCollection()
                    .WithInitialPlayerSetupFor(Adam, Resources(ResourceClutch.RoadSegment * 3), RoadBuildingCard(1),
                        VictoryPoints(6))
                    .WithInitialPlayerSetupFor(Babara, Resources(ResourceClutch.RoadSegment * 4))
                    .WhenPlayer(Adam)
                        .ReceivesStartTurnEvent(3, 3).ThenEndTurn()
                        .ReceivesRoadSegmentPlacementEvent(17, 7, Babara)
                        .ReceivesRoadSegmentPlacementEvent(7, 8, Babara)
                        .ReceivesRoadSegmentPlacementEvent(8, 9, Babara)
                        .ReceivesRoadSegmentPlacementEvent(9, 19, Babara)
                        .ReceivesLongestRoadChangedEvent(new uint[] { 18, 17, 7, 8, 9, 19 }, Babara)
                        .ReceivesStartTurnEvent(3, 3).ThenPlaceRoadSegment(4, 3)
                        .ReceivesRoadSegmentPlacementEvent(4, 3).ThenPlaceRoadSegment(3, 2)
                        .ReceivesRoadSegmentPlacementEvent(3, 2).ThenPlaceRoadSegment(2, 1)
                        .ReceivesRoadSegmentPlacementEvent(2, 1).ThenPlayRoadBuildingCard(1, 0, 0, 8)
                        .ReceivesRoadBuildingCardPlayedEvent(1, 0, 0, 8)
                        .ReceivesLongestRoadChangedEvent(new uint[] { 12, 4, 3, 2, 1, 0, 8 }, Adam, Babara)
                        .ReceivesPlayerWonEvent(10, Adam)
                    .VerifyPlayer(Babara)
                        .ReceivesStartTurnEvent(3, 3).ThenPlaceRoadSegment(17, 7)
                        .ReceivesRoadSegmentPlacementEvent(17, 7, Babara).ThenPlaceRoadSegment(7, 8)
                        .ReceivesRoadSegmentPlacementEvent(7, 8, Babara).ThenPlaceRoadSegment(8, 9)
                        .ReceivesRoadSegmentPlacementEvent(8, 9, Babara).ThenPlaceRoadSegment(9, 19)
                        .ReceivesRoadSegmentPlacementEvent(9, 19, Babara)
                        .ReceivesLongestRoadChangedEvent(new uint[] { 18, 17, 7, 8, 9, 19 }, Babara)
                        .ThenEndTurn()
                        .ReceivesRoadBuildingCardPlayedEvent(1, 0, 0, 8, Adam)
                        .ReceivesLongestRoadChangedEvent(new uint[] { 12, 4, 3, 2, 1, 0, 8 }, Adam, Babara)
                        .ReceivesPlayerWonEvent(10, Adam)
                    .VerifyPlayer(Charlie)
                        .ReceivesRoadSegmentPlacementEvent(17, 7, Babara)
                        .ReceivesRoadSegmentPlacementEvent(7, 8, Babara)
                        .ReceivesRoadSegmentPlacementEvent(8, 9, Babara)
                        .ReceivesRoadSegmentPlacementEvent(9, 19, Babara)
                        .ReceivesLongestRoadChangedEvent(new uint[] { 18, 17, 7, 8, 9, 19 }, Babara)
                        .ReceivesStartTurnEvent(3, 3).ThenEndTurn()
                        .ReceivesRoadBuildingCardPlayedEvent(1, 0, 0, 8, Adam)
                        .ReceivesLongestRoadChangedEvent(new uint[] { 12, 4, 3, 2, 1, 0, 8 }, Adam, Babara)
                        .ReceivesPlayerWonEvent(10, Adam)
                    .VerifyPlayer(Dana)
                        .ReceivesRoadSegmentPlacementEvent(17, 7, Babara)
                        .ReceivesRoadSegmentPlacementEvent(7, 8, Babara)
                        .ReceivesRoadSegmentPlacementEvent(8, 9, Babara)
                        .ReceivesRoadSegmentPlacementEvent(9, 19, Babara)
                        .ReceivesLongestRoadChangedEvent(new uint[] { 18, 17, 7, 8, 9, 19 }, Babara)
                        .ReceivesStartTurnEvent(3, 3).ThenEndTurn()
                        .ReceivesRoadBuildingCardPlayedEvent(1, 0, 0, 8, Adam)
                        .ReceivesLongestRoadChangedEvent(new uint[] { 12, 4, 3, 2, 1, 0, 8 }, Adam, Babara)
                        .ReceivesPlayerWonEvent(10, Adam)
                    .Run(this.logDirectory);
            }
            finally
            {
                this.AttachReports();
            }
        }

        [Test]
        public void PlayerPlaysMonopolyCard()
        {
            var expectedResources = new Dictionary<string, ResourceClutch>();
            expectedResources.Add(Babara, ResourceClutch.OneLumber);
            expectedResources.Add(Charlie, ResourceClutch.OneLumber);
            expectedResources.Add(Dana, ResourceClutch.OneLumber);

            try
            {
                this.CompletePlayerInfrastructureSetup()
                    .WithCustomResourceCollection(ScenarioGameBoard.ResourceCollectionTypes.SetupOnly)
                    .WithInitialPlayerSetupFor(Adam, MonopolyCard())
                    .WhenPlayer(Adam)
                        .ReceivesStartTurnEvent(3, 3).ThenPlayMonopolyCard(ResourceTypes.Lumber)
                        .ReceivesMonopolyCardPlayedEvent(expectedResources)
                        .ThenVerifyPlayerState()
                            .Resources(ResourceClutch.OneBrick + ResourceClutch.OneGrain + ResourceClutch.OneWool + (ResourceClutch.OneLumber * 3))
                        .EndPlayerVerification()
                    .VerifyPlayer(Babara)
                        .ReceivesMonopolyCardPlayedEvent(expectedResources, Adam)
                        .ThenVerifyPlayerState()
                            .Resources(ResourceClutch.OneGrain + ResourceClutch.OneWool)
                        .EndPlayerVerification()
                    .VerifyPlayer(Charlie)
                        .ReceivesMonopolyCardPlayedEvent(expectedResources, Adam)
                        .ThenVerifyPlayerState()
                            .Resources(ResourceClutch.OneOre + ResourceClutch.OneWool)
                        .EndPlayerVerification()
                    .VerifyPlayer(Dana)
                        .ReceivesMonopolyCardPlayedEvent(expectedResources, Adam)
                        .ThenVerifyPlayerState()
                            .Resources(ResourceClutch.OneGrain + ResourceClutch.OneWool)
                        .EndPlayerVerification()
                    .Run(this.logDirectory);
            }
            finally
            {
                this.AttachReports();
            }
        }

        [Test]
        public void PlayerPlaysMonopolyCardButGetsNoResources()
        {
            try
            {
                this.CompletePlayerInfrastructureSetup()
                    .WithCustomResourceCollection()
                    .WithInitialPlayerSetupFor(Adam, MonopolyCard())
                    .WhenPlayer(Adam)
                        .ReceivesStartTurnEvent(3, 3).ThenPlayMonopolyCard(ResourceTypes.Lumber)
                        .ReceivesMonopolyCardPlayedEvent(null)
                        .ThenVerifyPlayerState()
                            .Resources(ResourceClutch.Zero)
                        .EndPlayerVerification()
                    .VerifyPlayer(Babara)
                        .ReceivesMonopolyCardPlayedEvent(null, Adam)
                        .ThenVerifyPlayerState()
                            .Resources(ResourceClutch.Zero)
                        .EndPlayerVerification()
                    .VerifyPlayer(Charlie)
                        .ReceivesMonopolyCardPlayedEvent(null, Adam)
                        .ThenVerifyPlayerState()
                            .Resources(ResourceClutch.Zero)
                        .EndPlayerVerification()
                    .VerifyPlayer(Dana)
                        .ReceivesMonopolyCardPlayedEvent(null, Adam)
                        .ThenVerifyPlayerState()
                            .Resources(ResourceClutch.Zero)
                        .EndPlayerVerification()
                    .Run(this.logDirectory);
            }
            finally
            {
                this.AttachReports();
            }
        }

        [Test]
        public void PlayerPlaysMonopolyCardAndGetsSomeResources()
        {
            var expectedResources = new Dictionary<string, ResourceClutch>();
            expectedResources.Add(Babara, ResourceClutch.OneBrick * 2);
            expectedResources.Add(Dana, ResourceClutch.OneBrick * 3);
            try
            {
                this.CompletePlayerInfrastructureSetup()
                    .WithCustomResourceCollection()
                    .WithInitialPlayerSetupFor(Adam, MonopolyCard())
                    .WithInitialPlayerSetupFor(Babara, Resources(ResourceClutch.OneBrick * 2))
                    .WithInitialPlayerSetupFor(Charlie, Resources(ResourceClutch.OneGrain))
                    .WithInitialPlayerSetupFor(Dana, Resources(ResourceClutch.OneBrick * 3))
                    .WhenPlayer(Adam)
                        .ReceivesStartTurnEvent(3, 3).ThenPlayMonopolyCard(ResourceTypes.Brick)
                        .ReceivesMonopolyCardPlayedEvent(expectedResources)
                        .ThenVerifyPlayerState()
                            .Resources(ResourceClutch.OneBrick * 5)
                        .EndPlayerVerification()
                    .VerifyPlayer(Babara)
                        .ReceivesMonopolyCardPlayedEvent(expectedResources, Adam)
                        .ThenVerifyPlayerState()
                            .Resources(ResourceClutch.Zero)
                        .EndPlayerVerification()
                    .VerifyPlayer(Charlie)
                        .ReceivesMonopolyCardPlayedEvent(expectedResources, Adam)
                        .ThenVerifyPlayerState()
                            .Resources(ResourceClutch.OneGrain)
                        .EndPlayerVerification()
                    .VerifyPlayer(Dana)
                        .ReceivesMonopolyCardPlayedEvent(expectedResources, Adam)
                        .ThenVerifyPlayerState()
                            .Resources(ResourceClutch.Zero)
                        .EndPlayerVerification()
                    .Run(this.logDirectory);
            }
            finally
            {
                this.AttachReports();
            }
        }

        [Test]
        public void PlayerPlaysYearOfPlentyCard()
        {
            try
            {
                this.CompletePlayerInfrastructureSetup()
                    .WithCustomResourceCollection()
                    .WithInitialPlayerSetupFor(Adam, YearOfPlentyCard(1))
                    .WhenPlayer(Adam)
                        .ReceivesStartTurnEvent(3, 3).ThenPlayYearOfPlentyCard(ResourceTypes.Brick, ResourceTypes.Grain)
                        .ReceivesYearOfPlentyCardPlayedEvent(ResourceTypes.Brick, ResourceTypes.Grain)
                        .ThenVerifyPlayerState()
                            .Resources(ResourceClutch.OneBrick + ResourceClutch.OneGrain)
                        .EndPlayerVerification()
                    .VerifyPlayer(Babara)
                        .ReceivesYearOfPlentyCardPlayedEvent(ResourceTypes.Brick, ResourceTypes.Grain, Adam)
                    .VerifyPlayer(Charlie)
                        .ReceivesYearOfPlentyCardPlayedEvent(ResourceTypes.Brick, ResourceTypes.Grain, Adam)
                    .VerifyPlayer(Dana)
                        .ReceivesYearOfPlentyCardPlayedEvent(ResourceTypes.Brick, ResourceTypes.Grain, Adam)
                    .Run(this.logDirectory);
            }
            finally
            {
                this.AttachReports();
            }
        }

        [Test]
        public void PlayerPlaysKnightCardAndNewHexIsSameAsCurrentHex()
        {
			try
            {
                this.CompletePlayerInfrastructureSetup()
                    .WithCustomResourceCollection()
                    .WithInitialPlayerSetupFor(Adam, KnightCard())
                    .WhenPlayer(Adam)
                        .ReceivesStartTurnEvent(3, 3).ThenPlayKnightCard(0)
                        .ReceivesGameErrorEvent(ErrorCodes.NewRobberHexIsSameAsCurrentRobberHex, "New robber hex cannot be the same as previous robber hex")
                    .VerifyPlayer(Babara)
                        .DidNotReceiveEventOfType<KnightCardPlayedEvent>()
                        .DidNotReceiveEventOfType<GameErrorEvent>()
                    .VerifyPlayer(Charlie)
                        .DidNotReceiveEventOfType<KnightCardPlayedEvent>()
                        .DidNotReceiveEventOfType<GameErrorEvent>()
                    .VerifyPlayer(Dana)
                        .DidNotReceiveEventOfType<KnightCardPlayedEvent>()
                        .DidNotReceiveEventOfType<GameErrorEvent>()
                    .Run(this.logDirectory);
            }
            finally
            {
                this.AttachReports();
            }
        }

        [Test]
        public void PlayerPlaysKnightCardAfterInitialError()
        {
            try
            {
                this.CompletePlayerInfrastructureSetup()
                    .WithCustomResourceCollection()
                    .WithInitialPlayerSetupFor(Adam, KnightCard())
                    .WhenPlayer(Adam)
                        .ReceivesStartTurnEvent(3, 3).ThenPlayKnightCard(0)
                        .ReceivesGameErrorEvent(ErrorCodes.NewRobberHexIsSameAsCurrentRobberHex, "New robber hex cannot be the same as previous robber hex")
                        .ThenPlayKnightCard(4)
                        .DidNotReceiveEventOfTypeAfterCount<GameErrorEvent>(1)
                    .VerifyPlayer(Babara)
                        .DidNotReceiveEventOfType<GameErrorEvent>()
                    .VerifyPlayer(Charlie)
                        .DidNotReceiveEventOfType<GameErrorEvent>()
                    .VerifyPlayer(Dana)
                        .DidNotReceiveEventOfType<GameErrorEvent>()
                    .Run(this.logDirectory);
            }
            finally
            {
                this.AttachReports();
            }
        }

        [Test]
        public void PlayerPlaysKnightCardAndNewHexHasNoPlayers()
        {
			try
            { 
                this.CompletePlayerInfrastructureSetup()
                    .WithCustomResourceCollection()
                    .WithInitialPlayerSetupFor(Adam, KnightCard())
                    .WhenPlayer(Adam)
                        .ReceivesStartTurnEvent(3, 3).ThenPlayKnightCard(4)
                        .ReceivesKnightCardPlayedEvent(4).ThenDoNothing()
                    .WhenPlayer(Babara)
                        .ReceivesKnightCardPlayedEvent(Adam, 4).ThenDoNothing()
                    .WhenPlayer(Charlie)
                        .ReceivesKnightCardPlayedEvent(Adam, 4).ThenDoNothing()
                    .WhenPlayer(Dana)
                        .ReceivesKnightCardPlayedEvent(Adam, 4).ThenDoNothing()
                    .VerifyPlayer(Adam)
                        .DidNotReceiveEventOfType<RobbingChoicesEvent>()
                        .DidNotReceiveEventOfType<ResourcesGainedEvent>()
                    .VerifyPlayer(Babara)
                        .DidNotReceiveEventOfType<ResourcesLostEvent>()
                    .VerifyPlayer(Charlie)
                        .DidNotReceiveEventOfType<ResourcesLostEvent>()
                    .VerifyPlayer(Dana)
                        .DidNotReceiveEventOfType<ResourcesLostEvent>()
                    .Run(this.logDirectory);
            }
            finally
            {
                this.AttachReports();
            }
        }

        [Test]
        public void PlayerPlaysKnightCardAndNewHexHasOnePlayer()
        {
			try
            { 
                var robbedResource = ResourceClutch.OneLumber;
                this.CompletePlayerInfrastructureSetup()
                    .WithInitialPlayerSetupFor(Adam, KnightCard())
                    .WhenPlayer(Adam)
                        .ReceivesStartTurnEvent(1, 1).ThenPlayKnightCard(3)
                        .ReceivesKnightCardPlayedEvent(3).ThenDoNothing()
                        .ReceivesResourcesRobbedEvent(Babara, ResourceTypes.Lumber)
                        .ThenVerifyPlayerState()
                            .Resources(ResourceClutch.OneBrick + ResourceClutch.OneGrain + ResourceClutch.OneLumber + ResourceClutch.OneWool)
                        .EndPlayerVerification()
                    .WhenPlayer(Babara)
                        .ReceivesKnightCardPlayedEvent(Adam, 3).ThenDoNothing()
                        .ReceivesResourcesStolenEvent(robbedResource)
                        .ThenVerifyPlayerState()
                            .Resources(ResourceClutch.OneGrain + ResourceClutch.OneWool)
                        .EndPlayerVerification()
                    .WhenPlayer(Charlie)
                        .ReceivesKnightCardPlayedEvent(Adam, 3).ThenDoNothing()
                        .ReceivesResourcesStolenEvent(Babara, robbedResource)
                    .WhenPlayer(Dana)
                        .ReceivesKnightCardPlayedEvent(Adam, 3).ThenDoNothing()
                        .ReceivesResourcesStolenEvent(Babara, robbedResource)
                    .Run(this.logDirectory);
            }
            finally
            {
                this.AttachReports();
            }
        }

        [Test]
        public void PlayerPlaysKnightCardAndNewHexHasOnePlayerWhichIsCardPlayer()
        {
			try
            {
                this.CompletePlayerInfrastructureSetup()
                    .WithCustomResourceCollection()
                    .WithInitialPlayerSetupFor(Adam, KnightCard())
                    .WhenPlayer(Adam)
                        .ReceivesStartTurnEvent(3, 3).ThenPlayKnightCard(1)
                        .ReceivesKnightCardPlayedEvent(1).ThenDoNothing()
                    .WhenPlayer(Babara)
                        .ReceivesKnightCardPlayedEvent(Adam, 1).ThenDoNothing()
                    .WhenPlayer(Charlie)
                        .ReceivesKnightCardPlayedEvent(Adam, 1).ThenDoNothing()
                    .WhenPlayer(Dana)
                        .ReceivesKnightCardPlayedEvent(Adam, 1).ThenDoNothing()
                    .VerifyPlayer(Adam)
                        .DidNotReceiveEventOfType<RobbingChoicesEvent>()
                        .DidNotReceiveEventOfType<ResourcesGainedEvent>()
                    .VerifyPlayer(Babara)
                        .DidNotReceiveEventOfType<ResourcesLostEvent>()
                    .VerifyPlayer(Charlie)
                        .DidNotReceiveEventOfType<ResourcesLostEvent>()
                    .VerifyPlayer(Dana)
                        .DidNotReceiveEventOfType<ResourcesLostEvent>()
                    .Run(this.logDirectory);
            }
            finally
            {
                this.AttachReports();
            }
        }

        [Test]
        public void PlayerPlaysKnightCardAndSelectsInvalidPlayer()
        {
			try
            {
                var infrastructureSetupBuilder = new InfrastructureSetupBuilder();
                infrastructureSetupBuilder.Add(Adam, Adam_FirstSettlementLocation, Adam_FirstRoadEndLocation)
                    .Add(Babara, Babara_FirstSettlementLocation, Babara_FirstRoadEndLocation)
                    .Add(Charlie, Charlie_FirstSettlementLocation, Charlie_FirstRoadEndLocation)
                    .Add(Dana, Dana_FirstSettlementLocation, Dana_FirstRoadEndLocation)
                    .Add(Dana, 35, 24)
                    .Add(Charlie, 33, 32)
                    .Add(Babara, Babara_SecondSettlementLocation, Babara_SecondRoadEndLocation)
                    .Add(Adam, Adam_SecondSettlementLocation, Adam_SecondRoadEndLocation);

                var robbingChoices = new Dictionary<string, int>()
                {
                    { Charlie, 4 },
                    { Dana, 5 }
                };

                this.CompletePlayerInfrastructureSetup(infrastructureSetupBuilder.Build())
                    .WithInitialPlayerSetupFor(Adam, KnightCard())
                    .WhenPlayer(Adam)
                        .ReceivesStartTurnEvent(3, 3).ThenPlayKnightCard(9)
                        .ReceivesKnightCardPlayedEvent(9).ThenDoNothing()
                        .ReceivesRobbingChoicesEvent(robbingChoices).ThenSelectRobbedPlayer(Babara)
                        .ReceivesGameErrorEvent(ErrorCodes.InvalidPlayerSelection, "Invalid player selection")
                    .WhenPlayer(Babara)
                        .ReceivesKnightCardPlayedEvent(Adam, 9).ThenDoNothing()
                    .WhenPlayer(Charlie)
                        .ReceivesKnightCardPlayedEvent(Adam, 9).ThenDoNothing()
                    .WhenPlayer(Dana)
                        .ReceivesKnightCardPlayedEvent(Adam, 9).ThenDoNothing()
                    .VerifyPlayer(Adam)
                        .DidNotReceiveEventOfType<ChooseLostResourcesEvent>()
                    .VerifyPlayer(Babara)
                        .DidNotReceiveEventOfType<ChooseLostResourcesEvent>()
                    .VerifyPlayer(Charlie)
                        .DidNotReceiveEventOfType<ChooseLostResourcesEvent>()
                    .VerifyPlayer(Dana)
                        .DidNotReceiveEventOfType<ChooseLostResourcesEvent>()
                    .Run(this.logDirectory);
            }
            finally
            {
                this.AttachReports();
            }
        }

        [Test]
        public void PlayerPlaysKnightCardAndNewHexHasMultiplePlayers()
        {
			try
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
                    { Charlie, 4 },
                    { Dana, 5 }
                };

                this.CompletePlayerInfrastructureSetup(infrastructureSetupBuilder.Build())
                    .WithInitialPlayerSetupFor(Adam, KnightCard())
                    .WhenPlayer(Adam)
                        .ReceivesStartTurnEvent(3, 3).ThenPlayKnightCard(9)
                        .ReceivesKnightCardPlayedEvent(9).ThenDoNothing()
                        .ReceivesRobbingChoicesEvent(robbingChoices).ThenDoNothing()
                    .WhenPlayer(Babara)
                        .ReceivesKnightCardPlayedEvent(Adam, 9).ThenDoNothing()
                    .WhenPlayer(Charlie)
                        .ReceivesKnightCardPlayedEvent(Adam, 9).ThenDoNothing()
                    .WhenPlayer(Dana)
                        .ReceivesKnightCardPlayedEvent(Adam, 9).ThenDoNothing()
                    .VerifyPlayer(Adam)
                        .DidNotReceiveEventOfType<ChooseLostResourcesEvent>()
                    .VerifyPlayer(Babara)
                        .DidNotReceiveEventOfType<ChooseLostResourcesEvent>()
                    .VerifyPlayer(Charlie)
                        .DidNotReceiveEventOfType<ChooseLostResourcesEvent>()
                    .VerifyPlayer(Dana)
                        .DidNotReceiveEventOfType<ChooseLostResourcesEvent>()
                    .Run(this.logDirectory);
            }
            finally
            {
                this.AttachReports();
            }
        }

        [Test]
        public void PlayerPlaysKnightCardAndNewHexHasMultiplePlayersIncludingCardPlayer()
        {
			try
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
                    { Charlie, 4 },
                    { Dana, 5 }
                };

                this.CompletePlayerInfrastructureSetup(infrastructureSetupBuilder.Build())
                    .WithInitialPlayerSetupFor(Adam, KnightCard())
                    .WhenPlayer(Adam)
                        .ReceivesStartTurnEvent(3, 3).ThenPlayKnightCard(9)
                        .ReceivesKnightCardPlayedEvent(9).ThenDoNothing()
                        .ReceivesRobbingChoicesEvent(robbingChoices).ThenDoNothing()
                    .WhenPlayer(Babara)
                        .ReceivesKnightCardPlayedEvent(Adam, 9).ThenDoNothing()
                    .WhenPlayer(Charlie)
                        .ReceivesKnightCardPlayedEvent(Adam, 9).ThenDoNothing()
                    .WhenPlayer(Dana)
                        .ReceivesKnightCardPlayedEvent(Adam, 9).ThenDoNothing()
                    .VerifyPlayer(Adam)
                        .DidNotReceiveEventOfType<ChooseLostResourcesEvent>()
                    .VerifyPlayer(Babara)
                        .DidNotReceiveEventOfType<ChooseLostResourcesEvent>()
                    .VerifyPlayer(Charlie)
                        .DidNotReceiveEventOfType<ChooseLostResourcesEvent>()
                    .VerifyPlayer(Dana)
                        .DidNotReceiveEventOfType<ChooseLostResourcesEvent>()
                    .Run(this.logDirectory);
            }
            finally
            {
                this.AttachReports();
            }
        }

        [Test]
        public void PlayerRollsSevenAndAllPlayersWithMoreThanSevenResourcesLoseResources()
        {
			try
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

                this.CompletePlayerInfrastructureSetup()
                    .WithCustomResourceCollection()
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
                            .ReceivesResourcesStolenEvent(Adam, adamsLostResources)
                            .ReceivesResourcesStolenEvent(Babara, babarasLostResources)
                            .ReceivesResourcesStolenEvent(Dana, danasLostResources)
                        .ReceivesAllEnd()
                        .ThenVerifyPlayerState()
                            .Resources(adamsFinalResources)
                            .EndPlayerVerification()
                    .WhenPlayer(Babara)
                        .ReceivesChooseLostResourcesEvent(5).ThenChooseResourcesToLose(babarasLostResources)
                        .ReceivesAll()
                            .ReceivesResourcesStolenEvent(Adam, adamsLostResources)
                            .ReceivesResourcesStolenEvent(Babara, babarasLostResources)
                            .ReceivesResourcesStolenEvent(Dana, danasLostResources)
                        .ReceivesAllEnd()
                        .ThenVerifyPlayerState()
                            .Resources(babarasFinalResources)
                            .EndPlayerVerification()
                    .WhenPlayer(Charlie)
                        .ReceivesAll()
                            .ReceivesResourcesStolenEvent(Adam, adamsLostResources)
                            .ReceivesResourcesStolenEvent(Babara, babarasLostResources)
                            .ReceivesResourcesStolenEvent(Dana, danasLostResources)
                        .ReceivesAllEnd()
                        .ThenVerifyPlayerState()
                            .Resources(charliesInitialResources)
                            .EndPlayerVerification()
                    .WhenPlayer(Dana)
                        .ReceivesChooseLostResourcesEvent(4).ThenChooseResourcesToLose(danasLostResources)
                        .ReceivesAll()
                            .ReceivesResourcesStolenEvent(Adam, adamsLostResources)
                            .ReceivesResourcesStolenEvent(Babara, babarasLostResources)
                            .ReceivesResourcesStolenEvent(Dana, danasLostResources)
                        .ReceivesAllEnd()
                        .ThenVerifyPlayerState()
                            .Resources(danasFinalResources)
                            .EndPlayerVerification()
                    .VerifyPlayer(Charlie)
                        .DidNotReceiveEventOfType<ChooseLostResourcesEvent>()
                    .Run(this.logDirectory);
            }
            finally
            {
                this.AttachReports();
            }
        }

        [Test]
        public void PlayerRollsSevenAndPlayerSendsTooManyResources()
        {
			try
            {
                var adamsInitialResources = new ResourceClutch(1, 2, 2, 2, 2); // 9 resources
                var adamsLostResources = new ResourceClutch(1, 1, 1, 1, 1);

                this.CompletePlayerInfrastructureSetup()
                    .WithCustomResourceCollection()
                    .WithInitialPlayerSetupFor(
                        Adam,
                        Resources(adamsInitialResources))
                    .WhenPlayer(Adam)
                        .ReceivesStartTurnEvent(3, 4).ThenDoNothing()
                        .ReceivesChooseLostResourcesEvent(4).ThenChooseResourcesToLose(adamsLostResources)
                        .ReceivesGameErrorEvent(ErrorCodes.IncorrectResourceCount, "Expected 4 resources but received 5")
                    .VerifyPlayer(Babara)
                        .DidNotReceiveEventOfType<GameErrorEvent>()
                    .VerifyPlayer(Charlie)
                        .DidNotReceiveEventOfType<GameErrorEvent>()
                    .VerifyPlayer(Dana)
                        .DidNotReceiveEventOfType<GameErrorEvent>()
                    .Run(this.logDirectory);
            }
            finally
            {
                this.AttachReports();
            }
        }
        
        [Test]
        public void PlayerRollsSevenAndPlayerSendsTooLittleResources()
        {
			try
            {
                var adamsInitialResources = new ResourceClutch(1, 2, 2, 2, 2); // 9 resources
                var adamsLostResources = new ResourceClutch(0, 0, 1, 1, 1);

                this.CompletePlayerInfrastructureSetup()
                    .WithCustomResourceCollection()
                    .WithInitialPlayerSetupFor(
                        Adam,
                        Resources(adamsInitialResources))
                    .WhenPlayer(Adam)
                        .ReceivesStartTurnEvent(3, 4).ThenDoNothing()
                        .ReceivesChooseLostResourcesEvent(4).ThenChooseResourcesToLose(adamsLostResources)
                        .ReceivesGameErrorEvent(ErrorCodes.IncorrectResourceCount, "Expected 4 resources but received 3")
                    .VerifyPlayer(Babara)
                        .DidNotReceiveEventOfType<GameErrorEvent>()
                    .VerifyPlayer(Charlie)
                        .DidNotReceiveEventOfType<GameErrorEvent>()
                    .VerifyPlayer(Dana)
                        .DidNotReceiveEventOfType<GameErrorEvent>()
                    .Run(this.logDirectory);
            }
            finally
            {
                this.AttachReports();
            }
        }

        [Test]
        public void PlayerRollsSevenAndPlayerSendsResourcesResultingInNegativeResources()
        {
			try
            {
                var adamsInitialResources = new ResourceClutch(1, 2, 2, 2, 2); // 9 resources
                var adamsLostResources = new ResourceClutch(2, 0, 0, 1, 1);

                this.CompletePlayerInfrastructureSetup()
                    .WithCustomResourceCollection()
                    .WithInitialPlayerSetupFor(
                        Adam,
                        Resources(adamsInitialResources))
                    .WhenPlayer(Adam)
                        .ReceivesStartTurnEvent(3, 4).ThenDoNothing()
                        .ReceivesChooseLostResourcesEvent(4).ThenChooseResourcesToLose(adamsLostResources)
                        .ReceivesGameErrorEvent(ErrorCodes.IncorrectResourceCount, "Resources sent results in negative counts")
                    .VerifyPlayer(Babara)
                        .DidNotReceiveEventOfType<GameErrorEvent>()
                    .VerifyPlayer(Charlie)
                        .DidNotReceiveEventOfType<GameErrorEvent>()
                    .VerifyPlayer(Dana)
                        .DidNotReceiveEventOfType<GameErrorEvent>()
                    .Run(this.logDirectory);
            }
            finally
            {
                this.AttachReports();
            }
        }

        [Test]
        public void PlayerRollsSevenAndAllPlayersWithMoreThanSeventResourcesMustSendResourcesBeforeRobberCanBePlaced()
        {
			try
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

                this.CompletePlayerInfrastructureSetup()
                    .WithCustomResourceCollection()
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
                            .ReceivesResourcesStolenEvent(Adam, adamsLostResources)
                            .ReceivesResourcesStolenEvent(Babara, babarasLostResources)
                            .ReceivesResourcesStolenEvent(Dana, danasLostResources)
                        .ReceivesAllEnd().ThenDoNothing()
                        .ReceivesPlaceRobberEvent()
                    .WhenPlayer(Babara)
                        .ReceivesChooseLostResourcesEvent(5).ThenChooseResourcesToLose(babarasLostResources)
                        .ReceivesAll()
                            .ReceivesResourcesStolenEvent(Adam, adamsLostResources)
                            .ReceivesResourcesStolenEvent(Babara, babarasLostResources)
                            .ReceivesResourcesStolenEvent(Dana, danasLostResources)
                        .ReceivesAllEnd()
                        .ThenVerifyPlayerState()
                            .Resources(babarasFinalResources)
                            .EndPlayerVerification()
                    .WhenPlayer(Charlie)
                        .ReceivesAll()
                            .ReceivesResourcesStolenEvent(Adam, adamsLostResources)
                            .ReceivesResourcesStolenEvent(Babara, babarasLostResources)
                            .ReceivesResourcesStolenEvent(Dana, danasLostResources)
                        .ReceivesAllEnd()
                        .ThenVerifyPlayerState()
                            .Resources(charliesInitialResources)
                            .EndPlayerVerification()
                    .WhenPlayer(Dana)
                        .ReceivesChooseLostResourcesEvent(4).ThenChooseResourcesToLose(danasLostResources)
                        .ReceivesAll()
                            .ReceivesResourcesStolenEvent(Adam, adamsLostResources)
                            .ReceivesResourcesStolenEvent(Babara, babarasLostResources)
                            .ReceivesResourcesStolenEvent(Dana, danasLostResources)
                        .ReceivesAllEnd()
                        .ThenVerifyPlayerState()
                            .Resources(danasFinalResources)
                            .EndPlayerVerification()
                    .VerifyPlayer(Charlie)
                        .DidNotReceiveEventOfType<ChooseLostResourcesEvent>()
                    .Run(this.logDirectory);
            }
            finally
            {
                this.AttachReports();
            }
        }

        [Test]
        public void PlayerRollsSevenAndNewHexHasNoPlayers()
        {
			try
            {
                this.CompletePlayerInfrastructureSetup()
                    .WithCustomResourceCollection()
                    .WhenPlayer(Adam)
                        .ReceivesStartTurnEvent(3, 4).ThenDoNothing()
                        .ReceivesPlaceRobberEvent().ThenPlaceRobber(4)
                    .WhenPlayer(Babara)
                        .ReceivesRobberPlacedEvent(Adam, 4).ThenDoNothing()
                    .WhenPlayer(Charlie)
                        .ReceivesRobberPlacedEvent(Adam, 4).ThenDoNothing()
                    .WhenPlayer(Dana)
                        .ReceivesRobberPlacedEvent(Adam, 4).ThenDoNothing()
                    .VerifyPlayer(Adam)
                        .DidNotReceiveEventOfType<RobbingChoicesEvent>()
                    .VerifyPlayer(Babara)
                        .DidNotReceiveEventOfType<ChooseLostResourcesEvent>()
                    .VerifyPlayer(Charlie)
                        .DidNotReceiveEventOfType<ChooseLostResourcesEvent>()
                    .VerifyPlayer(Dana)
                        .DidNotReceiveEventOfType<ChooseLostResourcesEvent>()
                    .Run(this.logDirectory);
            }
            finally
            {
                this.AttachReports();
            }
        }

        [Test]
        public void PlayerRollsSevenAndNewHexIsSameAsCurrentHex()
        {
			try
            {
                this.CompletePlayerInfrastructureSetup()
                    .WithCustomResourceCollection()
                    .WhenPlayer(Adam)
                        .ReceivesStartTurnEvent(3, 4).ThenDoNothing()
                        .ReceivesPlaceRobberEvent().ThenPlaceRobber(0)
                        .ReceivesGameErrorEvent(ErrorCodes.NewRobberHexIsSameAsCurrentRobberHex, "New robber hex cannot be the same as previous robber hex")
                    .VerifyPlayer(Babara)
                        .DidNotReceiveEventOfType<GameErrorEvent>()
                    .VerifyPlayer(Charlie)
                        .DidNotReceiveEventOfType<GameErrorEvent>()
                    .VerifyPlayer(Dana)
                        .DidNotReceiveEventOfType<GameErrorEvent>()
                    .Run(this.logDirectory);
            }
            finally
            {
                this.AttachReports();
            }
        }

        [Test]
        public void PlayerRollsSevenAndNewHexHasOnePlayer()
        {
			try
            {
                var robbedResource = ResourceClutch.OneLumber;
                this.CompletePlayerInfrastructureSetup()
                    .WhenPlayer(Adam)
                        .ReceivesStartTurnEvent(3, 4).ThenDoNothing()
                        .ReceivesPlaceRobberEvent().ThenPlaceRobber(3)
                        .ReceivesResourcesRobbedEvent(Babara, ResourceTypes.Lumber)
                        .ThenVerifyPlayerState()
                            .Resources(ResourceClutch.OneBrick + ResourceClutch.OneGrain + ResourceClutch.OneLumber + ResourceClutch.OneWool)
                        .EndPlayerVerification()
                    .WhenPlayer(Babara)
                        .ReceivesRobberPlacedEvent(Adam, 3).ThenDoNothing()
                        .ReceivesResourcesStolenEvent(robbedResource)
                        .ThenVerifyPlayerState()
                            .Resources(ResourceClutch.OneGrain + ResourceClutch.OneWool)
                        .EndPlayerVerification()
                    .WhenPlayer(Charlie)
                        .ReceivesRobberPlacedEvent(Adam, 3).ThenDoNothing()
                        .ReceivesResourcesStolenEvent(Babara, robbedResource)
                    .WhenPlayer(Dana)
                        .ReceivesRobberPlacedEvent(Adam, 3).ThenDoNothing()
                        .ReceivesResourcesStolenEvent(Babara, robbedResource)
                    .Run(this.logDirectory);
            }
            finally
            {
                this.AttachReports();
            }
        }

        [Test]
        public void PlayerRollsSevenAndNewHexHasOnePlayerWhichIsRollingPlayer()
        {
			try
            {
                this.CompletePlayerInfrastructureSetup()
                    .WithCustomResourceCollection()
                    .WhenPlayer(Adam)
                        .ReceivesStartTurnEvent(3, 4).ThenDoNothing()
                        .ReceivesPlaceRobberEvent().ThenPlaceRobber(2)
                        .ReceivesRobberPlacedEvent(2)
                    .WhenPlayer(Babara)
                        .ReceivesRobberPlacedEvent(Adam, 2).ThenDoNothing()
                    .WhenPlayer(Charlie)
                        .ReceivesRobberPlacedEvent(Adam, 2).ThenDoNothing()
                    .WhenPlayer(Dana)
                        .ReceivesRobberPlacedEvent(Adam, 2).ThenDoNothing()
                    .VerifyPlayer(Adam)
                        .DidNotReceiveEventOfType<RobbingChoicesEvent>()
                        .DidNotReceiveEventOfType<ChooseLostResourcesEvent>()
                    .VerifyPlayer(Babara)
                        .DidNotReceiveEventOfType<ChooseLostResourcesEvent>()
                    .VerifyPlayer(Charlie)
                        .DidNotReceiveEventOfType<ChooseLostResourcesEvent>()
                    .VerifyPlayer(Dana)
                        .DidNotReceiveEventOfType<ChooseLostResourcesEvent>()
                    .Run(this.logDirectory);
            }
            finally
            {
                this.AttachReports();
            }
        }

        [Test]
        public void PlayerRollsSevenAndNewHexHasMultiplePlayers()
        {
			try
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

                this.CompletePlayerInfrastructureSetup(infrastructureSetupBuilder.Build())
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
                    .Run(this.logDirectory);
            }
            finally
            {
                this.AttachReports();
            }
        }

        [Test]
        public void PlayerRollsSevenAndNewHexHasMultiplePlayersIncludingRollingPlayer()
        {
			try
            { 
                var infrastructureSetupBuilder = new InfrastructureSetupBuilder();
                infrastructureSetupBuilder.Add(Adam, 21, 11)
                    .Add(Babara, Babara_FirstSettlementLocation, Babara_FirstRoadEndLocation)
                    .Add(Charlie, Charlie_FirstSettlementLocation, Charlie_FirstRoadEndLocation)
                    .Add(Dana, Dana_FirstSettlementLocation, Dana_FirstRoadEndLocation)
                    .Add(Dana, 35, 24)
                    .Add(Charlie, 33, 32)
                    .Add(Babara, Babara_SecondSettlementLocation, Babara_SecondRoadEndLocation)
                    .Add(Adam, Adam_SecondSettlementLocation, Adam_SecondRoadEndLocation);

                var robbingChoices = new Dictionary<string, int>()
                {
                    { Charlie, 3 },
                    { Dana, 3 }
                };

                this.CompletePlayerInfrastructureSetup(infrastructureSetupBuilder.Build())
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
                    .Run(this.logDirectory);
            }
            finally
            {
                this.AttachReports();
            }
        }

        /// <summary>
        /// Passing in an id of a player that is not on the selected robber hex when choosing the resource 
        /// causes an error to be raised.
        /// </summary>
        [Test]
        public void PlayerRollsSevenAndSelectsInvalidPlayer()
        {
			try
            {
                var infrastructureSetupBuilder = new InfrastructureSetupBuilder();
                infrastructureSetupBuilder.Add(Adam, Adam_FirstSettlementLocation, Adam_FirstRoadEndLocation)
                    .Add(Babara, Babara_FirstSettlementLocation, Babara_FirstRoadEndLocation)
                    .Add(Charlie, Charlie_FirstSettlementLocation, Charlie_FirstRoadEndLocation)
                    .Add(Dana, Dana_FirstSettlementLocation, Dana_FirstRoadEndLocation)
                    .Add(Dana, 35, 24)
                    .Add(Charlie, 33, 32)
                    .Add(Babara, Babara_SecondSettlementLocation, Babara_SecondRoadEndLocation)
                    .Add(Adam, Adam_SecondSettlementLocation, Adam_SecondRoadEndLocation);

                var robbingChoices = new Dictionary<string, int>()
                {
                    { Charlie, 3 },
                    { Dana, 3 }
                };

                this.CompletePlayerInfrastructureSetup(infrastructureSetupBuilder.Build())
                    .WhenPlayer(Adam)
                        .ReceivesStartTurnEvent(3, 4).ThenDoNothing()
                        .ReceivesPlaceRobberEvent().ThenPlaceRobber(9)
                        .ReceivesRobbingChoicesEvent(robbingChoices).ThenSelectRobbedPlayer(Babara)
                        .ReceivesGameErrorEvent(ErrorCodes.InvalidPlayerSelection, "Invalid player selection")
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
                    .Run(this.logDirectory);
            }
            finally
            {
                this.AttachReports();
            }
        }

        [Test]
        public void PlayerRollsSevenAndGetsResourceFromSelectedPlayer()
        {
			try
            {
                var infrastructureSetupBuilder = new InfrastructureSetupBuilder();
                infrastructureSetupBuilder.Add(Adam, Adam_FirstSettlementLocation, Adam_FirstRoadEndLocation)
                    .Add(Babara, Babara_FirstSettlementLocation, Babara_FirstRoadEndLocation)
                    .Add(Charlie, Charlie_FirstSettlementLocation, Charlie_FirstRoadEndLocation)
                    .Add(Dana, Dana_FirstSettlementLocation, Dana_FirstRoadEndLocation)
                    .Add(Dana, 35, 24)
                    .Add(Charlie, 33, 32)
                    .Add(Babara, Babara_SecondSettlementLocation, Babara_SecondRoadEndLocation)
                    .Add(Adam, Adam_SecondSettlementLocation, Adam_SecondRoadEndLocation);

                var robbingChoices = new Dictionary<string, int>()
                {
                    { Charlie, 3 },
                    { Dana, 3 }
                };

                var robbedResource = ResourceClutch.OneLumber;
                this.CompletePlayerInfrastructureSetup(infrastructureSetupBuilder.Build())
                    .WhenPlayer(Adam)
                        .ReceivesStartTurnEvent(3, 4).ThenDoNothing()
                        .ReceivesPlaceRobberEvent().ThenPlaceRobber(9)
                        .ReceivesRobbingChoicesEvent(robbingChoices).ThenSelectRobbedPlayer(Charlie)
                        .ReceivesResourcesRobbedEvent(Charlie, ResourceTypes.Lumber)
                        .ThenVerifyPlayerState()
                            .Resources(ResourceClutch.OneBrick + ResourceClutch.OneGrain + robbedResource + ResourceClutch.OneWool)
                        .EndPlayerVerification()
                        .ThenPlaceRoadSegment(4, 3)
                    .WhenPlayer(Babara)
                        .ReceivesRobberPlacedEvent(Adam, 9).ThenDoNothing()
                        .ReceivesResourcesStolenEvent(Charlie, robbedResource)
                        .ReceivesRoadSegmentPlacementEvent(Adam, 4, 3)
                    .WhenPlayer(Charlie)
                        .ReceivesRobberPlacedEvent(Adam, 9).ThenDoNothing()
                        .ReceivesResourcesStolenEvent(robbedResource)
                        .ThenVerifyPlayerState()
                            .Resources(ResourceClutch.OneGrain + ResourceClutch.OneWool)
                        .EndPlayerVerification()
                        .ReceivesRoadSegmentPlacementEvent(Adam, 4, 3)
                    .WhenPlayer(Dana)
                        .ReceivesRobberPlacedEvent(Adam, 9).ThenDoNothing()
                        .ReceivesResourcesStolenEvent(Charlie, robbedResource)
                        .ReceivesRoadSegmentPlacementEvent(Adam, 4, 3)
                    .Run(this.logDirectory);
            }
            finally
            {
                this.AttachReports();
            }
        }

        [Test]
        public void PlayerQuitsDuringFirstRoundOfGameSetup()
        {
			try
            {
                var expectedGameBoardSetup = new GameBoardSetup(new GameBoard(BoardSizes.Standard));
                var playerOrder = new[] { Adam, Babara, Charlie, Dana };
                ScenarioRunner.CreateScenarioRunner()
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
                    .Run(this.logDirectory);
            }
            finally
            {
                this.AttachReports();
            }
        }

        [Test]
        public void PlayerQuitsDuringSecondRoundOfGameSetup()
        {
			try
            {
                var expectedGameBoardSetup = new GameBoardSetup(new GameBoard(BoardSizes.Standard));
                var playerOrder = new[] { Adam, Babara, Charlie, Dana };
                ScenarioRunner.CreateScenarioRunner()
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
                    .Run(this.logDirectory);
            }
            finally
            {
                this.AttachReports();
            }
        }

        [Test]
        public void PlayerSendsIncorrectCommandDuringGameStartConfirmation()
        {
			try
            {
                var expectedGameBoardSetup = new GameBoardSetup(new GameBoard(BoardSizes.Standard));
                var playerOrder = new[] { Adam, Babara, Charlie, Dana };
                ScenarioRunner.CreateScenarioRunner()
                    .WithPlayer(Adam).WithPlayer(Babara).WithPlayer(Charlie).WithPlayer(Dana)
                    .WithTurnOrder(playerOrder)
                    .WhenPlayer(Adam)
                        .ReceivesPlaceInfrastructureSetupEvent()
                        .ThenPlaceStartingInfrastructure(Adam_FirstSettlementLocation, Adam_FirstRoadEndLocation)
                        .ReceivesPlaceInfrastructureSetupEvent()
                        .ThenPlaceStartingInfrastructure(Adam_SecondSettlementLocation, Adam_SecondRoadEndLocation)
                        .ReceivesConfirmGameStartEvent()
                        .ThenEndTurn()
                        .ReceivesGameErrorEvent(ErrorCodes.IllegalPlayerAction, "Received action type EndOfTurnAction. Expected one of ConfirmGameStartAction, QuitGameAction")
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
                    .Run(this.logDirectory);
            }
            finally
            {
                this.AttachReports();
            }
        }

        [Test]
        public void PlayerSendsIncorrectCommandDuringGameSetup()
        {
			try
            {
                var expectedGameBoardSetup = new GameBoardSetup(new GameBoard(BoardSizes.Standard));
                var playerOrder = new[] { Adam, Babara, Charlie, Dana };
                ScenarioRunner.CreateScenarioRunner()
                    .WithPlayer(Adam).WithPlayer(Babara).WithPlayer(Charlie).WithPlayer(Dana)
                    .WithTurnOrder(playerOrder)
                    .WhenPlayer(Adam)
                        .ReceivesPlaceInfrastructureSetupEvent().ThenEndTurn()
                        .ReceivesGameErrorEvent(ErrorCodes.IllegalPlayerAction, "Received action type EndOfTurnAction. Expected one of PlaceSetupInfrastructureAction, QuitGameAction")
                    .Run(this.logDirectory);
            }
            finally
            {
                this.AttachReports();
            }
        }

        [Test]
        public void PlayerTradesOneResourceWithAnotherPlayer()
        {
			try
            {
                var adamResources = ResourceClutch.OneWool;
                var babaraResources = ResourceClutch.OneGrain;

                this.CompletePlayerInfrastructureSetup()
                    .WithCustomResourceCollection()
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
                            .EndPlayerVerification()
                    .WhenPlayer(Babara)
                        .ReceivesAcceptDirectTradeEvent(Babara, ResourceClutch.OneWool, Adam, ResourceClutch.OneGrain)
                        .ThenVerifyPlayerState()
                            .Resources(ResourceClutch.OneWool)
                            .EndPlayerVerification()
                    .WhenPlayer(Charlie)
                        .ReceivesAcceptDirectTradeEvent(Babara, ResourceClutch.OneWool, Adam, ResourceClutch.OneGrain)
                        .ThenDoNothing()
                    .WhenPlayer(Dana)
                        .ReceivesAcceptDirectTradeEvent(Babara, ResourceClutch.OneWool, Adam, ResourceClutch.OneGrain)
                        .ThenDoNothing()
                    .Run(this.logDirectory);
            }
            finally
            {
                this.AttachReports();
            }
        }

        [Test]
        [TestCase(6u, 10u)]
        [TestCase(7u, 11u)]
        public void PlayerWithEightOrNinePointsGainsLargestArmyAndWins(uint initialVP, uint winningVP)
        {
            try
            {
                this.CompletePlayerInfrastructureSetup()
                    .WithInitialPlayerSetupFor(Adam, VictoryPoints(initialVP), KnightCard(3)) // initialVP + 2 settlements placed as part of startup = 8 or 9
                    .WhenPlayer(Adam)
                        .ReceivesStartTurnEvent(3, 3).ThenPlayKnightCard(4)
                        .ReceivesKnightCardPlayedEvent(4).ThenEndTurn()
                        .ReceivesStartTurnEvent(3, 3).ThenPlayKnightCard(0)
                        .ReceivesKnightCardPlayedEvent(0).ThenEndTurn()
                        .ReceivesStartTurnEvent(3, 3).ThenPlayKnightCard(4)
                        .ReceivesKnightCardPlayedEvent(4)
                        .ReceivesLargestArmyChangedEvent()
                        .ReceivesPlayerWonEvent(winningVP)
                        .ThenVerifyPlayerState()
                            .PlayedKnightCards(3)
                            .HeldCardsByType(DevelopmentCardTypes.Knight, 0)
                        .EndPlayerVerification()
                    .WhenPlayer(Babara)
                        .ReceivesStartTurnEvent(3, 3).ThenEndTurn()
                        .ReceivesStartTurnEvent(3, 3).ThenEndTurn()
                        .ReceivesKnightCardPlayedEvent(Adam, 4)
                        .ReceivesLargestArmyChangedEvent(Adam, null)
                        .ReceivesPlayerWonEvent(winningVP, Adam)
                    .WhenPlayer(Charlie)
                        .ReceivesStartTurnEvent(3, 3).ThenEndTurn()
                        .ReceivesStartTurnEvent(3, 3).ThenEndTurn()
                        .ReceivesKnightCardPlayedEvent(Adam, 4)
                        .ReceivesLargestArmyChangedEvent(Adam, null)
                        .ReceivesPlayerWonEvent(winningVP, Adam)
                    .WhenPlayer(Dana)
                        .ReceivesStartTurnEvent(3, 3).ThenEndTurn()
                        .ReceivesStartTurnEvent(3, 3).ThenEndTurn()
                        .ReceivesKnightCardPlayedEvent(Adam, 4)
                        .ReceivesLargestArmyChangedEvent(Adam, null)
                        .ReceivesPlayerWonEvent(winningVP, Adam)
                    .Run(this.logDirectory);
            }
            finally
            {
                this.AttachReports();
            }
        }

        [Test]
        public void PlayerWithLongestRoadDoesNotGetMoreVictoryPointsWhenBuildingSubsequentRoadSegments()
        {
            try
            {
                this.CompletePlayerInfrastructureSetup()
                    .WithInitialPlayerSetupFor(Adam, Resources(ResourceClutch.RoadSegment * 5))
                    .WhenPlayer(Adam)
                        .ReceivesStartTurnEvent(3, 3).ThenPlaceRoadSegment(4, 3)
                        .ReceivesRoadSegmentPlacementEvent(4, 3).ThenPlaceRoadSegment(3, 2)
                        .ReceivesRoadSegmentPlacementEvent(3, 2).ThenPlaceRoadSegment(2, 1)
                        .ReceivesRoadSegmentPlacementEvent(2, 1).ThenPlaceRoadSegment(1, 0)
                        .ReceivesRoadSegmentPlacementEvent(1, 0)
                        .ThenVerifyPlayerState()
                            .VictoryPoints(4)
                        .EndPlayerVerification()
                        .ThenEndTurn()
                        .ReceivesStartTurnEvent(3, 3).ThenPlaceRoadSegment(0, 8)
                        .ReceivesRoadSegmentPlacementEvent(0, 8)
                        .ThenVerifyPlayerState()
                            .VictoryPoints(4)
                        .EndPlayerVerification()
                        .DidNotReceiveEventOfType<GameWinEvent>()
                    .WhenPlayer(Babara)
                        .ReceivesStartTurnEvent(3, 3).ThenEndTurn()
                    .WhenPlayer(Charlie)
                        .ReceivesStartTurnEvent(3, 3).ThenEndTurn()
                    .WhenPlayer(Dana)
                        .ReceivesStartTurnEvent(3, 3).ThenEndTurn()
                    .Run(this.logDirectory);
            }
            finally
            {
                this.AttachReports();
            }
        }

        [Test]
        public void PlayerWithLongestRoadDoesNotGetMoreVictoryPointsWhenBuildingSubsequentRoadSegmentsInSameTurn()
        {
            try
            {
                this.CompletePlayerInfrastructureSetup()
                    .WithInitialPlayerSetupFor(Adam, Resources(ResourceClutch.RoadSegment * 5))
                    .WhenPlayer(Adam)
                        .ReceivesStartTurnEvent(3, 3).ThenPlaceRoadSegment(4, 3)
                        .ReceivesRoadSegmentPlacementEvent(4, 3).ThenPlaceRoadSegment(3, 2)
                        .ReceivesRoadSegmentPlacementEvent(3, 2).ThenPlaceRoadSegment(2, 1)
                        .ReceivesRoadSegmentPlacementEvent(2, 1).ThenPlaceRoadSegment(1, 0)
                        .ReceivesRoadSegmentPlacementEvent(1, 0)
                        .ThenVerifyPlayerState()
                            .VictoryPoints(4)
                        .EndPlayerVerification()
                        .ThenPlaceRoadSegment(0, 8)
                        .ReceivesRoadSegmentPlacementEvent(0, 8)
                        .ThenVerifyPlayerState()
                            .VictoryPoints(4)
                        .EndPlayerVerification()
                    .Run(this.logDirectory);
            }
            finally
            {
                this.AttachReports();
            }
        }

        [Test]
        public void PlayerWithLongestRoadDoesNotRaiseEventWhenBuildingSubsequentRoadSegments()
        {
            try
            {
                this.CompletePlayerInfrastructureSetup()
                    .WithInitialPlayerSetupFor(Adam, Resources(ResourceClutch.RoadSegment * 5))
                    .WhenPlayer(Adam)
                        .ReceivesStartTurnEvent(3, 3).ThenPlaceRoadSegment(4, 3)
                        .ReceivesRoadSegmentPlacementEvent(4, 3).ThenPlaceRoadSegment(3, 2)
                        .ReceivesRoadSegmentPlacementEvent(3, 2).ThenPlaceRoadSegment(2, 1)
                        .ReceivesRoadSegmentPlacementEvent(2, 1).ThenPlaceRoadSegment(1, 0)
                        .ReceivesRoadSegmentPlacementEvent(1, 0)
                        .ThenPlaceRoadSegment(0, 8)
                        .ReceivesRoadSegmentPlacementEvent(0, 8)
                        .DidNotReceiveEventOfTypeAfterCount<LongestRoadBuiltEvent>(1)
                    .Run(this.logDirectory);
            }
            finally
            {
                this.AttachReports();
            }
        }

        [Test]
        [TestCase(6u, 10u)]
        [TestCase(7u, 11u)]
        public void PlayerWithEightOrNinePointsGainsLongestRoadAndWins(uint initialVP, uint winningVP)
        {
            try
            {
                var longestRoadLocations = new uint[] { 12, 4, 3, 2, 1, 0 };
                this.CompletePlayerInfrastructureSetup()
                    // initialVP + 2 settlements placed as part of startup = 8 or 9
                    .WithInitialPlayerSetupFor(Adam, VictoryPoints(initialVP), Resources(ResourceClutch.RoadSegment * 4)) 
                    .WhenPlayer(Adam)
                        .ReceivesStartTurnEvent(3, 3).ThenPlaceRoadSegment(4, 3)
                        .ReceivesRoadSegmentPlacementEvent(4, 3).ThenPlaceRoadSegment(3, 2)
                        .ReceivesRoadSegmentPlacementEvent(3, 2).ThenPlaceRoadSegment(2, 1)
                        .ReceivesRoadSegmentPlacementEvent(2, 1).ThenPlaceRoadSegment(1, 0)
                        .ReceivesRoadSegmentPlacementEvent(1, 0)
                        .ReceivesLongestRoadChangedEvent(longestRoadLocations)
                        .ReceivesPlayerWonEvent(winningVP)
                    .WhenPlayer(Babara)
                        .ReceivesLongestRoadChangedEvent(longestRoadLocations, Adam)
                        .ReceivesPlayerWonEvent(winningVP, Adam)
                    .WhenPlayer(Charlie)
                        .ReceivesLongestRoadChangedEvent(longestRoadLocations, Adam)
                        .ReceivesPlayerWonEvent(winningVP, Adam)
                    .WhenPlayer(Dana)
                        .ReceivesLongestRoadChangedEvent(longestRoadLocations, Adam)
                        .ReceivesPlayerWonEvent(winningVP, Adam)
                    .Run(this.logDirectory);
            }
            finally
            {
                this.AttachReports();
            }
        }

        [Test]
        public void PlayersBuildRoadSegmentsLongestRoadEventFiresCorrectly()
        {
            try
            {
                var firstLongestRoadLocations = new uint[] { 12, 4, 3, 2, 1, 0 };
                var secondLongestRoadLocations = new uint[] { 43, 44, 45, 53, 52, 51, 50 };
                var thirdLongestRoadLocations = new uint[] { 12, 4, 3, 2, 1, 0, 8, 9 };
                this.CompletePlayerInfrastructureSetup()
                    .WithInitialPlayerSetupFor(Adam, Resources(ResourceClutch.RoadSegment * 6))
                    .WithInitialPlayerSetupFor(Babara, Resources(ResourceClutch.RoadSegment * 5))
                    .WhenPlayer(Adam)
                        .ReceivesStartTurnEvent(3, 3).ThenPlaceRoadSegment(4, 3)
                        .ReceivesRoadSegmentPlacementEvent(4, 3).ThenPlaceRoadSegment(3, 2)
                        .ReceivesRoadSegmentPlacementEvent(3, 2).ThenPlaceRoadSegment(2, 1)
                        .ReceivesRoadSegmentPlacementEvent(2, 1).ThenPlaceRoadSegment(1, 0)
                        .ReceivesRoadSegmentPlacementEvent(1, 0)
                        .ReceivesLongestRoadChangedEvent(firstLongestRoadLocations)
                        .ThenVerifyPlayerState()
                            .VictoryPoints(4)
                        .EndPlayerVerification().ThenEndTurn()
                        .ReceivesLongestRoadChangedEvent(secondLongestRoadLocations, Babara, Adam)
                        .ThenVerifyPlayerState()
                            .VictoryPoints(2)
                        .EndPlayerVerification()
                        .ReceivesStartTurnEvent(3, 3).ThenPlaceRoadSegment(0, 8)
                        .ReceivesRoadSegmentPlacementEvent(0, 8).ThenPlaceRoadSegment(8, 9)
                        .ReceivesRoadSegmentPlacementEvent(8, 9)
                        .ReceivesLongestRoadChangedEvent(thirdLongestRoadLocations, Adam, Babara)
                        .ThenVerifyPlayerState()
                            .VictoryPoints(4)
                        .EndPlayerVerification().ThenEndTurn()
                    .WhenPlayer(Babara)
                        .ReceivesLongestRoadChangedEvent(firstLongestRoadLocations, Adam)
                        .ReceivesStartTurnEvent(3, 3).ThenPlaceRoadSegment(44, 45)
                        .ReceivesRoadSegmentPlacementEvent(44, 45).ThenPlaceRoadSegment(45, 53)
                        .ReceivesRoadSegmentPlacementEvent(45, 53).ThenPlaceRoadSegment(53, 52)
                        .ReceivesRoadSegmentPlacementEvent(53, 52).ThenPlaceRoadSegment(52, 51)
                        .ReceivesRoadSegmentPlacementEvent(52, 51).ThenPlaceRoadSegment(51, 50)
                        .ReceivesRoadSegmentPlacementEvent(51, 50)
                        .ReceivesLongestRoadChangedEvent(secondLongestRoadLocations, Babara, Adam)
                        .ThenVerifyPlayerState()
                            .VictoryPoints(4)
                        .EndPlayerVerification().ThenEndTurn()
                        .ReceivesLongestRoadChangedEvent(thirdLongestRoadLocations, Adam, Babara)
                        .ThenVerifyPlayerState()
                            .VictoryPoints(2)
                        .EndPlayerVerification()
                        .ReceivesStartTurnEvent(3, 3).ThenQuitGame()
                    .WhenPlayer(Charlie)
                        .ReceivesLongestRoadChangedEvent(firstLongestRoadLocations, Adam).ThenDoNothing()
                        .ReceivesLongestRoadChangedEvent(secondLongestRoadLocations, Babara, Adam).ThenDoNothing()
                        .ReceivesStartTurnEvent(3, 3).ThenEndTurn()
                        .ReceivesLongestRoadChangedEvent(thirdLongestRoadLocations, Adam, Babara)
                        .ReceivesStartTurnEvent(3, 3).ThenQuitGame()
                    .WhenPlayer(Dana)
                        .ReceivesLongestRoadChangedEvent(firstLongestRoadLocations, Adam).ThenDoNothing()
                        .ReceivesLongestRoadChangedEvent(secondLongestRoadLocations, Babara, Adam).ThenDoNothing()
                        .ReceivesStartTurnEvent(3, 3).ThenEndTurn()
                        .ReceivesLongestRoadChangedEvent(thirdLongestRoadLocations, Adam, Babara)
                        .ReceivesStartTurnEvent(3, 3).ThenQuitGame()
                    .Run(this.logDirectory);
            }
            finally
            {
                this.AttachReports();
            }
        }

        [Test]
        public void PlayerWithLargestArmyDoesNotRaiseEventWhenPlayingSubsequentKnight()
        {
            try
            {
                this.CompletePlayerInfrastructureSetup()
                    .WithInitialPlayerSetupFor(Adam, KnightCard(4))
                    .WhenPlayer(Adam)
                        .ReceivesStartTurnEvent(3, 3)
                            .ThenPlayKnightCard(4)
                            .ReceivesKnightCardPlayedEvent(4)
                        .ThenEndTurn()
                        .ReceivesStartTurnEvent(3, 3)
                            .ThenPlayKnightCard(0)
                            .ReceivesKnightCardPlayedEvent(0)
                        .ThenEndTurn()
                        .ReceivesStartTurnEvent(3, 3)
                            .ThenPlayKnightCard(4)
                            .ReceivesKnightCardPlayedEvent(4)
                        .ThenEndTurn()
                        .ReceivesStartTurnEvent(3, 3)
                            .ThenPlayKnightCard(0)
                            .ReceivesKnightCardPlayedEvent(0)
                        .ThenEndTurn()
                        .DidNotReceiveEventOfTypeAfterCount<LargestArmyChangedEvent>(1)
                    .WhenPlayer(Babara)
                        .ReceivesStartTurnEvent(3, 3).ThenEndTurn()
                        .ReceivesStartTurnEvent(3, 3).ThenEndTurn()
                        .ReceivesStartTurnEvent(3, 3).ThenEndTurn()
                        .ReceivesStartTurnEvent(3, 3).ThenQuitGame()
                    .WhenPlayer(Charlie)
                        .ReceivesStartTurnEvent(3, 3).ThenEndTurn()
                        .ReceivesStartTurnEvent(3, 3).ThenEndTurn()
                        .ReceivesStartTurnEvent(3, 3).ThenEndTurn()
                        .ReceivesStartTurnEvent(3, 3).ThenQuitGame()
                    .WhenPlayer(Dana)
                        .ReceivesStartTurnEvent(3, 3).ThenEndTurn()
                        .ReceivesStartTurnEvent(3, 3).ThenEndTurn()
                        .ReceivesStartTurnEvent(3, 3).ThenEndTurn()
                        .ReceivesStartTurnEvent(3, 3).ThenQuitGame()
                    .Run(this.logDirectory);
            }
            finally
            {
                this.AttachReports();
            }
        }

        [Test]
        public void PlayerWithLargestArmyDoesNotGetMoreVictoryPointsWhenPlayingSubsequentKnight()
        {
            try
            {
                this.CompletePlayerInfrastructureSetup()
                    .WithCustomResourceCollection()
                    .WithInitialPlayerSetupFor(Adam, KnightCard(4))
                    .WhenPlayer(Adam)
                        .ReceivesStartTurnEvent(3, 3)
                            .ThenPlayKnightCard(4).ReceivesKnightCardPlayedEvent(4)
                        .ThenEndTurn()
                        .ReceivesStartTurnEvent(3, 3)
                            .ThenPlayKnightCard(0).ReceivesKnightCardPlayedEvent(0)
                        .ThenEndTurn()
                        .ReceivesStartTurnEvent(3, 3)
                            .ThenPlayKnightCard(4).ReceivesKnightCardPlayedEvent(4)
                            .ThenVerifyPlayerState()
                                .VictoryPoints(4)
                            .EndPlayerVerification()
                        .ThenEndTurn()
                        .ReceivesStartTurnEvent(3, 3)
                            .ThenPlayKnightCard(0).ReceivesKnightCardPlayedEvent(0)
                            .ThenVerifyPlayerState()
                                .VictoryPoints(4)
                            .EndPlayerVerification()
                        .ThenEndTurn()
                    .WhenPlayer(Babara)
                        .ReceivesStartTurnEvent(3, 3).ThenEndTurn()
                        .ReceivesStartTurnEvent(3, 3).ThenEndTurn()
                        .ReceivesStartTurnEvent(3, 3).ThenEndTurn()
                        .ReceivesStartTurnEvent(3, 3).ThenQuitGame()
                    .WhenPlayer(Charlie)
                        .ReceivesStartTurnEvent(3, 3).ThenEndTurn()
                        .ReceivesStartTurnEvent(3, 3).ThenEndTurn()
                        .ReceivesStartTurnEvent(3, 3).ThenEndTurn()
                        .ReceivesStartTurnEvent(3, 3).ThenQuitGame()
                    .WhenPlayer(Dana)
                        .ReceivesStartTurnEvent(3, 3).ThenEndTurn()
                        .ReceivesStartTurnEvent(3, 3).ThenEndTurn()
                        .ReceivesStartTurnEvent(3, 3).ThenEndTurn()
                        .ReceivesStartTurnEvent(3, 3).ThenQuitGame()
                    .Run(this.logDirectory);
            }
            finally
            {
                this.AttachReports();
            }
        }

        [Test]
        public void PlayerWithLargestArmyDoesNotGetMoreVictoryPointsWhenPlayingSubsequentKnightInSameTurn()
        {
            try
            {
                this.CompletePlayerInfrastructureSetup()
                    .WithInitialPlayerSetupFor(Adam, KnightCard(4))
                    .WhenPlayer(Adam)
                        .ReceivesStartTurnEvent(3, 3)
                            .ThenPlayKnightCard(4).ReceivesKnightCardPlayedEvent(4)
                        .ThenEndTurn()
                        .ReceivesStartTurnEvent(3, 3)
                            .ThenPlayKnightCard(0)
                            .ReceivesKnightCardPlayedEvent(0)
                        .ThenEndTurn()
                        .ReceivesStartTurnEvent(3, 3)
                            .ThenPlayKnightCard(4)
                            .ReceivesKnightCardPlayedEvent(4)
                            .ThenVerifyPlayerState()
                                .VictoryPoints(4)
                            .EndPlayerVerification()
                        .ThenEndTurn()
                        .ReceivesStartTurnEvent(3, 3)
                            .ThenPlayKnightCard(0)
                            .ReceivesKnightCardPlayedEvent(0)
                            .ThenVerifyPlayerState()
                                .VictoryPoints(4)
                            .EndPlayerVerification()
                        .ThenEndTurn()
                    .WhenPlayer(Babara)
                        .ReceivesStartTurnEvent(3, 3).ThenEndTurn()
                        .ReceivesStartTurnEvent(3, 3).ThenEndTurn()
                        .ReceivesStartTurnEvent(3, 3).ThenEndTurn()
                        .ReceivesStartTurnEvent(3, 3).ThenQuitGame()
                    .WhenPlayer(Charlie)
                        .ReceivesStartTurnEvent(3, 3).ThenEndTurn()
                        .ReceivesStartTurnEvent(3, 3).ThenEndTurn()
                        .ReceivesStartTurnEvent(3, 3).ThenEndTurn()
                        .ReceivesStartTurnEvent(3, 3).ThenQuitGame()
                    .WhenPlayer(Dana)
                        .ReceivesStartTurnEvent(3, 3).ThenEndTurn()
                        .ReceivesStartTurnEvent(3, 3).ThenEndTurn()
                        .ReceivesStartTurnEvent(3, 3).ThenEndTurn()
                        .ReceivesStartTurnEvent(3, 3).ThenQuitGame()
                    .Run(this.logDirectory);
            }
            finally
            {
                this.AttachReports();
            }
        }

        [Test]
        [TestCase(DevelopmentCardTypes.Knight)]
        [TestCase(DevelopmentCardTypes.Monopoly)]
        [TestCase(DevelopmentCardTypes.RoadBuilding)]
        [TestCase(DevelopmentCardTypes.YearOfPlenty)]
        public void PlayerTriesToPlayMoreThanOneDevelopmentCardDuringTurn(DevelopmentCardTypes cardType)
        {
            try
            {
                var scenarioRunner = this.CompletePlayerInfrastructureSetup()
                    .WithInitialPlayerSetupFor(Adam, KnightCard(2), MonopolyCard(2), RoadBuildingCard(2), YearOfPlentyCard(2))
                    .WhenPlayer(Adam)
                        .ReceivesStartTurnEvent(3, 3).ThenPlayKnightCard(4)
                        .ReceivesKnightCardPlayedEvent(4);

                switch (cardType)
                {
                    case DevelopmentCardTypes.Knight: scenarioRunner.ThenPlayKnightCard(0); break;
                    case DevelopmentCardTypes.Monopoly: scenarioRunner.ThenPlayMonopolyCard(ResourceTypes.Brick); break;
                    case DevelopmentCardTypes.RoadBuilding: scenarioRunner.ThenPlayRoadBuildingCard(4, 3, 3, 2); break;
                    case DevelopmentCardTypes.YearOfPlenty: scenarioRunner.ThenPlayYearOfPlentyCard(ResourceTypes.Brick, ResourceTypes.Brick); break;
                    default: throw new ArgumentException($"'{cardType}' not recognised", "cardType");
                }

                scenarioRunner.ReceivesGameErrorEvent(ErrorCodes.CannotPlayDevelopmentCard, "Cannot play more than one development card per turn")
                    .WhenPlayer(Babara)
                        .DidNotReceiveEventOfType<GameErrorEvent>()
                    .WhenPlayer(Charlie)
                        .DidNotReceiveEventOfType<GameErrorEvent>()
                    .WhenPlayer(Dana)
                        .DidNotReceiveEventOfType<GameErrorEvent>()
                    .Run(this.logDirectory);
            }
            finally
            {
                this.AttachReports();
            }
        }

        [Test]
        [TestCase(DevelopmentCardTypes.Knight, "No Knight card owned that can be played this turn")]
        [TestCase(DevelopmentCardTypes.Monopoly, "No Monopoly card owned that can be played this turn")]
        [TestCase(DevelopmentCardTypes.RoadBuilding, "No Road building card owned that can be played this turn")]
        [TestCase(DevelopmentCardTypes.YearOfPlenty, "No Year of Plenty card owned that can be played this turn")]
        public void PlayerTriesToPlayDevelopmentCardAfterBuyingItInSameTurn(DevelopmentCardTypes cardType, string expectedErrorMessage)
        {
            try
            {
                var scenarioRunner = this.CompletePlayerInfrastructureSetup()
                    .WithInitialPlayerSetupFor(Adam, Resources(ResourceClutch.DevelopmentCard))
                    .WhenPlayer(Adam)
                        .ReceivesStartTurnEvent(3, 3).ThenBuyDevelopmentCard()
                        .ReceivesDevelopmentCardBoughtEvent(cardType);

                switch(cardType)
                {
                    case DevelopmentCardTypes.Knight: scenarioRunner.ThenPlayKnightCard(4); break;
                    case DevelopmentCardTypes.Monopoly: scenarioRunner.ThenPlayMonopolyCard(ResourceTypes.Brick); break;
                    case DevelopmentCardTypes.RoadBuilding: scenarioRunner.ThenPlayRoadBuildingCard(4, 3, 3, 2); break;
                    case DevelopmentCardTypes.YearOfPlenty: scenarioRunner.ThenPlayYearOfPlentyCard(ResourceTypes.Brick, ResourceTypes.Brick); break;
                    default: throw new ArgumentException($"'{cardType}' not recognised", "cardType");
                }

                scenarioRunner
                    .ReceivesGameErrorEvent(ErrorCodes.NoDevelopmentCardCanBePlayed, expectedErrorMessage)
                    .WhenPlayer(Babara)
                        .DidNotReceiveEventOfType<GameErrorEvent>()
                    .WhenPlayer(Charlie)
                        .DidNotReceiveEventOfType<GameErrorEvent>()
                    .WhenPlayer(Dana)
                        .DidNotReceiveEventOfType<GameErrorEvent>()
                    .Run(this.logDirectory);
            }
            finally
            {
                this.AttachReports();
            }
        }

        private static CollectedResourcesBuilder CreateExpectedCollectedResources()
        {
            return new CollectedResourcesBuilder();
        }

        private static IPlayerSetupAction KnightCard(int cardCount = 1) => new KnightCardSetup(cardCount);
        private static IPlayerSetupAction MonopolyCard(int cardCount = 1) => new MonopolyCardSetup(cardCount);
        /// <summary>
        /// Set number of road segments that have been placed.
        /// </summary>
        /// <param name="value">Number of road segments</param>
        /// <returns></returns>
        private static IPlayerSetupAction PlacedRoadSegments(int value) => new PlacedRoadSegmentSetup(value);
        private static IPlayerSetupAction PlacedSettlements(int value) => new PlacedSettlementsSetup(value);
        private static IPlayerSetupAction Resources(ResourceClutch resources) => new ResourceSetup(resources);
        private static IPlayerSetupAction RoadBuildingCard(int cardCount = 1) => new RoadBuildingCardSetup(cardCount);
        private static IPlayerSetupAction VictoryPoints(uint value) => new VictoryPointSetup(value);
        private static IPlayerSetupAction YearOfPlentyCard(int cardCount = 1) => new YearOfPlentyCardSetup(cardCount);

        private void AttachReports()
        {
            var filePath = $"{this.logDirectory}\\Adam.html";
            if (File.Exists(filePath))
                TestContext.AddTestAttachment(filePath);

            filePath = $"{this.logDirectory}\\Barbara.html";
            if (File.Exists(filePath))
                TestContext.AddTestAttachment(filePath);

            filePath = $"{this.logDirectory}\\Charlie.html";
            if (File.Exists(filePath))
                TestContext.AddTestAttachment(filePath);

            filePath = $"{this.logDirectory}\\Dana.html";
            if (File.Exists(filePath))
                TestContext.AddTestAttachment(filePath);

            filePath = $"{this.logDirectory}\\GameServer.log";
            if (File.Exists(filePath))
                TestContext.AddTestAttachment(filePath);
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
            var actionNotRecognisedError = new ScenarioGameErrorEvent(null, (int)ErrorCodes.IllegalPlayerAction, null);
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
    }
}
