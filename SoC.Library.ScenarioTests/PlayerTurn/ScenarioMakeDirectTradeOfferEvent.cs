using System;
using System.Collections.Generic;
using Jabberwocky.SoC.Library;
using Jabberwocky.SoC.Library.GameEvents;
using SoC.Library.ScenarioTests.Instructions;

namespace SoC.Library.ScenarioTests.PlayerTurn
{
    internal class MakeDirectTradeOfferEventInstruction : EventInstruction
    {
        private readonly string buyingPlayerName;
        private readonly ResourceClutch wantedResources;

        public MakeDirectTradeOfferEventInstruction(string playerName, string buyingPlayerName, ResourceClutch wantedResources)
            : base(playerName)
        {
            this.buyingPlayerName = buyingPlayerName;
            this.wantedResources = wantedResources;
        }

        public override GameEvent GetEvent(IDictionary<string, Guid> playerIdsByName)
        {
            return new MakeDirectTradeOfferEvent(
                playerIdsByName[this.buyingPlayerName], 
                this.wantedResources);
        }
    }

    internal class AnswerDirectTradeOfferEventInstruction : EventInstruction
    {
        private readonly string buyingPlayerName;
        private readonly ResourceClutch wantedResources;

        public AnswerDirectTradeOfferEventInstruction(string playerName, string buyingPlayerName, ResourceClutch wantedResources)
            : base(playerName)
        {
            this.buyingPlayerName = buyingPlayerName;
            this.wantedResources = wantedResources;
        }

        public override GameEvent GetEvent(IDictionary<string, Guid> playerIdsByName)
        {
            return new AnswerDirectTradeOfferEvent(
                playerIdsByName[this.buyingPlayerName],
                this.wantedResources);
        }
    }
}