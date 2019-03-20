
namespace SoC.Library.ScenarioTests
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Jabberwocky.SoC.Library;
    using Jabberwocky.SoC.Library.GameEvents;
    using NUnit.Framework;
    using SoC.Library.ScenarioTests.Instructions;

    internal class PlayerAgent
    {
        #region Fields
        private readonly List<GameEvent> actualEvents = new List<GameEvent>();
        private readonly ConcurrentQueue<GameEvent> actualEventQueue = new ConcurrentQueue<GameEvent>();
        private readonly List<GameEvent> expectedEvents = new List<GameEvent>();
        private readonly GameController gameController;
        private readonly List<Instruction> instructions = new List<Instruction>();
        private readonly ILog log = new Log();
        private readonly Dictionary<GameEvent, bool> verificationStatusByGameEvent = new Dictionary<GameEvent, bool>();
        private int actualEventIndex;
        private int expectedEventIndex;
        private int instructionIndex;
        private string label;
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
        public Guid Id { get; private set; }
        public bool IsFinished
        {
            get {
                return this.instructionIndex >= this.instructions.Count &&
                  this.expectedEventIndex >= this.expectedEvents.Count;
            }
        }
        public string Name { get; private set; }
        #endregion

        #region Methods
        public void AddInstruction(Instruction instruction) => this.instructions.Add(instruction);

        public void JoinGame(LocalGameServer gameServer)
        {
            gameServer.JoinGame(this.Name, this.gameController);
        }

        public void SaveLog(string filePath) => this.log.WriteToFile(filePath);

        internal void StartAsync()
        {
            Task.Factory.StartNew(() => this.Run());
        }

        private void GameEventHandler(GameEvent gameEvent)
        {
            this.actualEventQueue.Enqueue(gameEvent);
        }

        private void GameExceptionEventHandler(Exception exception)
        {
            this.GameException = exception;
        }

        private void Run()
        {
            Thread.CurrentThread.Name = this.Name;

            try
            {
                while (!this.IsFinished)
                {
                    this.WaitForGameEvent();
                    this.ProcessInstructions();
                }
            }
            catch (Exception e)
            {
                this.GameException = e;
            }
        }

        private void ProcessInstructions()
        {
            while (this.instructionIndex < this.instructions.Count)
            {
                if (this.GameException != null)
                    throw this.GameException;

                var instruction = this.instructions[this.instructionIndex];
                if (instruction is LabelInstruction labelInstruction)
                {
                    this.instructionIndex++;
                    this.label = labelInstruction.Label;
                }
                else if (instruction is ActionInstruction actionInstruction)
                {
                    if (!this.VerifyEvents(false))
                        return;

                    this.instructionIndex++;
                    this.SendAction(actionInstruction);
                }
                else if (instruction is EventInstruction eventInstruction)
                {
                    this.instructionIndex++;
                    var expectedEvent = eventInstruction.GetEvent(this.playerIdsByName);
                    this.expectedEvents.Add(expectedEvent);
                    this.verificationStatusByGameEvent.Add(expectedEvent, false);
                }
                else if (instruction is PlayerStateInstruction playerStateInstruction)
                {
                    // Make request for player state from game server - place expected event
                    // into list for verification
                    if (!this.VerifyEvents(false))
                        return;

                    this.instructionIndex++;
                    this.expectedEvents.Add(playerStateInstruction.GetEvent(this.playerIdsByName));
                    this.SendAction(playerStateInstruction.GetAction());
                }
            }

            this.VerifyEvents(false);
        }

        private void SendAction(ActionInstruction action)
        {
            this.log.Add($"Sending {action.Operation} operation");
            switch (action.Operation)
            {
                case ActionInstruction.OperationTypes.AcceptTrade:
                {
                    this.gameController.AcceptDirectTradeOffer();
                    break;
                }
                case ActionInstruction.OperationTypes.AnswerDirectTradeOffer:
                {
                    this.gameController.AnswerDirectTradeOffer((ResourceClutch)action.Parameters[0]);
                    break;
                }
                case ActionInstruction.OperationTypes.EndOfTurn:
                {
                    this.gameController.EndTurn();
                    break;
                }
                case ActionInstruction.OperationTypes.MakeDirectTradeOffer:
                {
                    this.gameController.MakeDirectTradeOffer((ResourceClutch)action.Parameters[0]);
                    break;
                }
                case ActionInstruction.OperationTypes.PlaceStartingInfrastructure:
                {
                    this.gameController.PlaceSetupInfrastructure((uint)action.Parameters[0], (uint)action.Parameters[1]);
                    break;
                }
                case ActionInstruction.OperationTypes.RequestState:
                {
                    this.gameController.RequestState();
                    break;
                }
                default: throw new Exception($"Operation '{action.Operation}' not recognised");
            }
        }

        private bool VerifyEvents(bool throwIfNotVerified)
        {
            if (this.expectedEventIndex < this.expectedEvents.Count)
            {
                while (this.actualEventIndex < this.actualEvents.Count)
                {
                    if (this.expectedEvents[this.expectedEventIndex].Equals(this.actualEvents[this.actualEventIndex]))
                    {
                        this.verificationStatusByGameEvent[this.expectedEvents[this.expectedEventIndex]] = true;
                        this.expectedEventIndex++;
                    }

                    this.actualEventIndex++;
                }
            }

            if (throwIfNotVerified && this.expectedEventIndex < this.expectedEvents.Count)
            {
                // At least one expected event was not matched with an actual event.
                var expectedEvent = this.expectedEvents[this.expectedEventIndex];
                //Assert.Fail($"Did not find {expectedEvent.GetType()} event for '{this.PlayerName}' in round {this.RoundNumber}, turn {this.TurnNumber}.\r\n{/*this.GetEventDetails(expectedEvent)*/""}");
                Assert.Fail($"{this.label} Did not find {expectedEvent.GetType()} event for '{this.Name}'.\r\n");

                throw new NotImplementedException(); // Never reached - Have to do this to pass compliation
            }
            else
            {
                return this.expectedEventIndex >= this.expectedEvents.Count;
            }
        }

        private void WaitForGameEvent()
        {
            while (true)
            {
                Thread.Sleep(50);
                if (!this.actualEventQueue.TryDequeue(out var actualEvent))
                    continue;

                if (actualEvent is PlayerSetupEvent playerSetupEvent)
                    this.playerIdsByName = playerSetupEvent.PlayerIdsByName;

                this.log.Add($"Received {actualEvent.GetType().Name}");

                this.actualEvents.Add(actualEvent);
                break;
            }
        }
        #endregion
    }
}
