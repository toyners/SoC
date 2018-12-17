using System;
using System.Collections.Generic;
using System.Xml;
using Jabberwocky.SoC.Library;
using Jabberwocky.SoC.Library.GameBoards;
using Jabberwocky.SoC.Library.Interfaces;
using Jabberwocky.SoC.Library.Store;

namespace SoC.Library.ScenarioTests
{
    public class MockPlayerPool : IPlayerPool
    {
        private readonly Queue<IPlayer> players = new Queue<IPlayer>();
        //public Dictionary<Guid, MockComputerPlayer> ComputerPlayers = new Dictionary<Guid, MockComputerPlayer>(); 

        public IPlayer CreateComputerPlayer(GameBoard gameBoard, INumberGenerator numberGenerator)
        {
            return players.Dequeue();
        }

        public IPlayer CreateComputerPlayer(IGameDataSection<GameDataSectionKeys, GameDataValueKeys, ResourceTypes> data, GameBoard board, INumberGenerator numberGenerator)
        {
            throw new NotImplementedException();
        }

        public IPlayer CreatePlayer()
        {
            return players.Dequeue();
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
