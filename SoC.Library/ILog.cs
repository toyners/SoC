
using Jabberwocky.SoC.Library.GameEvents;

namespace Jabberwocky.SoC.Library
{
    public interface ILog
    {
        void Add(string message);
        void WriteToFile(string filePath);
    }

    public interface IActionLog
    {
        void AddActualEvent(GameEvent actualEvent);
        void AddExpectedEvent(GameEvent expectedEvent);
        void AddVerifiedEvent(GameEvent verifiedEvent);
    }
}
