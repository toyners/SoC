
namespace Jabberwocky.SoC.Library.UnitTests.LocalGameController_Tests
{
    using System;
    using System.Collections.Generic;
    using GameEvents;
    using Jabberwocky.SoC.Library.UnitTests.Extensions;
    using Mock;
    using NUnit.Framework;
    using Shouldly;

    [TestFixture]
    [Category("All")]
    [Category("LocalGameController")]
    [Category("LocalGameController.BuildSettlement")]
    public class LocalGameController_BuildSettlement_Tests : LocalGameControllerTestBase
    {
        #region Methods
        [Test]
        public void BuildSettlement_ValidScenario_SettlementBuiltEventRaised()
        {
            // Arrange
            var testInstances = LocalGameControllerTestCreator.CreateTestInstances(new MockGameBoardWithNoResourcesCollected());
            var localGameController = testInstances.LocalGameController;
            LocalGameControllerTestSetup.LaunchGameAndCompleteSetup(localGameController);

            testInstances.Dice.AddSequence(new[] { 8u });

            var player = testInstances.MainPlayer;
            player.AddResources(ResourceClutch.RoadSegment); // Need resources to build the precursor road
            player.AddResources(ResourceClutch.Settlement);

            Boolean settlementBuilt = false;
            localGameController.SettlementBuiltEvent = () => { settlementBuilt = true; };

            TurnToken turnToken = null;
            localGameController.StartPlayerTurnEvent = (TurnToken t) => { turnToken = t; };
            localGameController.StartGamePlay();
            localGameController.BuildRoadSegment(turnToken, MainRoadOneEnd, 3);

            // Act
            localGameController.BuildSettlement(turnToken, 3);

            // Assert
            settlementBuilt.ShouldBeTrue();
            player.VictoryPoints.ShouldBe(3u);
            player.ResourcesCount.ShouldBe(0);
        }

        [Test]
        [TestCase(0, 1, 1, 0, 1, "Cannot build settlement. Missing 1 brick.")] // Missing brick
        [TestCase(1, 0, 1, 0, 1, "Cannot build settlement. Missing 1 grain.")] // Missing grain
        [TestCase(1, 1, 0, 0, 1, "Cannot build settlement. Missing 1 lumber.")] // Missing lumber
        [TestCase(1, 1, 1, 0, 0, "Cannot build settlement. Missing 1 wool.")] // Missing wool
        [TestCase(0, 0, 0, 0, 0, "Cannot build settlement. Missing 1 brick and 1 grain and 1 lumber and 1 wool.")] // Missing all
        [TestCase(0, 1, 1, 0, 0, "Cannot build settlement. Missing 1 brick and 1 wool.")] // Missing brick and wool
        [TestCase(1, 0, 0, 0, 1, "Cannot build settlement. Missing 1 grain and 1 lumber.")] // Missing grain and lumber
        public void BuildSettlement_InsufficientResources_MeaningfulErrorIsReceived(Int32 brickCount, Int32 grainCount, Int32 lumberCount, Int32 oreCount, Int32 woolCount, String expectedMessage)
        {
            // Arrange
            var testInstances = LocalGameControllerTestCreator.CreateTestInstances(new MockGameBoardWithNoResourcesCollected());
            var localGameController = testInstances.LocalGameController;
            var player = testInstances.MainPlayer;
            LocalGameControllerTestSetup.LaunchGameAndCompleteSetup(localGameController);

            testInstances.Dice.AddSequence(new[] { 8u });

            player.AddResources(ResourceClutch.RoadSegment); // Need resources to build the precursor road
            player.AddResources(new ResourceClutch(brickCount, grainCount, lumberCount, oreCount, woolCount));

            Boolean settlementBuilt = false;
            localGameController.SettlementBuiltEvent = () => { settlementBuilt = true; };

            ErrorDetails errorDetails = null;
            localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

            TurnToken turnToken = null;
            localGameController.StartPlayerTurnEvent = (TurnToken t) => { turnToken = t; };
            localGameController.StartGamePlay();
            localGameController.BuildRoadSegment(turnToken, MainRoadOneEnd, 3);

            // Act
            localGameController.BuildSettlement(turnToken, 3);

            // Assert
            settlementBuilt.ShouldBeFalse();
            errorDetails.ShouldNotBeNull();
            errorDetails.Message.ShouldBe(expectedMessage);
        }

        [Test]
        public void BuildSettlement_AllSettlementsAreBuilt_MeaningfulErrorIsReceived()
        {
            // Arrange
            MockDice mockDice = null;
            MockPlayer player;
            MockComputerPlayer firstOpponent, secondOpponent, thirdOpponent;
            var localGameController = this.CreateLocalGameControllerAndCompleteGameSetup(out mockDice, out player, out firstOpponent, out secondOpponent, out thirdOpponent);
            mockDice.AddSequence(new[] { 8u });
            player.AddResources(ResourceClutch.Settlement * 3);
            player.AddResources(ResourceClutch.RoadSegment * 7);

            ErrorDetails errorDetails = null;
            localGameController.ErrorRaisedEvent = (ErrorDetails e) =>
            {
                if (errorDetails != null)
                {
              // Ensure that the error details are only received once.
              throw new Exception("Already received error details");
                }

                errorDetails = e;
            };

            TurnToken turnToken = null;
            localGameController.StartPlayerTurnEvent = (TurnToken t) => { turnToken = t; };
            localGameController.StartGamePlay();

            var roadSegmentDetails = new UInt32[] { 4, 3, 3, 2, 2, 1, 1, 0, 0, 8, 8, 7, 7, 17 };
            for (var index = 0; index < roadSegmentDetails.Length; index += 2)
            {
                localGameController.BuildRoadSegment(turnToken, roadSegmentDetails[index], roadSegmentDetails[index + 1]);
            }

            localGameController.BuildSettlement(turnToken, 3);
            localGameController.BuildSettlement(turnToken, 1);
            localGameController.BuildSettlement(turnToken, 8);

            // Act
            localGameController.BuildSettlement(turnToken, 17);

            // Assert
            errorDetails.ShouldNotBeNull();
            errorDetails.Message.ShouldBe("Cannot build settlement. All settlements already built.");
        }

        [Test]
        public void BuildSettlement_InsufficientResourcesAfterBuildingSettlement_MeaningfulErrorIsReceived()
        {
            // Arrange
            var testInstances = LocalGameControllerTestCreator.CreateTestInstances(new MockGameBoardWithNoResourcesCollected());
            var localGameController = testInstances.LocalGameController;
            var player = testInstances.MainPlayer;
            LocalGameControllerTestSetup.LaunchGameAndCompleteSetup(localGameController);
            testInstances.Dice.AddSequence(new[] { 8u });

            player.AddResources(ResourceClutch.RoadSegment * 3);
            player.AddResources(ResourceClutch.Settlement);

            Int32 settlementBuilt = 0;
            localGameController.SettlementBuiltEvent = () => { settlementBuilt++; };

            ErrorDetails errorDetails = null;
            localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

            TurnToken turnToken = null;
            localGameController.StartPlayerTurnEvent = (TurnToken t) => { turnToken = t; };
            localGameController.StartGamePlay();
            localGameController.BuildRoadSegment(turnToken, MainRoadOneEnd, 3);
            localGameController.BuildRoadSegment(turnToken, 3, 2);
            localGameController.BuildRoadSegment(turnToken, 2, 1);
            localGameController.BuildSettlement(turnToken, 3);

            // Act
            localGameController.BuildSettlement(turnToken, 1);

            // Assert
            settlementBuilt.ShouldBe(1);
            errorDetails.ShouldNotBeNull();
            errorDetails.Message.ShouldBe("Cannot build settlement. Missing 1 brick and 1 grain and 1 lumber and 1 wool.");
        }

        [Test]
        public void BuildSettlement_OnExistingSettlementBelongingToPlayer_MeaningfulErrorIsReceived()
        {
            // Arrange
            MockDice mockDice = null;
            MockPlayer player;
            MockComputerPlayer firstOpponent, secondOpponent, thirdOpponent;
            var localGameController = this.CreateLocalGameControllerAndCompleteGameSetup(out mockDice, out player, out firstOpponent, out secondOpponent, out thirdOpponent);

            mockDice.AddSequence(new[] { 8u });
            player.AddResources(ResourceClutch.RoadSegment);
            player.AddResources(ResourceClutch.Settlement * 2);

            ErrorDetails errorDetails = null;
            localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

            TurnToken turnToken = null;
            localGameController.StartPlayerTurnEvent = (TurnToken t) => { turnToken = t; };
            localGameController.StartGamePlay();
            localGameController.BuildRoadSegment(turnToken, MainRoadOneEnd, 3);
            localGameController.BuildSettlement(turnToken, 3);

            // Act
            localGameController.BuildSettlement(turnToken, 3);

            // Assert
            errorDetails.ShouldNotBeNull();
            errorDetails.Message.ShouldBe("Cannot build settlement. Location 3 already settled by you.");
        }

        [Test]
        public void BuildSettlement_OnExistingSettlementBelongingToOpponent_MeaningfulErrorIsReceived()
        {
            // Arrange
            MockDice mockDice = null;
            MockPlayer player;
            MockComputerPlayer firstOpponent, secondOpponent, thirdOpponent;
            var localGameController = this.CreateLocalGameControllerAndCompleteGameSetup(out mockDice, out player, out firstOpponent, out secondOpponent, out thirdOpponent);

            mockDice.AddSequence(new[] { 8u });
            player.AddResources(ResourceClutch.RoadSegment * 8);
            player.AddResources(ResourceClutch.Settlement);

            ErrorDetails errorDetails = null;
            localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

            TurnToken turnToken = null;
            localGameController.StartPlayerTurnEvent = (TurnToken t) => { turnToken = t; };
            localGameController.StartGamePlay();

            var roadSegmentDetails = new UInt32[] { 4, 3, 3, 2, 2, 1, 1, 0, 0, 8, 8, 9, 9, 19, 19, 18 };
            for (var index = 0; index < roadSegmentDetails.Length; index += 2)
            {
                localGameController.BuildRoadSegment(turnToken, roadSegmentDetails[index], roadSegmentDetails[index + 1]);
            }

            // Act
            localGameController.BuildSettlement(turnToken, 18);

            // Assert
            errorDetails.ShouldNotBeNull();
            errorDetails.Message.ShouldBe("Cannot build settlement. Location 18 already settled by player '" + FirstOpponentName + "'.");
        }

        [Test]
        public void BuildSettlement_ToCloseToAnotherSettlement_MeaningfulErrorIsReceived()
        {
            // Arrange
            MockDice mockDice = null;
            MockPlayer player;
            MockComputerPlayer firstOpponent, secondOpponent, thirdOpponent;
            var localGameController = this.CreateLocalGameControllerAndCompleteGameSetup(out mockDice, out player, out firstOpponent, out secondOpponent, out thirdOpponent);

            mockDice.AddSequence(new[] { 8u });
            player.AddResources(ResourceClutch.RoadSegment * 7);
            player.AddResources(ResourceClutch.Settlement);

            ErrorDetails errorDetails = null;
            localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

            TurnToken turnToken = null;
            localGameController.StartPlayerTurnEvent = (TurnToken t) => { turnToken = t; };
            localGameController.StartGamePlay();

            // Act
            localGameController.BuildSettlement(turnToken, 4);

            // Assert
            errorDetails.ShouldNotBeNull();
            errorDetails.Message.ShouldBe("Cannot build settlement. Too close to own settlement at location 12.");
        }

        [Test]
        public void BuildSettlement_ToCloseToOpponentSettlement_MeaningfulErrorIsReceived()
        {
            // Arrange
            MockDice mockDice = null;
            MockPlayer player;
            MockComputerPlayer firstOpponent, secondOpponent, thirdOpponent;
            var localGameController = this.CreateLocalGameControllerAndCompleteGameSetup(out mockDice, out player, out firstOpponent, out secondOpponent, out thirdOpponent);

            mockDice.AddSequence(new[] { 8u });
            player.AddResources(ResourceClutch.RoadSegment * 7);
            player.AddResources(ResourceClutch.Settlement);

            ErrorDetails errorDetails = null;
            localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

            TurnToken turnToken = null;
            localGameController.StartPlayerTurnEvent = (TurnToken t) => { turnToken = t; };
            localGameController.StartGamePlay();

            var roadSegmentDetails = new UInt32[] { 4, 3, 3, 2, 2, 1, 1, 0, 0, 8, 8, 9, 9, 19 };
            for (var index = 0; index < roadSegmentDetails.Length; index += 2)
            {
                localGameController.BuildRoadSegment(turnToken, roadSegmentDetails[index], roadSegmentDetails[index + 1]);
            }

            // Act
            localGameController.BuildSettlement(turnToken, 19);

            // Assert
            errorDetails.ShouldNotBeNull();
            errorDetails.Message.ShouldBe("Cannot build settlement. Too close to player '" + FirstOpponentName + "' at location 18.");
        }

        [Test]
        public void BuildSettlement_OffBoard_MeaningfulErrorIsReceived()
        {
            // Arrange
            MockDice mockDice = null;
            MockPlayer player;
            MockComputerPlayer firstOpponent, secondOpponent, thirdOpponent;
            var localGameController = this.CreateLocalGameControllerAndCompleteGameSetup(out mockDice, out player, out firstOpponent, out secondOpponent, out thirdOpponent);

            mockDice.AddSequence(new[] { 8u });
            player.AddResources(ResourceClutch.Settlement);

            ErrorDetails errorDetails = null;
            localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

            TurnToken turnToken = null;
            localGameController.StartPlayerTurnEvent = (TurnToken t) => { turnToken = t; };
            localGameController.StartGamePlay();

            // Act
            localGameController.BuildSettlement(turnToken, 54);

            // Assert
            errorDetails.ShouldNotBeNull();
            errorDetails.Message.ShouldBe("Cannot build settlement. Location 54 is outside of board range (0 - 53).");
        }

        [Test]
        public void BuildSettlement_NotConnectedToExistingRoad_MeaningfulErrorIsReceived()
        {
            // Arrange
            MockDice mockDice = null;
            MockPlayer player;
            MockComputerPlayer firstOpponent, secondOpponent, thirdOpponent;
            var localGameController = this.CreateLocalGameControllerAndCompleteGameSetup(out mockDice, out player, out firstOpponent, out secondOpponent, out thirdOpponent);

            mockDice.AddSequence(new[] { 8u });
            player.AddResources(ResourceClutch.RoadSegment);
            player.AddResources(ResourceClutch.Settlement);

            ErrorDetails errorDetails = null;
            localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

            TurnToken turnToken = null;
            localGameController.StartPlayerTurnEvent = (TurnToken t) => { turnToken = t; };
            localGameController.StartGamePlay();
            localGameController.BuildRoadSegment(turnToken, 4, 3);

            // Act
            localGameController.BuildSettlement(turnToken, 2);

            // Assert
            errorDetails.ShouldNotBeNull();
            errorDetails.Message.ShouldBe("Cannot build settlement. Location 2 not connected to existing road.");
        }

        [Test]
        public void BuildSettlement_TurnTokenNotCorrect_MeaningfulErrorIsReceived()
        {
            // Arrange
            var testInstances = LocalGameControllerTestCreator.CreateTestInstances();
            var localGameController = testInstances.LocalGameController;
            LocalGameControllerTestSetup.LaunchGameAndCompleteSetup(localGameController);

            testInstances.Dice.AddSequence(new[] { 8u });

            ErrorDetails errorDetails = null;
            localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

            // Act
            localGameController.BuildSettlement(new TurnToken(), 3);

            // Assert
            errorDetails.ShouldNotBeNull();
            errorDetails.Message.ShouldBe("Turn token not recognised.");
        }

        [Test]
        public void BuildSettlement_TurnTokenIsNull_MeaningfulErrorIsReceived()
        {
            // Arrange
            var testInstances = LocalGameControllerTestCreator.CreateTestInstances();
            var localGameController = testInstances.LocalGameController;
            LocalGameControllerTestSetup.LaunchGameAndCompleteSetup(localGameController);

            testInstances.Dice.AddSequence(new[] { 8u });

            ErrorDetails errorDetails = null;
            localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };
            localGameController.StartGamePlay();

            // Act
            localGameController.BuildSettlement(null, 4);

            // Assert
            errorDetails.ShouldNotBeNull();
            errorDetails.Message.ShouldBe("Turn token is null.");
        }

        [Test]
        public void BuildSettlement_GotNineVictoryPoints_EndOfGameEventRaisedWithPlayerAsWinner()
        {
            // Arrange
            var testInstances = LocalGameControllerTestCreator.CreateTestInstances(new MockGameBoardWithNoResourcesCollected());
            var localGameController = testInstances.LocalGameController;
            LocalGameControllerTestSetup.LaunchGameAndCompleteSetup(localGameController);

            testInstances.Dice.AddSequence(new[] { 8u });

            var player = testInstances.MainPlayer;
            player.AddResources(ResourceClutch.RoadSegment * 5);
            player.AddResources(ResourceClutch.Settlement * 4);
            player.AddResources(ResourceClutch.City * 4);

            TurnToken turnToken = null;
            localGameController.StartPlayerTurnEvent = (TurnToken t) => { turnToken = t; };

            Guid winningPlayer = Guid.Empty;
            localGameController.GameOverEvent = (Guid g) => { winningPlayer = g; };

            localGameController.StartGamePlay();
            localGameController.BuildRoadSegment(turnToken, 4u, 3u);
            localGameController.BuildRoadSegment(turnToken, 3u, 2u);
            localGameController.BuildRoadSegment(turnToken, 2u, 1u);
            localGameController.BuildRoadSegment(turnToken, 1u, 0u); // Got 2VP for longest road (4VP)
            localGameController.BuildRoadSegment(turnToken, 2u, 10u);

            localGameController.BuildSettlement(turnToken, 3);
            localGameController.BuildSettlement(turnToken, 10);

            localGameController.BuildCity(turnToken, 3);
            localGameController.BuildCity(turnToken, 10);
            localGameController.BuildCity(turnToken, 40);


            // Act
            localGameController.BuildSettlement(turnToken, 1);

            // Assert
            winningPlayer.ShouldBe(player.Id);
            player.VictoryPoints.ShouldBe(10u);
        }

        [Test]
        public void BuildSettlement_GameIsOver_MeaningfulErrorIsReceived()
        {
            // Arrange
            var testInstances = LocalGameControllerTestCreator.CreateTestInstances(new MockGameBoardWithNoResourcesCollected());
            var localGameController = testInstances.LocalGameController;
            LocalGameControllerTestSetup.LaunchGameAndCompleteSetup(localGameController);

            testInstances.Dice.AddSequence(new[] { 8u });

            var player = testInstances.MainPlayer;
            player.AddResources(ResourceClutch.RoadSegment * 5);
            player.AddResources(ResourceClutch.Settlement * 3);
            player.AddResources(ResourceClutch.City * 4);

            TurnToken turnToken = null;
            localGameController.StartPlayerTurnEvent = (TurnToken t) => { turnToken = t; };

            ErrorDetails errorDetails = null;
            localGameController.ErrorRaisedEvent = (ErrorDetails e) => { errorDetails = e; };

            localGameController.StartGamePlay();
            localGameController.BuildRoadSegment(turnToken, 4u, 3u);
            localGameController.BuildRoadSegment(turnToken, 3u, 2u);
            localGameController.BuildRoadSegment(turnToken, 2u, 1u);
            localGameController.BuildRoadSegment(turnToken, 1u, 0u); // Got 2VP for longest road (4VP)
            localGameController.BuildRoadSegment(turnToken, 2u, 10u);

            localGameController.BuildSettlement(turnToken, 3);
            localGameController.BuildSettlement(turnToken, 10);

            localGameController.BuildCity(turnToken, 3);
            localGameController.BuildCity(turnToken, 10);
            localGameController.BuildCity(turnToken, 12);
            localGameController.BuildCity(turnToken, 40); // Got 10VP, Game over event raised

            // Act
            localGameController.BuildSettlement(turnToken, 1);

            // Assert
            errorDetails.ShouldNotBeNull();
            errorDetails.Message.ShouldBe("Cannot build settlement. Game is over.");
        }

        [Test]
        public void Scenario_OpponentBuildsSettlement()
        {
            // Arrange
            var testInstances = LocalGameControllerTestCreator.CreateTestInstances(new MockGameBoardWithNoResourcesCollected());
            var localGameController = testInstances.LocalGameController;
            LocalGameControllerTestSetup.LaunchGameAndCompleteSetup(localGameController);

            testInstances.Dice.AddSequence(new uint[] { 8, 8, 8, 8, 8 });

            var firstOpponent = testInstances.FirstOpponent;
            firstOpponent.AddResources(ResourceClutch.RoadSegment);
            firstOpponent.AddResources(ResourceClutch.Settlement);

            firstOpponent.AddBuildRoadSegmentInstruction(new BuildRoadSegmentInstruction { Locations = new[] { 17u, 7u } })
              .AddBuildSettlementInstruction(new BuildSettlementInstruction { Location = 7u });

            TurnToken turnToken = null;
            localGameController.StartPlayerTurnEvent = (TurnToken t) => { turnToken = t; };

            var gameEvents = new List<List<GameEvent>>();
            localGameController.OpponentActionsEvent = (Guid g, List<GameEvent> e) => { gameEvents.Add(e); };

            localGameController.StartGamePlay();

            // Act
            localGameController.EndTurn(turnToken);

            // Assert
            gameEvents.Count.ShouldBe(3);
            gameEvents[0].ShouldContainExact(new GameEvent[] {
                new DiceRollEvent(firstOpponent.Id, 4, 4),
                new RoadSegmentBuiltEvent(firstOpponent.Id, 17u, 7u),
                new SettlementBuiltEvent(firstOpponent.Id, 7u)});
            firstOpponent.ResourcesCount.ShouldBe(0);
            firstOpponent.VictoryPoints.ShouldBe(3u);
        }

        [Test]
        public void Scenario_OpponentBuildsSettlementToWin()
        {
            // Arrange
            var testInstances = LocalGameControllerTestCreator.CreateTestInstances(new MockGameBoardWithNoResourcesCollected());
            var localGameController = testInstances.LocalGameController;
            LocalGameControllerTestSetup.LaunchGameAndCompleteSetup(localGameController);

            testInstances.Dice.AddSequence(new uint[] { 8, 8, 8, 8, 8 });

            var player = testInstances.MainPlayer;
            player.AddResources(ResourceClutch.RoadSegment * 5);
            player.AddResources(ResourceClutch.Settlement * 3);
            player.AddResources(ResourceClutch.City * 3);

            var firstOpponent = testInstances.FirstOpponent;
            firstOpponent
              .AddBuildRoadSegmentInstruction(new BuildRoadSegmentInstruction { Locations = new UInt32[] { 17, 7, 7, 8, 8, 0, 0, 1, 8, 9 } })
              .AddBuildSettlementInstruction(new BuildSettlementInstruction { Location = 7 })
              .AddBuildSettlementInstruction(new BuildSettlementInstruction { Location = 0 })
              .AddBuildCityInstruction(new BuildCityInstruction { Location = 7 })
              .AddBuildCityInstruction(new BuildCityInstruction { Location = 0 })
              .AddBuildCityInstruction(new BuildCityInstruction { Location = 18 })
              .AddBuildSettlementInstruction(new BuildSettlementInstruction { Location = 9 });

            TurnToken turnToken = null;
            localGameController.StartPlayerTurnEvent = (TurnToken t) => { turnToken = t; };

            var events = new List<List<GameEvent>>();
            localGameController.OpponentActionsEvent = (Guid g, List<GameEvent> e) => { events.Add(e); };

            localGameController.StartGamePlay();

            // Act
            localGameController.EndTurn(turnToken);

            // Assert
            var expectedWinningGameEvent = new GameWinEvent(firstOpponent.Id);

            events.Count.ShouldBe(3);
            events[0][13].ShouldBe(expectedWinningGameEvent);
            firstOpponent.VictoryPoints.ShouldBe(10u);
            localGameController.GamePhase.ShouldBe(LocalGameController.GamePhases.GameOver);
        }
        #endregion
    }
}
