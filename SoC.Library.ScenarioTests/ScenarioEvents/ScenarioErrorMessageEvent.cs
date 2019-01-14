
using System;
using Jabberwocky.SoC.Library.GameEvents;

namespace SoC.Library.ScenarioTests.ScenarioEvents
{
    internal class ScenarioErrorMessageEvent : GameEvent
    {
        public readonly string ExpectedErrorMessage;
        public ScenarioErrorMessageEvent(string expectedErrorMessage) : base(Guid.Empty)
        {
            this.ExpectedErrorMessage = expectedErrorMessage;
        }

        public override bool Equals(object obj)
        {
            if (!base.Equals(obj))
                return false;

            return this.ExpectedErrorMessage.Equals(((ScenarioErrorMessageEvent)obj).ExpectedErrorMessage);
        }
    }
}
