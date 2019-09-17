using System;
using System.Threading.Tasks;
using Jabberwocky.SoC.Library;
using Jabberwocky.SoC.Library.GameBoards;
using Jabberwocky.SoC.Library.GameEvents;
using Jabberwocky.SoC.Library.Interfaces;
using Jabberwocky.SoC.Library.PlayerActions;

namespace SoC.Library.ScenarioTests
{
    public interface IScenarioGameManager : IGameManager
    {
        void JoinGame(string playerName);
    }

    public class ScenarioGameManager : GameManager, IScenarioGameManager
    {
        public ScenarioGameManager(INumberGenerator numberGenerator, GameBoard gameBoard, IDevelopmentCardHolder developmentCardHolder, IPlayerFactory playerFactory, IEventSender eventSender)
            : base(numberGenerator, gameBoard, developmentCardHolder, playerFactory, eventSender) {}

        public void JoinGame(string playerName)
        {
            var player = this.playerFactory.CreatePlayer(playerName, this.idGenerator.Invoke());
            this.players[this.playerIndex++] = player;
            this.RaiseEvent(new GameJoinedEvent(player.Id), player);
        }
    }
}
