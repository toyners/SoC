using Jabberwocky.SoC.Library;

namespace SoC.Library.ScenarioTests
{
    internal class MockTurnTimer : IGameTimer
    {
        public bool IsLate => false;

        public void Reset() { }
    }
}