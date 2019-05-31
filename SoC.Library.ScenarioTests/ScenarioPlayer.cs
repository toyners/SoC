using System;
using Jabberwocky.SoC.Library;

namespace SoC.Library.ScenarioTests
{
    internal class ScenarioPlayer : Player
    {
        internal ScenarioPlayer(string name, Guid id) : base(name, id) {}

        internal void SetVictoryPoints(uint victoryPoints) => this.VictoryPoints = victoryPoints;
        internal void SetPlacedRoadSegments(int placedRoadSegments) => this.PlacedRoadSegments = placedRoadSegments;

        internal void SetPlacedSettlements(int placedSettlements)
        {
            throw new NotImplementedException();
        }
    }
}
