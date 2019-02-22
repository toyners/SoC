
namespace SoC.Library.ScenarioTests
{
    using System;
    using System.Collections.Generic;
    using Jabberwocky.SoC.Library;
    using Jabberwocky.SoC.Library.GameEvents;
    using NUnit.Framework;
    using SoC.Library.ScenarioTests.PlayerTurn;

    internal class Player2
    {
        private BasePlayerTurn currentTurn;
        private List<BasePlayerTurn> turns = new List<BasePlayerTurn>();

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
        public bool CurrentTurnIsFinished
        {
            get
            {
                return this.currentTurn.InstructionIndex >= this.Instructions.Count &&
                    this.currentTurn.IsVerified;
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
                this.AddActualEvent(gameEvent);
            }
        }

        private void GameExceptionEventHandler(Exception exception)
        {
            this.GameException = exception;
        }


        public void InsertTurnInstructions(BasePlayerTurn turn)
        {
            this.turns.Add(turn);
            /*var turn = new TurnInstructions
            {
                RoundNumber = roundNumber,
                TurnNumber = turnNumber
            };

            this.turns.Add(turn);
            if (instructions != null)
            {
                foreach (var instruction in instructions)
                {
                    if (instruction is GameEvent gameEvent)
                        turn.Instructions.Add(gameEvent);
                    else if (instruction is Instruction2 scenarioAction && 
                        scenarioAction.PlayerName == this.PlayerName)
                    {
                        turn.Instructions.Add(scenarioAction.Payload);
                    }
                }
            }*/

            if (this.currentTurn == null)
                this.currentTurn = this.turns[this.nextTurnIndex++];
        }

        private List<GameEvent> ExpectedEvents = new List<GameEvent>();
        private Queue<object> Instructions { get { return this.currentTurn.Instructions; } }
        //public List<GameEvent> ActualEvents { get { return this.currentTurn.ActualEvents; } }
        //public List<GameEvent> ExpectedEvents { get { return this.currentTurn.ExpectedEvents; } }
        public void Process()
        {
            while (this.Instructions.Count > 0)
            {
                if (this.GameException != null)
                    throw this.GameException;

                var instruction = (Instruction2)this.Instructions.Peek();
                if (instruction.PlayerName != this.PlayerName)
                    break;

                this.Instructions.Dequeue();
                var payload = instruction.Payload;
                if (payload is ActionInstruction action)
                {
                    if (this.VerifyEvents(false))
                    {
                        this.SendAction(action);
                        break;
                    }
                }
                else if (payload is GameEvent gameEvent)
                {
                    this.ExpectedEvents.Add(gameEvent);
                }
            }

            this.VerifyEvents(true);
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
                while (this.actualEventIndex < this.actualEvents.Count)
                {
                    if (this.ExpectedEvents[this.expectedEventIndex].Equals(this.actualEvents[this.actualEventIndex]))
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

        private List<GameEvent> actualEvents = new List<GameEvent>();
        public void AddActualEvent(GameEvent gameEvent)
        {
            this.actualEvents.Add(gameEvent);
        }

        /*private List<TurnInstructions> turns = new List<TurnInstructions>();
        private class TurnInstructions
        {
            private int nextActionIndex;
            public int RoundNumber, TurnNumber;
            
            public List<object> Instructions = new List<object>();
            public List<GameEvent> ActualEvents = new List<GameEvent>();
            public List<GameEvent> ExpectedEvents = new List<GameEvent>();
            public List<ComputerPlayerAction> Actions = new List<ComputerPlayerAction>();
        }*/
    }
}
