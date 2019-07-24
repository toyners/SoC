
namespace Jabberwocky.SoC.Library.Store
{
    using System;
    using System.Collections.Generic;
    using Jabberwocky.SoC.Library.DevelopmentCards;
    using Jabberwocky.SoC.Library.Interfaces;

    public class PlayerModel
    {
        public int CitiesBuilt;
        public List<DevelopmentCard> HeldCards;
        public Guid Id;
        public bool IsComputer;
        public int KnightCards;
        public string Name;
        public List<DevelopmentCard> PlayedCards;
        public ResourceClutch Resources;
        public int RoadSegmentsBuilt;
        public int SettlementsBuilt;
        public uint VictoryPoints; // TODO: This should derived from the other properties

        public PlayerModel() { } // For deserialization

        public PlayerModel(IPlayer player)
        {
            this.CitiesBuilt = player.PlacedCities;
            this.HeldCards = player.HeldCards;
            this.Id = player.Id;
            this.IsComputer = player.IsComputer;
            this.KnightCards = player.PlayedKnightCards;
            this.Name = player.Name;
            this.PlayedCards = player.PlayedCards;
            this.Resources = player.Resources;
            this.RoadSegmentsBuilt = player.PlacedRoadSegments;
            this.SettlementsBuilt = player.PlacedSettlements;
            this.VictoryPoints = player.VictoryPoints;
        }
    }
}
