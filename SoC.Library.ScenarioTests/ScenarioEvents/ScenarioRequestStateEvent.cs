
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

        /*public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != typeof(RequestStateEvent) || this.PlayerId != ((GameEvent)obj).PlayerId)
                return false;

            var other = (RequestStateEvent)obj;
            if (this.Resources.HasValue)
            {
                if (!this.Resources.Value.Equals(other.Resources))
                    return false;
            }

            return true;
        }*/
    }
}