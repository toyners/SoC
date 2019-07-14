
namespace SoC.Library.ScenarioTests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Jabberwocky.SoC.Library.GameEvents;
    using Jabberwocky.SoC.Library.PlayerActions;

    public class PlayerAgentLog : IPlayerAgentLog
    {
        private readonly List<ILogEvent> logEvents = new List<ILogEvent>();
        private const string titleFontSize = "15";
        private const string propertiesFontSize = "12";

        public static IDictionary<Guid, string> PlayerNamesById { get; set; }

        public void AddMatchedEvent(GameEvent actualEvent, GameEvent expectedEvent) =>
            this.logEvents.Add(new MatchedExpectedEvent(actualEvent, expectedEvent));

        public void AddActualEvent(GameEvent actualEvent) =>
            this.logEvents.Add(new ActualEvent(actualEvent));

        public void AddUnmatchedExpectedEvent(GameEvent expectedEvent) =>
            this.logEvents.Add(new ExpectedEvent(expectedEvent));

        public void AddException(Exception exception) =>
            this.logEvents.Add(new ExceptionEvent(exception));

        public void AddNote(string note) =>
            this.logEvents.Add(new NoteEvent(note));

        public void AddAction(PlayerAction action) =>
            this.logEvents.Add(new ActionEvent(action));

        public void WriteToFile(string filePath)
        {
            var content = "<html><head><title>Report</title>" +
                "<style>" +
                "table {" +
                "width: 100%; " +
                "border-collapse: collapse;" +
                "} " +
                ".border { " +
                "border: 1px solid black; " +
                "} " +
                ".actual span { font-size: 16px; } " +
                ".actual div { font-size: 14px; } " +
                ".expected { background-color: orange; } " +
                ".expected span { font-size: 16px; } " +
                ".expected div { font-size: 14px; } " +
                ".matched { background-color: lightgreen; } " +
                ".matched span { font-size: 16px; } " +
                ".matched div { font-size: 14px; } " +
                ".note div { font-size: 16px; } " +
                ".note { background-color: cyan; } " +
                "</style></head><body><div><table>";
            
            foreach (var logEvent in this.logEvents.Where(l => !(l is ExceptionEvent)))
            {
                content += logEvent.ToHtml();
            }

            /*content += "</table></div><br /><div><table>" +
                "<tr><th>Actual</th><th>Expected</th><th>Notes</th></tr>";
            foreach (var logEvent in this.logEvents)
            {
                content += logEvent.ToHtml();
            }*/

            File.WriteAllText(filePath, content + "</table></div></body>");
        }

        private interface ILogEvent
        {
            string ToHtml();
        }

        private class ActionEvent : ILogEvent
        {
            private PlayerAction playerAction;
            public ActionEvent(PlayerAction playerAction) => this.playerAction = playerAction;

            public string ToHtml()
            {
                return "Not comleted";
            }
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
                return $"<tr class=\"matched\">" +
                    $"<td class=\"border\">" +
                    $"<img src=\"..\\green_tick.png\" alt=\"\" height=\"20\" width=\"20\">" +
                    $"<span>{this.actualEvent.SimpleTypeName}</span><br>" +
                    $"<div>{GetEventProperties(this.actualEvent)}</div>" +
                    $"</td>" +
                    $"<td class=\"border\">" +
                    $"<span>{this.expectedEvent.SimpleTypeName}</span><br>" +
                    $"<div>{GetEventProperties(this.actualEvent)}</div></td></tr>";
            }
        }

        private class NoteEvent : ILogEvent
        {
            private string note;
            public NoteEvent(string note) => this.note = note;
        
            public string ToHtml()
            {
                return $"<tr class=\"note\"><td class=\"border\"><div>{this.note}</div></td></tr>";
            }
        }

        private class ActualEvent : ILogEvent
        {
            private GameEvent actualEvent;
            public ActualEvent(GameEvent actualEvent) => this.actualEvent = actualEvent;

            public string ToHtml()
            {
                return $"<tr><td class=\"actual border\"><span>{this.actualEvent.SimpleTypeName}</span><br><div>{GetEventProperties(this.actualEvent)}</div></td></tr>";
            }
        }

        private class ExpectedEvent : ILogEvent
        {
            private GameEvent expectedEvent;
            public ExpectedEvent(GameEvent expectedEvent) => this.expectedEvent = expectedEvent;

            public string ToHtml()
            {
                return $"<tr class=\"expected\">" +
                    $"<td class=\"border\">" +
                    $"<img src=\"..\\red_cross.png\" alt=\"\" height=\"20\" width=\"20\">" +
                    $"<span>{this.expectedEvent.SimpleTypeName}</span><br><div>{GetEventProperties(this.expectedEvent)}</div></td></tr>";
            }
        }

        private static string GetEventProperties(GameEvent gameEvent)
        {
            var result = "";
            if (gameEvent is GameErrorEvent gameErrorEvent)
            {
                result += $"Error code: <b>{gameErrorEvent.ErrorCode}</b><br>" +
                    $"Error message: <b>{gameErrorEvent.ErrorMessage}</b>";
            }
            else if (gameEvent is InfrastructurePlacedEvent infrastructurePlacedEvent)
            {
                result += $"Settlement Location: <b>{infrastructurePlacedEvent.SettlementLocation}</b><br>" +
                    $"Road segment end Location: <b>{infrastructurePlacedEvent.RoadSegmentEndLocation}</b>";
            }
            else if (gameEvent is PlayerSetupEvent playerSetupEvent)
            {
                foreach (var kv in playerSetupEvent.PlayerIdsByName)
                    result += $"Name <b>{kv.Key}</b> Id <b>{kv.Value}</b><br>";
            }

            return $"Player: <b>{GetPlayerName(gameEvent.PlayerId)}</b>" + (result.Length > 0 ? "<br>" : "") + result;
        }

        private static string GetPlayerName(Guid playerId)
        {
            if (PlayerNamesById == null || !PlayerNamesById.ContainsKey(playerId))
                return playerId.ToString();

            return PlayerNamesById[playerId];
        }
    }
}
