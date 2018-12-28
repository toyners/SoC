using System;
using Jabberwocky.SoC.Library;
using Jabberwocky.SoC.Library.Interfaces;

namespace SoC.Library.ScenarioTests
{
    internal class PlayerTurn
    {
        private readonly IPlayer player;
        private readonly LocalGameControllerScenarioRunner runner;

        public Guid PlayerId { get { return this.player.Id; } }

        public PlayerTurn(LocalGameControllerScenarioRunner runner, IPlayer player)
        {
            this.runner = runner;
            this.player = player;
        }

        public LocalGameControllerScenarioRunner EndTurn()
        {
            return this.runner;
        }

        public virtual PlayerTurn BuildRoad(uint roadSegmentStart, uint roadSegmentEnd)
        {
            throw new NotImplementedException();
        }

        public virtual PlayerTurn BuildSettlement(uint settlementLocation)
        {
            throw new NotImplementedException();
        }

        internal virtual PlayerTurn BuildCity(uint cityLocation)
        {
            throw new NotImplementedException();
        }

        public virtual PlayerTurn BuyDevelopmentCard(DevelopmentCardTypes developmentCardType)
        {
            throw new NotImplementedException();
        }

        protected void AddDevelopmentCard(Guid playerId, DevelopmentCardTypes developmentCardType)
        {
            this.runner.AddDevelopmentCardToBuy(playerId, developmentCardType);
        }

        internal PlayerTurn PlayKnightCard()
        {
            throw new NotImplementedException();
        }
    }
}
