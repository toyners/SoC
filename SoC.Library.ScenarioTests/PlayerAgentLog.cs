
namespace SoC.Library.ScenarioTests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Jabberwocky.SoC.Library;
    using Jabberwocky.SoC.Library.GameEvents;
    using SoC.Library.ScenarioTests.Instructions;
    using SoC.Library.ScenarioTests.ScenarioEvents;

    public class PlayerAgentLog : IPlayerAgentLog
    {
        private readonly List<ILogEvent> logEvents = new List<ILogEvent>();
        private const string TitleFontSize = "16";
        private const string PropertiesFontSize = "14";

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

        public void AddAction(ActionInstruction action) =>
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
                ".action { background-color: #5C6AFA; } " +
                $".action span {{ font-size: {TitleFontSize}px; }} " + 
                $".actual span {{ font-size: {TitleFontSize}px; }} " +
                $".actual div {{ font-size: {PropertiesFontSize}px; }} " +
                ".expected { background-color: orange; } " +
                $".expected span {{ font-size: {TitleFontSize}px; }} " +
                $".expected div {{ font-size: {PropertiesFontSize}px; }} " +
                ".matched { background-color: lightgreen; } " +
                $".matched span {{ font-size: {TitleFontSize}px; }} " +
                $".matched div {{ font-size: {PropertiesFontSize}px; }} " +
                $".note div {{ font-size: {TitleFontSize}px; }} " +
                ".note { background-color: cyan; } " +
                "</style></head><body><div><table>";
            
            foreach (var logEvent in this.logEvents.Where(l => !(l is ExceptionEvent)))
            {
                content += logEvent.ToHtml();
            }

            File.WriteAllText(filePath, content + "</table></div></body>");
        }

        private interface ILogEvent
        {
            string ToHtml();
        }

        private class ActionEvent : ILogEvent
        {
            private ActionInstruction playerAction;
            public ActionEvent(ActionInstruction playerAction) => this.playerAction = playerAction;

            public string ToHtml()
            {
                return $"<tr>" +
                    "<td /><td class=\"action border\">" +
                    $"<span>{this.playerAction.Operation} operation</span>" +
                    $"</td></tr>";
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
                    $"<img src=\"green_tick.png\" alt=\"\" height=\"20\" width=\"20\">" +
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
                return $"<tr><td />" +
                    $"<td class=\"expected border\">" +
                    $"<img src=\"red_cross.png\" alt=\"\" height=\"20\" width=\"20\">" +
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
            else if (gameEvent is GameWinEvent gameWinEvent)
            {
                result += $"Victory Points <b>{gameWinEvent.VictoryPoints}</b>";
            }
            else if (gameEvent is InfrastructurePlacedEvent infrastructurePlacedEvent)
            {
                result += $"Settlement Location: <b>{infrastructurePlacedEvent.SettlementLocation}</b><br>" +
                    $"Road segment end Location: <b>{infrastructurePlacedEvent.RoadSegmentEndLocation}</b>";
            }
            else if (gameEvent is LargestArmyChangedEvent largestArmyChangedEvent)
            {
                result += $"Previous Player: <b>{(largestArmyChangedEvent.PreviousPlayerId != null ? GetPlayerName(largestArmyChangedEvent.PreviousPlayerId.Value) : "[none]")}</b>";
            }
            else if (gameEvent is PlayerSetupEvent playerSetupEvent)
            {
                foreach (var kv in playerSetupEvent.PlayerIdsByName)
                    result += $"Name <b>{kv.Key}</b> Id <b>{kv.Value}</b><br>";
            }
            else if (gameEvent is RequestStateEvent requestStateEvent)
            {
                result += $"Cities <b>{requestStateEvent.Cities}</b><br>" +
                    $"Held Cards {GetFormattedCards(requestStateEvent.DevelopmentCardsByCount)}<br>" +
                    $"Played Knight Cards <b>{requestStateEvent.PlayedKnightCards}</b><br>" +
                    $"Resources <b>{requestStateEvent.Resources}</b><br>" +
                    $"Road Segments <b>{requestStateEvent.RoadSegments}</b><br>" +
                    $"Settlements <b>{requestStateEvent.Settlements}</b><br>" +
                    $"Victory Points <b>{requestStateEvent.VictoryPoints}</b>";
            }
            else if (gameEvent is ResourcesCollectedEvent resourcesCollectedEvent)
            {
                foreach (var kv in resourcesCollectedEvent.ResourcesCollectedByPlayerId)
                {
                    result += $"Name <b>{GetPlayerName(kv.Key)}</b><br>";
                    foreach (var kv2 in kv.Value)
                        result += $"Location <b>{kv2.Location}</b> Resources <b>{kv2.Resources}</b><br>";
                }
            }
            else if (gameEvent is RobbingChoicesEvent robbingChoicesEvent)
            {
                foreach (var kv in robbingChoicesEvent.RobbingChoices)
                    result += $"Name <b>{GetPlayerName(kv.Key)}</b> Resource count <b>{kv.Value}</b><br>";
            }
            else if (gameEvent is ScenarioRequestStateEvent scenarioRequestStateEvent)
            {
                result += $"Cities {GetFormattedProperty(scenarioRequestStateEvent.Cities)}<br>" +
                    $"Held Cards {GetFormattedCards(scenarioRequestStateEvent.DevelopmentCardsByCount)}<br>" + 
                    $"Played Knight Cards {GetFormattedProperty(scenarioRequestStateEvent.PlayedKnightCards)}<br>" +
                    $"Resources {GetFormattedProperty(scenarioRequestStateEvent.Resources)}<br>" +
                    $"Road Segments {GetFormattedProperty(scenarioRequestStateEvent.RoadSegments)}<br>" +
                    $"Settlements {GetFormattedProperty(scenarioRequestStateEvent.Settlements)}<br>" +
                    $"Victory Points {GetFormattedProperty(scenarioRequestStateEvent.VictoryPoints)}";
            }

            return $"Player: <b>{GetPlayerName(gameEvent.PlayerId)}</b>" + (result.Length > 0 ? "<br>" : "") + result;
        }

        private static string GetFormattedCards(Dictionary<DevelopmentCardTypes, int> developmentCardsByCount)
        {
            if (developmentCardsByCount == null)
                return "[not set]";

            if (developmentCardsByCount.Count == 0)
                return "<b>0 cards</b>";

            var result = "";
            var sortedKeys = new List<DevelopmentCardTypes>(developmentCardsByCount.Keys);
            sortedKeys.Sort();
            var totalCards = 0;
            foreach (var sortedKey in sortedKeys)
            {
                totalCards += developmentCardsByCount[sortedKey];
                result += $"<b>{sortedKey} = {developmentCardsByCount[sortedKey]},</b> ";
            }

            if (totalCards == 0)
                return "<b>0 cards</b>";
            return result.Substring(0, result.Length - ",</b> ".Length) + "</b>";
        }

        private static string GetFormattedProperty(uint? value) => value.HasValue ? "<b>" + value.ToString() + "</b>" : "[not set]";
        private static string GetFormattedProperty(int? value) => value.HasValue ? "<b>" + value.ToString() + "</b>" : "[not set]";
        private static string GetFormattedProperty(ResourceClutch? value) => value.HasValue ? "<b>" + value.ToString() + "</b>" : "[not set]";

        private static string GetPlayerName(Guid playerId)
        {
            if (PlayerNamesById == null || !PlayerNamesById.ContainsKey(playerId))
                return playerId.ToString();

            return PlayerNamesById[playerId];
        }
    }
}
