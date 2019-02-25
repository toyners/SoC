
namespace SoC.Library.ScenarioTests
{
    using System;
    using Jabberwocky.SoC.Library;
    using Jabberwocky.SoC.Library.GameEvents;

    internal class HumanPlayer : PlayerAgent
    {
        public HumanPlayer(string playerName) : base(playerName)
        {
            //this.gameController.GameEvent += this.GameEventHandler;
        }

        /*private void GameEventHandler(GameEvent gameEvent)
        {
            if (gameEvent is InitialBoardSetupEventArgs initialBoardSetupEventArgs)
            {
                return;
            }

            throw new NotImplementedException($"Game event {gameEvent.GetType()} not handled");
        }*/
    }
}
