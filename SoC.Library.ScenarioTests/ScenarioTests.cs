
using System;
using Jabberwocky.SoC.Library;
using Jabberwocky.SoC.Library.GameEvents;
using NUnit.Framework;
using static SoC.Library.ScenarioTests.LocalGameControllerScenarioRunner;

namespace SoC.Library.ScenarioTests
{
    [Category("All")]
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
            this.CreateStandardLocalGameControllerScenarioRunner()
                .WithStartingResourcesForPlayer(MainPlayerName, ResourceClutch.DevelopmentCard)
                .WithNoResourceCollection()
                .PlayerTurn(MainPlayerName, 4, 4)
                    .Events()
                        .DiceRollEvent(4, 4)
                        .End()
                    .EndTurn()
                .PlayerTurn(FirstOpponentName, 1, 2)
                    .Events()
                        .DiceRollEvent(1, 2)
                        .End()
                    .EndTurn()
                .PlayerTurn(SecondOpponentName, 3, 4)
                    .Events()
                        .DiceRollEvent(3, 4)
                        .End()
                    .EndTurn()
                .PlayerTurn(ThirdOpponentName, 5, 6)
                    .Events()
                        .DiceRollEvent(5, 6)
                        .End()
                    .EndTurn()
                .Build()
                .Run();
        }

        [Test]
        public void Scenario_Scenario_AllPlayersCollectResourcesAsPartOfFirstTurnStart()
        {
            this.CreateStandardLocalGameControllerScenarioRunner()
                .PlayerTurn(MainPlayerName, 1, 5)
                    .Events()
                        .ResourceCollectedEvent(FirstOpponentName, 
                            new Tuple<uint, ResourceClutch>(FirstOpponentFirstSettlementLocation, ResourceClutch.OneOre))
                        .ResourceCollectedEvent(SecondOpponentName,
                            new Tuple<uint, ResourceClutch>(SecondOpponentFirstSettlementLocation, ResourceClutch.OneLumber),
                            new Tuple<uint, ResourceClutch>(SecondOpponentSecondSettlementLocation, ResourceClutch.OneLumber))
                        .ResourceCollectedEvent(ThirdOpponentName,
                            new Tuple<uint, ResourceClutch>(ThirdOpponentFirstSettlementLocation, ResourceClutch.OneOre))
                        .End()
                    .EndTurn()
                .Build()
                .Run();
        }

        [Test]
        public void Scenario_AllPlayersCollectResourcesAsPartOfTurnStart()
        {
            var localGameController = this.CreateStandardLocalGameControllerScenarioRunner()
                .PlayerTurn(MainPlayerName, 4, 4)
                    .Events()
                        .ResourceCollectedEvent(
                            new Tuple<uint, ResourceClutch>(MainPlayerFirstSettlementLocation, ResourceClutch.OneBrick))
                        .ResourceCollectedEvent(FirstOpponentName, 
                            new Tuple<uint, ResourceClutch>(FirstOpponentSecondSettlementLocation, ResourceClutch.OneGrain))
                        .End()
                    .EndTurn()
                .PlayerTurn(FirstOpponentName, 4, 4)
                    .Events()
                        .ResourceCollectedEvent(MainPlayerName,
                            new Tuple<uint, ResourceClutch>(MainPlayerFirstSettlementLocation, ResourceClutch.OneBrick))
                        .ResourceCollectedEvent(
                            new Tuple<uint, ResourceClutch>(FirstOpponentSecondSettlementLocation, ResourceClutch.OneGrain))
                        .End()
                    .EndTurn()
                .PlayerTurn(SecondOpponentName, 3, 3)
                    .Events()
                        .ResourceCollectedEvent(FirstOpponentName,
                                new Tuple<uint, ResourceClutch>(FirstOpponentFirstSettlementLocation, ResourceClutch.OneOre))
                        .ResourceCollectedEvent(
                            new Tuple<uint, ResourceClutch>(SecondOpponentFirstSettlementLocation, ResourceClutch.OneLumber),
                            new Tuple<uint, ResourceClutch>(SecondOpponentSecondSettlementLocation, ResourceClutch.OneLumber))
                        .ResourceCollectedEvent(ThirdOpponentName,
                            new Tuple<uint, ResourceClutch>(ThirdOpponentFirstSettlementLocation, ResourceClutch.OneOre))
                        .End()
                    .EndTurn()
                .PlayerTurn(ThirdOpponentName, 1, 2)
                    .Events()
                        .ResourceCollectedEvent(SecondOpponentName, 
                            new Tuple<uint, ResourceClutch>(SecondOpponentSecondSettlementLocation, ResourceClutch.OneOre))
                        .End()
                    .EndTurn()
                .PlayerTurn(MainPlayerName, 6, 4)
                    .Events()
                        .ResourceCollectedEvent(
                                new Tuple<uint, ResourceClutch>(MainPlayerFirstSettlementLocation, ResourceClutch.OneWool))
                        .ResourceCollectedEvent(FirstOpponentName,
                                new Tuple<uint, ResourceClutch>(FirstOpponentSecondSettlementLocation, ResourceClutch.OneWool))
                         .End()
                    .EndTurn()
                .Build()
                .Run();
        }

        [Test]
        public void Scenario_ComputerPlayerBuildsSettlement()
        {
            var thirdOpponentStartingResources = (ResourceClutch.RoadSegment * 2) + ResourceClutch.Settlement;
            this.CreateStandardLocalGameControllerScenarioRunner()
                .WithNoResourceCollection()
                .WithStartingResourcesForPlayer(ThirdOpponentName, thirdOpponentStartingResources)
                .PlayerTurn(MainPlayerName, 2, 3).EndTurn()
                .PlayerTurn(FirstOpponentName, 2, 2).EndTurn()
                .PlayerTurn(SecondOpponentName, 1, 3).EndTurn()
                .PlayerTurn(ThirdOpponentName, 2, 3)
                    .Actions()
                        .BuildRoad(33, 22)
                        .BuildRoad(22, 23)
                        .BuildSettlement(23)
                        .End()
                    .Events()
                        .BuildRoadEvent(33, 22)
                        .BuildRoadEvent(22, 23)
                        .BuildSettlementEvent(23)
                        .End()
                    .EndTurn()
                .Build()
                .Run();
        }

        [Test]
        public void Scenario_ComputerPlayerBuildsSettlementToWin()
        {
            var firstOpponentResources = (ResourceClutch.RoadSegment * 5) + (ResourceClutch.Settlement * 3) + (ResourceClutch.City * 3);
            this.CreateStandardLocalGameControllerScenarioRunner()
                .WithNoResourceCollection()
                .WithStartingResourcesForPlayer(FirstOpponentName, firstOpponentResources)
                .PlayerTurn(MainPlayerName, 4, 4).EndTurn()
                .PlayerTurn(FirstOpponentName, 3, 3)
                    .Actions()
                        .BuildRoad(17, 7).BuildRoad(7, 8).BuildRoad(8, 0).BuildRoad(0, 1).BuildRoad(8, 9)
                        .BuildSettlement(7).BuildSettlement(0)
                        .BuildCity(7).BuildCity(0).BuildCity(18)
                        .BuildSettlement(9)
                        .End()
                    .Events()
                        .BuildRoadEvent(17, 7).BuildRoadEvent(7, 8).BuildRoadEvent(8, 0).BuildRoadEvent(0, 1).BuildRoadEvent(8, 9)
                        .BuildSettlementEvent(7).BuildSettlementEvent(0)
                        .BuildCityEvent(7).BuildCityEvent(0).BuildCityEvent(18)
                        .BuildSettlementEvent(9)
                        .GameWinEvent(10)
                        .End()
                    .EndTurn()
                .Build()
                .Run();
        }

        [Test]
        public void Scenario_ComputerPlayerBuildsCity()
        {
            this.CreateStandardLocalGameControllerScenarioRunner()
                .WithNoResourceCollection()
                .WithStartingResourcesForPlayer(ThirdOpponentName, ResourceClutch.City)
                .PlayerTurn(MainPlayerName, 5, 6).EndTurn()
                .PlayerTurn(FirstOpponentName, 5, 6).EndTurn()
                .PlayerTurn(SecondOpponentName, 3, 3).EndTurn()
                .PlayerTurn(ThirdOpponentName, 3, 3)
                    .Actions()
                        .BuildCity(33)
                        .End()
                    .Events()
                        .BuildCityEvent(33)
                        .End()
                    .EndTurn()
                .Build()
                .Run();
        }

        [Test]
        public void Scenario_ComputerPlayerBuildsCityToWin()
        {
            var firstOpponentResources = (ResourceClutch.RoadSegment * 5) + (ResourceClutch.Settlement * 4) + (ResourceClutch.City * 4);

            this.CreateStandardLocalGameControllerScenarioRunner()
                .WithNoResourceCollection()
                .WithStartingResourcesForPlayer(FirstOpponentName, firstOpponentResources)
                .PlayerTurn(MainPlayerName, 4, 4).EndTurn()
                .PlayerTurn(FirstOpponentName, 3, 3)
                    .Actions()
                        .BuildRoad(17, 7).BuildRoad(17, 16).BuildRoad(43, 51).BuildRoad(51, 50).BuildRoad(51, 52)
                        .BuildSettlement(7).BuildSettlement(16).BuildSettlement(50).BuildSettlement(52)
                        .BuildCity(7).BuildCity(16).BuildCity(50).BuildCity(52)
                        .End()
                    .Events()
                        .BuildRoadEvent(17, 7).BuildRoadEvent(17, 16)
                        .BuildRoadEvent(43, 51).BuildRoadEvent(51, 50)
                        .BuildRoadEvent(51, 52)
                        .BuildSettlementEvent(7).BuildSettlementEvent(16)
                        .BuildSettlementEvent(50).BuildSettlementEvent(52)
                        .BuildCityEvent(7).BuildCityEvent(16)
                        .BuildCityEvent(50).BuildCityEvent(52)
                        .GameWinEvent(10)
                        .End()
                    .EndTurn()
                .Build()
                .Run();
        }

        [Test]
        public void Scenario_ComputerPlayerHasEightVictoryPointsAndBuildsLargestArmyToWin()
        {
            var firstOpponentResources = (ResourceClutch.RoadSegment * 2) + (ResourceClutch.Settlement * 2) + 
                (ResourceClutch.City * 4) + (ResourceClutch.DevelopmentCard * 3);

            this.CreateStandardLocalGameControllerScenarioRunner()
                .WithNoResourceCollection()
                .WithStartingResourcesForPlayer(FirstOpponentName, firstOpponentResources)
                .PlayerTurn(MainPlayerName, 4, 4).EndTurn()
                .PlayerTurn(FirstOpponentName, 3, 3)
                    .Actions()
                        .BuildRoad(17, 7).BuildRoad(17, 16)
                        .BuildSettlement(7).BuildSettlement(16)
                        .BuildCity(7).BuildCity(16).BuildCity(18).BuildCity(43)
                        .BuyDevelopmentCard(DevelopmentCardTypes.Knight).BuyDevelopmentCard(DevelopmentCardTypes.Knight).BuyDevelopmentCard(DevelopmentCardTypes.Knight)
                        .End()
                    .Events()
                        .BuildRoadEvent(17, 7).BuildRoadEvent(17, 16)
                        .BuildSettlementEvent(7).BuildSettlementEvent(16)
                        .BuildCityEvent(7).BuildCityEvent(16).BuildCityEvent(18).BuildCityEvent(43)
                        .End()
                    .EndTurn()
                .PlayerTurn(SecondOpponentName, 3, 3).EndTurn()
                .PlayerTurn(ThirdOpponentName, 3, 3).EndTurn()
                .PlayerTurn(MainPlayerName, 3, 3).EndTurn()
                .PlayerTurn(FirstOpponentName, 3, 3)
                    .Actions()
                        .PlayKnightCard(4)
                        .End()
                    .EndTurn()
                .PlayerTurn(SecondOpponentName, 3, 3).EndTurn()
                .PlayerTurn(ThirdOpponentName, 3, 3).EndTurn()
                .PlayerTurn(MainPlayerName, 3, 3).EndTurn()
                .PlayerTurn(FirstOpponentName, 3, 3)
                    .Actions()
                        .PlayKnightCard(0)
                        .End()
                    .EndTurn()
                .PlayerTurn(SecondOpponentName, 3, 3).EndTurn()
                .PlayerTurn(ThirdOpponentName, 3, 3).EndTurn()
                .PlayerTurn(MainPlayerName, 3, 3).EndTurn()
                .PlayerTurn(FirstOpponentName, 3, 3)
                    .Actions()
                        .PlayKnightCard(4)
                        .End()
                    .Events()
                        .LargestArmyChangedEvent()
                        .GameWinEvent(10)
                        .End()
                    .EndTurn()
                .Build()
                .Run();
        }

        [Test]
        public void Scenario_ComputerPlayerHasEightVictoryPointsAndBuildsLongestRoadToWin()
        {
            var firstOpponentResources = (ResourceClutch.RoadSegment * 4) + (ResourceClutch.Settlement * 2) +
                (ResourceClutch.City * 4);

            this.CreateStandardLocalGameControllerScenarioRunner()
                .WithNoResourceCollection()
                .WithStartingResourcesForPlayer(FirstOpponentName, firstOpponentResources)
                .PlayerTurn(MainPlayerName, 4, 4).EndTurn()
                .PlayerTurn(FirstOpponentName, 3, 3)
                    .Actions()
                        .BuildRoad(17, 7).BuildRoad(7, 8).BuildRoad(8, 0)
                        .BuildSettlement(7).BuildSettlement(0)
                        .BuildCity(7).BuildCity(0).BuildCity(18).BuildCity(43)
                        .BuildRoad(0, 1)
                        .End()
                    .Events()
                        .BuildRoadEvent(17, 7).BuildRoadEvent(7, 8).BuildRoadEvent(8, 0)
                        .BuildSettlementEvent(7).BuildSettlementEvent(0)
                        .BuildCityEvent(7).BuildCityEvent(0).BuildCityEvent(18).BuildCityEvent(43)
                        .BuildRoadEvent(0, 1)
                        .LongestRoadBuiltEvent()
                        .GameWinEvent(10)
                        .End()
                    .EndTurn()
                .Build()
                .Run();
        }

        [Test]
        public void Scenario_ComputerPlayerHasNineVictoryPointsAndBuildsLargestArmyToWin()
        {
            var firstOpponentResources = (ResourceClutch.RoadSegment * 3) + (ResourceClutch.Settlement * 3) +
                (ResourceClutch.City * 4) + (ResourceClutch.DevelopmentCard * 3);

            this.CreateStandardLocalGameControllerScenarioRunner()
                .WithNoResourceCollection()
                .WithStartingResourcesForPlayer(FirstOpponentName, firstOpponentResources)
                .PlayerTurn(MainPlayerName, 4, 4).EndTurn()
                .PlayerTurn(FirstOpponentName, 3, 3)
                    .Actions()
                        .BuildRoad(17, 7).BuildRoad(17, 16).BuildRoad(44, 45)
                        .BuildSettlement(7).BuildSettlement(16).BuildSettlement(45)
                        .BuildCity(7).BuildCity(16).BuildCity(18).BuildCity(43)
                        .BuyDevelopmentCard(DevelopmentCardTypes.Knight).BuyDevelopmentCard(DevelopmentCardTypes.Knight).BuyDevelopmentCard(DevelopmentCardTypes.Knight)
                        .End()
                    .Events()
                        .BuildRoadEvent(17, 7).BuildRoadEvent(17, 16).BuildRoadEvent(44, 45)
                        .BuildSettlementEvent(7).BuildSettlementEvent(16).BuildSettlementEvent(45)
                        .BuildCityEvent(7).BuildCityEvent(16).BuildCityEvent(18).BuildCityEvent(43)
                        .End()
                    .EndTurn()
                .PlayerTurn(SecondOpponentName, 3, 3).EndTurn()
                .PlayerTurn(ThirdOpponentName, 3, 3).EndTurn()
                .PlayerTurn(MainPlayerName, 3, 3).EndTurn()
                .PlayerTurn(FirstOpponentName, 3, 3)
                    .Actions()
                        .PlayKnightCard(4)
                        .End()
                    .EndTurn()
                .PlayerTurn(SecondOpponentName, 3, 3).EndTurn()
                .PlayerTurn(ThirdOpponentName, 3, 3).EndTurn()
                .PlayerTurn(MainPlayerName, 3, 3).EndTurn()
                .PlayerTurn(FirstOpponentName, 3, 3)
                    .Actions()
                        .PlayKnightCard(0)
                        .End()
                    .EndTurn()
                .PlayerTurn(SecondOpponentName, 3, 3).EndTurn()
                .PlayerTurn(ThirdOpponentName, 3, 3).EndTurn()
                .PlayerTurn(MainPlayerName, 3, 3).EndTurn()
                .PlayerTurn(FirstOpponentName, 3, 3)
                    .Actions()
                        .PlayKnightCard(4)
                        .End()
                    .Events()
                        .LargestArmyChangedEvent()
                        .GameWinEvent(11)
                        .End()
                    .EndTurn()
                .Build()
                .Run();
        }

        [Test]
        public void Scenario_ComputerPlayerHasNineVictoryPointsAndBuildsLongestRoadToWin()
        {
            var firstOpponentResources = (ResourceClutch.RoadSegment * 5) + (ResourceClutch.Settlement * 3) +
                (ResourceClutch.City * 4);

            this.CreateStandardLocalGameControllerScenarioRunner()
                .WithNoResourceCollection()
                .WithStartingResourcesForPlayer(FirstOpponentName, firstOpponentResources)
                .PlayerTurn(MainPlayerName, 4, 4).EndTurn()
                .PlayerTurn(FirstOpponentName, 3, 3)
                    .Actions()
                        .BuildRoad(17, 7).BuildRoad(7, 8).BuildRoad(8, 0).BuildRoad(44, 45)
                        .BuildSettlement(7).BuildSettlement(0).BuildSettlement(45)
                        .BuildCity(7).BuildCity(0).BuildCity(18).BuildCity(43)
                        .BuildRoad(0, 1)
                        .End()
                    .Events()
                        .BuildRoadEvent(17, 7).BuildRoadEvent(7, 8)
                        .BuildRoadEvent(8, 0).BuildRoadEvent(44, 45)
                        .BuildSettlementEvent(7).BuildSettlementEvent(0)
                        .BuildSettlementEvent(45)
                        .BuildCityEvent(7).BuildCityEvent(0).BuildCityEvent(18).BuildCityEvent(43)
                        .BuildRoadEvent(0, 1)
                        .LongestRoadBuiltEvent()
                        .GameWinEvent(11)
                        .End()
                    .EndTurn()
                .Build()
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
            this.CreateStandardLocalGameControllerScenarioRunner()
                .WithNoResourceCollection()
                .WithStartingResourcesForPlayer(MainPlayerName, mainPlayerResources).WithStartingResourcesForPlayer(FirstOpponentName, firstOpponentResources)
                .WithNoResourceCollection()
                .PlayerTurn(MainPlayerName, 4, 4)
                    .Actions()
                        .BuyDevelopmentCard(DevelopmentCardTypes.Knight)
                        .End()
                    .Events()
                        .BuyDevelopmentCardEvent(DevelopmentCardTypes.Knight)
                        .End()
                    .EndTurn()
                .PlayerTurn(FirstOpponentName, 3, 3).EndTurn()
                .PlayerTurn(SecondOpponentName, 3, 3).EndTurn()
                .PlayerTurn(ThirdOpponentName, 3, 3).EndTurn()
                .PlayerTurn(MainPlayerName, 4, 4)
                    .Actions()
                        .PlayKnightCardAndCollectFrom(3, FirstOpponentName, ResourceTypes.Ore)
                        .End()
                    .Events()
                        .ResourcesGainedEvent(FirstOpponentName, ResourceClutch.OneOre)
                        .End()
                    .EndTurn()
                .Build()
                .Run();
        }

        [Test]
        public void Scenario_ComputerPlayerRollsSevenAndTakesResourceFromPlayer() { Assert.Fail(); }

        [Test]
        public void Scenario_ComputerPlayerRollsSevenAndTakesResourcesFromComputerPlayer() { Assert.Fail(); }

        [Test]
        public void Scenario_ComputerPlayerRollsSeventAndAllPlayersWithMoreThanSevenResourcesLosesResources()
        {
            Assert.Fail();
        }

        /// <summary>
        /// Test that the largest army event is not raised when the player plays knight cards and already has the largest army
        /// </summary>
        [Test]
        public void Scenario_LargestArmyEventOnlyRaisedFirstTimeThatPlayerHasMostKnightCardsPlayed()
        {
            var mainPlayerResources = ResourceClutch.DevelopmentCard * 4;
            this.CreateStandardLocalGameControllerScenarioRunner()
                .WithNoResourceCollection()
                .WithStartingResourcesForPlayer(MainPlayerName, mainPlayerResources)
                .PlayerTurn(MainPlayerName, 4, 4)
                    .Actions()
                        .BuyDevelopmentCard(DevelopmentCardTypes.Knight)
                        .BuyDevelopmentCard(DevelopmentCardTypes.Knight)
                        .BuyDevelopmentCard(DevelopmentCardTypes.Knight)
                        .BuyDevelopmentCard(DevelopmentCardTypes.Knight)
                        .End()
                    .EndTurn()
                .PlayerTurn(FirstOpponentName, 3, 3).EndTurn()
                .PlayerTurn(SecondOpponentName, 3, 3).EndTurn()
                .PlayerTurn(ThirdOpponentName, 3, 3).EndTurn()
                .PlayerTurn(MainPlayerName, 4, 4)
                    .Actions()
                        .PlayKnightCard(4)
                        .End()
                    .EndTurn()
                .PlayerTurn(FirstOpponentName, 3, 3).EndTurn()
                .PlayerTurn(SecondOpponentName, 3, 3).EndTurn()
                .PlayerTurn(ThirdOpponentName, 3, 3).EndTurn()
                .PlayerTurn(MainPlayerName, 4, 4)
                    .Actions()
                        .PlayKnightCard(0)
                        .End()
                    .EndTurn()
                .PlayerTurn(FirstOpponentName, 3, 3).EndTurn()
                .PlayerTurn(SecondOpponentName, 3, 3).EndTurn()
                .PlayerTurn(ThirdOpponentName, 3, 3).EndTurn()
                .PlayerTurn(MainPlayerName, 4, 4)
                    .Actions()
                        .PlayKnightCard(4)
                        .End()
                    .EndTurn()
                .PlayerTurn(FirstOpponentName, 3, 3).EndTurn()
                .PlayerTurn(SecondOpponentName, 3, 3).EndTurn()
                .PlayerTurn(ThirdOpponentName, 3, 3).EndTurn()
                .PlayerTurn(MainPlayerName, 4, 4)
                    .Actions()
                        .PlayKnightCard(0)
                        .End()
                    .EndTurn()
                .PlayerTurn(FirstOpponentName, 3, 3).EndTurn()
                .Build()
                .Run();
        }

        /// <summary>
        /// Test that the largest army event is not sent when the opponent plays knight cards and already has the largest army
        /// </summary>
        [Test]
        public void Scenario_LargestArmyEventOnlyReturnedFirstTimeThatOpponentHasMostKnightCardsPlayed()
        {
            var firstOpponentResources = ResourceClutch.DevelopmentCard * 4;
            this.CreateStandardLocalGameControllerScenarioRunner()
                .WithNoResourceCollection()
                .WithStartingResourcesForPlayer(FirstOpponentName, firstOpponentResources)
                .PlayerTurn(MainPlayerName, 4, 4).EndTurn()
                .PlayerTurn(FirstOpponentName, 3, 3)
                    .Actions()
                        .BuyDevelopmentCard(DevelopmentCardTypes.Knight)
                        .BuyDevelopmentCard(DevelopmentCardTypes.Knight)
                        .BuyDevelopmentCard(DevelopmentCardTypes.Knight)
                        .BuyDevelopmentCard(DevelopmentCardTypes.Knight)
                        .End()
                    .Events()
                        .BuyDevelopmentCardEvent(DevelopmentCardTypes.Knight)
                        .BuyDevelopmentCardEvent(DevelopmentCardTypes.Knight)
                        .BuyDevelopmentCardEvent(DevelopmentCardTypes.Knight)
                        .BuyDevelopmentCardEvent(DevelopmentCardTypes.Knight)
                        .End()
                    .EndTurn()
                .PlayerTurn(SecondOpponentName, 3, 3).EndTurn()
                .PlayerTurn(ThirdOpponentName, 3, 3).EndTurn()
                .PlayerTurn(MainPlayerName, 4, 4).EndTurn()
                .PlayerTurn(FirstOpponentName, 3, 3)
                    .Actions()
                        .PlayKnightCard(4)
                        .End()
                    .Events()
                        .PlayKnightCardEvent()
                        .End()
                    .EndTurn()
                .PlayerTurn(SecondOpponentName, 3, 3).EndTurn()
                .PlayerTurn(ThirdOpponentName, 3, 3).EndTurn()
                .PlayerTurn(MainPlayerName, 4, 4).EndTurn()
                .PlayerTurn(FirstOpponentName, 3, 3)
                    .Actions()
                        .PlayKnightCard(0)
                        .End()
                    .Events()
                        .PlayKnightCardEvent()
                        .End()
                    .EndTurn()
                .PlayerTurn(SecondOpponentName, 3, 3).EndTurn()
                .PlayerTurn(ThirdOpponentName, 3, 3).EndTurn()
                .PlayerTurn(MainPlayerName, 4, 4).EndTurn()
                .PlayerTurn(FirstOpponentName, 3, 3)
                    .Actions()
                        .PlayKnightCard(4)
                        .End()
                    .Events()
                        .PlayKnightCardEvent()
                        .LargestArmyChangedEvent(null)
                        .End()
                    .EndTurn()
                .PlayerTurn(SecondOpponentName, 3, 3).EndTurn()
                .PlayerTurn(ThirdOpponentName, 3, 3).EndTurn()
                .PlayerTurn(MainPlayerName, 4, 4).EndTurn()
                .PlayerTurn(FirstOpponentName, 3, 3)
                    .Actions()
                        .PlayKnightCard(0)
                        .End()
                    .Events()
                        .PlayKnightCardEvent()
                        .NoEventOfType<LargestArmyChangedEvent>()
                        .End()
                    .EndTurn()
                .Build()
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
            this.CreateStandardLocalGameControllerScenarioRunner()
                .WithNoResourceCollection()
                .WithStartingResourcesForPlayer(MainPlayerName, mainPlayerResources)
                .WithStartingResourcesForPlayer(FirstOpponentName, firstOpponentResources)
                .PlayerTurn(MainPlayerName, 4, 4)
                    .Actions()
                        .BuyDevelopmentCard(DevelopmentCardTypes.Knight)
                        .BuyDevelopmentCard(DevelopmentCardTypes.Knight)
                        .BuyDevelopmentCard(DevelopmentCardTypes.Knight)
                        .End()
                    .Events()
                        .BuyDevelopmentCardEvent(DevelopmentCardTypes.Knight)
                        .BuyDevelopmentCardEvent(DevelopmentCardTypes.Knight)
                        .BuyDevelopmentCardEvent(DevelopmentCardTypes.Knight)
                        .End()
                    .EndTurn()
                .PlayerTurn(FirstOpponentName, 3, 3)
                    .Actions()
                        .BuyDevelopmentCard(DevelopmentCardTypes.Knight)
                        .BuyDevelopmentCard(DevelopmentCardTypes.Knight)
                        .BuyDevelopmentCard(DevelopmentCardTypes.Knight)
                        .BuyDevelopmentCard(DevelopmentCardTypes.Knight)
                        .End()
                    .Events()
                        .BuyDevelopmentCardEvent(DevelopmentCardTypes.Knight)
                        .BuyDevelopmentCardEvent(DevelopmentCardTypes.Knight)
                        .BuyDevelopmentCardEvent(DevelopmentCardTypes.Knight)
                        .BuyDevelopmentCardEvent(DevelopmentCardTypes.Knight)
                        .End()
                    .EndTurn()
                .PlayerTurn(SecondOpponentName, 3, 3).EndTurn()
                .PlayerTurn(ThirdOpponentName, 3, 3).EndTurn()
                .PlayerTurn(MainPlayerName, 4, 4)
                    .Actions()
                        .PlayKnightCard(4)
                        .End()
                    .Events()
                        .PlayKnightCardEvent()
                        .End()
                    .EndTurn()
                .PlayerTurn(FirstOpponentName, 3, 3)
                    .Actions()
                        .PlayKnightCard(0)
                        .End()
                    .Events()
                        .PlayKnightCardEvent()
                        .End()
                    .EndTurn()
                .PlayerTurn(SecondOpponentName, 3, 3).EndTurn()
                .PlayerTurn(ThirdOpponentName, 3, 3).EndTurn()
                .PlayerTurn(MainPlayerName, 4, 4)
                    .Actions()
                        .PlayKnightCard(4)
                        .End()
                    .Events()
                        .PlayKnightCardEvent()
                        .End()
                    .EndTurn()
                .PlayerTurn(FirstOpponentName, 3, 3)
                    .Actions()
                        .PlayKnightCard(0)
                        .End()
                    .Events()
                        .PlayKnightCardEvent()
                        .End()
                    .EndTurn()
                .PlayerTurn(SecondOpponentName, 3, 3).EndTurn()
                .PlayerTurn(ThirdOpponentName, 3, 3).EndTurn()
                .PlayerTurn(MainPlayerName, 4, 4)
                    .Actions()
                        .PlayKnightCard(4)
                        .End()
                    .Events()
                        .PlayKnightCardEvent()
                        .LargestArmyChangedEvent(null)
                        .End()
                    .State(MainPlayerName)
                        .VictoryPoints(4)
                        .End()
                    .State(FirstOpponentName)
                        .VictoryPoints(2)
                        .End()
                    .EndTurn()
                .PlayerTurn(FirstOpponentName, 3, 3)
                    .Actions()
                        .PlayKnightCard(0)
                        .End()
                    .Events()
                        .PlayKnightCardEvent()
                        .End()
                    .EndTurn()
                .PlayerTurn(SecondOpponentName, 3, 3).EndTurn()
                .PlayerTurn(ThirdOpponentName, 3, 3).EndTurn()
                .PlayerTurn(MainPlayerName, 4, 4).EndTurn()
                .PlayerTurn(FirstOpponentName, 3, 3)
                    .Actions()
                        .PlayKnightCard(4)
                        .End()
                    .Events()
                        .PlayKnightCardEvent()
                        .LargestArmyChangedEvent(MainPlayerName)
                        .End()
                    .State(MainPlayerName)
                        .VictoryPoints(2)
                        .End()
                    .State(FirstOpponentName)
                        .VictoryPoints(4)
                        .End()
                    .EndTurn()
                .Build()
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
                .PlayerTurn(MainPlayerName, 4, 4)
                    .Actions()
                        .BuyDevelopmentCard(DevelopmentCardTypes.Knight)
                        .BuyDevelopmentCard(DevelopmentCardTypes.Knight)
                        .BuyDevelopmentCard(DevelopmentCardTypes.Knight)
                        .BuyDevelopmentCard(DevelopmentCardTypes.Knight)
                        .End()
                    .EndTurn()
                .PlayerTurn(FirstOpponentName, 3, 3).EndTurn()
                .PlayerTurn(SecondOpponentName, 3, 3).EndTurn()
                .PlayerTurn(ThirdOpponentName, 3, 3).EndTurn()
                .PlayerTurn(MainPlayerName, 4, 4)
                    .Actions()
                        .PlayKnightCard(4)
                        .End()
                    .EndTurn()
                .PlayerTurn(FirstOpponentName, 3, 3).EndTurn()
                .PlayerTurn(SecondOpponentName, 3, 3).EndTurn()
                .PlayerTurn(ThirdOpponentName, 3, 3).EndTurn()
                .PlayerTurn(MainPlayerName, 4, 4)
                    .Actions()
                        .PlayKnightCard(0)
                        .End()
                    .EndTurn()
                .PlayerTurn(FirstOpponentName, 3, 3).EndTurn()
                .PlayerTurn(SecondOpponentName, 3, 3).EndTurn()
                .PlayerTurn(ThirdOpponentName, 3, 3).EndTurn()
                .PlayerTurn(MainPlayerName, 4, 4)
                    .Actions()
                        .PlayKnightCard(4)
                        .End()
                    .State(MainPlayerName)
                        .VictoryPoints(4)
                        .End()
                    .EndTurn()
                .PlayerTurn(FirstOpponentName, 3, 3).EndTurn()
                .PlayerTurn(SecondOpponentName, 3, 3).EndTurn()
                .PlayerTurn(ThirdOpponentName, 3, 3).EndTurn()
                .PlayerTurn(MainPlayerName, 4, 4)
                    .Actions()
                        .PlayKnightCard(0)
                        .End()
                    .State(MainPlayerName)
                        .VictoryPoints(4)
                        .End()
                    .EndTurn()
                .Build()
                .Run();
        }

        [Test]
        public void Scenario_PlayerLosesResourceWhenComputerPlayerPlaysTheKnightCard()
        {
            var mainPlayerResources = ResourceClutch.OneOre;
            var firstOpponentResources = ResourceClutch.DevelopmentCard;
            this.CreateStandardLocalGameControllerScenarioRunner()
                .WithNoResourceCollection()
                .WithStartingResourcesForPlayer(MainPlayerName, mainPlayerResources).WithStartingResourcesForPlayer(FirstOpponentName, firstOpponentResources)
                .PlayerTurn(MainPlayerName, 4, 4).EndTurn()
                .PlayerTurn(FirstOpponentName, 3, 3)
                    .Actions()
                        .BuyDevelopmentCard(DevelopmentCardTypes.Knight)
                        .End()
                    .Events()
                        .BuyDevelopmentCardEvent(DevelopmentCardTypes.Knight)
                        .End()
                    .EndTurn()
                .PlayerTurn(SecondOpponentName, 3, 3).EndTurn()
                .PlayerTurn(ThirdOpponentName, 3, 3).EndTurn()
                .PlayerTurn(MainPlayerName, 4, 4).EndTurn()
                .PlayerTurn(FirstOpponentName, 3, 3)
                    .Actions()
                        .PlayKnightCardAndCollectFrom(3, MainPlayerName, ResourceTypes.Ore)
                        .End()
                    .Events()
                        .PlayKnightCardEvent()
                        .ResourcesGainedEvent(MainPlayerName, ResourceClutch.OneOre)
                        .End()
                    .EndTurn()
                .Build()
                .Run();
        }

        [Test]
        [TestCase(7, 0)]
        [TestCase(8, 4)]
        [TestCase(9, 4)]
        [TestCase(10, 5)]
        public void Scenario_PlayerRollsSevenAndReceivesReceivesRobberEventWithDropResourceCardsCount(int resourcesCount, int expectedResourcesToDrop)
        {
            var mainPlayerResources = ResourceClutch.OneBrick * resourcesCount;
            this.CreateStandardLocalGameControllerScenarioRunner()
                .WithNoResourceCollection()
                .WithStartingResourcesForPlayer(MainPlayerName, mainPlayerResources)
                .PlayerTurn(MainPlayerName, 3, 4)
                    .Events()
                        .RobberEvent(expectedResourcesToDrop)
                        .End()
                    .EndTurn()
                .Build()
                .Run();
        }

        [Test]
        public void Scenario_PlayerRollsSevenDoesNotPassBackResourcesAndReceivesErrorMessage()
        {
            var mainPlayerResources = ResourceClutch.OneBrick * 8;
            this.CreateStandardLocalGameControllerScenarioRunner()
                .WithNoResourceCollection()
                .WithStartingResourcesForPlayer(MainPlayerName, mainPlayerResources)
                .PlayerTurn(MainPlayerName, 3, 4)
                    .Actions()
                        .PlaceRobber(3)
                        .End()
                    .Events()
                        .ErrorMessageEvent("Cannot set robber location until expected resources (4) have been dropped via call to DropResources method.")
                        .End()
                    .EndTurn()
                .Build()
                .Run();
        }

        [Test]
        public void Scenario_PlayerRollsSevenAndSelectedHexHasSingleComputerPlayer()
        {
            var firstOpponentResources = ResourceClutch.OneGrain * 2;
        
            this.CreateStandardLocalGameControllerScenarioRunner()
                .WithNoResourceCollection()
                .WithStartingResourcesForPlayer(FirstOpponentName, firstOpponentResources)
                .PlayerTurn(MainPlayerName, 3, 4)
                    .Actions()
                        .PlaceRobber(3, FirstOpponentName, ResourceTypes.Grain)
                        .End()
                    .Events()
                        .RobbingChoicesEvent(new Tuple<string, int>(FirstOpponentName, 2))
                        .End()
                    .EndTurn()
                .Build()
                .Run();
        }

        [Test]
        public void Scenario_PlayerRollsSeventAndAllPlayersWithMoreThanSevenResourcesLosesResources()
        {
            Assert.Fail();
        }

        /// <summary>
        /// The robber hex set by the player has no adjacent settlements so the returned robbing choices
        /// is null.
        /// </summary>
        [Test]
        public void Scenario_PlayerRollsSevenAndSelectedHexHasNoPlayers()
        {
            this.CreateStandardLocalGameControllerScenarioRunner()
                .WithNoResourceCollection()
                .PlayerTurn(MainPlayerName, 3, 4)
                    .Actions()
                        .PlaceRobber(4)
                        .End()
                    .Events()
                        .RobbingChoicesEvent(null)
                        .End()
                    .EndTurn()
                .Build()
                .Run();
        }

        /// <summary>
        /// The robber hex set by the player has only player settlements so the returned robbing choices is null.
        /// </summary>
        [Test]
        public void Scenario_PlayerRollsSevenAndSelectedHexHasPlayerOnly()
        {
            this.CreateStandardLocalGameControllerScenarioRunner()
                .WithNoResourceCollection()
                .PlayerTurn(MainPlayerName, 3, 4)
                    .Actions()
                        .PlaceRobber(2)
                        .End()
                    .Events()
                        .RobbingChoicesEvent(null)
                        .End()
                    .EndTurn()
                .Build()
                .Run();
        }

        [Test]
        public void Scenario_PlayerRollsSevenAndGetsResourceFromSelectedComputerPlayer()
        {
            var firstOpponentResources = ResourceClutch.OneWool;
            this.CreateStandardLocalGameControllerScenarioRunner()
                .WithNoResourceCollection()
                .WithStartingResourcesForPlayer(FirstOpponentName, firstOpponentResources)
                .PlayerTurn(MainPlayerName, 3, 4)
                    .Actions()
                        .PlaceRobber(3)
                        .ChooseResourceFromOpponent(FirstOpponentName, ResourceTypes.Wool)
                        .End()
                    .Events()
                        .RobbingChoicesEvent(new Tuple<string, int>(FirstOpponentName, 1))
                        .ResourcesGainedEvent(FirstOpponentName, ResourceClutch.OneWool)
                        .End()
                    .State(MainPlayerName)
                        .Resources(ResourceClutch.OneWool)
                        .End()
                    .State(FirstOpponentName)
                        .Resources(ResourceClutch.Zero)
                        .End()
                    .EndTurn()
                .Build()
                .Run();
        }

        /// <summary>
        /// Passing in an id of a player that is not on the selected robber hex when choosing the resource 
        /// causes an error to be raised.
        /// </summary>
        [Test]
        public void Scenario_PlayerRollsSevenAndSelectsInvalidOpponentGettingErrorMessage()
        {
            var firstOpponentResources = ResourceClutch.OneWool;
            this.CreateStandardLocalGameControllerScenarioRunner()
                .WithNoResourceCollection()
                .WithStartingResourcesForPlayer(FirstOpponentName, firstOpponentResources)
                .PlayerTurn(MainPlayerName, 3, 4)
                    .Actions()
                        .PlaceRobber(3)
                        .ChooseResourceFromOpponent(SecondOpponentName, ResourceTypes.Wool)
                        .End()
                    .Events()
                        .RobbingChoicesEvent(new Tuple<string, int>(FirstOpponentName, 1))
                        .ErrorMessageEvent("Cannot pick resource card from invalid opponent.")
                        .End()
                    .EndTurn()
                .Build()
                .Run();
        }

        /// <summary>
        /// The robber hex set by the player has only player settlements so calling the CallingChooseResourceFromOpponent 
        /// method raises an error
        /// </summary>
        [Test]
        public void Scenario_RobberLocationOnlyHasPlayerSettlementsSoChooseResourceFromOpponentCallReturnsErrorMessage()
        {
            Assert.Fail();
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
