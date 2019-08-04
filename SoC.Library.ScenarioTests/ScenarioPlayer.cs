using System;
using System.Collections.Generic;
using Jabberwocky.SoC.Library;
using Jabberwocky.SoC.Library.DevelopmentCards;

namespace SoC.Library.ScenarioTests
{
    internal class ScenarioPlayer : Player
    {
        internal ScenarioPlayer(string name, Guid id) : base(name, id) {}

        internal void SetHeldCard(DevelopmentCardTypes developmentCardType)
        {
            if (this.HeldCards == null)
                this.HeldCards = new List<DevelopmentCard>();
        }

        internal void SetVictoryPoints(uint victoryPoints) => this.VictoryPoints = victoryPoints;
        internal void SetPlacedRoadSegments(int placedRoadSegments) => this.PlacedRoadSegments = placedRoadSegments;
        internal void SetPlacedSettlements(int placedSettlements) => this.PlacedSettlements = placedSettlements;
        internal void SetKnightCard(int cardCount)
        {
            while (cardCount-- > 0)
                this.HeldCards.Add(new KnightDevelopmentCard());
        }

        internal void SetRoadBuildingCard(int cardCount)
        {
            while (cardCount-- > 0)
                this.HeldCards.Add(new RoadBuildingDevelopmentCard());
        }
    }
}
