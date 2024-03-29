﻿
namespace SoC.Library.ScenarioTests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Jabberwocky.SoC.Library;
    using Jabberwocky.SoC.Library.GameEvents;
    using SoC.Library.ScenarioTests.Instructions;
    using SoC.Library.ScenarioTests.ScenarioEvents;

    public class ScenarioPlayerAgentLog : IPlayerAgentLog
    {
        #region Fields
        private readonly List<ILogEvent> logEvents = new List<ILogEvent>();
        private const string TitleFontSize = "16";
        private const string PropertiesFontSize = "14";
        #endregion

        #region Methods
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

        public void AddReceivedUnwantedEventException(ReceivedUnwantedEventException r) =>
            this.logEvents.Add(new ReceivedUnwantedEventExceptionEvent(r));

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
                ".exception { background-color: red; } " +
                ".expected { background-color: orange; } " +
                $".expected span {{ font-size: {TitleFontSize}px; }} " +
                $".expected div {{ font-size: {PropertiesFontSize}px; }} " +
                ".matched { background-color: lightgreen; } " +
                $".matched span {{ font-size: {TitleFontSize}px; }} " +
                $".matched div {{ font-size: {PropertiesFontSize}px; }} " +
                $".note div {{ font-size: {TitleFontSize}px; }} " +
                ".note { background-color: cyan; } " +
                "</style></head><body><div><table>";
            
            for (var index = this.logEvents.Count - 1; index >= 0; index--)
                content += this.logEvents[index].ToHtml();

            File.WriteAllText(filePath, content + "</table></div></body>");
        }

        private static string GetEventProperties(GameEvent gameEvent)
        {
            var result = "";
            if (gameEvent is DevelopmentCardBoughtEvent developmentCardBoughtEvent && 
                developmentCardBoughtEvent.CardType.HasValue)
            {
                result += $"Card type: <b>{developmentCardBoughtEvent.CardType}</b>";
            }
            else if (gameEvent is GameErrorEvent gameErrorEvent)
            {
                result += $"Error code: <b>{gameErrorEvent.ErrorCode}</b><br>" +
                    $"Error message: <b>{gameErrorEvent.ErrorMessage}</b>";
            }
            else if (gameEvent is GameWinEvent gameWinEvent)
            {
                result += $"Victory Points <b>{gameWinEvent.VictoryPoints}</b>";
            }
            else if (gameEvent is SetupInfrastructurePlacedEvent infrastructurePlacedEvent)
            {
                result += $"Settlement Location: <b>{infrastructurePlacedEvent.SettlementLocation}</b><br>" +
                    $"Road segment end Location: <b>{infrastructurePlacedEvent.RoadSegmentEndLocation}</b>";
            }
            else if (gameEvent is KnightCardPlayedEvent knightCardPlayedEvent)
            {
                result += $"Hex Location: <b>{knightCardPlayedEvent.HexLocation}</b><br>";
            }
            else if (gameEvent is LargestArmyChangedEvent largestArmyChangedEvent && !(gameEvent is LongestRoadBuiltEvent))
            {
                result += $"Previous Player: <b>{(largestArmyChangedEvent.PreviousPlayerId != null ? GetPlayerName(largestArmyChangedEvent.PreviousPlayerId.Value) : "[none]")}</b>";
            }
            else if (gameEvent is LongestRoadBuiltEvent longestRoadBuiltEvent)
            {
                result += $"Previous Player: <b>{(longestRoadBuiltEvent.PreviousPlayerId != null ? GetPlayerName(longestRoadBuiltEvent.PreviousPlayerId.Value) : "[none]")}</b><br>" +
                    $"Road Location: <b>";
                foreach (var location in longestRoadBuiltEvent.Locations)
                    result += $"{location}, ";

                result = result.Substring(0, result.Length - 2) + "</b>";
            }
            else if (gameEvent is PlayMonopolyCardEvent playMonopolyCardEvent)
            {
                for (var i = 0; i < playMonopolyCardEvent.ResourceTransactionList.Count; i++)
                {
                    var resourceTransaction = playMonopolyCardEvent.ResourceTransactionList[i];
                    result += $"<b>{PlayerNamesById[resourceTransaction.GivingPlayerId]}</b> " +
                        $"gave <b>{resourceTransaction.Resources}</b> " +
                        $"to <b>{PlayerNamesById[resourceTransaction.ReceivingPlayerId]}</b><br />";
                }
            }
            else if (gameEvent is PlayerSetupEvent playerSetupEvent)
            {
                foreach (var kv in playerSetupEvent.PlayerIdsByName)
                    result += $"Name <b>{kv.Key}</b> Id <b>{kv.Value}</b><br>";
            }
            else if (gameEvent is RequestStateEvent requestStateEvent)
            {
                result += $"Cities <b>{requestStateEvent.Cities}</b><br>" +
                    $"Held Cards {requestStateEvent.HeldCards}<br>" +
                    $"Held Cards by Type {GetFormattedCards(requestStateEvent.DevelopmentCardsByCount)}<br>" +
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
            else if (gameEvent is RoadSegmentPlacedEvent roadSegmentPlacedEvent)
            {
                result += $"Start: <b>{GetFormattedProperty(roadSegmentPlacedEvent.StartLocation)}</b> " +
                    $"End: <b>{roadSegmentPlacedEvent.EndLocation}</b>";
            }
            else if (gameEvent is RobbingChoicesEvent robbingChoicesEvent)
            {
                foreach (var kv in robbingChoicesEvent.RobbingChoices)
                    result += $"Name: <b>{GetPlayerName(kv.Key)}</b> Resource count: <b>{kv.Value}</b><br>";
            }
            else if (gameEvent is ScenarioRequestStateEvent scenarioRequestStateEvent)
            {
                result += $"Cities {GetFormattedProperty(scenarioRequestStateEvent.Cities)}<br>" +
                    $"Held Cards {GetFormattedProperty(scenarioRequestStateEvent.HeldCards)}<br>" +
                    $"Held Cards by Type {GetFormattedCards(scenarioRequestStateEvent.DevelopmentCardsByCount)}<br>" + 
                    $"Played Knight Cards {GetFormattedProperty(scenarioRequestStateEvent.PlayedKnightCards)}<br>" +
                    $"Resources {GetFormattedProperty(scenarioRequestStateEvent.Resources)}<br>" +
                    $"Road Segments {GetFormattedProperty(scenarioRequestStateEvent.RoadSegments)}<br>" +
                    $"Settlements {GetFormattedProperty(scenarioRequestStateEvent.Settlements)}<br>" +
                    $"Victory Points {GetFormattedProperty(scenarioRequestStateEvent.VictoryPoints)}";
            }
            else if (gameEvent is YearOfPlentyCardPlayedEvent yearOfPlentyCardPlayedEvent)
            {
                result += $"First Resource <b>{yearOfPlentyCardPlayedEvent.FirstResource}</b><br />" +
                    $"Second Resource <b>{yearOfPlentyCardPlayedEvent.SecondResource}</b>";
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
        #endregion

        #region Classes
        private interface ILogEvent
        {
            string ToHtml();
        }

        private class ActionEvent : ILogEvent
        {
            private readonly ActionInstruction playerAction;
            public ActionEvent(ActionInstruction playerAction) => this.playerAction = playerAction;

            public string ToHtml()
            {
                var message = $"<tr>" +
                    "<td /><td class=\"action border\">" +
                    $"<span>{this.playerAction.Operation} operation</span>";

                if (this.playerAction.Parameters != null)
                {
                    message += "<br><div>Parameters: ";
                    foreach(var parameter in this.playerAction.Parameters)
                        message += $"{parameter.ToString()}, ";

                    message = message.Substring(0, message.Length - 2) + "</div>";
                }
                    
                return message + $"</td></tr>";
            }
        }

        private class ExceptionEvent : ILogEvent
        {
            private readonly Exception exception;
            public ExceptionEvent(Exception exception) => this.exception = exception;

            public string ToHtml()
            {
                return $"<tr class=\"exception border\"><td colspan=\"2\">{this.exception.Message}</td></tr>";
            }
        }

        private class MatchedExpectedEvent : ILogEvent
        {
            private readonly GameEvent actualEvent, expectedEvent;
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
            private readonly string note;
            public NoteEvent(string note) => this.note = note;

            public string ToHtml()
            {
                return $"<tr class=\"note\"><td class=\"border\"><div>{this.note}</div></td></tr>";
            }
        }

        private class ReceivedUnwantedEventExceptionEvent : ILogEvent
        {
            private readonly ReceivedUnwantedEventException receivedUnwantedEventException;
            public ReceivedUnwantedEventExceptionEvent(ReceivedUnwantedEventException receivedUnwantedEventException)
            {
                this.receivedUnwantedEventException = receivedUnwantedEventException;
            }

            public string ToHtml()
            {
                var eventProperties = this.receivedUnwantedEventException.UnwantedEvent != null ?
                    "<br><div>" + GetEventProperties(this.receivedUnwantedEventException.UnwantedEvent) + "</div>" : "";
                return $"<tr><td class=\"exception border\"><span>{this.receivedUnwantedEventException.Message}</span>" +
                    $"{eventProperties}</td></tr>";
            }
        }

        private class ActualEvent : ILogEvent
        {
            private readonly GameEvent actualEvent;
            public ActualEvent(GameEvent actualEvent) => this.actualEvent = actualEvent;

            public string ToHtml()
            {
                return $"<tr><td class=\"actual border\"><span>{this.actualEvent.SimpleTypeName}</span><br><div>{GetEventProperties(this.actualEvent)}</div></td></tr>";
            }
        }

        private class ExpectedEvent : ILogEvent
        {
            private readonly GameEvent expectedEvent;
            public ExpectedEvent(GameEvent expectedEvent) => this.expectedEvent = expectedEvent;

            public string ToHtml()
            {
                return $"<tr><td />" +
                    $"<td class=\"expected border\">" +
                    $"<img src=\"red_cross.png\" alt=\"\" height=\"20\" width=\"20\">" +
                    $"<span>{this.expectedEvent.SimpleTypeName}</span><br><div>{GetEventProperties(this.expectedEvent)}</div></td></tr>";
            }
        }
        #endregion
    }
}
