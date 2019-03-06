
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
        #region Fields
        private readonly ConcurrentQueue<GameEvent> actualEventQueue = new ConcurrentQueue<GameEvent>();
        private readonly List<TurnInstructions> turns = new List<TurnInstructions>();
        private int currentInstructionIndex;
        private TurnInstructions currentTurn;
        private GameController gameController;
        private int nextTurnIndex;
        private IDictionary<string, Guid> playerIdsByName;
        #endregion

        #region Construction
        public PlayerAgent(string name)
        {
            this.Name = name;
            this.Id = Guid.NewGuid();
            this.gameController = new GameController();
            this.gameController.GameExceptionEvent += this.GameExceptionEventHandler;
            this.gameController.GameEvent += this.GameEventHandler;
        }
        #endregion

        #region Properties
        public Exception GameException { get; private set; }
        public string Name { get; private set; }
        public Guid Id { get; private set; }
        public bool IsFinished
        {
            get
            {
                return this.nextTurnIndex >= this.turns.Count;
            }
        }
        private List<GameEvent> ActualEvents { get { return this.currentTurn.ActualEvents; } }
        private bool CurrentTurnIsFinished
        {
            get
            {
                return this.currentTurn != null &&
                    this.currentInstructionIndex >= this.currentTurn.Instructions.Count &&
                    this.ExpectedEventIndex == this.ExpectedEvents.Count;
            }
        }
        private List<GameEvent> ExpectedEvents { get { return this.currentTurn.ExpectedEvents; } }

        private int RoundNumber { get { return this.currentTurn.RoundNumber; } }
        private int TurnNumber { get { return this.currentTurn.TurnNumber; } }

        // TODO: Clean up this - either better use or no use of properties to public vars
        private int ExpectedEventIndex { get { return this.currentTurn.ExpectedEventIndex; } set { this.currentTurn.ExpectedEventIndex = value; } }
        private int ActualEventIndex { get { return this.currentTurn.ActualEventIndex; } set { this.currentTurn.ActualEventIndex = value; } }
        #endregion

        #region Methods
        public void JoinGame(LocalGameServer gameServer)
        {
            gameServer.JoinGame(this.Name, this.gameController);
        }

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

        public void ProcessInstructions()
        {
            while (this.currentInstructionIndex < this.currentTurn.Instructions.Count)
            {
                if (this.GameException != null)
                    throw this.GameException;

                var instruction = this.currentTurn.Instructions[this.currentInstructionIndex];
                if (instruction is ActionInstruction actionInstruction)
                {
                    if (!this.VerifyEvents(false))
                        return;

                    this.currentInstructionIndex++;
                    this.SendAction(actionInstruction);
                }
                else if (instruction is EventInstruction eventInstruction)
                {
                    this.currentInstructionIndex++;
                    this.ExpectedEvents.Add(eventInstruction.GetEvent(this.playerIdsByName));
                }
                else if (instruction is PlayerStateInstruction playerStateInstruction)
                {
                    // Make request for player state from game server - place expected event
                    // into list for verification
                    if (!this.VerifyEvents(false))
                        return;

                    this.currentInstructionIndex++;
                    this.ExpectedEvents.Add(playerStateInstruction.GetEvent(this.playerIdsByName));
                    this.SendAction(playerStateInstruction.GetAction());
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

            try
            {
                while (!this.IsFinished)
                {
                    Thread.Sleep(50);
                    if (this.actualEventQueue.TryDequeue(out var actualEvent))
                        this.ProcessActualEvent(actualEvent);

                    if (this.currentTurn != null)
                        this.ProcessInstructions();
                }
            }
            catch (Exception e)
            {
                this.GameException = e;
            }
        }

        private void ProcessActualEvent(GameEvent actualEvent)
        {
            var changeTurn = actualEvent is PlayerSetupEvent ||
                actualEvent is InitialBoardSetupEventArgs ||
                actualEvent is PlaceSetupInfrastructureEvent;

            if (actualEvent is PlayerSetupEvent playerSetupEvent)
                this.playerIdsByName = playerSetupEvent.Item;

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
            switch (action.Operation)
            {
                case ActionInstruction.OperationTypes.EndOfTurn:
                {
                    this.gameController.EndTurn();
                    break;
                }
                case ActionInstruction.OperationTypes.PlaceStartingInfrastructure:
                {
                    this.gameController.PlaceStartingInfrastructure((uint)action.Parameters[0], (uint)action.Parameters[1]);
                    break;
                }
                case ActionInstruction.OperationTypes.RequestState:
                {
                    this.gameController.RequestState();
                    break;
                }
                default: throw new Exception();
            }
        }
        
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
        #endregion

        #region Structures
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
        #endregion
    }
}
