
namespace Jabberwocky.SoC.Library.Store
{
    using System;
    using System.Collections.Generic;
    using Jabberwocky.SoC.Library.GameBoards;

    public class GameBoardModel
    {
        public Dictionary<uint, Guid> Cities;
        public Tuple<ResourceTypes?, uint>[] Hexes;
        public Tuple<uint, uint, Guid>[] Roads;
        public Dictionary<uint, Guid> Settlements;

        public GameBoardModel() { } // For deserialization

        public GameBoardModel(GameBoard gameBoard)
        {
            this.Cities = gameBoard.GetCityData();
            this.Hexes = gameBoard.GetHexData();
            this.Roads = gameBoard.GetRoadData();
            this.Settlements = gameBoard.GetSettlementData();
        }
    }
}
