using Jabberwocky.SoC.Library;
using Jabberwocky.SoC.Library.GameEvents;
using Jabberwocky.SoC.Library.Interfaces;

namespace SoC.Library.ScenarioTests.PlayerTurn
{
    internal class ComputerPlayerTurn : GameTurn
    {
        private readonly ScenarioComputerPlayer computerPlayer;

        public ComputerPlayerTurn(IPlayer player, LocalGameControllerScenarioRunner runner, int roundNumber, int turnNumber) : base(player, runner)
        {
            this.computerPlayer = (ScenarioComputerPlayer)player;
        }

        public override void ResolveActions(GameToken turnToken, LocalGameController localGameController)
        {
            /*if (this.PlayerActions == null || this.PlayerActions.Count == 0)
                return;

            for (var index = 0; index < this.PlayerActions.Count; index++)
            {
                if (this.PlayerActions[index] is ScenarioPlaceRobberAction scenarioPlaceRobberAction)
                {
                    this.PlayerActions[index] = new PlaceRobberAction(scenarioPlaceRobberAction.NewRobberHex);
                }
                else if (this.PlayerActions[index] is ScenarioPlayKnightCardAction scenarioPlayKnightCardAction)
                {
                    var selectedPlayer = this.runner.GetPlayerFromName(scenarioPlayKnightCardAction.SelectedPlayerName);
                    this.SetupResourceSelectionOnPlayer(selectedPlayer, scenarioPlayKnightCardAction.ExpectedSingleResource);
                    this.PlayerActions[index] = new PlayKnightCardAction(scenarioPlayKnightCardAction.NewRobberHex, selectedPlayer.Id);
                }
                else if (this.PlayerActions[index] is ScenarioSelectResourceFromPlayerAction scenarioSelectResourceFromPlayerAction)
                {
                    var selectedPlayer = this.runner.GetPlayerFromName(scenarioSelectResourceFromPlayerAction.SelectedPlayerName);
                    this.SetupResourceSelectionOnPlayer(selectedPlayer, scenarioSelectResourceFromPlayerAction.ExpectedSingleResource);
                    this.PlayerActions[index] = new SelectResourceFromPlayerAction(selectedPlayer.Id);
                }
            }

            this.computerPlayer.AddActions(this.PlayerActions);*/
        }

        public override void AddEvent(GameEvent gameEvent)
        {
            this.computerPlayer.AddEvent(gameEvent);
        }
    }
}
