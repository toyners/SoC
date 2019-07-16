
using System;
using Jabberwocky.SoC.Library.GameEvents;
using SoC.Library.ScenarioTests.Instructions;

namespace SoC.Library.ScenarioTests
{
    public interface IPlayerAgentLog
    {
        void AddAction(ActionInstruction action);
        void AddActualEvent(GameEvent actualEvent);
        void AddException(Exception exception);
        void AddMatchedEvent(GameEvent actualEvent, GameEvent expectedEvent);
        void AddNote(string note);
        void AddUnmatchedExpectedEvent(GameEvent expectedEvent);
        void WriteToFile(string filePath);
    }
}
