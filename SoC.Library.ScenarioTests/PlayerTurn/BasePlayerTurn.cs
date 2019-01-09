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

        private List<GameEvent> actualEvents = new List<GameEvent>();

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

        public LocalGameControllerScenarioRunner EndTurn()
        {
            return this.runner;
        }

        public ExpectedEventsBuilder Events()
        {
            this.expectedEventsBuilder = new ExpectedEventsBuilder(this, this.runner.playersByName);
            return this.expectedEventsBuilder;
        }

        public void AddEvent(GameEvent gameEvent)
        {
            this.actualEvents.Add(gameEvent);
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

        public PlayerStateBuilder State()
        {
            this.expectedPlayerState = new PlayerStateBuilder(this);
            return this.expectedPlayerState;
        }

        internal List<GameEvent> GetExpectedEvents()
        {
            if (this.expectedEventsBuilder != null)
                return this.expectedEventsBuilder.expectedEvents;

            return null;
        }

        internal PlayerSnapshot GetPlayerSnapshot()
        {
            if (this.expectedPlayerState != null)
                return this.expectedPlayerState.playerSnapshot;

            return null;
        }

        internal List<RunnerAction> GetRunnerActions()
        {
            if (this.actionBuilder != null)
                return this.actionBuilder.runnerActions;

            return null;
        }

        internal List<Type> GetUnwantedEventTypes()
        {
            if (this.expectedEventsBuilder != null)
                return this.expectedEventsBuilder.unwantedEventTypes;

            return null;
        }

        internal void AddEvents(List<GameEvent> gameEvents)
        {
            this.actualEvents.AddRange(gameEvents);
        }

        internal void VerifyEvents()
        {
            if (this.expectedEventsBuilder == null)
                return;

            var unwantedEventTypes = this.expectedEventsBuilder.unwantedEventTypes;
            if (unwantedEventTypes != null && unwantedEventTypes.Count > 0)
            {
                var unwantedEvents = this.actualEvents.Where(e => unwantedEventTypes.Contains(e.GetType())).ToList();
                if (unwantedEvents.Count > 0)
                    Assert.Fail($"{unwantedEvents.Count} events of unwanted type {unwantedEvents[0].GetType()} found.\r\nFirst event:\r\n{GetEventDetails(unwantedEvents[0])}");
            }

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
                    Assert.Fail($"Did not find {expectedEvent.GetType()} event for '{this.player.Name}'.\r\n{this.GetEventDetails(expectedEvent)}");
            }
        }

        protected void AddDevelopmentCard(Guid playerId, DevelopmentCardTypes developmentCardType)
        {
            this.runner.AddDevelopmentCardToBuy(developmentCardType);
        }

        private string GetEventDetails(GameEvent gameEvent)
        {
            var message = "";

            if (gameEvent is DiceRollEvent diceRollEvent)
            {
                message += $"Dice 1 is {diceRollEvent.Dice1}, Dice roll 2 is {diceRollEvent.Dice2}";
            }
            else if (gameEvent is ResourcesCollectedEvent resourcesCollectedEvent)
            {
                message += $"Resources collected entries count is {resourcesCollectedEvent.ResourceCollection.Length}";
                foreach (var entry in resourcesCollectedEvent.ResourceCollection)
                    message += $"\r\nLocation {entry.Location}, Resources {entry.Resources}";
            }
            else if (gameEvent is ResourceTransactionEvent resourceTransactionEvent)
            {
                message += $"Resource transaction count is {resourceTransactionEvent.ResourceTransactions.Count}";
                for (var index = 0; index < resourceTransactionEvent.ResourceTransactions.Count; index++)
                {
                    var resourceTransaction = resourceTransactionEvent.ResourceTransactions[index];
                    var receivingPlayer = this.runner.GetPlayer(resourceTransaction.ReceivingPlayerId);
                    var givingPlayer = this.runner.GetPlayer(resourceTransaction.GivingPlayerId);
                    message += $"\r\nReceiving player '{receivingPlayer.Name}', Giving player '{givingPlayer.Name}', Resources {resourceTransaction.Resources}";
                }
            }
            else if (gameEvent is RoadSegmentBuiltEvent roadSegmentBuildEvent)
            {
                message += $"From {roadSegmentBuildEvent.StartLocation} to {roadSegmentBuildEvent.EndLocation}";
            }

            return message;
        }
    }
}
