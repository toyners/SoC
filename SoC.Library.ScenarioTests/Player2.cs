
namespace SoC.Library.ScenarioTests
{
    using System;
    using System.Collections.Generic;
    using Jabberwocky.SoC.Library;
    using Jabberwocky.SoC.Library.GameActions;
    using Jabberwocky.SoC.Library.GameEvents;
    using NUnit.Framework;

    internal class Player2
    {
        private TurnInstructions currentTurn;
        private int nextTurnIndex;
        protected GameController gameController;
        private TurnToken currentTurnToken;

        public Player2(string playerName)
        {
            this.PlayerName = playerName;
            this.gameController = new GameController();
            this.gameController.GameExceptionEvent += this.GameExceptionEventHandler;
            this.gameController.GameEvent += this.GameEventHandler;
        }

        public Exception GameException { get; private set; }
        public string PlayerName { get; private set; }
        public bool CurrentTurnIsFinished
        {
            get
            {
                return this.currentTurn != null &&
                    this.currentTurn.IsFinished;
            }
        }
        public bool IsFinished
        {
            get
            {
                return this.nextTurnIndex >= this.turns.Count;
            }
        }

        public void JoinGame(LocalGameServer gameServer)
        {
            gameServer.JoinGame(this.PlayerName, this.gameController);
        }

        protected void GameEventHandler(GameEvent gameEvent)
        {
            if (gameEvent is InitialBoardSetupEventArgs)
            {

            }
            else
            {
                if (gameEvent is PlaceSetupInfrastructureEventArgs placeSetupInfrastructureEventArgs)
                    this.currentTurnToken = placeSetupInfrastructureEventArgs.Item;

                this.currentTurn.AddActualEvent(gameEvent);
            }
        }

        private void GameExceptionEventHandler(Exception exception)
        {
            this.GameException = exception;
        }

        public void InsertTurnInstructions(IEnumerable<object> instructions)
        {
            var turn = new TurnInstructions();
            this.turns.Add(turn);
            if (instructions != null)
            {
                turn.Instructions = new List<object>(instructions);
                /*foreach (var instruction in instructions)
                {
                    if (instruction is GameEvent gameEvent)
                        turn.ExpectedEvents.Add(gameEvent);
                    else if (instruction is ScenarioActionWrapper scenarioAction && 
                        scenarioAction.PlayerName == this.PlayerName)
                    {
                        turn.Actions.Add(scenarioAction.Action);
                    }
                }*/
            }

            if (this.currentTurn == null)
                this.currentTurn = this.turns[this.nextTurnIndex++];
        }

        public void Process()
        {
            this.currentTurn.Process((action) => this.gameController.SendAction(this.currentTurnToken, action));
        }

        private List<TurnInstructions> turns = new List<TurnInstructions>();
        private class TurnInstructions
        {
            private int nextActionIndex;
            private int expectedEventIndex;
            private int actualEventIndex;
            private int instructionIndex;
            public List<object> Instructions = new List<object>();
            public List<GameEvent> ActualEvents = new List<GameEvent>();
            public List<GameEvent> ExpectedEvents = new List<GameEvent>();
            public List<ComputerPlayerAction> Actions = new List<ComputerPlayerAction>();

            public bool IsFinished
            {
                get
                {
                    return this.instructionIndex >= this.Instructions.Count;
                }
            }

            public void AddActualEvent(GameEvent gameEvent)
            {
                this.ActualEvents.Add(gameEvent);
            }

            public void Process(Action<ComputerPlayerAction> sendActionFunction)
            {
                for (; this.instructionIndex < this.Instructions.Count; this.instructionIndex++)
                {
                    var instruction = this.Instructions[this.instructionIndex];
                    if (instruction is ComputerPlayerAction action)
                    {
                        if (this.VerifyEvents(false))
                        {
                            sendActionFunction(action);
                            break;
                        }
                    }
                    else if (instruction is GameEvent gameEvent)
                    {
                        this.ExpectedEvents.Add(gameEvent);
                    }
                }

                this.VerifyEvents(true);
            }

            private bool VerifyEvents(bool throwIfNotVerified)
            {
                if (this.expectedEventIndex < this.ExpectedEvents.Count)
                {
                    while (this.actualEventIndex < this.ActualEvents.Count)
                    {
                        if (this.ExpectedEvents[this.expectedEventIndex].Equals(this.ActualEvents[this.actualEventIndex]))
                        {
                            this.expectedEventIndex++;
                        }

                        this.actualEventIndex++;
                    }
                }

                if (throwIfNotVerified && this.expectedEventIndex < this.ExpectedEvents.Count)
                {
                    // At least one expected event was not matched with an actual event.
                    var expectedEvent = this.ExpectedEvents[this.expectedEventIndex];
                    Assert.Fail($"Did not find {expectedEvent.GetType()}");
                    //Assert.Fail($"Did not find {expectedEvent.GetType()} event for '{this.PlayerName}' in round {this.roundNumber}, turn {this.turnNumber}.\r\n{this.GetEventDetails(expectedEvent)}");

                    throw new NotImplementedException(); // Not reached - Have to do this to pass compliation
                }
                else
                {
                    return this.expectedEventIndex == this.ExpectedEvents.Count;
                }
            }
        }
    }
}
