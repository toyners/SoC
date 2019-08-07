
namespace SoC.Library.ScenarioTests
{
    using System;
    using System.Runtime.Serialization;

    public class ReceivedUnwantedEventException : Exception
    {
        public ReceivedUnwantedEventException() : base() { }

        public ReceivedUnwantedEventException(string message) : base(message) { }

        public ReceivedUnwantedEventException(string message, Exception innerException) : base(message, innerException) { }

        protected ReceivedUnwantedEventException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
