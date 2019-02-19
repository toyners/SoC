
namespace SoC.Library.ScenarioTests
{
    using System;
    using System.Collections.Generic;
    using Jabberwocky.SoC.Library;
    using Jabberwocky.SoC.Library.GameActions;
    using Jabberwocky.SoC.Library.GameEvents;

    public class Player2
    {
        protected GameController gameController;

        public Player2(string playerName)
        {
            this.PlayerName = playerName;
            this.gameController = new GameController();
            this.gameController.GameExceptionEvent += this.GameExceptionEventHandler;
        }

        public Exception GameException { get; private set; }
        public string PlayerName { get; private set; }

        public void JoinGame(LocalGameServer gameServer)
        {
            gameServer.JoinGame(this.PlayerName, this.gameController);
        }

        private void GameExceptionEventHandler(Exception exception)
        {
            this.GameException = exception;
        }

        public void InsertTurnInstructions(IEnumerable<object> instructions)
        {
            var turn = new TurnInstructions();
            this.turns.Enqueue(turn);
            if (instructions != null)
            {
                foreach (var instruction in instructions)
                {
                    if (instruction is GameEvent gameEvent)
                        turn.ExpectedEvents.Add(gameEvent);
                    else if (instruction is ComputerPlayerAction action)
                    {

                    }
                }
            }
        }

        private Queue<TurnInstructions> turns = new Queue<TurnInstructions>();
        private class TurnInstructions
        {
            public List<GameEvent> ExpectedEvents = new List<GameEvent>();
            public List<ComputerPlayerAction> Actions = new List<ComputerPlayerAction>();
        }
    }
}
