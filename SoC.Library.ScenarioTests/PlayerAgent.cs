
namespace SoC.Library.ScenarioTests
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
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
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly List<JToken> didNotReceiveEvents = new List<JToken>();
        private readonly Dictionary<Type, int> maximumEventTypeCountsByEventType = new Dictionary<Type, int>();
        private readonly Dictionary<Type, int> eventTypeCountsByEventType = new Dictionary<Type, int>();

        private readonly List<EventActionPair> expectedEventActions = new List<EventActionPair>();
        private readonly HashSet<GameEvent> expectedEventsWithVerboseLogging = new HashSet<GameEvent>();
        private readonly GameController gameController;
        private readonly ILog log = new Log();
        private readonly bool verboseLogging;
        private CancellationToken cancellationToken;
        private int expectedEventIndex;
        private IDictionary<string, Guid> playerIdsByName;
        #endregion

        #region Construction
        public PlayerAgent(string name, bool verboseLogging = false)
        {
            this.Name = name;
            this.verboseLogging = verboseLogging;
            this.Id = Guid.NewGuid();
            this.gameController = new GameController();
            this.gameController.GameEvent += this.GameEventHandler;
            this.cancellationToken = this.cancellationTokenSource.Token;
        }
        #endregion

        #region Properties
        public Guid Id { get; private set; }
        public bool IsFinished { get { return this.expectedEventIndex >= this.expectedEventActions.Count; } }
        public string Name { get; private set; }
        public bool FinishWhenAllEventsVerified { get; set; } = true;
        private EventActionPair CurrentEventActionPair { get { return this.expectedEventActions[this.expectedEventIndex]; } }
        private EventActionPair LastEventActionPair { get { return this.expectedEventActions[this.expectedEventActions.Count - 1]; } }
        #endregion

        #region Methods
        public void AddDidNotReceiveEvent(GameEvent gameEvent)
        {
            var gameEventToken = JToken.Parse(gameEvent.ToJSONString());
            this.didNotReceiveEvents.Add(gameEventToken);
        }

        public void AddDidNotReceiveEventType<T>() where T : GameEvent
        {
            this.AddDidNotReceiveEventType<T>(1);
        }

        public void AddDidNotReceiveEventType<T>(int eventCount) where T : GameEvent
        {
            this.maximumEventTypeCountsByEventType.Add(typeof(T), eventCount);
            this.eventTypeCountsByEventType.Add(typeof(T), 0);
        }

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

        public void JoinGame(LocalGameManager gameServer)
        {
            gameServer.JoinGame(this.Name, this.gameController);
        }

        public void Quit()
        {
            this.cancellationTokenSource.Cancel();
        }

        public void SaveEvents(string filePath)
        {
            string contents = null;
            int number = 1;
            this.expectedEventActions.ForEach(eventAction => {
                contents += $"{number++:00} {eventAction.ToString()}\r\n";
            });
            System.IO.File.WriteAllText(filePath, contents);
        }

        public void SaveLog(string filePath) => this.log.WriteToFile(filePath);

        public Task StartAsync()
        {
            return Task.Factory.StartNew(o => { this.Run(); }, this, this.cancellationToken);
        }

        private void GameEventHandler(GameEvent gameEvent)
        {
            this.actualEventQueue.Enqueue(gameEvent);
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

                if (!this.FinishWhenAllEventsVerified)
                {
                    this.log.Add("Continue running and receiving game events");
                    while (!this.cancellationToken.IsCancellationRequested)
                    {
                        Thread.Sleep(50);
                        if (this.actualEventQueue.TryDequeue(out var actualEvent))
                        {
                            this.VerifyActualEvent(actualEvent);
                            this.log.Add($"Received {actualEvent.GetType().Name}");
                        }
                    }
                }

                this.log.Add("Finished");
            }
            catch (OperationCanceledException)
            {
                return;
            }
            catch (Exception e)
            {
                this.log.Add($"ERROR: {e.Message}: {e.StackTrace}");
                throw e;
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

        public void SetVerboseLoggingOnVerificationOfPreviousEvent(bool verboseLogging)
        {
            throw new NotImplementedException();
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
                this.log.Add($"   EXPECTED: " +
                    $"{(expectedEvent.PlayerId != Guid.Empty ? "Player name is " + this.GetPlayerName(expectedEvent.PlayerId) : "")}\r\n" +
                    $"    {expectedJSON}");
                this.log.Add($"   ACTUAL: " +
                    $"{(actualEvent.PlayerId != Guid.Empty ? "Player name is " + this.GetPlayerName(actualEvent.PlayerId) : "")}\r\n" +
                    $"    {actualJSON}");
            }
            else
            {
                this.log.Add($"{(result ? "MATCHED" : "NOT MATCHED")} - Expected {expectedEvent.SimpleTypeName}, Actual {actualEvent.SimpleTypeName}");
            }

            return result;
        }

        private string GetPlayerName(Guid playerId)
        {
            foreach(var kv in this.playerIdsByName)
            {
                if (kv.Value == playerId)
                    return kv.Key;
            }

            throw new KeyNotFoundException();
        }

        private bool IsRequestStateEventVerified(ScenarioRequestStateEvent expectedEvent, RequestStateEvent actualEvent)
        {
            var result = expectedEvent.Resources.HasValue && expectedEvent.Resources.Value == actualEvent.Resources;
            result &= expectedEvent.RoadSegment.HasValue && expectedEvent.RoadSegment.Value == actualEvent.RoadSegments;
            result &= expectedEvent.VictoryPoints.HasValue && expectedEvent.VictoryPoints.Value == actualEvent.VictoryPoints;

            this.log.Add($"{(result ? "MATCHED" : "NOT MATCHED")} - Expected {expectedEvent.SimpleTypeName}, Actual {actualEvent.SimpleTypeName}");
            if (!result ||
                this.verboseLogging ||
                this.expectedEventsWithVerboseLogging.Contains(expectedEvent))
            {
                this.log.Add($" EXPECTED: {expectedEvent.Resources.Value}");
                this.log.Add($" ACTUAL: {actualEvent.Resources}");
            }

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
                this.cancellationToken.ThrowIfCancellationRequested();
                Thread.Sleep(50);
                if (!this.actualEventQueue.TryDequeue(out var actualEvent))
                    continue;

                if (actualEvent is PlayerSetupEvent playerSetupEvent)
                    this.playerIdsByName = playerSetupEvent.PlayerIdsByName;

                this.VerifyActualEvent(actualEvent);
                
                this.log.Add($"Received {actualEvent.GetType().Name}");

                this.actualEvents.Add(actualEvent);
                break;
            }
        }

        private void VerifyActualEvent(GameEvent actualEvent)
        {
            var actualEventType = actualEvent.GetType();
            if (this.maximumEventTypeCountsByEventType.ContainsKey(actualEventType))
            {
                this.eventTypeCountsByEventType[actualEventType]++;
                if (this.eventTypeCountsByEventType[actualEventType] > 
                    this.maximumEventTypeCountsByEventType[actualEventType])
                {
                    if (this.maximumEventTypeCountsByEventType[actualEventType] > 0)
                    {
                        throw new Exception($"Received {this.eventTypeCountsByEventType[actualEventType]} event(s)" +
                            $" of type {actualEvent.GetType()} but should have received only " +
                            $"{this.maximumEventTypeCountsByEventType[actualEventType]}");
                    }

                    throw new Exception($"Received event of type {actualEvent.GetType()} but should not have");
                }
            }

            if (this.didNotReceiveEvents.FirstOrDefault(d => JToken.DeepEquals(d, JToken.Parse(actualEvent.ToJSONString()))) != null)
                throw new Exception($"Received event {actualEvent.GetType()} but should not have");
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

            public override string ToString()
            {
                return $"{this.ToExpectedEventString()} -> " +
                    $"{(this.Verified ? "Verified": "NOT VERIFIED")}" +
                    $"{(this.Action != null ? ", " + "ACTION: " + this.Action.Operation : "(no action)")}";
            }

            private string ToExpectedEventString()
            {
                if (this.ExpectedEvent is DiceRollEvent diceRollEvent)
                    return $"{diceRollEvent.SimpleTypeName}[{diceRollEvent.Dice1},{diceRollEvent.Dice2}]";

                return this.ExpectedEvent.SimpleTypeName;
            }
        }
        #endregion
    }
}
