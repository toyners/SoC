
namespace SoC.Library.ScenarioTests
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Jabberwocky.SoC.Library;
    using Jabberwocky.SoC.Library.GameEvents;
    using Newtonsoft.Json.Linq;
    using SoC.Library.ScenarioTests.Instructions;

    internal class PlayerAgent
    {
        #region Fields
        private readonly List<GameEvent> actualEvents = new List<GameEvent>();
        private readonly ConcurrentQueue<GameEvent> actualEventQueue = new ConcurrentQueue<GameEvent>();
        private readonly List<GameEvent> expectedEvents = new List<GameEvent>();
        private readonly HashSet<GameEvent> expectedEventsWithVerboseLogging = new HashSet<GameEvent>();
        private readonly GameController gameController;
        private readonly List<Instruction> instructions = new List<Instruction>();
        private readonly ILog log = new Log();
        private readonly bool verboseLogging;
        //private readonly Dictionary<GameEvent, bool> verificationStatusByGameEvent = new Dictionary<GameEvent, bool>();
        private int actualEventIndex;
        private int expectedEventIndex;
        private IDictionary<string, Guid> playerIdsByName;

        private List<EventActionPair> expectedEventActions = new List<EventActionPair>();
        #endregion

        #region Construction
        public PlayerAgent(string name, bool verboseLogging = false)
        {
            this.Name = name;
            this.verboseLogging = verboseLogging;
            this.Id = Guid.NewGuid();
            this.gameController = new GameController();
            this.gameController.GameExceptionEvent += this.GameExceptionEventHandler;
            this.gameController.GameEvent += this.GameEventHandler;
        }
        #endregion

        #region Properties
        public Exception GameException { get; private set; }
        public Guid Id { get; private set; }
        //public bool InstructionsProcessed { get { return this.instructionIndex >= this.instructions.Count; } }
        public bool IsFinished { get { return this.expectedEventIndex >= this.expectedEventActions.Count; } }
        public string Name { get; private set; }
        private EventActionPair CurrentEventActionPair { get { return this.expectedEventActions[this.expectedEventIndex]; } }
        private EventActionPair LastEventActionPair { get { return this.expectedEventActions[this.expectedEventActions.Count - 1]; } }
        #endregion

        #region Methods
        public void AddInstruction(Instruction instruction)
        {
            if (instruction is EventInstruction eventInstruction)
            {
                this.expectedEventActions.Add(new EventActionPair(eventInstruction.GetEvent()));
            }
            else if (instruction is ActionInstruction actionInstruction)
            {
                this.LastEventActionPair.Action = actionInstruction;
            }
            else if (instruction is PlayerStateInstruction playerStateInstruction)
            {
                this.LastEventActionPair.Action = playerStateInstruction.GetAction();
                this.expectedEventActions.Add(new EventActionPair(playerStateInstruction.GetEvent()));
            }
        }

        public List<Tuple<GameEvent, ActionInstruction, bool>> GetEventResults()
        {
            var eventResults = new List<Tuple<GameEvent, ActionInstruction, bool>>();
            this.expectedEventActions.ForEach(eventActionPair => {
                eventResults.Add(
                    new Tuple<GameEvent, ActionInstruction, bool>(
                        eventActionPair.ExpectedEvent,
                        eventActionPair.Action,
                        eventActionPair.Verified));
            });

            return eventResults;
        }

        public void JoinGame(LocalGameServer gameServer)
        {
            gameServer.JoinGame(this.Name, this.gameController);
        }

        public void SaveEvents(string filePath)
        {
            string contents = null;
            int number = 1;
            this.GetEventResults().ForEach(tuple => {
                contents += $"{number++:00} {tuple.Item1.SimpleTypeName}{(tuple.Item2 != null ? ", " + tuple.Item2.Operation : "")} => {tuple.Item3}\r\n";
            });
            System.IO.File.WriteAllText(filePath, contents);
        }

        public void SaveLog(string filePath) => this.log.WriteToFile(filePath);

        public Task StartAsync()
        {
            return Task.Factory.StartNew(o => { this.Run(); }, this, CancellationToken.None);
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
                    this.VerifyEvents();
                }
            }
            catch (Exception e)
            {
                this.GameException = e;
                this.log.Add($"ERROR: {e.Message}: {e.StackTrace}");
            }
        }

        private void SendAction(ActionInstruction action)
        {
            this.log.Add($"Sending {action.Operation} operation");
            switch (action.Operation)
            {
                case ActionInstruction.OperationTypes.AcceptTrade:
                {
                    this.gameController.AcceptDirectTradeOffer(this.playerIdsByName[(string)action.Parameters[0]]);
                    break;
                }
                case ActionInstruction.OperationTypes.AnswerDirectTradeOffer:
                {
                    this.gameController.AnswerDirectTradeOffer((ResourceClutch)action.Parameters[0]);
                    break;
                }
                case ActionInstruction.OperationTypes.ConfirmStart:
                {
                    this.gameController.ConfirmStart();
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
                case ActionInstruction.OperationTypes.QuitGame:
                {
                    this.gameController.QuitGame();
                    break;
                }
                default: throw new Exception($"Operation '{action.Operation}' not recognised");
            }
        }

        private bool IsEventVerified(GameEvent expectedEvent, GameEvent actualEvent)
        {
            if (expectedEvent is ScenarioRequestStateEvent expectedRequestEvent && actualEvent is RequestStateEvent actualRequestEvent)
                return this.IsRequestStateEventVerified(expectedRequestEvent, actualRequestEvent);
            
            return this.IsStandardEventVerified(expectedEvent, actualEvent);
        }

        private bool IsStandardEventVerified(GameEvent expectedEvent, GameEvent actualEvent)
        {
            var expectedJSON = JToken.Parse(expectedEvent.ToJSONString());
            var actualJSON = JToken.Parse(actualEvent.ToJSONString());
            var result = JToken.DeepEquals(expectedJSON, actualJSON);

            if ((!result && expectedEvent.GetType() == actualEvent.GetType()) || 
                this.verboseLogging || 
                this.expectedEventsWithVerboseLogging.Contains(expectedEvent))
            {
                this.log.Add($"{(result ? "MATCHED" : "NOT MATCHED")}");
                this.log.Add($" EXPECTED: {expectedJSON}");
                this.log.Add($" ACTUAL: {actualJSON}");
            }
            else
            {
                this.log.Add($"{(result ? "MATCHED" : "NOT MATCHED")} - Expected {expectedEvent.SimpleTypeName}, Actual {actualEvent.SimpleTypeName}");
            }

            return result;
        }

        private bool IsRequestStateEventVerified(ScenarioRequestStateEvent expectedEvent, RequestStateEvent actualEvent)
        {
            var result = expectedEvent.Resources.HasValue && expectedEvent.Resources.Value != actualEvent.Resources;

            this.log.Add($"{(result ? "MATCHED" : "NOT MATCHED")} - Expected {expectedEvent.SimpleTypeName}, Actual {actualEvent.SimpleTypeName}");

            return result;
        }

        private void VerifyEvents()
        {
            if (this.expectedEventIndex >= this.expectedEventActions.Count)
                return;

            if (this.IsEventVerified(this.CurrentEventActionPair.ExpectedEvent,
                    this.actualEvents[this.actualEvents.Count - 1]))
            {
                this.CurrentEventActionPair.Verified = true;
                if (this.CurrentEventActionPair.Action != null)
                    this.SendAction(this.CurrentEventActionPair.Action);
                this.expectedEventIndex++;
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

        #region Structures
        private class EventActionPair
        {
            public EventActionPair(GameEvent gameEvent)
            {
                this.ExpectedEvent = gameEvent;
            }

            public GameEvent ExpectedEvent { get; private set; }
            public ActionInstruction Action { get; set; }
            public bool Verified { get; set; }
        }
        #endregion
    }
}
