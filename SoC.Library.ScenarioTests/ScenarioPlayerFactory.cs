
namespace SoC.Library.ScenarioTests
{
    using System;
    using System.Collections.Generic;
    using System.Xml;
    using Jabberwocky.SoC.Library;
    using Jabberwocky.SoC.Library.GameBoards;
    using Jabberwocky.SoC.Library.Interfaces;
    using Jabberwocky.SoC.Library.Store;

    internal class ScenarioPlayerFactory : IPlayerFactory
    {
        private readonly Queue<string> names = new Queue<string>();
        private readonly Dictionary<string, IPlayerSetupAction[]> playerSetupActionsByName = new Dictionary<string, IPlayerSetupAction[]>();

        public IPlayer CreateComputerPlayer(GameBoard gameBoard, LocalGameController localGameController, INumberGenerator numberGenerator)
        {
            var player = new ScenarioComputerPlayer(this.names.Dequeue(), gameBoard, localGameController, numberGenerator);
            return player;
        }

        public IPlayer CreateComputerPlayer(IGameDataSection<GameDataSectionKeys, GameDataValueKeys, ResourceTypes> data, GameBoard board, INumberGenerator numberGenerator)
        {
            throw new NotImplementedException();
        }

        public IPlayer CreatePlayer()
        {
            throw new NotImplementedException();
        }

        public IPlayer CreatePlayer(XmlReader reader)
        {
            throw new NotImplementedException();
        }

        public IPlayer CreatePlayer(IGameDataSection<GameDataSectionKeys, GameDataValueKeys, ResourceTypes> data)
        {
            throw new NotImplementedException();
        }

        public Guid GetBankId()
        {
            throw new NotImplementedException();
        }

        public void AddPlayer(string name)
        {
            this.names.Enqueue(name);
        }
        
        public void AddPlayerSetup(string playerName, IPlayerSetupAction[] playerSetupActions)
            => this.playerSetupActionsByName.Add(playerName, playerSetupActions);

        public IPlayer CreatePlayer(string name, Guid id)
        {
            var scenarioPlayer = new ScenarioPlayer(name, id);
            if (this.playerSetupActionsByName.ContainsKey(name))
            {
                foreach (var playerSetupAction  in this.playerSetupActionsByName[name])
                {
                    playerSetupAction.Process(scenarioPlayer);
                }
            }

            return scenarioPlayer;
        }
    }
}
