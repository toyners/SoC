
namespace Jabberwocky.SoC.Library.GameBoards
{
    using System;
    using System.Collections.Generic;

    public class GameBoardSetup
    {
        public readonly Tuple<ResourceTypes?, uint>[] HexData;
        public readonly Dictionary<uint, Guid> SettlementData;
        public readonly Tuple<uint, uint, Guid>[] RoadSegmentData;
        public readonly Dictionary<uint, Guid> CityData;

        public GameBoardSetup(IGameBoard board)
        {
            this.HexData = board.GetHexData();
            this.SettlementData = board.GetSettlementData();
            this.RoadSegmentData = board.GetRoadData();
            this.CityData = board.GetCityData();
        }
    }
}
