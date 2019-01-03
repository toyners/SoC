using System;
using System.Collections.Generic;
using System.Linq;
using Jabberwocky.SoC.Library;
using Jabberwocky.SoC.Library.DevelopmentCards;
using Jabberwocky.SoC.Library.Enums;
using Jabberwocky.SoC.Library.GameActions;
using Jabberwocky.SoC.Library.Interfaces;

namespace SoC.Library.ScenarioTests
{
    internal class PlayerTurn //: IPlayerTurn
    {
        private readonly IPlayer player;
        private readonly LocalGameControllerScenarioRunner runner;
        protected readonly Queue<ComputerPlayerAction> actions = new Queue<ComputerPlayerAction>();

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

        public virtual PlayerTurn BuildCity(uint cityLocation)
        {
            this.actions.Enqueue(new BuildCityAction(cityLocation));
            return this;
        }

        public virtual PlayerTurn BuildRoad(uint roadSegmentStart, uint roadSegmentEnd)
        {
            this.actions.Enqueue(new BuildRoadSegmentAction(roadSegmentStart, roadSegmentEnd));
            return this;
        }

        public virtual PlayerTurn BuildSettlement(uint settlementLocation)
        {
            this.actions.Enqueue(new BuildSettlementAction(settlementLocation));
            return this;
        }

        public virtual PlayerTurn BuyDevelopmentCard(DevelopmentCardTypes developmentCardType)
        {
            this.AddDevelopmentCard(this.PlayerId, developmentCardType);
            this.actions.Enqueue(new ComputerPlayerAction(ComputerPlayerActionTypes.BuyDevelopmentCard));
            return this;
        }

        public virtual PlayerTurn PlayKnightCard(uint hexLocation)
        {
            this.actions.Enqueue(new PlayKnightCardAction(hexLocation));
            return this;
        }

        public virtual PlayerTurn PlayKnightCardAndCollectFrom(uint hexLocation, string selectedPlayerName, ResourceClutch expectedGainedResources)
        {
            return this;
        }

        public virtual void ResolveActions(TurnToken turnToken, LocalGameController localGameController)
        {
            while (this.actions.Count > 0)
            {
                var action = this.actions.Dequeue();
                switch(action.ActionType)
                {
                    case ComputerPlayerActionTypes.BuyDevelopmentCard: localGameController.BuyDevelopmentCard(turnToken); break;
                    case ComputerPlayerActionTypes.PlayKnightCard:
                    {
                        var knightCard = (KnightDevelopmentCard)this.player.HeldCards.Where(c => c.Type == DevelopmentCardTypes.Knight).First();
                        localGameController.UseKnightCard(turnToken, knightCard, ((PlayKnightCardAction)action).NewRobberHex);
                        break;
                    }
                    default: throw new Exception($"Action type {action.ActionType} not handled");
                }
            }
        }

        protected void AddDevelopmentCard(Guid playerId, DevelopmentCardTypes developmentCardType)
        {
            this.runner.AddDevelopmentCardToBuy(playerId, developmentCardType);
        }
    }
}
