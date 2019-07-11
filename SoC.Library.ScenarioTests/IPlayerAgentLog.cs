
using System;
using System.Collections.Generic;
using Jabberwocky.SoC.Library.GameEvents;

namespace SoC.Library.ScenarioTests
{
    public interface IPlayerAgentLog
    {
        void AddActualEvent(GameEvent actualEvent);
        void AddMatchedEvent(GameEvent actualEvent, GameEvent expectedEvent);
        void AddUnmatchedExpectedEvent(GameEvent expectedEvent);
        void AddException(Exception exception);
        void AddNote(string note);
        void WriteToFile(string filePath);
    }
}
