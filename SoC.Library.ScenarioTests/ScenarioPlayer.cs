using System;
using Jabberwocky.SoC.Library;
using Jabberwocky.SoC.Library.DevelopmentCards;

namespace SoC.Library.ScenarioTests
{
    internal class ScenarioPlayer : Player
    {
        internal ScenarioPlayer(string name, Guid id) : base(name, id) {}

        internal void SetHeldCard(int cardCount, DevelopmentCardTypes developmentCardType)
        {
            while (cardCount-- > 0)
            {
                switch (developmentCardType)
                {
                    case DevelopmentCardTypes.Knight: this.HeldCards.Add(new KnightDevelopmentCard()); break;
                    case DevelopmentCardTypes.Monopoly: this.HeldCards.Add(new MonopolyDevelopmentCard()); break;
                    case DevelopmentCardTypes.RoadBuilding: this.HeldCards.Add(new RoadBuildingDevelopmentCard()); break;
                    case DevelopmentCardTypes.YearOfPlenty: this.HeldCards.Add(new YearOfPlentyDevelopmentCard()); break;
                    default: throw new ArgumentException($"{developmentCardType} type not recognised", "developmentCardType");
                }
            }
        }
        internal void SetVictoryPoints(uint victoryPoints) => this.VictoryPoints = victoryPoints;
        internal void SetPlacedRoadSegments(int placedRoadSegments) => this.PlacedRoadSegments = placedRoadSegments;
        internal void SetPlacedSettlements(int placedSettlements) => this.PlacedSettlements = placedSettlements;
        
    }
}
