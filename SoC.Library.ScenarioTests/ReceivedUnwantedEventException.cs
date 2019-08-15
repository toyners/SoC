
namespace SoC.Library.ScenarioTests
{
    using System;
    using System.Runtime.Serialization;
    using Jabberwocky.SoC.Library.GameEvents;

    public class ReceivedUnwantedEventException : Exception
    {
        public ReceivedUnwantedEventException() : base() { }

        public ReceivedUnwantedEventException(string message, GameEvent gameEvent) : base(message) { this.UnwantedEvent = gameEvent; }

        public ReceivedUnwantedEventException(string message, GameEvent gameEvent, Exception innerException) : base(message, innerException) { this.UnwantedEvent = gameEvent; }

        protected ReceivedUnwantedEventException(GameEvent gameEvent, SerializationInfo info, StreamingContext context) : base(info, context) { this.UnwantedEvent = gameEvent; }

        public GameEvent UnwantedEvent { get; private set; }
    }
}
