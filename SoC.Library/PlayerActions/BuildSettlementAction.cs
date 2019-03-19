﻿
namespace Jabberwocky.SoC.Library.PlayerActions
{
    using System;

    public class BuildSettlementAction : PlayerAction
    {
        public readonly uint SettlementLocation;

        public BuildSettlementAction(uint settlementLocation) : base(Guid.Empty)
        {
            this.SettlementLocation = settlementLocation;
        }
    }
}