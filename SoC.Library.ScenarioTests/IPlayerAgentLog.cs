
using System;
using Jabberwocky.SoC.Library.GameEvents;
using Jabberwocky.SoC.Library.PlayerActions;

namespace SoC.Library.ScenarioTests
{
    public interface IPlayerAgentLog
    {
        void AddAction(PlayerAction playerAction);
        void AddActualEvent(GameEvent actualEvent);
        void AddException(Exception exception);
        void AddMatchedEvent(GameEvent actualEvent, GameEvent expectedEvent);
        void AddNote(string note);
        void AddUnmatchedExpectedEvent(GameEvent expectedEvent);
        void WriteToFile(string filePath);
    }
}
