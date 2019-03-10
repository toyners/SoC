
namespace SoC.Library.ScenarioTests
{
    using System;
    using Jabberwocky.SoC.Library;
    using Jabberwocky.SoC.Library.GameEvents;

    internal class ComputerPlayer2 : PlayerAgent_Old
    {
        public ComputerPlayer2(string playerName) : base(playerName)
        {
            //this.gameController.GameEvent += this.GameEventHandler;
        }

        /*private void GameEventHandler(GameEvent gameEvent)
        {
            if (gameEvent is InitialBoardSetupEventArgs initialBoardSetupEventArgs)
            {
                this.curr
                return;
            }

            throw new NotImplementedException($"Game event {gameEvent.GetType()} not handled");
        }*/
    }
}
