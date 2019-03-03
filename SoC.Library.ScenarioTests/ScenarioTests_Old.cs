
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
    public class ScenarioTests_Old
    {
        const string MainPlayerName = "Player";
        const string MainPlayer = "Player";
        const string FirstOpponentName = "Barbara";
        const string FirstOpponent_Babara = "Barbara";
        const string SecondOpponentName = "Charlie";
        const string SecondOpponent_Charlie = "Charlie";
        const string ThirdOpponentName = "Dana";
        const string ThirdOpponent_Dana = "Dana";

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
        public void New_Scenario_AllPlayersHaveDiceRollEventInTheirTurn()
        {
            this.CreateStandardLocalGameControllerScenarioRunner_Old()
                .WithStartingResourcesForPlayer(MainPlayerName, ResourceClutch.DevelopmentCard)
                .WithNoResourceCollection()
                .PlayerTurn(MainPlayerName, 4, 4)
                    .DiceRollEvent(4, 4)
                    .EndTurn()
                .PlayerTurn(FirstOpponentName, 1, 2)
                    .DiceRollEvent(1, 2)
                    .EndTurn()
                .PlayerTurn(SecondOpponentName, 3, 4)
                    .DiceRollEvent(3, 4)
                    .EndTurn()
                .PlayerTurn(ThirdOpponentName, 5, 6)
                    .DiceRollEvent(5, 6)
                    .EndTurn()
                .Build()
                .Run_Old();
        }

        [Test]
        public void Scenario_Scenario_AllPlayersCollectResourcesAsPartOfFirstTurnStart()
        {
            this.CreateStandardLocalGameControllerScenarioRunner_Old()
                .PlayerTurn(MainPlayerName, 1, 5)
                    .ResourceCollectedEvent(FirstOpponentName,
                        new Tuple<uint, ResourceClutch>(FirstOpponentFirstSettlementLocation, ResourceClutch.OneOre))
                    .ResourceCollectedEvent(SecondOpponentName,
                        new Tuple<uint, ResourceClutch>(SecondOpponentFirstSettlementLocation, ResourceClutch.OneLumber),
                        new Tuple<uint, ResourceClutch>(SecondOpponentSecondSettlementLocation, ResourceClutch.OneLumber))
                    .ResourceCollectedEvent(ThirdOpponentName,
                        new Tuple<uint, ResourceClutch>(ThirdOpponentFirstSettlementLocation, ResourceClutch.OneOre))
                    .EndTurn()
                .Build()
                .Run_Old();
        }

        [Test]
        public void New_Scenario_AllPlayersCollectResourcesAsPartOfTurnStart()
        {
            var localGameController = this.CreateStandardLocalGameControllerScenarioRunner_Old()
                .PlayerTurn(MainPlayerName, 4, 4)
                    .ResourceCollectedEvent(MainPlayerName,
                        new Tuple<uint, ResourceClutch>(MainPlayerFirstSettlementLocation, ResourceClutch.OneBrick))
                    .ResourceCollectedEvent(FirstOpponentName, 
                        new Tuple<uint, ResourceClutch>(FirstOpponentSecondSettlementLocation, ResourceClutch.OneGrain))
                    .EndTurn()
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
                .Run_Old();
        }

        [Test]
        public void New_Scenario_ComputerPlayerBuildsSettlement()
        {
            var thirdOpponentStartingResources = (ResourceClutch.RoadSegment * 2) + ResourceClutch.Settlement;
            this.CreateStandardLocalGameControllerScenarioRunner_Old()
                .WithNoResourceCollection()
                .WithStartingResourcesForPlayer(ThirdOpponentName, thirdOpponentStartingResources)
                .PlayerTurn(MainPlayerName, 2, 3).EndTurn()
                .PlayerTurn(FirstOpponentName, 2, 2).EndTurn()
                .PlayerTurn(SecondOpponentName, 1, 3).EndTurn()
                .PlayerTurn(ThirdOpponentName, 2, 3)
                    .BuildRoad(33, 22).BuildRoadEvent(33, 22)
                    .BuildRoad(22, 23).BuildRoadEvent(22, 23)
                    .BuildSettlement(23).BuildSettlementEvent(23)
                    .EndTurn()
                .Build()
                .Run_Old();
        }

        [Test]
        public void New_Scenario_ComputerPlayerBuildsSettlementToWin()
        {
            var firstOpponentResources = (ResourceClutch.RoadSegment * 5) + (ResourceClutch.Settlement * 3) + (ResourceClutch.City * 3);
            this.CreateStandardLocalGameControllerScenarioRunner_Old()
                .WithNoResourceCollection()
                .WithStartingResourcesForPlayer(FirstOpponentName, firstOpponentResources)
                .PlayerTurn(MainPlayerName, 4, 4).EndTurn()
                .PlayerTurn(FirstOpponentName, 3, 3)
                        .BuildRoad(17, 7).BuildRoadEvent(17, 7)
                        .BuildRoad(7, 8).BuildRoadEvent(7, 8)
                        .BuildRoad(8, 0).BuildRoadEvent(8, 0)
                        .BuildRoad(0, 1).BuildRoadEvent(0, 1)
                        .BuildRoad(8, 9).BuildRoadEvent(8, 9)
                        .BuildSettlement(7).BuildSettlementEvent(7)
                        .BuildSettlement(0).BuildSettlementEvent(0)
                        .BuildCity(7).BuildCityEvent(7)
                        .BuildCity(0).BuildCityEvent(0)
                        .BuildCity(18).BuildCityEvent(18)
                        .BuildSettlement(9).BuildSettlementEvent(9)
                        .GameWinEvent(10)
                    .EndTurn()
                .Build()
                .Run_Old();
        }

        [Test]
        public void New_Scenario_ComputerPlayerBuildsCity()
        {
            this.CreateStandardLocalGameControllerScenarioRunner_Old()
                .WithNoResourceCollection()
                .WithStartingResourcesForPlayer(ThirdOpponentName, ResourceClutch.City)
                .PlayerTurn(MainPlayerName, 5, 6).EndTurn()
                .PlayerTurn(FirstOpponentName, 5, 6).EndTurn()
                .PlayerTurn(SecondOpponentName, 3, 3).EndTurn()
                .PlayerTurn(ThirdOpponentName, 3, 3)
                    .BuildCity(33)
                    .BuildCityEvent(33)
                    .EndTurn()
                .Build()
                .Run_Old();
        }

        [Test]
        public void New_Scenario_BothPlayerAndComputerPlayerBuildCities()
        {
            this.CreateStandardLocalGameControllerScenarioRunner_Old()
                .WithNoResourceCollection()
                .WithStartingResourcesForPlayer(MainPlayer, ResourceClutch.City)
                .WithStartingResourcesForPlayer(ThirdOpponent_Dana, ResourceClutch.City)
                .PlayerTurn(MainPlayerName, 5, 6)
                    .BuildCity(12)
                    .BuildCityEvent(12)
                    .EndTurn()
                .PlayerTurn(FirstOpponentName, 5, 6).EndTurn()
                .PlayerTurn(SecondOpponentName, 3, 3).EndTurn()
                .PlayerTurn(ThirdOpponentName, 3, 3)
                    .BuildCity(33)
                    .BuildCityEvent(33)
                    .EndTurn()
                .Build()
                .Run_Old();
        }

        [Test]
        public void New_Scenario_ComputerPlayerBuildsCityToWin()
        {
            var firstOpponentResources = (ResourceClutch.RoadSegment * 5) + (ResourceClutch.Settlement * 4) + (ResourceClutch.City * 4);

            this.CreateStandardLocalGameControllerScenarioRunner_Old()
                .WithNoResourceCollection()
                .WithStartingResourcesForPlayer(FirstOpponentName, firstOpponentResources)
                .PlayerTurn(MainPlayerName, 4, 4).EndTurn()
                .PlayerTurn(FirstOpponentName, 3, 3)
                        .BuildRoad(17, 7).BuildRoadEvent(17, 7)
                        .BuildRoad(17, 16).BuildRoadEvent(17, 16)
                        .BuildRoad(43, 51).BuildRoadEvent(43, 51)
                        .BuildRoad(51, 50).BuildRoadEvent(51, 50)
                        .BuildRoad(51, 52).BuildRoadEvent(51, 52)
                        .BuildSettlement(7).BuildSettlementEvent(7)
                        .BuildSettlement(16).BuildSettlementEvent(16)
                        .BuildSettlement(50).BuildSettlementEvent(50)
                        .BuildSettlement(52).BuildSettlementEvent(52)
                        .BuildCity(7).BuildCityEvent(7)
                        .BuildCity(16).BuildCityEvent(16)
                        .BuildCity(50).BuildCityEvent(50)
                        .BuildCity(52).BuildCityEvent(52)
                        .GameWinEvent(10)
                    .EndTurn()
                .Build()
                .Run_Old();
        }

        [Test]
        public void New_Scenario_ComputerPlayerHasEightVictoryPointsAndBuildsLargestArmyToWin()
        {
            var firstOpponentResources = (ResourceClutch.RoadSegment * 2) + (ResourceClutch.Settlement * 2) + 
                (ResourceClutch.City * 4) + (ResourceClutch.DevelopmentCard * 3);

            this.CreateStandardLocalGameControllerScenarioRunner_Old()
                .WithNoResourceCollection()
                .WithStartingResourcesForPlayer(FirstOpponent_Babara, firstOpponentResources)
                .PlayerTurn(MainPlayerName, 4, 4).EndTurn()
                .PlayerTurn(FirstOpponent_Babara, 3, 3)
                        .BuildRoad(17, 7).BuildRoadEvent(17, 7)
                        .BuildRoad(17, 16).BuildRoadEvent(17, 16)
                        .BuildSettlement(7).BuildSettlementEvent(7)
                        .BuildSettlement(16).BuildSettlementEvent(16)
                        .BuildCity(7).BuildCityEvent(7)
                        .BuildCity(16).BuildCityEvent(16)
                        .BuildCity(18).BuildCityEvent(18)
                        .BuildCity(43).BuildCityEvent(43)
                        .BuyDevelopmentCard(DevelopmentCardTypes.Knight).DevelopmentCardBoughtEvent()
                        .BuyDevelopmentCard(DevelopmentCardTypes.Knight).DevelopmentCardBoughtEvent()
                        .BuyDevelopmentCard(DevelopmentCardTypes.Knight).DevelopmentCardBoughtEvent()
                    .EndTurn()
                .PlayerTurn(SecondOpponent_Charlie, 3, 3).EndTurn()
                .PlayerTurn(ThirdOpponent_Dana, 3, 3).EndTurn()
                .PlayerTurn(MainPlayerName, 3, 3).EndTurn()
                .PlayerTurn(FirstOpponent_Babara, 3, 3)
                        .PlayKnightCard(4).KnightCardPlayedEvent(4)
                    .EndTurn()
                .PlayerTurn(SecondOpponent_Charlie, 3, 3).EndTurn()
                .PlayerTurn(ThirdOpponent_Dana, 3, 3).EndTurn()
                .PlayerTurn(MainPlayerName, 3, 3).EndTurn()
                .PlayerTurn(FirstOpponent_Babara, 3, 3)
                        .PlayKnightCard(0).KnightCardPlayedEvent(0)
                    .EndTurn()
                .PlayerTurn(SecondOpponent_Charlie, 3, 3).EndTurn()
                .PlayerTurn(ThirdOpponent_Dana, 3, 3).EndTurn()
                .PlayerTurn(MainPlayerName, 3, 3).EndTurn()
                .PlayerTurn(FirstOpponent_Babara, 3, 3)
                        .PlayKnightCard(4).KnightCardPlayedEvent(4)
                        .LargestArmyChangedEvent()
                        .GameWinEvent(10)
                        .State(FirstOpponent_Babara)
                            .VictoryPoints(10)
                            .End()
                    .EndTurn()
                .Build()
                .Run_Old();
        }

        [Test]
        public void New_Scenario_ComputerPlayerHasEightVictoryPointsAndBuildsLongestRoadToWin()
        {
            var firstOpponentResources = (ResourceClutch.RoadSegment * 4) + (ResourceClutch.Settlement * 2) +
                (ResourceClutch.City * 4);

            this.CreateStandardLocalGameControllerScenarioRunner_Old()
                .WithNoResourceCollection()
                .WithStartingResourcesForPlayer(FirstOpponentName, firstOpponentResources)
                .PlayerTurn(MainPlayerName, 4, 4).EndTurn()
                .PlayerTurn(FirstOpponentName, 3, 3)
                    .BuildRoad(17, 7).BuildRoadEvent(17, 7)
                    .BuildRoad(7, 8).BuildRoadEvent(7, 8)
                    .BuildRoad(8, 0).BuildRoadEvent(8, 0)
                    .BuildSettlement(7).BuildSettlementEvent(7)
                    .BuildSettlement(0).BuildSettlementEvent(0)
                    .BuildCity(7).BuildCityEvent(7)
                    .BuildCity(0).BuildCityEvent(0)
                    .BuildCity(18).BuildCityEvent(18)
                    .BuildCity(43).BuildCityEvent(43)
                    .BuildRoad(0, 1).BuildRoadEvent(0, 1)
                    .LongestRoadBuiltEvent()
                    .GameWinEvent(10)
                    .State(FirstOpponentName)
                        .VictoryPoints(10)
                        .End()
                    .EndTurn()
                .Build()
                .Run_Old();
        }

        [Test]
        public void New_Scenario_ComputerPlayerHasNineVictoryPointsAndBuildsLargestArmyToWin()
        {
            var firstOpponentResources = (ResourceClutch.RoadSegment * 3) + (ResourceClutch.Settlement * 3) +
                (ResourceClutch.City * 4) + (ResourceClutch.DevelopmentCard * 3);

            this.CreateStandardLocalGameControllerScenarioRunner_Old()
                .WithNoResourceCollection()
                .WithStartingResourcesForPlayer(FirstOpponentName, firstOpponentResources)
                .PlayerTurn(MainPlayerName, 4, 4).EndTurn()
                .PlayerTurn(FirstOpponentName, 3, 3)
                    .BuildRoad(17, 7).BuildRoadEvent(17, 7)
                    .BuildRoad(17, 16).BuildRoadEvent(17, 16)
                    .BuildRoad(44, 45).BuildRoadEvent(44, 45)
                    .BuildSettlement(7).BuildSettlementEvent(7)
                    .BuildSettlement(16).BuildSettlementEvent(16)
                    .BuildSettlement(45).BuildSettlementEvent(45)
                    .BuildCity(7).BuildCityEvent(7)
                    .BuildCity(16).BuildCityEvent(16)
                    .BuildCity(18).BuildCityEvent(18)
                    .BuildCity(43).BuildCityEvent(43)
                    .BuyDevelopmentCard(DevelopmentCardTypes.Knight).DevelopmentCardBoughtEvent()
                    .BuyDevelopmentCard(DevelopmentCardTypes.Knight).DevelopmentCardBoughtEvent()
                    .BuyDevelopmentCard(DevelopmentCardTypes.Knight).DevelopmentCardBoughtEvent()
                    .EndTurn()
                .PlayerTurn(SecondOpponentName, 3, 3).EndTurn()
                .PlayerTurn(ThirdOpponentName, 3, 3).EndTurn()
                .PlayerTurn(MainPlayerName, 3, 3).EndTurn()
                .PlayerTurn(FirstOpponentName, 3, 3)
                    .PlayKnightCard(4).KnightCardPlayedEvent(4)
                    .EndTurn()
                .PlayerTurn(SecondOpponentName, 3, 3).EndTurn()
                .PlayerTurn(ThirdOpponentName, 3, 3).EndTurn()
                .PlayerTurn(MainPlayerName, 3, 3).EndTurn()
                .PlayerTurn(FirstOpponentName, 3, 3)
                    .PlayKnightCard(0).KnightCardPlayedEvent(0)
                    .EndTurn()
                .PlayerTurn(SecondOpponentName, 3, 3).EndTurn()
                .PlayerTurn(ThirdOpponentName, 3, 3).EndTurn()
                .PlayerTurn(MainPlayerName, 3, 3).EndTurn()
                .PlayerTurn(FirstOpponentName, 3, 3)
                    .PlayKnightCard(4).KnightCardPlayedEvent(4)
                    .LargestArmyChangedEvent()
                    .GameWinEvent(11)
                    .State(FirstOpponentName)
                        .VictoryPoints(11)
                        .End()
                    .EndTurn()
                .Build()
                .Run_Old();
        }

        [Test]
        public void New_Scenario_ComputerPlayerHasNineVictoryPointsAndBuildsLongestRoadToWin()
        {
            var firstOpponentResources = (ResourceClutch.RoadSegment * 5) + (ResourceClutch.Settlement * 3) +
                (ResourceClutch.City * 4);

            this.CreateStandardLocalGameControllerScenarioRunner_Old()
                .WithNoResourceCollection()
                .WithStartingResourcesForPlayer(FirstOpponentName, firstOpponentResources)
                .PlayerTurn(MainPlayerName, 4, 4).EndTurn()
                .PlayerTurn(FirstOpponentName, 3, 3)
                        .BuildRoad(17, 7).BuildRoadEvent(17, 7)
                        .BuildRoad(7, 8).BuildRoadEvent(7, 8)
                        .BuildRoad(8, 0).BuildRoadEvent(8, 0)
                        .BuildRoad(44, 45).BuildRoadEvent(44, 45)
                        .BuildSettlement(7).BuildSettlementEvent(7)
                        .BuildSettlement(0).BuildSettlementEvent(0)
                        .BuildSettlement(45).BuildSettlementEvent(45)
                        .BuildCity(7).BuildCityEvent(7)
                        .BuildCity(0).BuildCityEvent(0)
                        .BuildCity(18).BuildCityEvent(18)
                        .BuildCity(43).BuildCityEvent(43)
                        .BuildRoad(0, 1).BuildRoadEvent(0, 1)
                        .LongestRoadBuiltEvent()
                        .GameWinEvent(11)
                        .State(FirstOpponent_Babara)
                            .VictoryPoints(11)
                        .End()
                    .EndTurn()
                .Build()
                .Run_Old();
        }

        /// <summary>
        /// Test that the transaction between players happens as expected when human plays the knight card and the robber
        /// is moved to a hex populated by two computer players.
        /// </summary>
        [Test]
        public void New_Scenario_ComputerPlayerLosesResourceWhenPlayerPlaysTheKnightCard()
        {
            var mainPlayerResources = ResourceClutch.DevelopmentCard;
            var firstOpponentResources = ResourceClutch.OneOre;
            this.CreateStandardLocalGameControllerScenarioRunner_Old()
                .WithNoResourceCollection()
                .WithStartingResourcesForPlayer(MainPlayerName, mainPlayerResources).WithStartingResourcesForPlayer(FirstOpponentName, firstOpponentResources)
                .PlayerTurn(MainPlayerName, 4, 4)
                    .BuyDevelopmentCard(DevelopmentCardTypes.Knight).DevelopmentCardBoughtEvent()
                    .State(MainPlayerName)
                        .HeldCards(DevelopmentCardTypes.Knight)
                        .End()
                    .EndTurn()
                .PlayerTurn(FirstOpponent_Babara, 3, 3).EndTurn()
                .PlayerTurn(SecondOpponentName, 3, 3).EndTurn()
                .PlayerTurn(ThirdOpponentName, 3, 3).EndTurn()
                .PlayerTurn(MainPlayerName, 4, 4)
                    .PlayKnightCard(3, FirstOpponent_Babara, ResourceTypes.Ore).KnightCardPlayedEvent(3)
                    .ResourcesGainedEvent(MainPlayer, FirstOpponent_Babara, ResourceClutch.OneOre)
                    .State(MainPlayer)
                        .Resources(ResourceClutch.OneOre)
                        .End()
                    .State(FirstOpponent_Babara)
                        .Resources(ResourceClutch.Zero)
                        .End()
                    .EndTurn()
                .Build()
                .Run_Old();
        }

        [Test]
        public void Scenario_ComputerPlayerRollsSevenAndTakesResourceFromPlayer()
        {
            var mainPlayerResources = ResourceClutch.OneWool;
            this.CreateStandardLocalGameControllerScenarioRunner_Old()
                .WithNoResourceCollection()
                .WithStartingResourcesForPlayer(MainPlayerName, mainPlayerResources)
                .PlayerTurn_Old(MainPlayerName, 3, 3)
                    .OldState(MainPlayerName)
                        .Resources(ResourceClutch.OneWool)
                        .End()
                    .OldState(FirstOpponentName)
                        .Resources(ResourceClutch.Zero)
                        .End()
                    .EndTurn()
                .PlayerTurn_Old(FirstOpponentName, 3, 4)
                    .Actions()
                        .PlaceRobber(2)
                        .ChooseResourceFromOpponent(MainPlayerName, ResourceTypes.Wool)
                        .End()
                    .Events()
                        .ResourcesGainedEvent(MainPlayerName, ResourceClutch.OneWool)
                        .End()
                    .OldState(MainPlayerName)
                        .Resources(ResourceClutch.Zero)
                        .End()
                    .OldState(FirstOpponentName)
                        .Resources(ResourceClutch.OneWool)
                        .End()
                    .EndTurn()
                .Build()
                .Run_Old();
        }

        [Test]
        public void Scenario_ComputerPlayerRollsSevenAndTakesResourcesFromComputerPlayer()
        {
            Assert.Fail("Not Implemented");
        }

        [Test]
        public void New_Scenario_ComputerPlayerRollsSeventAndAllPlayersWithMoreThanSevenResourcesLosesResources()
        {
            this.CreateStandardLocalGameControllerScenarioRunner_Old()
                .WithNoResourceCollection()
                .WithStartingResourcesForPlayer(MainPlayerName, ResourceClutch.OneBrick * 7)
                .WithStartingResourcesForPlayer(FirstOpponentName, ResourceClutch.OneBrick * 8)
                .WithStartingResourcesForPlayer(SecondOpponentName, ResourceClutch.OneBrick * 9)
                .WithStartingResourcesForPlayer(ThirdOpponentName, ResourceClutch.OneBrick * 10)
                .PlayerTurn(MainPlayerName, 3, 3)
                    .DiceRollEvent(3, 3)
                    .EndTurn()
                .PlayerTurn(FirstOpponentName, 3, 4)
                    .DiceRollEvent(3, 4)
                    .DropResources(MainPlayerName, ResourceClutch.OneBrick * 4)
                    .DropResources(FirstOpponentName, ResourceClutch.OneBrick * 4)
                    .DropResources(SecondOpponentName, ResourceClutch.OneBrick * 5)
                    .DropResources(ThirdOpponentName, ResourceClutch.OneBrick * 5)
                    .ResourcesLostEvent(
                        new Tuple<string, ResourceClutch>(FirstOpponentName, ResourceClutch.OneBrick * 4),
                        new Tuple<string, ResourceClutch>(SecondOpponentName, ResourceClutch.OneBrick * 5),
                        new Tuple<string, ResourceClutch>(ThirdOpponentName, ResourceClutch.OneBrick * 5))
                    .State(MainPlayerName)
                        .Resources(ResourceClutch.OneBrick * 3)
                        .End()
                    .State(FirstOpponentName)
                        .Resources(ResourceClutch.OneBrick * 4)
                        .End()
                    .State(SecondOpponentName)
                        .Resources(ResourceClutch.OneBrick * 4)
                        .End()
                    .State(ThirdOpponentName)
                        .Resources(ResourceClutch.OneBrick * 5)
                        .End()
                    .EndTurn()
                .Build()
                .Run_Old();
        }

        /// <summary>
        /// Test that the largest army event is not raised when the player plays knight cards and already has the largest army
        /// </summary>
        [Test]
        public void Scenario_LargestArmyEventOnlyRaisedFirstTimeThatPlayerHasMostKnightCardsPlayed()
        {
            var mainPlayerResources = ResourceClutch.DevelopmentCard * 4;
            this.CreateStandardLocalGameControllerScenarioRunner_Old()
                .WithNoResourceCollection()
                .WithStartingResourcesForPlayer(MainPlayerName, mainPlayerResources)
                .PlayerTurn_Old(MainPlayerName, 4, 4)
                    .Actions()
                        .BuyDevelopmentCard(DevelopmentCardTypes.Knight)
                        .BuyDevelopmentCard(DevelopmentCardTypes.Knight)
                        .BuyDevelopmentCard(DevelopmentCardTypes.Knight)
                        .BuyDevelopmentCard(DevelopmentCardTypes.Knight)
                        .End()
                    .EndTurn()
                .PlayerTurn_Old(FirstOpponentName, 3, 3).EndTurn()
                .PlayerTurn_Old(SecondOpponentName, 3, 3).EndTurn()
                .PlayerTurn_Old(ThirdOpponentName, 3, 3).EndTurn()
                .PlayerTurn_Old(MainPlayerName, 4, 4)
                    .Actions()
                        .PlayKnightCard(4)
                        .End()
                    .EndTurn()
                .PlayerTurn_Old(FirstOpponentName, 3, 3).EndTurn()
                .PlayerTurn_Old(SecondOpponentName, 3, 3).EndTurn()
                .PlayerTurn_Old(ThirdOpponentName, 3, 3).EndTurn()
                .PlayerTurn_Old(MainPlayerName, 4, 4)
                    .Actions()
                        .PlayKnightCard(0)
                        .End()
                    .EndTurn()
                .PlayerTurn_Old(FirstOpponentName, 3, 3).EndTurn()
                .PlayerTurn_Old(SecondOpponentName, 3, 3).EndTurn()
                .PlayerTurn_Old(ThirdOpponentName, 3, 3).EndTurn()
                .PlayerTurn_Old(MainPlayerName, 4, 4)
                    .Actions()
                        .PlayKnightCard(4)
                        .End()
                    .EndTurn()
                .PlayerTurn_Old(FirstOpponentName, 3, 3).EndTurn()
                .PlayerTurn_Old(SecondOpponentName, 3, 3).EndTurn()
                .PlayerTurn_Old(ThirdOpponentName, 3, 3).EndTurn()
                .PlayerTurn_Old(MainPlayerName, 4, 4)
                    .Actions()
                        .PlayKnightCard(0)
                        .End()
                    .EndTurn()
                .PlayerTurn_Old(FirstOpponentName, 3, 3).EndTurn()
                .Build()
                .Run_Old();
        }

        /// <summary>
        /// Test that the largest army event is not sent when the opponent plays knight cards and already has the largest army
        /// </summary>
        [Test]
        public void Scenario_LargestArmyEventOnlyReturnedFirstTimeThatOpponentHasMostKnightCardsPlayed()
        {
            var firstOpponentResources = ResourceClutch.DevelopmentCard * 4;
            this.CreateStandardLocalGameControllerScenarioRunner_Old()
                .WithNoResourceCollection()
                .WithStartingResourcesForPlayer(FirstOpponentName, firstOpponentResources)
                .PlayerTurn_Old(MainPlayerName, 4, 4).EndTurn()
                .PlayerTurn_Old(FirstOpponentName, 3, 3)
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
                .PlayerTurn_Old(SecondOpponentName, 3, 3).EndTurn()
                .PlayerTurn_Old(ThirdOpponentName, 3, 3).EndTurn()
                .PlayerTurn_Old(MainPlayerName, 4, 4).EndTurn()
                .PlayerTurn_Old(FirstOpponentName, 3, 3)
                    .Actions()
                        .PlayKnightCard(4)
                        .End()
                    .Events()
                        .PlayKnightCardEvent()
                        .End()
                    .EndTurn()
                .PlayerTurn_Old(SecondOpponentName, 3, 3).EndTurn()
                .PlayerTurn_Old(ThirdOpponentName, 3, 3).EndTurn()
                .PlayerTurn_Old(MainPlayerName, 4, 4).EndTurn()
                .PlayerTurn_Old(FirstOpponentName, 3, 3)
                    .Actions()
                        .PlayKnightCard(0)
                        .End()
                    .Events()
                        .PlayKnightCardEvent()
                        .End()
                    .EndTurn()
                .PlayerTurn_Old(SecondOpponentName, 3, 3).EndTurn()
                .PlayerTurn_Old(ThirdOpponentName, 3, 3).EndTurn()
                .PlayerTurn_Old(MainPlayerName, 4, 4).EndTurn()
                .PlayerTurn_Old(FirstOpponentName, 3, 3)
                    .Actions()
                        .PlayKnightCard(4)
                        .End()
                    .Events()
                        .PlayKnightCardEvent()
                        .LargestArmyChangedEvent(null)
                        .End()
                    .EndTurn()
                .PlayerTurn_Old(SecondOpponentName, 3, 3).EndTurn()
                .PlayerTurn_Old(ThirdOpponentName, 3, 3).EndTurn()
                .PlayerTurn_Old(MainPlayerName, 4, 4).EndTurn()
                .PlayerTurn_Old(FirstOpponentName, 3, 3)
                    .Actions()
                        .PlayKnightCard(0)
                        .End()
                    .Events()
                        .PlayKnightCardEvent()
                        .NoEventOfType<LargestArmyChangedEvent>()
                        .End()
                    .EndTurn()
                .Build()
                .Run_Old();
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
            this.CreateStandardLocalGameControllerScenarioRunner_Old()
                .WithNoResourceCollection()
                .WithStartingResourcesForPlayer(MainPlayerName, mainPlayerResources)
                .WithStartingResourcesForPlayer(FirstOpponentName, firstOpponentResources)
                .PlayerTurn_Old(MainPlayerName, 4, 4)
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
                .PlayerTurn_Old(FirstOpponentName, 3, 3)
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
                .PlayerTurn_Old(SecondOpponentName, 3, 3).EndTurn()
                .PlayerTurn_Old(ThirdOpponentName, 3, 3).EndTurn()
                .PlayerTurn_Old(MainPlayerName, 4, 4)
                    .Actions()
                        .PlayKnightCard(4)
                        .End()
                    .Events()
                        .PlayKnightCardEvent()
                        .End()
                    .EndTurn()
                .PlayerTurn_Old(FirstOpponentName, 3, 3)
                    .Actions()
                        .PlayKnightCard(0)
                        .End()
                    .Events()
                        .PlayKnightCardEvent()
                        .End()
                    .EndTurn()
                .PlayerTurn_Old(SecondOpponentName, 3, 3).EndTurn()
                .PlayerTurn_Old(ThirdOpponentName, 3, 3).EndTurn()
                .PlayerTurn_Old(MainPlayerName, 4, 4)
                    .Actions()
                        .PlayKnightCard(4)
                        .End()
                    .Events()
                        .PlayKnightCardEvent()
                        .End()
                    .EndTurn()
                .PlayerTurn_Old(FirstOpponentName, 3, 3)
                    .Actions()
                        .PlayKnightCard(0)
                        .End()
                    .Events()
                        .PlayKnightCardEvent()
                        .End()
                    .EndTurn()
                .PlayerTurn_Old(SecondOpponentName, 3, 3).EndTurn()
                .PlayerTurn_Old(ThirdOpponentName, 3, 3).EndTurn()
                .PlayerTurn_Old(MainPlayerName, 4, 4)
                    .Actions()
                        .PlayKnightCard(4)
                        .End()
                    .Events()
                        .PlayKnightCardEvent()
                        .LargestArmyChangedEvent(null)
                        .End()
                    .OldState(MainPlayerName)
                        .VictoryPoints(4)
                        .End()
                    .OldState(FirstOpponentName)
                        .VictoryPoints(2)
                        .End()
                    .EndTurn()
                .PlayerTurn_Old(FirstOpponentName, 3, 3)
                    .Actions()
                        .PlayKnightCard(0)
                        .End()
                    .Events()
                        .PlayKnightCardEvent()
                        .End()
                    .EndTurn()
                .PlayerTurn_Old(SecondOpponentName, 3, 3).EndTurn()
                .PlayerTurn_Old(ThirdOpponentName, 3, 3).EndTurn()
                .PlayerTurn_Old(MainPlayerName, 4, 4).EndTurn()
                .PlayerTurn_Old(FirstOpponentName, 3, 3)
                    .Actions()
                        .PlayKnightCard(4)
                        .End()
                    .Events()
                        .PlayKnightCardEvent()
                        .LargestArmyChangedEvent(MainPlayerName)
                        .End()
                    .OldState(MainPlayerName)
                        .VictoryPoints(2)
                        .End()
                    .OldState(FirstOpponentName)
                        .VictoryPoints(4)
                        .End()
                    .EndTurn()
                .Build()
                .Run_Old();
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
                .PlayerTurn_Old(MainPlayerName, 4, 4)
                    .Actions()
                        .BuyDevelopmentCard(DevelopmentCardTypes.Knight)
                        .BuyDevelopmentCard(DevelopmentCardTypes.Knight)
                        .BuyDevelopmentCard(DevelopmentCardTypes.Knight)
                        .BuyDevelopmentCard(DevelopmentCardTypes.Knight)
                        .End()
                    .EndTurn()
                .PlayerTurn_Old(FirstOpponentName, 3, 3).EndTurn()
                .PlayerTurn_Old(SecondOpponentName, 3, 3).EndTurn()
                .PlayerTurn_Old(ThirdOpponentName, 3, 3).EndTurn()
                .PlayerTurn_Old(MainPlayerName, 4, 4)
                    .Actions()
                        .PlayKnightCard(4)
                        .End()
                    .EndTurn()
                .PlayerTurn_Old(FirstOpponentName, 3, 3).EndTurn()
                .PlayerTurn_Old(SecondOpponentName, 3, 3).EndTurn()
                .PlayerTurn_Old(ThirdOpponentName, 3, 3).EndTurn()
                .PlayerTurn_Old(MainPlayerName, 4, 4)
                    .Actions()
                        .PlayKnightCard(0)
                        .End()
                    .EndTurn()
                .PlayerTurn_Old(FirstOpponentName, 3, 3).EndTurn()
                .PlayerTurn_Old(SecondOpponentName, 3, 3).EndTurn()
                .PlayerTurn_Old(ThirdOpponentName, 3, 3).EndTurn()
                .PlayerTurn_Old(MainPlayerName, 4, 4)
                    .Actions()
                        .PlayKnightCard(4)
                        .End()
                    .OldState(MainPlayerName)
                        .VictoryPoints(4)
                        .End()
                    .EndTurn()
                .PlayerTurn_Old(FirstOpponentName, 3, 3).EndTurn()
                .PlayerTurn_Old(SecondOpponentName, 3, 3).EndTurn()
                .PlayerTurn_Old(ThirdOpponentName, 3, 3).EndTurn()
                .PlayerTurn_Old(MainPlayerName, 4, 4)
                    .Actions()
                        .PlayKnightCard(0)
                        .End()
                    .OldState(MainPlayerName)
                        .VictoryPoints(4)
                        .End()
                    .EndTurn()
                .Build()
                .Run_Old();
        }

        [Test]
        public void Scenario_PlayerLosesResourceWhenComputerPlayerPlaysTheKnightCard()
        {
            var mainPlayerResources = ResourceClutch.OneOre;
            var firstOpponentResources = ResourceClutch.DevelopmentCard;
            this.CreateStandardLocalGameControllerScenarioRunner_Old()
                .WithNoResourceCollection()
                .WithStartingResourcesForPlayer(MainPlayerName, mainPlayerResources).WithStartingResourcesForPlayer(FirstOpponentName, firstOpponentResources)
                .PlayerTurn_Old(MainPlayerName, 4, 4).EndTurn()
                .PlayerTurn_Old(FirstOpponentName, 3, 3)
                    .Actions()
                        .BuyDevelopmentCard(DevelopmentCardTypes.Knight)
                        .End()
                    .Events()
                        .BuyDevelopmentCardEvent(DevelopmentCardTypes.Knight)
                        .End()
                    .EndTurn()
                .PlayerTurn_Old(SecondOpponentName, 3, 3).EndTurn()
                .PlayerTurn_Old(ThirdOpponentName, 3, 3).EndTurn()
                .PlayerTurn_Old(MainPlayerName, 4, 4).EndTurn()
                .PlayerTurn_Old(FirstOpponentName, 3, 3)
                    .Actions()
                        .PlayKnightCardAndCollectFrom(3, MainPlayerName, ResourceTypes.Ore)
                        .End()
                    .Events()
                        .PlayKnightCardEvent()
                        .ResourcesGainedEvent(MainPlayerName, ResourceClutch.OneOre)
                        .End()
                    .EndTurn()
                .Build()
                .Run_Old();
        }

        [Test]
        [TestCase(7, 0)]
        [TestCase(8, 4)]
        [TestCase(9, 4)]
        [TestCase(10, 5)]
        public void Scenario_PlayerRollsSevenAndReceivesReceivesRobberEventWithDropResourceCardsCount(int resourcesCount, int expectedResourcesToDrop)
        {
            var mainPlayerResources = ResourceClutch.OneBrick * resourcesCount;
            this.CreateStandardLocalGameControllerScenarioRunner_Old()
                .WithNoResourceCollection()
                .WithStartingResourcesForPlayer(MainPlayerName, mainPlayerResources)
                .PlayerTurn_Old(MainPlayerName, 3, 4)
                    .Events()
                        .RobberEvent(expectedResourcesToDrop)
                        .End()
                    .EndTurn()
                .Build()
                .Run_Old();
        }

        [Test]
        public void Scenario_PlayerRollsSevenDoesNotPassBackResourcesAndReceivesErrorMessage()
        {
            var mainPlayerResources = ResourceClutch.OneBrick * 8;
            this.CreateStandardLocalGameControllerScenarioRunner_Old()
                .WithNoResourceCollection()
                .WithStartingResourcesForPlayer(MainPlayerName, mainPlayerResources)
                .PlayerTurn_Old(MainPlayerName, 3, 4)
                    .Actions()
                        .PlaceRobber(3)
                        .End()
                    .Events()
                        .ErrorMessageEvent("Cannot set robber location until expected resources (4) have been dropped via call to DropResources method.")
                        .End()
                    .EndTurn()
                .Build()
                .Run_Old();
        }

        [Test]
        public void Scenario_PlayerRollsSevenAndSelectedHexHasSingleComputerPlayer()
        {
            var firstOpponentResources = ResourceClutch.OneGrain * 2;
        
            this.CreateStandardLocalGameControllerScenarioRunner_Old()
                .WithNoResourceCollection()
                .WithStartingResourcesForPlayer(FirstOpponentName, firstOpponentResources)
                .PlayerTurn_Old(MainPlayerName, 3, 4)
                    .Actions()
                        .PlaceRobber(3, FirstOpponentName, ResourceTypes.Grain)
                        .End()
                    .Events()
                        .RobbingChoicesEvent(new Tuple<string, int>(FirstOpponentName, 2))
                        .End()
                    .EndTurn()
                .Build()
                .Run_Old();
        }

        [Test]
        public void Scenario_PlayerRollsSevenAndAllPlayersWithMoreThanSevenResourcesLosesResources()
        {
            this.CreateStandardLocalGameControllerScenarioRunner_Old()
                .WithNoResourceCollection()
                .WithStartingResourcesForPlayer(MainPlayerName, ResourceClutch.OneBrick * 7)
                .WithStartingResourcesForPlayer(FirstOpponentName, ResourceClutch.OneBrick * 8)
                .WithStartingResourcesForPlayer(SecondOpponentName, ResourceClutch.OneBrick * 9)
                .WithStartingResourcesForPlayer(ThirdOpponentName, ResourceClutch.OneBrick * 10)
                .PlayerTurn_Old(MainPlayerName, 3, 4)
                    .Responses()
                        .WhenDiceRollIsSevenThenDropResources(MainPlayerName, ResourceClutch.OneBrick * 4)
                        .WhenDiceRollIsSevenThenDropResources(FirstOpponentName, ResourceClutch.OneBrick * 4)
                        .WhenDiceRollIsSevenThenDropResources(SecondOpponentName, ResourceClutch.OneBrick * 5)
                        .WhenDiceRollIsSevenThenDropResources(ThirdOpponentName, ResourceClutch.OneBrick * 5)
                        .End()
                    .Events()
                        .ResourcesLostEvent(new Tuple<string, ResourceClutch>(FirstOpponentName, ResourceClutch.OneBrick * 4),
                            new Tuple<string, ResourceClutch>(SecondOpponentName, ResourceClutch.OneBrick * 5),
                            new Tuple<string, ResourceClutch>(ThirdOpponentName, ResourceClutch.OneBrick * 5))
                        .End()
                    .OldState(MainPlayerName)
                        .Resources(ResourceClutch.OneBrick * 3)
                        .End()
                    .OldState(FirstOpponentName)
                        .Resources(ResourceClutch.OneBrick * 4)
                        .End()
                    .OldState(SecondOpponentName)
                        .Resources(ResourceClutch.OneBrick * 4)
                        .End()
                    .OldState(ThirdOpponentName)
                        .Resources(ResourceClutch.OneBrick * 5)
                        .End()
                    .EndTurn()
                .Build()
                .Run_Old();
        }

        /// <summary>
        /// The robber hex set by the player has no adjacent settlements so the returned robbing choices
        /// is null.
        /// </summary>
        [Test]
        public void Scenario_PlayerRollsSevenAndSelectedHexHasNoPlayers()
        {
            this.CreateStandardLocalGameControllerScenarioRunner_Old()
                .WithNoResourceCollection()
                .PlayerTurn_Old(MainPlayerName, 3, 4)
                    .Actions()
                        .PlaceRobber(4)
                        .End()
                    .Events()
                        .RobbingChoicesEvent(null)
                        .End()
                    .EndTurn()
                .Build()
                .Run_Old();
        }

        /// <summary>
        /// The robber hex set by the player has only player settlements so the returned robbing choices is null.
        /// </summary>
        [Test]
        public void Scenario_PlayerRollsSevenAndSelectedHexHasPlayerOnly()
        {
            this.CreateStandardLocalGameControllerScenarioRunner_Old()
                .WithNoResourceCollection()
                .PlayerTurn_Old(MainPlayerName, 3, 4)
                    .Actions()
                        .PlaceRobber(2)
                        .End()
                    .Events()
                        .RobbingChoicesEvent(null)
                        .End()
                    .EndTurn()
                .Build()
                .Run_Old();
        }

        [Test]
        public void Scenario_PlayerRollsSevenAndGetsResourceFromSelectedComputerPlayer()
        {
            var firstOpponentResources = ResourceClutch.OneWool;
            this.CreateStandardLocalGameControllerScenarioRunner_Old()
                .WithNoResourceCollection()
                .WithStartingResourcesForPlayer(FirstOpponentName, firstOpponentResources)
                .PlayerTurn_Old(MainPlayerName, 3, 4)
                    .Actions()
                        .PlaceRobber(3)
                        .ChooseResourceFromOpponent(FirstOpponentName, ResourceTypes.Wool)
                        .End()
                    .Events()
                        .RobbingChoicesEvent(new Tuple<string, int>(FirstOpponentName, 1))
                        .ResourcesGainedEvent(FirstOpponentName, ResourceClutch.OneWool)
                        .End()
                    .OldState(MainPlayerName)
                        .Resources(ResourceClutch.OneWool)
                        .End()
                    .OldState(FirstOpponentName)
                        .Resources(ResourceClutch.Zero)
                        .End()
                    .EndTurn()
                .Build()
                .Run_Old();
        }

        /// <summary>
        /// Passing in an id of a player that is not on the selected robber hex when choosing the resource 
        /// causes an error to be raised.
        /// </summary>
        [Test]
        public void Scenario_PlayerRollsSevenAndSelectsInvalidOpponentGettingErrorMessage()
        {
            var firstOpponentResources = ResourceClutch.OneWool;
            this.CreateStandardLocalGameControllerScenarioRunner_Old()
                .WithNoResourceCollection()
                .WithStartingResourcesForPlayer(FirstOpponentName, firstOpponentResources)
                .PlayerTurn_Old(MainPlayerName, 3, 4)
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
                .Run_Old();
        }

        /// <summary>
        /// The robber hex set by the player has only player settlements so calling the CallingChooseResourceFromOpponent 
        /// method raises an error
        /// </summary>
        [Test]
        public void Scenario_RobberLocationOnlyHasPlayerSettlementsSoChooseResourceFromOpponentCallReturnsErrorMessage()
        {
            var mainPlayerResources = ResourceClutch.OneWool;
            this.CreateStandardLocalGameControllerScenarioRunner_Old()
                .WithNoResourceCollection()
                .WithStartingResourcesForPlayer(MainPlayerName, mainPlayerResources)
                .PlayerTurn_Old(MainPlayerName, 3, 4)
                    .Actions()
                        .PlaceRobber(2)
                        .ChooseResourceFromOpponent(MainPlayerName, ResourceTypes.Wool)
                        .End()
                    .Events()
                        .ErrorMessageEvent("Cannot call 'ChooseResourceFromOpponent' when no robbing choices are available.")
                        .End()
                    .EndTurn()
                .Build()
                .Run_Old();
        }

        [Test]
        public void New_Scenario_PlayerTradesOneResourceWithComputerPlayer(string[] args)
        {
            var mainPlayerResources = ResourceClutch.OneWool;
            var firstOpponentResources = ResourceClutch.OneGrain;
            this.CreateStandardLocalGameControllerScenarioRunner_Old()
                .WithNoResourceCollection()
                .WithStartingResourcesForPlayer(MainPlayerName, mainPlayerResources)
                .WithStartingResourcesForPlayer(FirstOpponent_Babara, firstOpponentResources)
                .PlayerTurn(MainPlayerName, 3, 3)
                    .MakeDirectTradeOffer(ResourceClutch.OneGrain,
                        new Tuple<string, ResourceClutch>(FirstOpponent_Babara, ResourceClutch.OneWool))
                    .FinaliseTrade(ResourceClutch.OneWool, FirstOpponent_Babara, ResourceClutch.OneGrain)
                    .TradeWithPlayerCompletedEvent(MainPlayer, FirstOpponent_Babara, ResourceClutch.OneWool, ResourceClutch.OneGrain)
                    .State(MainPlayer)
                        .Resources(ResourceClutch.OneGrain)
                        .End()
                    .State(FirstOpponent_Babara)
                        .Resources(ResourceClutch.OneWool)
                        .End()
                    .EndTurn()
                .Build()
                .Run_Old();
        }

        [Scenario]
        public void New_Scenario_ComputerPlayerTradesOneResourceWithPlayer(string[] args)
        {
            var mainPlayerResources = ResourceClutch.OneWool;
            var firstOpponentResources = ResourceClutch.OneGrain;
            this.CreateStandardLocalGameControllerScenarioRunner(args)
                .WithNoResourceCollection()
                .WithStartingResourcesForPlayer(MainPlayerName, mainPlayerResources)
                .WithStartingResourcesForPlayer(FirstOpponent_Babara, firstOpponentResources)
                .PlayerTurn(MainPlayerName, 3, 3).EndTurn()
                .PlayerTurn(FirstOpponent_Babara, 3, 3)
                    .MakeDirectTradeOffer(ResourceClutch.OneWool)
                    .MakeDirectTradeOfferEvent(MainPlayer, FirstOpponent_Babara, ResourceClutch.OneWool)
                    .MakeDirectTradeOfferEvent(SecondOpponent_Charlie, FirstOpponent_Babara, ResourceClutch.OneWool)
                    .MakeDirectTradeOfferEvent(ThirdOpponent_Dana, FirstOpponent_Babara, ResourceClutch.OneWool)
                    .AnswerDirectTradeOffer(MainPlayer, ResourceClutch.OneGrain)
                    .AnswerDirectTradeOfferEvent(FirstOpponent_Babara, MainPlayer, ResourceClutch.OneGrain)
                    .TradeWithPlayerCompletedEvent(FirstOpponent_Babara, MainPlayer, ResourceClutch.OneGrain, ResourceClutch.OneWool)
                    .State(MainPlayer)
                        .Resources(ResourceClutch.OneGrain)
                        .End()
                    .State(FirstOpponent_Babara)
                        .Resources(ResourceClutch.OneWool)
                        .End()
                    .EndTurn()
                .Build()
                .Run_Old();
        }

        private LocalGameControllerScenarioRunner CreateStandardLocalGameControllerScenarioRunner_Old()
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

        [Test]
        [Scenario]
        public void Scenario_AllPlayersHaveDiceRollEventInTheirTurn()
        {
            LocalGameController()
                .WithHumanPlayer(MainPlayerName)
                .WithComputerPlayer2(FirstOpponentName)
                .WithComputerPlayer2(SecondOpponentName)
                .WithComputerPlayer2(ThirdOpponentName)
                .WithStartingResourcesForPlayer(MainPlayerName, ResourceClutch.DevelopmentCard)
                .WithNoResourceCollection()
                .PlayerTurn(MainPlayerName, 4, 4)
                    .DiceRollEvent(4, 4)
                    .EndTurn()
                .PlayerTurn(FirstOpponentName, 1, 2)
                    .DiceRollEvent(1, 2)
                    .EndTurn()
                .PlayerTurn(SecondOpponentName, 3, 4)
                    .DiceRollEvent(3, 4)
                    .EndTurn()
                .PlayerTurn(ThirdOpponentName, 5, 6)
                    .DiceRollEvent(5, 6)
                    .EndTurn()
                .Run();
        }

        /*[Scenario]
        public void Scenario_AllPlayersCompleteSetup(string[] args)
        {
            this.CreateStandardLocalGameControllerScenarioRunner(args)
                .WithNoResourceCollection()
                .InitialBoardSetupEvent()
                .PlayerSetupTurn(MainPlayer, MainPlayerFirstSettlementLocation, MainPlayerFirstRoadEnd)
                .PlayerSetupTurn(FirstOpponent_Babara, FirstOpponentFirstSettlementLocation, FirstOpponentFirstRoadEnd)
                .PlayerSetupTurn(SecondOpponent_Charlie, SecondOpponentFirstSettlementLocation, SecondOpponentFirstRoadEnd)
                .PlayerSetupTurn(ThirdOpponent_Dana, ThirdOpponentFirstSettlementLocation, ThirdOpponentFirstRoadEnd)
                .PlayerSetupTurn(ThirdOpponent_Dana, ThirdOpponentSecondSettlementLocation, ThirdOpponentSecondRoadEnd)
                .PlayerSetupTurn(SecondOpponent_Charlie, SecondOpponentSecondSettlementLocation, SecondOpponentSecondRoadEnd)
                .PlayerSetupTurn(FirstOpponent_Babara, FirstOpponentSecondSettlementLocation, FirstOpponentSecondRoadEnd)
                .PlayerSetupTurn(MainPlayer, MainPlayerSecondSettlementLocation, MainPlayerSecondRoadEnd)
                .Run();
        }*/

        private LocalGameControllerScenarioRunner CreateStandardLocalGameControllerScenarioRunner(string[] args)
        {
            return LocalGameController(args)
                .WithHumanPlayer(MainPlayer)
                .WithComputerPlayer2(FirstOpponent_Babara)
                .WithComputerPlayer2(SecondOpponent_Charlie)
                .WithComputerPlayer2(ThirdOpponent_Dana)
                .WithTurnOrder(MainPlayer, FirstOpponent_Babara, SecondOpponent_Charlie, ThirdOpponent_Dana);
        }

        [Test]
        [Scenario]
        public void Scenario_AllPlayersCollectResourcesAsPartOfTurnStart()
        {
            var localGameController = this.CreateStandardLocalGameControllerScenarioRunner_Old()
                .PlayerTurn(MainPlayerName, 4, 4)
                    .ResourceCollectedEvent(MainPlayerName,
                        new Tuple<uint, ResourceClutch>(MainPlayerFirstSettlementLocation, ResourceClutch.OneBrick))
                    .ResourceCollectedEvent(FirstOpponentName,
                        new Tuple<uint, ResourceClutch>(FirstOpponentSecondSettlementLocation, ResourceClutch.OneGrain))
                    .EndTurn()
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
                .Run_Old();
        }
    }
}
