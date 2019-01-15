using System;
using Jabberwocky.SoC.Library;
using Jabberwocky.SoC.Library.GameActions;
using Jabberwocky.SoC.Library.Interfaces;
using SoC.Library.ScenarioTests.ScenarioActions;

namespace SoC.Library.ScenarioTests.PlayerTurn
{
    internal class ComputerPlayerTurn : BasePlayerTurn
    {
        private readonly ScenarioComputerPlayer computerPlayer;

        public ComputerPlayerTurn(IPlayer player, LocalGameControllerScenarioRunner runner, int roundNumber, int turnNumber) : base(player, runner, roundNumber, turnNumber)
        {
            this.computerPlayer = (ScenarioComputerPlayer)player;
        }

        public override void ResolveActions(TurnToken turnToken, LocalGameController localGameController)
        {
            if (this.PlayerActions == null || this.PlayerActions.Count == 0)
                return;

            for (var index = 0; index < this.PlayerActions.Count; index++)
            {
                if (this.PlayerActions[index] is ScenarioPlayKnightCardAction scenarioPlayKnightCardAction)
                {
                    this.ResolveScenarioPlayKnightCardAction(scenarioPlayKnightCardAction,
                    (location, playerId) =>
                    {
                        this.PlayerActions[index] = new PlayKnightCardAction(location, playerId);
                    });
                }
            }

            this.computerPlayer.AddActions(this.PlayerActions);
        }
    }
}
