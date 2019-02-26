
namespace SoC.Library.ScenarioTests
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Jabberwocky.SoC.Library;
    using Jabberwocky.SoC.Library.GameActions;
    using Jabberwocky.SoC.Library.GameEvents;
    using NUnit.Framework;
    using SoC.Library.ScenarioTests.PlayerTurn;

    internal class PlayerAgent
    {
        private TurnInstructions currentTurn;
        private readonly List<TurnInstructions> turns = new List<TurnInstructions>();

        private int nextTurnIndex;
        protected GameController gameController;

        public PlayerAgent(string name)
        {
            this.Name = name;
            this.gameController = new GameController();
            this.gameController.GameExceptionEvent += this.GameExceptionEventHandler;
            this.gameController.GameEvent += this.GameEventHandler;
        }

        public Exception GameException { get; private set; }
        public string Name { get; private set; }
        private bool CurrentTurnIsFinished
        {
            get
            {
                return this.currentTurn != null &&
                    this.currentInstructionIndex >= this.currentTurn.Instructions.Count &&
                    this.ExpectedEventIndex == this.ExpectedEvents.Count;
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
            gameServer.JoinGame(this.Name, this.gameController);
        }

        private ConcurrentQueue<GameEvent> actualEventQueue = new ConcurrentQueue<GameEvent>();
        protected void GameEventHandler(GameEvent gameEvent)
        {
            this.actualEventQueue.Enqueue(gameEvent);
        }

        private void GameExceptionEventHandler(Exception exception)
        {
            this.GameException = exception;
        }

        public void AddTurnInstructions(BasePlayerTurn bpt)
        {
            if (bpt == null || !bpt.HasInstructions)
                return;

            var instructions = bpt.Instructions.Where(i => ((Instruction)i).PlayerName == this.Name).ToList();
            if (instructions.Count == 0)
                return;

            var turn = new TurnInstructions
            {
                RoundNumber = bpt.RoundNumber,
                TurnNumber = bpt.TurnNumber
            };
            turn.Instructions = new List<object>(instructions);
            this.turns.Add(turn);
        }

        public List<GameEvent> ActualEvents { get { return this.currentTurn.ActualEvents; } }
        public List<GameEvent> ExpectedEvents { get { return this.currentTurn.ExpectedEvents; } }
        private int currentInstructionIndex;
        public void ProcessInstructions()
        {
            while (this.currentInstructionIndex < this.currentTurn.Instructions.Count)
            {
                if (this.GameException != null)
                    throw this.GameException;

                var instruction = (Instruction)this.currentTurn.Instructions[this.currentInstructionIndex];
                var payload = instruction.Payload;
                if (payload is ActionInstruction action)
                {
                    if (!this.VerifyEvents(false))
                        return;

                    this.currentInstructionIndex++;
                    this.SendAction(action);
                }
                else if (payload is GameEvent expectedEvent)
                {
                    this.currentInstructionIndex++;
                    this.ExpectedEvents.Add(expectedEvent);
                }
            }
        }

        internal void StartAsync()
        {
            Task.Factory.StartNew(() => this.Run());
        }

        private void Run()
        {
            Thread.CurrentThread.Name = this.Name;
            while (!this.IsFinished)
            {
                Thread.Sleep(50);
                if (this.actualEventQueue.TryDequeue(out var actualEvent))
                    this.ProcessActualEvent(actualEvent);

                if (this.currentTurn != null)
                    this.ProcessInstructions();
            }
        }

        private void ProcessActualEvent(GameEvent actualEvent)
        {
            var changeTurn = false;
            if (actualEvent is InitialBoardSetupEventArgs)
            {
                changeTurn = true;
            }
            else if (actualEvent is PlaceSetupInfrastructureEventArgs)
            {
                changeTurn = true;
            }

            if (changeTurn)
            {
                if (this.currentTurn != null)
                    this.VerifyEvents(true);

                this.currentTurn = this.turns[this.nextTurnIndex++];
                this.currentInstructionIndex = 0;
            }

            this.ActualEvents.Add(actualEvent);
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

        // TODO: Clean up this - either better use of no use of properties to public vars
        private int ExpectedEventIndex { get { return this.currentTurn.ExpectedEventIndex; }  set { this.currentTurn.ExpectedEventIndex = value; } }
        private int ActualEventIndex { get { return this.currentTurn.ActualEventIndex; } set { this.currentTurn.ActualEventIndex = value; } }
        private bool VerifyEvents(bool throwIfNotVerified)
        {
            if (this.ExpectedEventIndex < this.ExpectedEvents.Count)
            {
                while (this.ActualEventIndex < this.ActualEvents.Count)
                {
                    if (this.ExpectedEvents[this.ExpectedEventIndex].Equals(this.ActualEvents[this.ActualEventIndex]))
                    {
                        this.ExpectedEventIndex++;
                    }

                    this.ActualEventIndex++;
                }
            }

            if (throwIfNotVerified && this.ExpectedEventIndex < this.ExpectedEvents.Count)
            {
                // At least one expected event was not matched with an actual event.
                var expectedEvent = this.ExpectedEvents[this.ExpectedEventIndex];
                //Assert.Fail($"Did not find {expectedEvent.GetType()} event for '{this.PlayerName}' in round {this.RoundNumber}, turn {this.TurnNumber}.\r\n{/*this.GetEventDetails(expectedEvent)*/""}");
                Assert.Fail($"Did not find {expectedEvent.GetType()} event for '{this.Name}' in round {this.RoundNumber}, turn {this.TurnNumber}.\r\n");

                throw new NotImplementedException(); // Never reached - Have to do this to pass compliation
            }
            else
            {
                return this.ExpectedEventIndex == this.ExpectedEvents.Count;
            }
        }

        public void AddActualEvent(GameEvent gameEvent)
        {
            this.ActualEvents.Add(gameEvent);
        }

        private class TurnInstructions
        {
            public int RoundNumber, TurnNumber;
            public int ExpectedEventIndex, ActualEventIndex;
            public List<object> Instructions = new List<object>();
            public List<GameEvent> ActualEvents = new List<GameEvent>();
            public List<GameEvent> ExpectedEvents = new List<GameEvent>();
            public List<ComputerPlayerAction> Actions = new List<ComputerPlayerAction>();

            public bool IsEmpty { get { return this.Instructions.Count == 0; } }
        }
    }
}
