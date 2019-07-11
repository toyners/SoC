
namespace SoC.Library.ScenarioTests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Jabberwocky.SoC.Library.GameEvents;

    public class PlayerAgentLog : IPlayerAgentLog
    {
        private readonly List<ILogEvent> logEvents = new List<ILogEvent>();

        public static IDictionary<Guid, string> PlayerNamesById { get; set; }

        public void AddMatchedEvent(GameEvent actualEvent, GameEvent expectedEvent) =>
            this.logEvents.Add(new MatchedExpectedEvent(actualEvent, expectedEvent));

        public void AddActualEvent(GameEvent actualEvent) =>
            this.logEvents.Add(new UnmatchedActualEvent(actualEvent));

        public void AddUnmatchedExpectedEvent(GameEvent expectedEvent) =>
            this.logEvents.Add(new UnmatchedExpectedEvent(expectedEvent));

        public void AddException(Exception exception) =>
            this.logEvents.Add(new ExceptionEvent(exception));

        public void AddNote(string note) =>
            this.logEvents.Add(new NoteEvent(note));

        public void WriteToFile(string filePath)
        {
            var content = "<html><head><title>Report</title>" +
                "<style>" +
                "table {" +
                "border: 1px solid black;" +
                "border-collapse: collapse;" +
                "} " +
                "td {" +
                "border: 1px solid black } " +
                ".matched { background-color: lightgreen; } " +
                ".unmatched_expected { background-color: orange; } " +
                "</style></head><body><div><table>" +
                "<tr><th>Actual</th><th>Expected</th></tr>";
            
            foreach (var logEvent in this.logEvents.Where(l => !(l is NoteEvent || l is ExceptionEvent)))
            {
                content += logEvent.ToHtml();
            }

            content += "</table></div><br /><div><table>" +
                "<tr><th>Actual</th><th>Expected</th><th>Notes</th></tr>";
            foreach (var logEvent in this.logEvents)
            {
                content += logEvent.ToHtml();
            }

            File.WriteAllText(filePath, content + "</table></div></body>");
        }

        private interface ILogEvent
        {
            string ToHtml();
        }

        private class ExceptionEvent : ILogEvent
        {
            private Exception exception;
            public ExceptionEvent(Exception exception) => this.exception = exception;

            public string ToHtml()
            {
                return $"<tr><td colspan=\"2\">{this.exception.Message}</td></tr>";
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

            public string ToHtml()
            {
                return $"<tr class=\"matched\"><td>{this.actualEvent.SimpleTypeName}</td><td>{this.expectedEvent.SimpleTypeName}</td></tr>";
            }
        }

        private class NoteEvent : ILogEvent
        {
            private string note;
            public NoteEvent(string note) => this.note = note;
        
            public string ToHtml()
            {
                return $"<tr><td /><td /><td>{this.note}</td></tr>";
            }
        }

        private class UnmatchedActualEvent : ILogEvent
        {
            private GameEvent actualEvent;
            public UnmatchedActualEvent(GameEvent actualEvent) => this.actualEvent = actualEvent;

            public string ToHtml()
            {
                return $"<tr><td>{this.actualEvent.SimpleTypeName}<br>{GetEventProperties(this.actualEvent)}</td><td></td></tr>";
            }
        }

        private class UnmatchedExpectedEvent : ILogEvent
        {
            private GameEvent expectedEvent;
            public UnmatchedExpectedEvent(GameEvent expectedEvent) => this.expectedEvent = expectedEvent;

            public string ToHtml()
            {
                return $"<tr class=\"unmatched_expected\">" +
                    $"<td></td>" +
                    $"<td>{this.expectedEvent.SimpleTypeName}<br>{GetEventProperties(this.expectedEvent)}</td></tr>";
            }
        }

        private static string GetEventProperties(GameEvent gameEvent)
        {
            if (gameEvent is GameErrorEvent gameErrorEvent)
            {
                return $"Error code: <b>{gameErrorEvent.ErrorCode}</b><br>" +
                    $"Error message: <b>{gameErrorEvent.ErrorMessage}</b>";
            }

            return "Not recognised";
        }

        private static string GetPlayerName(Guid playerId)
        {
            if (PlayerNamesById == null)
                return playerId.ToString();

            return PlayerNamesById[playerId];
        }
    }
}
