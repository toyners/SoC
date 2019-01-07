using System;
using Jabberwocky.SoC.Library;
using Jabberwocky.SoC.Library.GameActions;
using Jabberwocky.SoC.Library.Interfaces;

namespace SoC.Library.ScenarioTests.PlayerTurn
{
    internal class ComputerPlayerTurn : BasePlayerTurn
    {
        private readonly ScenarioComputerPlayer computerPlayer;

        public ComputerPlayerTurn(IPlayer player, uint dice1, uint dice2, LocalGameControllerScenarioRunner runner) : base(player, dice1, dice2, runner)
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
                    var selectedPlayer = this.runner.GetPlayerFromName(scenarioPlayKnightCardAction.SelectedPlayerName);

                    var randomNumber = int.MinValue;
                    switch (scenarioPlayKnightCardAction.ExpectedSingleResource)
                    {
                        case ResourceTypes.Ore:
                            randomNumber = selectedPlayer.Resources.BrickCount +
                            selectedPlayer.Resources.GrainCount +
                            selectedPlayer.Resources.LumberCount;
                            break;
                        default: throw new Exception($"Resource type '{scenarioPlayKnightCardAction.ExpectedSingleResource}' not handled");
                    }

                    this.runner.NumberGenerator.AddRandomNumber(randomNumber);
                    this.actionBuilder.playerActions[index] = new PlayKnightCardAction(scenarioPlayKnightCardAction.NewRobberHex, selectedPlayer.Id);
                }
            }

            this.computerPlayer.AddActions(this.actionBuilder.playerActions);
        }
    }
}
