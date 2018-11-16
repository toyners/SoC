
namespace Jabberwocky.SoC.Library.GameBoards
{
    using System;
    using System.Collections.Generic;

    public interface IGameBoard
    {
        IBoardQueryEngine BoardQuery { get; }

        Dictionary<uint, Guid> GetCityData();

        Tuple<ResourceTypes?, uint>[] GetHexData();

        Tuple<uint, uint, Guid>[] GetRoadData();

        Dictionary<uint, Guid> GetSettlementData();

    }
}
