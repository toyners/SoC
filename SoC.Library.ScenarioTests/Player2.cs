
namespace SoC.Library.ScenarioTests
{
    using System;
    using System.Collections.Generic;
    using Jabberwocky.SoC.Library;
    using Jabberwocky.SoC.Library.GameActions;
    using Jabberwocky.SoC.Library.GameEvents;

    internal class Player2
    {
        private TurnInstructions currentTurn;
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
                foreach (var instruction in instructions)
                {
                    if (instruction is GameEvent gameEvent)
                        turn.ExpectedEvents.Add(gameEvent);
                    else if (instruction is ScenarioActionWrapper scenarioAction && 
                        scenarioAction.PlayerName == this.PlayerName)
                    {
                        turn.Actions.Add(scenarioAction.Action);
                    }
                }
            }

            if (this.currentTurn == null)
                this.currentTurn = this.turns[this.nextTurnIndex++];
        }

        private List<TurnInstructions> turns = new List<TurnInstructions>();
        private class TurnInstructions
        {
            private int nextActionIndex;
            private int nextExpectedIndex;
            public List<GameEvent> ActualEvents = new List<GameEvent>();
            public List<GameEvent> ExpectedEvents = new List<GameEvent>();
            public List<ComputerPlayerAction> Actions = new List<ComputerPlayerAction>();

            public bool IsFinished
            {
                get
                {
                    return this.nextActionIndex >= this.Actions.Count &&
                        this.nextExpectedIndex >= this.ExpectedEvents.Count;
                }
            }
        }

        public void Process()
        {
            throw new NotImplementedException();
        }
    }
}
