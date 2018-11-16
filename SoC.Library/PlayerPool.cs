
namespace Jabberwocky.SoC.Library
{
    using System;
    using System.Xml;
    using Interfaces;
    using GameBoards;
    using System.Collections.Generic;
    using Jabberwocky.SoC.Library.Store;

    public class PlayerPool : IPlayerPool
    {
        private Guid bankId = Guid.NewGuid();
        private Queue<string> names = new Queue<string>(new[] { "Bob", "Carol", "Dana" });

        /// <summary>
        /// Create a computer player instance
        /// </summary>
        /// <param name="gameBoard">Game board instance for use in decision making.</param>
        /// <returns>Computer player instance.</returns>
        public IPlayer CreateComputerPlayer(GameBoard gameBoard, INumberGenerator numberGenerator)
        {
            return new ComputerPlayer(this.names.Dequeue(), gameBoard, numberGenerator, null);
        }

        public IPlayer CreateComputerPlayer(IGameDataSection<GameDataSectionKeys, GameDataValueKeys, ResourceTypes> data, GameBoard board, INumberGenerator numberGenerator)
        {
            return new ComputerPlayer(data, board, numberGenerator);
        }

        /// <summary>
        /// Create a player instance.
        /// </summary>
        /// <returns>Player instance</returns>
        public IPlayer CreatePlayer()
        {
            return new Player();
        }

        /// <summary>
        /// Create a player instance from XML reader.
        /// </summary>
        /// <param name="reader">Xml reader containing player data.</param>
        /// <returns>Player instance</returns>
        public IPlayer CreatePlayer(XmlReader reader)
        {
            var isComputer = false;
            var isComputerValue = reader.GetAttribute("iscomputer");
            if (!string.IsNullOrEmpty(isComputerValue) && bool.TryParse(isComputerValue, out isComputer) && isComputer)
            {
                return ComputerPlayer.CreateFromXML(reader);
            }

            var player = new Player();
            player.Load(reader);
            return player;
        }

        [Obsolete("Deprecated.")]
        public IPlayer CreatePlayer(IGameDataSection<GameDataSectionKeys, GameDataValueKeys, ResourceTypes> data)
        {
            return new Player(data);
        }

        /// <summary>
        /// Get the id of the bank
        /// </summary>
        /// <returns>Bank id</returns>
        public Guid GetBankId()
        {
            return this.bankId;
        }
    }
}
