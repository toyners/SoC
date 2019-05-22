
namespace SoC.Library.ScenarioTests.Instructions
{
    using System;
    using Jabberwocky.SoC.Library;
    using Jabberwocky.SoC.Library.GameEvents;

    internal class ScenarioRequestStateEvent : GameEvent
    {
        public ScenarioRequestStateEvent(Guid playerId) : base(playerId)
        {
        }

        public ResourceClutch? Resources { get; set; }
        public uint? RoadSegments { get; set; }
        public uint? VictoryPoints { get; set; }
    }
}