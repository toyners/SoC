
namespace Jabberwocky.SoC.Library.Storage
{
    using System;
    using System.Collections.Generic;

    public class SaveObject
    {
        public PlayerSaveObject Player1;
        public PlayerSaveObject Player2;
        public PlayerSaveObject Player3;
        public PlayerSaveObject Player4;
        public Tuple<ResourceTypes?, uint>[] Hexes;
        public Dictionary<uint, Guid> Settlements;
        public Tuple<uint, uint, Guid>[] Roads;
        public uint RobberLocation;
        public DevelopmentCard[] DevelopmentCards;
        public uint Dice1, Dice2;
    }
}
