﻿
namespace Jabberwocky.SoC.Library.PlayerActions
{
    using System;

    public class BuildStartingInfrastructure : PlayerAction
    {
        public readonly uint SettlementLocation, RoadEnd;
        public BuildStartingInfrastructure(uint settlementLocation, uint roadEnd) : base(Guid.Empty)
        {
            this.SettlementLocation = settlementLocation;
            this.RoadEnd = roadEnd;
        }
    }
}
