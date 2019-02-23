
namespace SoC.Library.ScenarioTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Jabberwocky.SoC.Library;
    using Jabberwocky.SoC.Library.GameActions;
    using Jabberwocky.SoC.Library.GameEvents;
    using NUnit.Framework;
    using SoC.Library.ScenarioTests.PlayerTurn;

    internal class Player2
    {
        private TurnInstructions currentTurn;
        private List<TurnInstructions> turns = new List<TurnInstructions>();

        private int nextTurnIndex;
        protected GameController gameController;

        public Player2(string playerName)
        {
            this.PlayerName = playerName;
            this.gameController = new GameController();
            this.gameController.GameExceptionEvent += this.GameExceptionEventHandler;
            this.gameController.GameEvent += this.GameEventHandler;
        }

        public Exception GameException { get; private set; }
        public string PlayerName { get; private set; }
        private bool CurrentTurnIsFinished
        {
            get
            {
                return this.currentTurn != null &&
                    this.currentInstructionIndex >= this.currentTurn.Instructions.Count &&
                    this.expectedEventIndex == this.ExpectedEvents.Count;
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
            if (gameEvent is PlaceSetupInfrastructureEventArgs)
            {
                this.currentTurn = this.turns[this.nextTurnIndex++];
            }

            this.AddActualEvent(gameEvent);
        }

        private void GameExceptionEventHandler(Exception exception)
        {
            this.GameException = exception;
        }

        public void AddTurnInstructions(BasePlayerTurn bpt)
        {
            var turn = new TurnInstructions
            {
                RoundNumber = bpt.RoundNumber,
                TurnNumber = bpt.TurnNumber
            };

            this.turns.Add(turn);
            if (bpt != null && bpt.HasInstructions)
            {
                turn.Instructions = new List<object>(bpt.Instructions.Where(i => ((Instruction2)i).PlayerName == this.PlayerName));
            }
        }

        //private List<GameEvent> ExpectedEvents = new List<GameEvent>();
        //private Queue<object> Instructions { get { return this.currentTurn.Instructions; } }
        public List<GameEvent> ActualEvents { get { return this.currentTurn.ActualEvents; } }
        public List<GameEvent> ExpectedEvents { get { return this.currentTurn.ExpectedEvents; } }
        private int currentInstructionIndex;
        public void Process()
        {
            if (this.IsFinished)
            {
                this.currentTurn = null;
                return;
            }

            if (this.CurrentTurnIsFinished)
            {
                this.currentTurn = this.turns[this.nextTurnIndex++];
                this.currentInstructionIndex = 0;
            }

            while (this.currentInstructionIndex < this.currentTurn.Instructions.Count)
            {
                if (this.GameException != null)
                    throw this.GameException;

                var instruction = (Instruction2)this.currentTurn.Instructions[this.currentInstructionIndex];
                var payload = instruction.Payload;
                if (payload is ActionInstruction action)
                {
                    if (this.VerifyEvents(false))
                    {
                        this.currentInstructionIndex++;
                        this.SendAction(action);
                        return;
                    }
                }
                else if (payload is GameEvent gameEvent)
                {
                    this.currentInstructionIndex++;
                    this.ExpectedEvents.Add(gameEvent);
                }
            }

            this.VerifyEvents(true);
        }

        internal void StartAsync()
        {
            Task.Factory.StartNew(() => {
                while (true)
                {
                    Thread.Sleep(50);
                    if (this.currentTurn != null)
                        this.Process();
                }
            });
        }

        private void SendAction(ActionInstruction action)
        {
            switch (action.Type)
            {
                case ActionInstruction.Types.EndOfTurn:
                {
                    this.gameController.EndTurn();
                    break;
                }
                case ActionInstruction.Types.PlaceStartingInfrastructure:
                {
                    this.gameController.PlaceStartingInfrastructure((uint)action.Parameters[0], (uint)action.Parameters[1]);
                    break;
                }
                default: throw new Exception();
            }
        }

        private int RoundNumber { get { return this.currentTurn.RoundNumber; } }
        private int TurnNumber { get { return this.currentTurn.TurnNumber; } }

        private int expectedEventIndex;
        private int actualEventIndex;
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
                //Assert.Fail($"Did not find {expectedEvent.GetType()}");
                //Assert.Fail($"Did not find {expectedEvent.GetType()} event for '{this.PlayerName}' in round {this.RoundNumber}, turn {this.TurnNumber}.\r\n{/*this.GetEventDetails(expectedEvent)*/""}");
                Assert.Fail($"Did not find {expectedEvent.GetType()} event for '{this.PlayerName}' in round {this.RoundNumber}, turn {this.TurnNumber}.\r\n");

                throw new NotImplementedException(); // Not reached - Have to do this to pass compliation
            }
            else
            {
                return this.expectedEventIndex == this.ExpectedEvents.Count;
            }
        }

        public void AddActualEvent(GameEvent gameEvent)
        {
            this.ActualEvents.Add(gameEvent);
        }

        private class TurnInstructions
        {
            private int nextActionIndex;
            public int RoundNumber, TurnNumber;
            
            public List<object> Instructions = new List<object>();
            public List<GameEvent> ActualEvents = new List<GameEvent>();
            public List<GameEvent> ExpectedEvents = new List<GameEvent>();
            public List<ComputerPlayerAction> Actions = new List<ComputerPlayerAction>();
        }
    }
}
