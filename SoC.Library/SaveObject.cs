
namespace Jabberwocky.SoC.Library
{
    using System;
    using System.Collections.Generic;
    using Jabberwocky.SoC.Library.Interfaces;

    public class SaveObject
    {
        public PlayerSaveModel Player1;
        public PlayerSaveModel Player2;
        public PlayerSaveModel Player3;
        public PlayerSaveModel Player4;
        public Tuple<ResourceTypes?, uint>[] Hexes;
        public Dictionary<uint, Guid> Settlements;
        public Tuple<uint, uint, Guid>[] Roads;
        public uint RobberLocation;
        public DevelopmentCard[] DevelopmentCards;
        public uint Dice1, Dice2;
    }

    public class PlayerSaveModel
    {
        public int CitiesBuilt;
        public Guid Id;
        public string Name;
        public ResourceClutch Resources;
        public bool IsComputer;
        public uint KnightCards;
        public int RoadSegmentsBuilt;
        public int SettlementsBuilt;
        public uint VictoryPoints;

        public PlayerSaveModel() { }

        public PlayerSaveModel(IPlayer player)
        {
            this.Id = player.Id;
            this.Name = player.Name;
            this.Resources = player.Resources;
            this.IsComputer = player.IsComputer;
        }
    }
}
