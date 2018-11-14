
namespace Jabberwocky.SoC.Library.Storage
{
    using System;
    using Jabberwocky.SoC.Library.Interfaces;

    public class PlayerSaveObject
    {
        public int CitiesBuilt;
        public Guid Id;
        public bool IsComputer;
        public uint KnightCards;
        public string Name;
        public ResourceClutch Resources;
        public int RoadSegmentsBuilt;
        public int SettlementsBuilt;
        public uint VictoryPoints;

        public PlayerSaveObject() { }

        public PlayerSaveObject(IPlayer player)
        {
            this.CitiesBuilt = player.CitiesBuilt;
            this.Id = player.Id;
            this.IsComputer = player.IsComputer;
            this.KnightCards = player.KnightCards;
            this.Name = player.Name;
            this.Resources = player.Resources;
            this.RoadSegmentsBuilt = player.RoadSegmentsBuilt;
            this.SettlementsBuilt = player.SettlementsBuilt;
            this.VictoryPoints = player.VictoryPoints;
        }

        public static IPlayer CreatePlayer(PlayerSaveObject playerSaveObject)
        {
            if (playerSaveObject.IsComputer)
                return new ComputerPlayer(playerSaveObject);

            return new Player(playerSaveObject);
        }
    }
}
