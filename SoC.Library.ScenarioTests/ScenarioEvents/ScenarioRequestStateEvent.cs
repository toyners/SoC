
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

        public uint? Cities { get; internal set; }
        public ResourceClutch? Resources { get; internal set; }
        public uint? RoadSegments { get; internal set; }
        public uint? VictoryPoints { get; internal set; }
        public uint? Settlements { get; internal set; }
    }
}