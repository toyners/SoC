using System;
using Jabberwocky.SoC.Library;
using Jabberwocky.SoC.Library.GameActions;
using Jabberwocky.SoC.Library.Interfaces;

namespace SoC.Library.ScenarioTests.PlayerTurn
{
    internal class ComputerPlayerTurn : BasePlayerTurn
    {
        private readonly ScenarioComputerPlayer computerPlayer;

        public ComputerPlayerTurn(IPlayer player, uint dice1, uint dice2, LocalGameControllerScenarioRunner runner, int roundNumber, int turnNumber) : base(player, dice1, dice2, runner, roundNumber, turnNumber)
        {
            this.computerPlayer = (ScenarioComputerPlayer)player;
        }

        public override void ResolveActions(TurnToken turnToken, LocalGameController localGameController)
        {
            if (this.actionBuilder == null)
                return;

            for (var index = 0; index < this.actionBuilder.playerActions.Count; index++)
            {
                if (this.actionBuilder.playerActions[index] is ScenarioPlayKnightCardAction scenarioPlayKnightCardAction)
                {
                    this.ResolveScenarioPlayKnightCardAction(scenarioPlayKnightCardAction,
                    (location, playerId) =>
                    {
                        this.actionBuilder.playerActions[index] = new PlayKnightCardAction(location, playerId);
                    });
                }
            }

            this.computerPlayer.AddActions(this.actionBuilder.playerActions);
        }
    }
}
