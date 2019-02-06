
namespace SoC.Library.ScenarioTests
{
    using System;
    using System.Collections.Generic;
    using System.Xml;
    using Jabberwocky.SoC.Library;
    using Jabberwocky.SoC.Library.GameBoards;
    using Jabberwocky.SoC.Library.Interfaces;
    using Jabberwocky.SoC.Library.Store;

    public class ScenarioPlayerPool : IPlayerPool
    {
        private readonly Queue<IPlayer> players = new Queue<IPlayer>();

        public IPlayer CreateComputerPlayer(GameBoard gameBoard, LocalGameController localGameController, INumberGenerator numberGenerator)
        {
            return this.players.Dequeue();
        }

        public IPlayer CreateComputerPlayer(IGameDataSection<GameDataSectionKeys, GameDataValueKeys, ResourceTypes> data, GameBoard board, INumberGenerator numberGenerator)
        {
            throw new NotImplementedException();
        }

        public IPlayer CreatePlayer()
        {
            return this.players.Dequeue();
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

        public void AddPlayer(IPlayer player)
        {
            this.players.Enqueue(player);
        }
    }
}
