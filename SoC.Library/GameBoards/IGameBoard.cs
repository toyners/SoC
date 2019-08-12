
namespace Jabberwocky.SoC.Library.GameBoards
{
    using System;
    using System.Collections.Generic;

    public interface IGameBoard
    {
        IGameBoardQuery BoardQuery { get; }

        Dictionary<uint, Guid> GetCityData();

        Tuple<ResourceTypes?, uint>[] GetHexData();

        Tuple<uint, uint, Guid>[] GetRoadData();

        Dictionary<uint, Guid> GetSettlementData();

        PlacementStatusCodes TryPlaceRoadSegment(Guid playerId, uint roadSegmentStartLocation, uint roadSegmentEndLocation);
    }
}
