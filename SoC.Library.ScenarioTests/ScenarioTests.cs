﻿
using Jabberwocky.SoC.Library;
using Jabberwocky.SoC.Library.GameEvents;
using NUnit.Framework;
using static SoC.Library.ScenarioTests.LocalGameControllerScenarioRunner;

namespace SoC.Library.ScenarioTests
{
    [Category("ScenarioTests")]
    [TestFixture]
    public class ScenarioTests
    {
        const string MainPlayerName = "Player";
        const string FirstOpponentName = "Barbara";
        const string SecondOpponentName = "Charlie";
        const string ThirdOpponentName = "Dana";

        const uint MainPlayerFirstSettlementLocation = 12u;
        const uint FirstOpponentFirstSettlementLocation = 18u;
        const uint SecondOpponentFirstSettlementLocation = 25u;
        const uint ThirdOpponentFirstSettlementLocation = 31u;

        const uint ThirdOpponentSecondSettlementLocation = 33u;
        const uint SecondOpponentSecondSettlementLocation = 35u;
        const uint FirstOpponentSecondSettlementLocation = 43u;
        const uint MainPlayerSecondSettlementLocation = 40u;

        const uint MainPlayerFirstRoadEnd = 4;
        const uint FirstOpponentFirstRoadEnd = 17;
        const uint SecondOpponentFirstRoadEnd = 15;
        const uint ThirdOpponentFirstRoadEnd = 30;

        const uint ThirdOpponentSecondRoadEnd = 32;
        const uint SecondOpponentSecondRoadEnd = 24;
        const uint FirstOpponentSecondRoadEnd = 44;
        const uint MainPlayerSecondRoadEnd = 39;

        [Test]
        public void Scenario_AllPlayersHaveDiceRollEventInTheirTurn()
        {
            var localGameController = this.CreateStandardLocalGameControllerScenarioRunner()
                .DuringPlayerTurn(MainPlayerName, 4, 4).EndTurn()
                .DuringPlayerTurn(FirstOpponentName, 2, 2).EndTurn()
                .DuringPlayerTurn(SecondOpponentName, 1, 1).EndTurn()
                .DuringPlayerTurn(ThirdOpponentName, 2, 2).EndTurn()
                .Build()
                .DiceRollEvent(MainPlayerName, 4, 4)
                .DiceRollEvent(FirstOpponentName, 2, 2)
                .DiceRollEvent(SecondOpponentName, 1, 1)
                .DiceRollEvent(ThirdOpponentName, 2, 2)
                .Run();
        }

        [Test]
        public void Scenario_AllPlayersHaveDiceRollEventInTheirTurn2()
        {
            this.CreateStandardLocalGameControllerScenarioRunner()
                .WithStartingResourcesForPlayer(MainPlayerName, ResourceClutch.DevelopmentCard)
                .WithNoResourceCollection()
                .PlayerTurn(MainPlayerName, 4, 4)
                    .Actions()
                        .BuyDevelopmentCard(DevelopmentCardTypes.Knight)
                        .End()
                    .Events()
                        .DiceRollEvent(4, 4)
                        .BuyDevelopmentCardEvent()
                        .End()
                    .State()
                        .HeldCards(DevelopmentCardTypes.Monopoly)
                        .End()
                    .End()
                .PlayerTurn(FirstOpponentName, 1, 2)
                    .Events()
                        .DiceRollEvent(1, 2)
                        .End()
                    .End()
                .PlayerTurn(SecondOpponentName, 3, 4)
                    .Events()
                        .DiceRollEvent(3, 4)
                        .End()
                    .End()
                .PlayerTurn(ThirdOpponentName, 5, 6)
                    .Events()
                        .DiceRollEvent(5, 6)
                        .End()
                    .End()
                .Build()
                .Run();
        }

        [Test]
        public void Scenario_AllPlayersCollectResourcesAsPartOfFirstTurnStart()
        {
            var localGameController = this.CreateStandardLocalGameControllerScenarioRunner()
                .DuringPlayerTurn(MainPlayerName, 1, 5).EndTurn()
                .Build()
                .ResourcesCollectedEvent(FirstOpponentName, FirstOpponentFirstSettlementLocation, ResourceClutch.OneOre)
                .StartResourcesCollectedEvent(SecondOpponentName)
                    .AddResourceCollection(SecondOpponentFirstSettlementLocation, ResourceClutch.OneLumber)
                    .AddResourceCollection(SecondOpponentSecondSettlementLocation, ResourceClutch.OneLumber)
                    .FinishResourcesCollectedEvent()
                .ResourcesCollectedEvent(ThirdOpponentName, ThirdOpponentFirstSettlementLocation, ResourceClutch.OneOre)
                .Run();
        }

        [Test]
        public void Scenario_AllPlayersCollectResourcesAsPartOfTurnStartAfterComputerPlayersCompleteTheirTurns()
        {
            var localGameController = this.CreateStandardLocalGameControllerScenarioRunner()
                .DuringPlayerTurn(MainPlayerName, 1, 2).EndTurn()
                .DuringPlayerTurn(FirstOpponentName, 4, 4).EndTurn()
                .DuringPlayerTurn(SecondOpponentName, 4, 4).EndTurn()
                .DuringPlayerTurn(ThirdOpponentName, 4, 4).EndTurn()
                .DuringPlayerTurn(MainPlayerName, 3, 3).EndTurn()
                .Build()
                .IgnoredEvents(typeof(ResourcesCollectedEvent), 4)
                .ResourcesCollectedEvent(SecondOpponentName, SecondOpponentSecondSettlementLocation, ResourceClutch.OneOre)
                .ResourcesCollectedEvent(MainPlayerName, MainPlayerFirstSettlementLocation, ResourceClutch.OneBrick)
                .ResourcesCollectedEvent(FirstOpponentName, FirstOpponentSecondSettlementLocation, ResourceClutch.OneGrain)
                .ResourcesCollectedEvent(MainPlayerName, MainPlayerFirstSettlementLocation, ResourceClutch.OneBrick)
                .ResourcesCollectedEvent(FirstOpponentName, FirstOpponentSecondSettlementLocation, ResourceClutch.OneGrain)
                .ResourcesCollectedEvent(MainPlayerName, MainPlayerFirstSettlementLocation, ResourceClutch.OneBrick)
                .ResourcesCollectedEvent(FirstOpponentName, FirstOpponentSecondSettlementLocation, ResourceClutch.OneGrain)
                .ResourcesCollectedEvent(FirstOpponentName, FirstOpponentFirstSettlementLocation, ResourceClutch.OneOre)
                .StartResourcesCollectedEvent(SecondOpponentName)
                    .AddResourceCollection(SecondOpponentFirstSettlementLocation, ResourceClutch.OneLumber)
                    .AddResourceCollection(SecondOpponentSecondSettlementLocation, ResourceClutch.OneLumber)
                    .FinishResourcesCollectedEvent()
                .ResourcesCollectedEvent(ThirdOpponentName, ThirdOpponentFirstSettlementLocation, ResourceClutch.OneOre)
                .Run();
        }

        [Test]
        public void Scenario_ComputerPlayerBuildsSettlement()
        {
            this.CreateStandardLocalGameControllerScenarioRunner()
                .DuringPlayerTurn(MainPlayerName, 2, 3).EndTurn()
                .DuringPlayerTurn(FirstOpponentName, 2, 2).EndTurn()
                .DuringPlayerTurn(SecondOpponentName, 1, 3).EndTurn()
                .DuringPlayerTurn(ThirdOpponentName, 2, 3).EndTurn()
                .DuringPlayerTurn(MainPlayerName, 2, 3).EndTurn()
                .DuringPlayerTurn(FirstOpponentName, 2, 2).EndTurn()
                .DuringPlayerTurn(SecondOpponentName, 1, 3).EndTurn()
                .DuringPlayerTurn(ThirdOpponentName, 2, 3)
                    .BuildRoad(33, 22)
                    .BuildRoad(22, 23)
                    .BuildSettlement(23).EndTurn()
                .Build()
                .BuildRoadEvent(ThirdOpponentName, 33, 22)
                .BuildRoadEvent(ThirdOpponentName, 22, 23)
                .BuildSettlementEvent(ThirdOpponentName, 23)
                .Run();
        }

        [Test]
        public void Scenario_ComputerPlayerBuildsSettlementToWin()
        {
            var firstOpponentResources = (ResourceClutch.RoadSegment * 5) + (ResourceClutch.Settlement * 3) + (ResourceClutch.City * 3);
            this.CreateStandardLocalGameControllerScenarioRunner(ResourceClutch.Zero, firstOpponentResources, ResourceClutch.Zero, ResourceClutch.Zero)
                .DuringPlayerTurn(MainPlayerName, 4, 4).EndTurn()
                .DuringPlayerTurn(FirstOpponentName, 3, 3)
                    .BuildRoad(17, 7).BuildRoad(7, 8).BuildRoad(8, 0).BuildRoad(0, 1).BuildRoad(8, 9)
                    .BuildSettlement(7).BuildSettlement(0)
                    .BuildCity(7).BuildCity(0).BuildCity(18)
                    .BuildSettlement(9).EndTurn()
                .Build()
                    .DiceRollEvent(MainPlayerName, 4, 4)
                    .DiceRollEvent(FirstOpponentName, 3, 3)
                    .BuildRoadEvent(FirstOpponentName, 17, 7).BuildRoadEvent(FirstOpponentName, 7, 8).BuildRoadEvent(FirstOpponentName, 8, 0).BuildRoadEvent(FirstOpponentName, 0, 1).BuildRoadEvent(FirstOpponentName, 8, 9)
                    .BuildSettlementEvent(FirstOpponentName, 7).BuildSettlementEvent(FirstOpponentName, 0)
                    .BuildCityEvent(FirstOpponentName, 7).BuildCityEvent(FirstOpponentName, 0).BuildCityEvent(FirstOpponentName, 18)
                    .BuildSettlementEvent(FirstOpponentName, 9)
                    .GameWinEvent(FirstOpponentName, 10)
                .Run();
        }

        [Test]
        public void Scenario_ComputerPlayerBuildsCity()
        {
            this.CreateStandardLocalGameControllerScenarioRunner()
                .DuringPlayerTurn(MainPlayerName, 3, 3).EndTurn()
                .DuringPlayerTurn(FirstOpponentName, 3, 3).EndTurn()
                .DuringPlayerTurn(SecondOpponentName, 3, 3).EndTurn()
                .DuringPlayerTurn(ThirdOpponentName, 2, 3).EndTurn()
                .DuringPlayerTurn(MainPlayerName, 5, 6).EndTurn()
                .DuringPlayerTurn(FirstOpponentName, 5, 6).EndTurn()
                .DuringPlayerTurn(SecondOpponentName, 3, 3).EndTurn()
                .DuringPlayerTurn(ThirdOpponentName, 3, 3)
                    .BuildCity(33).EndTurn()
                .Build()
                .BuildCityEvent(ThirdOpponentName, 33)
                .Run();
        }

        [Test]
        public void Scenario_ComputerPlayerBuildsCityToWin()
        {
            var firstOpponentResources = (ResourceClutch.RoadSegment * 5) + (ResourceClutch.Settlement * 4) + (ResourceClutch.City * 4);

            this.CreateStandardLocalGameControllerScenarioRunner(ResourceClutch.Zero, firstOpponentResources, ResourceClutch.Zero, ResourceClutch.Zero)
                .DuringPlayerTurn(MainPlayerName, 4, 4).EndTurn()
                .DuringPlayerTurn(FirstOpponentName, 3, 3)
                    .BuildRoad(17, 7).BuildRoad(17, 16).BuildRoad(43, 51).BuildRoad(51, 50).BuildRoad(51, 52)
                    .BuildSettlement(7).BuildSettlement(16).BuildSettlement(50).BuildSettlement(52)
                    .BuildCity(7).BuildCity(16).BuildCity(50).BuildCity(52).EndTurn()
                .Build()
                    .DiceRollEvent(MainPlayerName, 4, 4)
                    .DiceRollEvent(FirstOpponentName, 3, 3)
                    .BuildRoadEvent(FirstOpponentName, 17, 7).BuildRoadEvent(FirstOpponentName, 17, 16)
                    .BuildRoadEvent(FirstOpponentName, 43, 51).BuildRoadEvent(FirstOpponentName, 51, 50)
                    .BuildRoadEvent(FirstOpponentName, 51, 52)
                    .BuildSettlementEvent(FirstOpponentName, 7).BuildSettlementEvent(FirstOpponentName, 16)
                    .BuildSettlementEvent(FirstOpponentName, 50).BuildSettlementEvent(FirstOpponentName, 52)
                    .BuildCityEvent(FirstOpponentName, 7).BuildCityEvent(FirstOpponentName, 16)
                    .BuildCityEvent(FirstOpponentName, 50).BuildCityEvent(FirstOpponentName, 52)
                    .GameWinEvent(FirstOpponentName, 10)
                .Run();
        }

        [Test]
        public void Scenario_ComputerPlayerHasEightVictoryPointsAndBuildsLargestArmyToWin()
        {
            var firstOpponentResources = (ResourceClutch.RoadSegment * 2) + (ResourceClutch.Settlement * 2) + 
                (ResourceClutch.City * 4) + (ResourceClutch.DevelopmentCard * 3);

            this.CreateStandardLocalGameControllerScenarioRunner(ResourceClutch.Zero, firstOpponentResources, ResourceClutch.Zero, ResourceClutch.Zero)
                .DuringPlayerTurn(MainPlayerName, 4, 4).EndTurn()
                .DuringPlayerTurn(FirstOpponentName, 3, 3)
                    .BuildRoad(17, 7).BuildRoad(17, 16)
                    .BuildSettlement(7).BuildSettlement(16)
                    .BuildCity(7).BuildCity(16).BuildCity(18).BuildCity(43)
                    .BuyDevelopmentCard(DevelopmentCardTypes.Knight).BuyDevelopmentCard(DevelopmentCardTypes.Knight).BuyDevelopmentCard(DevelopmentCardTypes.Knight)
                    .EndTurn()
                .DuringPlayerTurn(SecondOpponentName, 3, 3).EndTurn()
                .DuringPlayerTurn(ThirdOpponentName, 3, 3).EndTurn()
                .DuringPlayerTurn(MainPlayerName, 3, 3).EndTurn()
                .DuringPlayerTurn(FirstOpponentName, 3, 3)
                    .PlayKnightCard(4).EndTurn()
                .DuringPlayerTurn(SecondOpponentName, 3, 3).EndTurn()
                .DuringPlayerTurn(ThirdOpponentName, 3, 3).EndTurn()
                .DuringPlayerTurn(MainPlayerName, 3, 3).EndTurn()
                .DuringPlayerTurn(FirstOpponentName, 3, 3)
                    .PlayKnightCard(0).EndTurn()
                .DuringPlayerTurn(SecondOpponentName, 3, 3).EndTurn()
                .DuringPlayerTurn(ThirdOpponentName, 3, 3).EndTurn()
                .DuringPlayerTurn(MainPlayerName, 3, 3).EndTurn()
                .DuringPlayerTurn(FirstOpponentName, 3, 3)
                    .PlayKnightCard(4).EndTurn()
                .Build()
                    .BuildRoadEvent(FirstOpponentName, 17, 7).BuildRoadEvent(FirstOpponentName, 17, 16)
                    .BuildSettlementEvent(FirstOpponentName, 7).BuildSettlementEvent(FirstOpponentName, 16)
                    .BuildCityEvent(FirstOpponentName, 7).BuildCityEvent(FirstOpponentName, 16).BuildCityEvent(FirstOpponentName, 18).BuildCityEvent(FirstOpponentName, 43)
                    .LargestArmyChangedEvent(FirstOpponentName)
                    .GameWinEvent(FirstOpponentName, 10)
                .Run();
        }

        [Test]
        public void Scenario_ComputerPlayerHasEightVictoryPointsAndBuildsLongestRoadToWin()
        {
            var firstOpponentResources = (ResourceClutch.RoadSegment * 4) + (ResourceClutch.Settlement * 2) +
                (ResourceClutch.City * 4);

            this.CreateStandardLocalGameControllerScenarioRunner(ResourceClutch.Zero, firstOpponentResources, ResourceClutch.Zero, ResourceClutch.Zero)
                .DuringPlayerTurn(MainPlayerName, 4, 4).EndTurn()
                .DuringPlayerTurn(FirstOpponentName, 3, 3)
                    .BuildRoad(17, 7).BuildRoad(7, 8).BuildRoad(8, 0)
                    .BuildSettlement(7).BuildSettlement(0)
                    .BuildCity(7).BuildCity(0).BuildCity(18).BuildCity(43)
                    .BuildRoad(0, 1)
                    .EndTurn()
                .Build()
                    .BuildRoadEvent(FirstOpponentName, 17, 7).BuildRoadEvent(FirstOpponentName, 7, 8).BuildRoadEvent(FirstOpponentName, 8, 0)
                    .BuildSettlementEvent(FirstOpponentName, 7).BuildSettlementEvent(FirstOpponentName, 0)
                    .BuildCityEvent(FirstOpponentName, 7).BuildCityEvent(FirstOpponentName, 0).BuildCityEvent(FirstOpponentName, 18).BuildCityEvent(FirstOpponentName, 43)
                    .BuildRoadEvent(FirstOpponentName, 0, 1)
                    .LongestRoadBuiltEvent(FirstOpponentName)
                    .GameWinEvent(FirstOpponentName, 10)
                .Run();
        }

        [Test]
        public void Scenario_ComputerPlayerHasNineVictoryPointsAndBuildsLargestArmyToWin()
        {
            var firstOpponentResources = (ResourceClutch.RoadSegment * 3) + (ResourceClutch.Settlement * 3) +
                (ResourceClutch.City * 4) + (ResourceClutch.DevelopmentCard * 3);

            this.CreateStandardLocalGameControllerScenarioRunner(ResourceClutch.Zero, firstOpponentResources, ResourceClutch.Zero, ResourceClutch.Zero)
                .DuringPlayerTurn(MainPlayerName, 4, 4).EndTurn()
                .DuringPlayerTurn(FirstOpponentName, 3, 3)
                    .BuildRoad(17, 7).BuildRoad(17, 16).BuildRoad(44, 45)
                    .BuildSettlement(7).BuildSettlement(16).BuildSettlement(45)
                    .BuildCity(7).BuildCity(16).BuildCity(18).BuildCity(43)
                    .BuyDevelopmentCard(DevelopmentCardTypes.Knight).BuyDevelopmentCard(DevelopmentCardTypes.Knight).BuyDevelopmentCard(DevelopmentCardTypes.Knight)
                    .EndTurn()
                .DuringPlayerTurn(SecondOpponentName, 3, 3).EndTurn()
                .DuringPlayerTurn(ThirdOpponentName, 3, 3).EndTurn()
                .DuringPlayerTurn(MainPlayerName, 3, 3).EndTurn()
                .DuringPlayerTurn(FirstOpponentName, 3, 3)
                    .PlayKnightCard(4).EndTurn()
                .DuringPlayerTurn(SecondOpponentName, 3, 3).EndTurn()
                .DuringPlayerTurn(ThirdOpponentName, 3, 3).EndTurn()
                .DuringPlayerTurn(MainPlayerName, 3, 3).EndTurn()
                .DuringPlayerTurn(FirstOpponentName, 3, 3)
                    .PlayKnightCard(0).EndTurn()
                .DuringPlayerTurn(SecondOpponentName, 3, 3).EndTurn()
                .DuringPlayerTurn(ThirdOpponentName, 3, 3).EndTurn()
                .DuringPlayerTurn(MainPlayerName, 3, 3).EndTurn()
                .DuringPlayerTurn(FirstOpponentName, 3, 3)
                    .PlayKnightCard(4).EndTurn()
                .Build()
                    .BuildRoadEvent(FirstOpponentName, 17, 7).BuildRoadEvent(FirstOpponentName, 17, 16).BuildRoadEvent(FirstOpponentName, 44, 45)
                    .BuildSettlementEvent(FirstOpponentName, 7).BuildSettlementEvent(FirstOpponentName, 16).BuildSettlementEvent(FirstOpponentName, 45)
                    .BuildCityEvent(FirstOpponentName, 7).BuildCityEvent(FirstOpponentName, 16).BuildCityEvent(FirstOpponentName, 18).BuildCityEvent(FirstOpponentName, 43)
                    .LargestArmyChangedEvent(FirstOpponentName)
                    .GameWinEvent(FirstOpponentName, 11)
                .Run();
        }

        [Test]
        public void Scenario_ComputerPlayerHasNineVictoryPointsAndBuildsLongestRoadToWin()
        {
            var firstOpponentResources = (ResourceClutch.RoadSegment * 5) + (ResourceClutch.Settlement * 3) +
                (ResourceClutch.City * 4);

            this.CreateStandardLocalGameControllerScenarioRunner(ResourceClutch.Zero, firstOpponentResources, ResourceClutch.Zero, ResourceClutch.Zero)
                .DuringPlayerTurn(MainPlayerName, 4, 4).EndTurn()
                .DuringPlayerTurn(FirstOpponentName, 3, 3)
                    .BuildRoad(17, 7).BuildRoad(7, 8).BuildRoad(8, 0).BuildRoad(44, 45)
                    .BuildSettlement(7).BuildSettlement(0).BuildSettlement(45)
                    .BuildCity(7).BuildCity(0).BuildCity(18).BuildCity(43)
                    .BuildRoad(0, 1)
                    .EndTurn()
                .Build()
                    .BuildRoadEvent(FirstOpponentName, 17, 7).BuildRoadEvent(FirstOpponentName, 7, 8)
                    .BuildRoadEvent(FirstOpponentName, 8, 0).BuildRoadEvent(FirstOpponentName, 44, 45)
                    .BuildSettlementEvent(FirstOpponentName, 7).BuildSettlementEvent(FirstOpponentName, 0)
                    .BuildSettlementEvent(FirstOpponentName, 45)
                    .BuildCityEvent(FirstOpponentName, 7).BuildCityEvent(FirstOpponentName, 0).BuildCityEvent(FirstOpponentName, 18).BuildCityEvent(FirstOpponentName, 43)
                    .BuildRoadEvent(FirstOpponentName, 0, 1)
                    .LongestRoadBuiltEvent(FirstOpponentName)
                    .GameWinEvent(FirstOpponentName, 11)
                .Run();
        }

        /// <summary>
        /// Test that the transaction between players happens as expected when human plays the knight card and the robber
        /// is moved to a hex populated by two computer players.
        /// </summary>
        [Test]
        public void Scenario_ComputerPlayerLosesResourceWhenPlayerPlaysTheKnightCard()
        {
            var mainPlayerResources = ResourceClutch.DevelopmentCard;
            var firstOpponentResources = ResourceClutch.OneOre;
            this.CreateStandardLocalGameControllerScenarioRunner(mainPlayerResources, firstOpponentResources, ResourceClutch.Zero, ResourceClutch.Zero)
                .WithNoResourceCollection()
                .DuringPlayerTurn(MainPlayerName, 4, 4)
                    .BuyDevelopmentCard(DevelopmentCardTypes.Knight)
                    .EndTurn()
                .DuringPlayerTurn(FirstOpponentName, 3, 3).EndTurn()
                .DuringPlayerTurn(SecondOpponentName, 3, 3).EndTurn()
                .DuringPlayerTurn(ThirdOpponentName, 3, 3).EndTurn()
                .DuringPlayerTurn(MainPlayerName, 4, 4)
                    .PlayKnightCardAndCollectFrom(3, FirstOpponentName, ResourceTypes.Ore)
                    .EndTurn()
                .Build()
                    .BuyDevelopmentCardEvent(MainPlayerName, DevelopmentCardTypes.Knight)
                    .ResourcesGainedEvent(MainPlayerName, FirstOpponentName, ResourceClutch.OneOre)
                .Run();
        }

        /// <summary>
        /// Test that the largest army event is not raised when the player plays knight cards and already has the largest army
        /// </summary>
        [Test]
        public void Scenario_LargestArmyEventOnlyRaisedFirstTimeThatPlayerHasMostKnightCardsPlayed()
        {
            var mainPlayerResources = ResourceClutch.DevelopmentCard * 4;
            this.CreateStandardLocalGameControllerScenarioRunner(mainPlayerResources, ResourceClutch.Zero, ResourceClutch.Zero, ResourceClutch.Zero)
                .DuringPlayerTurn(MainPlayerName, 4, 4)
                    .BuyDevelopmentCard(DevelopmentCardTypes.Knight)
                    .BuyDevelopmentCard(DevelopmentCardTypes.Knight)
                    .BuyDevelopmentCard(DevelopmentCardTypes.Knight)
                    .BuyDevelopmentCard(DevelopmentCardTypes.Knight)
                    .EndTurn()
                .DuringPlayerTurn(FirstOpponentName, 3, 3).EndTurn()
                .DuringPlayerTurn(SecondOpponentName, 3, 3).EndTurn()
                .DuringPlayerTurn(ThirdOpponentName, 3, 3).EndTurn()
                .DuringPlayerTurn(MainPlayerName, 4, 4)
                    .PlayKnightCard(4)
                    .EndTurn()
                .DuringPlayerTurn(FirstOpponentName, 3, 3).EndTurn()
                .DuringPlayerTurn(SecondOpponentName, 3, 3).EndTurn()
                .DuringPlayerTurn(ThirdOpponentName, 3, 3).EndTurn()
                .DuringPlayerTurn(MainPlayerName, 4, 4)
                    .PlayKnightCard(0)
                    .EndTurn()
                .DuringPlayerTurn(FirstOpponentName, 3, 3).EndTurn()
                .DuringPlayerTurn(SecondOpponentName, 3, 3).EndTurn()
                .DuringPlayerTurn(ThirdOpponentName, 3, 3).EndTurn()
                .DuringPlayerTurn(MainPlayerName, 4, 4)
                    .PlayKnightCard(4)
                    .EndTurn()
                .DuringPlayerTurn(FirstOpponentName, 3, 3).EndTurn()
                .DuringPlayerTurn(SecondOpponentName, 3, 3).EndTurn()
                .DuringPlayerTurn(ThirdOpponentName, 3, 3).EndTurn()
                .DuringPlayerTurn(MainPlayerName, 4, 4)
                    .PlayKnightCard(0)
                    .EndTurn()
                .DuringPlayerTurn(FirstOpponentName, 3, 3).EndTurn()
                .Build()
                    .BuyDevelopmentCardEvent(MainPlayerName, DevelopmentCardTypes.Knight)
                    .BuyDevelopmentCardEvent(MainPlayerName, DevelopmentCardTypes.Knight)
                    .BuyDevelopmentCardEvent(MainPlayerName, DevelopmentCardTypes.Knight)
                    .BuyDevelopmentCardEvent(MainPlayerName, DevelopmentCardTypes.Knight)
                    .PlayKnightCardEvent(MainPlayerName)
                    .PlayKnightCardEvent(MainPlayerName)
                    .PlayKnightCardEvent(MainPlayerName)
                    .LargestArmyChangedEvent(MainPlayerName, null, EventPositions.Last)
                    .PlayKnightCardEvent(MainPlayerName)
                .Run();
        }

        /// <summary>
        /// Test that the largest army event is not sent when the opponent plays knight cards and already has the largest army
        /// </summary>
        [Test]
        public void Scenario_LargestArmyEventOnlyReturnedFirstTimeThatOpponentHasMostKnightCardsPlayed()
        {
            var firstOpponentResources = ResourceClutch.DevelopmentCard * 4;
            this.CreateStandardLocalGameControllerScenarioRunner(ResourceClutch.Zero, firstOpponentResources, ResourceClutch.Zero, ResourceClutch.Zero)
                .DuringPlayerTurn(MainPlayerName, 4, 4).EndTurn()
                .DuringPlayerTurn(FirstOpponentName, 3, 3)
                    .BuyDevelopmentCard(DevelopmentCardTypes.Knight)
                    .BuyDevelopmentCard(DevelopmentCardTypes.Knight)
                    .BuyDevelopmentCard(DevelopmentCardTypes.Knight)
                    .BuyDevelopmentCard(DevelopmentCardTypes.Knight)
                    .EndTurn()
                .DuringPlayerTurn(SecondOpponentName, 3, 3).EndTurn()
                .DuringPlayerTurn(ThirdOpponentName, 3, 3).EndTurn()
                .DuringPlayerTurn(MainPlayerName, 4, 4).EndTurn()
                .DuringPlayerTurn(FirstOpponentName, 3, 3)
                    .PlayKnightCard(4)
                    .EndTurn()
                .DuringPlayerTurn(SecondOpponentName, 3, 3).EndTurn()
                .DuringPlayerTurn(ThirdOpponentName, 3, 3).EndTurn()
                .DuringPlayerTurn(MainPlayerName, 4, 4).EndTurn()
                .DuringPlayerTurn(FirstOpponentName, 3, 3)
                    .PlayKnightCard(0)
                    .EndTurn()
                .DuringPlayerTurn(SecondOpponentName, 3, 3).EndTurn()
                .DuringPlayerTurn(ThirdOpponentName, 3, 3).EndTurn()
                .DuringPlayerTurn(MainPlayerName, 4, 4).EndTurn()
                .DuringPlayerTurn(FirstOpponentName, 3, 3)
                    .PlayKnightCard(4)
                    .EndTurn()
                .DuringPlayerTurn(SecondOpponentName, 3, 3).EndTurn()
                .DuringPlayerTurn(ThirdOpponentName, 3, 3).EndTurn()
                .DuringPlayerTurn(MainPlayerName, 4, 4).EndTurn()
                .DuringPlayerTurn(FirstOpponentName, 3, 3)
                    .PlayKnightCard(0)
                    .EndTurn()
                .Build()
                    .BuyDevelopmentCardEvent(FirstOpponentName, DevelopmentCardTypes.Knight)
                    .BuyDevelopmentCardEvent(FirstOpponentName, DevelopmentCardTypes.Knight)
                    .BuyDevelopmentCardEvent(FirstOpponentName, DevelopmentCardTypes.Knight)
                    .BuyDevelopmentCardEvent(FirstOpponentName, DevelopmentCardTypes.Knight)
                    .PlayKnightCardEvent(FirstOpponentName)
                    .PlayKnightCardEvent(FirstOpponentName)
                    .PlayKnightCardEvent(FirstOpponentName)
                    .LargestArmyChangedEvent(FirstOpponentName, null, EventPositions.Last)
                    .PlayKnightCardEvent(FirstOpponentName)
                .Run();
        }

        /// <summary>
        /// Test that the largest army event is raised when the player has played 3 knight cards and 
        /// when the opponent plays more knight cards. Verify the victory point changes.
        /// </summary>
        [Test]
        public void Scenario_LargestArmyEventsRaisedWhenBothPlayerAndOpponentPlayTheMostKnightDevelopmentCards()
        {
            var mainPlayerResources = ResourceClutch.DevelopmentCard * 3;
            var firstOpponentResources = ResourceClutch.DevelopmentCard * 4;
            this.CreateStandardLocalGameControllerScenarioRunner(mainPlayerResources, firstOpponentResources, ResourceClutch.Zero, ResourceClutch.Zero)
                .DuringPlayerTurn(MainPlayerName, 4, 4)
                    .BuyDevelopmentCard(DevelopmentCardTypes.Knight)
                    .BuyDevelopmentCard(DevelopmentCardTypes.Knight)
                    .BuyDevelopmentCard(DevelopmentCardTypes.Knight)
                    .EndTurn()
                .DuringPlayerTurn(FirstOpponentName, 3, 3)
                    .BuyDevelopmentCard(DevelopmentCardTypes.Knight)
                    .BuyDevelopmentCard(DevelopmentCardTypes.Knight)
                    .BuyDevelopmentCard(DevelopmentCardTypes.Knight)
                    .BuyDevelopmentCard(DevelopmentCardTypes.Knight)
                    .EndTurn()
                .DuringPlayerTurn(SecondOpponentName, 3, 3).EndTurn().DuringPlayerTurn(ThirdOpponentName, 3, 3).EndTurn()
                .DuringPlayerTurn(MainPlayerName, 4, 4)
                    .PlayKnightCard(4)
                    .EndTurn()
                .DuringPlayerTurn(FirstOpponentName, 3, 3)
                    .PlayKnightCard(0)
                    .EndTurn()
                .DuringPlayerTurn(SecondOpponentName, 3, 3).EndTurn().DuringPlayerTurn(ThirdOpponentName, 3, 3).EndTurn()
                .DuringPlayerTurn(MainPlayerName, 4, 4)
                    .PlayKnightCard(4)
                    .EndTurn()
                .DuringPlayerTurn(FirstOpponentName, 3, 3)
                    .PlayKnightCard(0)
                    .EndTurn()
                .DuringPlayerTurn(SecondOpponentName, 3, 3).EndTurn().DuringPlayerTurn(ThirdOpponentName, 3, 3).EndTurn()
                .DuringPlayerTurn(MainPlayerName, 4, 4)
                    .PlayKnightCard(4)
                    .EndTurn()
                .DuringPlayerTurn(FirstOpponentName, 3, 3)
                    .PlayKnightCard(0)
                    .EndTurn()
                .DuringPlayerTurn(SecondOpponentName, 3, 3).EndTurn().DuringPlayerTurn(ThirdOpponentName, 3, 3).EndTurn()
                .DuringPlayerTurn(MainPlayerName, 4, 4).EndTurn()
                .DuringPlayerTurn(FirstOpponentName, 3, 3)
                    .PlayKnightCard(4)
                    .EndTurn()
                .Build()
                    .BuyDevelopmentCardEvent(MainPlayerName, DevelopmentCardTypes.Knight)
                    .BuyDevelopmentCardEvent(MainPlayerName, DevelopmentCardTypes.Knight)
                    .BuyDevelopmentCardEvent(MainPlayerName, DevelopmentCardTypes.Knight)
                    .BuyDevelopmentCardEvent(FirstOpponentName, DevelopmentCardTypes.Knight)
                    .BuyDevelopmentCardEvent(FirstOpponentName, DevelopmentCardTypes.Knight)
                    .BuyDevelopmentCardEvent(FirstOpponentName, DevelopmentCardTypes.Knight)
                    .BuyDevelopmentCardEvent(FirstOpponentName, DevelopmentCardTypes.Knight)
                    .PlayKnightCardEvent(MainPlayerName)
                    .PlayKnightCardEvent(FirstOpponentName)
                    .PlayKnightCardEvent(MainPlayerName)
                    .PlayKnightCardEvent(FirstOpponentName)
                    .PlayKnightCardEvent(MainPlayerName)
                    .LargestArmyChangedEvent(MainPlayerName)
                    .ExpectPlayer(MainPlayerName)
                        .VictoryPoints(4).End()
                    .ExpectPlayer(FirstOpponentName)
                        .VictoryPoints(2).End()
                    .PlayKnightCardEvent(FirstOpponentName)
                    .PlayKnightCardEvent(FirstOpponentName)
                    .LargestArmyChangedEvent(FirstOpponentName, MainPlayerName)
                    .ExpectPlayer(MainPlayerName)
                        .VictoryPoints(2).End()
                    .ExpectPlayer(FirstOpponentName)
                        .VictoryPoints(4).End()
                .Run();
        }

        /// <summary>
        /// Test that the player only gets the largest army VP the first time they have the largest army
        /// (until another player has the largest army).
        /// </summary>
        [Test]
        public void Scenario_LargestArmyVictoryPointsOnlyChangedFirstTimeThatPlayerHasMostKnightCardsPlayed()
        {
            var mainPlayerResources = ResourceClutch.DevelopmentCard * 4;
            this.CreateStandardLocalGameControllerScenarioRunner(mainPlayerResources, ResourceClutch.Zero, ResourceClutch.Zero, ResourceClutch.Zero)
                .DuringPlayerTurn(MainPlayerName, 4, 4)
                    .BuyDevelopmentCard(DevelopmentCardTypes.Knight)
                    .BuyDevelopmentCard(DevelopmentCardTypes.Knight)
                    .BuyDevelopmentCard(DevelopmentCardTypes.Knight)
                    .BuyDevelopmentCard(DevelopmentCardTypes.Knight)
                    .EndTurn()
                .DuringPlayerTurn(FirstOpponentName, 3, 3).EndTurn()
                .DuringPlayerTurn(SecondOpponentName, 3, 3).EndTurn()
                .DuringPlayerTurn(ThirdOpponentName, 3, 3).EndTurn()
                .DuringPlayerTurn(MainPlayerName, 4, 4)
                    .PlayKnightCard(4)
                    .EndTurn()
                .DuringPlayerTurn(FirstOpponentName, 3, 3).EndTurn()
                .DuringPlayerTurn(SecondOpponentName, 3, 3).EndTurn()
                .DuringPlayerTurn(ThirdOpponentName, 3, 3).EndTurn()
                .DuringPlayerTurn(MainPlayerName, 4, 4)
                    .PlayKnightCard(0)
                    .EndTurn()
                .DuringPlayerTurn(FirstOpponentName, 3, 3).EndTurn()
                .DuringPlayerTurn(SecondOpponentName, 3, 3).EndTurn()
                .DuringPlayerTurn(ThirdOpponentName, 3, 3).EndTurn()
                .DuringPlayerTurn(MainPlayerName, 4, 4)
                    .PlayKnightCard(4)
                    .EndTurn()
                .DuringPlayerTurn(FirstOpponentName, 3, 3).EndTurn()
                .DuringPlayerTurn(SecondOpponentName, 3, 3).EndTurn()
                .DuringPlayerTurn(ThirdOpponentName, 3, 3).EndTurn()
                .DuringPlayerTurn(MainPlayerName, 4, 4)
                    .PlayKnightCard(0)
                    .EndTurn()
                .Build()
                    .ExpectPlayer(MainPlayerName)
                        .VictoryPoints(4).End()
                .Run();
        }

        [Test]
        public void Scenario_PlayerLosesResourceWhenComputerPlayerPlaysTheKnightCard()
        {
            var mainPlayerResources = ResourceClutch.OneOre;
            var firstOpponentResources = ResourceClutch.DevelopmentCard;
            this.CreateStandardLocalGameControllerScenarioRunner(mainPlayerResources, firstOpponentResources, ResourceClutch.Zero, ResourceClutch.Zero)
                .WithNoResourceCollection()
                .DuringPlayerTurn(MainPlayerName, 4, 4).EndTurn()
                .DuringPlayerTurn(FirstOpponentName, 3, 3)
                    .BuyDevelopmentCard(DevelopmentCardTypes.Knight)
                    .EndTurn()
                .DuringPlayerTurn(SecondOpponentName, 3, 3).EndTurn()
                .DuringPlayerTurn(ThirdOpponentName, 3, 3).EndTurn()
                .DuringPlayerTurn(MainPlayerName, 4, 4).EndTurn()
                .DuringPlayerTurn(FirstOpponentName, 3, 3)
                    .PlayKnightCardAndCollectFrom(3, MainPlayerName, ResourceTypes.Ore)
                    .EndTurn()
                .Build()
                    .BuyDevelopmentCardEvent(FirstOpponentName, DevelopmentCardTypes.Knight)
                    .PlayKnightCardEvent(FirstOpponentName)
                    .ResourcesGainedEvent(FirstOpponentName, MainPlayerName, ResourceClutch.OneOre)
                .Run();
        }

        private LocalGameControllerScenarioRunner CreateStandardLocalGameControllerScenarioRunner()
        {
            return LocalGameController()
                .WithMainPlayer(MainPlayerName)
                .WithComputerPlayer(FirstOpponentName).WithComputerPlayer(SecondOpponentName).WithComputerPlayer(ThirdOpponentName)
                .WithPlayerSetup(MainPlayerName, MainPlayerFirstSettlementLocation, MainPlayerFirstRoadEnd, MainPlayerSecondSettlementLocation, MainPlayerSecondRoadEnd)
                .WithPlayerSetup(FirstOpponentName, FirstOpponentFirstSettlementLocation, FirstOpponentFirstRoadEnd, FirstOpponentSecondSettlementLocation, FirstOpponentSecondRoadEnd)
                .WithPlayerSetup(SecondOpponentName, SecondOpponentFirstSettlementLocation, SecondOpponentFirstRoadEnd, SecondOpponentSecondSettlementLocation, SecondOpponentSecondRoadEnd)
                .WithPlayerSetup(ThirdOpponentName, ThirdOpponentFirstSettlementLocation, ThirdOpponentFirstRoadEnd, ThirdOpponentSecondSettlementLocation, ThirdOpponentSecondRoadEnd)
                .WithTurnOrder(MainPlayerName, FirstOpponentName, SecondOpponentName, ThirdOpponentName);
        }

        private LocalGameControllerScenarioRunner CreateStandardLocalGameControllerScenarioRunner(ResourceClutch mainPlayerResources, ResourceClutch firstOpponentResources, ResourceClutch secondOpponentResources, ResourceClutch thirdOpponentResources)
        {
            return LocalGameController()
                .WithMainPlayer(MainPlayerName)
                .WithComputerPlayer(FirstOpponentName).WithComputerPlayer(SecondOpponentName).WithComputerPlayer(ThirdOpponentName)
                .WithPlayerSetup(MainPlayerName, MainPlayerFirstSettlementLocation, MainPlayerFirstRoadEnd, MainPlayerSecondSettlementLocation, MainPlayerSecondRoadEnd)
                .WithPlayerSetup(FirstOpponentName, FirstOpponentFirstSettlementLocation, FirstOpponentFirstRoadEnd, FirstOpponentSecondSettlementLocation, FirstOpponentSecondRoadEnd)
                .WithPlayerSetup(SecondOpponentName, SecondOpponentFirstSettlementLocation, SecondOpponentFirstRoadEnd, SecondOpponentSecondSettlementLocation, SecondOpponentSecondRoadEnd)
                .WithPlayerSetup(ThirdOpponentName, ThirdOpponentFirstSettlementLocation, ThirdOpponentFirstRoadEnd, ThirdOpponentSecondSettlementLocation, ThirdOpponentSecondRoadEnd)
                .WithStartingResourcesForPlayer(MainPlayerName, mainPlayerResources)
                .WithStartingResourcesForPlayer(FirstOpponentName, firstOpponentResources)
                .WithStartingResourcesForPlayer(SecondOpponentName, secondOpponentResources)
                .WithStartingResourcesForPlayer(ThirdOpponentName, thirdOpponentResources)
                .WithTurnOrder(MainPlayerName, FirstOpponentName, SecondOpponentName, ThirdOpponentName);
        }
    }
}
