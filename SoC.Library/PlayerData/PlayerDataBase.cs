
namespace Jabberwocky.SoC.Library.PlayerData
{
    using System;
    using System.Collections.Generic;
    using Jabberwocky.SoC.Library.Interfaces;

    public class PlayerDataBase
    {
        public List<DevelopmentCard> PlayedDevelopmentCards;
        public bool HasLongestRoad;
        public Guid Id;
        public bool IsComputer;
        public string Name;

        public PlayerDataBase(IPlayer player)
        {
            this.Name = player.Name;
            this.Id = player.Id;
            this.IsComputer = player.IsComputer;
            this.HasLongestRoad = player.HasLongestRoad;
            // TODO: Played development cards
        }
    }
}
