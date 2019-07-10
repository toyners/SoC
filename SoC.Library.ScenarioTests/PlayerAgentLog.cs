
namespace SoC.Library.ScenarioTests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Jabberwocky.SoC.Library.GameEvents;

    public class PlayerAgentLog : IPlayerAgentLog
    {
        private readonly List<ILogEvent> gameEvents = new List<ILogEvent>();

        public void AddMatchedEvent(GameEvent actualEvent, GameEvent expectedEvent) =>
            this.gameEvents.Add(new MatchedExpectedEvent(actualEvent, expectedEvent));

        public void AddActualEvent(GameEvent actualEvent) =>
            this.gameEvents.Add(new UnmatchActualEvent(actualEvent));

        public void AddUnmatchedExpectedEvent(GameEvent expectedEvent) =>
            this.gameEvents.Add(new UnmatchedExpectedEvent(expectedEvent));

        public void AddException(Exception exception) =>
            this.gameEvents.Add(new ExceptionEvent(exception));

        public void AddNote(string note) =>
            this.gameEvents.Add(new NoteEvent(note));

        public void WriteToFile(string filePath)
        {
            var content = "";
            File.WriteAllText(filePath, content);
        }

        private interface ILogEvent
        {
            string ToHtmlRow();
        }

        private class UnmatchActualEvent : ILogEvent
        {
            private GameEvent actualEvent;
            public UnmatchActualEvent(GameEvent actualEvent) => this.actualEvent = actualEvent;

            public string ToHtmlRow()
            {
                return "";
            }
        }

        private class UnmatchedExpectedEvent : ILogEvent
        {
            private GameEvent expectedEvent;
            public UnmatchedExpectedEvent(GameEvent expectedEvent) => this.expectedEvent = expectedEvent;

            public string ToHtmlRow()
            {
                return "";
            }
        }

        private class MatchedExpectedEvent : ILogEvent
        {
            private GameEvent actualEvent, expectedEvent;
            public MatchedExpectedEvent(GameEvent actualEvent, GameEvent expectedEvent)
            {
                this.actualEvent = actualEvent;
                this.expectedEvent = expectedEvent;
            }

            public string ToHtmlRow()
            {
                return "";
            }
        }

        private class ExceptionEvent : ILogEvent
        {
            private Exception exception;
            public ExceptionEvent(Exception exception) => this.exception = exception;

            public string ToHtmlRow()
            {
                return "";
            }
        }

        private class NoteEvent : ILogEvent
        {
            private string note;
            public NoteEvent(string note) => this.note = note;

            public string ToHtmlRow()
            {
                return "";
            }
        }
    }
}
