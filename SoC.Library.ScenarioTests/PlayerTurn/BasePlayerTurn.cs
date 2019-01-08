using System;
using System.Collections.Generic;
using System.Linq;
using Jabberwocky.SoC.Library;
using Jabberwocky.SoC.Library.DevelopmentCards;
using Jabberwocky.SoC.Library.Enums;
using Jabberwocky.SoC.Library.GameActions;
using Jabberwocky.SoC.Library.GameEvents;
using Jabberwocky.SoC.Library.Interfaces;
using Jabberwocky.SoC.Library.ScenarioTests.Builders;
using NUnit.Framework;
using SoC.Library.ScenarioTests.Builders;

namespace SoC.Library.ScenarioTests.PlayerTurn
{
    internal abstract class BasePlayerTurn
    {
        protected PlayerActionBuilder actionBuilder;
        private ExpectedEventsBuilder expectedEventsBuilder;
        private PlayerStateBuilder expectedPlayerState;

        public readonly IPlayer player;
        protected readonly LocalGameControllerScenarioRunner runner;

        public BasePlayerTurn(IPlayer player, uint dice1, uint dice2, LocalGameControllerScenarioRunner runner)
        {
            this.runner = runner;
            this.player = player;
            this.Dice1 = dice1;
            this.Dice2 = dice2;
        }

        #region Properties
        public Guid PlayerId { get { return this.player.Id; } }
        public uint Dice1 { get; }
        public uint Dice2 { get; }
        #endregion

        public PlayerActionBuilder Actions()
        {
            this.actionBuilder = new PlayerActionBuilder(this);
            return this.actionBuilder;
        }

        public void CompareSnapshot()
        {
            if (this.expectedPlayerState == null)
                return;

            if (player.HeldCards.Count != this.expectedPlayerState.playerSnapshot.heldCards.Count)
                Assert.Fail("Held cards count is not same");

            if (player.HeldCards[0].Type != this.expectedPlayerState.playerSnapshot.heldCards[0])
                Assert.Fail("Held card does not match");
        }

        public ExpectedEventsBuilder Events()
        {
            this.expectedEventsBuilder = new ExpectedEventsBuilder(this, this.runner.playersByName);
            return this.expectedEventsBuilder;
        }

        public PlayerStateBuilder State()
        {
            this.expectedPlayerState = new PlayerStateBuilder(this);
            return this.expectedPlayerState;
        }

        public LocalGameControllerScenarioRunner EndTurn()
        {
            return this.runner;
        }


        public virtual BasePlayerTurn BuildCity(uint roadSegmentStart) { return this; }

        public virtual BasePlayerTurn BuildRoad(uint roadSegmentStart, uint roadSegmentEnd)
        {
            //this.actions.Enqueue(new BuildRoadSegmentAction(roadSegmentStart, roadSegmentEnd));
            return this;
        }

        public virtual BasePlayerTurn BuildSettlement(uint settlementLocation)
        {
            //this.actions.Enqueue(new BuildSettlementAction(settlementLocation));
            return this;
        }

        public virtual BasePlayerTurn BuyDevelopmentCard(DevelopmentCardTypes developmentCardType)
        {
            this.AddDevelopmentCard(this.PlayerId, developmentCardType);
            //this.actions.Enqueue(new ComputerPlayerAction(ComputerPlayerActionTypes.BuyDevelopmentCard));
            return this;
        }

        public virtual BasePlayerTurn PlayKnightCard(uint hexLocation)
        {
            //this.actions.Enqueue(new PlayKnightCardAction(hexLocation));
            return this;
        }

        public void AddEvent(GameEvent gameEvent)
        {
            this.actualEvents.Add(gameEvent);
        }

        public virtual BasePlayerTurn PlayKnightCardAndCollectFrom(uint hexLocation, string selectedPlayerName, ResourceTypes expectedSingleResource)
        {
            //this.actions.Enqueue(new ScenarioPlayKnightCardAction(hexLocation, selectedPlayerName, expectedSingleResource));
            return this;
        }

        public virtual void ResolveActions(TurnToken turnToken, LocalGameController localGameController)
        {
            if (this.actionBuilder == null)
                return;

            foreach (var action in this.actionBuilder.playerActions)
            {
                if (action is ScenarioPlayKnightCardAction scenarioPlayKnightCardAction)
                {
                    var selectedPlayer = this.runner.GetPlayerFromName(scenarioPlayKnightCardAction.SelectedPlayerName);

                    var randomNumber = int.MinValue;
                    switch (scenarioPlayKnightCardAction.ExpectedSingleResource)
                    {
                        case ResourceTypes.Ore:
                            randomNumber = selectedPlayer.Resources.BrickCount +
                            selectedPlayer.Resources.GrainCount +
                            selectedPlayer.Resources.LumberCount;
                            break;
                        default: throw new Exception($"Resource type '{scenarioPlayKnightCardAction.ExpectedSingleResource}' not handled");
                    }

                    this.runner.NumberGenerator.AddRandomNumber(randomNumber);

                    var knightCard = (KnightDevelopmentCard)this.player.HeldCards.Where(c => c.Type == DevelopmentCardTypes.Knight).First();
                    localGameController.UseKnightCard(turnToken, knightCard, scenarioPlayKnightCardAction.NewRobberHex,
                        selectedPlayer.Id);
                }
                else if (action is PlayKnightCardAction playKnightCardAction)
                {
                    var knightCard = (KnightDevelopmentCard)this.player.HeldCards.Where(c => c.Type == DevelopmentCardTypes.Knight).First();
                    localGameController.UseKnightCard(turnToken, knightCard, playKnightCardAction.NewRobberHex, playKnightCardAction.PlayerId);
                }
                else if (action is ComputerPlayerAction)
                {
                    switch (action.ActionType)
                    {
                        case ComputerPlayerActionTypes.BuyDevelopmentCard: localGameController.BuyDevelopmentCard(turnToken); break;
                        default: throw new Exception($"Action type '{action.ActionType}' not handled");
                    }
                }
                else
                {
                    throw new Exception($"Action of type '{action.GetType()}' not handled");
                }
            }
        }

        protected void AddDevelopmentCard(Guid playerId, DevelopmentCardTypes developmentCardType)
        {
            this.runner.AddDevelopmentCardToBuy(developmentCardType);
        }

        internal List<GameEvent> GetExpectedEvents()
        {
            if (this.expectedEventsBuilder != null)
                return this.expectedEventsBuilder.expectedEvents;

            return null;
        }

        internal List<RunnerAction> GetRunnerActions()
        {
            if (this.actionBuilder != null)
                return this.actionBuilder.runnerActions;

            return null;
        }

        internal PlayerSnapshot GetPlayerSnapshot()
        {
            if (this.expectedPlayerState != null)
                return this.expectedPlayerState.playerSnapshot;

            return null;
        }

        internal List<Type> GetUnwantedEventTypes()
        {
            if (this.expectedEventsBuilder != null)
                return this.expectedEventsBuilder.unwantedEventTypes;

            return null;
        }

        private List<GameEvent> actualEvents = new List<GameEvent>();
        internal void AddEvents(List<GameEvent> gameEvents)
        {
            this.actualEvents.AddRange(gameEvents);
        }

        internal void VerifyEvents()
        {
            if (this.expectedEventsBuilder == null)
                return;

            var expectedEvents = this.expectedEventsBuilder.expectedEvents;
            if (expectedEvents == null)
                return;

            var actualEventIndex = 0;
            foreach (var expectedEvent in expectedEvents)
            {
                var foundEvent = false;
                while (actualEventIndex < this.actualEvents.Count)
                {
                    var actualEvent = this.actualEvents[actualEventIndex++];
                    if (expectedEvent.Equals(actualEvent))
                    {
                        foundEvent = true;
                        break;
                    }
                }

                if (!foundEvent)
                    Assert.Fail(this.ToMessage(expectedEvent));
            }
        }

        private string ToMessage(GameEvent gameEvent)
        {
            //var player = this.players.Where(p => p.Id.Equals(gameEvent.PlayerId)).FirstOrDefault();

            var message = $"Did not find {gameEvent.GetType()} event for '{this.player.Name}'.";

            if (gameEvent is DiceRollEvent diceRollEvent)
            {
                message += $"\r\nDice 1 is {diceRollEvent.Dice1}, Dice roll 2 is {diceRollEvent.Dice2}";
            }
            else if (gameEvent is ResourcesCollectedEvent resourcesCollectedEvent)
            {
                message += $"\r\nResources collected entries count is {resourcesCollectedEvent.ResourceCollection.Length}";
                foreach (var entry in resourcesCollectedEvent.ResourceCollection)
                    message += $"\r\nLocation {entry.Location}, Resources {entry.Resources}";
            }
            else if (gameEvent is ResourceTransactionEvent resourceTransactionEvent)
            {
                message += $"\r\nResource transaction count is {resourceTransactionEvent.ResourceTransactions.Count}";
                for (var index = 0; index < resourceTransactionEvent.ResourceTransactions.Count; index++)
                {
                    var resourceTransaction = resourceTransactionEvent.ResourceTransactions[index];
                    var receivingPlayer = this.runner.GetPlayer(resourceTransaction.ReceivingPlayerId);
                    var givingPlayer = this.runner.GetPlayer(resourceTransaction.GivingPlayerId);
                    message += $"\r\nTransaction is: Receiving player '{receivingPlayer.Name}', Giving player '{givingPlayer.Name}', Resources {resourceTransaction.Resources}";
                }
            }
            else if (gameEvent is RoadSegmentBuiltEvent roadSegmentBuildEvent)
            {
                message += $"\r\nFrom {roadSegmentBuildEvent.StartLocation} to {roadSegmentBuildEvent.EndLocation}";
            }

            return message;
        }
    }
}
