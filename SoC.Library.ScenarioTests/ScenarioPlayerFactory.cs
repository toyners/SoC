
namespace SoC.Library.ScenarioTests
{
    using System;
    using System.Collections.Generic;
    using System.Xml;
    using Jabberwocky.SoC.Library;
    using Jabberwocky.SoC.Library.GameBoards;
    using Jabberwocky.SoC.Library.Interfaces;
    using Jabberwocky.SoC.Library.Store;

    internal class ScenarioPlayerFactory : IPlayerPool
    {
        private readonly Queue<string> names = new Queue<string>();

        public Dictionary<string, IPlayer> PlayersByName { get; } = new Dictionary<string, IPlayer>();

        public IPlayer CreateComputerPlayer(GameBoard gameBoard, LocalGameController localGameController, INumberGenerator numberGenerator)
        {
            var player = new ScenarioComputerPlayer(this.names.Dequeue(), gameBoard, localGameController, numberGenerator);
            this.PlayersByName.Add(player.Name, player);
            return player;
        }

        public IPlayer CreateComputerPlayer(IGameDataSection<GameDataSectionKeys, GameDataValueKeys, ResourceTypes> data, GameBoard board, INumberGenerator numberGenerator)
        {
            throw new NotImplementedException();
        }

        public IPlayer CreatePlayer()
        {
            var player = new ScenarioPlayer(this.names.Dequeue());
            this.PlayersByName.Add(player.Name, player);
            return player;
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

        public void AddPlayerSetup(string name, IPlayerSetupActions[] playerSetupActions)
            => throw new NotImplementedException();
    }
}
