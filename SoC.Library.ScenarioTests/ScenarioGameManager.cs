
namespace SoC.Library.ScenarioTests
{
    using System.Linq;
    using Jabberwocky.SoC.Library;
    using Jabberwocky.SoC.Library.GameBoards;
    using Jabberwocky.SoC.Library.GameEvents;
    using Jabberwocky.SoC.Library.Interfaces;
    using SoC.Library.ScenarioTests.Interfaces;

    public class ScenarioGameManager : GameManager, IScenarioGameManager
    {
        public ScenarioGameManager(
            INumberGenerator numberGenerator, 
            GameBoard gameBoard, 
            IDevelopmentCardHolder developmentCardHolder, 
            IPlayerFactory playerFactory, 
            IEventSender eventSender, 
            GameOptions gameOptions)
            : base(numberGenerator, gameBoard, developmentCardHolder, playerFactory, eventSender, gameOptions) {}

        public void AddResourcesToPlayer(string playerName, ResourceClutch value)
        {
            this.players
                .Where(p => p.Name == playerName)
                .FirstOrDefault()
                ?.AddResources(value);
        }

        /*public void JoinGame(string playerName)
        {
            var player = this.playerFactory.CreatePlayer(playerName, this.idGenerator.Invoke());
            this.players[this.playerIndex++] = player;
            this.RaiseEvent(new GameJoinedEvent(player.Id), player);
        }*/
    }
}
