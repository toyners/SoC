﻿using System;
using System.Collections.Generic;
using Jabberwocky.SoC.Library;
using Jabberwocky.SoC.Library.GameActions;
using Jabberwocky.SoC.Library.Interfaces;

namespace SoC.Library.ScenarioTests
{
    internal class ComputerPlayerTurn : PlayerTurn
    {
        private MockComputerPlayer computerPlayer;
        private IList<ComputerPlayerAction> actions = new List<ComputerPlayerAction>();

        public ComputerPlayerTurn(LocalGameControllerScenarioRunner runner, IPlayer player) : base(runner, player)
        {
            this.computerPlayer = (MockComputerPlayer)player;
        }

        public override PlayerTurn BuildCity(uint cityLocation)
        {
            this.actions.Add(new BuildCityAction(cityLocation));
            return this;
        }

        public override PlayerTurn BuildRoad(uint roadSegmentStart, uint roadSegmentEnd)
        {
            this.actions.Add(new BuildRoadSegmentAction(roadSegmentStart, roadSegmentEnd));
            return this;
        }

        public override PlayerTurn BuildSettlement(uint settlementLocation)
        {
            this.actions.Add(new BuildSettlementAction(settlementLocation));
            return this;
        }

        public override PlayerTurn BuyDevelopmentCard(DevelopmentCardTypes developmentCardType)
        {
            base.AddDevelopmentCard(this.PlayerId, developmentCardType);
            this.actions.Add(new ComputerPlayerAction(Jabberwocky.SoC.Library.Enums.ComputerPlayerActionTypes.BuyDevelopmentCard));
            return this;
        }

        public void ResolveActions()
        {
            this.computerPlayer.AddActions(this.actions);
        }

        public override PlayerTurn PlayKnightCard()
        {
            this.actions.Add(new ComputerPlayerAction(Jabberwocky.SoC.Library.Enums.ComputerPlayerActionTypes.PlayKnightCard));
            return this;
        }
    }
}