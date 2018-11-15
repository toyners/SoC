
namespace Jabberwocky.SoC.Library.Store
{
    using System;
    using System.Collections.Generic;
    using Jabberwocky.SoC.Library.Interfaces;

    public class PlayerModel
    {
        public int CitiesBuilt;
        public List<DevelopmentCard> HeldCards;
        public Guid Id;
        public bool IsComputer;
        public uint KnightCards;
        public string Name;
        public List<DevelopmentCard> PlayedCards;
        public ResourceClutch Resources;
        public int RoadSegmentsBuilt;
        public int SettlementsBuilt;
        public uint VictoryPoints; // TODO: This should derived from the other properties

        public PlayerModel() { }

        public PlayerModel(IPlayer player)
        {
            this.CitiesBuilt = player.CitiesBuilt;
            this.HeldCards = player.HeldCards;
            this.Id = player.Id;
            this.IsComputer = player.IsComputer;
            this.KnightCards = player.KnightCards;
            this.Name = player.Name;
            this.PlayedCards = player.PlayedCards;
            this.Resources = player.Resources;
            this.RoadSegmentsBuilt = player.RoadSegmentsBuilt;
            this.SettlementsBuilt = player.SettlementsBuilt;
            this.VictoryPoints = player.VictoryPoints;
        }

        public static IPlayer CreatePlayer(PlayerModel playerModel)
        {
            if (playerModel.IsComputer)
                return new ComputerPlayer(playerModel);

            return new Player(playerModel);
        }
    }
}
