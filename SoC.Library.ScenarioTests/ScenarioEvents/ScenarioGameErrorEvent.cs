
using System;
using Jabberwocky.SoC.Library.GameEvents;

namespace SoC.Library.ScenarioTests.ScenarioEvents
{
    internal class ScenarioGameErrorEvent : GameEvent
    {
        public ScenarioGameErrorEvent(Guid? playerId, int errorCode, string errorMessage) : base(playerId.GetValueOrDefault())
        {
            this.Id = playerId;
            this.ErrorCode = errorCode;
            this.ErrorMessage = errorMessage;
        }

        public Guid? Id { get; private set; }
        public int ErrorCode { get; private set; }
        public string ErrorMessage { get; private set; }
    }
}
